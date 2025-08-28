using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.SO;
using System;

namespace CompiledVersion.DAC
{
    public sealed class POLineExt : PXCacheExtension<PX.Objects.PO.POLine>
    {
        public static bool IsActive() => true;

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

        #region UsrShippingTerms
        [PXString(1, IsFixed = true)]
        [SOShipComplete.List()]
        [PXUIField(DisplayName = "Shipping Rule", Enabled = false)]
        public string UsrShippingTerms { get; set; }
        public abstract class usrShippingTerms : PX.Data.BQL.BqlString.Field<usrShippingTerms> { }
        #endregion

        #region UsrSkipPrint
        [PXDBBool]
        [PXUIField(DisplayName = "Skip Print")]
        public bool? UsrSkipPrint { get; set; }
        public abstract class usrSkipPrint : PX.Data.BQL.BqlBool.Field<usrSkipPrint> { }
        #endregion

        #region UsrPrepaymentLine
        [PXDBBool]
        [PXUIField(DisplayName = "Prepayment Line")]
        public bool? UsrPrepaymentLine { get; set; }
        public abstract class usrPrepaymentLine : PX.Data.BQL.BqlBool.Field<usrPrepaymentLine> { }
        #endregion

        #region UsrVendorID
        [Vendor(typeof(Search2<BAccountR.bAccountID, LeftJoin<BranchAlias, On<BAccount.isBranch, Equal<True>, And<BranchAlias.bAccountID, Equal<BAccountR.bAccountID>>>>, Where<Vendor.type, NotEqual<BAccountType.employeeType>>>))]
        [PXRestrictor(typeof(Where<Vendor.vStatus, IsNull, Or<Vendor.vStatus, In3<VendorStatus.active, VendorStatus.oneTime, VendorStatus.holdPayments>>>), "The vendor status is '{0}'.", new Type[] { typeof(Vendor.vStatus) })]
        //[VendorActive(DisplayName = "Vendor", DescriptionField = typeof(Vendor.acctName), Enabled = false)]
        public int? UsrVendorID { get; set; }
        public abstract class usrVendorID : PX.Data.BQL.BqlInt.Field<usrVendorID> { }
        #endregion

        #region UsrVendorLocationID
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current<usrVendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
        [PXFormula(typeof(Default<usrVendorID>))]
        //[LocationActive(typeof(Where<PX.Objects.CR.Location.bAccountID, Equal<Optional<usrVendorID>>, And<MatchWithBranch<PX.Objects.CR.Location.vBranchID>>>), 
        //    DescriptionField = typeof(PX.Objects.CR.Location.descr), 
        //    Visibility = PXUIVisibility.SelectorVisible, 
        //    Enabled = false)]
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

        #region UsrItemSpecs
        [PXDBString(2000, IsUnicode = true)]
        [PXUIField(DisplayName = "Specification", Enabled = false)]
        public string UsrItemSpecs { get; set; }
        public abstract class usrItemSpecs : PX.Data.BQL.BqlString.Field<usrItemSpecs> { }
        #endregion

        #region UsrSWKRTHCost
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Cost", Enabled = false)]
        public decimal? UsrSWKRTHCost { get; set; }
        public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
        #endregion

        #region UsrSWKSPCCode
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "SPC Code", Enabled = false)]
        public string UsrSWKSPCCode { get; set; }
        public abstract class usrSWKSPCCode : PX.Data.BQL.BqlString.Field<usrSWKSPCCode> { }
        #endregion
    }
}