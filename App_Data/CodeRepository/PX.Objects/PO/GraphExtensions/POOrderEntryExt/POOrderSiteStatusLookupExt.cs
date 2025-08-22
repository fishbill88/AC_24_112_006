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
using PX.Objects.IN;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.Extensions.AddItemLookup;
using PX.Data.BQL.Fluent;
using System.Collections.Generic;
using System;

namespace PX.Objects.PO.GraphExtensions.POOrderEntryExt
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class POOrderSiteStatusLookupExt : AlternateIDLookupExt<POOrderEntry, POOrder, POLine, POSiteStatusSelected, POSiteStatusFilter, POSiteStatusSelected.purchaseUnit>
	{
		protected override POLine CreateNewLine(POSiteStatusSelected line)
		{
			POLine newline = new POLine();
			newline.SiteID = line.SiteID;
			switch (Base.Document.Current.OrderType)
			{
				case POOrderType.DropShip:
					newline.LineType = POLineType.GoodsForDropShip;
					break;
				case POOrderType.ProjectDropShip:
					newline.LineType = POLineType.GoodsForProject;
					break;
				default:
					newline.LineType = POLineType.GoodsForInventory;
					break;
			}
			newline.InventoryID = line.InventoryID;
			newline.SubItemID = line.SubItemID;
			newline.UOM = line.PurchaseUnit;
			newline.OrderQty = line.QtySelected;
			newline.AlternateID = line.AlternateID;
			return Base.Transactions.Insert(newline);
		}
		protected override Dictionary<string, int> GetAlternateTypePriority() => new Dictionary<string, int> {
			{ INAlternateType.VPN, 0 },
			{ INAlternateType.Global, 10 },
			{ INAlternateType.Barcode, 20 },
			{ INAlternateType.GIN, 30  },
			{ INAlternateType.CPN, 40 }
		};

		protected override bool ScreenSpecificFilter(INItemXRef xRef)
		{
			if (xRef.AlternateType != INAlternateType.VPN)
				return true;

			return xRef.BAccountID == Base.Document.Current?.VendorID;
		}

		protected virtual void _(Events.RowInserted<POSiteStatusFilter> e)
		{
			if (e.Row != null && Base.Document.Current != null)
			{
				Location location = SelectFrom<Location>.
										Where<Location.locationID.IsEqual<POOrder.vendorLocationID.FromCurrent>.
										And<Location.bAccountID.IsEqual<POOrder.vendorID.FromCurrent>>>.
										View.SelectWindowed(Base, 0, 1);

				var vSite = INSite.PK.Find(Base, location?.VSiteID);
				if (vSite != null && (PXAccess.FeatureInstalled<FeaturesSet.interBranch>()
					|| PXAccess.IsSameParentOrganization(vSite.BranchID, Base.Document.Current.BranchID)))
				{
					e.Row.SiteID = vSite.SiteID;
				}
				e.Row.VendorID = Base.Document.Current.VendorID;
			}
		}

		protected virtual void _(Events.RowSelected<POOrder> e)
		{
			if (e.Row == null)
			{
				return;
			}

			bool vendorSelected = e.Row.VendorID != null && e.Row.VendorLocationID != null;
			showItems.SetEnabled(vendorSelected);
		}

		protected override PXView CreateItemInfoView()
		{
			PXCache filterCache = Base.Caches<POSiteStatusFilter>();

			if (filterCache.Current != null && ((POSiteStatusFilter)filterCache.Current).OnlyAvailable == true)
			{
				BqlCommand command = new
					SelectFrom<POSiteStatusSelected>.
					InnerJoin<POVendorInventoryOnly>.On<
						POVendorInventoryOnly.inventoryID.IsEqual<POSiteStatusSelected.inventoryID>.
						And<POVendorInventoryOnly.vendorID.IsEqual<POSiteStatusFilter.vendorID.FromCurrent>>.
						And<POVendorInventoryOnly.subItemID.IsEqual<POSiteStatusSelected.subItemID>.
							Or<POSiteStatusSelected.subItemID.IsNull>>>()
				.WhereAnd(CreateWhere());

				return new LookupView(Base, command);
			}

			return base.CreateItemInfoView();
		}
	}
}
