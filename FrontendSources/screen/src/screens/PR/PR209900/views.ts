import {
	PXView,
	PXFieldState,
	gridConfig,
	PXFieldOptions,
	columnConfig
} from 'client-controls';

export class CertifiedProjectFilter extends PXView {
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({syncPosition: true, adjustPageSize: true}) 
export class PMLaborCostRate extends PXView {
	@columnConfig({allowUpdate: false})
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	Description: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TaskID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	WageRate: PXFieldState;
	@columnConfig({allowUpdate: false})
	EffectiveDate: PXFieldState;
}

@gridConfig({syncPosition: true, adjustPageSize: true}) 
export class PRDeductionAndBenefitProjectPackage extends PXView {
	@columnConfig({ allowUpdate: false })
	DeductionAndBenefitCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	LaborItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	PRDeductCode__Description: PXFieldState;
	@columnConfig({ allowUpdate: false })
	PRDeductCode__ContribType: PXFieldState;
	@columnConfig({ allowUpdate: false })
	PRDeductCode__DedCalcType: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DeductionAmount: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DeductionRate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	PRDeductCode__CntCalcType: PXFieldState;
	@columnConfig({ allowUpdate: false })
	BenefitAmount: PXFieldState;
	@columnConfig({ allowUpdate: false })
	BenefitRate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Contract extends PXView {
	Status: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState<PXFieldOptions.Disabled>;
	FileEmptyCertifiedReport: PXFieldState;
	WageAbovePrevailingAnnualizationException: PXFieldState;
	ApplyOTMultiplierToFringeRate: PXFieldState;
	BenefitCodeReceivingFringeRate: PXFieldState;
}

@gridConfig({syncPosition: true, adjustPageSize: true}) 
export class PRProjectFringeBenefitRate extends PXView {
	@columnConfig({ allowUpdate: false })
	LaborItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	InventoryItem__Descr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	Rate: PXFieldState;
	@columnConfig({allowUpdate: false})
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({syncPosition: true, adjustPageSize: true}) 
export class PRProjectFringeBenefitRateReducingDeduct extends PXView {
	@columnConfig({ allowUpdate: false })
	IsActive: PXFieldState;
	@columnConfig({allowUpdate: false})
	DeductCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	DeductCodeID_Description: PXFieldState;
	@columnConfig({allowUpdate: false})
	PRDeductCode__CertifiedReportType: PXFieldState;
	@columnConfig({allowUpdate: false})
	AnnualizationException: PXFieldState;
}
