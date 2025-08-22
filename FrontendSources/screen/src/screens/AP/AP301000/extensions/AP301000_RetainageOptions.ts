import { AP301000 } from '../AP301000';
import { PXView, PXFieldState, PXFieldOptions, createSingle } from 'client-controls';

export interface AP3010000_RetainageOptions extends AP301000 { }
export class AP3010000_RetainageOptions {

	// PX.Objects.AP.APInvoiceEntryRetainage

	ReleaseRetainageOptions = createSingle(RetainageOptions);

}

export class RetainageOptions extends PXView {

	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageUnreleasedAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceNbr: PXFieldState<PXFieldOptions.CommitChanges>;

}
