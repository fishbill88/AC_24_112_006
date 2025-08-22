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
using PX.Objects.Common.Interfaces;
using PX.Objects.IN;
using System.Collections;

namespace PX.Objects.Extensions.AddItemLookup
{
	public abstract class SiteStatusLookupExt<TGraph, TDocument, TLine, TItemInfo, TItemFilter> : AddItemLookupBaseExt<TGraph, TDocument, TItemInfo, TItemFilter>
		where TGraph : PXGraph
		where TDocument : class, IBqlTable, new()
		where TLine : class, IBqlTable, new()
		where TItemInfo : class, IQtySelectable, IPXSelectable, IBqlTable, new()
		where TItemFilter : INSiteStatusFilter, IBqlTable, new()
	{
		protected readonly string QtySelected = nameof(IQtySelectable.QtySelected);

		public override void Initialize()
		{
			Base.FieldUpdated.AddHandler(typeof(TItemInfo), nameof(IPXSelectable.Selected), OnSelectedUpdated);

			base.Initialize();
		}

		protected override IEnumerable ShowItemsHandler(PXAdapter adapter)
		{
			if (ItemInfo.AskExt((g, viewName) => ItemFilter.Cache.Clear()) == WebDialogResult.OK)
			{
				return AddSelectedItems(adapter);
			}
			ItemFilter.Cache.Clear();
			ItemInfo.Cache.Clear();
			return adapter.Get();
		}

		protected override IEnumerable AddSelectedItemsHandler(PXAdapter adapter)
		{
			var lines = Base.Caches<TLine>();
			lines.ForceExceptionHandling = true;

			foreach (TItemInfo line in ItemInfo.Cache.Updated)
			{
				if (LineShallBeAdded(line))
				{
					CreateNewLine(line);
				}
			}

			ItemInfo.Cache.Clear();
			return adapter.Get();
		}

		protected override TItemInfo SetValuesOfSelectedRow(TItemInfo updatedItem, TItemInfo originalItem)
		{
			decimal? qty = updatedItem.QtySelected;
			ItemInfo.Cache.RestoreCopy(updatedItem, originalItem);
			updatedItem.Selected = true;
			updatedItem.QtySelected = qty;
			return updatedItem;
		}

		protected virtual bool LineShallBeAdded(TItemInfo line)
		{
			var cache = Base.Caches<TItemInfo>();
			bool? selected = line.Selected;
			decimal qtySelected = line.QtySelected ?? 0m;

			return selected == true && qtySelected > 0m;
		}

		protected virtual void OnSelectedUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			bool? selected = (bool?)sender.GetValue(e.Row, Selected);
			decimal? qty = (decimal?)sender.GetValue(e.Row, QtySelected);
			if (selected == true)
			{
				if (qty == null || qty == 0m)
					sender.SetValue(e.Row, QtySelected, 1m);
			}
			else
				sender.SetValue(e.Row, QtySelected, 0m);
		}

		protected abstract TLine CreateNewLine(TItemInfo line);

		protected override void _(Events.RowSelected<TItemInfo> e)
		{
			base._(e);
			PXUIFieldAttribute.SetEnabled(e.Cache, e.Row, QtySelected, true);
		}
	}
}
