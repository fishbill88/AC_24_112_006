import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo,
	viewInfo,
	localizable
} from 'client-controls';

import {
	Document,
	DocumentSettings,
	Details,
	Markups,
	Approval,
	ReasonApproveRejectParams,
	ReassignApprovalFilter
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PM.ChangeRequest.ChangeRequestEntry',
	primaryView: 'Document', showUDFIndicator: true
})
export class PM308500 extends PXScreen {
	Document = createSingle(Document);
	DocumentSettings = createSingle(DocumentSettings);
	Details = createCollection(Details);
	Markups = createCollection(Markups);
	Approval = createCollection(Approval);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}

