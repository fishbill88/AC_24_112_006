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
} from 'client-controls';

@graphInfo({
	graphType: 'PX.Objects.CR.ValidateCRDocumentAddressProcess',
	primaryView: 'Filter'
})
export class CR508000 extends PXScreen {
	ViewDocument: PXActionState;

	Filter = createSingle(Filter);
	DocumentAddresses = createCollection(DocumentAddresses);
}


export class Filter extends PXView {
	Country: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({nullText: 'All'})DocumentType: PXFieldState<PXFieldOptions.CommitChanges>;
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

	@columnConfig({
		allowCheckAll: true,
		allowSort: false
	})
	Selected: PXFieldState;
	@linkCommand("ViewDocument")
	DocumentNbr: PXFieldState;
	DocumentType: PXFieldState;
	Status: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState;
	CountryID: PXFieldState;
}

