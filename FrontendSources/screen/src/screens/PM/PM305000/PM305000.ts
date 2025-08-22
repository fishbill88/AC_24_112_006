import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	Document,
	ProjectTotals,
	Details,
	Approval,
	AvailableCostBudget,
	History,
	CopyDialog,
	ReasonApproveRejectParams,
	ReassignApprovalFilter
} from './views';

@graphInfo({
	graphType: 'PX.Objects.CN.ProjectAccounting.CostProjectionEntry',
	primaryView: 'Document'
})
export class PM305000 extends PXScreen {
	AppendSelectedCostBudget: PXActionState;

	Document = createSingle(Document);
	ProjectTotals = createSingle(ProjectTotals);
	Details = createCollection(Details);
	Approval = createCollection(Approval);
	AvailableCostBudget = createCollection(AvailableCostBudget);
	History = createCollection(History);
	CopyDialog = createSingle(CopyDialog);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}

