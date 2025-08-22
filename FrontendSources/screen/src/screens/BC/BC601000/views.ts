import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	linkCommand,
	gridConfig
} from "client-controls";

export class Filter extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	MasterEntity: PXFieldState<PXFieldOptions.CommitChanges>;
	DocumentDateWithinXDays: PXFieldState<PXFieldOptions.CommitChanges>;
	Days: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
	autoAdjustColumns: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class SelectedItems extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState;
	@linkCommand("OpenEntity")
	Reference: PXFieldState;
	MasterEntityType: PXFieldState;
	Pseudonymized: PXFieldState;
	ShipToCompanyName: PXFieldState;
	ShipToAttention: PXFieldState;
	ShipToEmail: PXFieldState;
	ShipToPhone1: PXFieldState;
	ShipToAddressLine1: PXFieldState;
	ShipToAddressLine2: PXFieldState;
	ShipToCity: PXFieldState;
	ShipToState: PXFieldState;
	ShipToCountry: PXFieldState;
	ShipToPostalCode: PXFieldState;
	BillToCompanyName: PXFieldState;
	BillToAttention: PXFieldState;
	BillToEmail: PXFieldState;
	BillToPhone1: PXFieldState;
	BillToAddressLine1: PXFieldState;
	BillToAddressLine2: PXFieldState;
	BillToCity: PXFieldState;
	BillToState: PXFieldState;
	BillToCountry: PXFieldState;
	BillToPostalCode: PXFieldState;
}
