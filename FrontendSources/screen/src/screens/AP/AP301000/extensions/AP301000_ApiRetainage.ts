import { AP301000 } from '../AP301000';
import { PXView, PXFieldState, createSingle } from 'client-controls';

export interface AP3010000_ApiRetainage extends AP301000 { }

export class AP3010000_ApiRetainage {

	//PX.Objects.AP.APInvoiceEntryApiRetainage

	ReleaseRetainageFilter = createSingle(RetainageOptionsFilter); //PX.Objects.AP.APInvoiceEntryApiRetainage

}

export class RetainageOptionsFilter extends PXView {

	Date: PXFieldState;
	PostPeriod: PXFieldState;
	AmountToRelease: PXFieldState;

}
