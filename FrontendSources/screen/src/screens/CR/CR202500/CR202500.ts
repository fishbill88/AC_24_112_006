import { autoinject } from 'aurelia-framework';
import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from 'client-controls';

import { CSAttributeGroup } from "../../interfaces/CR/AttributeGroup";

@graphInfo({ graphType: 'PX.Objects.CR.CRCampaignClassMaint', primaryView: 'CampaignClass', showActivitiesIndicator: true })
export class CR202500 extends PXScreen {
	CampaignClass = createSingle(CRCampaignType);
	Mapping = createCollection(CSAttributeGroup);
}

export class CRCampaignType extends PXView {
	TypeID: PXFieldState;
	Description: PXFieldState;
}
