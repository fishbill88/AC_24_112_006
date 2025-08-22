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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.Api.Export;
using PX.Web.UI;

namespace PX.Objects.IN
{
    [NonOptimizable(IgnoreOptimizationBehavior = true)]
    public class INCategoryMaint : PXGraph<INCategoryMaint>
    {
        public static class AddItemsTypesList
        {
            public class ListAttribute : PXStringListAttribute
            {
				public ListAttribute() : base(
					new[]
					{
						Pair(AddAllItems, "All Items"),
						Pair(AddItemsByClass, "By Class"),
					}) {}
            }

            public const string AddAllItems = "A";
            public const string AddItemsByClass = "I";
        }

        #region DAC
        public class ClassFilter : PXBqlTable, IBqlTable
        {
            #region PriceBasis
            [PXString(1, IsFixed = true)]
            [PXUIField(DisplayName = ActionsMessages.AddItems)]
            [AddItemsTypesList.List()]
            [PXDefault(AddItemsTypesList.AddItemsByClass)]
            public virtual String AddItemsTypes { get; set; }
            public abstract class addItemsTypes : BqlString.Field<addItemsTypes> { }
            #endregion
            #region ItemClassID
            [PXInt]
            [PXUIField(DisplayName = "Item Class", Visibility = PXUIVisibility.SelectorVisible)]
            [PXDimensionSelector(INItemClass.Dimension, typeof(INItemClass.itemClassID), typeof(INItemClass.itemClassCD), DescriptionField = typeof(INItemClass.descr), ValidComboRequired = true)]
            public virtual int? ItemClassID { get; set; }
            public abstract class itemClassID : BqlInt.Field<itemClassID> { }
            #endregion
		}

        [PXHidden]
        public class SelectedNode : PXBqlTable, IBqlTable
        {
            #region FolderID
            [PXInt()]
            [PXUIField(Visible = false)]
            public virtual int? FolderID { get; set; }
            public abstract class folderID : BqlInt.Field<folderID> { }
            #endregion
            #region CategoryID
            [PXInt()]
            [PXUIField(Visible = false)]
            public virtual int? CategoryID { get; set; }
            public abstract class categoryID : BqlInt.Field<categoryID> { }
            #endregion
        }

        public class INFolderCategory : INCategory { }

        public class INCategoryCurrent : INCategory { }

        #endregion

        #region Views
        public PXFilter<ClassFilter> ClassInfo;
        public PXFilter<SelectedNode> SelectedFolders;

        public
            PXSelectOrderBy<INCategory,
            OrderBy<Asc<INCategory.sortOrder>>>
            Folders;
        protected virtual IEnumerable folders([PXInt] int? categoryID) => GetFolders(categoryID);

        public
            PXSelect<INCategory,
            Where<INCategory.categoryID, Equal<Current<INCategory.categoryID>>>>
            CurrentCategory;
        protected virtual IEnumerable currentCategory() => GetCurrentCategory();

        public
            PXSelectJoin<INItemCategory,
            LeftJoin<InventoryItem, On<INItemCategory.FK.InventoryItem>>,
            Where<INItemCategory.categoryID, Equal<Current<INCategory.categoryID>>>>
            Members;
        protected virtual IEnumerable members() => GetMembers();

        public
            PXSelectOrderBy<INFolderCategory,
            OrderBy<Asc<INFolderCategory.sortOrder>>>
            ParentFolders;
        protected virtual IEnumerable parentFolders([PXInt] int? categoryID) => GetParentFolders(categoryID);

        public PXFilter<INItemCategoryBuffer> Buffer;

        [PXHidden]
        public
            PXSelect<InventoryItem,
            Where<InventoryItem.inventoryID, Equal<Optional<INItemCategory.inventoryID>>>>
            RelatedInventoryItem;
        #endregion

        #region Delegates
        private IEnumerable<INCategory> GetFolders(int? categoryID)
        {
            if (categoryID == null)
            {
                yield return new INCategory
                {
                    CategoryID = 0,
                    Description = PXSiteMap.RootNode.Title
                };
            }

	        foreach (INCategory item in CategoryCache<INCategory>.GetChildren(this, categoryID))
	        {
		        if (!string.IsNullOrEmpty(item.Description))
			        yield return item;
	        }
        }

