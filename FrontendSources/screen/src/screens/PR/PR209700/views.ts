import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig
} from "client-controls";

export class UnionLocal extends PXView {
	UnionID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class EarningRates extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	WageRate: PXFieldState<PXFieldOptions.CommitChanges>;
	EffectiveDate: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class DeductionsAndBenefitsPackage extends PXView {
	DeductionAndBenefitCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	LaborItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	PRDeductCode__Description: PXFieldState;
	PRDeductCode__ContribType: PXFieldState;
	PRDeductCode__DedCalcType: PXFieldState;
	DeductionAmount: PXFieldState;
	DeductionRate: PXFieldState;
	PRDeductCode__CntCalcType: PXFieldState;
	BenefitAmount: PXFieldState;
	BenefitRate: PXFieldState;
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

