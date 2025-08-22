import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	Project,
	ProjectProperties,
	Billing,
	RetainageSteps,
	Tasks,
	RevenueBudget,
	CostBudget,
	EmployeeContract,
	ContractRates,
	EquipmentRates,
	Accounts,
	Markups,
	Answers,
	LienWaiverRecipients,
	NotificationSources,
	NotificationRecipients,
	CopyDialog,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PM.TemplateMaint',
	primaryView: 'Project'
})
export class PM208000 extends PXScreen {
	Refresh: PXActionState;
	ViewTask: PXActionState;

	Project = createSingle(Project);
	ProjectProperties = createSingle(ProjectProperties);
	Billing = createSingle(Billing);
	RetainageSteps = createCollection(RetainageSteps);
	Tasks = createCollection(Tasks);
	RevenueBudget = createCollection(RevenueBudget);
	CostBudget = createCollection(CostBudget);
	EmployeeContract = createCollection(EmployeeContract);
	ContractRates = createCollection(ContractRates);
	EquipmentRates = createCollection(EquipmentRates);
	Accounts = createCollection(Accounts);
	Markups = createCollection(Markups);
	Answers = createCollection(Answers);
	LienWaiverRecipients = createCollection(LienWaiverRecipients);
	NotificationSources = createCollection(NotificationSources);
	NotificationRecipients = createCollection(NotificationRecipients);
	CopyDialog = createSingle(CopyDialog);
}