        private IEnumerable<INCategory> GetCurrentCategory()
        {
            if (Folders.Current != null)
            {
                PXUIFieldAttribute.SetEnabled<INCategory.description>(Caches[typeof(INCategory)], null, Folders.Current.ParentID != null);
                PXUIFieldAttribute.SetEnabled<INCategory.parentID>(Caches[typeof(INCategory)], null, Folders.Current.ParentID != null);
                Caches[typeof(INItemCategory)].AllowInsert = Folders.Current.ParentID != null;
                Caches[typeof(INItemCategory)].AllowDelete = Folders.Current.ParentID != null;
                Caches[typeof(INItemCategory)].AllowUpdate = Folders.Current.ParentID != null;
                Actions["Copy"].SetEnabled(Folders.Current.ParentID != null);
                Actions["Cut"].SetEnabled(Folders.Current.ParentID != null);
                Actions["Paste"].SetEnabled(Folders.Current.ParentID != null);
                Actions["AddItemsbyClass"].SetEnabled(Folders.Current.ParentID != null);

                foreach (INCategory item in
                    PXSelect<INCategory,
                    Where<INCategory.categoryID, Equal<Required<INCategory.categoryID>>>>
                    .Select(this, Folders.Current.CategoryID))
                {
                    yield return item;
                }
            }
        }

        private IEnumerable GetMembers()
        {
            if (Folders.Current != null)
            {
                PXUIFieldAttribute.SetEnabled<INCategory.description>(Caches[typeof(INCategory)], null, Folders.Current.ParentID != null);
                PXUIFieldAttribute.SetEnabled<INCategory.parentID>(Caches[typeof(INCategory)], null, Folders.Current.ParentID != null);
                Caches[typeof(INItemCategory)].AllowInsert = Folders.Current.ParentID != null;
                Caches[typeof(INItemCategory)].AllowUpdate = Folders.Current.ParentID != null;
                Actions["Copy"].SetEnabled(Folders.Current.ParentID != null);
                Actions["Cut"].SetEnabled(Folders.Current.ParentID != null);
                Actions["Paste"].SetEnabled(Folders.Current.ParentID != null);
                Actions["AddItemsbyClass"].SetEnabled(Folders.Current.ParentID != null);

                PXSelectBase<INItemCategory> cmd = new
                    PXSelectJoin<INItemCategory,
                    LeftJoin<InventoryItem, On<INItemCategory.FK.InventoryItem>>,
                    Where<INItemCategory.categoryID, Equal<Required<INCategory.categoryID>>>>
                    (this);

                int startRow = PXView.StartRow;
                int totalRows = 0;

                foreach (var res in cmd.View.Select(PXView.Currents, new object[] { Folders.Current.CategoryID }, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows))
                {
                    yield return res;
                    PXView.StartRow = 0;
                }
            }
        }

        private IEnumerable<INFolderCategory> GetParentFolders(int? categoryID)
        {
            if (categoryID == null)
            {
                yield return new INFolderCategory
                {
                    CategoryID = 0,
                    Description = PXSiteMap.RootNode.Title
                };
            }

            foreach (INFolderCategory item in CategoryCache<INFolderCategory>.GetChildren(this, categoryID))
                if (!string.IsNullOrEmpty(item.Description) && item.CategoryID != Folders.Current.CategoryID)
                    yield return item;
        }

