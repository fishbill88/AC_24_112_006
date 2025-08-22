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
using System.Collections.Generic;
using System.Linq;
using System.Net;

using PX.CCProcessingBase.Interfaces.V2;
using PX.Common;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.AR.CCPaymentProcessing.Helpers;
using PX.Objects.AR.CCPaymentProcessing.Repositories;
using PX.Objects.AR.CCPaymentProcessing.Wrappers;
using PX.Objects.CA;
using PX.Objects.CC.Utility;
using PX.Objects.Extensions.PaymentTransaction;

using CCTranStatus = PX.Objects.AR.CCPaymentProcessing.Common.CCTranStatus;
using V2 = PX.CCProcessingBase.Interfaces.V2;

namespace PX.Objects.CC.PaymentProcessing
{
	public class Level3Processing
	{
		private ICCPaymentProcessingRepository _repository;

		private Func<object, CCProcessingContext, ICardTransactionProcessingWrapper> _transactionProcessingWrapper;

		public Level3Processing() : this(CCPaymentProcessingRepository.GetCCPaymentProcessingRepository())
		{
		}

		public Level3Processing(ICCPaymentProcessingRepository repository)
		{
			_repository = repository; 
			_transactionProcessingWrapper = (plugin, context) => CardTransactionProcessingWrapper.GetTransactionProcessingWrapper(plugin, new CardProcessingReadersProvider(context));
		}

		public virtual TranOperationResult UpdateLevel3Data(Payment payment, int? transactionId)
		{
			V2.L3DataInput l3DataInput = CopyTranProcessingToL3DataInput(payment.L3Data);
			return this.UpdateL3Data(l3DataInput, transactionId, GetAndCheckProcessingCenterFromTransaction(payment.ProcessingCenterID, payment.PMInstanceID, payment.CuryID));
		}


		protected TranOperationResult UpdateL3Data(V2.L3DataInput l3DataInput, int? transactionId, CCProcessingCenter processingCenter)
		{
			TranOperationResult ret = new TranOperationResult
			{
				TransactionId = transactionId,
			};
			TranProcessingResult result = new TranProcessingResult();
			bool hasError = false;
			CCProcessingContext context = new CCProcessingContext()
			{
				callerGraph = _repository.Graph,
				processingCenter = processingCenter,
			};
			ICardProcessingReadersProvider provider = new CardProcessingReadersProvider(context);
			ICCLevel3Processor l3Processor = CreateLevel3Processor(processingCenter, provider);

			V2.CCL3ProcessingException l3ProcessingException = null;

			try
			{
				l3Processor.CreateOrUpdateLevel3Data(l3DataInput);
			}
			catch (V2.CCL3ProcessingException procException)
			{
				hasError = true;
				l3ProcessingException = procException;

				switch (procException.Reason)
				{
					case V2.CCL3ProcessingException.ExceptionReason.Failed:
						result.L3Status = V2Converter.GetL3TranStatus(ExtTransactionL3StatusCode.Failed);
						result.L3Error = procException.Message;
						break;
					case V2.CCL3ProcessingException.ExceptionReason.Rejected:
						result.L3Status = V2Converter.GetL3TranStatus(ExtTransactionL3StatusCode.Rejected);
						break;
					case V2.CCL3ProcessingException.ExceptionReason.ResendRejected:
						result.L3Status = V2Converter.GetL3TranStatus(ExtTransactionL3StatusCode.ResendRejected);
						result.L3Error = procException.Message;
						break;
					case V2.CCL3ProcessingException.ExceptionReason.ResendFailed:
						result.L3Status = V2Converter.GetL3TranStatus(ExtTransactionL3StatusCode.ResendFailed);
						result.L3Error = procException.Message;
						break;
				}

				PXTrace.WriteInformation($"CCPaymentProcessing.UpdateL3Data.V2.CCL3ProcessingException. ErrorSource:{result.ErrorSource}; ErrorText:{result.ErrorText}");
			}
			catch (WebException webExn)
			{
				hasError = true;
				result.L3Status = V2Converter.GetL3TranStatus(ExtTransactionL3StatusCode.Failed);
				result.L3Error = webExn.Message;
				PXTrace.WriteInformation($"CCPaymentProcessing.UpdateL3Data.WebException. ErrorSource:{result.ErrorSource}; ErrorText:{result.ErrorText}");
			}
			catch (Exception exn)
			{
				hasError = true;
				result.L3Status = V2Converter.GetL3TranStatus(ExtTransactionL3StatusCode.Failed);
				result.L3Error = exn.Message;
				throw new PXException(AR.Messages.ERR_CCPaymentProcessingInternalError, l3DataInput.TransactionId, exn.Message);
			}
			finally
			{
				transactionId = _repository.GetCCProcTranByTranID(transactionId).First().TranNbr;
				result.Level3Support = true;
				if (!hasError)
				{
					result.L3Status = V2Converter.GetL3TranStatus(ExtTransactionL3StatusCode.Sent);
					result.L3Error = null;
				}
				ret.Success = true;
				result.TranStatus = CCTranStatus.Approved;
				UpdateExternalTransaction(transactionId.Value, result, CCProcStatus.Finalized);
				if (hasError && l3ProcessingException != null)
				{
					throw l3ProcessingException;
				}
			}
			return ret;
		}

