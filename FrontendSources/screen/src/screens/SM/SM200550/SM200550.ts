import { graphInfo, PXScreen, PXView, PXFieldState, PXFieldOptions, createSingle, createCollection, gridConfig, columnConfig } from "client-controls";

@graphInfo({ graphType: 'PX.SM.LocaleMaintenance', primaryView: 'Locales', bpEventsIndicator: false })
export class SM200550 extends PXScreen {
	Locales = createCollection(Locales);
	AlternativeDetails = createCollection(AlternativeDetails);

	AlternativeHeader = createSingle(AlternativeHeader);
	Formats = createSingle(Formats);
}

@gridConfig({mergeToolbarWith: 'ScreenToolbar', autoAdjustColumns: true})
export class Locales extends PXView {
	@columnConfig({hideViewLink: true})
	LocaleName: PXFieldState;
	CultureReadableName: PXFieldState;
	TranslatedName: PXFieldState;
	Description: PXFieldState;
	// eslint-disable-next-line id-denylist
	Number: PXFieldState;
	IsActive: PXFieldState;
	ShowValidationWarnings: PXFieldState;
	IsDefault: PXFieldState;
	IsAlternative: PXFieldState;
}

export class AlternativeHeader extends PXView {
	DefaultLanguageName: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({syncPosition: true, allowInsert: false, allowDelete: false, allowUpdate: false})
export class AlternativeDetails extends PXView {
	@columnConfig({allowSort: false, allowCheckAll: true})
	IsAlternative: PXFieldState;
	LanguageName: PXFieldState;
	NativeName: PXFieldState;
}

export class Formats extends PXView {
	TemplateLocale: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTimePattern: PXFieldState;
	TimeShortPattern: PXFieldState;
	TimeLongPattern: PXFieldState;
	DateShortPattern: PXFieldState;
	DateLongPattern: PXFieldState;
	AMDesignator: PXFieldState;
	PMDesignator: PXFieldState;
	NumberDecimalSeporator: PXFieldState<PXFieldOptions.CommitChanges>;
	NumberGroupSeparator: PXFieldState<PXFieldOptions.CommitChanges>;
}

