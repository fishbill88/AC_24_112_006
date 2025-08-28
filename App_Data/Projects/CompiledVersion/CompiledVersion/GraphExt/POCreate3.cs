using CompiledVersion.DAC;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.PO;
using System.Linq;
using static PX.Objects.PO.POCreate;
using static PX.Objects.SO.SOCreate;

namespace PX.Objects.PO3
{
    public class POCreate_Extension : PXGraphExtension<PX.Objects.PO.POCreate>
    {
        public static bool IsActive() => true;
        private void UpdateUsrPrice()
        {
            var filter = Base.Filter.Current;
            if (filter == null) return;
            var filterExt = PXCache<POCreateFilter>.GetExtension<POCreateFilterExt>(filter);
            decimal? sumPrice = Base.FixedDemand.Cache.Updated.RowCast<POFixedDemand>()
                                .Where(r => r.Selected == true).Sum(r => r.EffPrice);

            //foreach (POFixedDemand demand in Base.FixedDemand.Cache.Updated.RowCast<POFixedDemand>().Where(r=>r.Selected == true).Sum(r=>r.EffPrice))
            //{
            //    if (demand.Selected == true)
            //    {
            //        sumPrice += (demand.EffPrice ?? 0m);
            //    }
            //}
            //foreach (POFixedDemand row in Base.FixedDemand.Select())
            //{
            //    if (row.Selected == true && row.EffPrice != null && row.OrderQty != null)
            //    {
            //        sumPrice += (row.EffPrice ?? 0m);
            //    }
            //}
            filterExt.UsrPrice = sumPrice;
            Base.Filter.Cache.SetValueExt<POCreateFilterExt.usrPrice>(filter, sumPrice);
        }

        protected void POFixedDemand_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            var row = e.Row as POFixedDemand;
            if (row == null) return;
            if (sender.GetStatus(row) != PXEntryStatus.Deleted)
            {
                UpdateUsrPrice();
            }
        }

        protected void POFixedDemand_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            var row = e.Row as POFixedDemand;
            if (row == null) return;
            UpdateUsrPrice();
        }

        //protected virtual void _(Events.RowSelecting<POFixedDemand> e)
        //{
        //    var row = e.Row as POFixedDemand;
        //    if (row == null) return;
        //    if (row.VendorID != null && row.InventoryID != null)
        //    {
        //        row.AlternateID = null;
        //        INItemXRef itemXRef = PXSelect<INItemXRef,
        //            Where<INItemXRef.inventoryID, Equal<Required<INItemXRef.inventoryID>>,
        //                And<INItemXRef.alternateType, Equal<INAlternateType.global>>>>.
        //            Select(Base, row.InventoryID);
        //        if (itemXRef != null)
        //        {
        //            row.AlternateID = itemXRef.AlternateID;
        //        }
        //    }
        //}

    }
}