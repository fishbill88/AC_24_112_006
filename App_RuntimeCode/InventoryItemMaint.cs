using PX.Data;
using PX.Objects.IN;
using ItemRequestCustomization;

namespace SWKTechCustomization
{
    // Extension for Stock Items (IN202500)
    public class InventoryItemMaintExt : PXGraphExtension<InventoryItemMaint>
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