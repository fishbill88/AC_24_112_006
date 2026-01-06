-- =============================================================================
-- SQL Scripts for Hierarchical Item Class Implementation
-- Acumatica ERP - Ameer Customization
-- Created: January 1, 2026
-- =============================================================================

-- =============================================================================
-- Script 1: Validate Item Class Hierarchy (Prevent Circular References)
-- =============================================================================

/*
    Function: ValidateItemClassHierarchy
    Purpose: Validates that setting a parent item class would not create a circular reference
    Returns: TRUE if valid, FALSE if circular reference detected
    
    Usage Example:
        SELECT ValidateItemClassHierarchy(123, 456);
        -- Returns TRUE if setting ItemClassID 456 as parent of 123 is valid
*/

CREATE OR REPLACE FUNCTION ValidateItemClassHierarchy(
    p_ItemClassID INT,
    p_ParentItemClassID INT
) RETURNS BOOLEAN AS $$
DECLARE
    v_CurrentID INT;
    v_Counter INT := 0;
    v_MaxDepth INT := 10;
BEGIN
    -- NULL parent is always valid
    IF p_ParentItemClassID IS NULL THEN
        RETURN TRUE;
    END IF;
    
    -- Self-reference is invalid
    IF p_ItemClassID = p_ParentItemClassID THEN
        RETURN FALSE;
    END IF;
    
    -- Traverse up the parent hierarchy
    v_CurrentID := p_ParentItemClassID;
    
    WHILE v_CurrentID IS NOT NULL AND v_Counter < v_MaxDepth LOOP
        -- Check if we've encountered the original item class (circular reference)
        IF v_CurrentID = p_ItemClassID THEN
            RETURN FALSE;
        END IF;
        
        -- Get the next parent in the chain
        SELECT ParentItemClassID INTO v_CurrentID
        FROM INItemClass
        WHERE ItemClassID = v_CurrentID;
        
        v_Counter := v_Counter + 1;
    END LOOP;
    
    -- If we exceeded max depth, consider it invalid to prevent issues
    IF v_Counter >= v_MaxDepth THEN
        RETURN FALSE;
    END IF;
    
    RETURN TRUE;
END;
$$ LANGUAGE plpgsql;


-- =============================================================================
-- Script 2: Get Item Class Hierarchy Path
-- =============================================================================

/*
    Recursive CTE Query: Get Full Hierarchy Path
    Purpose: Returns the complete hierarchy from a specific item class to the root
    
    Usage Example:
        Execute this query replacing @SelectedItemClassID with your item class ID
*/

-- Example query to get hierarchy for a specific item class
-- REPLACE @SelectedItemClassID with the actual ItemClassID value

WITH RECURSIVE ItemClassHierarchy AS (
    -- Base case: start with the selected class
    SELECT 
        ic.ItemClassID,
        ic.ItemClassCD,
        ic.Descr,
        ic.ParentItemClassID,
        ic.StkItem,
        ic.ItemClassCD::TEXT as HierarchyPath,
        0 as Level
    FROM INItemClass ic
    WHERE ic.ItemClassID = @SelectedItemClassID  -- REPLACE THIS VALUE
    
    UNION ALL
    
    -- Recursive case: get parent classes
    SELECT 
        ic.ItemClassID,
        ic.ItemClassCD,
        ic.Descr,
        ic.ParentItemClassID,
        ic.StkItem,
        ic.ItemClassCD || ' → ' || ich.HierarchyPath as HierarchyPath,
        ich.Level + 1 as Level
    FROM INItemClass ic
    INNER JOIN ItemClassHierarchy ich ON ic.ItemClassID = ich.ParentItemClassID
    WHERE ich.Level < 10  -- Prevent infinite loops
)
SELECT 
    ItemClassID,
    ItemClassCD,
    Descr,
    ParentItemClassID,
    HierarchyPath,
    Level
FROM ItemClassHierarchy
ORDER BY Level DESC;


-- =============================================================================
-- Script 3: Get All Cascading Attributes from Hierarchy
-- =============================================================================

/*
    Recursive CTE Query: Get All Cascading Attributes
    Purpose: Returns all attributes from an item class and its parent hierarchy
             Child attributes override parent attributes with the same AttributeID
    
    Usage Example:
        Execute this query replacing @SelectedItemClassID with your item class ID
*/

