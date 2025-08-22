import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnType,
	GridColumnShowHideMode,
	linkCommand,
	columnConfig,
	ICurrencyInfo,
	gridConfig
} from "client-controls";

export class Filter extends PXView {
	Country: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		nullText: 'All'
	})
	DocumentType: PXFieldState<PXFieldOptions.CommitChanges>;
	IsOverride: PXFieldState;
}

@gridConfig({
	mergeToolbarWith: 'ScreenToolbar',
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class DocumentAddresses extends PXView {
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

