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
using System.Linq;

using PX.Data;
using PX.Objects.AR;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.AR.CCPaymentProcessing.Helpers;
using PX.Objects.AR.Standalone;
using PX.Objects.CC.Utility;
using PX.Objects.CS;
using PX.Objects.Extensions.PaymentTransaction;
using PX.Objects.SO;

namespace PX.Objects.CC.GraphExtensions
{
	public class ARCashSaleEntryLevel3 : Level3Graph<ARCashSaleEntry, ARCashSale, ARCashSaleEntryLevel3>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.acumaticaPayments>();
		}

		public PXAction<ARCashSale> updateL3Data;
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

			payment.L3Data.TransactionId = ExternalTranHelper.GetActiveTransaction(GetExtTrans())?.TranNumber ?? GetExtTrans().First().TranNumber;

			foreach (ARTran arTran in Base.Transactions.Select())
			{
				if (arTran.LineType != SOLineType.Discount)
				{
					TranProcessingL3DataLineItemInput newItem = new TranProcessingL3DataLineItemInput
					{
						Description = string.Format("{0} {1}", arTran.LineNbr, arTran.TranDesc),
						UnitCost = arTran.CuryUnitPrice ?? 0,
						Quantity = arTran.Qty,
						UnitCode = GetL3Code(arTran.UOM),
						DiscountAmount = arTran.CuryDiscAmt,
						DiscountRate = arTran.DiscPct,
						DebitCredit = "D",
					};
					Level3Helper.RetriveInventoryInfo(Base, arTran.InventoryID, newItem);

					payment.L3Data.LineItems.Add(newItem);
				}
			}
			if (payment.L3Data.LineItems.Count == 0)
			{
				return false;
			}
			ARCashSale arCashSale = ARCashSale.PK.Find(Base, payment.DocType, payment.RefNbr);
			Level3Helper.FillL3Header(Base, payment, arCashSale.CuryTaxTotal ?? 0);
			return true;
		}

		protected override PaymentMapping GetPaymentMapping()
		{
			return new PaymentMapping(typeof(ARCashSale));
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
