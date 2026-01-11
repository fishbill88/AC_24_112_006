using PX.Common;

namespace ItemClassCustomization
{
    /// <summary>
    /// Static messages for Item Class Customization module
    /// </summary>
    [PXLocalizable]
    public static class Messages
    {
        #region Item Class Hierarchy

        public const string CircularReferenceError = "Cannot set this parent class as it would create a circular reference in the hierarchy.";
        public const string MaxHierarchyDepthError = "Cannot set this parent class as it would exceed the maximum hierarchy depth of 10 levels.";
        public const string DifferentStockItemWarning = "Warning: Parent class has different Stock Item setting. This may cause issues.";
        public const string ItemClassUsedByItemsWarning = "Warning: This item class is used by {0} inventory item(s). Changing the parent class will affect attributes for these items.";
        public const string CircularReferenceOnSaveError = "Cannot save: Circular reference detected in item class hierarchy.";
        public const string DeleteItemClassWithChildrenTitle = "Delete Item Class with Children";
        public const string DeleteItemClassWithChildrenMessage = "This item class has {0} child class(es). Deleting it will make the child classes orphaned (no parent). Do you want to continue?";
        public const string NoAttributesFoundInHierarchy = "No attributes found in the hierarchy for {0}.";

        #endregion

        #region Inventory Item

        public const string SelectInventoryItemFirst = "Please select an inventory item with an item class first.";
        public const string ErrorPopulatingAttributes = "Error populating attributes from item class hierarchy: {0}";
        public const string ErrorRefreshingAttributes = "Error refreshing attributes: {0}";

        #endregion
    }
}
