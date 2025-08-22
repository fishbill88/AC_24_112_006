import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	columnConfig,
    gridConfig,
    GridPagerMode
} from "client-controls";

export class Filter extends PXView {
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, autoRepaint: ["WorkCompensationRates"] })
export class WorkCompensationCodes extends PXView {
	ViewMaximumInsurableWages: PXActionState;

	IsActive: PXFieldState;
	WorkCodeID: PXFieldState;
	Description: PXFieldState;
}

@gridConfig(
	{
		syncPosition: true,
		adjustPageSize: false,
		pagerMode: GridPagerMode.InfiniteScroll,
		initNewRow: true,
		autoRepaint: ["ProjectTaskSources", "LaborItemSources", "CostCodeRanges"]
	})
export class WorkCompensationRates extends PXView {
	IsActive: PXFieldState;
	DeductCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeductionCalcType: PXFieldState;
	PRDeductCode__CntCalcType: PXFieldState;
	@columnConfig({ width: 110 })
	PRDeductCode__State: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeductionRate: PXFieldState;
	Rate: PXFieldState;
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, adjustPageSize: false, pagerMode: GridPagerMode.InfiniteScroll, initNewRow: true })
export class ProjectTaskSources extends PXView {
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState;
}

@gridConfig({ syncPosition: true, adjustPageSize: false, pagerMode: GridPagerMode.InfiniteScroll, initNewRow: true })
export class LaborItemSources extends PXView {
	LaborItemID: PXFieldState;
}

@gridConfig({ syncPosition: true, adjustPageSize: false, pagerMode: GridPagerMode.InfiniteScroll, initNewRow: true })
export class PMWorkCodeCostCodeRange extends PXView {
	CostCodeFrom: PXFieldState;
	CostCodeTo: PXFieldState;
}

@gridConfig({ adjustPageSize: true, syncPosition: true, allowInsert: true, allowDelete: true, initNewRow: true })
export class MaximumInsurableWages extends PXView {
	DeductCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	State__StateID: PXFieldState;
	MaximumInsurableWage: PXFieldState;
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
}
