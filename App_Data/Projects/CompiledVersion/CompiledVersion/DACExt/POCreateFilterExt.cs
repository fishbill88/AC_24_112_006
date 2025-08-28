using System;
using PX.Data;
using static PX.Objects.PO.POCreate;

namespace CompiledVersion.DAC
{
    public sealed class POCreateFilterExt : PXCacheExtension<POCreateFilter>
    {
        public static bool IsActive() => true;
        #region UsrPrice
        [PXDecimal(6)]
        [PXUIField(DisplayName = "Price", Enabled = false)]
        public decimal? UsrPrice { get; set; }
        public abstract class usrPrice : PX.Data.BQL.BqlDecimal.Field<usrPrice> { }
        #endregion
    }
}