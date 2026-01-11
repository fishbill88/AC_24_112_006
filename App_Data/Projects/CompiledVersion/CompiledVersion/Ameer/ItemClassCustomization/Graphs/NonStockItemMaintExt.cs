using System;
using System.Linq;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.CS;

namespace ItemClassCustomization
{
    /// <summary>
    /// Extension for NonStockItemMaint (Non-Stock Items) to implement cascading attributes from item class hierarchy
    /// </summary>
    public class NonStockItemMaintExt : PXGraphExtension<NonStockItemMaint>
    {
        private bool itemClassChanged = false;

        #region Event Handlers

        /// <summary>
        /// Track when ItemClassID is changed
        /// </summary>
        protected virtual void _(Events.FieldUpdated<InventoryItem, InventoryItem.itemClassID> e)
        {
            if (e.Row == null)
                return;

            // Track that item class changed
            if (e.ExternalCall && e.OldValue != e.NewValue)
            {
                itemClassChanged = true;
            }
        }

        /// <summary>
        /// After row is updated, populate cascading attributes from the entire hierarchy
        /// This runs AFTER the base graph's FieldUpdated events and Answers.Cache.Clear()
        /// </summary>
        protected virtual void _(Events.RowUpdated<InventoryItem> e)
        {
            if (e.Row == null)
            {
                return;
            }

            if (e.Row.ItemClassID == null)
            {
                return;
            }

            // Only process if item class was changed by user
            if (!itemClassChanged)
            {
                return;
            }

            try
            {
                itemClassChanged = false; // Reset flag

                // Small delay to ensure base processing is complete
                System.Threading.Thread.Sleep(100);

                // Populate cascading attributes from hierarchy
                PopulateCascadingAttributes(e.Row);

                // Show information message
                var cascadingAttributes = ItemClassHierarchyHelper.GetCascadingAttributes(Base, e.Row.ItemClassID);
                var hierarchyPath = ItemClassHierarchyHelper.GetHierarchyPath(Base, e.Row.ItemClassID);

                if (cascadingAttributes.Any())
                {
                    // Force refresh of Answers view
                    try
                    {
                        var answersView = Base.Views["Answers"];
                        if (answersView != null)
                        {
                            answersView.RequestRefresh();
                        }
                    }
                    catch (Exception)
                    {
                        // Ignore view refresh errors
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PXException(string.Format(Messages.ErrorPopulatingAttributes, ex.Message));
            }
        }

        /// <summary>
        /// When a new inventory item is inserted, track for attribute population
        /// </summary>
        protected virtual void _(Events.RowInserted<InventoryItem> e)
        {
            if (e.Row == null || e.Row.ItemClassID == null)
                return;

            // For new items, mark that item class should be processed
            if (e.ExternalCall)
            {
                itemClassChanged = true;
            }
        }

        /// <summary>
        /// Hide actions from the UI
        /// </summary>
        protected virtual void _(Events.RowSelected<InventoryItem> e)
        {
            RefreshCascadingAttributes.SetVisible(false);
            ViewAttributeHierarchy.SetVisible(false);
        }

        /// <summary>
        /// After row is persisted, ensure attributes are properly saved
        /// </summary>
        protected virtual void _(Events.RowPersisted<InventoryItem> e)
        {
            if (e.Row == null || e.TranStatus != PXTranStatus.Open)
                return;

            // Reset tracking flag after persist
            itemClassChanged = false;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Populates cascading attributes for the inventory item from the item class hierarchy
        /// </summary>
        /// <param name="item">The inventory item</param>
        private void PopulateCascadingAttributes(InventoryItem item)
        {
            if (item?.ItemClassID == null || item.NoteID == null)
                return;

            ItemClassHierarchyHelper.PopulateItemAttributesFromHierarchy(
                Base,
                item.InventoryID,
                item.ItemClassID,
                item.NoteID);
        }

        #endregion

        #region Actions

        /// <summary>
        /// Action to manually refresh cascading attributes from item class hierarchy
        /// </summary>
        public PXAction<InventoryItem> RefreshCascadingAttributes;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Refresh Cascading Attributes", MapEnableRights = PXCacheRights.Update)]
        protected virtual void refreshCascadingAttributes()
        {
            var current = Base.Item.Current;
            if (current == null || current.ItemClassID == null)
            {
                throw new PXException(Messages.SelectInventoryItemFirst);
            }

            try
            {
                // Get current attribute count
                var beforeCount = PXSelect<CSAnswers,
                    Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>
                    .Select(Base, current.NoteID)
                    .Count;

                // Repopulate attributes
                PopulateCascadingAttributes(current);

                // Get new attribute count
                var afterCount = PXSelect<CSAnswers,
                    Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>
                    .Select(Base, current.NoteID)
                    .Count;

                var hierarchyPath = ItemClassHierarchyHelper.GetHierarchyPath(Base, current.ItemClassID);
                var message = $"Attributes refreshed successfully.\n\n" +
                             $"Before: {beforeCount} attribute(s)\n" +
                             $"After: {afterCount} attribute(s)\n\n" +
                             $"Hierarchy: {hierarchyPath}";

                throw new PXException(PXMessages.LocalizeFormatNoPrefix(message));
            }
            catch (PXException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PXException(string.Format(Messages.ErrorRefreshingAttributes, ex.Message));
            }
        }

        /// <summary>
        /// Action to view the attribute hierarchy for the current item
        /// </summary>
        public PXAction<InventoryItem> ViewAttributeHierarchy;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "View Attribute Hierarchy", MapEnableRights = PXCacheRights.Select)]
        protected virtual void viewAttributeHierarchy()
        {
            var current = Base.Item.Current;
            if (current == null || current.ItemClassID == null)
            {
                throw new PXException(Messages.SelectInventoryItemFirst);
            }

            var hierarchy = ItemClassHierarchyHelper.GetItemClassHierarchy(Base, current.ItemClassID);
            var cascadingAttributes = ItemClassHierarchyHelper.GetCascadingAttributes(Base, current.ItemClassID);

            var message = $"Attribute Hierarchy for {current.InventoryCD}\n\n";
            message += "Item Class Hierarchy:\n";
            
            var hierarchyPath = ItemClassHierarchyHelper.GetHierarchyPath(Base, current.ItemClassID);
            message += $"  {hierarchyPath}\n\n";

            message += $"Cascading Attributes ({cascadingAttributes.Count}):\n";
            foreach (var attr in cascadingAttributes)
            {
                message += $"  • {attr.AttributeID}: {attr.Description}";
                if (attr.Required == true)
                    message += " (Required)";
                if (!string.IsNullOrEmpty(attr.DefaultValue))
                    message += $" [Default: {attr.DefaultValue}]";
                message += "\n";
            }

            // Show current values
            var currentAnswers = PXSelect<CSAnswers,
                Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>
                .Select(Base, current.NoteID)
                .FirstTableItems
                .ToList();

            if (currentAnswers.Any())
            {
                message += $"\n\nCurrent Values ({currentAnswers.Count}):\n";
                foreach (var answer in currentAnswers.OrderBy(a => a.AttributeID))
                {
                    message += $"  • {answer.AttributeID}: {answer.Value ?? "(not set)"}\n";
                }
            }

            throw new PXException(PXMessages.LocalizeFormatNoPrefix(message));
        }

        #endregion
    }
}
