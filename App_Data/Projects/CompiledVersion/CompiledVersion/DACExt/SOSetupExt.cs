using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;

namespace CompiledVersion.DAC
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

        #region Purchase Order Settings

        #region UsrCopyHeaderNotesToPO
        [PXDBBool]
        [PXUIField(DisplayName = "Copy Header Notes to PO")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrCopyHeaderNotesToPO { get; set; }
        public abstract class usrCopyHeaderNotesToPO : PX.Data.BQL.BqlBool.Field<usrCopyHeaderNotesToPO> { }
        #endregion

        #region UsrCopyLineNotesToPO
        [PXDBBool]
        [PXUIField(DisplayName = "Copy Line Notes to PO")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrCopyLineNotesToPO { get; set; }
        public abstract class usrCopyLineNotesToPO : PX.Data.BQL.BqlBool.Field<usrCopyLineNotesToPO> { }
        #endregion

        #region UsrCopyHeaderAttachmentsToPO
        [PXDBBool]
        [PXUIField(DisplayName = "Copy Header Attachments to PO")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrCopyHeaderAttachmentsToPO { get; set; }
        public abstract class usrCopyHeaderAttachmentsToPO : PX.Data.BQL.BqlBool.Field<usrCopyHeaderAttachmentsToPO> { }
        #endregion

        #region UsrCopyLineAttachmentsToPO
        [PXDBBool]
        [PXUIField(DisplayName = "Copy Line Attachments to PO")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrCopyLineAttachmentsToPO { get; set; }
        public abstract class usrCopyLineAttachmentsToPO : PX.Data.BQL.BqlBool.Field<usrCopyLineAttachmentsToPO> { }
        #endregion

        #endregion

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