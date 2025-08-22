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

using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.Common.Interfaces;
using PX.Objects.IN;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.Extensions.AddItemLookup
{
	public abstract class AlternateIDLookupExt<TGraph, TDocument, TLine, TItemInfo, TItemFilter, TUnitField> : SiteStatusLookupExt<TGraph, TDocument, TLine, TItemInfo, TItemFilter>
		where TGraph : PXGraph
		where TDocument : class, IBqlTable, new()
		where TLine : class, IBqlTable, new()
		where TItemInfo : class, IQtySelectable, IPXSelectable, IBqlTable, IAlternateSelectable, new()
		where TItemFilter : INSiteStatusFilter, IBqlTable, new()
		where TUnitField : class, IBqlField, IImplement<IBqlString>
	{
		protected virtual void _(Events.RowSelecting<TItemInfo> e)
		{
			TItemFilter filter = (TItemFilter)Base.Caches<TItemFilter>().Current;
			// Acuminator disable once PX1042 DatabaseQueriesInRowSelecting [Not needed on this version]
			FillAlternateFields(e.Row, filter);
		}

		#region Filling of Alternate fields
		protected virtual void FillAlternateFields(TItemInfo itemInfo, TItemFilter filter)
		{
			if (!string.IsNullOrEmpty(filter.Inventory))
			{
				FillAlternateFieldsForInventory(itemInfo);
				return;
			}

			if (!string.IsNullOrEmpty(filter.BarCode))
			{
				FillAlternateFieldsForBarCode(itemInfo);
				return;
			}

			if (string.IsNullOrEmpty(itemInfo.AlternateID))
				FillAlternateFieldsForEmptyFilter(itemInfo);			
		}

		protected virtual void FillAlternateFieldsForBarCode(TItemInfo itemInfo)
		{
			if (itemInfo.BarCode != null)
			{
				itemInfo.AlternateType = itemInfo.BarCodeType;
				itemInfo.AlternateID = itemInfo.BarCode;
				itemInfo.AlternateDescr = itemInfo.BarCodeDescr;
			}
		}

		protected virtual void FillAlternateFieldsForInventory(TItemInfo itemInfo)
		{
			if (itemInfo.InventoryAlternateID != null)
			{
				itemInfo.AlternateType = itemInfo.InventoryAlternateType;
				itemInfo.AlternateID = itemInfo.InventoryAlternateID;
				itemInfo.AlternateDescr = itemInfo.InventoryAlternateDescr;
			}
		}

		protected virtual void FillAlternateFieldsForEmptyFilter(TItemInfo itemInfo)
		{
			INItemXRef xRef =
				SelectFrom<INItemXRef>.
					Where<
						INItemXRef.inventoryID.IsEqual<@P.AsInt>
						.And<Where<INItemXRef.uOM.IsEqual<@P.AsString>.Or<INItemXRef.uOM.IsNull>>>>
					.View.Select(
						Base,
						itemInfo.InventoryID,
						Base.Caches<TItemInfo>().GetValue<TUnitField>(itemInfo))
					.RowCast<INItemXRef>().AsEnumerable().Where(ScreenSpecificFilter)
					.OrderBy(
						_ => (_.UOM, _.AlternateType),
						new AlternateTypeComparer(GetAlternateTypePriority())).FirstOrDefault();

			if (xRef != null)
			{
				itemInfo.AlternateType = xRef.AlternateType;
				itemInfo.AlternateID = xRef.AlternateID;
				itemInfo.AlternateDescr = xRef.Descr;
			}
		}
		#endregion

		#region Abstract
		protected abstract bool ScreenSpecificFilter(INItemXRef xRef);

		protected abstract Dictionary<string, int> GetAlternateTypePriority();
		#endregion

		#region Nested
		protected class AlternateTypeComparer : IComparer<(string, string)>
		{
			protected Dictionary<string, int> _alternateTypePriority;
			public AlternateTypeComparer(Dictionary<string, int> alternateTypePriority)
			{
				_alternateTypePriority = alternateTypePriority;
			}

			public int Compare((string, string) x, (string, string) y)
			{
				int priorityComparison =
					(_alternateTypePriority.TryGetValue(x.Item2, out int xPriority) ? xPriority : int.MaxValue)
				  - (_alternateTypePriority.TryGetValue(y.Item2, out int yPriority) ? yPriority : int.MaxValue);
				if (priorityComparison != 0)
					return priorityComparison;

				bool xHasUOM = !string.IsNullOrEmpty(x.Item1);
				bool yHasUOM = !string.IsNullOrEmpty(y.Item1);
				if (xHasUOM ^ yHasUOM)
					return xHasUOM ? -1 : 1;

				return 0;
			}
		}
		#endregion
	}
}
