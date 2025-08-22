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
using System.Collections.Generic;
using System;

namespace PX.Objects.PO
{
	// The POSiteStatusLookup class has been moved to this file from "WebSites/Pure/PX.Objects/PO/POOrderEntry.cs".
	// All changes made to this class must be duplicated in the POOrderSiteStatusLookupExt class.
	[Obsolete("This class is obsolete. Use POOrderSiteStatusLookupExt instead.")]
	public class POSiteStatusLookup<Status, StatusFilter> : INSiteStatusLookup<Status, StatusFilter>
		where StatusFilter : INSiteStatusFilter, new()
		where Status : class, IBqlTable, new()
	{
		public POSiteStatusLookup(PXGraph graph)
			: base(graph)
		{
		}

		public POSiteStatusLookup(PXGraph graph, Delegate handler)
			: base(graph, handler)
		{
		}

		protected override PXView CreateIntView(PXGraph graph)
		{
			PXCache cache = graph.Caches[typeof(Status)];
			PXCache fcache = graph.Caches[typeof(StatusFilter)];

			List<Type> select = new List<Type>();

			if (fcache.Current != null && ((StatusFilter)fcache.Current).OnlyAvailable == true)
			{
				select.Add(typeof(Select2<,,>));
				select.Add(typeof(Status));
				select.Add(typeof(InnerJoin<POVendorInventoryOnly,
					On<POVendorInventoryOnly.inventoryID, Equal<POSiteStatusSelected.inventoryID>,
						And<POVendorInventoryOnly.vendorID, Equal<Current<POSiteStatusFilter.vendorID>>,
						And<Where<POVendorInventoryOnly.subItemID, Equal<POSiteStatusSelected.subItemID>, Or<POSiteStatusSelected.subItemID, IsNull>>>>>>));
				select.Add(CreateWhere(graph));

				Type selectType = BqlCommand.Compose(select.ToArray());

				return new LookupView(graph, BqlCommand.CreateInstance(selectType));
			}

			return base.CreateIntView(graph);
		}
	}
}
