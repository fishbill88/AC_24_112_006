import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, PXFieldOptions } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.FA.DepreciationTableMethodMaint', primaryView: 'Method', })
export class FA202600 extends PXScreen {

	@viewInfo({ containerName: 'Depreciation Method Summary' })
	Method = createSingle(FADepreciationMethod);

	@viewInfo({ containerName: 'Depreciation Method Details' })
	details = createCollection(FADepreciationMethodLines);

}

export class FADepreciationMethod extends PXView {

	MethodCD: PXFieldState;
	RecordType: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	UsefulLife: PXFieldState<PXFieldOptions.CommitChanges>;
	RecoveryPeriod: PXFieldState;
	Description: PXFieldState;
	AveragingConvention: PXFieldState<PXFieldOptions.CommitChanges>;
	AveragingConvPeriod: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayTotalPercents: PXFieldState<PXFieldOptions.Disabled>;
	Source: PXFieldState<PXFieldOptions.Disabled>;

}

export class FADepreciationMethodLines extends PXView {

	Year: PXFieldState;
	DisplayRatioPerYear: PXFieldState<PXFieldOptions.CommitChanges>;

}
