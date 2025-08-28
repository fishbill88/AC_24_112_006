using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;
using PX.Objects.IN;
using System;

namespace CompiledVersion.DAC
{
    public sealed class INSetupExt : PXCacheExtension<PX.Objects.IN.INSetup>
    {
        public static bool IsActive() => true;

        #region UsrDefaultWarehouse
        [PX.Objects.IN.Site(DisplayName = "Item Request Dflt Warehouse", DescriptionField = typeof(INSite.descr))]
        public int? UsrDefaultWarehouse { get; set; }
        public abstract class usrDefaultWarehouse : BqlInt.Field<usrDefaultWarehouse> { }
        #endregion

        #region UsrProductBrandAttributeID
        public abstract class usrProductBrandAttributeID : PX.Data.BQL.BqlString.Field<usrProductBrandAttributeID> { }
        protected String _UsrProductBrandAttributeID;
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXUIField(DisplayName = "Product Brand Attribute ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<CSAttribute.attributeID,
            Where<CSAttribute.controlType, Equal<int1>>>))]
        public String UsrProductBrandAttributeID
        {
            get
            {
                return this._UsrProductBrandAttributeID;
            }
            set
            {
                this._UsrProductBrandAttributeID = value;
            }
        }
        #endregion

    }
}