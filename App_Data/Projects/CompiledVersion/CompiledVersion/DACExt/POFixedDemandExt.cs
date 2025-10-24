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
        public int? UsrVendorLocationID { get; set; }
        public abstract class usrVendorLocationID : PX.Data.BQL.BqlInt.Field<usrVendorLocationID> { }
        #endregion

        #region UsrVendorAddress
        [PXString(2000, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Address", Enabled = false)]
        public string UsrVendorAddress { get; set; }
        public abstract class usrVendorAddress : PX.Data.BQL.BqlString.Field<usrVendorAddress> { }

        #endregion

        #region UsrSWKSPCCode
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "SPC Code")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public string UsrSWKSPCCode { get; set; }
        public abstract class usrSWKSPCCode : PX.Data.BQL.BqlString.Field<usrSWKSPCCode> { }
        #endregion

        #region UsrSOSPCCost
        [PXDecimal(2)]
        [PXDBScalar(typeof(Search<SOLineExt.usrSWKSPCCost,
            Where<SOLine.orderType, Equal<POFixedDemand.orderType>,
            And<SOLine.orderNbr, Equal<POFixedDemand.orderNbr>,
            And<SOLine.lineNbr, Equal<POFixedDemand.lineNbr>>>>>))]
        [PXFormula(typeof(Default<POFixedDemand.orderType, POFixedDemand.orderNbr, POFixedDemand.lineNbr>))]
        [PXUIField(DisplayName = "SO SPC Cost", Enabled = false)]
        public decimal? UsrSOSPCCost { get; set; }
        public abstract class usrSOSPCCost : PX.Data.BQL.BqlDecimal.Field<usrSOSPCCost> { }
        #endregion

        #region UsrSOSPCCode
        [PXString(30, IsUnicode = true)]
        [PXDBScalar(typeof(Search<SOLineExt.usrSWKSPCCode,
            Where<SOLine.orderType, Equal<POFixedDemand.orderType>,
            And<SOLine.orderNbr, Equal<POFixedDemand.orderNbr>,
            And<SOLine.lineNbr, Equal<POFixedDemand.lineNbr>>>>>))]
        [PXFormula(typeof(Default<POFixedDemand.orderType, POFixedDemand.orderNbr, POFixedDemand.lineNbr>))]
        [PXUIField(DisplayName = "SO SPC Code", Enabled = false)]
        public string UsrSOSPCCode { get; set; }
        public abstract class usrSOSPCCode : PX.Data.BQL.BqlString.Field<usrSOSPCCode> { }
        #endregion

        #region UsrSOVendorNotes
        [PXString(500, IsUnicode = true)]
        [PXDBScalar(typeof(Search<SOLineExt.usrVendorNotes,
            Where<SOLine.orderType, Equal<POFixedDemand.orderType>,
            And<SOLine.orderNbr, Equal<POFixedDemand.orderNbr>,
            And<SOLine.lineNbr, Equal<POFixedDemand.lineNbr>>>>>))]
        [PXFormula(typeof(Default<POFixedDemand.orderType, POFixedDemand.orderNbr, POFixedDemand.lineNbr>))]
        [PXUIField(DisplayName = "SO Vendor Notes", Enabled = false)]
        public string UsrSOVendorNotes { get; set; }
        public abstract class usrSOVendorNotes : PX.Data.BQL.BqlString.Field<usrSOVendorNotes> { }
        #endregion

        #region UsrSOVendorSpecialTerms
        [PXString(250, IsUnicode = true)]
        [PXDBScalar(typeof(Search<SOLineExt.usrVendorSpecTerms,
            Where<SOLine.orderType, Equal<POFixedDemand.orderType>,
            And<SOLine.orderNbr, Equal<POFixedDemand.orderNbr>,
            And<SOLine.lineNbr, Equal<POFixedDemand.lineNbr>>>>>))]
        [PXFormula(typeof(Default<POFixedDemand.orderType, POFixedDemand.orderNbr, POFixedDemand.lineNbr>))]
        [PXUIField(DisplayName = "SO Vendor Special Terms", Enabled = false)]
        public string UsrSOVendorSpecialTerms { get; set; }
        public abstract class usrSOVendorSpecialTerms : PX.Data.BQL.BqlString.Field<usrSOVendorSpecialTerms> { }
        #endregion
    }

    public sealed class INItemPlanExt : PXCacheExtension<INItemPlan>
    {
        public static bool IsActive() => true;

        #region UsrVendorID
        [PXDBInt]
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
        public int? UsrVendorLocationID { get; set; }
        public abstract class usrVendorLocationID : PX.Data.BQL.BqlInt.Field<usrVendorLocationID> { }
        #endregion

        #region UsrSWKSPCCode
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "SPC Code")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public string UsrSWKSPCCode { get; set; }
        public abstract class usrSWKSPCCode : PX.Data.BQL.BqlString.Field<usrSWKSPCCode> { }
        #endregion

        #region UsrVendorAddress
        [PXString(2000, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Address", Enabled = false)]
        public string UsrVendorAddress { get; set; }
        public abstract class usrVendorAddress : PX.Data.BQL.BqlString.Field<usrVendorAddress> { }
        #endregion
    }
}
