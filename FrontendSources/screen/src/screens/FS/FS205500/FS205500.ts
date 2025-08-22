import {
	graphInfo,
	gridConfig,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.SrvManagementEmployeeMaint', primaryView: 'SrvManagementStaffRecords' })
export class FS205500 extends PXScreen {
	SrvManagementStaffRecords = createCollection(BAccountStaffMember);
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar"
})
export class BAccountStaffMember extends PXView {
	AcctCD: PXFieldState;
	AcctName: PXFieldState;
	Type: PXFieldState;
	BAccountStaffMember__ParentBAccountID: PXFieldState;
	Contact__EMail: PXFieldState;
	Contact__Phone1: PXFieldState;
}
