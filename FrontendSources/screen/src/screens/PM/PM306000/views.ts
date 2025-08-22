import {
	PXFieldState,
	columnConfig,
	linkCommand,
	PXFieldOptions,
	gridConfig,
	PXView
} from "client-controls";

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class Items extends PXView {
	POLineCommitment__RelatedDocumentTypeExt: PXFieldState;
	@linkCommand("PMCommitment$RefNoteID$Link")
	RefNoteID: PXFieldState;
	@linkCommand("ViewVendor")
	POLineCommitment__VendorID: PXFieldState;
	POLineCommitment__VendorName: PXFieldState;
	@columnConfig({ hideViewLink: true })
	POOrder__OwnerID: PXFieldState;
	Type: PXFieldState;
	Status: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;
	@linkCommand("ViewProject")
	ProjectID: PXFieldState;
	PMProject__Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ProjectTaskID: PXFieldState;
	AccountGroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState;
	@linkCommand("ViewExternalCommitment")
	ExtRefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	ProjectCuryID: PXFieldState;
	OrigQty: PXFieldState;
	OrigAmount: PXFieldState;
	CommittedCOQty: PXFieldState;
	CommittedCOAmount: PXFieldState;
	Qty: PXFieldState;
	Amount: PXFieldState;
	OpenQty: PXFieldState;
	OpenAmount: PXFieldState;
	ReceivedQty: PXFieldState;
	InvoicedQty: PXFieldState;
	InvoicedAmount: PXFieldState;
	CommittedVarianceQty: PXFieldState;
	CommittedVarianceAmount: PXFieldState;
	@columnConfig({
		hideViewLink: true,
		visible: true
	})
	PMProject__OwnerID: PXFieldState;
}
