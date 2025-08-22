import {
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXActionState,
	PXFieldState,
	PXFieldOptions,
} from "client-controls";

@graphInfo({
	graphType: "PX.SM.PreferencesGeneralMaint",
	primaryView: "Prefs",
	hideFilesIndicator: true,
	hideNotesIndicator: true,
})
export class SM200505 extends PXScreen {
	ResetColors: PXActionState;

	@viewInfo({ containerName: "General Settings" })
	Prefs = createSingle(PreferencesGeneral);
	@viewInfo({ containerName: "Global Settings" })
	PrefsGlobal = createSingle(PreferencesGlobal);
}

export class PreferencesGeneral extends PXView {
	HomePage: PXFieldState;
	HelpPage: PXFieldState<PXFieldOptions.CommitChanges>;
	UseMLSearch: PXFieldState;
	AddressLookupPluginID: PXFieldState;
	MapViewer: PXFieldState;
	TimeZone: PXFieldState;
	Theme: PXFieldState<PXFieldOptions.CommitChanges>;
	PrimaryColor: PXFieldState<PXFieldOptions.CommitChanges>;
	BackgroundColor: PXFieldState<PXFieldOptions.CommitChanges>;
	GetLinkTemplate: PXFieldState<PXFieldOptions.CommitChanges>;
	PortalExternalAccessLink: PXFieldState;
	PersonNameFormat: PXFieldState<PXFieldOptions.CommitChanges>;
	GridFastFilterCondition: PXFieldState;
	GridFastFilterMaxLength: PXFieldState;
	GridActionsText: PXFieldState;
	DeletingMLEventsMode: PXFieldState<PXFieldOptions.CommitChanges>;
	MLEventsRetentionAge: PXFieldState;
	SpellCheck: PXFieldState;
	Border: PXFieldState;
	BorderColor: PXFieldState;
	HiddenSkip: PXFieldState;
	ApplyToEmptyCells: PXFieldState;
	HeaderFont: PXFieldState;
	HeaderFontSize: PXFieldState;
	HeaderFontColor: PXFieldState;
	HeaderFontType: PXFieldState;
	HeaderFillColor: PXFieldState;
	BodyFont: PXFieldState;
	BodyFontSize: PXFieldState;
	BodyFontColor: PXFieldState;
	BodyFontType: PXFieldState;
	BodyFillColor: PXFieldState;
}

export class PreferencesGlobal extends PXView {
	EnableTelemetry: PXFieldState<PXFieldOptions.CommitChanges>;
}
