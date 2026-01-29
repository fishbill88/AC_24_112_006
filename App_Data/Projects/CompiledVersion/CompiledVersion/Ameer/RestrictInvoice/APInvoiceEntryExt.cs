using PX.Data;
using PX.Objects.AP;
using PX.Objects.PO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RestrictInvoice
{
    /// <summary>
    /// Extension for APInvoiceEntry to enforce PO-Bill restrictions
    /// </summary>
    public class APInvoiceEntryExt : PXGraphExtension<APInvoiceEntry>
    {
        #region Data Views

        public PXSetup<APSetup> APSetupView;

        #endregion

        #region Delegate Overrides

        public delegate IEnumerable ReleaseDelegate(PXAdapter adapter);

        [PXOverride]
        public IEnumerable Release(PXAdapter adapter, ReleaseDelegate baseMethod)
        {
            // Get setup configuration
            APSetup setup = APSetupView.Current;
            APSetupExt setupExt = setup != null ? PXCache<APSetup>.GetExtension<APSetupExt>(setup) : null;

            // Only validate if restriction is enabled
            if (setupExt?.UsrEnablePOBillRestriction == true)
            {
                APInvoice currentBill = Base.Document.Current;
                if (currentBill != null)
                {
                    ValidateBillBeforeRelease(currentBill, setupExt.UsrPOBillAmountTolerance ?? 0.01m);
                }
            }

            // Call base release method
            return baseMethod(adapter);
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Main validation method called before bill release
        /// </summary>
        protected virtual void ValidateBillBeforeRelease(APInvoice bill, decimal amountTolerance)
        {
            if (bill == null) return;

            // Get all transaction lines with PO connections from current bill
            var connectedLines = PXSelect<APTran,
                Where<APTran.tranType, Equal<Required<APTran.tranType>>,
                    And<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                    And<APTran.pONbr, IsNotNull>>>>
                .Select(Base, bill.DocType, bill.RefNbr)
                .RowCast<APTran>()
                .ToList();

            // If no PO connections, no validation needed
            if (!connectedLines.Any())
                return;

            // Group by PO and validate each PO separately
            var poGroups = connectedLines.GroupBy(l => new { l.POOrderType, l.PONbr });

            foreach (var poGroup in poGroups)
            {
                if (string.IsNullOrEmpty(poGroup.Key.PONbr))
                    continue;

                ValidateOneBillPerPO(bill, poGroup.Key.POOrderType, poGroup.Key.PONbr);
                ValidateOneReceiptPerPO(bill, poGroup.ToList(), poGroup.Key.POOrderType, poGroup.Key.PONbr);
                ValidateBillAmountMatchesPO(bill, poGroup.ToList(), poGroup.Key.POOrderType, poGroup.Key.PONbr, amountTolerance);
            }
        }

        /// <summary>
        /// Validate that only one bill exists per PO
        /// </summary>
        protected virtual void ValidateOneBillPerPO(APInvoice currentBill, string poOrderType, string poNbr)
        {
            // Check if other bills exist for this PO (excluding current bill)
            var otherBills = PXSelect<APTran,
                Where<APTran.pOOrderType, Equal<Required<APTran.pOOrderType>>,
                    And<APTran.pONbr, Equal<Required<APTran.pONbr>>,
                    And<Where<APTran.tranType, NotEqual<Required<APTran.tranType>>,
                        Or<APTran.refNbr, NotEqual<Required<APTran.refNbr>>>>>>>>
                .Select(Base, poOrderType, poNbr, currentBill.DocType, currentBill.RefNbr)
                .RowCast<APTran>()
                .Where(t => t.TranType == "INV" || t.TranType == "ADR") // Only invoices and adjustments
                .GroupBy(t => new { t.TranType, t.RefNbr })
                .ToList();

            if (otherBills.Any())
            {
                var firstOtherBill = otherBills.First().First();
                throw new PXException(
                    $"Cannot release bill. Purchase Order {poNbr} is already connected to another bill: " +
                    $"{firstOtherBill.TranType} {firstOtherBill.RefNbr}. " +
                    "Only one bill per PO is allowed per setup configuration.");
            }
        }

        /// <summary>
        /// Validate that only one receipt exists per PO
        /// </summary>
        protected virtual void ValidateOneReceiptPerPO(APInvoice currentBill, List<APTran> poLines, string poOrderType, string poNbr)
        {
            // Get distinct receipts from current bill's lines for this PO
            var receiptsInCurrentBill = poLines
                .Where(l => !string.IsNullOrEmpty(l.ReceiptNbr))
                .Select(l => new { l.ReceiptType, l.ReceiptNbr })
                .Distinct()
                .ToList();

            if (receiptsInCurrentBill.Count > 1)
            {
                var receiptList = string.Join(", ", receiptsInCurrentBill.Select(r => $"{r.ReceiptType} {r.ReceiptNbr}"));
                throw new PXException(
                    $"Cannot release bill. Purchase Order {poNbr} is connected to multiple receipts: {receiptList}. " +
                    "Only one receipt per PO is allowed (1 PO = 1 Bill = 1 Receipt).");
            }

            // Check if other bills have different receipts for the same PO
            if (receiptsInCurrentBill.Any())
            {
                var currentReceipt = receiptsInCurrentBill.First();

                var otherReceipts = PXSelect<APTran,
                    Where<APTran.pOOrderType, Equal<Required<APTran.pOOrderType>>,
                        And<APTran.pONbr, Equal<Required<APTran.pONbr>>,
                        And<APTran.receiptNbr, IsNotNull,
                        And<Where<APTran.tranType, NotEqual<Required<APTran.tranType>>,
                            Or<APTran.refNbr, NotEqual<Required<APTran.refNbr>>>>>>>>>
                    .Select(Base, poOrderType, poNbr, currentBill.DocType, currentBill.RefNbr)
                    .RowCast<APTran>()
                    .Where(t => t.ReceiptType != currentReceipt.ReceiptType || t.ReceiptNbr != currentReceipt.ReceiptNbr)
                    .Select(t => new { t.ReceiptType, t.ReceiptNbr, t.TranType, t.RefNbr })
                    .Distinct()
                    .ToList();

                if (otherReceipts.Any())
                {
                    var otherReceipt = otherReceipts.First();
                    throw new PXException(
                        $"Cannot release bill. Purchase Order {poNbr} is already connected to a different receipt " +
                        $"{otherReceipt.ReceiptType} {otherReceipt.ReceiptNbr} in bill {otherReceipt.TranType} {otherReceipt.RefNbr}. " +
                        "Only one receipt per PO is allowed (1 PO = 1 Bill = 1 Receipt).");
                }
            }
        }

        /// <summary>
        /// Validate that bill amount matches PO amount within tolerance
        /// </summary>
        protected virtual void ValidateBillAmountMatchesPO(APInvoice currentBill, List<APTran> poLines, string poOrderType, string poNbr, decimal tolerance)
        {
            // Get PO Order
            POOrder poOrder = PXSelect<POOrder,
                Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
                    And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>
                .Select(Base, poOrderType, poNbr);

            if (poOrder == null)
            {
                throw new PXException($"Purchase Order {poNbr} not found.");
            }

            // Calculate bill total for this PO (sum of all lines connected to this PO)
            decimal billTotal = poLines.Sum(l => l.CuryTranAmt ?? 0m);
            decimal poTotal = poOrder.CuryOrderTotal ?? 0m;

            // Check if amounts match within tolerance
            decimal difference = Math.Abs(billTotal - poTotal);

            if (difference > tolerance)
            {
                throw new PXException(
                    $"Cannot release bill. Bill amount for PO {poNbr} ({billTotal:N2}) does not match " +
                    $"PO amount ({poTotal:N2}). Difference: {difference:N2}, Allowed tolerance: {tolerance:N2}. " +
                    "Bill and PO amounts must match within configured tolerance.");
            }
        }

        #endregion
    }
}
