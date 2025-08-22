import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection, createSingle, PXScreen, graphInfo,
	PXActionState, viewInfo, PXView, PXFieldState, PXFieldOptions } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.PO.LandedCostCodeMaint', primaryView: 'LandedCostCode' })
export class PO202000 extends PXScreen {
	@viewInfo({ containerName: 'Landed Cost Code' })
	LandedCostCode = createSingle(LandedCostCode);
}

export class LandedCostCode extends PXView {
	LandedCostCodeID: PXFieldState;
	Descr: PXFieldState;
	LCType: PXFieldState;
	AllocationMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsID: PXFieldState;
	ReasonCode: PXFieldState;
	LCAccrualAcct: PXFieldState<PXFieldOptions.CommitChanges>;
	LCAccrualSub: PXFieldState;
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	LCVarianceAcct: PXFieldState<PXFieldOptions.CommitChanges>;
	LCVarianceSub: PXFieldState;
}
