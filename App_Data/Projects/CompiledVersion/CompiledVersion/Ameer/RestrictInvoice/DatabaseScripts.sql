-- =============================================
-- RestrictInvoice Customization - Database Scripts
-- =============================================
-- Description: Adds custom fields to APSetup table for PO-Bill restriction feature
-- Table: APSetup
-- Fields: 
--   - UsrEnablePOBillRestriction (bit) - Enable/disable restriction
--   - UsrPOBillAmountTolerance (decimal) - Configurable tolerance amount
-- =============================================

-- Check if columns already exist before adding
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[APSetup]') AND name = 'UsrEnablePOBillRestriction')
BEGIN
    ALTER TABLE [dbo].[APSetup]
    ADD [UsrEnablePOBillRestriction] bit NULL
    
    -- Set default value to 0 (false) for existing records
    UPDATE [dbo].[APSetup]
    SET [UsrEnablePOBillRestriction] = 0
    WHERE [UsrEnablePOBillRestriction] IS NULL
    
    PRINT 'Column UsrEnablePOBillRestriction added successfully to APSetup table'
END
ELSE
BEGIN
    PRINT 'Column UsrEnablePOBillRestriction already exists in APSetup table'
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[APSetup]') AND name = 'UsrPOBillAmountTolerance')
BEGIN
    ALTER TABLE [dbo].[APSetup]
    ADD [UsrPOBillAmountTolerance] decimal(19, 2) NULL
    
    -- Set default value to 0.01 for existing records
    UPDATE [dbo].[APSetup]
    SET [UsrPOBillAmountTolerance] = 0.01
    WHERE [UsrPOBillAmountTolerance] IS NULL
    
    PRINT 'Column UsrPOBillAmountTolerance added successfully to APSetup table'
END
ELSE
BEGIN
    PRINT 'Column UsrPOBillAmountTolerance already exists in APSetup table'
END
GO

-- Verify columns were added
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'APSetup' 
  AND COLUMN_NAME IN ('UsrEnablePOBillRestriction', 'UsrPOBillAmountTolerance')
ORDER BY COLUMN_NAME
GO

PRINT 'Database update completed successfully!'
GO
