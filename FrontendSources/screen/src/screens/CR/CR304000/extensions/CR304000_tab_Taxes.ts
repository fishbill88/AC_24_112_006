import { CR304000 } from "../CR304000";
import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createCollection,
	gridConfig,
	viewInfo,
	PXActionState,
	columnConfig,
	GridPreset,
} from "client-controls";

export interface CR304000_Taxes extends CR304000 {}
export class CR304000_Taxes {
	@viewInfo({ containerName: "Taxes" })
	Taxes = createCollection(CRTaxTran);
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	allowDelete: true,
	fastFilterByAllFields: false,
})
export class CRTaxTran extends PXView {
	@columnConfig({ allowUpdate: false }) TaxID: PXFieldState;
	@columnConfig({ allowUpdate: false }) TaxRate: PXFieldState;
	CuryTaxableAmt: PXFieldState;
	CuryTaxAmt: PXFieldState;
	CuryExemptedAmt: PXFieldState;
	Tax__TaxType: PXFieldState;
	Tax__PendingTax: PXFieldState;
	Tax__ReverseTax: PXFieldState;
	Tax__ExemptTax: PXFieldState;
	Tax__StatisticalTax: PXFieldState;
}
