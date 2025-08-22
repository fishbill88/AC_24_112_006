import {
	PXScreen,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	headerDescription,
} from 'client-controls';

export class EstimateClassRecords extends PXView {
	EstimateClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	@headerDescription Description: PXFieldState;
	ItemClassID: PXFieldState;
	TaxCategoryID: PXFieldState;
	EngineerID: PXFieldState;
	LeadTime: PXFieldState;
	OrderQty: PXFieldState;
	LaborMarkupPct: PXFieldState;
	MachineMarkupPct: PXFieldState;
	MaterialMarkupPct: PXFieldState;
	ToolMarkupPct: PXFieldState;
	OverheadMarkupPct: PXFieldState;
	SubcontractMarkupPct: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.EstimateClassMaint', primaryView: 'EstimateClassRecords' })
export class AM206000 extends PXScreen {
	EstimateClassRecords = createSingle(EstimateClassRecords);
}
