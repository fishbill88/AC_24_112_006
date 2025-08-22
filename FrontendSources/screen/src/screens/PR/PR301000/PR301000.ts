import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	Document,
	PRBatchTotalsFilter,
	Transactions,
	EarningDetails,
	Deductions,
	CurrentDocument,
	BatchOvertimeRules,
	AddEmployeeFilter,
	Employees,
	CurrentTransaction,
	EmployeeEarningDetails,
	ImportTimeActivitiesFilter,
	TimeActivities,
	UpdateTaxesPopupView,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PR.PRPayBatchEntry',
	primaryView: 'Document'
})
export class PR301000 extends PXScreen {
	AddSelectedEmployees: PXActionState;
	AddSelectedEmployeesAndClose: PXActionState;

	CopySelectedEarningDetailLine: PXActionState;
	ViewPayCheck: PXActionState;
	ViewVoidPayCheck: PXActionState;
	ViewTimeActivity: PXActionState;

	Document = createSingle(Document);
	PRBatchTotalsFilter = createSingle(PRBatchTotalsFilter);
	Transactions = createCollection(Transactions);
	EarningDetails = createCollection(EarningDetails);
	Deductions = createCollection(Deductions);
	CurrentDocument = createSingle(CurrentDocument);
	BatchOvertimeRules = createCollection(BatchOvertimeRules);
	AddEmployeeFilter = createSingle(AddEmployeeFilter);
	Employees = createCollection(Employees);
	CurrentTransaction = createSingle(CurrentTransaction);
	EmployeeEarningDetails = createCollection(EmployeeEarningDetails);
	ImportTimeActivitiesFilter = createSingle(ImportTimeActivitiesFilter);
	TimeActivities = createCollection(TimeActivities);
	UpdateTaxesPopupView = createSingle(UpdateTaxesPopupView);
}

