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
using PX.Objects.CS;
using System;

namespace PX.Objects.AP
{
	// The APAddItemLookup class has been moved to this file from "WebSites/Pure/PX.Objects/AP/APPriceWorksheetMaint.cs".
	// All changes made to this class must be duplicated in the APPriceWorksheetAddItemLookupExt class.
	[Obsolete("This class is obsolete. Use APPriceWorksheetAddItemLookupExt instead.")]
	public class APAddItemLookup<Status, StatusFilter> : INSiteStatusLookup<Status, StatusFilter>
	where Status : class, IBqlTable, new()
	where StatusFilter : AddItemFilter, new()
	{
		#region Ctor
		public APAddItemLookup(PXGraph graph)
			: base(graph)
		{
			graph.RowSelecting.AddHandler(typeof(APAddItemSelected), OnRowSelecting);
		}

		public APAddItemLookup(PXGraph graph, Delegate handler)
			: base(graph, handler)
		{
			graph.RowSelecting.AddHandler(typeof(APAddItemSelected), OnRowSelecting);
		}
		#endregion
		protected virtual void OnRowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			//remove
		}

		protected override void OnFilterSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			base.OnFilterSelected(sender, e);
			AddItemFilter filter = (AddItemFilter)e.Row;
			PXCache status = sender.Graph.Caches[typeof(APAddItemSelected)];
			PXUIFieldAttribute.SetVisible<APAddItemSelected.curyID>(status, null, true);
			PXUIFieldAttribute.SetEnabled<AddItemFilter.workGroupID>(sender, e.Row, filter?.MyWorkGroup != true);
			PXUIFieldAttribute.SetEnabled<AddItemFilter.ownerID>(sender, e.Row, filter?.MyOwner != true);
		}

		protected override PXView CreateIntView(PXGraph graph)
		{
			var view = base.CreateIntView(graph);

			view.WhereAnd<Where<Current<AddItemFilter.ownerID>, IsNull,
						Or<Current<AddItemFilter.ownerID>, Equal<APAddItemSelected.productManagerID>>>>();

			view.WhereAnd<Where<Current<AddItemFilter.myWorkGroup>, Equal<boolFalse>,
						 Or<APAddItemSelected.productWorkgroupID, PX.TM.IsWorkgroupOfContact<CurrentValue<AddItemFilter.currentOwnerID>>>>>();

			view.WhereAnd<Where<Current<AddItemFilter.workGroupID>, IsNull,
						Or<Current<AddItemFilter.workGroupID>, Equal<APAddItemSelected.productWorkgroupID>>>>();

			return view;
		}
	}
}
