using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.IN;
using System;

namespace ItemRequestCustomization
{
    public class INSetupExt : PXCacheExtension<PX.Objects.IN.INSetup>
    {
        #region UsrDefaultWarehouse
        [PX.Objects.IN.Site(DisplayName = "Item Request Dflt Warehouse", DescriptionField = typeof(INSite.descr))]
        //[PX.Objects.IN.Site(DisplayName = "Default Warehouse", DescriptionField = typeof(INSite.descr))]
        public virtual int? UsrDefaultWarehouse { get; set; }
        public abstract class usrDefaultWarehouse : BqlInt.Field<usrDefaultWarehouse> { }
        #endregion
        #region UsrProductBrandAttributeID
        public abstract class usrProductBrandAttributeID : PX.Data.BQL.BqlString.Field<usrProductBrandAttributeID> { }
        protected String _UsrProductBrandAttributeID;
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXUIField(DisplayName = "Product Brand Attribute ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<CSAttribute.attributeID,
            Where<CSAttribute.controlType, Equal<int1>>>))]
        public virtual String UsrProductBrandAttributeID
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