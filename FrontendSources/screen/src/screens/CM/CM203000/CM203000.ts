import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, GridColumnShowHideMode } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CM.TranslationDefinitionMaint', primaryView: 'TranslDefRecords', })
export class CM203000 extends PXScreen {

	TranslDefRecords = createSingle(TranslDef);
	TranslDefDetailsRecords = createCollection(TranslDefDet);

}

export class TranslDef extends PXView {
	TranslDefId: PXFieldState;
	SourceLedgerId: PXFieldState<PXFieldOptions.CommitChanges>;
	DestLedgerId: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceCuryID: PXFieldState<PXFieldOptions.Disabled>;
	DestCuryID: PXFieldState<PXFieldOptions.Disabled>;
}

export class TranslDefDet extends PXView {
	AccountIdFrom: PXFieldState;
	SubIdFrom: PXFieldState;
	AccountIdTo: PXFieldState;
	SubIdTo: PXFieldState;
	CalcMode: PXFieldState;
	RateTypeId: PXFieldState;
	AccountIdFrom_Account_description: PXFieldState;
	AccountIdTo_Account_description: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	TranslDefId: PXFieldState<PXFieldOptions.Hidden>;
}
