import { AP303000 } from '../AP303000';
import { ContactInfo } from '../views';
import { PXFieldState, PXFieldOptions, featureInstalled, } from 'client-controls';

export interface AP303000_PersonalDataPrivacy extends AP303000 { }
@featureInstalled('PX.Objects.CS.FeaturesSet+gDPRCompliance')
export class AP303000_PersonalDataPrivacy {
}

export interface ContactInfo_PersonalDataPrivacy extends ContactInfo { }
@featureInstalled('PX.Objects.CS.FeaturesSet+gDPRCompliance')
export class ContactInfo_PersonalDataPrivacy {
	ConsentAgreement: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
}
