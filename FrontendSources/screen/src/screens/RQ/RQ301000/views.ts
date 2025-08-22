import {
	PXView,
	PXFieldState,
	gridConfig,
	ICurrencyInfo,
	PXFieldOptions,
	columnConfig,
	GridColumnShowHideMode,
	PXActionState
} from 'client-controls';

export class RQRequest extends PXView  {
	OrderNbr: PXFieldState;
	ReqClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	OrderDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Priority: PXFieldState;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	DepartmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	CuryEstExtCostTotal: PXFieldState<PXFieldOptions.Disabled>;
	OpenOrderQty: PXFieldState;
}

export class RQRequest2 extends PXView  {
	ShipDestType: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipToBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipToLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState;
	VendorHidden: PXFieldState;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	BudgetValidation: PXFieldState;
	WorkgroupID: PXFieldState;
	OwnerID: PXFieldState;
}

@gridConfig({
	initNewRow: true,
	syncPosition: true
})
export class RQRequestLine extends PXView  {
	ShowItems: PXActionState;
	viewDetails: PXActionState;

	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})
	InventoryID: PXFieldState;
	@columnConfig({hideViewLink: true})
	SubItemID: PXFieldState;
	Description: PXFieldState;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState;
	OrderQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEstUnitCost: PXFieldState;
	ManualPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEstExtCost: PXFieldState;
	@columnConfig({hideViewLink: true})
	ExpenseAcctID: PXFieldState;
	@columnConfig({hideViewLink: true})
	ExpenseSubID: PXFieldState;
	@columnConfig({hideViewLink: true})
	VendorID: PXFieldState;
	@columnConfig({hideViewLink: true})
	VendorLocationID: PXFieldState;
	VendorName: PXFieldState;
	VendorRefNbr: PXFieldState;
	VendorDescription: PXFieldState;
	AlternateID: PXFieldState;
	RequestedDate: PXFieldState;
	PromisedDate: PXFieldState;
	IssueStatus: PXFieldState;
	Cancelled: PXFieldState;
}

export class POContact extends PXView  {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState;
}

export class POAddress extends PXView  {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class POContact2 extends PXView  {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState;
}

export class POAddress2 extends PXView  {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false
})
export class EPApproval extends PXView  {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;
	Status: PXFieldState;
	Reason: PXFieldState;
	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false
})
export class RQBudget extends PXView  {
	@columnConfig({hideViewLink: true})
	ExpenseAcctID: PXFieldState;
	@columnConfig({hideViewLink: true})
	ExpenseSubID: PXFieldState;
	@columnConfig({hideViewLink: true})
	CuryID: PXFieldState;
	DocRequestAmt: PXFieldState;
	RequestAmt: PXFieldState;
	BudgetAmt: PXFieldState;
	UsageAmt: PXFieldState;
	AprovedAmt: PXFieldState;
	UnaprovedAmt: PXFieldState;
}

export class RecalcDiscountsParamFilter extends PXView  {
	RecalcTarget: PXFieldState<PXFieldOptions.Disabled>;
	RecalcUnitPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false
})
export class RQRequisitionContent extends PXView  {
	viewRequisition: PXActionState;

	RQRequisition__ReqNbr: PXFieldState;
	RQRequisition__Priority: PXFieldState;
	RQRequisition__OrderDate: PXFieldState;
	RQRequisition__Status: PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true})
	RQRequisitionLineReceived__InventoryID: PXFieldState;
	@columnConfig({allowUpdate: false, hideViewLink: true})
	RQRequisitionLineReceived__UOM: PXFieldState;
	@columnConfig({allowUpdate: false})
	RQRequisitionLineReceived__Description: PXFieldState;
	ItemQty: PXFieldState;
	RQRequisitionLineReceived__OrderQty: PXFieldState;
	RQRequisitionLineReceived__POOrderQty: PXFieldState;
	RQRequisitionLineReceived__POReceivedQty: PXFieldState;
}

export class ReasonApproveRejectFilter extends PXView  {
	Reason: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReassignApprovalFilter extends PXView  {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class AddressLookupFilter extends PXView  {
	SearchAddress: PXFieldState;
	ViewName: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	AddressLine3: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
}