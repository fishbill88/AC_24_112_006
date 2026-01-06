-- ===============================================================================
-- Quick Setup and Verification for Item Class Hierarchy
-- ===============================================================================

-- STEP 1: Verify your current item classes and their parent relationships
-- Run this query to see the current hierarchy setup:

SELECT 
    ItemClassID,
    ItemClassCD,
    Descr,
    ParentItemClassID,
    (SELECT ItemClassCD FROM INItemClass p WHERE p.ItemClassID = c.ParentItemClassID) as ParentItemClassCD,
    StkItem
FROM INItemClass c
WHERE ItemClassCD LIKE '%CONSUMER%QWE%'
   OR ItemClassCD LIKE '%CONSUMER%KRJRJR%'
ORDER BY ItemClassCD;

-- EXPECTED OUTPUT: You should see ParentItemClassID populated for child classes
-- If ParentItemClassID is NULL, the hierarchy is not set up!


-- ===============================================================================
-- STEP 2: Manual Setup - Update ParentItemClassID for your existing item classes
-- ===============================================================================

-- First, identify your three item classes:
-- Let's say you have:
--   1. CONSUMER (Parent) - should have ParentItemClassID = NULL
--   2. CONSUMER-QWE (Child of CONSUMER) - should have ParentItemClassID = CONSUMER's ID
--   3. CONSUMER-QWE-KRJRJR (Child of CONSUMER-QWE) - should have ParentItemClassID = CONSUMER-QWE's ID

-- Example: Link CONSUMER-QWE to CONSUMER as parent
/*
UPDATE INItemClass
SET ParentItemClassID = (SELECT ItemClassID FROM INItemClass WHERE ItemClassCD = 'CONSUMER')
WHERE ItemClassCD = 'CONSUMER-QWE';
*/

-- Example: Link CONSUMER-QWE-KRJRJR to CONSUMER-QWE as parent
/*
UPDATE INItemClass
SET ParentItemClassID = (SELECT ItemClassID FROM INItemClass WHERE ItemClassCD = 'CONSUMER-QWE')
WHERE ItemClassCD = 'CONSUMER-QWE-KRJRJR';
*/


-- ===============================================================================
-- STEP 3: Verify the hierarchy is correct
-- ===============================================================================

-- This query shows the complete hierarchy with levels:
WITH RECURSIVE ItemClassHierarchy AS (
    -- Start with root classes (no parent)
    SELECT 
        ItemClassID,
        ItemClassCD,
        Descr,
        ParentItemClassID,
        0 as Level,
        ItemClassCD::TEXT as Path
    FROM INItemClass
    WHERE ParentItemClassID IS NULL
      AND ItemClassCD LIKE '%CONSUMER%'
    
    UNION ALL
    
    -- Get child classes
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
    Level,
    ItemClassCD,
    Descr,
    Path as HierarchyPath,
    ItemClassID,
    ParentItemClassID
FROM ItemClassHierarchy
ORDER BY Path;

-- EXPECTED OUTPUT: You should see 3 levels (0, 1, 2) with proper hierarchy path


-- ===============================================================================
-- STEP 4: Verify attributes are assigned to each class
-- ===============================================================================

SELECT 
    ic.ItemClassCD,
    ag.AttributeID,
    ag.Description,
    ag.IsActive,
    ag.SortOrder
FROM INItemClass ic
LEFT JOIN CSAttributeGroup ag 
    ON ag.EntityClassID = CAST(ic.ItemClassID AS VARCHAR)
    AND ag.EntityType = 'PX.Objects.IN.InventoryItem'
    AND ag.IsActive = TRUE
WHERE ic.ItemClassCD LIKE '%CONSUMER%QWE%'
   OR ic.ItemClassCD LIKE '%CONSUMER%KRJRJR%'
ORDER BY ic.ItemClassCD, ag.SortOrder;

-- EXPECTED OUTPUT: Each item class should have at least one attribute


-- ===============================================================================
-- STEP 5: Test cascading query manually
-- ===============================================================================

-- Replace 'CONSUMER-QWE-KRJRJR' with your actual item class CD:
WITH RECURSIVE ItemClassHierarchy AS (
    SELECT 
        ic.ItemClassID, 
        CAST(ic.ItemClassID AS VARCHAR(50)) as ItemClassStrID,
        ic.ItemClassCD,
        0 as Level
    FROM INItemClass ic
    WHERE ic.ItemClassCD = 'CONSUMER-QWE-KRJRJR'  -- CHANGE THIS TO YOUR ITEM CLASS
    
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
    ich.Level,
    ag.AttributeID,
    ag.Description,
    ag.SortOrder
FROM ItemClassHierarchy ich
LEFT JOIN CSAttributeGroup ag 
    ON ag.EntityClassID = ich.ItemClassStrID
    AND ag.EntityType = 'PX.Objects.IN.InventoryItem'
    AND ag.IsActive = TRUE
ORDER BY ich.Level, ag.SortOrder;

-- EXPECTED OUTPUT: Should show attributes from all 3 levels
-- Level 0 = Child's own attributes
-- Level 1 = Parent's attributes  
-- Level 2 = Grandparent's attributes


-- ===============================================================================
-- TROUBLESHOOTING: If ParentItemClassID is NULL
-- ===============================================================================

-- Check if the Parent Item Class field exists and is editable in Acumatica UI
-- The field should be visible in Item Classes screen (IN201000)

-- If you need to set it via SQL (BE CAREFUL - backup first):
/*
-- Get the ItemClassID values first:
SELECT ItemClassID, ItemClassCD FROM INItemClass 
WHERE ItemClassCD IN (
    'CONSUMER',           -- Parent (root)
    'CONSUMER-QWE',       -- Child
    'CONSUMER-QWE-KRJRJR' -- Sub-child
);

-- Then update based on the IDs you got above:
-- Update child to point to parent
UPDATE INItemClass 
SET ParentItemClassID = [PARENT_ID_HERE]
WHERE ItemClassCD = 'CONSUMER-QWE';

-- Update sub-child to point to child  
UPDATE INItemClass 
SET ParentItemClassID = [CHILD_ID_HERE]
WHERE ItemClassCD = 'CONSUMER-QWE-KRJRJR';
*/
