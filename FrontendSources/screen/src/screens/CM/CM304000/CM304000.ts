import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXPageLoadBehavior,
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CM.TranslationHistoryMaint",
	primaryView: "TranslHistRecords",
	pageLoadBehavior: PXPageLoadBehavior.GoLastRecord,
	showUDFIndicator: true,
})
export class CM304000 extends PXScreen {
	ViewBatch: PXActionState;

	TranslHistRecords = createSingle(TranslationHistory);
	TranslHistDetRecords = createCollection(TranslationHistoryDetails);
}

export class TranslationHistory extends PXView {
	ReferenceNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	DateEntered: PXFieldState<PXFieldOptions.Disabled>;
	CuryEffDate: PXFieldState<PXFieldOptions.Disabled>;
	FinPeriodID: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState<PXFieldOptions.Disabled>;
	TranslDefId: PXFieldState<PXFieldOptions.Disabled>;
	BranchID: PXFieldState<PXFieldOptions.Disabled>;
	LedgerID: PXFieldState<PXFieldOptions.Disabled>;
	DestCuryID: PXFieldState<PXFieldOptions.Disabled>;
	BatchNbr: PXFieldState<PXFieldOptions.Disabled>;
	DebitTot: PXFieldState<PXFieldOptions.Disabled>;
	CreditTot: PXFieldState<PXFieldOptions.Disabled>;
	ControlTot: PXFieldState;
}

export class TranslationHistoryDetails extends PXView {
	BranchID: PXFieldState;
	AccountID: PXFieldState;
	AccountID_Account_description: PXFieldState;
	SubID: PXFieldState;
	CalcMode: PXFieldState;
	SourceAmt: PXFieldState;
	TranslatedAmt: PXFieldState;
	OrigTranslatedAmt: PXFieldState;
	RateTypeID: PXFieldState;
	CuryRate: PXFieldState;
	DebitAmt: PXFieldState;
	CreditAmt: PXFieldState;
}
