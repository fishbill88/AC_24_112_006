import {
	SO303000
} from '../SO303000';

import {
	PXView,
	PXFieldState,
	featureInstalled,
	viewInfo,
	createSingle,
	PXActionState
} from 'client-controls';

export interface SO303000_PaymentLinks extends SO303000 {}
@featureInstalled('PX.Objects.CS.FeaturesSet+AcumaticaPayments')
export class SO303000_PaymentLinks {

	CreateLink: PXActionState;
	SyncLink: PXActionState;
	ResendLink: PXActionState;

	@viewInfo({containerName: 'Payment Links'})
	PayLink = createSingle(CCPayLink);
}

export class CCPayLink extends PXView  {
	Url: PXFieldState;
	LinkStatus: PXFieldState;
}

