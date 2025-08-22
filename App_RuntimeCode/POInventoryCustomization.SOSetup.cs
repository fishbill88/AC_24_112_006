using PX.Data;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.SO;
using PX.Objects;
using System.Collections.Generic;
using System;

namespace FreightCustomization
{
    public sealed class SOSetupExt : PXCacheExtension<PX.Objects.SO.SOSetup>
    {

        public static bool IsActive() => true;
        #region UsrNotToExceed
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXUIField(DisplayName = "Not to Exceed")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(ShipTerms.shipTermsID), DescriptionField = typeof(ShipTerms.description), CacheGlobal = true)]
        public string UsrNotToExceed { get; set; }
        public abstract class usrNotToExceed : PX.Data.BQL.BqlString.Field<usrNotToExceed> { }
        #endregion


        #region UsrPrepayAndAdd
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXUIField(DisplayName = "Prepay and Add")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(ShipTerms.shipTermsID), DescriptionField = typeof(ShipTerms.description), CacheGlobal = true)]
        public string UsrPrepayAndAdd { get; set; }
        public abstract class usrPrepayAndAdd : PX.Data.BQL.BqlString.Field<usrPrepayAndAdd> { }
        #endregion

        #region UsrFreeFreightAllowed
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXUIField(DisplayName = "Free Freight Allowed")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(ShipTerms.shipTermsID), DescriptionField = typeof(ShipTerms.description), CacheGlobal = true)]
        public string UsrFreeFreightAllowed { get; set; }
        public abstract class usrFreeFreightAllowed : PX.Data.BQL.BqlString.Field<usrFreeFreightAllowed> { }
        #endregion
    }
}