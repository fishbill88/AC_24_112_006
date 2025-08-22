/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using PX.Data;
using PX.Objects.CA;
using PX.Objects.AR.CCPaymentProcessing.Repositories;
using PX.Objects.AR.CCPaymentProcessing.Helpers;
using PX.Objects.AR.GraphExtensions;
using PX.Objects.Extensions.PaymentTransaction;
using PX.CCProcessingBase.Interfaces.V2;
using PX.Objects.Common.Abstractions;

namespace PX.Objects.AR.CCPaymentProcessing
{
	public class PaymentDocCreator
	{
		public class InputParams
		{
			public string DocType { get; set; } = ARDocType.Payment;
			public string TransactionID { get; set; }
			public int? Customer { get; set; }
			public int? CashAccountID { get; set; }
			public int? PMInstanceID { get; set; }
			public string PaymentMethodID { get; set; }
			public string ProcessingCenterID { get; set; }
		}

		protected ICCPaymentProcessingRepository repo;
		protected CCPaymentProcessing paymentProc;

		public PaymentDocCreator()
		{
			PXGraph graph = PXGraph.CreateInstance<CCPaymentHelperGraph>();
			repo = new CCPaymentProcessingRepository(graph);
			paymentProc = new CCPaymentProcessing(repo);
		}

		public PaymentDocCreator(PXGraph graph)
		{
			repo = new CCPaymentProcessingRepository(graph);
			paymentProc = new CCPaymentProcessing(repo);
		}

		public virtual IDocumentKey CreateDoc(InputParams inputParams)
		{
			CheckInput(inputParams);
			TransactionData tranData = paymentProc.GetTransactionById(inputParams.TransactionID, inputParams.ProcessingCenterID);
			ExecValidations(tranData, inputParams);
			IDocumentKey ret = SaveDocWithTransaction(tranData, inputParams);
			return ret;
		}

		protected virtual void CheckInput(InputParams inputParams)
		{
			if (inputParams.DocType != ARDocType.Payment && inputParams.DocType != ARDocType.Refund)
			{
				throw new PXException(Messages.DocTypeNotSupported);
			}
			if (string.IsNullOrEmpty(inputParams.TransactionID))
			{
				throw new PXException(ErrorMessages.FieldIsEmpty, nameof(inputParams.TransactionID));
			}
			if (inputParams.Customer == null)
			{
				throw new PXException(ErrorMessages.FieldIsEmpty, nameof(inputParams.Customer));
			}
			if (string.IsNullOrEmpty(inputParams.PaymentMethodID))
			{
				throw new PXException(ErrorMessages.FieldIsEmpty, nameof(inputParams.PaymentMethodID));
			}
			if (string.IsNullOrEmpty(inputParams.ProcessingCenterID))
			{
				throw new PXException(ErrorMessages.FieldIsEmpty, nameof(inputParams.ProcessingCenterID));
			}
			if (inputParams.CashAccountID == null)
			{
				throw new PXException(ErrorMessages.FieldIsEmpty, nameof(inputParams.CashAccountID));
			}
		}

		protected virtual void ExecValidations(TransactionData tranData, InputParams inputParams)
		{
			TranValidationHelper.CheckRecordedTranStatus(tranData);
			if (tranData.TranType == CCTranType.Void)
			{
				throw new PXException(Messages.ERR_IncorrectVoidTranType, tranData.TranID);
			}
			if (tranData.TranType == CCTranType.Credit && inputParams.DocType != ARDocType.Refund)
			{
				throw new PXException(Messages.ERR_IncorrectRefundTranType, tranData.TranID);
			}
			if (tranData.TranType != CCTranType.Credit && inputParams.DocType == ARDocType.Refund)
			{
				throw new PXException(Messages.ERR_IncorrectTranType, tranData.TranID);
			}

			var paymentMethod = repo.GetPaymentMethod(inputParams.PaymentMethodID);

			if (!((paymentMethod.PaymentType == PaymentMethodType.EFT && tranData.PaymentMethodType == MeansOfPayment.EFT) ||
				(paymentMethod.PaymentType == PaymentMethodType.CreditCard && (tranData.PaymentMethodType == MeansOfPayment.CreditCard || tranData.PaymentMethodType == null))))
			{
				throw new PXException(AR.Messages.ERR_IncorrectPaymentMethodType, paymentMethod.PaymentType, tranData.PaymentMethodType);
			}

			var prms = new TranValidationHelper.AdditionalParams();
			prms.PMInstanceId = inputParams.PMInstanceID;
			prms.ProcessingCenter = inputParams.ProcessingCenterID;
			prms.CustomerID = inputParams.Customer;
			prms.Repo = repo;
			TranValidationHelper.CheckPaymentProfile(tranData, prms);
			TranValidationHelper.CheckTranAlreadyRecorded(tranData, prms);
		}

