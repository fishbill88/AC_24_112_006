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
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.SO;
using System;

namespace PX.Objects.AR
{
	public class SOInvoiceEntryVATRecognitionOnPrepayments : PXGraphExtension<SOInvoiceEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.vATRecognitionOnPrepayments>();
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		protected virtual void _(Events.RowSelected<ARInvoice> e)
		{
			PXUIFieldAttribute.SetVisible<ARTran.sOOrderNbr>(Base.Transactions.Cache, null, true);
			PXUIFieldAttribute.SetVisible<ARTran.sOOrderType>(Base.Transactions.Cache, null, true);
		}

		protected virtual void _(Events.RowSelected<SOOrderShipment> eventArgs)
		{
			PXUIFieldAttribute.SetEnabled(eventArgs.Cache, eventArgs.Row, false);

			SOOrderShipment sOOrderShipment = eventArgs.Row as SOOrderShipment;
			if (sOOrderShipment != null)
			{
				SOAdjust pPIPendingPayment = PXSelectJoin<SOAdjust,
					InnerJoin<ARPayment, On<ARPayment.docType, Equal<SOAdjust.adjgDocType>,
						And<ARPayment.refNbr, Equal<SOAdjust.adjgRefNbr>>>>,
					Where<SOAdjust.adjdOrderType, Equal<Required<SOAdjust.adjdOrderType>>,
						And<SOAdjust.adjdOrderNbr, Equal<Required<SOAdjust.adjdOrderNbr>>,
						And<ARPayment.docType, Equal<ARDocType.prepaymentInvoice>,
						And<ARPayment.status, Equal<ARDocStatus.pendingPayment>,
						And<SOAdjust.curyAdjdAmt, NotEqual<decimal0>>>>>>>
					.SelectSingleBound(Base, null, new[] { sOOrderShipment.OrderType, sOOrderShipment.OrderNbr });
				if (pPIPendingPayment != null)
				{
					PXUIFieldAttribute.SetWarning<SOOrderShipment.orderNbr>(eventArgs.Cache, eventArgs.Row,
						PXMessages.LocalizeFormatNoPrefix(SO.Messages.CannotCreateInvoiceForSOWithUnpaidPPI, pPIPendingPayment.AdjgRefNbr));
					PXUIFieldAttribute.SetEnabled<SOOrderShipment.selected>(eventArgs.Cache, eventArgs.Row, false);
				}
				else
				{
					PXUIFieldAttribute.SetEnabled<SOOrderShipment.selected>(eventArgs.Cache, eventArgs.Row, true);
				}
			}
			else
			{
				PXUIFieldAttribute.SetEnabled<SOOrderShipment.selected>(eventArgs.Cache, eventArgs.Row, true);
			}
		}

		[Obsolete(Common.Messages.MethodIsObsoleteAndWillBeRemoved2025R1)]
		public virtual void InvoicePreProcessingValidations(InvoiceOrderArgs args)
		{
		}

		[PXOverride]
		public virtual void AfterInsertApplication(SOOrderShipment orderShipment)
		{
			if (Base.Document.Current.CuryUnpaidBalance > 0)
			{
				SOAdjust partiallyPaidPPI = SelectFrom<SOAdjust>
					.InnerJoin<ARRegister>
						.On<ARRegister.docType.IsEqual<SOAdjust.adjgDocType>
						.And<ARRegister.refNbr.IsEqual<SOAdjust.adjgRefNbr>>>
					.Where<SOAdjust.adjdOrderType.IsEqual<@P.AsString.ASCII>
						.And<SOAdjust.adjdOrderNbr.IsEqual<@P.AsString>
						.And<ARRegister.openDoc.IsEqual<True>
						.And<ARRegister.docType.IsEqual<ARDocType.prepaymentInvoice>
						.And<ARRegister.released.IsEqual<True>
						.And<ARRegister.pendingPayment.IsEqual<True>
						.And<ARRegister.voided.IsEqual<False>
						.And<ARRegister.curyOrigDocAmt.IsNotEqual<ARRegister.curyDocBal>
						.And<SOAdjust.curyAdjdAmt.IsGreater<decimal0>>
						>>>>>>>>
					.View.Select(Base, orderShipment.OrderType, orderShipment.OrderNbr);

				if (partiallyPaidPPI != null)
				{
					throw new PXException(SO.Messages.CannotCreateInvoiceForSOWithUnpaidPPI, partiallyPaidPPI.AdjgRefNbr);
				}
			}
		}
	}
}
