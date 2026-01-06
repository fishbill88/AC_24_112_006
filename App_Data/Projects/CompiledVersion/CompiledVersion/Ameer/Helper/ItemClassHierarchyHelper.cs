using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;

namespace ACustom
{
    /// <summary>
    /// Helper class for managing hierarchical item class relationships and cascading attributes
    /// </summary>
    public static class ItemClassHierarchyHelper
    {
        private const int MAX_HIERARCHY_DEPTH = 10;

        /// <summary>
        /// Gets the complete hierarchy path from the specified item class to the root parent.
        /// Returns list ordered from child to root (most specific to most general).
        /// </summary>
        /// <param name="graph">The PXGraph instance</param>
        /// <param name="itemClassID">The starting item class ID</param>
        /// <returns>List of INItemClass records in the hierarchy path</returns>
        public static List<INItemClass> GetItemClassHierarchy(PXGraph graph, int? itemClassID)
        {
            var hierarchy = new List<INItemClass>();
            
            if (itemClassID == null)
            {
                return hierarchy;
            }

            var currentID = itemClassID;
            var visitedIDs = new HashSet<int?>(); // Prevent infinite loops
            var depth = 0;

            while (currentID != null && depth < MAX_HIERARCHY_DEPTH)
            {
                // Detect circular reference
                if (visitedIDs.Contains(currentID))
                {
                    break;
                }

                visitedIDs.Add(currentID);

                // Fetch the item class
                var itemClass = PXSelect<INItemClass,
                    Where<INItemClass.itemClassID, Equal<Required<INItemClass.itemClassID>>>>
                    .Select(graph, currentID)
                    .FirstOrDefault()?.GetItem<INItemClass>();

                if (itemClass == null)
                {
                    break;
                }

                hierarchy.Add(itemClass);
                
                currentID = itemClass.ParentItemClassID;
                depth++;
            }

            return hierarchy;
        }

        /// <summary>
        /// Gets all attributes from the item class hierarchy, with child attributes overriding parent attributes.
        /// Returns attributes ordered from most specific (child) to most general (parent).
        /// </summary>
        /// <param name="graph">The PXGraph instance</param>
        /// <param name="itemClassID">The item class ID</param>
        /// <returns>List of CSAttributeGroup records with duplicates removed (child wins)</returns>
        public static List<CSAttributeGroup> GetCascadingAttributes(PXGraph graph, int? itemClassID)
        {
            if (itemClassID == null)
                return new List<CSAttributeGroup>();

            var hierarchy = GetItemClassHierarchy(graph, itemClassID);

            var attributeMap = new Dictionary<string, CSAttributeGroup>(StringComparer.OrdinalIgnoreCase);
            var entityType = typeof(InventoryItem).FullName;

            // Process from parent to child (reverse order) so child attributes override parent
            for (int i = hierarchy.Count - 1; i >= 0; i--)
            {
                var itemClass = hierarchy[i];
                var itemClassStrID = itemClass.ItemClassID?.ToString();

                if (string.IsNullOrEmpty(itemClassStrID))
                    continue;

                // Get all attributes for this item class
                var attributes = PXSelect<CSAttributeGroup,
                    Where<CSAttributeGroup.entityClassID, Equal<Required<CSAttributeGroup.entityClassID>>,
                        And<CSAttributeGroup.entityType, Equal<Required<CSAttributeGroup.entityType>>,
                        And<CSAttributeGroup.isActive, Equal<True>>>>>
                    .Select(graph, itemClassStrID, entityType)
                    .FirstTableItems
                    .ToList();

                // Add or override attributes (child overrides parent)
                foreach (var attr in attributes)
                {
                    if (!string.IsNullOrEmpty(attr.AttributeID))
                    {
                        attributeMap[attr.AttributeID] = attr;
                    }
                }
            }

            var result = attributeMap.Values
                .OrderBy(a => a.SortOrder ?? int.MaxValue)
                .ThenBy(a => a.AttributeID)
                .ToList();

            // Return attributes sorted by sort order
            return result;
        }

        /// <summary>
        /// Validates that setting newParentID as parent of itemClassID would not create a circular reference.
        /// </summary>
        /// <param name="graph">The PXGraph instance</param>
        /// <param name="itemClassID">The item class being modified</param>
        /// <param name="newParentID">The proposed new parent ID</param>
        /// <returns>True if valid, false if circular reference would be created</returns>
        public static bool ValidateCircularReference(PXGraph graph, int? itemClassID, int? newParentID)
        {
            if (itemClassID == null || newParentID == null)
                return true;

            // Self-reference check
            if (itemClassID == newParentID)
                return false;

            // Check if itemClassID appears in the hierarchy of newParentID
            var parentHierarchy = GetItemClassHierarchy(graph, newParentID);
            return !parentHierarchy.Any(ic => ic.ItemClassID == itemClassID);
        }

