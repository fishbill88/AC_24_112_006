using PX.Common;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM.Extensions;
using PX.Objects.Common.Discount.Attributes;
using PX.Objects.Common.Discount;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN.Attributes;
using PX.Objects.IN.Matrix.Interfaces;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;

namespace POInventoryCustomization
{
  public class POLineExt : PXCacheExtension<PX.Objects.PO.POLine>
  {
    #region UsrVendorID
    [VendorActive(DisplayName = "Vendor", DescriptionField = typeof(Vendor.acctName), Enabled = false)]
    public virtual int? UsrVendorID { get; set; }
    public abstract class usrVendorID : PX.Data.BQL.BqlInt.Field<usrVendorID> { }
    #endregion

    #region UsrVendorLocationID
    [LocationActive(typeof(Where<PX.Objects.CR.Location.bAccountID, Equal<Optional<usrVendorID>>, And<MatchWithBranch<PX.Objects.CR.Location.vBranchID>>>), DescriptionField = typeof(PX.Objects.CR.Location.descr), Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
    [PXDefault(typeof(Coalesce<Search2<Vendor.defLocationID, InnerJoin<PX.Objects.CR.Standalone.Location, On<PX.Objects.CR.Standalone.Location.locationID, Equal<Vendor.defLocationID>, And<PX.Objects.CR.Standalone.Location.bAccountID, Equal<Vendor.bAccountID>>>>, Where<Vendor.bAccountID, Equal<Current<usrVendorID>>, And<PX.Objects.CR.Standalone.Location.isActive, Equal<True>, And<MatchWithBranch<PX.Objects.CR.Standalone.Location.vBranchID>>>>>, Search<PX.Objects.CR.Standalone.Location.locationID, Where<PX.Objects.CR.Standalone.Location.bAccountID, Equal<Current<usrVendorID>>, And<PX.Objects.CR.Standalone.Location.isActive, Equal<True>, And<MatchWithBranch<PX.Objects.CR.Standalone.Location.vBranchID>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
    [PXForeignReference(typeof(CompositeKey<Field<usrVendorID>.IsRelatedTo<PX.Objects.CR.Location.bAccountID>, Field<usrVendorLocationID>.IsRelatedTo<PX.Objects.CR.Location.locationID>>))]
    public virtual int? UsrVendorLocationID{ get; set; }
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