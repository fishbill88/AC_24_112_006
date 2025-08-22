using PX.Data;
using PX.Objects.IN;
using ItemRequestCustomization;

namespace SWKTechCustomization
{
    // Extension for Non-Stock Items (IN202000)
    public class NonStockItemMaintExt : PXGraphExtension<NonStockItemMaint>
    {
        public static bool IsActive() => true;

        protected virtual void _(Events.RowSelected<InventoryItem> e)
        {
            if (e.Row == null) return;

            var itemExt = e.Row.GetExtension<InventoryItemExt>();
            // Make RTH Cost field editable
            PXUIFieldAttribute.SetEnabled<InventoryItemExt.usrSWKRTHCost>(e.Cache, e.Row, true);
        }
    }
}