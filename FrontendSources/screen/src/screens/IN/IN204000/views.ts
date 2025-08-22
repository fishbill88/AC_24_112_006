import {
	PXView,
	PXFieldState,
	gridConfig,
	PXFieldOptions,
	columnConfig,
	PXActionState
} from 'client-controls';

export class ChangeIDParam extends PXView {
	CD: PXFieldState;
}

export class INSite extends PXView {
	SiteCD: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplenishmentClassID: PXFieldState;
	Active: PXFieldState;
	Descr: PXFieldState;
	LocationValid: PXFieldState;
	AvgDefaultCost: PXFieldState;
	FIFODefaultCost: PXFieldState;
}

export class INSiteAccounts extends PXView {
	ReceiptLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReturnLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	DropShipLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	UseItemDefaultLocationForPicking: PXFieldState;
	NonStockPickingLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideInvtAccSub: PXFieldState;
	InvtAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvtSubID: PXFieldState;
	ReasonCodeSubID: PXFieldState;
	SalesAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesSubID: PXFieldState;
	COGSAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	COGSSubID: PXFieldState;
	StdCstVarAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	StdCstVarSubID: PXFieldState;
	StdCstRevAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	StdCstRevSubID: PXFieldState;
	POAccrualAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	POAccrualSubID: PXFieldState;
	PPVAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PPVSubID: PXFieldState;
	LCVarianceAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	LCVarianceSubID: PXFieldState;
	BuildingID: PXFieldState;
	CarrierFacility: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	pageSize: 50
})
export class INLocation extends PXView {
	LocationCD: PXFieldState;
	Descr: PXFieldState;
	Active: PXFieldState;
	IsSorting: PXFieldState;
	InclQtyAvail: PXFieldState;
	IsCosted: PXFieldState;
	SalesValid: PXFieldState;
	ReceiptsValid: PXFieldState;
	TransfersValid: PXFieldState;
	AssemblyValid: PXFieldState;
	ProductionValid: PXFieldState;
	PickPriority: PXFieldState<PXFieldOptions.CommitChanges>;
	PathPriority: PXFieldState<PXFieldOptions.CommitChanges>;
	PrimaryItemValid: PXFieldState;
	PrimaryItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	PrimaryItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	TaskID: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true
})
export class INCart extends PXView {
	ViewTotesInCart: PXActionState;

	@columnConfig({ hideViewLink: true })
	CartCD: PXFieldState;
	Descr: PXFieldState;
	AssignedNbrOfTotes: PXFieldState;
	Active: PXFieldState;
}

@gridConfig({
	adjustPageSize: true
})
export class INTote extends PXView {
	@columnConfig({ hideViewLink: true })
	ToteCD: PXFieldState;
	Descr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	AssignedCartID: PXFieldState;
	Active: PXFieldState;
}

export class Contact extends PXView {
	FullName: PXFieldState;
	Attention: PXFieldState;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState;
	Phone2: PXFieldState;
	Fax: PXFieldState;
}

export class Address extends PXView {
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
	allowImport: false,
	adjustPageSize: true
})
export class INToteCart extends PXView {
	@columnConfig({ hideViewLink: true })
	ToteCD: PXFieldState;
	Descr: PXFieldState;
	Active: PXFieldState;
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