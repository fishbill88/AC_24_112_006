using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.IN
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
        /// Uses ItemClassTree to dynamically compute parent relationships from ItemClassCD segments.
        /// </summary>
        public static List<INItemClass> GetItemClassHierarchy(PXGraph graph, int? itemClassID)
        {
            var hierarchy = new List<INItemClass>();
            
            if (itemClassID == null)
            {
                PXTrace.WriteWarning("[ItemClassHierarchy] GetItemClassHierarchy called with NULL itemClassID");
                return hierarchy;
            }

            PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] GetItemClassHierarchy starting for ItemClassID: {0}", itemClassID));

            try
            {
                // Get the ItemClassTree singleton instance
                var tree = ItemClassTree.Instance;
                
                // Get the current node
                var currentNode = tree.GetNodeByID(itemClassID.Value);
                if (currentNode == null)
                {
                    PXTrace.WriteWarning(string.Format("[ItemClassHierarchy] Could not find node in ItemClassTree for ItemClassID: {0}", itemClassID));
                    return hierarchy;
                }

                PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Found current node: {0}", currentNode.ItemClassCD));

                // Add current node to hierarchy
                hierarchy.Add(currentNode);
                PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Level 0: ItemClassCD={0}, ItemClassID={1}", currentNode.ItemClassCD, currentNode.ItemClassID));

                // Get all parent nodes using ItemClassTree
                var parents = tree.GetParentsOf(currentNode.ItemClassCD).ToList();
                PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Found {0} parent(s) in tree", parents.Count));

                // Add parents to hierarchy (they're returned in order from immediate parent to root)
                for (int i = 0; i < parents.Count; i++)
                {
                    var parent = parents[i];
                    hierarchy.Add(parent);
                    PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Level {0}: ItemClassCD={1}, ItemClassID={2}", i + 1, parent.ItemClassCD, parent.ItemClassID));
                }

                PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] GetItemClassHierarchy complete. Found {0} level(s)", hierarchy.Count));
            }
            catch (Exception ex)
            {
                PXTrace.WriteError(string.Format("[ItemClassHierarchy] Exception in GetItemClassHierarchy: {0}", ex.Message));
                PXTrace.WriteError(string.Format("[ItemClassHierarchy] Stack trace: {0}", ex.StackTrace));
            }

            return hierarchy;
        }

        /// <summary>
        /// Gets all attributes from the item class hierarchy, with child attributes overriding parent attributes.
        /// </summary>
        public static List<CSAttributeGroup> GetCascadingAttributes(PXGraph graph, int? itemClassID)
        {
            if (itemClassID == null)
                return new List<CSAttributeGroup>();

            PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] GetCascadingAttributes called for ItemClassID: {0}", itemClassID));

            var hierarchy = GetItemClassHierarchy(graph, itemClassID);
            PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Hierarchy depth: {0}", hierarchy.Count));

            var attributeMap = new Dictionary<string, CSAttributeGroup>(StringComparer.OrdinalIgnoreCase);
            var entityType = typeof(InventoryItem).FullName;

            // Process from parent to child (reverse order) so child attributes override parent
            for (int i = hierarchy.Count - 1; i >= 0; i--)
            {
                var itemClass = hierarchy[i];
                var itemClassStrID = itemClass.ItemClassID?.ToString();

                PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Processing level {0}: ItemClassCD={1}, ItemClassID={2}, ItemClassStrID={3}", i, itemClass.ItemClassCD, itemClass.ItemClassID, itemClassStrID));

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

                PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Found {0} attribute(s) for {1}", attributes.Count, itemClass.ItemClassCD));

                // Add or override attributes (child overrides parent)
                foreach (var attr in attributes)
                {
                    if (!string.IsNullOrEmpty(attr.AttributeID))
                    {
                        var action = attributeMap.ContainsKey(attr.AttributeID) ? "Overriding" : "Adding";
                        PXTrace.WriteInformation(string.Format("[ItemClassHierarchy]   {0} attribute: {1} - {2}", action, attr.AttributeID, attr.Description));
                        attributeMap[attr.AttributeID] = attr;
                    }
                }
            }

            var result = attributeMap.Values
                .OrderBy(a => a.SortOrder ?? int.MaxValue)
                .ThenBy(a => a.AttributeID)
                .ToList();

            PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Total unique cascading attributes: {0}", result.Count));
            return result;
        }

        /// <summary>
        /// Populates CSAnswers records for an inventory item based on cascading attributes from item class hierarchy.
        /// </summary>
        public static void PopulateItemAttributesFromHierarchy(PXGraph graph, int? inventoryID, int? itemClassID, Guid? refNoteID)
        {
            if (inventoryID == null || itemClassID == null || refNoteID == null)
            {
                PXTrace.WriteWarning(string.Format("[ItemClassHierarchy] PopulateItemAttributesFromHierarchy - Invalid parameters: InventoryID={0}, ItemClassID={1}, RefNoteID={2}", inventoryID, itemClassID, refNoteID));
                return;
            }

            PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] PopulateItemAttributesFromHierarchy - InventoryID: {0}, ItemClassID: {1}, RefNoteID: {2}", inventoryID, itemClassID, refNoteID));

            var cascadingAttributes = GetCascadingAttributes(graph, itemClassID);
            PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Retrieved {0} cascading attributes", cascadingAttributes.Count));

            var answersCache = graph.Caches[typeof(CSAnswers)];

            // Get existing answers
            var existingAnswers = PXSelect<CSAnswers,
                Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>
                .Select(graph, refNoteID)
                .FirstTableItems
                .ToDictionary(a => a.AttributeID, StringComparer.OrdinalIgnoreCase);

            PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Found {0} existing answer(s)", existingAnswers.Count));

            int addedCount = 0;
            int updatedCount = 0;

            foreach (var attrGroup in cascadingAttributes)
            {
                CSAnswers answer;

                if (existingAnswers.TryGetValue(attrGroup.AttributeID, out var existingAnswer))
                {
                    answer = (CSAnswers)answersCache.CreateCopy(existingAnswer);
                    answer.IsRequired = attrGroup.Required;
                    
                    if (string.IsNullOrEmpty(answer.Value) && !string.IsNullOrEmpty(attrGroup.DefaultValue))
                    {
                        answer.Value = attrGroup.DefaultValue;
                    }
                    
                    answersCache.Update(answer);
                    updatedCount++;
                    PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Updated answer: {0}", attrGroup.AttributeID));
                }
                else
                {
                    answer = (CSAnswers)answersCache.CreateInstance();
                    answer.RefNoteID = refNoteID;
                    answer.AttributeID = attrGroup.AttributeID;
                    answer.Value = attrGroup.DefaultValue;
                    answer.IsRequired = attrGroup.Required;
                    
                    answersCache.Insert(answer);
                    addedCount++;
                    PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Inserted answer: {0}", attrGroup.AttributeID));
                }
            }

            PXTrace.WriteInformation(string.Format("[ItemClassHierarchy] Attribute population complete - Added: {0}, Updated: {1}", addedCount, updatedCount));
        }

        public static string GetHierarchyPath(PXGraph graph, int? itemClassID)
        {
            if (itemClassID == null)
                return string.Empty;

            var hierarchy = GetItemClassHierarchy(graph, itemClassID);
            
            if (!hierarchy.Any())
                return string.Empty;

            var pathSegments = hierarchy
                .AsEnumerable()
                .Reverse()
                .Select(ic => ic.ItemClassCD)
                .Where(cd => !string.IsNullOrEmpty(cd));

            return string.Join(" → ", pathSegments);
        }
    }
}
