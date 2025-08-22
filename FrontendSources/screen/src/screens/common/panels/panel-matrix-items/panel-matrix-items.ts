import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	GridColumnGeneration,
	createSingle,
	viewInfo,
	createCollection,
} from "client-controls";

export abstract class PanelMatrixItemsBase {
	// panel-matrix-items - begin
	Header = createSingle(AddMatrixItemHeader);

	@viewInfo({ containerName: "Add Matrix Item" })
	AdditionalAttributes = createCollection(MatrixAttributes);

	@viewInfo({ containerName: "Add Matrix Item - Matrix View" })
	Matrix = createCollection(EntryMatrix);

	@viewInfo({ containerName: "Add Matrix Item - Table View" })
	MatrixItems = createCollection(MatrixItems);
	// panel-matrix-items - end
}

export class AddMatrixItemHeader extends PXView {
	TemplateItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	ColAttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	RowAttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowAvailable: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false,
	generateColumns: GridColumnGeneration.Recreate,
})
export class MatrixAttributes extends PXView {}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false,
	initNewRow: true,
})
export class EntryMatrix extends PXView {}

export class MatrixItems extends PXView {
	AttributeValue0: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState;
	Qty: PXFieldState;
	InventoryCD: PXFieldState;
	Descr: PXFieldState;
	New: PXFieldState;
	StkItem: PXFieldState;
	BasePrice: PXFieldState;
	ItemClassID: PXFieldState;
	TaxCategoryID: PXFieldState;
}