WITH RECURSIVE ItemClassHierarchy AS (
    -- Base case: start with the selected class
    SELECT 
        ic.ItemClassID, 
        ic.ParentItemClassID, 
        CAST(ic.ItemClassID AS VARCHAR(50)) as ItemClassStrID,
        0 as Level
    FROM INItemClass ic
    WHERE ic.ItemClassID = @SelectedItemClassID  -- REPLACE THIS VALUE
    
    UNION ALL
    
    -- Recursive case: get parent classes
    SELECT 
        ic.ItemClassID, 
        ic.ParentItemClassID, 
        CAST(ic.ItemClassID AS VARCHAR(50)) as ItemClassStrID,
        ich.Level + 1 as Level
    FROM INItemClass ic
    INNER JOIN ItemClassHierarchy ich ON ic.ItemClassID = ich.ParentItemClassID
    WHERE ich.Level < 10  -- Prevent infinite loops
),
AttributesWithPriority AS (
    SELECT 
        ag.AttributeID,
        ag.Description,
        ag.SortOrder,
        ag.Required,
        ag.DefaultValue,
        ag.AttributeCategory,
        ag.IsActive,
        ich.Level,
        ich.ItemClassStrID,
        ROW_NUMBER() OVER (
            PARTITION BY ag.AttributeID 
            ORDER BY ich.Level ASC  -- Lower level (child) wins
        ) as AttributePriority
    FROM ItemClassHierarchy ich
    INNER JOIN CSAttributeGroup ag 
        ON ag.EntityClassID = ich.ItemClassStrID
        AND ag.EntityType = 'PX.Objects.IN.InventoryItem'
        AND ag.IsActive = TRUE
)
SELECT 
    AttributeID,
    Description,
    SortOrder,
    Required,
    DefaultValue,
    AttributeCategory,
    Level,
    ItemClassStrID
FROM AttributesWithPriority
WHERE AttributePriority = 1  -- Only keep the highest priority (child overrides parent)
ORDER BY SortOrder, AttributeID;


-- =============================================================================
-- Script 4: Create Sample Hierarchical Item Classes
-- =============================================================================

/*
    Sample Data Creation Script
    Purpose: Creates a test hierarchy matching the requirements
    
    Hierarchy Structure:
        Parent: A
          ├─ Child: A1
          │    ├─ Sub-child: aa1
          │    ├─ Sub-child: aa2
          │    └─ Sub-child: aa3
          └─ Child: B1
               ├─ Sub-child: bb1
               ├─ Sub-child: bb2
               └─ Sub-child: bb3
*/

-- Step 1: Create Parent Class A
DO $$
DECLARE
    v_ParentID_A INT;
    v_ChildID_A1 INT;
    v_ChildID_B1 INT;
    v_MaxSegmentID INT;
BEGIN
    -- Get the next available SegmentID for the dimension
    SELECT COALESCE(MAX(SegmentID), 0) + 1 INTO v_MaxSegmentID
    FROM Segment
    WHERE DimensionID = 'INITEMCLASS';

    -- Insert Parent Class A
    INSERT INTO INItemClass (
        ItemClassCD, 
        Descr, 
        StkItem, 
        ParentItemClassID,
        tstamp
    )
    VALUES (
        'A', 
        'Parent Class A - Test Hierarchy', 
        TRUE, 
        NULL,
        1
    )
    RETURNING ItemClassID INTO v_ParentID_A;
    
    RAISE NOTICE 'Created Parent Class A with ItemClassID: %', v_ParentID_A;

    -- Insert Child Class A1 (child of A)
    INSERT INTO INItemClass (
        ItemClassCD, 
        Descr, 
        StkItem, 
        ParentItemClassID,
        tstamp
    )
    VALUES (
        'A1', 
        'Child Class A1', 
        TRUE, 
        v_ParentID_A,
        1
    )
    RETURNING ItemClassID INTO v_ChildID_A1;
    
    RAISE NOTICE 'Created Child Class A1 with ItemClassID: %', v_ChildID_A1;

    -- Insert Child Class B1 (child of A)
    INSERT INTO INItemClass (
        ItemClassCD, 
        Descr, 
        StkItem, 
        ParentItemClassID,
        tstamp
    )
    VALUES (
        'B1', 
        'Child Class B1', 
        TRUE, 
        v_ParentID_A,
        1
    )
    RETURNING ItemClassID INTO v_ChildID_B1;
    
    RAISE NOTICE 'Created Child Class B1 with ItemClassID: %', v_ChildID_B1;

    -- Insert Sub-children for A1: aa1, aa2, aa3
    INSERT INTO INItemClass (ItemClassCD, Descr, StkItem, ParentItemClassID, tstamp)
    VALUES 
        ('aa1', 'Sub-child aa1', TRUE, v_ChildID_A1, 1),
        ('aa2', 'Sub-child aa2', TRUE, v_ChildID_A1, 1),
        ('aa3', 'Sub-child aa3', TRUE, v_ChildID_A1, 1);
    
    RAISE NOTICE 'Created Sub-children for A1: aa1, aa2, aa3';

    -- Insert Sub-children for B1: bb1, bb2, bb3
    INSERT INTO INItemClass (ItemClassCD, Descr, StkItem, ParentItemClassID, tstamp)
    VALUES 
        ('bb1', 'Sub-child bb1', TRUE, v_ChildID_B1, 1),
        ('bb2', 'Sub-child bb2', TRUE, v_ChildID_B1, 1),
        ('bb3', 'Sub-child bb3', TRUE, v_ChildID_B1, 1);
    
    RAISE NOTICE 'Created Sub-children for B1: bb1, bb2, bb3';
    
    RAISE NOTICE 'Sample hierarchy created successfully!';
