using System;
using PX.Data;
using static PX.Objects.PO.POCreate;

namespace CompiledVersion.DAC
{
    public sealed class POCreateFilterExt : PXCacheExtension<POCreateFilter>
    {
        public static bool IsActive() => true;
        #region UsrPrice
        [PXDecimal]
        [PXUIField(DisplayName = "Price", Visibility = PXUIVisibility.SelectorVisible)]
        public decimal? UsrPrice { get; set; }
        public abstract class usrPrice : PX.Data.BQL.BqlDecimal.Field<usrPrice> { }
        #endregion
    }
}