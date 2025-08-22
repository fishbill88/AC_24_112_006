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
using System;

namespace PX.Objects.SO
{
	// The SOSiteStatusLookup class has been moved to this file from "WebSites/Pure/PX.Objects/SO/Descriptor/Attribute.cs".
	// All changes made to this class must be duplicated in the SOOrderSiteStatusLookupExt class.
	[Obsolete("This class is obsolete. Use SOOrderSiteStatusLookupExt instead.")]
	public class SOSiteStatusLookup<Status, StatusFilter> : INSiteStatusLookup<Status, StatusFilter>
		where Status : class, IBqlTable, new()
		where StatusFilter : SOSiteStatusFilter, new()
	{
		#region Ctor
		public SOSiteStatusLookup(PXGraph graph)
			: base(graph)
		{
			graph.RowSelecting.AddHandler(typeof(SOOrderSiteStatusSelected), OnRowSelecting);
		}

		public SOSiteStatusLookup(PXGraph graph, Delegate handler)
			: base(graph, handler)
		{
			graph.RowSelecting.AddHandler(typeof(SOOrderSiteStatusSelected), OnRowSelecting);
		}
		#endregion
		protected virtual void OnRowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			if (sender.Fields.Contains(typeof(SOOrderSiteStatusSelected.curyID).Name) &&
					sender.GetValue<SOOrderSiteStatusSelected.curyID>(e.Row) == null)
			{
				PXCache orderCache = sender.Graph.Caches[typeof(SOOrder)];
				sender.SetValue<SOOrderSiteStatusSelected.curyID>(e.Row,
					orderCache.GetValue<SOOrder.curyID>(orderCache.Current));
				sender.SetValue<SOOrderSiteStatusSelected.curyInfoID>(e.Row,
					orderCache.GetValue<SOOrder.curyInfoID>(orderCache.Current));
			}
		}

		protected override void OnFilterSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			base.OnFilterSelected(sender, e);
			SOSiteStatusFilter filter = (SOSiteStatusFilter)e.Row;
			PXUIFieldAttribute.SetVisible<SOSiteStatusFilter.historyDate>(sender, null, filter.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<SOSiteStatusFilter.dropShipSales>(sender, null, filter.Mode == SOAddItemMode.ByCustomer);
			sender.Adjust<PXUIFieldAttribute>().For<SOSiteStatusFilter.customerLocationID>(a =>
				a.Visible = a.Enabled = (filter.Behavior == SOBehavior.BL));

			PXCache status = sender.Graph.Caches[typeof(SOOrderSiteStatusSelected)];
			PXUIFieldAttribute.SetVisible<SOOrderSiteStatusSelected.qtyLastSale>(status, null, filter.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<SOOrderSiteStatusSelected.curyID>(status, null, filter.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<SOOrderSiteStatusSelected.curyUnitPrice>(status, null, filter.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<SOOrderSiteStatusSelected.lastSalesDate>(status, null, filter.Mode == SOAddItemMode.ByCustomer);

			status.Adjust<PXUIFieldAttribute>()
				.For<SOOrderSiteStatusSelected.dropShipLastBaseQty>(x => { x.Visible = filter.DropShipSales == true; })
				.SameFor<SOOrderSiteStatusSelected.dropShipLastQty>()
				.SameFor<SOOrderSiteStatusSelected.dropShipLastUnitPrice>()
				.SameFor<SOOrderSiteStatusSelected.dropShipCuryUnitPrice>()
				.SameFor<SOOrderSiteStatusSelected.dropShipLastDate>();

			if (filter.HistoryDate == null)
				// Acuminator disable once PX1047 RowChangesInEventHandlersForbiddenForArgs - Obsolete code
				filter.HistoryDate = sender.Graph.Accessinfo.BusinessDate.GetValueOrDefault().AddMonths(-3);
		}
	}
}
