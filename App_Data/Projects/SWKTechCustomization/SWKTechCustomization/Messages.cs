using PX.Data;
using PX.Common;
using PX.Objects.SO;
using PX.Objects.PO;
using PX.Objects.CS;

namespace SWKTechCustomization
{
    [PXLocalizable]
    public static class Messages
    {
        public const string SPCCodeRequired = "SPC Code is required when SPC Cost is greater than zero.";
        public const string SPCCodeOnlyWithCost = "SPC Code can only be entered when SPC Cost is greater than zero.";
        public const string PurchaseOrderCreated = "Purchase Order Created";
        public const string MissingVendorOrLocation = "Missing Vendor or Location.";
        // Add other message constants as needed
    }
}