		private ICCLevel3Processor CreateLevel3Processor(CCProcessingCenter processingCenter, ICardProcessingReadersProvider provider)
		{
			if (processingCenter == null)
			{
				throw new PXException(AR.Messages.ERR_CCProcessingCenterNotFound);
			}
			var plugin = CreatePlugin(processingCenter);
			return GetLevel3Processor(plugin, provider);
		}

		private object CreatePlugin(CCProcessingCenter processingCenter)
		{
			object plugin;
			try
			{
				plugin = CCPluginTypeHelper.CreatePluginInstance(processingCenter);
			}
			catch (PXException)
			{
				throw;
			}
			catch
			{
				throw new PXException(AR.Messages.ERR_ProcessingCenterTypeInstanceCreationFailed,
					processingCenter.ProcessingTypeName,
					processingCenter.ProcessingCenterID);
			}
			return plugin;
		}

		private ICCLevel3Processor GetLevel3Processor(object pluginObject, ICardProcessingReadersProvider provider)
		{
			var v2Interface = CCProcessingHelper.IsV2ProcessingInterface(pluginObject);

			V2SettingsGenerator seetingsGen = new V2SettingsGenerator(provider);
			ICCLevel3Processor processor = v2Interface.CreateProcessor<ICCLevel3Processor>(seetingsGen.GetSettings());
			if (processor == null)
			{
				CreateAndThrowException(CCProcessingFeature.Level3);
			}
			return processor;
		}

		private void CreateAndThrowException(CCProcessingFeature feature)
		{
			string errorMessage = PXMessages.LocalizeFormatNoPrefixNLA(AR.Messages.FeatureNotSupportedByProcessing, feature);
			throw new PXException(errorMessage);
		}

		private V2.L3DataInput CopyTranProcessingToL3DataInput(TranProcessingL3DataInput input)
		{
			if (input == null)
				return null;
			return new V2.L3DataInput
			{
				CustomerVatRegistration = input.CustomerVatRegistration,
				DestinationCountryCode = input.DestinationCountryCode,
				DutyAmount = input.DutyAmount,
				FreightAmount = input.FreightAmount,
				LineItems = CopyTranProcessingLineItemsToL3DataInputLineItems(input.LineItems),
				MerchantVatRegistration = input.MerchantVatRegistration,
				NationalTax = input.NationalTax,
				OrderDate = input.OrderDate,
				SalesTax = input.SalesTax,
				ShipfromZipCode = input.ShipfromZipCode,
				ShiptoZipCode = input.ShiptoZipCode,
				SummaryCommodityCode = input.SummaryCommodityCode,
				TaxAmount = input.TaxAmount,
				TaxExempt = input.TaxExempt,
				TaxRate = input.TaxRate,
				TransactionId = input.TransactionId,
				UniqueVatRefNumber = input.UniqueVatRefNumber,
				CardType = V2Converter.ConvertCardType(input.CardType),
			};
		}

