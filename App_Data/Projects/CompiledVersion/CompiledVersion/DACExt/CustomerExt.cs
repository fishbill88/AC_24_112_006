using PX.Data;
using PX.Objects.AR;

namespace CompiledVersion.DAC
{
    public sealed class CustomerExt : PXCacheExtension<Customer>
    {
        public static bool IsActive() => true;

        #region UsrShippingInstructions
        [PXDBString(500, IsUnicode = true)]
        [PXUIField(DisplayName = "Shipping Instructions")]
        public string UsrShippingInstructions { get; set; }
        public abstract class usrShippingInstructions : PX.Data.BQL.BqlString.Field<usrShippingInstructions> { }
        #endregion
    }
}