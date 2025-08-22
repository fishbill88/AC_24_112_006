import {
	PXView,
	PXFieldState,
	commitChanges,
	graphInfo,
	PXScreen,
	createCollection,
	createSingle,
	gridConfig,
	linkCommand,
	PXFieldOptions,
	GridFilterBarVisibility,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: 'PX.Objects.CR.CRValidationProcess',
	primaryView: 'Filter'
})
export class CR503430 extends PXScreen {
	Filter = createSingle(Filter);
	Contacts = createCollection(Contacts);
}

export class Filter extends PXView {
	@commitChanges ValidationType: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	adjustPageSize: true,
	showFilterBar: GridFilterBarVisibility.False,
	suppressNoteFiles: true,
})
export class Contacts extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;
	AggregatedType: PXFieldState;
	@linkCommand("Contacts_BAccount_ViewDetails") BAccountID: PXFieldState;
	AcctName: PXFieldState;
	@linkCommand("Contacts_Contact_ViewDetails") DisplayName: PXFieldState;
	AggregatedStatus: PXFieldState;
	DuplicateStatus: PXFieldState;
}
