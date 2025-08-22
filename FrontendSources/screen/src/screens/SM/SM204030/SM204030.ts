import {
	PXView,
	PXFieldState,
	linkCommand,
	columnConfig,
	graphInfo,
	PXScreen,
	createCollection,
	createSingle,
	gridConfig,
	PXActionState,
	PXFieldOptions,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CS.Email.EmailsSyncMaint",
	primaryView: "Filter",
})
export class SM204030 extends PXScreen {
	ViewEmployee: PXActionState;

	Filter = createSingle(EMailAccountSyncFilter);

	@linkCommand("Status")
	SelectedItems = createCollection(EMailSyncAccount);
}

class EMailAccountSyncFilter extends PXView {
	ServerID: PXFieldState;
	PolicyName: PXFieldState;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	fastFilterByAllFields: false,
	suppressNoteFiles: true,
})
class EMailSyncAccount extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	Status: PXActionState;

	Selected: PXFieldState;
	ServerID: PXFieldState;
	Address: PXFieldState;
	EmailAccountID: PXFieldState;
	@linkCommand("ViewEmployee") EmployeeID: PXFieldState;
	EmployeeCD: PXFieldState;
	PolicyName: PXFieldState;
}
