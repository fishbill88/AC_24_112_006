import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo
} from 'client-controls';

import {
	INItemSite,
	INItemSiteGeneral,
	INItemSiteReplenishmentSettings,
	INItemSiteInventoryPlanning,
	INItemSitePreferredVendor,
	POVendorInventory,
	INItemSiteProductionOrder,
	SubitemReplenishment,
	INItemSiteManufacturing,
	AMSubItemDefault
} from './views';

@graphInfo({graphType: 'PX.Objects.IN.INItemSiteMaint', primaryView: 'itemsiterecord', showUDFIndicator: true })
export class IN204500 extends PXScreen {
	ViewBOM: PXActionState;
	ViewPlanningBOM: PXActionState;
	UpdateReplenishment: PXActionState;

	@viewInfo({containerName: 'Item/Warehouse Summary'})
	itemsiterecord = createSingle(INItemSite);

	@viewInfo({containerName: 'General'})
	itemsitesettings = createSingle(INItemSiteGeneral);

	@viewInfo({containerName: 'Replenishment Settings'})
	PlanningReplenishmentSettings = createSingle(INItemSiteReplenishmentSettings);

	@viewInfo({containerName: 'Inventory Planning'})
	inventoryPlanningSettings = createSingle(INItemSiteInventoryPlanning);

	@viewInfo({containerName: 'Preferred Vendor'})
	preferedVendorFields = createSingle(INItemSitePreferredVendor);

	@viewInfo({containerName: 'Vendor Inventory'})
	PreferredVendorItem = createSingle(POVendorInventory);

	@viewInfo({containerName: 'Inventory Planning'})
	productionOrderDefaultSettings = createSingle(INItemSiteProductionOrder);

	@viewInfo({containerName: 'Subitem Replenishment Info'})
	subitemrecords = createCollection(SubitemReplenishment);

	@viewInfo({containerName: 'Manufacturing'})
	manufacturingSettings = createSingle(INItemSiteManufacturing);

	@viewInfo({containerName: 'Sub Item Defaults'})
	AMSubItemDefaults = createCollection(AMSubItemDefault);

}
