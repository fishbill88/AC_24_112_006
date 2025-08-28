using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using System;
using static CompiledVersion.DAC.POFixedDemandExt;
using static PX.Objects.PO.POFixedDemand;

namespace CompiledVersion.DAC
{
    public sealed class POFixedDemandExt : PXCacheExtension<POFixedDemand>
    {
        public static bool IsActive() => true;

        #region UsrSWKRTHCost
        [PXDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Cost", Enabled = false)]
        public decimal? UsrSWKRTHCost { get; set; }
        public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
        #endregion

        #region UsrSOLineUsrVendorID
        [PXInt]
        [PXDBScalar(typeof(Search<SOLineExt.usrVendorID, Where<SOLine.orderType, Equal<POFixedDemand.orderType>, And<SOLine.orderNbr, Equal<POFixedDemand.orderNbr>, And<SOLine.lineNbr, Equal<POFixedDemand.lineNbr>>>>>))]
        [PXFormula(typeof(Default<POFixedDemand.orderNbr, POFixedDemand.orderType, POFixedDemand.lineNbr>))]
        [PXUIField(DisplayName = "Sales Order Line Alternate Vendor", Enabled = false)]
        public int? UsrSOLineUsrVendorID { get; set; }
        public abstract class usrSOLineUsrVendorID : PX.Data.BQL.BqlInt.Field<usrSOLineUsrVendorID> { }
        #endregion

        #region UsrOrderTypeUsrShowVendorID
        [PXBool]
        [PXDBScalar(typeof(Search<SOOrderTypeExt.usrShowVendorID, Where<SOOrderType.orderType, Equal<POFixedDemand.orderType>>>))]
        [PXFormula(typeof(Default<POFixedDemand.orderNbr, POFixedDemand.orderType, POFixedDemand.lineNbr>))]
        [PXUIField(DisplayName = "Show Alternate Vendor", Enabled = false)]
        public bool? UsrOrderTypeUsrShowVendorID { get; set; }
        public abstract class usrOrderTypeUsrShowVendorID : PX.Data.BQL.BqlBool.Field<usrOrderTypeUsrShowVendorID> { }
        #endregion

        #region UsrVendorID
        [PXDBInt()]
        //[PXSelector(
        //    typeof(Search2<
        //        BAccountR.bAccountID,
        //        LeftJoin<BranchAlias,
        //            On<BAccountR.isBranch, Equal<True>,
        //            And<BranchAlias.bAccountID, Equal<BAccountR.bAccountID>>>>,
        //        Where<BAccountR.type, NotEqual<BAccountType.employeeType>>>),
        //    typeof(BAccountR.acctCD),
        //    typeof(BAccountR.acctName),
        //    typeof(BAccountR.classID),
        //    typeof(BAccountR.vStatus),
        //    SubstituteKey = typeof(BAccountR.acctCD)
        //    )]
        [PXSelector(
            typeof(Search2<
                BAccountR.bAccountID,
                LeftJoin<BranchAlias,
                    On<BAccountR.isBranch, Equal<True>,
                    And<BranchAlias.bAccountID, Equal<BAccountR.bAccountID>>>>,
                Where<BAccountR.type, NotEqual<BAccountType.employeeType>>>),
            typeof(BAccountR.acctCD),
            typeof(BAccountR.acctName),
            typeof(BAccountR.classID),
            typeof(BAccountR.vStatus),
            SubstituteKey = typeof(BAccountR.acctCD)
            )]

        [PXFormula(typeof(Switch<Case<Where<usrOrderTypeUsrShowVendorID, Equal<True>>, usrSOLineUsrVendorID>,
            vendorID>))]
        [PXFormula(typeof(Default<POFixedDemand.orderNbr, POFixedDemand.orderType, POFixedDemand.lineNbr>))]
        [PXUIField(DisplayName = "Alternate Vendor",Visible = false)]
        public int? UsrVendorID { get; set; }
        public abstract class usrVendorID : PX.Data.BQL.BqlInt.Field<usrVendorID> { }
        #endregion

        #region UsrVendorLocationID
        [LocationActive(typeof(Where<Location.bAccountID, Equal<Current<usrVendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.Invisible, Visible = false)]
        [PXFormula(typeof(Default<usrVendorID>))]
        //[LocationActive(typeof(Where<PX.Objects.CR.Location.bAccountID, Equal<Optional<usrVendorID>>, And<MatchWithBranch<PX.Objects.CR.Location.vBranchID>>>), DescriptionField = typeof(PX.Objects.CR.Location.descr), Visibility = PXUIVisibility.SelectorVisible,IsDBField = false)]
        //[PXDefault(typeof(Coalesce<Search2<Vendor.defLocationID, InnerJoin<PX.Objects.CR.Standalone.Location, On<PX.Objects.CR.Standalone.Location.locationID, Equal<Vendor.defLocationID>, And<PX.Objects.CR.Standalone.Location.bAccountID, Equal<Vendor.bAccountID>>>>, Where<Vendor.bAccountID, Equal<Current<usrVendorID>>, And<PX.Objects.CR.Standalone.Location.isActive, Equal<True>, And<MatchWithBranch<PX.Objects.CR.Standalone.Location.vBranchID>>>>>, Search<PX.Objects.CR.Standalone.Location.locationID, Where<PX.Objects.CR.Standalone.Location.bAccountID, Equal<Current<usrVendorID>>, And<PX.Objects.CR.Standalone.Location.isActive, Equal<True>, And<MatchWithBranch<PX.Objects.CR.Standalone.Location.vBranchID>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXForeignReference(typeof(CompositeKey<Field<usrVendorID>.IsRelatedTo<PX.Objects.CR.Location.bAccountID>, Field<usrVendorLocationID>.IsRelatedTo<PX.Objects.CR.Location.locationID>>))]
        public int? UsrVendorLocationID { get; set; }
        public abstract class usrVendorLocationID : PX.Data.BQL.BqlInt.Field<usrVendorLocationID> { }
        #endregion

        #region UsrVendorAddress
        [PXString(2000, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Address", Enabled = false)]
        public string UsrVendorAddress { get; set; }
        public abstract class usrVendorAddress : PX.Data.BQL.BqlString.Field<usrVendorAddress> { }

        #endregion
    }

    public sealed class INItemPlanExt : PXCacheExtension<INItemPlan>
    {
        public static bool IsActive() => true;

        #region UsrVendorID
        [PXDBInt] // Declare unbound integer type so VendorAttribute can work correctly
        [PXSelector(
            typeof(Search2<
                BAccountR.bAccountID,
                LeftJoin<BranchAlias,
                    On<BAccountR.isBranch, Equal<True>,
                    And<BranchAlias.bAccountID, Equal<BAccountR.bAccountID>>>>,
                Where<BAccountR.type, NotEqual<BAccountType.employeeType>>>),
            typeof(BAccountR.acctCD),
            typeof(BAccountR.acctName),
            typeof(BAccountR.classID),
            typeof(BAccountR.vStatus),
            SubstituteKey = typeof(BAccountR.acctCD)
            )]
        [PXUIField(DisplayName = "Alternate Vendor")]
        public int? UsrVendorID { get; set; }
        public abstract class usrVendorID : PX.Data.BQL.BqlInt.Field<usrVendorID> { }
        #endregion

        #region UsrVendorLocationID
        [LocationActive(typeof(Where<Location.bAccountID, Equal<Current<usrVendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
        [PXFormula(typeof(Default<usrVendorID>))]
        //[LocationActive(typeof(Where<PX.Objects.CR.Location.bAccountID, Equal<Optional<usrVendorID>>, And<MatchWithBranch<PX.Objects.CR.Location.vBranchID>>>), DescriptionField = typeof(PX.Objects.CR.Location.descr), Visibility = PXUIVisibility.SelectorVisible,IsDBField = false)]
        //[PXDefault(typeof(Coalesce<Search2<Vendor.defLocationID, InnerJoin<PX.Objects.CR.Standalone.Location, On<PX.Objects.CR.Standalone.Location.locationID, Equal<Vendor.defLocationID>, And<PX.Objects.CR.Standalone.Location.bAccountID, Equal<Vendor.bAccountID>>>>, Where<Vendor.bAccountID, Equal<Current<usrVendorID>>, And<PX.Objects.CR.Standalone.Location.isActive, Equal<True>, And<MatchWithBranch<PX.Objects.CR.Standalone.Location.vBranchID>>>>>, Search<PX.Objects.CR.Standalone.Location.locationID, Where<PX.Objects.CR.Standalone.Location.bAccountID, Equal<Current<usrVendorID>>, And<PX.Objects.CR.Standalone.Location.isActive, Equal<True>, And<MatchWithBranch<PX.Objects.CR.Standalone.Location.vBranchID>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXForeignReference(typeof(CompositeKey<Field<usrVendorID>.IsRelatedTo<PX.Objects.CR.Location.bAccountID>, Field<usrVendorLocationID>.IsRelatedTo<PX.Objects.CR.Location.locationID>>))]
        public int? UsrVendorLocationID { get; set; }
        public abstract class usrVendorLocationID : PX.Data.BQL.BqlInt.Field<usrVendorLocationID> { }
        #endregion


        #region UsrVendorAddress
        [PXString(2000, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Address", Enabled = false)]
        public string UsrVendorAddress { get; set; }
        public abstract class usrVendorAddress : PX.Data.BQL.BqlString.Field<usrVendorAddress> { }

        #endregion
    }
}
