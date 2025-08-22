import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo,
	viewInfo
} from 'client-controls';

import {
	Document,
	Summary,
	Details,
	Approval,
	ReasonApproveRejectParams,
	ReassignApprovalFilter
} from './views';

@graphInfo({
	graphType: 'PX.Objects.EP.EquipmentTimeCardMaint',
	primaryView: 'Document',
	showUDFIndicator: true
})
export class EP308000 extends PXScreen {
	Document = createSingle(Document);
	Summary = createCollection(Summary);
	Details = createCollection(Details);
	Approval = createCollection(Approval);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}

