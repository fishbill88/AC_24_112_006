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

using PX.ACHPlugInBase;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.AP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;

namespace PX.Objects.CA
{
	public static class ACHPlugInHelper
	{
		public static IACHPlugIn GetPlugIn(this PaymentMethod pt)
		{
			Type pluginType = PXBuildManager.GetType(pt.APBatchExportPlugInTypeName, true);
			IACHPlugIn plugin;

			if (typeof(PXGraph).IsAssignableFrom(pluginType))
			{
				plugin = PXGraph.CreateInstance(pluginType) as PX.ACHPlugInBase.IACHPlugIn;
			}
			else
			{
				plugin = Activator.CreateInstance(pluginType) as PX.ACHPlugInBase.IACHPlugIn;
			}

			return plugin;
		}

		public static void RunWithVerifications(this IACHPlugIn plugin, Action action, CABatchEntry graph, CABatch document)
			=> plugin.RunWithVerifications(action, graph, document, Messages.SomeVendorPaymentMethodSettingAreInvalidOrEmpty, Messages.SomePaymentsCannotBeExported);

		public static void RunWithVerifications(this IACHPlugIn plugin, Action action, CABatchEntry graph, CABatch document, string errorVendorPaymentMethodSettingAreInvalidOrEmpty, string errorSomePaymentsCannotBeExported)

		{
			try
			{
				plugin.VerifySettings(graph, document.BatchNbr);

				// Acuminator disable once PX1008 LongOperationDelegateSynchronousExecution [due to the Export() doesn't causes any updates of caches and uses the graph as data provider for selects]
				PXLongOperation.StartOperation(graph, delegate ()
				{
					action();
				});
			}
			catch (InvalidMappingSettingException e)
			{
				graph.Document.Cache.RaiseExceptionHandling<CABatch.paymentMethodID>(graph.Document.Current, graph.Document.Current.PaymentMethodID,
					new PXSetPropertyException(e.Message, PXErrorLevel.Error));
				throw;
			}
			catch (InvalidRemittanceSettingException e)
			{
				var ca = graph.cashAccount.SelectSingle();
				graph.Document.Cache.RaiseExceptionHandling<CABatch.cashAccountID>(graph.Document.Current, ca?.CashAccountCD,
					new PXSetPropertyException(e.Message, PXErrorLevel.Error));
				throw;
			}
			catch (InvalidPaymentInstructionsException e)
			{
				foreach (var payment in e.IssuedPayments)
				{
					CABatchDetail detail = PXSelect<CABatchDetail, Where<CABatchDetail.batchNbr, Equal<Current<CABatch.batchNbr>>,
						And<CABatchDetail.origModule, Equal<BatchModule.moduleAP>,
						And<CABatchDetail.origDocType, Equal<Required<APPayment.docType>>,
						And<CABatchDetail.origRefNbr, Equal<Required<APPayment.refNbr>>>>>>>.Select(graph, payment.DocType, payment.RefNbr);

					graph.BatchPayments.Cache.RaiseExceptionHandling<CABatchDetail.origRefNbr>(detail, detail.OrigRefNbr,
						new PXSetPropertyException(e.GetErrorFor(payment), PXErrorLevel.RowError));
				}
				throw new PXException(PXLocalizer.LocalizeFormat(errorVendorPaymentMethodSettingAreInvalidOrEmpty, graph.Document.Current?.PaymentMethodID));
			}
			catch (InvalidPaymentsDataException e)
			{
				foreach (var payment in e.IssuedPayments)
				{
					CABatchDetail detail = PXSelect<CABatchDetail, Where<CABatchDetail.batchNbr, Equal<Current<CABatch.batchNbr>>,
						And<CABatchDetail.origModule, Equal<BatchModule.moduleAP>,
						And<CABatchDetail.origDocType, Equal<Required<APPayment.docType>>,
						And<CABatchDetail.origRefNbr, Equal<Required<APPayment.refNbr>>>>>>>.Select(graph, payment.DocType, payment.RefNbr);

					graph.BatchPayments.Cache.RaiseExceptionHandling<CABatchDetail.origRefNbr>(detail, detail.OrigRefNbr,
						new PXSetPropertyException(e.GetErrorFor(payment), PXErrorLevel.RowError));
				}
				throw new PXException(PXLocalizer.LocalizeFormat(errorSomePaymentsCannotBeExported));
			}
		}
	}
}
