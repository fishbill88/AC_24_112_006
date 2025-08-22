import {
	PXActionState,
	PXView,
	PXFieldState,
	gridConfig,
	ICurrencyInfo,
	PXFieldOptions,
	linkCommand,
	columnConfig,
} from "client-controls";

// Views

export class CRQuote extends PXView {
	Products$ImportAction: PXActionState;

	OpportunityID: PXFieldState;
	QuoteNbr: PXFieldState;
	IsPrimary: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	DocumentDate: PXFieldState;
	ExpirationDate: PXFieldState;
	Subject: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Multiline>;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualTotalEntry: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryLineDiscountTotal: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDiscTot: PXFieldState<PXFieldOptions.CommitChanges>;
	AMCuryEstimateTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryProductsAmount: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CRQuote2 extends PXView {
	AllowOverrideContactAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternalRef: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRegistrationID: PXFieldState;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternalTaxExemptionNumber: PXFieldState;
	AvalaraCustomerUsageType: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	CarrierID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipTermsID: PXFieldState;
	ShipZoneID: PXFieldState;
	FOBPointID: PXFieldState;
	Resedential: PXFieldState;
	SaturdayDelivery: PXFieldState;
	Insurance: PXFieldState;
	ShipComplete: PXFieldState;
}

export class ReasonApproveRejectFilter extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class AddressLookupFilter extends PXView {
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

export class AMEstimateItem extends PXView {
	EstimateID: PXFieldState<PXFieldOptions.CommitChanges>;
	AddExisting: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryCD: PXFieldState<PXFieldOptions.CommitChanges>;
	IsNonInventory: PXFieldState;
	SubItemID: PXFieldState;
	SiteID: PXFieldState;
	ItemDesc: PXFieldState<PXFieldOptions.CommitChanges>;
	EstimateClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class AMEstimateItem2 extends PXView {
	EstimateID: PXFieldState;
	RevisionID: PXFieldState;
	InventoryCD: PXFieldState;
	IsNonInventory: PXFieldState;
	SubItemID: PXFieldState;
	SiteID: PXFieldState;
	ItemDesc: PXFieldState;
	EstimateClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedLaborCost: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedLaborOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	VariableLaborCost: PXFieldState<PXFieldOptions.CommitChanges>;
	VariableLaborOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineCost: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	MaterialCost: PXFieldState<PXFieldOptions.CommitChanges>;
	MaterialOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	ToolCost: PXFieldState<PXFieldOptions.CommitChanges>;
	ToolOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedOverheadCost: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedOverheadOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	VariableOverheadCost: PXFieldState<PXFieldOptions.CommitChanges>;
	VariableOverheadOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtCostDisplay: PXFieldState;
	OrderQty: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	MarkupPct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	PriceOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtPrice: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState;
	CuryEffDate: PXFieldState;
	RecipRate: PXFieldState;
	SampleCuryRate: PXFieldState;
	SampleRecipRate: PXFieldState;
	CuryID: PXFieldState;
}
