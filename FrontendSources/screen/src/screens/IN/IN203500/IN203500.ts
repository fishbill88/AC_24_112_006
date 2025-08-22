import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, PXView, PXFieldState, PXFieldOptions, gridConfig, columnConfig, GridColumnGeneration, GridPreset } from "client-controls";

@graphInfo({graphType: "PX.Objects.IN.Matrix.Graphs.CreateMatrixItems", primaryView: "Header" })
export class IN203500 extends PXScreen {
	@viewInfo({containerName: "Header"})
	Header = createSingle(EntryHeader);

	@viewInfo({containerName: "Additional Attributes"})
	AdditionalAttributes = createCollection(AdditionalAttributes);

	@viewInfo({containerName: "Matrix"})
	Matrix = createCollection(EntryMatrix);

   	@viewInfo({containerName: "Create Matrix Items"})
	MatrixItemsForCreation = createCollection(InventoryItem);
}

// Views

export class EntryHeader extends PXView  {
	CreateUpdate: PXActionState;

	TemplateItemID : PXFieldState<PXFieldOptions.CommitChanges>;
	ColAttributeID : PXFieldState<PXFieldOptions.CommitChanges>;
	RowAttributeID : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Attributes,
	syncPosition: true,
	generateColumns: GridColumnGeneration.Recreate
})
export class AdditionalAttributes extends PXView  {
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	generateColumns: GridColumnGeneration.AppendDynamic
})
export class EntryMatrix extends PXView  {
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	generateColumns: GridColumnGeneration.Recreate
})
export class InventoryItem extends PXView  {
	@columnConfig({allowCheckAll: true}) Selected : PXFieldState;
	InventoryCD : PXFieldState;
	Descr : PXFieldState;
	StkItem : PXFieldState;
	@columnConfig({hideViewLink: true}) ItemClassID : PXFieldState;
	ItemType : PXFieldState;
	ValMethod : PXFieldState;
	@columnConfig({hideViewLink: true}) LotSerClassID : PXFieldState;
	@columnConfig({hideViewLink: true}) DfltSiteID : PXFieldState;
	@columnConfig({hideViewLink: true}) TaxCategoryID : PXFieldState;
}
