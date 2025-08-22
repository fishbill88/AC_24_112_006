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
using PX.Objects.IN;
using PX.Objects.Localizations.CA.AP.DAC;
using PX.Objects.Localizations.CA.IN.DAC;
using PX.Data.BQL.Fluent;
using System.Collections.Generic;

namespace PX.Objects.Localizations.CA.AP
{
	public class APInvoiceEntryExt : PXGraphExtension<APInvoiceEntry>
	{
		#region IsActive

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.canadianLocalization>();
		}

		#endregion

		#region APInvoice Events
		protected virtual void APInvoice_BranchID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APInvoice row = e.Row as APInvoice;
			sender.SetValueExt<APInvoice.termsID>(row, row.TermsID);
		}

		protected virtual void APTran_T5018Service_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			APTran apTran = (APTran)e.Row;
			if (apTran == null) return;

			if (LocalizationServiceExtensions.LocalizationEnabled<FeaturesSet.canadianLocalization>(this.Base))
			{
				if (apTran.InventoryID == null)
				{
					e.NewValue = true;
				}
				else
				{
					InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(Base, apTran.InventoryID);
					InventoryItemExt itemExt = PXCache<InventoryItem>.GetExtension<InventoryItemExt>(item);

					if (item.StkItem == true)
					{
						e.NewValue = false;
					}
					else
					{
						e.NewValue = itemExt.T5018Service;
					}
				}
			}
		}

		protected virtual void APTran_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APTran apTran = (APTran)e.Row;
			if (apTran == null) return;

			if (LocalizationServiceExtensions.LocalizationEnabled<FeaturesSet.canadianLocalization>(this.Base))
			{
				APTranExt tranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
				if (apTran.InventoryID == null)
				{
					sender.SetValueExt<APTranExt.t5018Service>(e.Row, true);
				}
				else
				{
					InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(Base, apTran.InventoryID);
					InventoryItemExt itemExt = PXCache<InventoryItem>.GetExtension<InventoryItemExt>(item);
					if (item.StkItem == true)
					{
						sender.SetValueExt<APTranExt.t5018Service>(e.Row, false);
					}
					else
					{
						sender.SetValueExt<APTranExt.t5018Service>(e.Row, itemExt.T5018Service);
					}
				}
			}
		}

		protected virtual void APInvoice_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			PXUIFieldAttribute.SetVisible<APTranExt.t5018Service>(Base.Transactions.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<APTranExt.t5018Service>(Base.Transactions.Cache, null, false);

			bool isPPM = Base.CurrentDocument.Current.DocType == APDocType.Prepayment;
			bool isRetainageBill = Base.CurrentDocument.Current.DocType == APDocType.Invoice && Base.CurrentDocument.Current.OrigRefNbr != null;
			bool isRetainagePayByLine = Base.CurrentDocument.Current.PaymentsByLinesAllowed ?? false;

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

			if (LocalizationServiceExtensions.LocalizationEnabled<FeaturesSet.canadianLocalization>(this.Base)
				&& vendorT5018 && !isPPM && !(isRetainageBill && !isRetainagePayByLine))
			{
				PXUIFieldAttribute.SetVisible<APTranExt.t5018Service>(Base.Transactions.Cache, null, true);

				bool letEdit;

				foreach (APTran tran in Base.Transactions.Select())
				{
					letEdit = true;

					if (tran.InventoryID != null)
					{
						InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(Base, tran.InventoryID);
						if (item.StkItem == true)
							letEdit = false;
					}

					if (letEdit)
					{
						PXUIFieldAttribute.SetEnabled<APTranExt.t5018Service>(Base.Transactions.Cache, null, true);

						if (Base.Transactions.Cache.AllowUpdate == false)
						{
							Base.Transactions.Cache.AllowUpdate = true;
							PXUIFieldAttribute.SetEnabled(Base.Transactions.Cache, null, false);
							PXUIFieldAttribute.SetEnabled(Base.Transactions.Cache, nameof(APTranExt.t5018Service), true);
						}
					}
					else
					{
						PXUIFieldAttribute.SetEnabled<APTranExt.t5018Service>(Base.Transactions.Cache, tran, isEnabled: false);
					}
				}
			}
		}

		#endregion
	}
}
