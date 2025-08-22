import {
	CS102000
} from '../CS102000';

import {
	PXView, PXFieldState, PXFieldOptions,
	createCollection, viewInfo, featureInstalled	
} from 'client-controls';

export interface CS102000_TaxRegistration extends CS102000 { }

@featureInstalled('PX.Objects.CS.FeaturesSet+CanadianLocalization')
export class CS102000_TaxRegistration {
	@viewInfo({ containerName: 'Taxes' })
	Taxes = createCollection(TaxRegistration);
}

export class TaxRegistration extends PXView {
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxID_Tax_descr: PXFieldState;
	TaxRegistrationNumber: PXFieldState;
}