END $$;


-- =============================================================================
-- Script 5: Create Sample Attributes for Hierarchy Testing
-- =============================================================================

/*
    Sample Attribute Creation Script
    Purpose: Creates test attributes for the hierarchy
    
    Attributes:
        - A: attr1 (on parent A)
        - A1: 1attr1 (on child A1, plus inherits attr1 from A)
        - aa1: aa1attr1 (on sub-child aa1, plus inherits from A1 and A)
        - aa2: aa2attr2 (on sub-child aa2)
*/

DO $$
DECLARE
    v_ParentID_A INT;
    v_ChildID_A1 INT;
    v_SubChildID_aa1 INT;
    v_SubChildID_aa2 INT;
    v_AttrID_attr1 VARCHAR(10);
    v_AttrID_1attr1 VARCHAR(10);
    v_AttrID_aa1attr1 VARCHAR(10);
    v_AttrID_aa2attr2 VARCHAR(10);
BEGIN
    -- Get ItemClassIDs
    SELECT ItemClassID INTO v_ParentID_A FROM INItemClass WHERE ItemClassCD = 'A';
    SELECT ItemClassID INTO v_ChildID_A1 FROM INItemClass WHERE ItemClassCD = 'A1';
    SELECT ItemClassID INTO v_SubChildID_aa1 FROM INItemClass WHERE ItemClassCD = 'aa1';
    SELECT ItemClassID INTO v_SubChildID_aa2 FROM INItemClass WHERE ItemClassCD = 'aa2';

    IF v_ParentID_A IS NULL THEN
        RAISE EXCEPTION 'Please run Script 4 first to create the sample hierarchy';
    END IF;

    -- Create Attributes if they don't exist
    v_AttrID_attr1 := 'ATTR1';
    v_AttrID_1attr1 := '1ATTR1';
    v_AttrID_aa1attr1 := 'AA1ATTR1';
    v_AttrID_aa2attr2 := 'AA2ATTR2';

    -- Insert attributes into CSAttribute if not exists
    INSERT INTO CSAttribute (AttributeID, Description, ControlType, EntryMask, RegExp, List, tstamp)
    VALUES 
        (v_AttrID_attr1, 'Attribute from Class A', 1, NULL, NULL, NULL, 1),
        (v_AttrID_1attr1, 'Attribute from Class A1', 1, NULL, NULL, NULL, 1),
        (v_AttrID_aa1attr1, 'Attribute from Class aa1', 1, NULL, NULL, NULL, 1),
        (v_AttrID_aa2attr2, 'Attribute from Class aa2', 1, NULL, NULL, NULL, 1)
    ON CONFLICT (AttributeID) DO NOTHING;

    -- Assign attributes to item classes
    -- Parent A gets attr1
    INSERT INTO CSAttributeGroup (
        EntityType, EntityClassID, AttributeID, 
        SortOrder, Required, IsActive, ControlType, tstamp
    )
    VALUES (
        'PX.Objects.IN.InventoryItem', 
        CAST(v_ParentID_A AS VARCHAR), 
        v_AttrID_attr1,
        1, FALSE, TRUE, 1, 1
    )
    ON CONFLICT (EntityType, EntityClassID, AttributeID) DO NOTHING;

    -- Child A1 gets 1attr1
    INSERT INTO CSAttributeGroup (
        EntityType, EntityClassID, AttributeID, 
        SortOrder, Required, IsActive, ControlType, tstamp
    )
    VALUES (
        'PX.Objects.IN.InventoryItem', 
        CAST(v_ChildID_A1 AS VARCHAR), 
        v_AttrID_1attr1,
        1, FALSE, TRUE, 1, 1
    )
    ON CONFLICT (EntityType, EntityClassID, AttributeID) DO NOTHING;

    -- Sub-child aa1 gets aa1attr1
    INSERT INTO CSAttributeGroup (
        EntityType, EntityClassID, AttributeID, 
        SortOrder, Required, IsActive, ControlType, tstamp
    )
    VALUES (
        'PX.Objects.IN.InventoryItem', 
        CAST(v_SubChildID_aa1 AS VARCHAR), 
        v_AttrID_aa1attr1,
        1, FALSE, TRUE, 1, 1
    )
    ON CONFLICT (EntityType, EntityClassID, AttributeID) DO NOTHING;

    -- Sub-child aa2 gets aa2attr2
    INSERT INTO CSAttributeGroup (
        EntityType, EntityClassID, AttributeID, 
        SortOrder, Required, IsActive, ControlType, tstamp
    )
    VALUES (
        'PX.Objects.IN.InventoryItem', 
        CAST(v_SubChildID_aa2 AS VARCHAR), 
        v_AttrID_aa2attr2,
        1, FALSE, TRUE, 1, 1
    )
    ON CONFLICT (EntityType, EntityClassID, AttributeID) DO NOTHING;

    RAISE NOTICE 'Sample attributes created successfully!';
    RAISE NOTICE 'Expected cascading attributes for aa1: attr1, 1attr1, aa1attr1';