		private List<V2.L3DataLineItemInput> CopyTranProcessingLineItemsToL3DataInputLineItems(List<TranProcessingL3DataLineItemInput> inputLineItems)
		{
			if (inputLineItems == null)
				return null;
			return inputLineItems.Select(lineItem => new V2.L3DataLineItemInput
			{
				AlternateTaxId = lineItem.AlternateTaxId,
				CommodityCode = lineItem.CommodityCode,
				DebitCredit = lineItem.DebitCredit,
				Description = lineItem.Description,
				DiscountAmount = lineItem.DiscountAmount,
				DiscountRate = lineItem.DiscountRate,
				TaxAmount = lineItem.TaxAmount,
				OtherTaxAmount = lineItem.OtherTaxAmount,
				ProductCode = lineItem.ProductCode,
				Quantity = lineItem.Quantity,
				TaxRate = lineItem.TaxRate,
				TaxTypeApplied = lineItem.TaxTypeApplied,
				TaxTypeId = lineItem.TaxTypeId,
				UnitCode = lineItem.UnitCode,
				UnitCost = lineItem.UnitCost,
			}).ToList();
		}

		public virtual CCProcessingCenter GetAndCheckProcessingCenterFromTransaction(string processingCenterID, int? pMInstanceID, string curyID)
		{
			CCProcessingCenter procCenter;
			if (!string.IsNullOrEmpty(processingCenterID))
			{
				procCenter = CCProcessingCenter.PK.Find(_repository.Graph, processingCenterID);
			}
			else
			{
				procCenter = _repository.FindProcessingCenter(pMInstanceID, curyID);
			}

			CheckProcessingCenter(procCenter);

			if (procCenter == null || string.IsNullOrEmpty(procCenter.ProcessingTypeName))
			{
				throw new PXException(AR.Messages.ERR_ProcessingCenterForCardNotConfigured);
			}

			return procCenter;
		}

		private void CheckProcessingCenter(CCProcessingCenter procCenter)
		{
			CCPluginTypeHelper.GetPluginTypeWithCheck(procCenter);
		}

		protected ICardTransactionProcessingWrapper GetProcessingWrapper(CCProcessingContext context)
		{
			object processor = GetProcessor(context.processingCenter);
			return _transactionProcessingWrapper(processor, context);
		}

		protected object GetProcessor(CCProcessingCenter processingCenter)
		{
			if (processingCenter == null)
			{
				throw new PXException(AR.Messages.ERR_CCProcessingCenterNotFound);
			}

			object processor = null;

			try
			{
				processor = CCPluginTypeHelper.CreatePluginInstance(processingCenter);

			}
			catch (PXException)
			{
				throw;
			}
			catch
			{
				throw new PXException(AR.Messages.ERR_ProcessingCenterTypeInstanceCreationFailed,
					processingCenter.ProcessingTypeName,
					processingCenter.ProcessingCenterID);
			}
			return processor;
		}

		protected virtual void UpdateExternalTransaction(int aTranID, TranProcessingResult aRes, string aProcStatus)
		{
			CCProcTran tran = CCProcTran.PK.Find(_repository.Graph, aTranID);
			ExternalTransaction extTran = ExternalTransaction.PK.Find(_repository.Graph, tran.TransactionID);
			Level3Helper.SetL3StatusExternalTransaction(extTran, aRes.L3Status, aRes.L3Error);
			_repository.UpdateExternalTransaction(extTran);
			_repository.Save();
		}
	}
}
