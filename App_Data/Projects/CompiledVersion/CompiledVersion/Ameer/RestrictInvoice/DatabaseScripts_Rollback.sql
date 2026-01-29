-- =============================================
-- RestrictInvoice Customization - ROLLBACK Script
-- =============================================
-- Description: Removes custom fields from APSetup table
-- WARNING: This will permanently delete the columns and all data in them
-- Table: APSetup
-- =============================================

-- Backup existing values before dropping (optional - uncomment if needed)
/*
SELECT 
    CompanyID,
    UsrEnablePOBillRestriction,
    UsrPOBillAmountTolerance
INTO APSetup_CustomFields_Backup
FROM [dbo].[APSetup]
WHERE UsrEnablePOBillRestriction IS NOT NULL 
   OR UsrPOBillAmountTolerance IS NOT NULL
GO
*/

-- Drop UsrPOBillAmountTolerance column
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[APSetup]') AND name = 'UsrPOBillAmountTolerance')
BEGIN
    ALTER TABLE [dbo].[APSetup]
    DROP COLUMN [UsrPOBillAmountTolerance]
    
    PRINT 'Column UsrPOBillAmountTolerance removed from APSetup table'
END
ELSE
BEGIN
    PRINT 'Column UsrPOBillAmountTolerance does not exist in APSetup table'
END
GO

-- Drop UsrEnablePOBillRestriction column
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[APSetup]') AND name = 'UsrEnablePOBillRestriction')
BEGIN
    ALTER TABLE [dbo].[APSetup]
    DROP COLUMN [UsrEnablePOBillRestriction]
    
    PRINT 'Column UsrEnablePOBillRestriction removed from APSetup table'
END
ELSE
BEGIN
    PRINT 'Column UsrEnablePOBillRestriction does not exist in APSetup table'
END
GO

-- Verify columns were removed
IF NOT EXISTS (
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'APSetup' 
      AND COLUMN_NAME IN ('UsrEnablePOBillRestriction', 'UsrPOBillAmountTolerance')
)
BEGIN
    PRINT 'Rollback completed successfully - all custom columns removed'
END
ELSE
BEGIN
    PRINT 'WARNING: Some columns may still exist. Please verify manually.'
    
    SELECT 
        COLUMN_NAME,
        DATA_TYPE
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'APSetup' 
      AND COLUMN_NAME IN ('UsrEnablePOBillRestriction', 'UsrPOBillAmountTolerance')
END
GO
