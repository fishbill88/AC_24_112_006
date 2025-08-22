/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using System;
using PX.Data;

using PX.Objects.Common.Interfaces;

using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.AR;

namespace PX.Objects.SO
{
	/// <exclude />
	[PXCacheName(Messages.InvoiceInventoryLookupRow)]
	[SOSiteStatusProjection(typeof(Location.cBranchID), typeof(ARInvoice.customerID), typeof(ARInvoice.customerLocationID), typeof(Null))]
	public partial class SOInvoiceSiteStatusSelected : PXBqlTable, IQtySelectable, IPXSelectable, IBqlTable
	{
		#region Selected
		[PXBool]
		[PXUnboundDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected { get; set; }
		public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
		#endregion

		#region InventoryID
		[Inventory(BqlField = typeof(InventoryItem.inventoryID), IsKey = true)]
		[PXDefault]
		public virtual Int32? InventoryID { get; set; }
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
		#endregion

		#region InventoryCD
		[PXDefault]
		[InventoryRaw(BqlField = typeof(InventoryItem.inventoryCD))]
		public virtual String InventoryCD { get; set; }
		public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
		#endregion

		#region Descr
		[PXDBLocalizableString(Common.Constants.TranDescLength, IsUnicode = true, BqlField = typeof(InventoryItem.descr), IsProjection = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Descr { get; set; }
		public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }
		#endregion

		#region ItemClassID
		[PXDBInt(BqlField = typeof(InventoryItem.itemClassID))]
		[PXUIField(DisplayName = "Item Class ID", Visible = false)]
		[PXDimensionSelector(INItemClass.Dimension, typeof(INItemClass.itemClassID), typeof(INItemClass.itemClassCD), ValidComboRequired = true)]
		public virtual int? ItemClassID { get; set; }
		public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }
		#endregion

		#region ItemClassCD
		[PXDBString(30, IsUnicode = true, BqlField = typeof(INItemClass.itemClassCD))]
		public virtual string ItemClassCD { get; set; }
		public abstract class itemClassCD : PX.Data.BQL.BqlString.Field<itemClassCD> { }
		#endregion

		#region ItemClassDescription
		[PXDBLocalizableString(Common.Constants.TranDescLength, IsUnicode = true, BqlField = typeof(INItemClass.descr), IsProjection = true)]
		[PXUIField(DisplayName = "Item Class Description", Visible = false, ErrorHandling = PXErrorHandling.Always)]
		public virtual String ItemClassDescription { get; set; }
		public abstract class itemClassDescription : PX.Data.BQL.BqlString.Field<itemClassDescription> { }
		#endregion

		#region PriceClassID
		[PXDBString(10, IsUnicode = true, BqlField = typeof(InventoryItem.priceClassID))]
		[PXUIField(DisplayName = "Price Class ID", Visible = false)]
		public virtual String PriceClassID { get; set; }
		public abstract class priceClassID : PX.Data.BQL.BqlString.Field<priceClassID> { }
		#endregion

		#region PriceClassDescription
		[PXDBString(Common.Constants.TranDescLength, IsUnicode = true, BqlField = typeof(INPriceClass.description))]
		[PXUIField(DisplayName = "Price Class Description", Visible = false, ErrorHandling = PXErrorHandling.Always)]
		public virtual String PriceClassDescription { get; set; }
		public abstract class priceClassDescription : PX.Data.BQL.BqlString.Field<priceClassDescription> { }
		#endregion

		#region PreferredVendorID
		[AP.VendorNonEmployeeActive(DisplayName = "Preferred Vendor ID", Required = false, DescriptionField = typeof(BAccountR.acctName), BqlField = typeof(InventoryItemCurySettings.preferredVendorID), Visible = false, ErrorHandling = PXErrorHandling.Always)]
		public virtual Int32? PreferredVendorID { get; set; }
		public abstract class preferredVendorID : PX.Data.BQL.BqlInt.Field<preferredVendorID> { }
		#endregion

		#region PreferredVendorDescription
		[PXDBString(250, IsUnicode = true, BqlField = typeof(BAccountR.acctName))]
		[PXUIField(DisplayName = "Preferred Vendor Name", Visible = false, ErrorHandling = PXErrorHandling.Always)]
		public virtual String PreferredVendorDescription { get; set; }
		public abstract class preferredVendorDescription : PX.Data.BQL.BqlString.Field<preferredVendorDescription> { }
		#endregion

		#region BarCode
		[PXDBString(255, BqlField = typeof(INItemXRef.alternateID), IsUnicode = true)]
		[PXUIField(DisplayName = "Barcode", Visible = false)]
		public virtual String BarCode { get; set; }
		public abstract class barCode : PX.Data.BQL.BqlString.Field<barCode> { }
		#endregion

		#region AlternateID
		[PXDBString(225, IsUnicode = true, InputMask = "", BqlField = typeof(INItemPartNumber.alternateID))]
		[PXUIField(DisplayName = "Alternate ID")]
		[PXExtraKey]
		public virtual String AlternateID { get; set; }
		public abstract class alternateID : PX.Data.BQL.BqlString.Field<alternateID> { }
		#endregion

		#region AlternateType
		[PXDBString(4, BqlField = typeof(INItemPartNumber.alternateType))]
		[INAlternateType.List]
		[PXDefault(INAlternateType.Global)]
		[PXUIField(DisplayName = "Alternate Type")]
		public virtual String AlternateType { get; set; }
		public abstract class alternateType : PX.Data.BQL.BqlString.Field<alternateType> { }
		#endregion

		#region Descr
		[PXDBString(60, IsUnicode = true, BqlField = typeof(INItemPartNumber.descr))]
		[PXUIField(DisplayName = "Alternate Description", Visible = false)]
		public virtual String AlternateDescr { get; set; }
		public abstract class alternateDescr : PX.Data.BQL.BqlString.Field<alternateDescr> { }
		#endregion

		#region SiteID
		[PXUIField(DisplayName = "Warehouse")]
		[Site(BqlField = typeof(INSiteStatusByCostCenter.siteID))]
		public virtual Int32? SiteID { get; set; }
		public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
		#endregion

		#region SiteCD
		[PXString(IsUnicode = true, IsKey = true)]
		[PXDBCalced(typeof(IsNull<RTrim<INSite.siteCD>, Empty>), typeof(string))]
		public virtual String SiteCD { get; set; }
		public abstract class siteCD : PX.Data.BQL.BqlString.Field<siteCD> { }
		#endregion

		#region SubItemID
		[SubItem(typeof(inventoryID), BqlField = typeof(INSubItem.subItemID))]
		public virtual Int32? SubItemID { get; set; }
		public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
		#endregion

		#region SubItemCD
		[PXString(IsUnicode = true, IsKey = true)]
		[PXDBCalced(typeof(IsNull<RTrim<INSubItem.subItemCD>, Empty>), typeof(string))]
		public virtual String SubItemCD { get; set; }
		public abstract class subItemCD : PX.Data.BQL.BqlString.Field<subItemCD> { }
		#endregion

		#region BaseUnit
		[INUnit(DisplayName = "Base Unit", Visibility = PXUIVisibility.Visible, BqlField = typeof(InventoryItem.baseUnit))]
		public virtual String BaseUnit { get; set; }
		public abstract class baseUnit : PX.Data.BQL.BqlString.Field<baseUnit> { }
		#endregion

		#region CuryID
		[PXString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String CuryID { get; set; }
		public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }
		#endregion

		#region CuryInfoID
		[PXLong]
		[CurrencyInfo]
		public virtual Int64? CuryInfoID { get; set; }
		public abstract class curyInfoID : PX.Data.BQL.BqlLong.Field<curyInfoID> { }
		#endregion

		#region SalesUnit
		[INUnit(typeof(inventoryID), DisplayName = "Sales Unit", BqlField = typeof(InventoryItem.salesUnit))]
		public virtual String SalesUnit { get; set; }
		public abstract class salesUnit : PX.Data.BQL.BqlString.Field<salesUnit> { }
		#endregion

		#region QtySelected
		public abstract class qtySelected : PX.Data.BQL.BqlDecimal.Field<qtySelected> { }

		protected Decimal? _QtySelected;
		[PXQuantity]
		[PXUnboundDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Selected")]
		public virtual Decimal? QtySelected
		{
			get
			{
				return _QtySelected ?? 0m;
			}
			set
			{
				if (value != null && value != 0m)
					Selected = true;
				_QtySelected = value;
			}
		}
		#endregion

		#region QtyOnHand
		[PXDBQuantity(BqlField = typeof(INSiteStatusByCostCenter.qtyOnHand))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. On Hand")]
		public virtual Decimal? QtyOnHand { get; set; }
		public abstract class qtyOnHand : PX.Data.BQL.BqlDecimal.Field<qtyOnHand> { }
		#endregion

		#region QtyAvail
		[PXDBQuantity(BqlField = typeof(INSiteStatusByCostCenter.qtyAvail))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Available")]
		public virtual Decimal? QtyAvail { get; set; }
		public abstract class qtyAvail : PX.Data.BQL.BqlDecimal.Field<qtyAvail> { }
		#endregion

		#region QtyLast
		[PXDBQuantity(BqlField = typeof(INItemCustSalesStats.lastQty))]
		public virtual Decimal? QtyLast { get; set; }
		public abstract class qtyLast : PX.Data.BQL.BqlDecimal.Field<qtyLast> { }
		#endregion

		#region BaseUnitPrice
		[PXDBPriceCost(true, BqlField = typeof(INItemCustSalesStats.lastUnitPrice))]
		public virtual Decimal? BaseUnitPrice { get; set; }
		public abstract class baseUnitPrice : PX.Data.BQL.BqlDecimal.Field<baseUnitPrice> { }
		#endregion

		#region CuryUnitPrice
		[PXUnitPriceCuryConv(typeof(curyInfoID), typeof(baseUnitPrice))]
		[PXUIField(DisplayName = "Last Unit Price", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryUnitPrice { get; set; }
		public abstract class curyUnitPrice : PX.Data.BQL.BqlDecimal.Field<curyUnitPrice> { }
		#endregion

		#region QtyAvailSale
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INSiteStatusByCostCenter.qtyAvail, INUnit.unitRate>>,
			Div<INSiteStatusByCostCenter.qtyAvail, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Available")]
		public virtual Decimal? QtyAvailSale { get; set; }
		public abstract class qtyAvailSale : PX.Data.BQL.BqlDecimal.Field<qtyAvailSale> { }
		#endregion

		#region QtyOnHandSale
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INSiteStatusByCostCenter.qtyOnHand, INUnit.unitRate>>,
			Div<INSiteStatusByCostCenter.qtyOnHand, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. On Hand")]
		public virtual Decimal? QtyOnHandSale { get; set; }
		public abstract class qtyOnHandSale : PX.Data.BQL.BqlDecimal.Field<qtyOnHandSale> { }
		#endregion

		#region QtyLastSale
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INItemCustSalesStats.lastQty, INUnit.unitRate>>,
			Div<INItemCustSalesStats.lastQty, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity]
		[PXUIField(DisplayName = "Qty. Last Sales")]
		public virtual Decimal? QtyLastSale { get; set; }
		public abstract class qtyLastSale : PX.Data.BQL.BqlDecimal.Field<qtyLastSale> { }
		#endregion

		#region LastSalesDate
		[PXDBDate(BqlField = typeof(INItemCustSalesStats.lastDate))]
		[PXUIField(DisplayName = "Last Sales Date")]
		public virtual DateTime? LastSalesDate { get; set; }
		public abstract class lastSalesDate : PX.Data.BQL.BqlDateTime.Field<lastSalesDate> { }
		#endregion

		#region DropShipLastQty
		[PXDBQuantity(BqlField = typeof(INItemCustSalesStats.dropShipLastQty))]
		public virtual Decimal? DropShipLastBaseQty { get; set; }
		public abstract class dropShipLastBaseQty : PX.Data.BQL.BqlDecimal.Field<dropShipLastBaseQty> { }
		#endregion

		#region DropShipLastQty
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INItemCustSalesStats.dropShipLastQty, INUnit.unitRate>>,
			Div<INItemCustSalesStats.dropShipLastQty, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity]
		[PXUIField(DisplayName = "Qty. of Last Drop Ship")]
		public virtual Decimal? DropShipLastQty { get; set; }
		public abstract class dropShipLastQty : PX.Data.BQL.BqlDecimal.Field<dropShipLastQty> { }
		#endregion

		#region DropShipLastUnitPrice
		[PXDBPriceCost(true, BqlField = typeof(INItemCustSalesStats.dropShipLastUnitPrice))]
		public virtual Decimal? DropShipLastUnitPrice { get; set; }
		public abstract class dropShipLastUnitPrice : PX.Data.BQL.BqlDecimal.Field<dropShipLastUnitPrice> { }
		#endregion

		#region DropShipCuryUnitPrice
		[PXUnitPriceCuryConv(typeof(curyInfoID), typeof(dropShipLastUnitPrice))]
		[PXUIField(DisplayName = "Unit Price of Last Drop Ship", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? DropShipCuryUnitPrice { get; set; }
		public abstract class dropShipCuryUnitPrice : PX.Data.BQL.BqlDecimal.Field<dropShipCuryUnitPrice> { }
		#endregion

		#region DropShipLastDate
		[PXDBDate(BqlField = typeof(INItemCustSalesStats.dropShipLastDate))]
		[PXUIField(DisplayName = "Date of Last Drop Ship")]
		public virtual DateTime? DropShipLastDate { get; set; }
		public abstract class dropShipLastDate : PX.Data.BQL.BqlDateTime.Field<dropShipLastDate> { }
		#endregion

		#region NoteID
		[PXNote(BqlField = typeof(InventoryItem.noteID))]
		public virtual Guid? NoteID { get; set; }
		public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
		#endregion
	}
}
