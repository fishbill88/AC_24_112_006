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
using PX.Common;
using PX.Data;
using PX.TaxProvider;
using PX.Objects.Common.Extensions;
using PX.Objects.CS;

namespace PX.Objects.AR
{
	public class ARInvoiceEntryExternalTaxImportExternalZone : PXGraphExtension<ARInvoiceEntryExternalTax, ARInvoiceEntryExternalTaxImport, ARInvoiceEntry>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.avalaraTax>();

		[PXOverride]
		public virtual bool IsSkipExternalTaxCalcOnSave()
		{
			return Base2.skipExternalTaxCalcOnSave;
		}

		[PXOverride]
		public virtual void OnExternalTaxZone(ARInvoice invoice)
		{
			GetTaxResult result = new GetTaxResult();
			List<PX.TaxProvider.TaxLine> taxLines = new List<PX.TaxProvider.TaxLine>();
			List<PX.TaxProvider.TaxDetail> taxDetails = new List<PX.TaxProvider.TaxDetail>();
			decimal totalTaxAmount = 0m;
			Sign sign = Base2.GetDocumentSign(invoice);

			foreach (ARTaxTranImported taxTran in Base1.ImportedTaxes.Cache.Inserted)
			{
				decimal taxableAmount = sign * taxTran.CuryTaxableAmt ?? 0m;
				decimal taxAmount = sign * taxTran.CuryTaxAmt ?? 0m;
				decimal rate = !taxTran.TaxRate.IsNullOrZero() ? (taxTran.TaxRate ?? 0m) :
						(taxTran.CuryTaxableAmt.IsNullOrZero() ? 0m :
						Decimal.Round((taxTran.CuryTaxAmt ?? 0m) / (taxTran.CuryTaxableAmt ?? 1m), 6));

				PX.TaxProvider.TaxDetail taxDetail = new TaxProvider.TaxDetail
				{
					TaxName = taxTran.TaxID,
					TaxableAmount = taxableAmount,
					TaxAmount = taxAmount,
					Rate = rate
				};

				if (taxTran.LineNbr == 32000)
				{
					PX.TaxProvider.TaxLine taxLine = new TaxProvider.TaxLine
					{
						Index = short.MinValue,
						TaxableAmount = taxableAmount,
						TaxAmount = taxAmount,
						Rate = rate
					};
					taxLines.Add(taxLine);
				}

				totalTaxAmount += taxTran.CuryTaxAmt ?? 0m;

				taxDetails.Add(taxDetail);
			}
			result.TaxSummary = taxDetails.ToArray();
			result.TotalTaxAmount = sign * totalTaxAmount;

			Base1.ImportedTaxes.Cache.Clear();

			using (new PXTimeStampScope(null))
			{
				Base2.ApplyTax(invoice, result);
			}
		}
	}
}
