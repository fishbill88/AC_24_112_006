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
using System.Linq;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.IN;
using PX.Objects.IN.GraphExtensions;

namespace PX.Objects.SO.GraphExtensions.ManageSalesAllocationsExt
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class InventoryLinkFilterExt : InventoryLinkFilterExtensionBase<ManageSalesAllocations, SalesAllocationsFilter, SalesAllocationsFilter.inventoryID>
	{
		// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
		public class descr : AttachedInventoryDescription<descr> { }

		[PXMergeAttributes(Method = MergeMethod.Replace)]
		[StockItem(IsKey = true)]
		[PXRestrictor(typeof(Where<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.noSales>>),
			IN.Messages.InventoryItemIsInStatus, typeof(InventoryItem.itemStatus), ShowWarning = true)]
		protected override void _(Events.CacheAttached<InventoryLinkFilter.inventoryID> e) { }

		protected virtual void _(Events.RowUpdated<SalesAllocationsFilter> e)
		{
			if (!e.Cache.ObjectsEqual<SalesAllocationsFilter.inventoryID>(e.Row, e.OldRow))
			{
				SelectedItems.Cache.Clear();
				SelectedItems.Cache.ClearQueryCache();
			}
		}

		/// Overrides <see cref="ManageSalesAllocations.CreateBaseQuery(SalesAllocationsFilter, List{object})"/>
		[PXOverride]
		public PXSelectBase<SalesAllocation> CreateBaseQuery(SalesAllocationsFilter filter, List<object> parameters,
			Func<SalesAllocationsFilter, List<object>, PXSelectBase<SalesAllocation>> base_CreateBaseQuery)
		{
			var query = base_CreateBaseQuery(filter, parameters);

			var inventories = GetSelectedEntities(filter).ToArray();
			if (inventories.Length > 0)
			{
				query.WhereAnd<Where<SalesAllocation.inventoryID.IsIn<@P.AsInt>>>();
				parameters.Add(inventories);
			}

			return query;
		}
	}
}
