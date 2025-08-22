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
using PX.Data.BQL;

using PX.Objects.Common.Bql;

using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.SO
{
	public class SOSiteStatusProjectionAttribute : PXProjectionAttribute
	{
		public SOSiteStatusProjectionAttribute(Type branch, Type customer, Type customerLocation, Type currentBehavior)
			: base(BqlTemplate.OfCommand<
				Select2<InventoryItem,
				LeftJoin<INSiteStatusByCostCenter,
					On<INSiteStatusByCostCenter.inventoryID, Equal<InventoryItem.inventoryID>,
						And<InventoryItem.stkItem, Equal<boolTrue>,
						And<INSiteStatusByCostCenter.siteID, NotEqual<SiteAttribute.transitSiteID>,
						And<INSiteStatusByCostCenter.costCenterID, Equal<CostCenter.freeStock>>>>>,
				LeftJoin<INSubItem,
					On<INSiteStatusByCostCenter.FK.SubItem>,
				LeftJoin<INSite,
					On2<INSiteStatusByCostCenter.FK.Site,
						And<Where<INSite.baseCuryID, EqualBaseCuryID<Current2<BqlPlaceholder.B>>,
							Or<CurrentValue<BqlPlaceholder.B>, IsNull,
								And<INSite.baseCuryID, EqualBaseCuryID<Current2<AccessInfo.branchID>>>>>>>,
				LeftJoin<INItemXRef,
					On<INItemXRef.inventoryID, Equal<InventoryItem.inventoryID>,
						And2<Where<INItemXRef.subItemID, Equal<INSiteStatusByCostCenter.subItemID>,
								Or<INSiteStatusByCostCenter.subItemID, IsNull>>,
						And<Where<CurrentValue<SOSiteStatusFilter.barCode>, IsNotNull,
						And<INItemXRef.alternateType, In3<INAlternateType.barcode, INAlternateType.gIN>>>>>>,
				LeftJoin<INItemPartNumber,
					On<INItemPartNumber.inventoryID, Equal<InventoryItem.inventoryID>,
						And<INItemPartNumber.alternateID, Like<CurrentValue<SOSiteStatusFilter.inventory_Wildcard>>,
						And2<Where<INItemPartNumber.bAccountID, Equal<Zero>,
							Or<INItemPartNumber.bAccountID, Equal<CurrentValue<BqlPlaceholder.C>>,
							Or<INItemPartNumber.alternateType, Equal<INAlternateType.vPN>>>>,
						And<Where<INItemPartNumber.subItemID, Equal<INSiteStatusByCostCenter.subItemID>,
							Or<INSiteStatusByCostCenter.subItemID, IsNull>>>>>>,
				LeftJoin<INItemClass,
					On<InventoryItem.FK.ItemClass>,
				LeftJoin<INPriceClass,
					On<INPriceClass.priceClassID, Equal<InventoryItem.priceClassID>>,
				LeftJoin<InventoryItemCurySettings,
					On<InventoryItemCurySettings.inventoryID, Equal<InventoryItem.inventoryID>,
						And<Where<InventoryItemCurySettings.curyID, EqualBaseCuryID<Current2<BqlPlaceholder.B>>,
							Or<CurrentValue<BqlPlaceholder.B>, IsNull,
								And<InventoryItemCurySettings.curyID, EqualBaseCuryID<Current2<AccessInfo.branchID>>>>>>>,
				LeftJoin<BAccountR,
					On<BAccountR.bAccountID, Equal<InventoryItemCurySettings.preferredVendorID>>,
				LeftJoin<INItemCustSalesStats,
					On<CurrentValue<SOSiteStatusFilter.mode>, Equal<SOAddItemMode.byCustomer>,
						And<INItemCustSalesStats.inventoryID, Equal<InventoryItem.inventoryID>,
						And<INItemCustSalesStats.subItemID, Equal<INSiteStatusByCostCenter.subItemID>,
						And<INItemCustSalesStats.siteID, Equal<INSiteStatusByCostCenter.siteID>,
						And<INItemCustSalesStats.bAccountID, Equal<CurrentValue<BqlPlaceholder.C>>,
						And<Where<INItemCustSalesStats.lastDate, GreaterEqual<CurrentValue<SOSiteStatusFilter.historyDate>>,
							Or<CurrentValue<SOSiteStatusFilter.dropShipSales>, Equal<True>,
								And<INItemCustSalesStats.dropShipLastDate, GreaterEqual<CurrentValue<SOSiteStatusFilter.historyDate>>>>>>>>>>>,
				LeftJoin<Location,
					On<Location.bAccountID, Equal<CurrentValue<BqlPlaceholder.C>>,
						And<Location.locationID, Equal<CurrentValue<BqlPlaceholder.L>>>>,
				LeftJoin<INUnit,
					On<INUnit.inventoryID, Equal<InventoryItem.inventoryID>,
						And<INUnit.unitType, Equal<INUnitType.inventoryItem>,
						And<INUnit.fromUnit, Equal<InventoryItem.salesUnit>,
						And<INUnit.toUnit, Equal<InventoryItem.baseUnit>>>>>
					>>>>>>>>>>>>,
				Where<CurrentValue<BqlPlaceholder.C>, IsNotNull,
					And2<CurrentMatch<InventoryItem, AccessInfo.userName>,
					And2<Where<INSiteStatusByCostCenter.siteID, IsNull, Or<INSite.branchID, IsNotNull, And2<CurrentMatch<INSite, AccessInfo.userName>,
						And<Where2<FeatureInstalled<FeaturesSet.interBranch>,
							Or2<SameOrganizationBranch<INSite.branchID, Current2<BqlPlaceholder.B>>,
							Or2<Where<CurrentValue<BqlPlaceholder.B>, IsNull,
								And<SameOrganizationBranch<INSite.branchID, Current2<AccessInfo.branchID>>>>,
							Or<BqlPlaceholder.E, Equal<SOBehavior.qT>>>>>>>>>,
					And2<Where<INSiteStatusByCostCenter.subItemID, IsNull, Or<CurrentMatch<INSubItem, AccessInfo.userName>>>,
					And2<Where<CurrentValue<INSiteStatusFilter.onlyAvailable>, Equal<boolFalse>,
						Or<INSiteStatusByCostCenter.qtyAvail, Greater<CS.decimal0>>>,
					And2<Where<CurrentValue<SOSiteStatusFilter.mode>, Equal<SOAddItemMode.bySite>,
						Or<INItemCustSalesStats.lastQty, Greater<decimal0>,
						Or<CurrentValue<SOSiteStatusFilter.dropShipSales>, Equal<True>, And<INItemCustSalesStats.dropShipLastQty, Greater<decimal0>>>>>,
					And<InventoryItem.isTemplate, Equal<False>,
					And<InventoryItem.itemStatus, NotIn3<
						InventoryItemStatus.unknown,
						InventoryItemStatus.inactive,
						InventoryItemStatus.markedForDeletion,
						InventoryItemStatus.noSales>>>>>>>>>>>
				.Replace<BqlPlaceholder.B>(branch)
				.Replace<BqlPlaceholder.C>(customer)
				.Replace<BqlPlaceholder.L>(customerLocation)
				.Replace<BqlPlaceholder.E>(currentBehavior)
				.ToType())
		{
		}
	}
}
