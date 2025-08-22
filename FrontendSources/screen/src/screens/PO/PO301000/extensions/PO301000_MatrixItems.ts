import {
	PO301000
} from '../PO301000';

import {
	PXView,
	createCollection,
	PXFieldState,
	PXFieldOptions,
	featureInstalled,
	GridColumnGeneration,
	createSingle,
	viewInfo,
	gridConfig
} from 'client-controls';

export interface PO301000_MatrixItems extends PO301000 { }
@featureInstalled('PX.Objects.CS.FeaturesSet+MatrixItem')
export class PO301000_MatrixItems {
	Header = createSingle(AddMatrixItemHeader);

	@viewInfo({ containerName: "Add Matrix Item" })
	AdditionalAttributes = createCollection(MatrixAttributes);

	@viewInfo({ containerName: "Add Matrix Item - Matrix View" })
	Matrix = createCollection(EntryMatrix);

	@viewInfo({ containerName: "Add Matrix Item - Table View" })
	MatrixItems = createCollection(MatrixItems);
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
	generateColumns: GridColumnGeneration.Recreate
})
export class MatrixAttributes extends PXView {
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false,
	initNewRow: true
})
export class EntryMatrix extends PXView {
}

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
