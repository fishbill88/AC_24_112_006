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

using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.SO.GraphExtensions.SOInvoiceEntryExt
{
	public class SOInvoiceSiteStatusLookupExt : SiteStatusLookupExt<SOInvoiceEntry, ARInvoice, ARTran, SOInvoiceSiteStatusSelected, SOSiteStatusFilter>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.advancedSOInvoices>();
		}

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[InterBranchRestrictor(typeof(Where2<SameOrganizationBranch<INSite.branchID, Current2<ARInvoice.branchID>>,
			Or<Where<Current2<ARInvoice.branchID>, IsNull,
				And<SameOrganizationBranch<INSite.branchID, Current<AccessInfo.branchID>>>>>>))]
		protected virtual void _(Events.CacheAttached<SOSiteStatusFilter.siteID> e) { }

		protected override ARTran CreateNewLine(SOInvoiceSiteStatusSelected line)
		{
			ARTran newline = Base.Transactions.Insert(new ARTran());
			newline.SiteID = line.SiteID ?? newline.SiteID;
			newline.InventoryID = line.InventoryID;
			if (line.SubItemID != null)
			{
				// line.SubItemID is null when the line doesn't have INSiteStatusByCostCenter
				newline.SubItemID = line.SubItemID;
			}
			newline.UOM = line.SalesUnit;
			newline = Base.Transactions.Update(newline);
			newline.Qty = line.QtySelected;
			return Base.Transactions.Update(newline);
		}

		protected virtual void _(Events.RowInserted<SOSiteStatusFilter> e)
		{
			if (e.Row == null)
				return;

			if (!PXAccess.FeatureInstalled<FeaturesSet.inventory>())
				e.Row.OnlyAvailable = false;

			if (e.Row.HistoryDate == null)
				e.Row.HistoryDate = e.Cache.Graph.Accessinfo.BusinessDate.GetValueOrDefault().AddMonths(-3);
		}

		protected virtual void _(Events.RowSelecting<SOInvoiceSiteStatusSelected> e)
		{
			if (e.Cache.Fields.Contains(typeof(SOInvoiceSiteStatusSelected.curyID).Name) &&
					e.Cache.GetValue<SOInvoiceSiteStatusSelected.curyID>(e.Row) == null)
			{
				PXCache cache = e.Cache.Graph.Caches<ARInvoice>();
				e.Cache.SetValue<SOInvoiceSiteStatusSelected.curyID>(e.Row,
					cache.GetValue<ARInvoice.curyID>(cache.Current));
				e.Cache.SetValue<SOInvoiceSiteStatusSelected.curyInfoID>(e.Row,
					cache.GetValue<ARInvoice.curyInfoID>(cache.Current));
			}
		}

		protected override void _(Events.RowSelected<SOSiteStatusFilter> e)
		{
			base._(e);

			PXUIFieldAttribute.SetVisible<SOSiteStatusFilter.historyDate>(e.Cache, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<SOSiteStatusFilter.dropShipSales>(e.Cache, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			e.Cache.Adjust<PXUIFieldAttribute>().For<SOSiteStatusFilter.customerLocationID>(a => a.Visible = a.Enabled = false);

			PXCache status = e.Cache.Graph.Caches<SOInvoiceSiteStatusSelected>();
			PXUIFieldAttribute.SetVisible<SOInvoiceSiteStatusSelected.qtyLastSale>(status, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<SOInvoiceSiteStatusSelected.curyID>(status, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<SOInvoiceSiteStatusSelected.curyUnitPrice>(status, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<SOInvoiceSiteStatusSelected.lastSalesDate>(status, null, e.Row.Mode == SOAddItemMode.ByCustomer);

			status.Adjust<PXUIFieldAttribute>()
				.For<SOInvoiceSiteStatusSelected.dropShipLastBaseQty>(x => x.Visible = e.Row.DropShipSales == true)
				.SameFor<SOInvoiceSiteStatusSelected.dropShipLastQty>()
				.SameFor<SOInvoiceSiteStatusSelected.dropShipLastUnitPrice>()
				.SameFor<SOInvoiceSiteStatusSelected.dropShipCuryUnitPrice>()
				.SameFor<SOInvoiceSiteStatusSelected.dropShipLastDate>();
		}

		protected virtual void _(Events.RowSelected<ARInvoice> e)
		{
			showItems.SetEnabled(Base.Transactions.Cache.AllowInsert);
		}
	}
}
