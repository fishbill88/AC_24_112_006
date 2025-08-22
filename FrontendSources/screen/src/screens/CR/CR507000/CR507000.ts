import {
	PXView,
	PXFieldState,
	graphInfo,
	PXScreen,
	createCollection,
	createSingle,
	gridConfig,
	PXFieldOptions,
	columnConfig,
	GridFilterBarVisibility,
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.CR.CRCaseReleaseProcess', primaryView: 'Filter' })
export class CR507000 extends PXScreen {
	Filter = createSingle(CaseFilter);
	Items = createCollection(CRCase);
}

export class CaseFilter extends PXView {
	CaseClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ContractID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	adjustPageSize: true,
	showFilterBar: GridFilterBarVisibility.False,
	allowUpdate: false,
	suppressNoteFiles: true,
	mergeToolbarWith: 'ScreenToolbar',
})
export class CRCase extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	CaseCD: PXFieldState;
	Subject: PXFieldState;
	@columnConfig({ hideViewLink: true }) CaseClassID: PXFieldState;
	Customer__AcctName: PXFieldState;
	@columnConfig({ hideViewLink: true }) Customer__ClassID: PXFieldState;
	@columnConfig({ hideViewLink: true }) ContractID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	TimeBillable: PXFieldState;
	OverTimeBillable: PXFieldState;
	CreatedDateTime: PXFieldState;
	ResolutionDate: PXFieldState;
}
