import {
	AR302000
} from '../AR302000';

import {
	PXView,
	createCollection,
	createSingle,
	PXFieldState,
	PXFieldOptions,
	featureInstalled,
	PXActionState,
	linkCommand,
	localizable
} from 'client-controls';


export interface AR302000_IntegratedCardProcessing extends AR302000 { }

@featureInstalled('PX.Objects.CS.FeaturesSet+IntegratedCardProcessing')
export class AR302000_IntegratedCardProcessing {
	ccPaymentInfo = createSingle(InputPaymentInfo);
}

export class InputPaymentInfo extends PXView {
	PCTranNumber: PXFieldState;
	AuthNumber: PXFieldState;
}
