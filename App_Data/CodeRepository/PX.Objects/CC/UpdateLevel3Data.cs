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

using System;
using System.Collections;
using System.Collections.Generic;

using PX.CCProcessingBase.Interfaces.V2;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.AR.CCPaymentProcessing.Interfaces;
using PX.Objects.CC.GraphExtensions;
using PX.Objects.Common;
using PX.Objects.Common.Attributes;
using PX.Objects.CS;
using PX.Objects.SO;

using ARCashSale = PX.Objects.AR.Standalone.ARCashSale;

namespace PX.Objects.CC
{
	public class UpdateLevel3Data : PXGraph<UpdateLevel3Data>
	{
		#region Ctor

		public UpdateLevel3Data()
		{
			PXUIFieldAttribute.SetDisplayName<ExternalTransaction.docType>(Caches[typeof(ExternalTransaction)], "Type");
			PXUIFieldAttribute.SetDisplayName<ExternalTransaction.refNbr>(Caches[typeof(ExternalTransaction)], "Reference Nbr.");

			L3DocumentProcessingFilter filter = Filter.Current;
			if (filter == null) return;

			L3Payments.SetProcessDelegate(delegate (List<ARPayment> list)
			{
				UpdateLevel3Data graph = CreateInstance<UpdateLevel3Data>();
				UpdateL3Data(graph, list);
			});
		}

		#endregion

		#region Public members

		public PXCancel<L3DocumentProcessingFilter> Cancel;
		public PXFilter<L3DocumentProcessingFilter> Filter;

		public PXAction<L3DocumentProcessingFilter> ViewDocument;