		protected class CategoryCache<TCategory> : IPrefetchable<PXGraph>
			where TCategory : INCategory, new()
		{
			private ILookup<int?, TCategory> _lookup;

			public void Prefetch(PXGraph parameter)
			{
				_lookup =
					PXSelectOrderBy<TCategory, OrderBy<Asc<INCategory.parentID, Asc<INCategory.sortOrder>>>>
						.Select(parameter)
						.RowCast<TCategory>()
						.ToLookup(r => r.ParentID);
			}

			private static CategoryCache<TCategory> GetInstance(PXGraph graph)
			{
				return PXDatabase.GetLocalizableSlot<CategoryCache<TCategory>, PXGraph>(typeof(CategoryCache<TCategory>).FullName, graph, typeof(INCategory));
			}

			public static IEnumerable<TCategory> GetChildren(PXGraph graph, int? categoryID)
			{
				CategoryCache<TCategory> instance = GetInstance(graph);
				TCategory[] children = instance._lookup[categoryID].ToArray();

				var cache = graph.Caches<TCategory>();
				var cached = cache.Rows.Cached.Where(c => c.ParentID == categoryID && cache.GetStatus(c).IsIn(PXEntryStatus.Inserted, PXEntryStatus.Updated, PXEntryStatus.Deleted, PXEntryStatus.Modified)).ToArray();
				if (cached.Length > 0)
				{
					var untouchedShared = children.Except(cached, cache.GetComparer()).ToArray();
					var aliveLocal = cached.Where(c => cache.GetStatus(c) != PXEntryStatus.Deleted).ToArray();
					children = untouchedShared.Concat(aliveLocal).ToArray();
				}

				for (int i = 0; i < children.Length; i++)
				{
					var item = cache.Locate(children[i]);
					if (item != null)
						children[i] = item;
				}

				return children;
			}

			public static void Clear(PXGraph graph) => GetInstance(graph).Prefetch(graph);
		}
        #endregion

        #region Actions
        public PXSave<SelectedNode> Save;
        public PXCancel<SelectedNode> Cancel;

