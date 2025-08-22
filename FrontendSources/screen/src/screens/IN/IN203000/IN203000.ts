import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo
} from "client-controls";

import {
	ChangeIDParam,
	InventoryItem,
	InventoryItem2,
	InventoryItemCurySettings,
	INUnit,
	INItemCategory,
	INItemBox,
	VendorItems,
	Attributes,
	INMatrixGenerationRule,
	DescriptionGenerationRules,
	EntryHeader,
	AdditionalAttributes,
	EntryMatrix,
	MatrixItemsForCreation,
	INMatrixExcludedData,
	AttributesExcludedFromUpdate,
	MatrixItems,
	BCInventoryFileUrls,
	POVendorPriceUpdate,
	INItemClass
} from "./views";

@graphInfo({graphType: "PX.Objects.IN.Matrix.Graphs.TemplateInventoryItemMaint", primaryView: "Item"})
export class IN203000 extends PXScreen {
	IdRowUp: PXActionState;
	IdRowDown: PXActionState;
	DescriptionRowUp: PXActionState;
	DescriptionRowDown: PXActionState;
	DeleteItems: PXActionState;
	ViewMatrixItem: PXActionState;
	CreateUpdate: PXActionState;

   	@viewInfo({containerName: "Specify New ID"})
	ChangeIDDialog = createSingle(ChangeIDParam);
   	@viewInfo({containerName: "Template Item Summary"})
	Item = createSingle(InventoryItem);
	@viewInfo({containerName: "Template Item Configuration"})
	ItemSettings = createSingle(InventoryItem2);
   	@viewInfo({containerName: "Warehouse Defaults"})
	ItemCurySettings = createSingle(InventoryItemCurySettings);
   	@viewInfo({containerName: "Conversions"})
	itemunits = createCollection(INUnit);
   	@viewInfo({containerName: "Fulfillment"})
	Category = createCollection(INItemCategory);
   	@viewInfo({containerName: "Boxes"})
	Boxes = createCollection(INItemBox);
   	@viewInfo({containerName: "Vendors"})
	VendorItems = createCollection(VendorItems);
   	@viewInfo({containerName: "Attributes"})
	Answers = createCollection(Attributes);
   	@viewInfo({containerName: "Item Generation Rules"})
	IdGenerationRules = createCollection(INMatrixGenerationRule);
   	@viewInfo({containerName: "Description Generation Rules"})
	DescriptionGenerationRules = createCollection(DescriptionGenerationRules);
   	@viewInfo({containerName: "Item Creation"})
	Header = createSingle(EntryHeader);
   	@viewInfo({containerName: "Additional Attributes"})
	AdditionalAttributes = createCollection(AdditionalAttributes);
   	@viewInfo({containerName: "Item Creation"})
	Matrix = createCollection(EntryMatrix);
   	@viewInfo({containerName: "Create Matrix Items"})
	MatrixItemsForCreation = createCollection(MatrixItemsForCreation);
   	@viewInfo({containerName: "Excluded Fields"})
	FieldsExcludedFromUpdate = createCollection(INMatrixExcludedData);
   	@viewInfo({containerName: "Excluded Attributes"})
	AttributesExcludedFromUpdate = createCollection(AttributesExcludedFromUpdate);
   	@viewInfo({containerName: "Matrix Items"})
	MatrixItems = createCollection(MatrixItems);
   	@viewInfo({containerName: "Media URLs"})
	InventoryFileUrls = createCollection(BCInventoryFileUrls);
   	@viewInfo({containerName: "Update Effective Vendor Prices"})
	VendorInventory$UpdatePrice = createSingle(POVendorPriceUpdate);
   	@viewInfo({containerName: "ItemClass"})
	ItemClass = createSingle(INItemClass);
}