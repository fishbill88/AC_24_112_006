using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.TX;
using PX.SM;
using System;
using static PX.Objects.GL.ControlAccountModule;

namespace ItemRequestCustomization
{
    [Serializable]
    [PXCacheName("Item Request")]
    public class InventoryRequest : PXBqlTable, IBqlTable
    {

        public static class FK
        {
            public class DefaultSite : INSite.PK.ForeignKeyOf<InventoryItemCurySettings>.By<defaultWarehouse> { }
            public class VendorLocation : Location.PK.ForeignKeyOf<POVendorInventory>.By<vendorID, vendorLocationID> { }
        }

        #region RefNbr
        [PXDBString(15, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC", IsUnicode = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Reference Nbr")]
        [PXSelector(typeof(Search<InventoryRequest.refNbr>),
                    typeof(InventoryRequest.refNbr),
                    typeof(InventoryRequest.dateSubmitted),
                    typeof(InventoryRequest.requestorName),
                    typeof(InventoryRequest.status)
            )]
        //[AutoNumber(typeof(INSetupExt.usrItemRequestNumberingID), typeof(AccessInfo.businessDate))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : BqlString.Field<refNbr> { }
        #endregion

        #region InventoryCD
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Inventory ID")]
        [PXUIVisible(typeof(Where<InventoryRequest.inventoryID, IsNull>))]
        public virtual string InventoryCD { get; set; }
        public abstract class inventoryCD : BqlString.Field<inventoryCD> { }
        #endregion

        #region InventoryID
        [StockItem()]
        [PXUIVisible(typeof(Where<InventoryRequest.inventoryID, IsNotNull>))]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : BqlInt.Field<inventoryID> { }
        #endregion

        #region DateSubmitted
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Date Submitted", Enabled = false)]
        public virtual DateTime? DateSubmitted { get; set; }
        public abstract class dateSubmitted : BqlDateTime.Field<dateSubmitted> { }
        #endregion

