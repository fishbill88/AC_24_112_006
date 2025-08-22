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
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Messages = PX.Objects.Common.Messages;

namespace PX.Objects.Extensions.AddItemLookup
{
	public abstract class AddItemLookupBaseExt<TGraph, TDocument, TItemInfo, TItemFilter> : PXGraphExtension<TGraph>
		where TGraph : PXGraph
		where TDocument : class, IBqlTable, new()
		where TItemInfo : class, IPXSelectable, IBqlTable, new()
		where TItemFilter : INSiteStatusFilter, IBqlTable, new()
	{
		protected readonly string Selected = nameof(IPXSelectable.Selected);

		[PXFilterable]
		[PXCopyPasteHiddenView]
		public SelectFrom<TItemInfo>.View ItemInfo;

		public PXFilter<TItemFilter> ItemFilter;

		public PXAction<TDocument> showItems;

		[PXUIField(DisplayName = Messages.ShowItems, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton(CommitChanges = true, DisplayOnMainToolbar = false)]
		public virtual IEnumerable ShowItems(PXAdapter adapter)
		{
			return ShowItemsHandler(adapter);
		}

		protected abstract IEnumerable ShowItemsHandler(PXAdapter adapter);

		public PXAction<TDocument> addSelectedItems;

		[PXUIField(DisplayName = Messages.Add, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton(CommitChanges = true, DisplayOnMainToolbar = false)]
		public virtual IEnumerable AddSelectedItems(PXAdapter adapter)
		{
			return AddSelectedItemsHandler(adapter);
		}

		protected abstract IEnumerable AddSelectedItemsHandler(PXAdapter adapter);

		protected virtual IEnumerable itemInfo()
		{
			var intView = CreateItemInfoView();
			var startRow = PXView.StartRow;
			var totalRows = 0;

			var rows = new PXDelegateResult();

			foreach (var rec in intView.Select(null, null, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters,
								 ref startRow, PXView.MaximumRows, ref totalRows))
			{
				TItemInfo item = PXResult.Unwrap<TItemInfo>(rec);
				TItemInfo result = item;
				TItemInfo updated = ItemInfo.Locate(item);
				if (updated != null && updated.Selected == true)
				{
					result = SetValuesOfSelectedRow(updated, item);
				}
				rows.Add(result);
			}
			PXView.StartRow = 0;

			if (PXView.ReverseOrder)
				rows.Reverse();

			rows.IsResultSorted = true;

			return rows;
		}

		protected virtual TItemInfo SetValuesOfSelectedRow(TItemInfo updatedItem, TItemInfo originalItem)
		{
			ItemInfo.Cache.RestoreCopy(updatedItem, originalItem);
			updatedItem.Selected = true;
			return updatedItem;
		}

		protected virtual PXView CreateItemInfoView()
		{
			return new LookupView(Base, new Select<TItemInfo>().WhereAnd(CreateWhere()));
		}

		protected Type CreateWhere()
		{
			var filterFields = ItemFilter.Cache.Fields;
			var statusViewFields = ItemInfo.Cache.Fields;
			Type where = typeof(Where<True, Equal<True>>);
			foreach (string field in filterFields)
			{
				if (statusViewFields.Contains(field))
				{
					if (filterFields.Contains(field + "Wildcard")) continue;
					if (field.Contains("SubItem") && !PXAccess.FeatureInstalled<FeaturesSet.subItem>()) continue;
					if (field.Contains("Site") && !PXAccess.FeatureInstalled<FeaturesSet.warehouse>()) continue;
					if (field.Contains("Location") && !PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>()) continue;
					Type sourceType = ItemFilter.Cache.GetBqlField(field);
					Type destinationType = ItemInfo.Cache.GetBqlField(field);
					if (sourceType != null && destinationType != null)
					{
						where = BqlTemplate.OfCondition<Where<
							Brackets<BqlPlaceholder.S.AsField.FromCurrent.IsNull.
							Or<BqlPlaceholder.S.AsField.FromCurrent.IsEqual<BqlPlaceholder.D.AsOperand>>>.
							And<BqlPlaceholder.W>>>
							.Replace<BqlPlaceholder.S>(sourceType)
							.Replace<BqlPlaceholder.D>(destinationType)
							.Replace<BqlPlaceholder.W>(where)
							.ToType();
					}
				}
				string f;
				if (field.Length > 8 && field.EndsWith("Wildcard") && statusViewFields.Contains(f = field.Substring(0, field.Length - 8)))
				{
					if (field.Contains("SubItem") && !PXAccess.FeatureInstalled<FeaturesSet.subItem>()) continue;
					if (field.Contains("Site") && !PXAccess.FeatureInstalled<FeaturesSet.warehouse>()) continue;
					if (field.Contains("Location") && !PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>()) continue;
					Type like = ItemFilter.Cache.GetBqlField(field);
					Type dest = ItemInfo.Cache.GetBqlField(f);
					where = BqlTemplate.OfCondition<Where<Brackets<BqlPlaceholder.L.AsField.FromCurrent.IsNull.
						Or<BqlPlaceholder.D.AsOperand.IsLike<BqlPlaceholder.L.AsField.FromCurrent>>>.
						And<BqlPlaceholder.W>>>
						.Replace<BqlPlaceholder.L>(like)
						.Replace<BqlPlaceholder.D>(dest)
						.Replace<BqlPlaceholder.W>(where)
						.ToType();
				}
			}
			return where;
		}

		protected virtual void _(Events.RowSelected<TItemInfo> e)
		{
			PXUIFieldAttribute.SetEnabled(e.Cache, e.Row, false);
			PXUIFieldAttribute.SetEnabled(e.Cache, e.Row, Selected, true);
		}

		protected virtual void _(Events.RowSelected<TItemFilter> e)
		{
			if (e.Row != null)
				PXUIFieldAttribute.SetVisible(ItemInfo.Cache, nameof(INSite.SiteID), e.Row.SiteID == null);
		}

		protected class LookupView : PXView
		{
			public LookupView(PXGraph graph, BqlCommand command)
				: base(graph, true, command)
			{
			}

			public LookupView(PXGraph graph, BqlCommand command, Delegate handler)
				: base(graph, true, command, handler)
			{
			}

			protected PXSearchColumn CorrectFieldName(PXSearchColumn orig, bool idFound)
			{
				switch (orig.Column.ToLower())
				{
					case "inventoryid":
						if (!idFound)
							return new PXSearchColumn(nameof(InventoryItem.InventoryCD), orig.Descending, orig.OrigSearchValue ?? orig.SearchValue);
						else
							return null;
					case "subitemid":
						return new PXSearchColumn(nameof(INSubItem.SubItemCD), orig.Descending, orig.OrigSearchValue ?? orig.SearchValue);
					case "siteid":
						return new PXSearchColumn(nameof(INSite.SiteCD), orig.Descending, orig.OrigSearchValue ?? orig.SearchValue);
					case "locationid":
						return new PXSearchColumn(nameof(INLocation.LocationCD), orig.Descending, orig.OrigSearchValue ?? orig.SearchValue);
				}
				return orig;
			}

			protected override List<object> InvokeDelegate(object[] parameters)
			{
				if (MaximumRows == 1 && StartRow == 0 &&
					Searches?.Length == Cache.Keys.Count &&
					Searches.All(s => s != null) &&
					SortColumns?.Length == Cache.Keys.Count &&
					SortColumns.SequenceEqual(Cache.Keys, StringComparer.OrdinalIgnoreCase))
				{
					return base.InvokeDelegate(parameters);
				}

				var context = _Executing.Peek();
				var orig = context.Sorts;

				bool idFound = false;
				var result = new List<PXSearchColumn>();
				const string iD = nameof(InventoryItem.InventoryCD);
				for (int i = 0; i < orig.Length - Cache.Keys.Count; i++)
				{
					result.Add(CorrectFieldName(orig[i], false));
					if (orig[i].Column == iD)
						idFound = true;
				}

				for (int i = orig.Length - Cache.Keys.Count; i < orig.Length; i++)
				{
					var col = CorrectFieldName(orig[i], idFound);
					if (col != null)
						result.Add(col);
				}
				context.Sorts = result.ToArray();

				return base.InvokeDelegate(parameters);
			}
		}
	}
}