		protected virtual IDocumentKey SaveDocWithTransaction(TransactionData tranData, InputParams inputParams)
		{
			ARPaymentEntry paymentGraph = PXGraph.CreateInstance<ARPaymentEntry>();
			ARSetup setup = paymentGraph.arsetup.Current;
			if (ResetHold(tranData))
			{
				setup.HoldEntry = false;
			}
			ARPayment payment = new ARPayment();
			payment.DocType = inputParams.DocType;
			payment = paymentGraph.Document.Insert(payment);

			payment.CustomerID = inputParams.Customer;
			payment.PaymentMethodID = inputParams.PaymentMethodID;
			payment.CuryOrigDocAmt = tranData.Amount;
			payment = paymentGraph.Document.Update(payment);

			if (inputParams.PMInstanceID > 0)
			{
				payment.PMInstanceID = inputParams.PMInstanceID;
			}
			else
			{
				payment.PMInstanceID = PaymentTranExtConstants.NewPaymentProfile;
				payment.ProcessingCenterID = inputParams.ProcessingCenterID;
			}

			payment.CashAccountID = inputParams.CashAccountID;
			payment = paymentGraph.Document.Update(payment);

			if (payment.DocType == ARDocType.Refund && tranData.RefTranID != null)
			{
				UpdateOrigTranNumber(paymentGraph, tranData, inputParams);
			}

			var extension = paymentGraph.GetExtension<ARPaymentEntryPaymentTransaction>();

			using (var txscope = new PXTransactionScope())
			{
				paymentGraph.Save.Press();
				CCPaymentEntry entry = new CCPaymentEntry(paymentGraph);
				extension.CheckSaveCardOption(tranData);
				extension.RecordTransaction(paymentGraph.Document.Current, tranData, entry);

				txscope.Complete();
			}
			return payment;
		}

		protected virtual void UpdateOrigTranNumber(ARPaymentEntry paymentGraph, TransactionData tranData, InputParams inputParams)
		{
			ARPayment payment = paymentGraph.Document.Current;
			payment.RefTranExtNbr = tranData.RefTranID;
			payment = paymentGraph.Document.Update(payment);

			bool needUpdate = false;
			if (payment.PMInstanceID == PaymentTranExtConstants.NewPaymentProfile)
			{
				if (inputParams.ProcessingCenterID != payment.ProcessingCenterID)
				{
					payment.ProcessingCenterID = inputParams.ProcessingCenterID;
					needUpdate = true;
				}
			}

			if (inputParams.CashAccountID != payment.CashAccountID)
			{
				payment.CashAccountID = inputParams.CashAccountID;
				needUpdate = true;
			}

			if (needUpdate)
			{
				paymentGraph.Document.Update(payment);
			}

		}

		protected virtual CustomerPaymentMethod GetCustomerPaymentMethod(PXGraph graph, int? pmInstanceId)
		{
			return CustomerPaymentMethod.PK.Find(graph, pmInstanceId);
		}

		private bool ResetHold(TransactionData tranData)
		{
			bool resetHold = false;
			var procStatus = CCProcessingHelper.GetProcessingStatusByTranData(tranData);
			if (procStatus == Common.ProcessingStatus.CaptureSuccess)
			{
				resetHold = true;
			}
			return resetHold;
		}
	}
}
