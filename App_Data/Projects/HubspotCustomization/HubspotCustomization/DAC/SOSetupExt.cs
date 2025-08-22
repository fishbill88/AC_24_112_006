using PX.Data;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubspotCustomization
{
    [PXCacheName(Messages.SOSetupExtension)]
    public sealed class SOSetupExt : PXCacheExtension<PX.Objects.SO.SOSetup>
    {
        public static bool IsActive() => true;

        #region UsrNonstock1
        [NonStockItem(DisplayName = Messages.NonStock1)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public int? UsrNonstock1 { get; set; }
        public abstract class usrNonstock1 : PX.Data.BQL.BqlInt.Field<usrNonstock1> { }
        #endregion

        #region UsrNonstock2
        [NonStockItem(DisplayName = Messages.NonStock2)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public int? UsrNonstock2 { get; set; }
        public abstract class usrNonstock2 : PX.Data.BQL.BqlInt.Field<usrNonstock2> { }
        #endregion

        #region UsrNonstock3
        [NonStockItem(DisplayName = Messages.NonStock3)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public int? UsrNonstock3 { get; set; }
        public abstract class usrNonstock3 : PX.Data.BQL.BqlInt.Field<usrNonstock3> { }
        #endregion

        #region UsrHidePrintingMethod
        [PXDBBool]
        [PXUIField(DisplayName = Messages.HidePrintingMethod)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrHidePrintingMethod { get; set; }
        public abstract class usrHidePrintingMethod : PX.Data.BQL.BqlBool.Field<usrHidePrintingMethod> { }
        #endregion

        #region UsrHidePrintingMethod2
        [PXDBBool]
        [PXUIField(DisplayName = Messages.HidePrintingMethod2)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrHidePrintingMethod2 { get; set; }
        public abstract class usrHidePrintingMethod2 : PX.Data.BQL.BqlBool.Field<usrHidePrintingMethod2> { }
        #endregion
    }
}
