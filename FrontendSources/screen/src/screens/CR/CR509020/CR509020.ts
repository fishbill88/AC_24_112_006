import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	gridConfig,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.CRValidateAddressProcess",
	primaryView: "Filter",
})
export class CR509020 extends PXScreen {
	viewDetails: PXActionState;

	Filter = createSingle(Filter);
	AddressList = createCollection(DocumentAddresses);
}

export class Filter extends PXView {
	Country: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ nullText: "All" })
	BAccountType: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ nullText: "All" })
	BAccountStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	IsOverride: PXFieldState;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	wrapToolbar: true,
})
export class DocumentAddresses extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ allowCheckAll: true, allowSort: false })
	Selected: PXFieldState;
	AcctCD: PXFieldState;
	AcctName: PXFieldState;
	Type: PXFieldState;
	Status: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState;
	CountryID: PXFieldState;
}
