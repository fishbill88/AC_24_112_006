import {
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Locations extends PXView {
	LocationCD: PXFieldState;
	Description: PXFieldState;
	IsActive: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Address extends PXView {
	AddressID: PXFieldState;
	AddressLine1: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine2: PXFieldState<PXFieldOptions.CommitChanges>;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	IsDisabled: PXFieldState<PXFieldOptions.Disabled>;
}

