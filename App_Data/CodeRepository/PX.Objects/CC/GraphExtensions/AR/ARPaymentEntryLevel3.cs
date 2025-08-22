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

using System.Collections;
using System.Collections.Generic;

using PX.Common;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.CC.Utility;
using PX.Objects.CM.Extensions;
using PX.Objects.CM.TemporaryHelpers;
using PX.Objects.Common.Extensions;
using PX.Objects.CS;
using PX.Objects.Extensions.PaymentTransaction;
using PX.Objects.SO;

using ARAdjust = PX.Objects.AR.ARAdjust;
using ARInvoice = PX.Objects.AR.ARInvoice;
using ARPayment = PX.Objects.AR.ARPayment;

namespace PX.Objects.CC.GraphExtensions
{
	public class ARPaymentEntryLevel3 : Level3Graph<ARPaymentEntry, ARPayment, ARPaymentEntryLevel3>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.acumaticaPayments>();
		}

		public PXAction<ARPayment> updateL3Data;
		[PXUIField(DisplayName = "Send Level 3 Data", Visible = false, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable UpdateL3Data(PXAdapter adapter)
		{
			if (Base.Document.Current != null &&
			Base.Document.Current.IsCCPayment == true)
			{
				if (CollectL3Data(PaymentDoc.Current))
				{
					return base.UpdateLevel3DataCCPayment(adapter);
				}
			}
			return adapter.Get();
		}

		public virtual bool CollectL3Data(Payment payment)
		{
			payment.L3Data = new TranProcessingL3DataInput
			{
				LineItems = new List<TranProcessingL3DataLineItemInput>(),
			};

			decimal paymentTax = 0;
			decimal freighAmount = 0;

			string currentDocRefNbr = string.Empty;

			// AR section
			PXSelectJoin<ARAdjust,
				InnerJoin<ARInvoice,
					On<ARInvoice.docType, Equal<ARAdjust.adjdDocType>,
					And<ARInvoice.refNbr, Equal<ARAdjust.adjdRefNbr>>>,
				InnerJoin<ARTran,
					On<ARTran.tranType, Equal<ARInvoice.docType>,
					And<ARTran.refNbr, Equal<ARInvoice.refNbr>,
					And<Where<ARTran.lineType, IsNull, Or<ARTran.lineType, NotEqual<SOLineType.discount>>>>>>>>,
				Where<ARAdjust.adjgDocType, Equal<Required<ARPayment.docType>>,
					And<ARAdjust.adjgRefNbr, Equal<Required<ARPayment.refNbr>>,
					And<ARAdjust.adjdDocType, In3<ARDocType.invoice, ARDocType.finCharge, ARDocType.debitMemo, ARDocType.creditMemo>,
					And<ARAdjust.voided, NotEqual<True>,
					And<ARInvoice.paymentsByLinesAllowed, NotEqual<True>,
					And<ARTran.curyTranAmt, Greater<decimal0>,
					And<ARTran.inventoryID, IsNotNull>>>>>>>>
				.Select(Base, payment.DocType, payment.RefNbr)
				.ForEach(arApplication =>
				{
					ARAdjust arAdjust = PXResult.Unwrap<ARAdjust>(arApplication);
					ARInvoice invoice = PXResult.Unwrap<ARInvoice>(arApplication);
					ARTran arTran = PXResult.Unwrap<ARTran>(arApplication);

					decimal adjdCuryRate = arAdjust.AdjdCuryRate ?? 1;

					if (currentDocRefNbr != invoice.RefNbr)
					{
						freighAmount += (invoice.CuryFreightTot ?? 0) * adjdCuryRate;
						paymentTax += (invoice.CuryTaxTotal ?? 0) * adjdCuryRate;
						currentDocRefNbr = invoice.RefNbr;
					}

					TranProcessingL3DataLineItemInput newItem = new TranProcessingL3DataLineItemInput
					{
						Description = string.Format("{0} {1}", arTran.LineNbr, arTran.TranDesc),
						UnitCost = (arTran.CuryUnitPrice ?? 0) * adjdCuryRate,
						Quantity = arTran.Qty,
						UnitCode = GetL3Code(arTran.UOM),
						DiscountAmount = arTran.CuryDiscAmt * adjdCuryRate,
						DiscountRate = arTran.DiscPct,
						DebitCredit = "D",
					};
					Level3Helper.RetriveInventoryInfo(Base, arTran.InventoryID, newItem);					

					payment.L3Data.LineItems.Add(newItem);
				});

			// SO section
			PXSelectJoin<SOAdjust,
				InnerJoin<SOOrder,
					On<SOAdjust.FK.Order>,
				InnerJoin<SOLine,
					On<SOLine.orderType, Equal<SOOrder.orderType>,
					And<SOLine.orderNbr, Equal<SOOrder.orderNbr>>>>>,
				Where<SOAdjust.adjgDocType, Equal<Required<ARPayment.docType>>,
					And<SOAdjust.adjgRefNbr, Equal<Required<ARPayment.refNbr>>,
					And<SOAdjust.voided, NotEqual<True>,
					And<SOLine.operation, Equal<SOOperation.issue>,
					And<SOOrder.hold, NotEqual<True>>>>>>>
				.Select(Base, payment.DocType, payment.RefNbr)
				.ForEach(soApplication =>
				{
					SOAdjust soAdjust = PXResult.Unwrap<SOAdjust>(soApplication);
					SOOrder order = PXResult.Unwrap<SOOrder>(soApplication);
					SOLine soLine = PXResult.Unwrap<SOLine>(soApplication);

					CurrencyInfo adjdCurrencyInfo = MultiCurrencyCalculator.GetCurrencyInfo<SOAdjust.adjdOrigCuryInfoID>(Base, soAdjust);
					CurrencyInfo adjgCurrencyInfo = MultiCurrencyCalculator.GetCurrencyInfo<SOAdjust.adjgCuryInfoID>(Base, soAdjust);
					decimal crossRate = (adjdCurrencyInfo.CuryRate ?? 1) / (adjgCurrencyInfo.CuryRate ?? 1);

					if (currentDocRefNbr != order.OrderNbr)
					{
						freighAmount += (order.CuryFreightTot ?? 0) * crossRate;
						paymentTax += (order.CuryTaxTotal ?? 0) * crossRate;
						currentDocRefNbr = order.OrderNbr;
					}					

					TranProcessingL3DataLineItemInput newItem = new TranProcessingL3DataLineItemInput
					{
						Description = string.Format("{0} {1}", soLine.LineNbr, soLine.TranDesc),
						UnitCost = (soLine.CuryUnitPrice ?? 0) * crossRate,
						Quantity = soLine.Qty,
						UnitCode = GetL3Code(soLine.UOM),
						DiscountAmount = soLine.CuryDiscAmt * crossRate,
						DiscountRate = soLine.DiscPct,
						DebitCredit = "D",
					};
					Level3Helper.RetriveInventoryInfo(Base, soLine.InventoryID, newItem);

					payment.L3Data.LineItems.Add(newItem);
				});

			if (payment.L3Data.LineItems.Count == 0)
			{
				return false;
			}

			payment.L3Data.FreightAmount = freighAmount;
			Level3Helper.FillL3Header(Base, payment, paymentTax);
			return true;
		}

		protected override PaymentMapping GetPaymentMapping()
		{
			return new PaymentMapping(typeof(ARPayment));
		}
		protected override ExternalTransactionDetailMapping GetExternalTransactionMapping()
		{
			return new ExternalTransactionDetailMapping(typeof(ExternalTransaction));
		}

		public virtual string GetL3Code(string uomName)
		{
			switch (uomName?.Trim())
			{
				case "HOUR": return "HUR";
				case "KG": return "KGM";
				case "LITER": return "LTR";
				case "METER": return "MTR";
				case "MINUTE": return "MIN";
				case "PACK": return "NMP";
				case "PIECE": return "PCB";
				default: return "EA ";
			}
		}
	}
}