        /// <summary>
        /// Gets the hierarchy depth (level) of the specified item class.
        /// Root classes have depth 0.
        /// </summary>
        /// <param name="graph">The PXGraph instance</param>
        /// <param name="itemClassID">The item class ID</param>
        /// <returns>The depth level (0 for root)</returns>
        public static int GetHierarchyDepth(PXGraph graph, int? itemClassID)
        {
            if (itemClassID == null)
                return 0;

            var hierarchy = GetItemClassHierarchy(graph, itemClassID);
            return hierarchy.Count - 1; // Subtract 1 because the list includes the item itself
        }

        /// <summary>
        /// Gets the full hierarchy path as a formatted string (e.g., "A → A1 → aa1").
        /// </summary>
        /// <param name="graph">The PXGraph instance</param>
        /// <param name="itemClassID">The item class ID</param>
        /// <returns>Formatted hierarchy path string</returns>
        public static string GetHierarchyPath(PXGraph graph, int? itemClassID)
        {
            if (itemClassID == null)
                return string.Empty;

            var hierarchy = GetItemClassHierarchy(graph, itemClassID);
            
            if (!hierarchy.Any())
                return string.Empty;

            // Reverse to show from root to child
            var pathSegments = hierarchy
                .AsEnumerable()
                .Reverse()
                .Select(ic => ic.ItemClassCD)
                .Where(cd => !string.IsNullOrEmpty(cd));

            return string.Join(" → ", pathSegments);
        }

        /// <summary>
        /// Checks if the specified item class has any child classes.
        /// </summary>
        /// <param name="graph">The PXGraph instance</param>
        /// <param name="itemClassID">The item class ID</param>
        /// <returns>True if has children, false otherwise</returns>
        public static bool HasChildClasses(PXGraph graph, int? itemClassID)
        {
            if (itemClassID == null)
                return false;

            var childCount = PXSelect<INItemClass,
                Where<INItemClass.parentItemClassID, Equal<Required<INItemClass.parentItemClassID>>>>
                .Select(graph, itemClassID)
                .Count;

            return childCount > 0;
        }

        /// <summary>
        /// Populates CSAnswers records for an inventory item based on cascading attributes from item class hierarchy.
        /// </summary>
        /// <param name="graph">The PXGraph instance</param>
        /// <param name="inventoryID">The inventory ID</param>
        /// <param name="itemClassID">The item class ID</param>
        /// <param name="refNoteID">The reference note ID of the inventory item</param>
        public static void PopulateItemAttributesFromHierarchy(PXGraph graph, int? inventoryID, int? itemClassID, Guid? refNoteID)
        {
            if (inventoryID == null || itemClassID == null || refNoteID == null)
            {
                return;
            }

            var cascadingAttributes = GetCascadingAttributes(graph, itemClassID);

            var answersCache = graph.Caches[typeof(CSAnswers)];

            // Get existing answers to preserve user-entered values
            var existingAnswers = PXSelect<CSAnswers,
                Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>
                .Select(graph, refNoteID)
                .FirstTableItems
                .ToDictionary(a => a.AttributeID, StringComparer.OrdinalIgnoreCase);

            int addedCount = 0;
            int updatedCount = 0;
            int deletedCount = 0;

            foreach (var attrGroup in cascadingAttributes)
            {
                CSAnswers answer;

                if (existingAnswers.TryGetValue(attrGroup.AttributeID, out var existingAnswer))
                {
                    // Update existing answer but preserve user-entered value
                    answer = (CSAnswers)answersCache.CreateCopy(existingAnswer);
                    answer.IsRequired = attrGroup.Required;
                    answer.AttributeCategory = attrGroup.AttributeCategory;
                    
                    // Only set default value if current value is empty
                    if (string.IsNullOrEmpty(answer.Value) && !string.IsNullOrEmpty(attrGroup.DefaultValue))
                    {
                        answer.Value = attrGroup.DefaultValue;
                    }
                    
                    answersCache.Update(answer);
                    updatedCount++;
                }
                else
                {
                    // Create new answer
                    answer = (CSAnswers)answersCache.CreateInstance();
                    answer.RefNoteID = refNoteID;
                    answer.AttributeID = attrGroup.AttributeID;
                    answer.Value = attrGroup.DefaultValue;
                    answer.IsRequired = attrGroup.Required;
                    answer.AttributeCategory = attrGroup.AttributeCategory;
                    
                    answersCache.Insert(answer);
                    addedCount++;
                }
            }

            // Remove answers for attributes that are no longer in the hierarchy
            var hierarchyAttributeIDs = new HashSet<string>(
                cascadingAttributes.Select(a => a.AttributeID),
                StringComparer.OrdinalIgnoreCase);

            foreach (var existingAnswer in existingAnswers.Values)
            {
                if (!hierarchyAttributeIDs.Contains(existingAnswer.AttributeID))
                {
                    answersCache.Delete(existingAnswer);
                    deletedCount++;
                }
            }
        }
    }
}
