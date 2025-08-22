import { graphInfo, PXFieldState, PXScreen, PXView, PXFieldOptions, viewInfo, createSingle, createCollection, columnConfig} from "client-controls";

@graphInfo({graphType: 'PX.Api.SYSubstitutionMaint', primaryView: 'Substitution'})
export class SM206026 extends PXScreen {
	Substitution = createSingle(SYSubstitution);
	SubstitutionValues = createCollection (SYSubstitutionValues);
}

export class SYSubstitution extends PXView {
	SubstitutionID: PXFieldState;
	TableName: PXFieldState<PXFieldOptions.CommitChanges>;
	FieldName: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class SYSubstitutionValues extends PXView {
	OriginalValue: PXFieldState;
	@columnConfig({fullState: true}) SubstitutedValue: PXFieldState;
}
