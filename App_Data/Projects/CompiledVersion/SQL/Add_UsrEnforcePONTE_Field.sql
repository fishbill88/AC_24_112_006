-- =============================================
-- NTE Validation - Database Schema Update
-- Add UsrEnforcePONTE field to SOSetup table
-- =============================================
-- Project: CompiledVersion
-- Feature: Drop-Ship PO Not-To-Exceed Validation
-- Date: December 19, 2025
-- =============================================

USE [AcumaticaDB]  -- Replace with your actual database name
GO

-- Check if column already exists before adding
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'dbo.SOSetup') 
    AND name = 'UsrEnforcePONTE'
)
BEGIN
    PRINT 'Adding UsrEnforcePONTE column to SOSetup table...'
    
    -- Add the column
    ALTER TABLE dbo.SOSetup
    ADD UsrEnforcePONTE bit NULL
    GO
    
    -- Set default value for existing records (false = warning mode)
    UPDATE dbo.SOSetup
    SET UsrEnforcePONTE = 0
    WHERE UsrEnforcePONTE IS NULL
    GO
    
    PRINT 'UsrEnforcePONTE column added successfully.'
END
ELSE
BEGIN
    PRINT 'UsrEnforcePONTE column already exists in SOSetup table.'
END
GO

-- Verify the column was added
IF EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'dbo.SOSetup') 
    AND name = 'UsrEnforcePONTE'
)
BEGIN
    PRINT 'Verification: UsrEnforcePONTE column exists.'
    
    -- Display column information
    SELECT 
        c.name AS ColumnName,
        t.name AS DataType,
        c.max_length AS MaxLength,
        c.is_nullable AS IsNullable,
        ISNULL(dc.definition, 'No Default') AS DefaultValue
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    LEFT JOIN sys.default_constraints dc ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
    WHERE c.object_id = OBJECT_ID(N'dbo.SOSetup')
    AND c.name = 'UsrEnforcePONTE'
END
ELSE
BEGIN
    PRINT 'ERROR: UsrEnforcePONTE column was not created successfully.'
END
GO

-- =============================================
-- NOTES:
-- =============================================
-- 1. Replace 'AcumaticaDB' with your actual Acumatica database name
-- 2. This script is idempotent - safe to run multiple times
-- 3. Default value is 0 (false) = Warning mode
-- 4. To enable hard stop mode, set value to 1 (true) in SO Preferences
-- =============================================

-- Optional: Display current setup values
-- Uncomment to view existing SOSetup records
/*
SELECT 
    CompanyID,
    UsrEnforcePONTE AS [Enforce PO NTE],
    UsrNotToExceed AS [NTE Ship Terms],
    UsrCopyHeaderNotesToPO AS [Copy Header Notes],
    UsrCopyLineNotesToPO AS [Copy Line Notes],
    UsrCopyHeaderAttachmentsToPO AS [Copy Header Attachments],
    UsrCopyLineAttachmentsToPO AS [Copy Line Attachments]
FROM dbo.SOSetup
*/
GO
