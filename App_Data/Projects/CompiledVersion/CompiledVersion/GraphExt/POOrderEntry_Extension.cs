using CompiledVersion.DAC;
using PX.Data;
using PX.Common;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.Common.DAC;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using System;
using System.Linq;
using static PX.Data.BQL.BqlPlaceholder;
using static PX.Objects.PO.POOrderEntry;

namespace CompiledVersion.Graphs
{
    public class POOrderEntry_Extension : PXGraphExtension<PX.Objects.PO.POOrderEntry>
    {
        public static bool IsActive() => true;
        public bool skipCostDefaulting = false;

        public PXSetup<SOSetup> sosetup;
        public PXSelect<SOOrder> allsoorder;

        [PXCopyPasteHiddenView()]
        public PXSelect<DropShipLink,
      Where<DropShipLink.pOOrderType, Equal<Required<POLine.orderType>>,
        And<DropShipLink.pOOrderNbr, Equal<Required<POLine.orderNbr>>,
        And<DropShipLink.pOLineNbr, Equal<Required<POLine.lineNbr>>>>>> STDropShipLinks;

        private bool _postPersistSaveInProgress;

        #region Methods
        public virtual DropShipLink GetDropShipLink(POLine line)
        {
            if (line == null || !POLineType.IsDropShip(line.LineType))
                return null;

            return STDropShipLinks.SelectWindowed(0, 1, line.OrderType, line.OrderNbr, line.LineNbr);
        }

        private void RecalculateRTHHeaderTotals()
        {
            var order = Base.Document.Current;
            if (order == null) return;
            var orderExt = order.GetExtension<POOrderExt>();

            var lines = Base.Transactions.Select().RowCast<POLine>()
                .Where(l => l.GetExtension<POLineExt>()?.UsrPrepaymentLine != true);

            orderExt.UsrRTHDetailTotal = lines.Sum(l => l.CuryLineAmt ?? 0m);
            orderExt.UsrRTHLineDiscount = lines.Sum(l => l.CuryDiscAmt ?? 0m);
            // For Doc Discount and Tax, you may need to recalculate or copy as needed. Here, set to0 for now.
            orderExt.UsrRTHDocDiscount = order.CuryDiscTot;
            orderExt.UsrRTHTaxTotal = order.CuryTaxTotal;
            orderExt.UsrRTHOrderTotal = (orderExt.UsrRTHDetailTotal ?? 0m)
                - (orderExt.UsrRTHLineDiscount ?? 0m)
                - (orderExt.UsrRTHDocDiscount ?? 0m)
                + (orderExt.UsrRTHTaxTotal ?? 0m);

            Base.Document.Cache.SetValueExt<POOrderExt.usrRTHDetailTotal>(order, orderExt.UsrRTHDetailTotal);
            Base.Document.Cache.SetValueExt<POOrderExt.usrRTHLineDiscount>(order, orderExt.UsrRTHLineDiscount);
            Base.Document.Cache.SetValueExt<POOrderExt.usrRTHDocDiscount>(order, orderExt.UsrRTHDocDiscount);
            Base.Document.Cache.SetValueExt<POOrderExt.usrRTHTaxTotal>(order, orderExt.UsrRTHTaxTotal);
            Base.Document.Cache.SetValueExt<POOrderExt.usrRTHOrderTotal>(order, orderExt.UsrRTHOrderTotal);

        }

        private bool ForceCopyHeaderNoteWithPXDatabase(POOrder poOrder, SOOrder sourceSO, PXCache poCache, PXCache soCache)
        {
            if (poOrder == null || sourceSO == null) return false;
            // Ensure PO has a NoteID
            PXNoteAttribute.GetNoteID<POOrder.noteID>(poCache, poOrder);

            string srcNote = PXNoteAttribute.GetNote(soCache, sourceSO);
            if (string.IsNullOrWhiteSpace(srcNote))
                return false;

            // Direct DB update as a last resort
            if (poOrder.NoteID != null)
            {
                PXDatabase.Update<Note>(
                    new PXDataFieldAssign(nameof(Note.NoteText), PXDbType.NVarChar, srcNote),
                    new PXDataFieldRestrict(nameof(Note.NoteID), PXDbType.UniqueIdentifier, poOrder.NoteID));

                // Keep cache/UI in sync
                PXNoteAttribute.SetNote(poCache, poOrder, srcNote);
                poCache.Update(poOrder);
                return true;
            }
            return false;
        }

        private static decimal RoundByPrecision(decimal value, int precision)
        {
            return Math.Round(value, precision < 0 ? 2 : precision, MidpointRounding.AwayFromZero);
        }

        private decimal RoundCury(POOrder order, decimal value)
        {
            try
            {
                if (order?.CuryInfoID != null)
                {
                    var ci = Base.FindImplementation<IPXCurrencyHelper>()?.GetCurrencyInfo(order.CuryInfoID);
                    int prec = ci?.GetCM()?.CuryPrecision ?? 2;
                    return RoundByPrecision(value, prec);
                }
            }
            catch { }
            return RoundByPrecision(value, 2);
        }

