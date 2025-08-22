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
	viewInfo,
	GridColumnShowHideMode,
	linkCommand,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.GDPR.GDPREraseProcess",
	primaryView: "Filter",
	bpEventsIndicator: false,
	udfTypeField: "",
})
export class GD102020 extends PXScreen {
	@viewInfo({ containerName: "Operation" })
	Filter = createSingle(ObfuscateType);
	SelectedItems = createCollection(ObfuscateEntity);
}

export class ObfuscateType extends PXView {
	Search: PXFieldState<PXFieldOptions.CommitChanges>;
	MasterEntity: PXFieldState<PXFieldOptions.CommitChanges>;
	NoConsent: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentExpired: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	allowUpdate: false,
	fastFilterByAllFields: false,
})
export class ObfuscateEntity extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({
		allowCheckAll: true,
		allowShowHide: GridColumnShowHideMode.False,
		width: 35,
	})
	Selected: PXFieldState;

	Deleted: PXFieldState;
	@linkCommand("OpenContact") ContactID: PXFieldState;
	ContactType: PXFieldState;
	AcctCD: PXFieldState;
	DisplayName: PXFieldState;
	MidName: PXFieldState;
	LastName: PXFieldState;
	Salutation: PXFieldState;
	FullName: PXFieldState;
	EMail: PXFieldState;
	WebSite: PXFieldState;
	Fax: PXFieldState;
	Phone1: PXFieldState;
	Phone2: PXFieldState;
	Phone3: PXFieldState;
}
