import {
	graphInfo,
	createSingle,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.LicenseMaint', primaryView: 'LicenseRecords' })
export class FS201000 extends PXScreen {
	LicenseRecords = createSingle(FSLicense);
}

export class FSLicense extends PXView {
	RefNbr: PXFieldState;
	LicenseTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	IssueDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	NeverExpires: PXFieldState<PXFieldOptions.CommitChanges>;
}
