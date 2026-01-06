using System;
using System.Linq;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.CS;

namespace ACustom
{
    /// <summary>
    /// Extension for INItemClassMaint to implement hierarchy validation and populate hierarchy fields
    /// </summary>
    public class INItemClassMaintExt : PXGraphExtension<INItemClassMaint>
    {
        #region Event Handlers

        /// <summary>
        /// Validate circular references when ParentItemClassID is changed
        /// </summary>
        protected virtual void _(Events.FieldVerifying<INItemClass, INItemClass.parentItemClassID> e)
        {
            if (e.Row == null || e.NewValue == null)
                return;

            var newParentID = (int?)e.NewValue;
            var currentItemClassID = e.Row.ItemClassID;

            // Validate circular reference
            if (!ItemClassHierarchyHelper.ValidateCircularReference(Base, currentItemClassID, newParentID))
            {
                throw new PXSetPropertyException(ACustom.Messages.CircularReferenceError, PXErrorLevel.Error);
            }

            // Validate hierarchy depth
            var parentDepth = ItemClassHierarchyHelper.GetHierarchyDepth(Base, newParentID);
            if (parentDepth >= 9) // Max depth is 10, so parent can be at most 9
            {
                throw new PXSetPropertyException(ACustom.Messages.MaxHierarchyDepthError, PXErrorLevel.Error);
            }

            // Check if parent has same stock type
            var parentClass = PXSelect<INItemClass,
                Where<INItemClass.itemClassID, Equal<Required<INItemClass.itemClassID>>>>
                .Select(Base, newParentID)
                .FirstOrDefault()?.GetItem<INItemClass>();

            if (parentClass != null && parentClass.StkItem != e.Row.StkItem)
            {
                Base.itemclass.Cache.RaiseExceptionHandling<INItemClass.parentItemClassID>(
                    e.Row,
                    e.NewValue,
                    new PXSetPropertyException(ACustom.Messages.DifferentStockItemWarning, PXErrorLevel.Warning));
            }
        }

        /// <summary>
        /// Update hierarchy display fields when item class is selected
        /// </summary>
        protected virtual void _(Events.RowSelected<INItemClass> e)
        {
            if (e.Row == null)
                return;

            var ext = e.Row.GetExtension<INItemClassExt>();
            if (ext == null)
                return;

            // Calculate and populate hierarchy fields
            ext.UsrHierarchyLevel = ItemClassHierarchyHelper.GetHierarchyDepth(Base, e.Row.ItemClassID);
            ext.UsrFullHierarchyPath = ItemClassHierarchyHelper.GetHierarchyPath(Base, e.Row.ItemClassID);
            ext.UsrHasChildren = ItemClassHierarchyHelper.HasChildClasses(Base, e.Row.ItemClassID);

            // Count cascading attributes
            var cascadingAttributes = ItemClassHierarchyHelper.GetCascadingAttributes(Base, e.Row.ItemClassID);
            ext.UsrAttributeCount = cascadingAttributes.Count;
        }

        /// <summary>
        /// Warn user when changing parent of an item class that has existing inventory items
        /// </summary>
        protected virtual void _(Events.FieldUpdated<INItemClass, INItemClass.parentItemClassID> e)
        {
            if (e.Row == null)
                return;

            // Check if this item class is used by any inventory items
            var itemCount = PXSelect<InventoryItem,
                Where<InventoryItem.itemClassID, Equal<Required<InventoryItem.itemClassID>>>>
                .Select(Base, e.Row.ItemClassID)
                .Count;

            if (itemCount > 0)
            {
                Base.itemclass.Cache.RaiseExceptionHandling<INItemClass.parentItemClassID>(
                    e.Row,
                    e.Row.ParentItemClassID,
                    new PXSetPropertyException(
                        string.Format(ACustom.Messages.ItemClassUsedByItemsWarning, itemCount),
                        PXErrorLevel.Warning));
            }

            // Refresh hierarchy display fields
            var ext = e.Row.GetExtension<INItemClassExt>();
            if (ext != null)
            {
                ext.UsrHierarchyLevel = ItemClassHierarchyHelper.GetHierarchyDepth(Base, e.Row.ItemClassID);
                ext.UsrFullHierarchyPath = ItemClassHierarchyHelper.GetHierarchyPath(Base, e.Row.ItemClassID);
            }
        }

