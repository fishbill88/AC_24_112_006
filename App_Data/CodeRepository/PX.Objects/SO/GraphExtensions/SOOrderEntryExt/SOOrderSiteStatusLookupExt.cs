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

using PX.Objects.Common;
using PX.Objects.Common.Bql;
using PX.Objects.Extensions.AddItemLookup;
using PX.Objects.IN;
using System.Collections.Generic;

using PX.Objects.IN;
using PX.Objects.CS;
using System;

namespace PX.Objects.SO.GraphExtensions.SOOrderEntryExt
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class SOOrderSiteStatusLookupExt : AlternateIDLookupExt<SOOrderEntry, SOOrder, SOLine, SOOrderSiteStatusSelected, SOSiteStatusFilter, SOOrderSiteStatusSelected.salesUnit>
	{
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[InterBranchRestrictor(typeof(Where2<SameOrganizationBranch<INSite.branchID, Current<SOOrder.branchID>>,
			Or<Current<SOOrder.behavior>, Equal<SOBehavior.qT>>>))]
		protected virtual void _(Events.CacheAttached<SOSiteStatusFilter.siteID> e) { }

		protected override SOLine CreateNewLine(SOOrderSiteStatusSelected line)
		{
			SOLine newline = PXCache<SOLine>.CreateCopy(Base.Transactions.Insert(new SOLine()));
			newline.SiteID = line.SiteID ?? newline.SiteID;
			newline.InventoryID = line.InventoryID;
			if (line.SubItemID != null) // line.SubItemID is null when the line doesn't have INSiteStatusByCostCenter
				newline.SubItemID = line.SubItemID;

			newline.UOM = line.SalesUnit;
			newline.AlternateID = line.AlternateID;
			if (ItemFilter.Current?.CustomerLocationID != null)
				newline.CustomerLocationID = ItemFilter.Current.CustomerLocationID;
			newline = PXCache<SOLine>.CreateCopy(Base.Transactions.Update(newline));
			if (newline.RequireLocation != true)
			{
				newline.LocationID = null;
				newline = PXCache<SOLine>.CreateCopy(Base.Transactions.Update(newline));
			}
			newline.Qty = line.QtySelected;
			return Base.Transactions.Update(newline);
		}

		protected override Dictionary<string, int> GetAlternateTypePriority() => new Dictionary<string, int> {
			{ INAlternateType.CPN, 0 },
			{ INAlternateType.Global, 10 },
			{ INAlternateType.Barcode, 20 },
			{ INAlternateType.GIN, 30 },
			{ INAlternateType.VPN, 40 }
		};

		protected override bool ScreenSpecificFilter(INItemXRef xRef)
		{
			if (xRef.AlternateType != INAlternateType.CPN)
				return true;

			return xRef.BAccountID == Base.Document.Current?.CustomerID;
		}

		protected virtual void _(Events.RowInserted<SOSiteStatusFilter> e)
		{
			if (e.Row == null)
			{
				return;
			}

			if (Base.Document.Current != null)
			{
				e.Row.SiteID = Base.Document.Current.DefaultSiteID;
				e.Row.Behavior = Base.Document.Current.Behavior;
				e.Row.CustomerLocationID = Base.Document.Current.CustomerLocationID;
			}

			if (!PXAccess.FeatureInstalled<FeaturesSet.inventory>())
				e.Row.OnlyAvailable = false;

			if (e.Row.HistoryDate == null)
				e.Row.HistoryDate = e.Cache.Graph.Accessinfo.BusinessDate.GetValueOrDefault().AddMonths(-3);
		}

		protected virtual void _(Events.RowSelecting<SOOrderSiteStatusSelected> e)
		{
			if (e.Cache.Fields.Contains(typeof(SOOrderSiteStatusSelected.curyID).Name) &&
					e.Cache.GetValue<SOOrderSiteStatusSelected.curyID>(e.Row) == null)
			{
				PXCache orderCache = e.Cache.Graph.Caches<SOOrder>();
				e.Cache.SetValue<SOOrderSiteStatusSelected.curyID>(e.Row,
					orderCache.GetValue<SOOrder.curyID>(orderCache.Current));
				e.Cache.SetValue<SOOrderSiteStatusSelected.curyInfoID>(e.Row,
					orderCache.GetValue<SOOrder.curyInfoID>(orderCache.Current));
			}
		}

		protected override void _(Events.RowSelected<SOSiteStatusFilter> e)
		{
			base._(e);

			PXUIFieldAttribute.SetVisible<SOSiteStatusFilter.historyDate>(e.Cache, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<SOSiteStatusFilter.dropShipSales>(e.Cache, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			e.Cache.Adjust<PXUIFieldAttribute>().For<SOSiteStatusFilter.customerLocationID>(a =>
				a.Visible = a.Enabled = e.Row.Behavior == SOBehavior.BL);

			PXCache status = e.Cache.Graph.Caches<SOOrderSiteStatusSelected>();
			PXUIFieldAttribute.SetVisible<SOOrderSiteStatusSelected.qtyLastSale>(status, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<SOOrderSiteStatusSelected.curyID>(status, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<SOOrderSiteStatusSelected.curyUnitPrice>(status, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<SOOrderSiteStatusSelected.lastSalesDate>(status, null, e.Row.Mode == SOAddItemMode.ByCustomer);

			status.Adjust<PXUIFieldAttribute>()
				.For<SOOrderSiteStatusSelected.dropShipLastBaseQty>(x => x.Visible = e.Row.DropShipSales == true)
				.SameFor<SOOrderSiteStatusSelected.dropShipLastQty>()
				.SameFor<SOOrderSiteStatusSelected.dropShipLastUnitPrice>()
				.SameFor<SOOrderSiteStatusSelected.dropShipCuryUnitPrice>()
				.SameFor<SOOrderSiteStatusSelected.dropShipLastDate>();
		}

		protected virtual void _(Events.RowSelected<SOOrder> e)
		{
			if (e.Row?.IsExpired == true)
			{
				showItems.SetEnabled(false);
			}
			else
			{
				showItems.SetEnabled(Base.Transactions.Cache.AllowInsert);
			}
		}
	}
}
