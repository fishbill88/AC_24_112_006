import { AR303000, ContactInfo } from '../AR303000';
import {
	PXFieldState,
	PXFieldOptions,
	featureInstalled,
} from 'client-controls';

export interface AR303000_PersonalDataPrivacy extends AR303000 { }
@featureInstalled('PX.Objects.CS.FeaturesSet+gDPRCompliance')
export class AR303000_PersonalDataPrivacy {
}

export interface ContactInfo_PersonalDataPrivacy extends ContactInfo { }
@featureInstalled('PX.Objects.CS.FeaturesSet+gDPRCompliance')
export class ContactInfo_PersonalDataPrivacy {
	ConsentAgreement: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
}