        /// <summary>
        /// Validate before persisting to ensure no circular references were created
        /// </summary>
        protected virtual void _(Events.RowPersisting<INItemClass> e)
        {
            if (e.Row == null || e.Operation == PXDBOperation.Delete)
                return;

            // Final validation before save
            if (e.Row.ParentItemClassID != null)
            {
                if (!ItemClassHierarchyHelper.ValidateCircularReference(Base, e.Row.ItemClassID, e.Row.ParentItemClassID))
                {
                    throw new PXException(ACustom.Messages.CircularReferenceOnSaveError);
                }
            }
        }

        /// <summary>
        /// Warn user when deleting an item class that has children
        /// </summary>
        protected virtual void _(Events.RowDeleting<INItemClass> e)
        {
            if (e.Row == null)
                return;

            // Check if this class has children
            if (ItemClassHierarchyHelper.HasChildClasses(Base, e.Row.ItemClassID))
            {
                var childCount = PXSelect<INItemClass,
                    Where<INItemClass.parentItemClassID, Equal<Required<INItemClass.parentItemClassID>>>>
                    .Select(Base, e.Row.ItemClassID)
                    .Count;

                if (Base.itemclass.Ask(
                    ACustom.Messages.DeleteItemClassWithChildrenTitle,
                    string.Format(ACustom.Messages.DeleteItemClassWithChildrenMessage, childCount),
                    MessageButtons.YesNo) != WebDialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion

        #region Actions

        /// <summary>
        /// Action to view the complete hierarchy tree for the current item class
        /// </summary>
        public PXAction<INItemClass> ViewHierarchy;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "View Hierarchy", MapEnableRights = PXCacheRights.Select)]
        protected virtual void viewHierarchy()
        {
            var current = Base.itemclass.Current;
            if (current == null)
                return;

            var hierarchy = ItemClassHierarchyHelper.GetItemClassHierarchy(Base, current.ItemClassID);
            
            if (hierarchy.Any())
            {
                var message = "Item Class Hierarchy (from current to root):\n\n";
                for (int i = 0; i < hierarchy.Count; i++)
                {
                    var indent = new string(' ', i * 2);
                    message += $"{indent}Level {i}: {hierarchy[i].ItemClassCD} - {hierarchy[i].Descr}\n";
                }

                var cascadingAttributes = ItemClassHierarchyHelper.GetCascadingAttributes(Base, current.ItemClassID);
                message += $"\n\nTotal Cascading Attributes: {cascadingAttributes.Count}\n";
                
                if (cascadingAttributes.Any())
                {
                    message += "\nAttributes (with priority):\n";
                    foreach (var attr in cascadingAttributes)
                    {
                        message += $"  • {attr.AttributeID} - {attr.Description}\n";
                    }
                }

                throw new PXException(message);
            }
        }

        /// <summary>
        /// Action to show all cascading attributes for the current item class
        /// </summary>
        public PXAction<INItemClass> ViewCascadingAttributes;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "View Cascading Attributes", MapEnableRights = PXCacheRights.Select)]
        protected virtual void viewCascadingAttributes()
        {
            var current = Base.itemclass.Current;
            if (current == null)
                return;

            var cascadingAttributes = ItemClassHierarchyHelper.GetCascadingAttributes(Base, current.ItemClassID);
            
            if (cascadingAttributes.Any())
            {
                var message = $"Cascading Attributes for {current.ItemClassCD}:\n\n";
                message += $"Total: {cascadingAttributes.Count} attributes\n\n";
                
                foreach (var attr in cascadingAttributes)
                {
                    message += $"• {attr.AttributeID}: {attr.Description}\n";
                    message += $"  Required: {(attr.Required == true ? "Yes" : "No")}\n";
                    if (!string.IsNullOrEmpty(attr.DefaultValue))
                        message += $"  Default: {attr.DefaultValue}\n";
                    message += "\n";
                }

                throw new PXException(message);
            }
            else
            {
                throw new PXException(string.Format(ACustom.Messages.NoAttributesFoundInHierarchy, current.ItemClassCD));
            }
        }

        #endregion
    }
}
