using PX.Data;
using PX.Objects.CS;
using PX.Objects.PO;
using System;

namespace FreightCustomization
{
    public sealed class POSetupExt : PXCacheExtension<PX.Objects.PO.POSetup>
    {
        #region Purchase Order Settings
        [PXDBBool]
        [PXUIField(DisplayName = "Copy Header Notes to PO")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrCopyHeaderNotesToPO { get; set; }
        public abstract class usrCopyHeaderNotesToPO : PX.Data.BQL.BqlBool.Field<usrCopyHeaderNotesToPO> { }

        [PXDBBool]
        [PXUIField(DisplayName = "Copy Line Notes to PO")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrCopyLineNotesToPO { get; set; }
        public abstract class usrCopyLineNotesToPO : PX.Data.BQL.BqlBool.Field<usrCopyLineNotesToPO> { }

        [PXDBBool]
        [PXUIField(DisplayName = "Copy Header Attachments to PO")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrCopyHeaderAttachmentsToPO { get; set; }
        public abstract class usrCopyHeaderAttachmentsToPO : PX.Data.BQL.BqlBool.Field<usrCopyHeaderAttachmentsToPO> { }

        [PXDBBool]
        [PXUIField(DisplayName = "Copy Line Attachments to PO")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrCopyLineAttachmentsToPO { get; set; }
        public abstract class usrCopyLineAttachmentsToPO : PX.Data.BQL.BqlBool.Field<usrCopyLineAttachmentsToPO> { }
        #endregion
    }
}
