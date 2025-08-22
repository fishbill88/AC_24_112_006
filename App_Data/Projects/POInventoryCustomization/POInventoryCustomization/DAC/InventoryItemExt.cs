using PX.Data;

namespace POInventoryCustomization
{
    public class InventoryItemExt : PXCacheExtension<PX.Objects.IN.InventoryItem>
    {
        public static bool IsActive()
        {
            // This method is used to determine if the extension is active.
            // You can add your custom logic here to enable or disable the extension.
            return true;
        }
        #region UsrItemSpecs
        [PXDBString(2000, IsUnicode = true)]
        [PXUIField(DisplayName = "Item Specs")]
        public virtual string UsrItemSpecs { get; set; }
        public abstract class usrItemSpecs : PX.Data.BQL.BqlString.Field<usrItemSpecs> { }
        #endregion
    }
}