        private void EnsureExtCostAndUnitCostFailsafe(PXCache cache, POLine line, bool raiseErrors)
        {
            if (line == null) return;

            var order = Base.Document.Current;
            var qty = line.OrderQty ?? 0m;
            var unitCost = line.CuryUnitCost ?? 0m;
            var ext = line.CuryExtCost ?? 0m;

            var lineExt = line.GetExtension<POLineExt>();
            var rthUnit = lineExt?.UsrSWKRTHCost ?? 0m;

            PXTrace.WriteInformation($"=== EnsureExtCostAndUnitCostFailsafe START === POLine {line.OrderType}-{line.OrderNbr}-{line.LineNbr}, Inv: {line.InventoryID}");
            PXTrace.WriteInformation($"Initial Values: Qty={qty}, UnitCost={unitCost}, ExtCost={ext}, RTHUnitCost={rthUnit}, skipCostDefaulting={skipCostDefaulting}");

            // =====================================================================================
            // Price Hierarchy Re-Evaluation (SPC Cost -> Vendor Price -> RTH Cost -> Last Cost)
            // Only attempt if not explicitly skipping cost defaulting.
            // =====================================================================================
            if (!skipCostDefaulting)
            {
                decimal? expectedUnitCost = null;
                bool expectedIsVendorOrSPC = false; // determines bypass of RTH enforcement
                string costSource = "None";

                //1) SPC Cost (from linked SO line hierarchy)
                try
                {
                    DropShipLink ds0 = GetDropShipLink(line);
                    if (ds0 != null)
                    {
                        SOLine soLine0 = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>, And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>.Select(Base, ds0.SOOrderType, ds0.SOOrderNbr, ds0.SOLineNbr);
                        if (soLine0 != null)
                        {
                            var soLineExt0 = soLine0.GetExtension<SOLineExt>();
                            if (soLineExt0?.UsrSWKSPCCost > 0m)
                            {
                                expectedUnitCost = soLineExt0.UsrSWKSPCCost;
                                expectedIsVendorOrSPC = true;
                                costSource = "SPC Cost (DropShip link)";
                                PXTrace.WriteInformation($"✓ Using SPC Cost (DropShip link): {expectedUnitCost}");
                            }
                        }
                    }

                    if (!expectedUnitCost.HasValue && POLineType.IsDropShip(line.LineType))
                    {
                        var allDropShipMatches = PXSelectJoin<SOLine,
                            InnerJoin<SOLineSplit, On<SOLine.orderType, Equal<SOLineSplit.orderType>,
                                And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
                                And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
                            Where<SOLineSplit.pOCreate, Equal<True>,
                                And<SOLineSplit.inventoryID, Equal<Required<SOLineSplit.inventoryID>>,
                                And<SOLineSplit.siteID, Equal<Required<SOLineSplit.siteID>>,
                                And<SOLineSplit.vendorID, Equal<Required<SOLineSplit.vendorID>>>>>>>
                            .Select(Base, line.InventoryID, line.SiteID, order?.VendorID)
                            .RowCast<SOLine>()
                            .ToList();
                        if (allDropShipMatches.Any())
                        {
                            var best = allDropShipMatches.OrderByDescending(so => so.GetExtension<SOLineExt>()?.UsrSWKSPCCost ?? 0m).First();
                            var bExt = best.GetExtension<SOLineExt>();
                            if (bExt?.UsrSWKSPCCost > 0m)
                            {
                                expectedUnitCost = bExt.UsrSWKSPCCost;
                                expectedIsVendorOrSPC = true;
                                costSource = "SPC Cost (DropShip search)";
                                PXTrace.WriteInformation($"✓ Using SPC Cost (search): {expectedUnitCost}");
                            }
                        }
                    }
                    if (!expectedUnitCost.HasValue && !POLineType.IsDropShip(line.LineType))
                    {
                        if (line.OrderType != null && line.OrderNbr != null && line.LineNbr != null)
                        {
                            var soSplitLinked = PXSelectJoin<SOLine,
                                InnerJoin<SOLineSplit, On<SOLine.orderType, Equal<SOLineSplit.orderType>,
                                    And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
                                    And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
                                Where<SOLineSplit.pOType, Equal<Required<SOLineSplit.pOType>>,
                                    And<SOLineSplit.pONbr, Equal<Required<SOLineSplit.pONbr>>,
                                    And<SOLineSplit.pOLineNbr, Equal<Required<SOLineSplit.pOLineNbr>>>>>>
                            .Select(Base, line.OrderType, line.OrderNbr, line.LineNbr)
                            .RowCast<SOLine>()
                            .ToList();
                            if (soSplitLinked.Any())
                            {
                                var best = soSplitLinked.OrderByDescending(so => so.GetExtension<SOLineExt>()?.UsrSWKSPCCost ?? 0m).First();
                                var bExt = best.GetExtension<SOLineExt>();
                                if (bExt?.UsrSWKSPCCost > 0m)
                                {
                                    expectedUnitCost = bExt.UsrSWKSPCCost;
                                    expectedIsVendorOrSPC = true;
                                    costSource = "SPC Cost (linked)";
                                    PXTrace.WriteInformation($"✓ Using SPC Cost (linked): {expectedUnitCost}");
                                }
                            }
                        }
                    }
                    if (!expectedUnitCost.HasValue && line.InventoryID != null && line.SiteID != null && order?.VendorID != null)
                    {
                        var soSplitPending = PXSelectJoin<SOLine,
                            InnerJoin<SOLineSplit, On<SOLine.orderType, Equal<SOLineSplit.orderType>,
                                And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
                                And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
                            Where<SOLineSplit.pOCreate, Equal<True>,
                                And<SOLineSplit.inventoryID, Equal<Required<SOLineSplit.inventoryID>>,
                                And<SOLineSplit.siteID, Equal<Required<SOLineSplit.siteID>>,
                                And<SOLineSplit.vendorID, Equal<Required<SOLineSplit.vendorID>>,
                                And<SOLineSplit.pONbr, IsNull>>>>>>
                            .Select(Base, line.InventoryID, line.SiteID, order.VendorID)
                            .RowCast<SOLine>()
                            .ToList();
                        if (soSplitPending.Any())
                        {
                            var best = soSplitPending.OrderByDescending(so => so.GetExtension<SOLineExt>()?.UsrSWKSPCCost ?? 0m).First();
                            var bExt = best.GetExtension<SOLineExt>();
                            if (bExt?.UsrSWKSPCCost > 0m)
                            {
                                expectedUnitCost = bExt.UsrSWKSPCCost;
                                expectedIsVendorOrSPC = true;
                                costSource = "SPC Cost (pending)";
                                PXTrace.WriteInformation($"✓ Using SPC Cost (pending): {expectedUnitCost}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    PXTrace.WriteError($"SPC Cost Lookup Failed: {ex.Message}");
                }

                //2) Vendor Price (if SPC not found)
                if (!expectedUnitCost.HasValue && line.UOM != null && order?.VendorID != null && order?.CuryInfoID != null)
                {
                    try
                    {
                        var ci = Base.FindImplementation<IPXCurrencyHelper>()?.GetCurrencyInfo(order.CuryInfoID);
                        var vendCost = APVendorPriceMaint.CalculateUnitCost(cache, order.VendorID, order.VendorLocationID, line.InventoryID, line.SiteID, ci?.GetCM(), line.UOM, qty, order.OrderDate ?? Base.Accessinfo.BusinessDate.GetValueOrDefault(), line.CuryUnitCost);
                        PXTrace.WriteInformation($"Vendor Price Lookup: VendorID={order.VendorID}, Result={vendCost}");
                        if (vendCost.HasValue && vendCost.Value > 0m)
                        {
                            expectedUnitCost = vendCost.Value;
                            expectedIsVendorOrSPC = true; // vendor bypasses RTH floor
                            costSource = "Vendor Price";
                            PXTrace.WriteInformation($"✓ Using Vendor Price: {vendCost.Value}");
                        }
                    }
                    catch (Exception ex)
                    {
                        PXTrace.WriteError($"Vendor Price Lookup Failed: {ex.Message}");
                    }
                }

                //3) RTH Cost
                if (!expectedUnitCost.HasValue)
                {
                    try
                    {
                        decimal rth = lineExt?.UsrSWKRTHCost ?? 0m;
                        PXTrace.WriteInformation($"RTH Cost from POLineExt: {rth}");
                        if (rth <= 0m && line.InventoryID.HasValue)
                        {
                            InventoryItem item = InventoryItem.PK.Find(Base, line.InventoryID);
                            var itemExt = item?.GetExtension<InventoryItemExt>();
                            if (itemExt?.UsrSWKRTHCost.HasValue == true)
                            {
                                rth = itemExt.UsrSWKRTHCost.Value;
                                PXTrace.WriteInformation($"RTH Cost from InventoryItem: {rth}");
                            }
                        }
                        if (rth > 0m)
                        {
                            expectedUnitCost = rth;
                            costSource = "RTH Cost";
                            PXTrace.WriteInformation($"✓ Using RTH Cost: {rth}");
                        }
                    }
                    catch (Exception ex)
                    {
                        PXTrace.WriteError($"RTH Cost Lookup Failed: {ex.Message}");
                    }
                }

                //4) Last Cost fallback
                if (!expectedUnitCost.HasValue)
                {
                    try
                    {
                        expectedUnitCost = POItemCostManager.Fetch<POLine.inventoryID, POLine.curyInfoID>(cache.Graph, line, order?.VendorID, order?.VendorLocationID, order?.OrderDate, order?.CuryID, line.InventoryID, line.SubItemID, line.SiteID, line.UOM);
                        PXTrace.WriteInformation($"Last Cost Lookup Result: {expectedUnitCost}");
                        if (expectedUnitCost.HasValue)
                        {
                            costSource = "Last Cost";
                            PXTrace.WriteInformation($"✓ Using Last Cost: {expectedUnitCost.Value}");
                        }
                    }
                    catch (Exception ex)
                    {
                        PXTrace.WriteError($"Last Cost Lookup Failed: {ex.Message}");
                    }
                }

                PXTrace.WriteInformation($"Cost Source Selected: {costSource}, Value: {expectedUnitCost}, IsVendorOrSPC: {expectedIsVendorOrSPC}");

                // Apply expected unit cost if materially different
                if (expectedUnitCost.HasValue && Math.Abs(unitCost - expectedUnitCost.Value) > 0.0000001m)
                {
                    PXTrace.WriteInformation($"APPLY UnitCost: {unitCost} -> {expectedUnitCost.Value}");
                    cache.SetValueExt<POLine.curyUnitCost>(line, expectedUnitCost.Value);
                    unitCost = expectedUnitCost.Value;
                }
                else
                {
                    PXTrace.WriteInformation($"UnitCost unchanged: Current={unitCost}, Expected={expectedUnitCost}");
                }

                // Maintain flag for downstream RTH enforcement
                lineExt.UsrUsedVendorPrice = expectedIsVendorOrSPC;
                PXTrace.WriteInformation($"SET UsrUsedVendorPrice = {expectedIsVendorOrSPC}");
            }
            else
            {
                PXTrace.WriteInformation("Cost hierarchy evaluation SKIPPED (skipCostDefaulting = true)");
            }

            // If vendor price or SPC cost was used, honor it: do not bump to RTH, only ensure non-negative
            if (lineExt?.UsrUsedVendorPrice == true)
            {
                PXTrace.WriteInformation("Vendor/SPC price used - RTH enforcement BYPASSED, only checking non-negative");
                if (unitCost < 0m)
                {
                    PXTrace.WriteInformation($"UnitCost negative ({unitCost}), setting to0");
                    cache.SetValueExt<POLine.curyUnitCost>(line, 0m);
                    unitCost = 0m;
                }
            }
            else
            {
                PXTrace.WriteInformation($"RTH Enforcement Active: RTHUnitCost={rthUnit}, CurrentUnitCost={unitCost}");
                // Ensure UnitCost is not below RTH Unit Cost
                if (rthUnit > 0m && unitCost < rthUnit)
                {
                    var newUnit = rthUnit;
                    PXTrace.WriteInformation($"UnitCost below RTH minimum! RAISE to RTH: {unitCost} -> {newUnit}");
                    cache.RaiseExceptionHandling<POLine.curyUnitCost>(line, unitCost,
                    new PXSetPropertyException(line, Messages.UnitCostIncreasedToRTH, PXErrorLevel.Warning));
                    cache.SetValueExt<POLine.curyUnitCost>(line, newUnit);
                    unitCost = newUnit;
                }
            }

            // Failsafe: re-calc Extended Cost from (UnitCost * Qty)
            var expectedExt = RoundCury(order, unitCost * qty);
            PXTrace.WriteInformation($"ExtCost Recalculation: UnitCost({unitCost}) × Qty({qty}) = {unitCost * qty}, Rounded = {expectedExt}");
            if (Math.Abs(ext - expectedExt) > 0.009m || line.CuryExtCost == null)
            {
                PXTrace.WriteInformation($"APPLY ExtCost: {ext} -> {expectedExt} (Diff: {Math.Abs(ext - expectedExt)}, IsNull: {line.CuryExtCost == null})");
                cache.SetValueExt<POLine.curyExtCost>(line, expectedExt);
                ext = expectedExt;
            }
            else
            {
                PXTrace.WriteInformation($"ExtCost unchanged: Current={ext}, Expected={expectedExt}");
            }

            // Enforce RTH minimum on ExtCost only if vendor price/SPC not used
            if (lineExt?.UsrUsedVendorPrice != true)
            {
                var rthMin = RoundCury(order, (lineExt?.UsrSWKRTHCost ?? 0m) * qty);
                PXTrace.WriteInformation($"RTH ExtCost Minimum Check: Current ExtCost={ext}, RTH Minimum={rthMin}");
                if (ext + 0.009m < rthMin)
                {
                    var msg = Messages.ExtCostBelowRTH;
                    PXTrace.WriteWarning($"ExtCost below RTH minimum! ExtCost={ext}, RTHMin={rthMin}, raiseErrors={raiseErrors}");
                    cache.RaiseExceptionHandling<POLine.curyExtCost>(line, ext,
                    new PXSetPropertyException(line, msg, raiseErrors ? PXErrorLevel.Error : PXErrorLevel.Warning));
                    if (raiseErrors)
                    {
                        PXTrace.WriteError($"THROW exception: {msg}");
                        throw new PXSetPropertyException(line, msg);
                    }
                    // Force to minimum if warning-only path
                    if (!raiseErrors)
                    {
                        cache.SetValueExt<POLine.curyExtCost>(line, rthMin);
                        ext = rthMin;
                        PXTrace.WriteInformation($"ExtCost raised to RTH minimum: {ext}");
                    }
                }
                else
                {
                    PXTrace.WriteInformation("ExtCost meets RTH minimum requirement");
                }
            }
            else
            {
                PXTrace.WriteInformation("RTH ExtCost enforcement SKIPPED (Vendor/SPC price was used)");
            }

            PXTrace.WriteInformation($"=== EnsureExtCostAndUnitCostFailsafe END === Final: UnitCost={unitCost}, ExtCost={ext}");
        }

        #endregion

        #region Events

        #region POLine
        protected virtual void _(Events.FieldDefaulting<POLine, POLineExt.usrSWKSPCCode> e)
        {
            if (e.Row == null) return;

            // Try to find the related SO line to copy SPC Code
            var soLine = PXSelectJoin<SOLine,
                InnerJoin<SOLineSplit, On<SOLine.orderType, Equal<SOLineSplit.orderType>,
                    And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
                    And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
                Where<SOLineSplit.pOType, Equal<Current<POLine.orderType>>,
                    And<SOLineSplit.pONbr, Equal<Current<POLine.orderNbr>>,
                    And<SOLineSplit.pOLineNbr, Equal<Current<POLine.lineNbr>>>>>>
                .SelectSingleBound(Base, new object[] { e.Row });

            if (soLine != null)
            {
                var soLineExt = ((SOLine)soLine).GetExtension<SOLineExt>();
                if (!string.IsNullOrEmpty(soLineExt?.UsrSWKSPCCode))
                {
                    e.NewValue = soLineExt.UsrSWKSPCCode;
                    e.Cancel = true;
                }
            }
        }
        protected virtual void _(Events.FieldUpdated<POLine, POLine.inventoryID> e)
        {
            if (e.Row == null) return;
            var poLineExt = e.Row.GetExtension<POLineExt>();
            InventoryItem item = InventoryItem.PK.Find(Base, e.Row.InventoryID);
            InventoryItemExt itemExt = item.GetExtension<InventoryItemExt>();
            poLineExt.UsrSWKRTHCost = (itemExt?.UsrSWKRTHCost ?? 0m);
        }
        protected virtual void POLine_CuryUnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (skipCostDefaulting)
            {
return;
            }

            POLine line = e.Row as POLine;
    POOrder order = Base.Document.Current;
    if (line == null || order == null || !line.InventoryID.HasValue)
   return;

    var lineExt = line.GetExtension<POLineExt>();
    lineExt.UsrUsedVendorPrice = false;

    // New hierarchy: SPC Cost -> Vendor Price -> RTH Cost -> Last Cost

    //1) SPC Cost
    decimal? spc = null;
 SOLine soLine = null;
    
    // ⭐ Try multiple strategies to find the SO line(s) - UPDATED for merged lines
    
    // Strategy 1: DropShipLink (works for existing drop-ship PO lines)
    DropShipLink ds = GetDropShipLink(line);
    if (ds != null)
    {
        soLine = PXSelect<SOLine, 
     Where<SOLine.orderType, Equal<Required<SOLine.orderType>>, 
 And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, 
        And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>.Select(Base, ds.SOOrderType, ds.SOOrderNbr, ds.SOLineNbr);
    }
 
    // Strategy 2: SOLineSplit lookup for drop-ship during Insert (when DropShipLink doesn't exist yet)
    // ⭐ UPDATED: Handle merged SO lines by selecting HIGHEST SPC cost
    if (soLine == null && POLineType.IsDropShip(line.LineType))
    {
   var allDropShipMatches = PXSelectJoin<SOLine,
       InnerJoin<SOLineSplit, On<SOLine.orderType, Equal<SOLineSplit.orderType>,
     And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
         Where<SOLineSplit.pOCreate, Equal<True>,
       And<SOLineSplit.inventoryID, Equal<Required<SOLineSplit.inventoryID>>,
       And<SOLineSplit.siteID, Equal<Required<SOLineSplit.siteID>>,
        And<SOLineSplit.vendorID, Equal<Required<SOLineSplit.vendorID>>>>>>>
  .Select(Base, line.InventoryID, line.SiteID, order.VendorID)
    .RowCast<SOLine>()
    .ToList();
        
        if (allDropShipMatches.Any())
        {
   // Pick SO line with highest SPC cost (handles merged lines)
         soLine = allDropShipMatches
    .OrderByDescending(so => so.GetExtension<SOLineExt>()?.UsrSWKSPCCost ?? 0m)
   .FirstOrDefault();
        }
    }

    // Strategy 3: SOLineSplit lookup for regular "Mark for PO" lines (works for both Insert and existing)
    // ⭐ UPDATED: Handle merged SO lines by selecting HIGHEST SPC cost
    if (soLine == null && !POLineType.IsDropShip(line.LineType))
    {
    // Try existing link first (for editing existing lines)
        if (line.OrderType != null && line.OrderNbr != null && line.LineNbr != null)
    {
            var soSplitLinked = PXSelectJoin<SOLine,
          InnerJoin<SOLineSplit, On<SOLine.orderType, Equal<SOLineSplit.orderType>,
       And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
        And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
           Where<SOLineSplit.pOType, Equal<Required<SOLineSplit.pOType>>,
And<SOLineSplit.pONbr, Equal<Required<SOLineSplit.pONbr>>,
     And<SOLineSplit.pOLineNbr, Equal<Required<SOLineSplit.pOLineNbr>>>>>>
     .Select(Base, line.OrderType, line.OrderNbr, line.LineNbr)
                .RowCast<SOLine>()
         .ToList();
 
      if (soSplitLinked.Any())
            {
       // Pick SO line with highest SPC cost (handles merged lines)
         soLine = soSplitLinked
         .OrderByDescending(so => so.GetExtension<SOLineExt>()?.UsrSWKSPCCost ?? 0m)
      .FirstOrDefault();
    }
        }
        
        // During Insert: Match by criteria (inventory/site/vendor, POCreate=true, not yet linked)
        if (soLine == null && line.InventoryID != null && line.SiteID != null && order.VendorID != null)
      {
          var soSplitPending = PXSelectJoin<SOLine,
      InnerJoin<SOLineSplit, On<SOLine.orderType, Equal<SOLineSplit.orderType>,
        And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
         And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
            Where<SOLineSplit.pOCreate, Equal<True>,
 And<SOLineSplit.inventoryID, Equal<Required<SOLineSplit.inventoryID>>,
      And<SOLineSplit.siteID, Equal<Required<SOLineSplit.siteID>>,
   And<SOLineSplit.vendorID, Equal<Required<SOLineSplit.vendorID>>,
          And<SOLineSplit.pONbr, IsNull>>>>>>
   .Select(Base, line.InventoryID, line.SiteID, order.VendorID)
  .RowCast<SOLine>()
      .ToList();
  
   if (soSplitPending.Any())
            {
           // Pick SO line with highest SPC cost (handles merged lines)
     soLine = soSplitPending
.OrderByDescending(so => so.GetExtension<SOLineExt>()?.UsrSWKSPCCost ?? 0m)
      .FirstOrDefault();
   }
        }
    }
  
    // Get SPC cost if SO line found
    if (soLine != null)
    {
 spc = soLine.GetExtension<CompiledVersion.DAC.SOLineExt>()?.UsrSWKSPCCost;
    }
    
    if (spc.HasValue && spc.Value > 0m)
    {
        e.NewValue = spc.Value;
        e.Cancel = true;
   // Treat SPC like vendor price for enforcement bypass
      lineExt.UsrUsedVendorPrice = true;
        return;
    }

    // ii) Vendor Price
    decimal? vendorUnit = null;
    if (line.UOM != null && order.VendorID != null)
    {
        var ci = Base.FindImplementation<IPXCurrencyHelper>()?.GetCurrencyInfo(order.CuryInfoID);
  vendorUnit = APVendorPriceMaint.CalculateUnitCost(sender, order.VendorID, order.VendorLocationID, line.InventoryID, line.SiteID, ci.GetCM(), line.UOM, line.OrderQty, order.OrderDate ?? Base.Accessinfo.BusinessDate.GetValueOrDefault(), line.CuryUnitCost);
        if (vendorUnit.HasValue && vendorUnit.Value > 0m)
        {
    e.NewValue = vendorUnit;
            e.Cancel = true;
    lineExt.UsrUsedVendorPrice = true; // vendor bypasses RTH floor
    return;
        }
    }

    // iii) RTH Cost from SO line or Item
    decimal? rth = lineExt?.UsrSWKRTHCost;
    if (!rth.HasValue || rth.Value <= 0m)
    {
        InventoryItem item = InventoryItem.PK.Find(Base, line.InventoryID);
        var itemExt = item?.GetExtension<InventoryItemExt>();
        rth = itemExt?.UsrSWKRTHCost;
    }
    if (rth.HasValue && rth.Value > 0m)
    {
    e.NewValue = rth.Value;
        e.Cancel = true;
    return;
    }

    // iv) Last Cost fallback
    e.NewValue = POItemCostManager.Fetch<POLine.inventoryID, POLine.curyInfoID>(sender.Graph, line, order.VendorID, order.VendorLocationID, order.OrderDate, order.CuryID, line.InventoryID, line.SubItemID, line.SiteID, line.UOM);
    APVendorPriceMaint.CheckNewUnitCost<POLine, POLine.curyUnitCost>(sender, line, e.NewValue);
}
        // Ensure Unit Cost is never below RTH Unit Cost
        //protected virtual void _(Events.FieldVerifying<POLine, POLine.curyUnitCost> e)
        //{
        //    if (e.Row == null) return;
        //    var line = e.Row as POLine;
        //    var lineExt = line?.GetExtension<POLineExt>();
        //    if (lineExt == null) return;

        //    // If vendor price was used, allow unit cost as-is (still clamp to >=0)
        //    if (lineExt.UsrUsedVendorPrice == true)
        //    {
        //        if (e.NewValue is decimal v && v < 0m)
        //            e.NewValue = 0m;
        //        return;
        //    }

        //    var rth = lineExt.UsrSWKRTHCost ?? 0m;
        //    if (rth > 0m && e.NewValue is decimal newUC && newUC < rth)
        //    {
        //        e.NewValue = rth;
        //        e.Cache.RaiseExceptionHandling<POLine.curyUnitCost>(line, newUC,
        //        new PXSetPropertyException(line, Messages.UnitCostRaisedToRTH, PXErrorLevel.Warning));
        //    }
        //}

        // Failsafe recalc when unit cost changes
        //protected virtual void _(Events.FieldUpdated<POLine, POLine.curyUnitCost> e)
        //{
        //    if (e.Row == null) return;
        //    EnsureExtCostAndUnitCostFailsafe(e.Cache, (POLine)e.Row, raiseErrors: false);
        //}

        // Failsafe recalc when quantity changes
        //protected virtual void _(Events.FieldUpdated<POLine, POLine.orderQty> e)
        //{
        //    if (e.Row == null) return;
        //    EnsureExtCostAndUnitCostFailsafe(e.Cache, (POLine)e.Row, raiseErrors: false);
        //}

        // Validate user-entered Extended Cost, and fix if off formula
        //protected virtual void _(Events.FieldVerifying<POLine, POLine.curyExtCost> e)
        //{
        //    if (e.Row == null) return;

        //    var line = (POLine)e.Row;
        //    var order = Base.Document.Current;
        //    var qty = line.OrderQty ?? 0m;
        //    var unitCost = line.CuryUnitCost ?? 0m;

        //    var expected = RoundCury(order, unitCost * qty);
        //    if (!(e.NewValue is decimal newExt)) newExt = 0m;

        //    // If new ext cost deviates from expected, snap back to expected
        //    if (Math.Abs(newExt - expected) > 0.009m)
        //    {
        //        e.NewValue = expected;
        //        e.Cache.RaiseExceptionHandling<POLine.curyExtCost>(line, newExt,
        //        new PXSetPropertyException(line, Messages.ExtCostAdjustedToFormula, PXErrorLevel.Warning));
        //    }

        //    // Make sure it is not below RTH minimum when vendor price not used
        //    var lineExt = line.GetExtension<POLineExt>();
        //    if (lineExt?.UsrUsedVendorPrice != true)
        //    {
        //        var rthUnit = lineExt?.UsrSWKRTHCost ?? 0m;
        //        var min = RoundCury(order, rthUnit * qty);
        //        if (newExt + 0.009m < min)
        //        {
        //            e.NewValue = min;
        //            e.Cache.RaiseExceptionHandling<POLine.curyExtCost>(line, newExt,
        //            new PXSetPropertyException(line, Messages.ExtCostRaisedToRTHMin, PXErrorLevel.Warning));
        //        }
        //    }
        //}

        private bool TryGetLinkedSOLine(POLine poLine, out SOLine soLine, out SOOrder soOrder)
        {
            soLine = null; soOrder = null;
            if (poLine == null) return false;

            if (POLineType.IsDropShip(poLine.LineType))
            {
                DropShipLink ds = PXSelect<DropShipLink,
                    Where<DropShipLink.pOOrderType, Equal<Required<DropShipLink.pOOrderType>>,
                      And<DropShipLink.pOOrderNbr, Equal<Required<DropShipLink.pOOrderNbr>>,
                      And<DropShipLink.pOLineNbr, Equal<Required<DropShipLink.pOLineNbr>>>>>>
                    .Select(Base, poLine.OrderType, poLine.OrderNbr, poLine.LineNbr);
                if (ds != null)
                {
                    soLine = PXSelect<SOLine,
                        Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                          And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
                          And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                        .Select(Base, ds.SOOrderType, ds.SOOrderNbr, ds.SOLineNbr);
                    if (soLine != null)
                        soOrder = PXSelect<SOOrder,
                            Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                              And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(Base, soLine.OrderType, soLine.OrderNbr);
                }
            }

            if (soLine == null)
            {
                PXResult<SOLine, SOLineSplit> soRes = (PXResult<SOLine, SOLineSplit>)PXSelectJoin<SOLine, InnerJoin<SOLineSplit,
                            On<SOLine.orderType, Equal<SOLineSplit.orderType>,
                            And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
                            And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
                        Where<SOLineSplit.pOType, Equal<Required<SOLineSplit.pOType>>,
                          And<SOLineSplit.pONbr, Equal<Required<SOLineSplit.pONbr>>,
                          And<SOLineSplit.pOLineNbr, Equal<Required<SOLineSplit.pOLineNbr>>>>>>
                    .Select(Base, poLine.OrderType, poLine.OrderNbr, poLine.LineNbr);
                if (soRes != null)
                {
                    soLine = soRes;
                    if (soLine != null)
                    {
                        soOrder = PXSelect<SOOrder, Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>, And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(Base, soLine.OrderType, soLine.OrderNbr);
                    }
                }
            }
            return soLine != null && soOrder != null;
        }

        //public delegate Boolean PrePersistDelegate();
        //[PXOverride]
        //public Boolean PrePersist(PrePersistDelegate baseMethod)
        //{
        //    // Call base first to ensure proper state
        //    var result = baseMethod();

        //    // Only copy notes after successful PrePersist validation and when not on PO entry screen
        //    if (result && Base.Accessinfo.ScreenID != "PO.30.10.00")
        //    {
        //        GetNotes();
        //    }

        //    return result;
        //}
        //public void GetNotes()
        //{
        //    // Prevent recursive saves
        //    if (_postPersistSaveInProgress)
        //        return;

        //    POOrder order = Base.CurrentDocument.Current;
        //    if (order == null)
        //        return;

        //    // Read setup once
        //    sosetup.Current = sosetup.Select();
        //    SOSetupExt setupExt = sosetup.Current?.GetExtension<SOSetupExt>();
        //    if (setupExt == null)
        //        return;

        //    PXCache orderCache = Base.Caches[typeof(POOrder)];
        //    PXCache lineCache = Base.Caches[typeof(POLine)];
        //    PXCache soOrderCache = Base.Caches[typeof(SOOrder)];
        //    PXCache soLineCache = Base.Caches[typeof(SOLine)];

        //    bool copiedAnything = false;
        //    bool headerCopiedThisSession = false;

        //    // Ensure NoteID exists for header before any copy
        //    PXNoteAttribute.GetNoteID<POOrder.noteID>(orderCache, order);

        //    // Process all lines available (retain current line-level logic)
        //    var allLines = PXSelect<POLine,
        //              Where<POLine.orderType, Equal<Required<POLine.orderType>>,
        //    And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>>>>.Select(Base, order.OrderType, order.OrderNbr).RowCast<POLine>();

        //    foreach (POLine line in allLines)
        //    {
        //        // Ensure destination NoteID exists for line before any copy
        //        PXNoteAttribute.GetNoteID<POLine.noteID>(lineCache, line);

        //        SOLine soLine;
        //        SOOrder soOrder;
        //        bool hasSO = TryGetLinkedSOLine(line, out soLine, out soOrder);
        //        if (!hasSO || soOrder == null)
        //            continue;

        //        // Copy header note and attachments once, as per old logic
        //        if (!headerCopiedThisSession)
        //        {
        //            if (setupExt.UsrCopyHeaderNotesToPO == true)
        //            {
        //                string noteText = PXNoteAttribute.GetNote(soOrderCache, soOrder);
        //                if (!string.IsNullOrEmpty(noteText))
        //                {
        //                    PXNoteAttribute.SetNote(orderCache, order, noteText);
        //                    ForceCopyHeaderNoteWithPXDatabase(order, soOrder, orderCache, soOrderCache);
        //                    copiedAnything = true;
        //                }
        //            }

        //            if (setupExt.UsrCopyHeaderAttachmentsToPO == true)
        //            {
        //                // Copy both notes and files to ensure attachments are moved, as in old code
        //                PXNoteAttribute.CopyNoteAndFiles(soOrderCache, soOrder, orderCache, order, true, true);
        //                orderCache.Update(order);
        //                copiedAnything = true;
        //            }

        //            headerCopiedThisSession = true;
        //        }

        //        // Retain current logic for line notes
        //        if (setupExt.UsrCopyLineNotesToPO == true)
        //        {
        //            string destNote = PXNoteAttribute.GetNote(lineCache, line);
        //            if (string.IsNullOrWhiteSpace(destNote))
        //            {
        //                PXNoteAttribute.CopyNoteAndFiles(soLineCache, soLine, lineCache, line, true, false);
        //                lineCache.Update(line);
        //                copiedAnything = true;
        //            }
        //        }

        //        // Retain current logic for line attachments
        //        if (setupExt.UsrCopyLineAttachmentsToPO == true)
        //        {
        //            bool lineHasFiles = (PXNoteAttribute.GetFileNotes(lineCache, line)?.Any() ?? false);
        //            if (!lineHasFiles)
        //            {
        //                PXNoteAttribute.CopyNoteAndFiles(soLineCache, soLine, lineCache, line, false, true);
        //                lineCache.Update(line);
        //                copiedAnything = true;
        //            }
        //        }
        //    }

        //    if (copiedAnything)
        //    {
        //        _postPersistSaveInProgress = true;
        //        try
        //        {
        //            Base.Caches[typeof(POOrder)].Persist(PXDBOperation.Update);
        //        }
        //        finally
        //        {
        //            _postPersistSaveInProgress = false;
        //        }
        //    }
        //}

        private SOOrder GetSourceSOOrder(POOrder order)
        {
            if (order == null) return null;

            // 1) If header has SO keys, use them
            var soFromHeader = FindSOOrder(order);
            if (soFromHeader != null) return soFromHeader;

            // 2) Try drop-ship link for this PO
            DropShipLink ds = PXSelect<DropShipLink,
                Where<DropShipLink.pOOrderType, Equal<Required<DropShipLink.pOOrderType>>,
                  And<DropShipLink.pOOrderNbr, Equal<Required<DropShipLink.pOOrderNbr>>>>>
                .SelectWindowed(Base, 0, 1, order.OrderType, order.OrderNbr);
            if (ds != null)
            {
                SOOrder so = PXSelect<SOOrder,
                    Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                      And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(Base, ds.SOOrderType, ds.SOOrderNbr);
                if (so != null) return so;
            }

            // 3) Try SOLineSplit referencing this PO
            var splitRes = PXSelect<SOLineSplit,
                Where<SOLineSplit.pOType, Equal<Required<SOLineSplit.pOType>>,
                  And<SOLineSplit.pONbr, Equal<Required<SOLineSplit.pONbr>>>>>
                .SelectWindowed(Base, 0, 1, order.OrderType, order.OrderNbr);
            SOLineSplit split = (splitRes != null && splitRes.Count > 0) ? (SOLineSplit)splitRes[0] : null;
            if (split != null)
            {
                SOOrder so = PXSelect<SOOrder,
                    Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                      And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(Base, split.OrderType, split.OrderNbr);
                if (so != null) return so;
            }

            return null;
        }

        private SOOrder FindSOOrder(POOrder order)
        {
            if (order?.SOOrderType == null || order.SOOrderNbr == null)
                return null;
            return PXSelect<SOOrder,
                        Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                            And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(Base, order.SOOrderType, order.SOOrderNbr);
        }

        protected virtual void _(Events.RowSelecting<POLine> e, PXRowSelecting baseMethod)
        {

            POOrder order = Base.CurrentDocument.Current;
            POLine line = (POLine)e.Row;
            if (line == null) return;

            using (new PXConnectionScope())
            {
                POOrderExt poExt = order.GetExtension<POOrderExt>();
                POLineExt lineExt = line.GetExtension<POLineExt>();
                sosetup.Current = sosetup.Select();
                SOSetupExt sOSetupExt = sosetup.Current.GetExtension<SOSetupExt>();
                DropShipLink link = GetDropShipLink(line);

                //show the freight cost and price only if SO shipTermsID = UsrPrepayAndAdd or UsrFreeFreightAllowed
                if (link != null && link.POOrderType == line.OrderType && link.POOrderNbr == line.OrderNbr && link.POLineNbr == line.LineNbr)
                {
                    SOOrder soOrder = PXSelect<SOOrder,
                        Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                            And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(Base, link.SOOrderType, link.SOOrderNbr);
                    SOOrderExt sOOrderExt = soOrder.GetExtension<SOOrderExt>();

                    poExt.UsrShipTermsIDTemp = soOrder.ShipTermsID;
                    poExt.UsrShowFreightCost = sOSetupExt.UsrPrepayAndAdd == soOrder.ShipTermsID ||
                       sOSetupExt.UsrFreeFreightAllowed == soOrder.ShipTermsID;
                    poExt.UsrShowFreightPrice = sOSetupExt.UsrFreeFreightAllowed == soOrder.ShipTermsID;
                    poExt.UsrShippingInstructions = sOOrderExt.UsrShippingInstructions;
                }

                if (e.Row != null && link != null)
                {
                    SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                     And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                                     .Select(Base, link.SOOrderType, link.SOOrderNbr, link.SOLineNbr);
                    if (soLine == null) return;
                    lineExt.UsrShippingTerms = soLine?.ShipComplete;
                    e.Cache.SetValueExt<POLineExt.usrShippingTerms>(line, soLine?.ShipComplete);
                }
            }
            baseMethod?.Invoke(e.Cache, e.Args);


        }
        protected virtual void _(Events.RowSelected<POLine> e, PXRowSelected baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            POLine line = (POLine)e.Row;
            if (line == null) return;

            PXUIFieldAttribute.SetEnabled<POLineExt.usrShippingTerms>(e.Cache, line, false);

            PXException warningVendorSpecTerms = null;
            PXException warningVendorNotes = null;
            PXException warningShippingTerms = null;

            DropShipLink link = GetDropShipLink(line);

            POLineExt lineExt = line.GetExtension<POLineExt>();
            if (lineExt.UsrVendorSpecTerms != null)
            {
                warningVendorSpecTerms = new PXSetPropertyException(line, Messages.Space, PXErrorLevel.Warning);
            }
            if (lineExt.UsrVendorNotes != null)
            {
                warningVendorNotes = new PXSetPropertyException(line, Messages.Space, PXErrorLevel.Warning);
            }
            if (lineExt.UsrShippingTerms != null)
            {
                warningShippingTerms = new PXSetPropertyException(line, Messages.Space, PXErrorLevel.Warning);
            }
            e.Cache.RaiseExceptionHandling<POLineExt.usrVendorSpecTerms>(e.Row, lineExt.UsrVendorSpecTerms, warningVendorSpecTerms);
            e.Cache.RaiseExceptionHandling<POLineExt.usrVendorNotes>(e.Row, lineExt.UsrVendorNotes, warningVendorNotes);
            e.Cache.RaiseExceptionHandling<POLineExt.usrShippingTerms>(e.Row, lineExt.UsrShippingTerms, warningShippingTerms);

            SOOrderTypeExt typeExt = null;
            if (e.Row != null && link != null)
            {
                SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                 And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                                 .Select(Base, link.SOOrderType, link.SOOrderNbr, link.SOLineNbr);
                if (soLine == null) return;

                SOOrderType orderType = SOOrderType.PK.Find(Base, link.SOOrderType);
                typeExt = orderType.GetExtension<SOOrderTypeExt>();
            }

            PXUIFieldAttribute.SetVisible<POLineExt.usrVendorID>(e.Cache, null, typeExt?.UsrShowVendorID ?? false);
            PXUIFieldAttribute.SetVisible<POLineExt.usrVendorLocationID>(e.Cache, null, typeExt?.UsrShowVendorLocationID ?? false);
            PXUIFieldAttribute.SetVisible<POLineExt.usrVendorAddress>(e.Cache, null, typeExt?.UsrShowVendorAddress ?? false);
            PXUIFieldAttribute.SetVisibility<POLineExt.usrVendorID>(e.Cache, null, ((typeExt?.UsrShowVendorID ?? false) ? PXUIVisibility.Visible : PXUIVisibility.Invisible));
            PXUIFieldAttribute.SetVisibility<POLineExt.usrVendorLocationID>(e.Cache, null, ((typeExt?.UsrShowVendorLocationID ?? false) ? PXUIVisibility.Visible : PXUIVisibility.Invisible));
            PXUIFieldAttribute.SetVisibility<POLineExt.usrVendorAddress>(e.Cache, null, ((typeExt?.UsrShowVendorAddress ?? false) ? PXUIVisibility.Visible : PXUIVisibility.Invisible));
            PXUIFieldAttribute.SetEnabled<POLineExt.usrSWKSPCCode>(e.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<POLineExt.usrSWKRTHCost>(e.Cache, null, false);
        }
        protected virtual void POLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            POLine line = (POLine)e.Row;
            if (line == null) return;

            POLineExt lineExt = line.GetExtension<POLineExt>();
            DropShipLink link = GetDropShipLink(line);

            if (e.Row != null && link != null)
            {
                SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                    And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                              .Select(Base, link.SOOrderType, link.SOOrderNbr, link.SOLineNbr);
                if (soLine != null)
                {
                    // FIX: get extension from the SO line, not the PO line
                    SOLineExt soLineExt = soLine.GetExtension<SOLineExt>();
                    lineExt.UsrVendorSpecTerms = soLineExt?.UsrVendorSpecTerms;
                    sender.SetValueExt<POLineExt.usrVendorSpecTerms>(line, soLineExt?.UsrVendorSpecTerms);


                    lineExt.UsrVendorNotes = soLineExt?.UsrVendorNotes;
                    sender.SetValueExt<POLineExt.usrVendorNotes>(line, soLineExt?.UsrVendorNotes);
                }
            }

            // Ensure ext. cost is consistent on insert as well
            //EnsureExtCostAndUnitCostFailsafe(sender, line, raiseErrors: false);

            RecalculateRTHHeaderTotals();
        }
        protected void _(Events.RowUpdated<POLine> e)
        {
            if (e.Row != null)
            {
                //EnsureExtCostAndUnitCostFailsafe(e.Cache, (POLine)e.Row, raiseErrors: false);
                RecalculateRTHHeaderTotals();
            }
        }

        protected void _(Events.RowDeleted<POLine> e)
        {
            if (e.Row != null)
                RecalculateRTHHeaderTotals();
        }

        protected virtual void _(Events.RowPersisting<POLine> e)
        {
            if (e.Row == null) return;
            // Enforce rule during save
            //EnsureExtCostAndUnitCostFailsafe(e.Cache, (POLine)e.Row, raiseErrors: true);
        }
        #endregion

        #region POOrder
        protected virtual void _(Events.FieldSelecting<POOrder, POOrderExt.usrFreightCost> e)
        {
            if (e.Row == null) return;
            POOrder order = e.Row;

            // Only calculate for drop-ship orders linked to SO
            if (order.SOOrderType == null || order.SOOrderNbr == null)
            {
                e.ReturnValue = 0m;
                return;
            }

            // Get all SO orders linked to this PO (handles merged SOs)
            var linkedSOOrders = PXSelectJoin<SOOrder,
                InnerJoin<DropShipLink,
                    On<DropShipLink.sOOrderType, Equal<SOOrder.orderType>,
                    And<DropShipLink.sOOrderNbr, Equal<SOOrder.orderNbr>>>>,
                Where<DropShipLink.pOOrderType, Equal<Required<POOrder.orderType>>,
                    And<DropShipLink.pOOrderNbr, Equal<Required<POOrder.orderNbr>>>>>
                .Select(Base, order.OrderType, order.OrderNbr)
                .RowCast<SOOrder>()
                .ToList();

            if (!linkedSOOrders.Any())
            {
                e.ReturnValue = 0m;
                return;
            }

            decimal totalFreightCost = 0m;

            // For each linked SO, find invoices created from shipments
            foreach (SOOrder soOrder in linkedSOOrders)
            {
                // Get all invoices created from this SO's shipments
                var invoices = PXSelectJoin<ARInvoice,
                    InnerJoin<SOOrderShipment,
                        On<SOOrderShipment.invoiceType, Equal<ARInvoice.docType>,
                        And<SOOrderShipment.invoiceNbr, Equal<ARInvoice.refNbr>>>>,
                    Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
                        And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
                    .Select(Base, soOrder.OrderType, soOrder.OrderNbr)
                    .RowCast<ARInvoice>()
                    .ToList();

                foreach (ARInvoice invoice in invoices)
                {
                    totalFreightCost += (invoice.CuryFreightCost ?? 0m);
                }
            }

            e.ReturnValue = totalFreightCost;
        }

        protected virtual void _(Events.FieldSelecting<POOrder, POOrderExt.usrFreightPrice> e)
        {
            if (e.Row == null) return;
            POOrder order = e.Row;

            // Only calculate for drop-ship orders linked to SO
            if (order.SOOrderType == null || order.SOOrderNbr == null)
            {
                e.ReturnValue = 0m;
                return;
            }

            // Get all SO orders linked to this PO (handles merged SOs)
            var linkedSOOrders = PXSelectJoin<SOOrder,
                InnerJoin<DropShipLink,
                    On<DropShipLink.sOOrderType, Equal<SOOrder.orderType>,
                    And<DropShipLink.sOOrderNbr, Equal<SOOrder.orderNbr>>>>,
                Where<DropShipLink.pOOrderType, Equal<Required<POOrder.orderType>>,
                    And<DropShipLink.pOOrderNbr, Equal<Required<POOrder.orderNbr>>>>>
                .Select(Base, order.OrderType, order.OrderNbr)
                .RowCast<SOOrder>()
                .ToList();

            if (!linkedSOOrders.Any())
            {
                e.ReturnValue = 0m;
                return;
            }

            decimal totalFreightPrice = 0m;

            // For each linked SO, find invoices created from shipments
            foreach (SOOrder soOrder in linkedSOOrders)
            {
                // Get all invoices created from this SO's shipments
                var invoices = PXSelectJoin<ARInvoice,
                    InnerJoin<SOOrderShipment,
                        On<SOOrderShipment.invoiceType, Equal<ARInvoice.docType>,
                        And<SOOrderShipment.invoiceNbr, Equal<ARInvoice.refNbr>>>>,
                    Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
                        And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
                    .Select(Base, soOrder.OrderType, soOrder.OrderNbr)
                    .RowCast<ARInvoice>()
                    .ToList();

                foreach (ARInvoice invoice in invoices)
                {
                    totalFreightPrice += (invoice.CuryFreightAmt ?? 0m);
                }
            }

            e.ReturnValue = totalFreightPrice;
        }

        protected virtual void _(Events.RowSelected<POOrder> e, PXRowSelected baseMethod)
        {
            POOrder order = (POOrder)e.Row;
            if (order == null) return;

            // Ensure SO keys exist before selecting
            if (order.SOOrderType == null || order.SOOrderNbr == null) { baseMethod?.Invoke(e.Cache, e.Args); return; }

            SOOrder soOrder = PXSelect<SOOrder,
                        Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                            And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(Base, order.SOOrderType, order.SOOrderNbr);

            sosetup.Current = sosetup.Select();
            SOSetupExt sOSetupExt = sosetup.Current?.GetExtension<SOSetupExt>();
            if (soOrder == null || sOSetupExt == null) { baseMethod?.Invoke(e.Cache, e.Args); return; }

            POOrderExt poExt = order.GetExtension<POOrderExt>();
            PXUIFieldAttribute.SetVisible<POOrderExt.usrFreightCost>(Base.Document.Cache, Base.CurrentDocument.Current,
                sOSetupExt.UsrPrepayAndAdd == soOrder.ShipTermsID ||
                       sOSetupExt.UsrFreeFreightAllowed == soOrder.ShipTermsID);
            PXUIFieldAttribute.SetVisible<POOrderExt.usrFreightPrice>(Base.Document.Cache, Base.CurrentDocument.Current,
                sOSetupExt.UsrPrepayAndAdd == soOrder.ShipTermsID);
            baseMethod?.Invoke(e.Cache, e.Args);

        }
        #endregion

        #endregion

        #region From Original
        public void UpdateSOLine(SOLineSplit3 split, int? vendorID, bool poCreated)
        {
            bool setVendor = split.VendorID != vendorID;
            bool setPOCreated = split.POCreated != poCreated;
            if (setVendor || setPOCreated)
            {
                SOLine5 origsoline = (SOLine5)Base.FixedDemandOrigSOLine.Select(split.OrderType, split.OrderNbr, split.LineNbr);
                bool changed = false;
                if (setVendor)
                {
                    split.VendorID = vendorID;
                    if (origsoline != null && origsoline.VendorID != vendorID)
                    {
                        origsoline.VendorID = vendorID;
                        changed = true;
                    }
                }
                if (setPOCreated)
                {
                    split.POCreated = poCreated;
                    if (origsoline != null && origsoline.POCreated != poCreated)
                    {
                        origsoline.POCreated = poCreated;
                        changed = true;
                    }
                }
                if (changed)
                    Base.FixedDemandOrigSOLine.Cache.MarkUpdated(origsoline, assertError: true);
            }
        }
        #endregion
    }
}