        #region RequestorName
        [PXDBString(128, IsUnicode = true)]
        [PXDefault(typeof(AccessInfo.userName))]
        [PXSelector(typeof(Search<Users.pKID>),
                            typeof(Users.pKID),
                            SubstituteKey = typeof(Users.username),
                            DescriptionField = typeof(Users.fullName))]
        [PXUIField(DisplayName = "Requestor's Name", Enabled = false)]
        public virtual string RequestorName { get; set; }
        public abstract class requestorName : BqlString.Field<requestorName> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true)]
        [PXDefault("H")]
        [PXStringList(new[] { "O", "H", "C" }, new[] { "Open", "On-Hold", "Closed" })]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        public virtual string Status { get; set; }
        public abstract class status : BqlString.Field<status> { }
        #endregion

        #region PartNumber
        [PXDBString(30, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Part Number")]
        public virtual string PartNumber { get; set; }
        public abstract class partNumber : BqlString.Field<partNumber> { }
        #endregion

        #region ItemDescription
        [PXDBString(256, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Item Description")]
        public virtual string ItemDescription { get; set; }
        public abstract class itemDescription : BqlString.Field<itemDescription> { }
        #endregion
        
        #region SerialClassID
        [PXString(10, IsUnicode = true)]
        [PXSelector(typeof(INLotSerClass.lotSerClassID), DescriptionField = typeof(INLotSerClass.descr), CacheGlobal = true)]
        [PXUIField(DisplayName = "Lot/Serial Class", Required = true)]
        [PXUnboundDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string SerialClassID { get; set; }
        public abstract class serialClassID : PX.Data.BQL.BqlString.Field<serialClassID> { }
        #endregion

        //#region ModelNumber
        //[PXDBString(50, IsUnicode = true)]
        ////[PXDefault()]
        //[PXUIField(DisplayName = "Model Number",Visible = false)]
        //public virtual string ModelNumber { get; set; }
        //public abstract class modelNumber : BqlString.Field<modelNumber> { }
        //#endregion

        #region Serialized
        [PXDBString(10, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXStringList(new[] { "SERIAL", "NONE" }, new[] { "Yes", "No" })]
        [PXUIField(DisplayName = "Serialized", Required = true)]
        public virtual string Serialized { get; set; }
        public abstract class serialized : BqlString.Field<serialized> { }
        #endregion

        #region ProductBrand
        [PXDBString(50, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Product Brand")]
        public virtual string ProductBrand { get; set; }
        public abstract class productBrand : BqlString.Field<productBrand> { }
        #endregion

        #region ItemClassID
        [PXDBInt]
        [PXDimensionSelector(INItemClass.Dimension, 
                                typeof(Search<INItemClass.itemClassID>), 
                                typeof(INItemClass.itemClassCD), 
                                typeof(INItemClass.taxCategoryID), 
                                typeof(INItemClass.postClassID), 
            DescriptionField = typeof(INItemClass.descr))]
        [PXDefault()]
        [PXUIField(DisplayName = "Item Class", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? ItemClassID { get; set; }
        public abstract class itemClassID : BqlInt.Field<itemClassID> { }
        #endregion

        #region PostClassID
        /// <summary>
        /// Identifier of the <see cref="INPostClass">Posting Class</see> associated with the item.
        /// </summary>
        /// <value>
        /// Defaults to the <see cref="INItemClass.PostClassID">Posting Class</see> selected for the <see cref="ItemClassID">item class</see>.
        /// Corresponds to the <see cref="INPostClass.PostClassID"/> field.
        /// </value>
        [PXString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Posting Class", Enabled = false)]
        [PXUnboundDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(Search<INPostClass.postClassID>),
                    typeof(INPostClass.postClassID),
                    typeof(INPostClass.descr),
                    DescriptionField = typeof(INPostClass.descr))]
        [PXFormula(typeof(Selector<itemClassID, INItemClass.postClassID>))]
        public virtual String PostClassID { get; set; }
        public abstract class postClassID : BqlString.Field<postClassID> { }
        #endregion

        #region TaxCategoryID
        /// <summary>
        /// Identifier of the <see cref="TaxCategory"/> associated with the item.
        /// </summary>
        /// <value>
        /// Defaults to the <see cref="INItemClass.TaxCategoryID">Tax Category</see> associated with the <see cref="ItemClassID">Item Class</see>.
        /// Corresponds to the <see cref="TaxCategory.TaxCategoryID"/> field.
        /// </value>
        [PXString(TaxCategory.taxCategoryID.Length, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [PXUnboundDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXFormula(typeof(Selector<itemClassID, INItemClass.taxCategoryID>))]
        public virtual String TaxCategoryID { get; set; }
        public abstract class taxCategoryID : BqlString.Field<taxCategoryID> { }
        #endregion

        #region IsAKit
        [PXBool()]
        [PXUIField(DisplayName = "Is a Kit")]
        public virtual bool? IsAKit { get; set; }
        public abstract class isAKit : BqlBool.Field<isAKit> { }
        #endregion

        #region StdUnitOfMeasure
        [INUnit(DisplayName = "Std Unit of Measure", Visibility = PXUIVisibility.SelectorVisible, DirtyRead = true)]
        [PXDefault("EA")]
        public virtual string StdUnitOfMeasure { get; set; }
        public abstract class stdUnitOfMeasure : BqlString.Field<stdUnitOfMeasure> { }
        #endregion

        #region DefaultWarehouse
        [PX.Objects.IN.Site(DisplayName = "Default Warehouse", DescriptionField = typeof(INSite.descr))]
        [PXForeignReference(typeof(FK.DefaultSite))]
        public virtual int? DefaultWarehouse { get; set; }
        public abstract class defaultWarehouse : BqlInt.Field<defaultWarehouse> { }
        #endregion

        #region MinOrderQty
        [PXDBDecimal(6)]
        [PXDefault()]
        [PXUIField(DisplayName = "Min Order Qty")]
        public virtual decimal? MinOrderQty { get; set; }
        public abstract class minOrderQty : BqlDecimal.Field<minOrderQty> { }
        #endregion

        #region ListPrice
        [PXDBDecimal(6)]
        [PXDefault()]
        [PXUIField(DisplayName = "List Price")]
        public virtual decimal? ListPrice { get; set; }
        public abstract class listPrice : BqlDecimal.Field<listPrice> { }
        #endregion

        #region OurCost
        [PXDBDecimal(6)]
        [PXDefault()]
        [PXUIField(DisplayName = "Our Cost")]
        public virtual decimal? OurCost { get; set; }
        public abstract class ourCost : BqlDecimal.Field<ourCost> { }
        #endregion

        #region NotesCatalogLink
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Notes/Catalog Link")]
        public virtual string NotesCatalogLink { get; set; }
        public abstract class notesCatalogLink : BqlString.Field<notesCatalogLink> { }
        #endregion

        #region CountryOfOrigin
        [PXDBString(100, IsUnicode = true)]
        [PXUIField(DisplayName = "Country Of Origin")]
        [Country]
        public virtual string CountryOfOrigin { get; set; }
        public abstract class countryOfOrigin : BqlString.Field<countryOfOrigin> { }
        #endregion

        #region HTSCode
        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "HTS Code")]
        public virtual string HTSCode { get; set; }
        public abstract class htsCode : BqlString.Field<htsCode> { }
        #endregion

        #region WeightLbs
        [PXDBDecimal(6)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Weight (Lbs)")]
        public virtual decimal? WeightLbs { get; set; }
        public abstract class weightLbs : BqlDecimal.Field<weightLbs> { }
        #endregion

        #region CustomerID
        [CustomerActive]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : BqlInt.Field<customerID> { }
        #endregion

        #region CustomerAlternateID
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Customer Alternate ID")]
        public virtual string CustomerAlternateID { get; set; }
        public abstract class customerAlternateID : BqlString.Field<customerAlternateID> { }
        #endregion

        #region PlanningMethod
        [PXDBString(1, IsFixed = true)]
        [PXDefault(INPlanningMethod.InventoryReplenishment)]
        [PXUIField(DisplayName = "Planning Method", Enabled = false)]
        [INPlanningMethod.List]
        public virtual string PlanningMethod { get; set; }
        public abstract class planningMethod : BqlString.Field<planningMethod> { }
        #endregion

        #region ReplenishmentClass1
        [PXDBString(20, IsUnicode = true)]
        [PXDefault("Purchase")]
        [PXSelector(typeof(Search<INReplenishmentClass.replenishmentClassID>))]
        [PXUIField(DisplayName = "Replenishment Class 1", Enabled = false)]
        public virtual string ReplenishmentClass1 { get; set; }
        public abstract class replenishmentClass1 : BqlString.Field<replenishmentClass1> { }
        #endregion

        #region Seasonality1
        [PXDBString(20, IsUnicode = true)]
        [PXDefault("NONE")]
        [PXSelector(typeof(Search<INReplenishmentPolicy.replenishmentPolicyID>), DescriptionField = typeof(INReplenishmentPolicy.descr))]
        [PXUIField(DisplayName = "Seasonality 1", Enabled = false)]
        public virtual string Seasonality1 { get; set; }
        public abstract class seasonality1 : BqlString.Field<seasonality1> { }
        #endregion

        #region Source1
        [PXDBString(1, IsFixed = true)]
        [PXDefault("P")]
        [PXUIField(DisplayName = "Source 1", Enabled = false)]
        [INReplenishmentSource.List]
        public virtual string Source1 { get; set; }
        public abstract class source1 : BqlString.Field<source1> { }
        #endregion

        #region Method1
        [PXDBString(1, IsFixed = true)]
        [PXDefault("M")]
        [INReplenishmentMethod.List]
        [PXUIField(DisplayName = "Method 1", Enabled = false)]
        public virtual string Method1 { get; set; }
        public abstract class method1 : BqlString.Field<method1> { }
        #endregion

        #region ReplenishmentClass2
        [PXDBString(20, IsUnicode = true)]
        [PXDefault("PURTOORD")]
        [PXSelector(typeof(Search<INReplenishmentClass.replenishmentClassID>))]
        [PXUIField(DisplayName = "Replenishment Class 2", Visible = false)]
        public virtual string ReplenishmentClass2 { get; set; }
        public abstract class replenishmentClass2 : BqlString.Field<replenishmentClass2> { }
        #endregion

        #region Seasonality2
        [PXDBString(20, IsUnicode = true)]
        [PXDefault("NONE")]
        [PXSelector(typeof(Search<INReplenishmentPolicy.replenishmentPolicyID>), DescriptionField = typeof(INReplenishmentPolicy.descr))]
        [PXUIField(DisplayName = "Seasonality 2", Visible = false)]
        public virtual string Seasonality2 { get; set; }
        public abstract class seasonality2 : BqlString.Field<seasonality2> { }
        #endregion

        #region Source2
        [PXDBString(1, IsFixed = true)]
        [PXDefault("O")]
        [PXUIField(DisplayName = "Source 2", Visible = false)]
        [INReplenishmentSource.List]
        public virtual string Source2 { get; set; }
        public abstract class source2 : BqlString.Field<source2> { }
        #endregion

        #region Method2
        [PXDBString(1, IsFixed = true)]
        [PXDefault("N")]
        [PXUIField(DisplayName = "Method 2", Visible = false)]
        [INReplenishmentMethod.List]
        public virtual string Method2 { get; set; }
        public abstract class method2 : BqlString.Field<method2> { }
        #endregion

        #region VendorID
        [Vendor(DisplayName = "Vendor")]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : BqlInt.Field<vendorID> { }
        #endregion

        #region VendorLocationID
        public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
        protected Int32? _VendorLocationID;
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current<vendorID>>>),
            DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Location")]
        [PXFormula(typeof(Default<vendorID>))]
        [PXParent(typeof(FK.VendorLocation))]
        [PXDefault()]
        [PXUIRequired(typeof(Where<Current<vendorID>,IsNotNull>))]
        public virtual Int32? VendorLocationID
        {
            get
            {
                return this._VendorLocationID;
            }
            set
            {
                this._VendorLocationID = value;
            }
        }
        #endregion

        #region VendorSiteID
        //[PX.Objects.IN.Site(DisplayName = "Default Warehouse", DescriptionField = typeof(INSite.descr),Enabled = false)]
        [PXDBInt]
        [PXUIField(DisplayName = "Vendor Site ID")]
        [PXSelector(
            typeof(Search2<INSite.siteID,
                InnerJoin<Location, On<Location.vSiteID, Equal<INSite.siteID>>>
                , Where<Location.bAccountID, Equal<Current<vendorID>>,
                And<Location.locationID, Equal<Current<vendorLocationID>>>>>),
            SubstituteKey = typeof(INSite.siteCD),
            DescriptionField = typeof(INSite.descr))]
        [PXFormula(typeof(Search<Location.vSiteID,
            Where<Location.bAccountID, Equal<Current<vendorID>>,
              And<Location.locationID, Equal<Current<vendorLocationID>>>>>))]
        [PXFormula(typeof(Default<InventoryRequest.vendorID, InventoryRequest.vendorLocationID>))]
        public virtual int? VendorSiteID { get; set; }
        public abstract class vendorSiteID : BqlInt.Field<vendorSiteID> { }
        #endregion

        #region VendorInventoryID
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Inventory ID")]
        [PXDefault()]
        [PXUIRequired(typeof(Where<Current<vendorID>, IsNotNull>))]
        public virtual string VendorInventoryID { get; set; }
        public abstract class vendorInventoryID : BqlString.Field<vendorInventoryID> { }
        #endregion

        #region ItemSpecs
        [PXDBString(2000, IsUnicode = true)]
        [PXUIField(DisplayName = "Item Specs")]
        public virtual string ItemSpecs { get; set; }
        public abstract class itemSpecs : PX.Data.BQL.BqlString.Field<itemSpecs> { }
        #endregion

        #region NoteID
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : BqlGuid.Field<noteID> { }
        #endregion

        #region CreatedByID
        /// <summary>
        /// Audit Bql field.
        /// </summary>
        public abstract class createdByID : IBqlField { }
        /// <summary>
        /// Audit Bql property field.
        /// </summary>
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        #endregion

        #region CreatedByScreenID
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        protected String _CreatedByScreenID;
        [PXDBCreatedByScreenID()]
        public virtual String CreatedByScreenID
        {
            get
            {
                return this._CreatedByScreenID;
            }
            set
            {
                this._CreatedByScreenID = value;
            }
        }
        #endregion

        #region CreatedDateTime
        /// <summary>
        /// Audit Bql field.
        /// </summary>
        public abstract class createdDateTime : IBqlField { }
        /// <summary>
        /// Audit Bql property field.
        /// </summary>
        [PXDBCreatedDateTime()]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime, Enabled = false, IsReadOnly = true)]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion

        #region LastModifiedByID
        /// <summary>
        /// Audit Bql field.
        /// </summary>
        public abstract class lastModifiedByID : IBqlField { }
        /// <summary>
        /// Audit Bql property field.
        /// </summary>
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        #endregion

        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        protected String _LastModifiedByScreenID;
        [PXDBLastModifiedByScreenID()]
        public virtual String LastModifiedByScreenID
        {
            get
            {
                return this._LastModifiedByScreenID;
            }
            set
            {
                this._LastModifiedByScreenID = value;
            }
        }
        #endregion

        #region LastModifiedDateTime
        /// <summary>
        /// Audit Bql field.
        /// </summary>
        public abstract class lastModifiedDateTime : IBqlField { }
        /// <summary>
        /// Audit Bql property field.
        /// </summary>
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedDateTime, Enabled = false, IsReadOnly = true)]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion

        #region tstamp
        /// <summary>
        /// Audit Bql field.
        /// </summary>
        public abstract class Tstamp : IBqlField { }
        /// <summary>
        /// Audit Bql property.
        /// </summary>
        [PXDBTimestamp()]
        public virtual Byte[] tstamp { get; set; }
        #endregion
    }
}