using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using static PX.Objects.PO.POFixedDemand;

namespace CompiledVersion.DAC
{
    public sealed class SOLineExt : PXCacheExtension<PX.Objects.SO.SOLine>
    {
        public static bool IsActive() => true;

        #region UsrATAIRTHLineNbr
        public abstract class usrATAIRTHLineNbr : PX.Data.BQL.BqlInt.Field<usrATAIRTHLineNbr> { }
        [PXDBInt]
        [PXUIField(DisplayName = "RTH Line Nbr", Enabled = false)]
        public int? UsrATAIRTHLineNbr { get; set; }
        #endregion

        #region UsrATAISkipPrint
        public abstract class usrATAISkipPrint : PX.Data.BQL.BqlBool.Field<usrATAISkipPrint> { }
        [PXDBBool]
        [PXUIField(DisplayName = "Skip Print")]
        public bool? UsrATAISkipPrint { get; set; }
        #endregion

        #region UsrVendorSpecTerms
        [PXDBString(250, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Special Terms")]
        public string UsrVendorSpecTerms { get; set; }
        public abstract class usrVendorSpecTerms : PX.Data.BQL.BqlString.Field<usrVendorSpecTerms> { }
        #endregion

        #region UsrVendorNotes
        [PXDBString(500, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Notes")]
        public string UsrVendorNotes { get; set; }
        public abstract class usrVendorNotes : PX.Data.BQL.BqlString.Field<usrVendorNotes> { }
        #endregion

        #region UsrItemSpecs
        [PXDBString(2000, IsUnicode = true)]
        [PXUIField(DisplayName = "Specification", Enabled = false)]
        [PXSelector(typeof(Search<InventoryItemExt.usrItemSpecs,
                            Where<InventoryItem.inventoryID,
                                Equal<Current<SOLine.inventoryID>>>>))]
        [PXDefault(typeof(Search<InventoryItemExt.usrItemSpecs,
                            Where<InventoryItem.inventoryID,
                                Equal<Current<SOLine.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<SOLine.inventoryID>))]
        public string UsrItemSpecs { get; set; }
        public abstract class usrItemSpecs : PX.Data.BQL.BqlString.Field<usrItemSpecs> { }
        #endregion

        #region UsrVendorID
        [Vendor(typeof(Search2<BAccountR.bAccountID, LeftJoin<BranchAlias, On<BAccount.isBranch, Equal<True>, And<BranchAlias.bAccountID, Equal<BAccountR.bAccountID>>>>, Where<Vendor.type, NotEqual<BAccountType.employeeType>>>))]
        [PXRestrictor(typeof(Where<Vendor.vStatus, IsNull, Or<Vendor.vStatus, In3<VendorStatus.active, VendorStatus.oneTime, VendorStatus.holdPayments>>>), "The vendor status is '{0}'.", new Type[] { typeof(Vendor.vStatus) })]
        //[PXRestrictor(typeof(Where2<Where<Current<salesCustomerID>, IsNull, Or<Vendor.bAccountID, NotEqual<Current<salesCustomerID>>>>, And<Where<BranchAlias.branchID, IsNull, Or<Current<salesBranchID>, IsNull, Or<BranchAlias.branchID, NotEqual<Current<salesBranchID>>>>>>>), "The vendor cannot be specified because either it has been extended from the branch of the sales order or it coincides with the customer of this order.", new Type[] { })]
        //[VendorActive(DisplayName = "Vendor", DescriptionField = typeof(Vendor.acctName))]
        public int? UsrVendorID { get; set; }
        public abstract class usrVendorID : PX.Data.BQL.BqlInt.Field<usrVendorID> { }
        #endregion

        #region UsrVendorLocationID
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current<usrVendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
        [PXFormula(typeof(Default<usrVendorID>))]
        //[LocationActive(typeof(Where<PX.Objects.CR.Location.bAccountID, Equal<Optional<usrVendorID>>, And<MatchWithBranch<PX.Objects.CR.Location.vBranchID>>>), DescriptionField = typeof(PX.Objects.CR.Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
        //[PXDefault(typeof(Coalesce<Search2<Vendor.defLocationID, InnerJoin<PX.Objects.CR.Standalone.Location, On<PX.Objects.CR.Standalone.Location.locationID, Equal<Vendor.defLocationID>, And<PX.Objects.CR.Standalone.Location.bAccountID, Equal<Vendor.bAccountID>>>>, Where<Vendor.bAccountID, Equal<Current<usrVendorID>>, And<PX.Objects.CR.Standalone.Location.isActive, Equal<True>, And<MatchWithBranch<PX.Objects.CR.Standalone.Location.vBranchID>>>>>, Search<PX.Objects.CR.Standalone.Location.locationID, Where<PX.Objects.CR.Standalone.Location.bAccountID, Equal<Current<usrVendorID>>, And<PX.Objects.CR.Standalone.Location.isActive, Equal<True>, And<MatchWithBranch<PX.Objects.CR.Standalone.Location.vBranchID>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXForeignReference(typeof(CompositeKey<Field<usrVendorID>.IsRelatedTo<PX.Objects.CR.Location.bAccountID>, Field<usrVendorLocationID>.IsRelatedTo<PX.Objects.CR.Location.locationID>>))]
        public int? UsrVendorLocationID { get; set; }
        public abstract class usrVendorLocationID : PX.Data.BQL.BqlInt.Field<usrVendorLocationID> { }
        #endregion

        #region UsrVendorAddress
        [PXDBString(2000, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Address", Enabled = false)]
        public string UsrVendorAddress { get; set; }
        public abstract class usrVendorAddress : PX.Data.BQL.BqlString.Field<usrVendorAddress> { }

        #endregion

        #region UsrSWKRTHCost
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Cost", Enabled = false)]
        public decimal? UsrSWKRTHCost { get; set; }
        public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
        #endregion

        #region UsrSWKSPCCost
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "SPC Cost")]
        public decimal? UsrSWKSPCCost { get; set; }
        public abstract class usrSWKSPCCost : PX.Data.BQL.BqlDecimal.Field<usrSWKSPCCost> { }
        #endregion

        #region UsrSWKSPCCode
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "SPC Code")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXUIRequired(typeof(Where<SOLineExt.usrSWKSPCCost, Greater<decimal0>>))]
        public string UsrSWKSPCCode { get; set; }
        public abstract class usrSWKSPCCode : PX.Data.BQL.BqlString.Field<usrSWKSPCCode> { }
        #endregion

        #region UsrSWKManualCost
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Manual Cost", Enabled = false)]
        public bool? UsrSWKManualCost { get; set; }
        public abstract class usrSWKManualCost : PX.Data.BQL.BqlBool.Field<usrSWKManualCost> { }
        #endregion
    }
}