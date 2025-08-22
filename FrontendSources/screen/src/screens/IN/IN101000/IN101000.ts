import
{
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	PXActionState,
	gridConfig
} from "client-controls";

@graphInfo({graphType: 'PX.Objects.IN.INSetupMaint', primaryView: 'setup'})
export class IN101000 extends PXScreen {

	@viewInfo({containerName: "IN Preferences"})
	setup = createSingle(insetup);

	@viewInfo({containerName: "Restriction Groups"})
	Groups = createCollection(RestrictionGroups);

	@viewInfo({containerName: "Default Sources"})
	Notifications = createCollection(Notifications);

	@viewInfo({containerName: "Warehouse Management"})
	ScanSetup = createSingle(ScanSetup);

	@viewInfo({containerName: "GS1 Units"})
	gs1setup = createSingle(gs1setup);
}

export class insetup extends PXView {
	BatchNumberingID: PXFieldState;
	ReceiptNumberingID: PXFieldState;
	IssueNumberingID: PXFieldState;
	AdjustmentNumberingID: PXFieldState;
	KitAssemblyNumberingID: PXFieldState;
	PINumberingID: PXFieldState;
	ReplenishmentNumberingID: PXFieldState;
	ServiceItemNumberingID: PXFieldState;

	UseInventorySubItem: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplanBackOrders: PXFieldState;
	AllocateDocumentsOnHold: PXFieldState;

	ARClearingAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARClearingSubID: PXFieldState;
	TransitBranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	INTransitAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	INTransitSubID: PXFieldState;
	INProgressAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	INProgressSubID: PXFieldState;

	UpdateGL: PXFieldState;
	SummPost: PXFieldState;
	AutoPost: PXFieldState;

	HoldEntry: PXFieldState;
	RequireControlTotal: PXFieldState;
	AddByOneBarcode: PXFieldState;
	AutoAddLineBarcode: PXFieldState;
	DfltStkItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltNonStkItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;

	ReceiptReasonCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IssuesReasonCode: PXFieldState<PXFieldOptions.CommitChanges>;
	AdjustmentReasonCode: PXFieldState<PXFieldOptions.CommitChanges>;
	PIReasonCode: PXFieldState<PXFieldOptions.CommitChanges>;

	PIUseTags: PXFieldState;
	PILastTagNumber: PXFieldState;
	TurnoverPeriodsPerYear: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoReleasePIAdjustment: PXFieldState;

	IncludeSaleInTurnover: PXFieldState;
	IncludeProductionInTurnover: PXFieldState;
	IncludeAssemblyInTurnover: PXFieldState;
	IncludeIssueInTurnover: PXFieldState;
	IncludeTransferInTurnover: PXFieldState;
}

@gridConfig({
	syncPosition: true
})
export class RestrictionGroups extends PXView {
	ViewRestrictionGroup: PXActionState;
	@columnConfig({ hideViewLink: true }) GroupName: PXFieldState;
	Description: PXFieldState;
	Active: PXFieldState;
	@columnConfig({ hideViewLink: true }) GroupType: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true
})
export class Notifications extends PXView {
	NotificationCD: PXFieldState;
	@columnConfig({ hideViewLink: true }) NBranchID: PXFieldState;
	@columnConfig({ hideViewLink: true }) EMailAccountID: PXFieldState;
	@columnConfig({ hideViewLink: true }) DefaultPrinterID: PXFieldState;
	@columnConfig({ hideViewLink: true }) ReportID: PXFieldState;
	NotificationID: PXFieldState;
	Format: PXFieldState;
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ScanSetup extends PXView {
	ExplicitLineConfirmation: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultWarehouse: PXFieldState<PXFieldOptions.CommitChanges>;
	UseDefaultQtyInReceipt: PXFieldState<PXFieldOptions.CommitChanges>;
	UseDefaultReasonCodeInReceipt: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestLocationForEachItemInReceipt: PXFieldState<PXFieldOptions.CommitChanges>;
	UseDefaultQtyInIssue: PXFieldState<PXFieldOptions.CommitChanges>;
	UseDefaultReasonCodeInIssue: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestLocationForEachItemInIssue: PXFieldState<PXFieldOptions.CommitChanges>;
	UseDefaultQtyInTransfer: PXFieldState<PXFieldOptions.CommitChanges>;
	UseDefaultReasonCodeInTransfer: PXFieldState<PXFieldOptions.CommitChanges>;
	UseDefaultLotSerialNbrInTransfer: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestLocationForEachItemInTransfer: PXFieldState<PXFieldOptions.CommitChanges>;
	UseCartsForTransfers: PXFieldState<PXFieldOptions.CommitChanges>;
	UseDefaultQtyInCount: PXFieldState<PXFieldOptions.CommitChanges>;
}


export class gs1setup extends PXView {
	Kilogram: PXFieldState;
	Pound: PXFieldState;
	Ounce: PXFieldState;
	TroyOunce: PXFieldState;
	KilogramPerSqrMetre: PXFieldState;
	Metre: PXFieldState;
	Inch: PXFieldState;
	Foot: PXFieldState;
	Yard: PXFieldState;
	SqrMetre: PXFieldState;
	SqrInch: PXFieldState;
	SqrFoot: PXFieldState;
	SqrYard: PXFieldState;
	CubicMetre: PXFieldState;
	CubicInch: PXFieldState;
	CubicFoot: PXFieldState;
	CubicYard: PXFieldState;
	Litre: PXFieldState;
	Quart: PXFieldState;
	GallonUS: PXFieldState;
}
