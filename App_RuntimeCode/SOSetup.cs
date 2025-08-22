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
    }
}