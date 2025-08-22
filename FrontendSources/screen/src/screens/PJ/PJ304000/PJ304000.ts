import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	DailyFieldReport,
	EmployeeActivities,
	ProgressWorksheetLines,
	ChangeRequests,
	ChangeOrders,
	Subcontractors,
	ProjectIssues,
	PhotoLogs,
	MainPhoto,
	Notes,
	PurchaseReceipts,
	Equipment,
	Weather,
	Visitors,
	EmployeeExpenses,
	Approval,
	History,
	costBudgetfilter,
	CostBudgets,
	ReasonApproveRejectParams,
	ReassignApprovalFilter
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PJ.DailyFieldReports.PJ.Graphs.DailyFieldReportEntry",
	primaryView: "DailyFieldReport", showUDFIndicator: true
})
export class PJ304000 extends PXScreen {
	Refresh: PXActionState;
	ViewTimeCard: PXActionState;
	ViewProgressWorksheet: PXActionState;
	ViewChangeRequest: PXActionState;
	ViewChangeOrder: PXActionState;
	ViewVendor: PXActionState;
	ViewProjectIssue: PXActionState;
	ViewPhotoLog: PXActionState;
	ViewPurchaseReceipt: PXActionState;
	ViewEquipmentTimeCard: PXActionState;
	ViewBAccount: PXActionState;
	ViewExpenseReceipt: PXActionState;
	ViewExpenseClaim: PXActionState;
	ViewAttachedReport: PXActionState;
	ViewAddressOnMap: PXActionState;
	AddSelectedBudgetLines: PXActionState;

	DailyFieldReport = createSingle(DailyFieldReport);
	EmployeeActivities = createCollection(EmployeeActivities);
	ProgressWorksheetLines = createCollection(ProgressWorksheetLines);
	ChangeRequests = createCollection(ChangeRequests);
	ChangeOrders = createCollection(ChangeOrders);
	Subcontractors = createCollection(Subcontractors);
	ProjectIssues = createCollection(ProjectIssues);
	PhotoLogs = createCollection(PhotoLogs);
	MainPhoto = createSingle(MainPhoto);
	Notes = createCollection(Notes);
	PurchaseReceipts = createCollection(PurchaseReceipts);
	Equipment = createCollection(Equipment);
	Weather = createCollection(Weather);
	Visitors = createCollection(Visitors);
	EmployeeExpenses = createCollection(EmployeeExpenses);
	Approval = createCollection(Approval);
	History = createCollection(History);
	costBudgetfilter = createSingle(costBudgetfilter);
	CostBudgets = createCollection(CostBudgets);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}
