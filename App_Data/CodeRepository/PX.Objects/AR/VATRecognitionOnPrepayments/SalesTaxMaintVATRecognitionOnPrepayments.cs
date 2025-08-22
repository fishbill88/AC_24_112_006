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
using System;

namespace PX.Objects.TX
{
	public class SalesTaxMaintVATRecognitionOnPrepayments : PXGraphExtension<SalesTaxMaint>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.vATRecognitionOnPrepayments>();
		}

		[Obsolete(Common.Messages.MethodIsObsoleteAndWillBeRemoved2025R1)]
		[PXOverride]
		public virtual void SetPendingGLAccountsUI(PXCache cache, Tax tax)
		{
		}

		[Obsolete(Common.Messages.MethodIsObsoleteAndWillBeRemoved2025R1)]
		public delegate void ResetPendingSalesTaxDelegate(PXCache cache, Tax newTax);

		[Obsolete(Common.Messages.MethodIsObsoleteAndWillBeRemoved2025R1)]
		[PXOverride]
		public virtual void ResetPendingSalesTax(PXCache cache, Tax newTax, ResetPendingSalesTaxDelegate baseMethod)
		{
			baseMethod(cache, newTax);
		}

		[PXOverride]
		public virtual void SetGLAccounts(PXCache cache, Tax tax)
		{
			SetOnARPrepaymentAccountsUI(cache, tax);
		}

		protected virtual void SetOnARPrepaymentAccountsUI(PXCache cache, Tax tax)
		{
			bool isVAT = tax.TaxType == CSTaxType.VAT;

			cache.Adjust<PXUIFieldAttribute>(tax)
				 .For<Tax.onARPrepaymentTaxAcctID>(a => a.Visible = a.Enabled = isVAT)
				 .SameFor<Tax.onARPrepaymentTaxSubID>();
		}

		public virtual void Tax_TaxVendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (!(e.Row is Tax tax))
				return;

			sender.SetDefaultExt<Tax.onARPrepaymentTaxAcctID>(tax);
			sender.SetDefaultExt<Tax.onARPrepaymentTaxAcctID>(tax);
		}
	}
}
