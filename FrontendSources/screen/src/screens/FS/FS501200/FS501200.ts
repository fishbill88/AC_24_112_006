import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.ProcessServiceContracts', primaryView: 'Filter' })
export class FS501200 extends PXScreen {
	Filter = createSingle(ServiceContractFilter);
	ServiceContracts = createCollection(FSServiceContract);
}
export class ServiceContractFilter extends PXView {
	ActionType: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class FSServiceContract extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	BranchID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchLocationID: PXFieldState;
	CustomerID: PXFieldState;
	@columnConfig({ hideViewLink: true }) CustomerLocationID: PXFieldState;
	RefNbr: PXFieldState;
	CustomerContractNbr: PXFieldState;
	DocDesc: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	UpcomingStatus: PXFieldState;
	StatusEffectiveUntilDate: PXFieldState;
	ExpirationType: PXFieldState;
}
