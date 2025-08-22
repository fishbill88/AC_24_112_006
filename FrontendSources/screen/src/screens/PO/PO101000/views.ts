import {
	PXView, PXFieldState, gridConfig, selectorSettings,
	PXFieldOptions, columnConfig, GridColumnShowHideMode } from 'client-controls';

// Views

export class POSetup extends PXView {
	StandardPONumberingID: PXFieldState;
	RegularPONumberingID: PXFieldState;
	ReceiptNumberingID: PXFieldState;
	LandedCostDocNumberingID: PXFieldState;
	RequireReceiptControlTotal: PXFieldState;
	RequireOrderControlTotal: PXFieldState;
	RequireBlanketControlTotal: PXFieldState;
	RequireDropShipControlTotal: PXFieldState;
	RequireProjectDropShipControlTotal: PXFieldState;
	RequireLandedCostsControlTotal: PXFieldState;
	PPVAllocationMode: PXFieldState<PXFieldOptions.CommitChanges>;
	PPVReasonCodeID: PXFieldState;
	APInvoiceValidation: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyLineDescrSO: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyLineNoteSO: PXFieldState;
	CopyLineNotesFromServiceOrder: PXFieldState;
	CopyLineAttachmentsFromServiceOrder: PXFieldState;
	CopyLineNotesToReceipt: PXFieldState;
	CopyLineFilesToReceipt: PXFieldState;
	AutoCreateInvoiceOnReceipt: PXFieldState;
	AutoCreateLCAP: PXFieldState;
	FreightExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	FreightExpenseSubID: PXFieldState;
	RCReturnReasonCodeID: PXFieldState;
	TaxReasonCodeID: PXFieldState;
	AutoReleaseIN: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoReleaseLCIN: PXFieldState;
	AutoReleaseAP: PXFieldState;
	HoldReceipts: PXFieldState;
	HoldLandedCosts: PXFieldState;
	AddServicesFromNormalPOtoPR: PXFieldState<PXFieldOptions.CommitChanges>;
	AddServicesFromDSPOtoPR: PXFieldState<PXFieldOptions.CommitChanges>;
	UpdateSubOnOwnerChange: PXFieldState;
	AutoAddLineReceiptBarcode: PXFieldState;
	ReceiptByOneBarcodeReceiptBarcode: PXFieldState;
	ReturnOrigCost: PXFieldState;
	ChangeCuryRateOnReceipt: PXFieldState;
	DefaultReceiptAssignmentMapID: PXFieldState;
	ShipDestType: PXFieldState;
	DefaultReceiptQty: PXFieldState;
	OrderRequestApproval: PXFieldState;
}

export class POSetupApproval extends PXView {
	OrderType: PXFieldState;
	AssignmentMapID: PXFieldState;
	AssignmentNotificationID: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	autoRepaint: ['Recipients']
})
export class Notifications extends PXView {
	Active: PXFieldState;
	NotificationCD: PXFieldState;
	@columnConfig({ hideViewLink: true })
	NBranchID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	EMailAccountID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DefaultPrinterID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ReportID: PXFieldState<PXFieldOptions.CommitChanges>;
	NotificationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Format: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true
})
export class Recipients extends PXView {
	Active: PXFieldState;
	ContactType: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactID: PXFieldState;
	Format: PXFieldState<PXFieldOptions.CommitChanges>;
	AddTo: PXFieldState;
}

export class POReceivePutAwaySetup extends PXView {
	ShowReceivingTab: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowPutAwayTab: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowReturningTab: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowScanLogTab: PXFieldState;
	UseDefaultQty: PXFieldState<PXFieldOptions.CommitChanges>;
	ExplicitLineConfirmation: PXFieldState;
	UseCartsForPutAway: PXFieldState;
	DefaultLotSerialNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultExpireDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SingleLocation: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultReceivingLocation: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestLocationForEachItemInReceive: PXFieldState;
	RequestLocationForEachItemInPutAway: PXFieldState;
	RequestLocationForEachItemInReturn: PXFieldState;
	PrintInventoryLabelsAutomatically: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryLabelsReportID: PXFieldState;
	PrintPurchaseReceiptAutomatically: PXFieldState<PXFieldOptions.CommitChanges>;
}