		[PXUIField(DisplayName = "", Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable viewDocument(PXAdapter adapter)
		{
			ARPayment arPayment = this.L3Payments.Current;
			PXGraph target = CCTransactionsHistoryEnq.FindSourceDocumentGraph(arPayment.DocType, arPayment.RefNbr, null, null);
			if (target != null)
				throw new PXRedirectRequiredException(target, true, "") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			return Filter.Select();
		}

		[PXFilterable]
		public PXFilteredProcessingJoin<ARPayment, L3DocumentProcessingFilter,
			InnerJoin<Customer, On<ARPayment.FK.Customer>,
			InnerJoin<ExternalTransaction, On<ExternalTransaction.docType.IsEqual<ARPayment.docType>.And<ExternalTransaction.refNbr.IsEqual<ARPayment.refNbr>>>>>> L3Payments;

		public IEnumerable l3Payments()
		{
			L3DocumentProcessingFilter filter = Filter.Current;

			var l3Documents = new SelectFrom<ARPayment>
				.InnerJoin<Customer>.On<ARPayment.FK.Customer>
				.InnerJoin<ExternalTransaction>.On<ExternalTransaction.docType.IsEqual<ARPayment.docType>.And<ExternalTransaction.refNbr.IsEqual<ARPayment.refNbr>>>
				.LeftJoin<ARTranCashSale>.On<ARTranCashSale.tranType.IsEqual<ARPayment.docType>.And<ARTranCashSale.refNbr.IsEqual<ARPayment.refNbr>>>
				.LeftJoin<ARAdjust>.On<ARAdjust.adjgDocType.IsEqual<ARPayment.docType>.And<ARAdjust.adjgRefNbr.IsEqual<ARPayment.refNbr>>
					.And<Where<ARAdjust.adjdDocType.IsIn<ARDocType.invoice, ARDocType.finCharge, ARDocType.debitMemo, ARDocType.creditMemo>
						.And<ARAdjust.voided.IsNotEqual<True>>>>>
				.LeftJoin<ARInvoice>.On<ARInvoice.docType.IsEqual<ARAdjust.adjdDocType>.And<ARInvoice.refNbr.IsEqual<ARAdjust.adjdRefNbr>>
					.And<Where<ARInvoice.paymentsByLinesAllowed.IsNotEqual<True>>>>
				.LeftJoin<ARTran>.On<ARTran.tranType.IsEqual<ARInvoice.docType>.And<ARTran.refNbr.IsEqual<ARInvoice.refNbr>>
					.And<Where<Brackets<ARTran.lineType.IsNull
							.Or<ARTran.lineType.IsNotEqual<SOLineType.discount>>>
						.And<ARTran.curyTranAmt.IsGreater<decimal0>>
						.And<ARTran.inventoryID.IsNotNull>>>>
				.LeftJoin<SOAdjust>.On<SOAdjust.adjgDocType.IsEqual<ARPayment.docType>.And<SOAdjust.adjgRefNbr.IsEqual<ARPayment.refNbr>>
					.And<Where<SOAdjust.voided.IsNotEqual<True>>>>
				.LeftJoin<SOOrder>.On<SOOrder.orderType.IsEqual<SOAdjust.adjdOrderType>.And<SOOrder.orderNbr.IsEqual<SOAdjust.adjdOrderNbr>>
					.And<Where<SOOrder.hold.IsNotEqual<True>>>>
				.LeftJoin<SOLine>.On<SOLine.orderType.IsEqual<SOOrder.orderType>.And<SOLine.orderNbr.IsEqual<SOOrder.orderNbr>>
					.And<Where<SOLine.operation.IsEqual<SOOperation.issue>>>>
				.Where<ARPayment.docType.IsIn<ARDocType.payment, ARDocType.prepayment, ARDocType.cashSale>
					.And<ExternalTransaction.procStatus.IsEqual<ExtTransactionProcStatusCode.captureSuccess>>
					.And<ExternalTransaction.l3Status.IsEqual<@P.AsString>>
					.And<ExternalTransaction.settled.IsEqual<False>>
					.And<ExternalTransaction.processingCenterID.IsEqual<@P.AsString>>>
				.AggregateTo<GroupBy<ARPayment.documentKey>,
					Count<ARTranCashSale.refNbr>,
					Count<ARTran.refNbr>,
					Count<SOLine.orderNbr>>
				.Having<ARTranCashSale.refNbr.Counted.IsGreater<decimal0>
					.Or<ARTran.refNbr.Counted.IsGreater<decimal0>
					.Or<SOLine.orderNbr.Counted.IsGreater<decimal0>>>>
				.OrderBy<ARPayment.refNbr.Desc>
				.View(this);

			PXDelegateResult delegateResult = new PXDelegateResult
			{
				IsResultFiltered = true,
				IsResultSorted = true,
				IsResultTruncated = true
			};

			using (new PXFieldScope(l3Documents.View,
				typeof(ARPayment.docType),
				typeof(ARPayment.refNbr),
				typeof(ARPayment.docDate),
				typeof(ARPayment.finPeriodID),
				typeof(ARPayment.customerID),
				typeof(Customer.acctName),
				typeof(ARPayment.status),
				typeof(ARPayment.curyOrigDocAmt),
				typeof(ARPayment.curyID),
				typeof(ARPayment.processingCenterID),
				typeof(ARPayment.paymentMethodID),
				typeof(ExternalTransaction.l3Status),
				typeof(ExternalTransaction.l3Error)))
			{
				foreach (var item in l3Documents.SelectWithViewContext(filter.ProcessingStatus, filter.ProcessingCenterID))
				{
					delegateResult.Add(item);
				}
			}
			return delegateResult;
		}

		#endregion

		#region Internal types

		[PXHidden]
		public partial class L3DocumentProcessingFilter : PXBqlTable, IBqlTable
		{
			#region ProcessingCenterID
			public abstract class processingCenterID : Data.BQL.BqlString.Field<processingCenterID> { }

			[PXDBString(10, IsUnicode = true)]
			[CCProcessingCenterSelector(AR.CCPaymentProcessing.Common.CCProcessingFeature.Level3)]
			[PXUIField(DisplayName = "Processing Center", Visibility = PXUIVisibility.SelectorVisible)]
			[DeprecatedProcessing(ChckVal = DeprecatedProcessingAttribute.CheckVal.ProcessingCenterId)]
			[DisabledProcCenter(CheckFieldValue = DisabledProcCenterAttribute.CheckFieldVal.ProcessingCenterId)]
			public virtual string ProcessingCenterID { get; set; }
			#endregion

			#region ProcessingStatus
			public abstract class processingStatus : Data.BQL.BqlString.Field<processingStatus> { }

			[PXDBString(IsUnicode = false)]
			[ExtTransactionL3StatusCode.List()]
			[PXDefault(ExtTransactionL3StatusCode.Pending)]
			[PXUIField(DisplayName = "Processing Status")]
			public virtual string ProcessingStatus { get; set; }
			#endregion
		}

		[PXHidden]
		public class ARTranCashSale : ARTran
		{
			public new abstract class tranType : PX.Data.BQL.BqlString.Field<tranType> { }
			public new abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
		}

		[Obsolete]
		[PXHidden]
		public partial class ExternalTransactionFilter : PXBqlTable, IBqlTable
		{
			#region ProcessingCenterID
			public abstract class processingCenterID : Data.BQL.BqlString.Field<processingCenterID> { }

			public virtual string ProcessingCenterID { get; set; }
			#endregion

			#region ProcessingStatus
			public abstract class processingStatus : Data.BQL.BqlString.Field<processingStatus> { }

			public virtual string ProcessingStatus { get; set; }
			#endregion
		}

		#endregion

		#region Business logic

		public static void UpdateL3Data(PXGraph graph, List<ARPayment> list)
		{
			bool failed = false;
			ARPaymentEntry arPaymentGraph = null;
			ARCashSaleEntry arCashSaleGraph = null;
			string errorText = string.Empty;
			int i = 0;
			foreach (ARPayment doc in list)
			{
				try
				{
					switch (doc.DocType)
					{
						case ARDocType.CashSale:
							ARCashSale arCashSale = ARCashSale.PK.Find(graph, doc.DocType, doc.RefNbr);
							if (arCashSale == null) continue;
							arCashSaleGraph = arCashSaleGraph != null ? arCashSaleGraph : CreateInstance<ARCashSaleEntry>();
							ARCashSaleEntryLevel3 extCashSale = arCashSaleGraph.GetExtension<ARCashSaleEntryLevel3>();
							arCashSaleGraph.Document.Current = arCashSale;
							extCashSale.updateL3Data.Press();
							break;
						case ARDocType.Payment:
						case ARDocType.Prepayment:
							ARPayment arPayment = ARPayment.PK.Find(graph, doc.DocType, doc.RefNbr);
							if (arPayment == null) continue;
							arPaymentGraph = arPaymentGraph != null ? arPaymentGraph : CreateInstance<ARPaymentEntry>();
							ARPaymentEntryLevel3 extPayment = arPaymentGraph.GetExtension<ARPaymentEntryLevel3>();
							arPaymentGraph.Document.Current = arPayment;
							extPayment.updateL3Data.Press();
							break;
					}
					PXProcessing<ARPayment>.SetInfo(i, ActionsMessages.RecordProcessed);
				}
				catch (CCL3ProcessingException l3Exception)
				{
					failed = false;
					PXProcessing<ARPayment>.SetWarning(i, l3Exception);
				}
				catch (Exception e)
				{
					failed = true;
					errorText = e.Message;
					PXProcessing<ARPayment>.SetError(i, e);
				}
				i++;
			}
			if (failed)
			{
				throw new PXOperationCompletedWithErrorException(errorText);
			}
		}

		[Obsolete(InternalMessages.MethodIsObsoleteAndWillBeRemoved2024R2)]
		public static void UpdateL3Data(PXGraph graph, List<IExternalTransaction> externalTransactions)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