END $$;


-- =============================================================================
-- Script 6: Query to Verify Hierarchy and Cascading Attributes
-- =============================================================================

/*
    Verification Query
    Purpose: Verify the hierarchy and cascading attributes are working correctly
*/

-- Query 1: Show all item classes with their hierarchy level
WITH RECURSIVE ItemClassHierarchy AS (
    SELECT 
        ic.ItemClassID,
        ic.ItemClassCD,
        ic.Descr,
        ic.ParentItemClassID,
        0 as Level,
        ic.ItemClassCD::TEXT as Path
    FROM INItemClass ic
    WHERE ic.ParentItemClassID IS NULL
    
    UNION ALL
    
    SELECT 
        ic.ItemClassID,
        ic.ItemClassCD,
        ic.Descr,
        ic.ParentItemClassID,
        ich.Level + 1,
        ich.Path || ' → ' || ic.ItemClassCD
    FROM INItemClass ic
    INNER JOIN ItemClassHierarchy ich ON ic.ParentItemClassID = ich.ItemClassID
    WHERE ich.Level < 10
)
SELECT 
    ItemClassCD,
    Descr,
    Level,
    Path as HierarchyPath
FROM ItemClassHierarchy
WHERE ItemClassCD IN ('A', 'A1', 'B1', 'aa1', 'aa2', 'aa3', 'bb1', 'bb2', 'bb3')
ORDER BY Path;


-- Query 2: Show cascading attributes for aa1
-- This should show: attr1 (from A), 1attr1 (from A1), aa1attr1 (from aa1)
WITH RECURSIVE ItemClassHierarchy AS (
    SELECT 
        ic.ItemClassID, 
        CAST(ic.ItemClassID AS VARCHAR(50)) as ItemClassStrID,
        ic.ItemClassCD,
        0 as Level
    FROM INItemClass ic
    WHERE ic.ItemClassCD = 'aa1'
    
    UNION ALL
    
    SELECT 
        ic.ItemClassID, 
        CAST(ic.ItemClassID AS VARCHAR(50)) as ItemClassStrID,
        ic.ItemClassCD,
        ich.Level + 1
    FROM INItemClass ic
    INNER JOIN ItemClassHierarchy ich ON ic.ItemClassID = ich.ParentItemClassID
)
SELECT 
    ich.ItemClassCD as SourceClass,
    ag.AttributeID,
    ag.Description,
    ich.Level,
    ag.SortOrder
FROM ItemClassHierarchy ich
INNER JOIN CSAttributeGroup ag 
    ON ag.EntityClassID = ich.ItemClassStrID
    AND ag.EntityType = 'PX.Objects.IN.InventoryItem'
    AND ag.IsActive = TRUE
ORDER BY ich.Level, ag.SortOrder;


-- =============================================================================
-- Script 7: Cleanup Script (Use with caution!)
-- =============================================================================

/*
    Cleanup Script
    Purpose: Removes the sample hierarchy and attributes
    WARNING: Only use this in development/testing environments!
*/

/*
-- Uncomment to use:

DO $$
BEGIN
    -- Delete sample item classes
    DELETE FROM INItemClass 
    WHERE ItemClassCD IN ('A', 'A1', 'B1', 'aa1', 'aa2', 'aa3', 'bb1', 'bb2', 'bb3');
    
    -- Delete sample attributes
    DELETE FROM CSAttributeGroup 
    WHERE AttributeID IN ('ATTR1', '1ATTR1', 'AA1ATTR1', 'AA2ATTR2');
    
    DELETE FROM CSAttribute 
    WHERE AttributeID IN ('ATTR1', '1ATTR1', 'AA1ATTR1', 'AA2ATTR2');
    
    RAISE NOTICE 'Sample data cleaned up successfully!';
END $$;
*/

-- =============================================================================
-- End of SQL Scripts
-- =============================================================================
