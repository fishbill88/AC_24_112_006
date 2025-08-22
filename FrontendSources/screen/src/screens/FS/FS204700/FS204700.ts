import {
	graphInfo,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	createSingle
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.MasterContractMaint', primaryView: 'MasterContracts' })
export class FS204700 extends PXScreen {
	MasterContracts = createSingle(FSMasterContract);
}

export class FSMasterContract extends PXView {
	MasterContractCD: PXFieldState;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
}
