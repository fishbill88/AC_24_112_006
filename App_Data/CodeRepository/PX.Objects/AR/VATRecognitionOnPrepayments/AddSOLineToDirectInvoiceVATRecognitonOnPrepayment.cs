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
using PX.Objects.CS;
using PX.Objects.SO;
using PX.Objects.SO.DAC.Projections;
using PX.Objects.SO.GraphExtensions.SOInvoiceEntryExt;

namespace PX.Objects.AR.VATRecognitionOnPrepayments
{
	public class AddSOLineToDirectInvoiceVATRecognitonOnPrepayment : PXGraphExtension<AddSOLineToDirectInvoice, SOInvoiceEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.vATRecognitionOnPrepayments>()
				&& PXAccess.FeatureInstalled<FeaturesSet.advancedSOInvoices>();
		}

		protected virtual void _(Events.RowSelected<SOLineForDirectInvoice> eventArgs)
		{
			PXCache cache = eventArgs.Cache;
			SOLineForDirectInvoice row = eventArgs.Row;

			SOLineForDirectInvoice sOLineForDirectInvoice = row as SOLineForDirectInvoice;
			if (sOLineForDirectInvoice != null)
			{
				SOAdjust pPIPendingPayment = PXSelectJoin<SOAdjust,
					InnerJoin<ARPayment, On<ARPayment.docType, Equal<SOAdjust.adjgDocType>,
						And<ARPayment.refNbr, Equal<SOAdjust.adjgRefNbr>>>>,
					Where<SOAdjust.adjdOrderType, Equal<Required<SOAdjust.adjdOrderType>>,
						And<SOAdjust.adjdOrderNbr, Equal<Required<SOAdjust.adjdOrderNbr>>,
						And<ARPayment.docType, Equal<ARDocType.prepaymentInvoice>,
						And<ARPayment.status, Equal<ARDocStatus.pendingPayment>,
						And<SOAdjust.curyAdjdAmt, NotEqual<decimal0>>>>>>>
					.SelectSingleBound(Base, null, new[] { sOLineForDirectInvoice.OrderType, sOLineForDirectInvoice.OrderNbr });
				if (pPIPendingPayment != null)
				{
					PXUIFieldAttribute.SetWarning<SOLineForDirectInvoice.orderNbr>(cache, row,
						PXMessages.LocalizeFormatNoPrefix(SO.Messages.CannotCreateInvoiceForSOWithUnpaidPPI, pPIPendingPayment.AdjgRefNbr));
					PXUIFieldAttribute.SetEnabled<SOLineForDirectInvoice.selected>(cache, row, false);
				}
				else
				{
					PXUIFieldAttribute.SetEnabled<SOLineForDirectInvoice.selected>(cache, row, true);
				}
			}
			else
			{
				PXUIFieldAttribute.SetEnabled<SOLineForDirectInvoice.selected>(cache, row, true);
			}
		}
	}
}
