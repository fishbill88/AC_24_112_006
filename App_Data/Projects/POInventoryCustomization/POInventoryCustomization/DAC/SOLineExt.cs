using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;

namespace POInventoryCustomization
{
    public class SOLineExt : PXCacheExtension<PX.Objects.SO.SOLine>
    {
        #region UsrItemSpecs
        [PXDBString(2000, IsUnicode = true)]
        [PXUIField(DisplayName = "Specification", Enabled = false)]
        [PXSelector(typeof(Search<InventoryItemExt.usrItemSpecs,
                            Where<InventoryItem.inventoryID,
                                Equal<Current<SOLine.inventoryID>>>>))]
        [PXDefault(typeof(Search<InventoryItemExt.usrItemSpecs,
                            Where<InventoryItem.inventoryID,
                                Equal<Current<SOLine.inventoryID>>>>),PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<SOLine.inventoryID>))]
        public virtual string UsrItemSpecs { get; set; }
        public abstract class usrItemSpecs : PX.Data.BQL.BqlString.Field<usrItemSpecs> { }
        #endregion

        #region UsrVendorID
        [VendorActive(DisplayName = "Vendor", DescriptionField = typeof(Vendor.acctName))]
        public virtual int? UsrVendorID { get; set; }
        public abstract class usrVendorID : PX.Data.BQL.BqlInt.Field<usrVendorID> { }
        #endregion

        #region UsrVendorLocationID
        [LocationActive(typeof(Where<PX.Objects.CR.Location.bAccountID, Equal<Optional<usrVendorID>>, And<MatchWithBranch<PX.Objects.CR.Location.vBranchID>>>), DescriptionField = typeof(PX.Objects.CR.Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault(typeof(Coalesce<Search2<Vendor.defLocationID, InnerJoin<PX.Objects.CR.Standalone.Location, On<PX.Objects.CR.Standalone.Location.locationID, Equal<Vendor.defLocationID>, And<PX.Objects.CR.Standalone.Location.bAccountID, Equal<Vendor.bAccountID>>>>, Where<Vendor.bAccountID, Equal<Current<usrVendorID>>, And<PX.Objects.CR.Standalone.Location.isActive, Equal<True>, And<MatchWithBranch<PX.Objects.CR.Standalone.Location.vBranchID>>>>>, Search<PX.Objects.CR.Standalone.Location.locationID, Where<PX.Objects.CR.Standalone.Location.bAccountID, Equal<Current<usrVendorID>>, And<PX.Objects.CR.Standalone.Location.isActive, Equal<True>, And<MatchWithBranch<PX.Objects.CR.Standalone.Location.vBranchID>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXForeignReference(typeof(CompositeKey<Field<usrVendorID>.IsRelatedTo<PX.Objects.CR.Location.bAccountID>, Field<usrVendorLocationID>.IsRelatedTo<PX.Objects.CR.Location.locationID>>))]
        public virtual int? UsrVendorLocationID { get; set; }
        public abstract class usrVendorLocationID : PX.Data.BQL.BqlInt.Field<usrVendorLocationID> { }
        #endregion

        #region UsrVendorAddress
        [PXDBString(2000, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Address", Enabled = false)]
        public virtual string UsrVendorAddress { get; set; }
        public abstract class usrVendorAddress : PX.Data.BQL.BqlString.Field<usrVendorAddress> { }

        #endregion
    }
}