import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Document,
	Project,
	Details,
	Approval,
	costBudgetfilter,
	CostBudgets,
	ReasonApproveRejectParams,
	ReassignApprovalFilter
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.ProgressWorksheetEntry",
	primaryView: "Document",
	showActivitiesIndicator: true, showUDFIndicator: true
})
export class PM303000 extends PXScreen {
	ViewDailyFieldReport: PXActionState;
	AddSelectedBudgetLines: PXActionState;

	Document = createSingle(Document);
	Project = createSingle(Project);
	Details = createCollection(Details);
	Approval = createCollection(Approval);
	costBudgetfilter = createSingle(costBudgetfilter);
	CostBudgets = createCollection(CostBudgets);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}
