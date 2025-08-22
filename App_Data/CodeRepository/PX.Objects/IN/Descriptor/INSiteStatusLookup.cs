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
using PX.Objects.CS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.IN
{
	// The INSiteStatusLookup class has been moved to this file from "WebSites/Pure/PX.Objects/IN/Descriptor/Attribute.cs".
	// All changes made to this class must be duplicated in the AddItemLookupBaseExt class.
	[Obsolete("This class is obsolete. Use AddItemLookupBaseExt instead.")]
	public class INSiteStatusLookup<Status, StatusFilter> : PXSelectBase<Status>
		where Status : class, IBqlTable, new()
		where StatusFilter : class, IBqlTable, new()
	{
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
							return new PXSearchColumn("InventoryCD", orig.Descending, orig.OrigSearchValue ?? orig.SearchValue);
						else
							return null;
					case "subitemid":
						return new PXSearchColumn("SubItemCD", orig.Descending, orig.OrigSearchValue ?? orig.SearchValue);
					case "siteid":
						return new PXSearchColumn("SiteCD", orig.Descending, orig.OrigSearchValue ?? orig.SearchValue);
					case "locationid":
						return new PXSearchColumn("LocationCD", orig.Descending, orig.OrigSearchValue ?? orig.SearchValue);
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

				var context = PXView._Executing.Peek();
				var orig = context.Sorts;

				bool idFound = false;
				var result = new List<PXSearchColumn>();
				const string iD = "InventoryCD";
				for (int i = 0; i < orig.Length - this.Cache.Keys.Count; i++)
				{
					result.Add(CorrectFieldName(orig[i], false));
					if (orig[i].Column == iD)
						idFound = true;
				}

				for (int i = orig.Length - this.Cache.Keys.Count; i < orig.Length; i++)
				{
					var col = CorrectFieldName(orig[i], idFound);
					if (col != null)
						result.Add(col);
				}
				context.Sorts = result.ToArray();

				return base.InvokeDelegate(parameters);
			}
		}

		public const string Selected = "Selected";
		public const string QtySelected = "QtySelected";
		private PXView intView;
		#region Ctor
		public INSiteStatusLookup(PXGraph graph)
		{
			this.View = new PXView(graph, false,
				BqlCommand.CreateInstance(BqlCommand.Compose(typeof(Select<>), typeof(Status))),
				new PXSelectDelegate(viewHandler));
			InitHandlers(graph);
		}

		public INSiteStatusLookup(PXGraph graph, Delegate handler)
		{
			this.View = new PXView(graph, false,
				BqlCommand.CreateInstance(typeof(Select<>), typeof(Status)),
				handler);
			InitHandlers(graph);
		}
		#endregion

		#region Implementations

		private void InitHandlers(PXGraph graph)
		{
			graph.RowSelected.AddHandler(typeof(StatusFilter), OnFilterSelected);
			graph.RowSelected.AddHandler(typeof(Status), OnRowSelected);
			graph.FieldUpdated.AddHandler(typeof(Status), Selected, OnSelectedUpdated);
		}

		protected virtual void OnFilterSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			INSiteStatusFilter row = e.Row as INSiteStatusFilter;
			if (row != null)
				PXUIFieldAttribute.SetVisible(sender.Graph.Caches[typeof(Status)], typeof(INSiteStatusByCostCenter.siteID).Name, row.SiteID == null);
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

		protected virtual void OnRowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
			PXUIFieldAttribute.SetEnabled(sender, e.Row, Selected, true);
			PXUIFieldAttribute.SetEnabled(sender, e.Row, QtySelected, true);
		}

		protected virtual PXView CreateIntView(PXGraph graph)
		{
			PXCache cache = graph.Caches[typeof(Status)];

			List<Type> select = new List<Type>();
			select.Add(typeof(Select<,>));
			select.Add(typeof(Status));
			select.Add(CreateWhere(graph));

			//select.Add(typeof(Aggregate<>));
			/*
			List<Type> groupFields = cache.BqlKeys;
			groupFields.AddRange(cache.BqlFields.Where(field => field.IsDefined(typeof (PXExtraKeyAttribute), false)));			

			for (int i = 0; i < groupFields.Count; i++)
			{
				select.Add((i != groupFields.Count - 1) ? typeof(GroupBy<,>) : typeof(GroupBy<>));
				select.Add(groupFields[i]);
			}
			*/
			Type selectType = BqlCommand.Compose(select.ToArray());

			return new LookupView(graph, BqlCommand.CreateInstance(selectType));
		}

		protected virtual IEnumerable viewHandler()
		{
			if (intView == null) intView = CreateIntView(this.View.Graph);
			var startRow = PXView.StartRow;
			var totalRows = 0;

			var rows = new PXDelegateResult();

			foreach (var rec in intView.Select(null, null, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters,
								 ref startRow, PXView.MaximumRows, ref totalRows))
			{
				Status item = PXResult.Unwrap<Status>(rec);
				Status result = item;
				Status updated = this.Cache.Locate(item) as Status;
				if (updated != null && this.Cache.GetValue(updated, Selected) as bool? == true)
				{
					Decimal? qty = this.Cache.GetValue(updated, QtySelected) as Decimal?;
					this.Cache.RestoreCopy(updated, item);
					this.Cache.SetValue(updated, Selected, true);
					this.Cache.SetValue(updated, QtySelected, qty);
					result = updated;
				}
				rows.Add(result);
			}
			PXView.StartRow = 0;

			if (PXView.ReverseOrder)
				rows.Reverse();

			rows.IsResultSorted = true;

			return rows;
		}

		protected static Type CreateWhere(PXGraph graph)
		{
			PXCache filter = graph.Caches[typeof(INSiteStatusFilter)];
			PXCache cache = graph.Caches[typeof(Status)];

			Type where = typeof(Where<boolTrue, Equal<boolTrue>>);
			foreach (string field in filter.Fields)
			{
				if (cache.Fields.Contains(field))
				{
					if (filter.Fields.Contains(field + "Wildcard")) continue;
					if (field.Contains("SubItem") && !PXAccess.FeatureInstalled<FeaturesSet.subItem>()) continue;
					if (field.Contains("Site") && !PXAccess.FeatureInstalled<FeaturesSet.warehouse>()) continue;
					if (field.Contains("Location") && !PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>()) continue;
					Type sourceType = filter.GetBqlField(field);
					Type destinationType = cache.GetBqlField(field);
					if (sourceType != null && destinationType != null)
					{
						where = BqlCommand.Compose(
							typeof(Where2<,>),
							typeof(Where<,,>),
							typeof(Current<>), sourceType, typeof(IsNull),
							typeof(Or<,>), destinationType, typeof(Equal<>), typeof(Current<>), sourceType,
							typeof(And<>), where
						);
					}
				}
				string f;
				if (field.Length > 8 && field.EndsWith("Wildcard") && cache.Fields.Contains(f = field.Substring(0, field.Length - 8)))
				{
					if (field.Contains("SubItem") && !PXAccess.FeatureInstalled<FeaturesSet.subItem>()) continue;
					if (field.Contains("Site") && !PXAccess.FeatureInstalled<FeaturesSet.warehouse>()) continue;
					if (field.Contains("Location") && !PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>()) continue;
					Type like = filter.GetBqlField(field);
					Type dest = cache.GetBqlField(f);
					where = BqlCommand.Compose(
						typeof(Where2<,>),
						typeof(Where<,,>), typeof(Current<>), like, typeof(IsNull),
						typeof(Or<,>), dest, typeof(Like<>), typeof(Current<>), like,
						typeof(And<>), where
						);
				}
			}
			return where;
		}

		protected static Type GetTypeField<Source>(string field)
		{
			Type sourceType = typeof(Source);
			Type fieldType = null;
			while (fieldType == null && sourceType != null)
			{
				fieldType = sourceType.GetNestedType(field, System.Reflection.BindingFlags.Public);
				sourceType = sourceType.BaseType;
			}
			return fieldType;
		}

		private class Zero : PX.Data.BQL.BqlDecimal.Constant<Zero>
		{
			public Zero() : base(0m) { }
		}
		#endregion
	}
}
