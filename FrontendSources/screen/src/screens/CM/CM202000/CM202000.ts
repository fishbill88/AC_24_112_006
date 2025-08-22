import { createSingle, PXScreen, graphInfo, PXView, PXFieldState, PXFieldOptions } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CM.CurrencyMaint', primaryView: 'CuryListRecords', hideFilesIndicator: true, hideNotesIndicator: true })
export class CM202000 extends PXScreen {

	CuryListRecords = createSingle(CurrencyList);
	CuryRecords = createSingle(Currency);

}

export class CurrencyList extends PXView {
	CuryID: PXFieldState;
	Description: PXFieldState;
	CurySymbol: PXFieldState;
	DecimalPlaces: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	IsFinancial: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Currency extends PXView {
	RoundingLimit: PXFieldState;
	RealGainAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	RealGainSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	RealLossAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	RealLossSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnrealizedGainAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnrealizedGainSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnrealizedLossAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnrealizedLossSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	APProvAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	APProvSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARProvAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARProvSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevalGainAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevalGainSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevalLossAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevalLossSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranslationGainAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranslationGainSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranslationLossAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranslationLossSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	RoundingGainAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	RoundingGainSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	RoundingLossAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	RoundingLossSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	UseARPreferencesSettings: PXFieldState<PXFieldOptions.CommitChanges>;
	ARInvoiceRounding: PXFieldState<PXFieldOptions.CommitChanges>;
	ARInvoicePrecision: PXFieldState;
	UseAPPreferencesSettings: PXFieldState<PXFieldOptions.CommitChanges>;
	APInvoiceRounding: PXFieldState<PXFieldOptions.CommitChanges>;
	APInvoicePrecision: PXFieldState;
}
