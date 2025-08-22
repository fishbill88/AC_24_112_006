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
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.CR;

namespace PX.Objects.Localizations.CA.AP
{
	public class APPaymentEntryExt : PXGraphExtension<APPaymentEntry>
	{
		#region IsActive

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.canadianLocalization>();
		}

		#endregion

		#region

		protected virtual void APPayment_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			bool isPPM = Base.CurrentDocument.Current.DocType == APDocType.Prepayment;

			bool vendorT5018 = false;

			if (Base.CurrentDocument.Current.VendorID != null)
			{
				Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID,
					Equal<Required<Vendor.bAccountID>>>>.Select(Base, Base.CurrentDocument.Current.VendorID);

				if (vendor == null) return;

				T5018VendorExt extension = PXCache<BAccount>.GetExtension<T5018VendorExt>(vendor);

				if (extension.VendorT5018 == true)
				{
					vendorT5018 = true;
				}
			}

			bool appliedInFull = (decimal)Base.CurrentDocument.Current.CuryDocBal == 0 && (decimal)(Base.CurrentDocument.Current.CuryApplAmt ?? 0) != 0;

			if (LocalizationServiceExtensions.LocalizationEnabled<FeaturesSet.canadianLocalization>(this.Base) && vendorT5018 && isPPM && !appliedInFull)
			{
				PXUIFieldAttribute.SetVisible<APPaymentExt.includeInT5018Report>(Base.Document.Cache, null, true);
				PXUIFieldAttribute.SetEnabled<APPaymentExt.includeInT5018Report>(Base.Document.Cache, null, true);
				if (Base.CurrentDocument.AllowUpdate == false)
				{
					Base.CurrentDocument.Cache.AllowUpdate = true;
					PXUIFieldAttribute.SetEnabled(Base.CurrentDocument.Cache, null, false);
					PXUIFieldAttribute.SetEnabled(Base.CurrentDocument.Cache, nameof(APPaymentExt.includeInT5018Report), true);
				}
			}
			else
			{
				PXUIFieldAttribute.SetVisible<APPaymentExt.includeInT5018Report>(Base.Document.Cache, null, false);
				PXUIFieldAttribute.SetEnabled<APPaymentExt.includeInT5018Report>(Base.Document.Cache, null, false);
			}
		}
		#endregion
	}
}