        public PXAction<SelectedNode> AddCategory;
        [PXUIField(DisplayName = " ", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton]
        public virtual IEnumerable addCategory(PXAdapter adapter)
        {
            int ParentID = Folders.Current.CategoryID.Value;
            var inserted = this.Caches<INCategory>().Insert(new INCategory
            {
                Description = PXMessages.LocalizeNoPrefix(Messages.NewKey),
                ParentID = ParentID,
            });
            inserted.TempChildID = inserted.CategoryID;
            inserted.TempParentID = ParentID;
            INCategory previous =
                PXSelect<INCategory,
                Where<INCategory.parentID, Equal<Required<INCategory.parentID>>>,
                OrderBy<Desc<INCategory.sortOrder>>>
                .SelectSingleBound(this, null, ParentID);

            int sortOrder = previous.SortOrder.Value;
            sortOrder++;
            inserted.SortOrder = previous != null ? sortOrder : 1;
            Folders.Cache.ActiveRow = inserted;
            return adapter.Get();
        }

        public PXAction<SelectedNode> DeleteCategory;
        [PXUIField(DisplayName = " ", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton]
        public virtual IEnumerable deleteCategory(PXAdapter adapter)
        {
            // ToDo recursive delete 
            this.Caches<INCategory>().Delete(Folders.Current);
            return adapter.Get();
        }

        public PXAction<SelectedNode> down;
        [PXUIField(DisplayName = " ", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton(ImageUrl = "~/Icons/NavBar/xps_collapse.gif", DisabledImageUrl = "~/Icons/NavBar/xps_collapseD.gif")]
        public virtual IEnumerable Down(PXAdapter adapter)
        {
            INCategory curr = Folders.Current;
            INCategory next =
                PXSelect<INCategory,
                Where<
                    INCategory.parentID, Equal<Required<INCategory.parentID>>,
                    And<INCategory.sortOrder, Greater<Required<INCategory.parentID>>>>,
                OrderBy<Asc<INCategory.sortOrder>>>
                .SelectSingleBound(this, null, Folders.Current.ParentID, Folders.Current.SortOrder);

            if (next != null && curr != null)
            {
                int temp = curr.SortOrder.Value;
                curr.SortOrder = next.SortOrder;
                next.SortOrder = temp;
                this.Caches<INCategory>().Update(next);
                this.Caches<INCategory>().Update(curr);
            }

            return adapter.Get();
        }

        public PXAction<SelectedNode> up;
        [PXUIField(DisplayName = " ", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton(ImageUrl = "~/Icons/NavBar/xps_expand.gif", DisabledImageUrl = "~/Icons/NavBar/xps_expandD.gif")]
        public virtual IEnumerable Up(PXAdapter adapter)
        {
            INCategory curr = Folders.Current;
            INCategory prev =
                PXSelect<INCategory,
                Where<
                    INCategory.parentID, Equal<Required<INCategory.parentID>>,
                    And<INCategory.sortOrder, Less<Required<INCategory.parentID>>>>,
                OrderBy<Desc<INCategory.sortOrder>>>
                .SelectSingleBound(this, null, Folders.Current.ParentID, Folders.Current.SortOrder);

            if (prev != null && curr != null)
            {
                int temp = curr.SortOrder.Value;
                curr.SortOrder = prev.SortOrder;
                prev.SortOrder = temp;
                this.Caches<INCategory>().Update(prev);
                this.Caches<INCategory>().Update(curr);
            }

            return adapter.Get();
        }

        public PXAction<SelectedNode> Copy;
        [PXButton(ImageKey = Sprite.Main.Copy, Tooltip = ActionsMessages.ttipCopyRec)]
        [PXUIField(DisplayName = ActionsMessages.CopyRec, Enabled = false)]
        public IEnumerable copy(PXAdapter adapter)
        {
            Buffer.Cache.Clear();
            foreach (INItemCategory pxResult in
                PXSelect<INItemCategory,
                Where<INItemCategory.categoryID, Equal<Required<INItemCategory.categoryID>>>>
                .Select(this, Folders.Current.CategoryID))
            {
                if (pxResult.CategorySelected == true)
                {
                    INItemCategoryBuffer insertnode = (INItemCategoryBuffer)Buffer.Cache.CreateInstance();
                    insertnode.InventoryID = pxResult.InventoryID;
                    Buffer.Insert(insertnode);
                }
            }
            return adapter.Get();
        }

        public PXAction<SelectedNode> Cut;
        [PXButton(ImageKey = Sprite.Main.Cut, Tooltip = ActionsMessages.ttipCut)]
        [PXUIField(DisplayName = ActionsMessages.Cut, Enabled = false)]
        internal IEnumerable cut(PXAdapter adapter)
        {
            Buffer.Cache.Clear();
            var delbuffer = new List<INItemCategory>();
            foreach (INItemCategory pxResult in
                PXSelect<INItemCategory,
                Where<INItemCategory.categoryID, Equal<Required<INItemCategory.categoryID>>>,
                OrderBy<Asc<InventoryItem.inventoryCD>>>
                .Select(this, Folders.Current.CategoryID))
            {
                if (pxResult.CategorySelected == true)
                {
                    INItemCategoryBuffer insertnode = (INItemCategoryBuffer)Buffer.Cache.CreateInstance();
                    insertnode.InventoryID = pxResult.InventoryID;
                    Buffer.Insert(insertnode);
                    delbuffer.Add(pxResult);
                }
            }

            foreach (INItemCategory pxResult in delbuffer)
            {
                Members.Delete(pxResult);
            }

            return adapter.Get();
        }

        public PXAction<SelectedNode> Paste;
        [PXButton(ImageKey = Sprite.Main.Paste, Tooltip = ActionsMessages.ttipPaste)]
        [PXUIField(DisplayName = ActionsMessages.Paste, Enabled = false)]
        internal IEnumerable paste(PXAdapter adapter)
        {
            foreach (INItemCategoryBuffer pxResult in Buffer.Cache.Cached)
            {
                INItemCategory insertnode = (INItemCategory)Members.Cache.CreateInstance();
                insertnode.InventoryID = pxResult.InventoryID;
                Members.Insert(insertnode);
            }

            return adapter.Get();
        }

        public PXAction<SelectedNode> AddItemsbyClass;
        [PXButton(Tooltip = ActionsMessages.AddItems)]
        [PXUIField(DisplayName = ActionsMessages.AddItems, Enabled = false)]
        internal IEnumerable addItemsbyClass(PXAdapter adapter)
        {
            if (ClassInfo.AskExt() == WebDialogResult.OK)
            {
				// place all members into the cache, so that PXCache.Insert could work correctly
				var membersView = new PXView(this, false, Members.View.BqlSelect);
				var members = membersView.SelectMulti();

				var selectItemsForAddingCmd = new
					PXSelectReadonly<InventoryItem,
					Where<
						InventoryItem.itemStatus, NotEqual<InventoryItemStatus.unknown>,
						And<InventoryItem.isTemplate, Equal<False>>>>
					(this);
				if (ClassInfo.Current.AddItemsTypes == AddItemsTypesList.AddItemsByClass)
				{
					selectItemsForAddingCmd.WhereAnd<Where<InventoryItem.itemClassID, Equal<Required<InventoryItem.itemClassID>>>>();
				}

				foreach (InventoryItem pxResult in selectItemsForAddingCmd.Select(ClassInfo.Current.ItemClassID))
				{
					INItemCategory insertnode = (INItemCategory)Members.Cache.CreateInstance();
					insertnode.InventoryID = pxResult.InventoryID;
					insertnode.CategorySelected = false;
					var member = Members.Insert(insertnode);
				}
            }
            Actions.PressSave();
            return adapter.Get();
        }

        public PXAction<SelectedNode> ViewDetails;
        [PXButton()]
        [PXUIField(DisplayName = "Inventory Details", Visible = false)]
        public virtual IEnumerable viewDetails(PXAdapter adapter)
        {
            if (Members.Current != null)
            {
                InventoryItem inventoryItem = InventoryItem.PK.Find(this, Members.Current.InventoryID);

                if (inventoryItem != null)
                {
					InventoryItemMaintBase graph = (inventoryItem.StkItem == true)
						? (InventoryItemMaintBase)PXGraph.CreateInstance<InventoryItemMaint>()
						: PXGraph.CreateInstance<NonStockItemMaint>();
					graph.Item.Current = graph.Item.Search<InventoryItem.inventoryID>(inventoryItem.InventoryID);
					PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.Same);
                }
            }
            return adapter.Get();
        }
        #endregion

        #region Event Handler
		#region INCategory
		protected virtual void _(Events.RowInserting<INCategory> e)
		{
			if (e.Row == null || e.Row.CategoryID == null || e.Row.ParentID == null || e.Row.Description == null)
				return;

			if (GetNamesakeInFolderFor(e.Row) != null)
			{
				e.Cancel = true;
				e.Cache.RaiseExceptionHandling<INCategory.description>(e.Row, e.Row.Description, GetExistingNamesakeExceptionFor(e.Row));
			}
		}

		protected virtual void _(Events.RowUpdating<INCategory> e)
		{
			if (e.NewRow == null || e.NewRow.CategoryID == null || e.NewRow.ParentID == null || e.NewRow.Description == null)
				return;

			if (GetNamesakeInFolderFor(e.NewRow) != null)
			{
				e.Cancel = true;

				if (e.Row.ParentID != e.NewRow.ParentID)
					e.Cache.RaiseExceptionHandling<INCategory.parentID>(e.NewRow, e.NewRow.ParentID, GetExistingNamesakeExceptionFor(e.NewRow));
				else
					e.Cache.RaiseExceptionHandling<INCategory.description>(e.NewRow, e.NewRow.Description, GetExistingNamesakeExceptionFor(e.NewRow));
			}
		}

        protected virtual void INCategory_RowDeleted(PXCache cache, PXRowDeletedEventArgs e)
        {
            INCategory row = e.Row as INCategory;
            if (row?.CategoryID == null) return;
            deleteRecurring(row);
        }
		#endregion
		#region INItemCategory
		protected virtual void INItemCategory_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			INItemCategory row = (INItemCategory)e.Row;
			if (row != null)
			{
				PXDefaultAttribute.SetDefault<INItemCategory.categorySelected>(cache, row, false);
			}
		}

		protected virtual void _(Events.RowInserted<INItemCategory> eventArgs)
		{
			if (eventArgs.Row?.InventoryID == null)
				return;

			var inventory = RelatedInventoryItem.SelectSingle();
			RelatedInventoryItem.Cache.MarkUpdated(inventory, assertError: true);
		}

		protected virtual void _(Events.RowUpdated<INItemCategory> eventArgs)
		{
			int? oldInventoryID = eventArgs.OldRow?.InventoryID;
			if (eventArgs.Row == null || eventArgs.Row.InventoryID == oldInventoryID)
				return;

			if (eventArgs.Row.InventoryID != null)
			{
				var inventory = RelatedInventoryItem.SelectSingle();
				RelatedInventoryItem.Cache.MarkUpdated(inventory, assertError: true);
			}
			if (oldInventoryID != null)
			{
				var inventory = RelatedInventoryItem.SelectSingle(oldInventoryID);
				RelatedInventoryItem.Cache.MarkUpdated(inventory, assertError: true);
			}
		}

		protected virtual void _(Events.RowDeleted<INItemCategory> eventArgs)
		{
			if (eventArgs.Row?.InventoryID == null)
				return;

			var inventory = RelatedInventoryItem.SelectSingle();
			RelatedInventoryItem.Cache.MarkUpdated(inventory, assertError: true);
		}
		#endregion
		#region ClassFilter
		protected virtual void ClassFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			ClassFilter row = (ClassFilter)e.Row;
			if (row == null) return;
			if (row.AddItemsTypes == AddItemsTypesList.AddAllItems)
			{
				row.ItemClassID = null;
			}
			PXUIFieldAttribute.SetEnabled<ClassFilter.itemClassID>(cache, row, row.AddItemsTypes != AddItemsTypesList.AddAllItems);
		}
		#endregion
		#endregion

		public override void Persist()
		{
			Buffer.Cache.Clear();
			base.Persist();
			foreach (INCategory item in this.Caches<INCategory>().Rows.Cached)
			{
				if (item.TempParentID < 0)
				{
					foreach (INCategory item2 in this.Caches<INCategory>().Rows.Cached)
					{
						if (item2.TempChildID == item.TempParentID)
						{
							item.ParentID = item2.CategoryID;
							item.TempParentID = item2.CategoryID;
							this.Caches<INCategory>().MarkUpdated(item, assertError: true);
						}
					}
				}
			}
			base.Persist();
			Members.View.RequestRefresh();
			CategoryCache<INCategory>.Clear(this);
			CategoryCache<INFolderCategory>.Clear(this);
		}

		private void deleteRecurring(INCategory map)
		{
			if (map != null)
			{
				foreach (INCategory child in PXSelect<INCategory, Where<INCategory.parentID, Equal<Required<INCategory.categoryID>>>>.Select(this, map.CategoryID))
					deleteRecurring(child);

				this.Caches<INCategory>().Delete(map);
			}
		}

		private INCategory GetNamesakeInFolderFor(INCategory category)
		{
			return
				SelectFrom<INCategory>.
				Where<
					INCategory.parentID.IsEqual<INCategory.parentID.FromCurrent>.
					And<INCategory.categoryID.IsNotEqual<INCategory.categoryID.FromCurrent>>.
					And<INCategory.description.IsEqual<INCategory.description.FromCurrent>>>.
				View.SelectSingleBound(this, new[] { category });
		}

		private PXException GetExistingNamesakeExceptionFor(INCategory category)
		{
			return new PXSetPropertyException(
				Msg.ExistingNamesakeError,
				PXErrorLevel.Error,
				GetParentDescriptionFor(category),
				category.Description);
		}

		private string GetParentDescriptionFor(INCategory category)
		{
			return category.ParentID == 0
				? PXSiteMap.RootNode.Title
				: SelectFrom<INCategory>.Where<INCategory.categoryID.IsEqual<@P.AsInt>>.View.Select(this, category.ParentID).Single().GetItem<INCategory>().Description;
		}

		[PXLocalizable]
		public static class Msg
		{
			public const string ExistingNamesakeError = "The {0} category already contains the {1} subcategory.";
		}
	}
}
