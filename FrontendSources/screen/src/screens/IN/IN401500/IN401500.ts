import {createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, PXView, PXFieldState, gridConfig, GridColumnGeneration, GridPreset, PXFieldOptions } from 'client-controls';

@graphInfo({graphType: "PX.Objects.IN.Matrix.Graphs.MatrixItemsStatusInquiry", primaryView: "Header" })
export class IN401500 extends PXScreen {
	MatrixGridCellChanged: PXActionState;
	ViewAllocationDetails: PXActionState;

	@viewInfo({containerName: "Header"})
	Header = createSingle(EntryHeader);

	@viewInfo({containerName: "Additional Attributes"})
	AdditionalAttributes = createCollection(AdditionalAttributes);

	@viewInfo({containerName: "Create Matrix Items"})
	Matrix = createCollection(EntryMatrix);
}

// Views

export class EntryHeader extends PXView  {
	TemplateItemID : PXFieldState<PXFieldOptions.CommitChanges>;
	ColAttributeID : PXFieldState<PXFieldOptions.CommitChanges>;
	RowAttributeID : PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID : PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID : PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayPlanType : PXFieldState<PXFieldOptions.CommitChanges>;
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
	generateColumns: GridColumnGeneration.Recreate
})
export class EntryMatrix extends PXView  {
}
