import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	GridPreset,
	columnConfig,
} from "client-controls";

import { CSAttributeGroup } from "../../interfaces/CR/AttributeGroup";

@graphInfo({ graphType: 'PX.Objects.CR.CRCaseClassMaint', primaryView: 'CaseClasses' })
export class CR206000 extends PXScreen {
	CaseClasses = createSingle(CRCaseClass);
	Mapping = createCollection(CSAttributeGroup);
	CaseClassesReaction = createCollection(CRClassSeverityTime);
	LaborMatrix = createCollection(CRCaseClassLaborMatrix);
	WorkCalendar = createSingle(CSCalendar);
}

class CRCaseClass extends PXView {
	CaseClassID: PXFieldState;
	IsInternal: PXFieldState;
	Description: PXFieldState;
	CalendarID: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireCustomer: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireContact: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowEmployeeAsContact: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireContract: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireClosureNotes: PXFieldState;
	DefaultEMailAccountID: PXFieldState;
	IsBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowOverrideBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	PerItemBilling: PXFieldState<PXFieldOptions.CommitChanges>;
	TrackSolutionsInActivities: PXFieldState<PXFieldOptions.CommitChanges>;
	LabourItemID: PXFieldState;
	OvertimeItemID: PXFieldState;
	RoundingInMinutes: PXFieldState;
	MinBillTimeInMinutes: PXFieldState;
	ReopenCaseTimeInDays: PXFieldState;
	StopTimeCounterType: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ preset: GridPreset.Details, fastFilterByAllFields: false })
class CRClassSeverityTime extends PXView {
	Severity: PXFieldState;
	@columnConfig({ allowNull: false })
	TrackInitialResponseTime: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	InitialResponseTimeTarget: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	InitialResponseGracePeriod: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	TrackResponseTime: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	ResponseTimeTarget: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	ResponseGracePeriod: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	TrackResolutionTime: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	ResolutionTimeTarget: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	ResolutionGracePeriod: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig ({ preset: GridPreset.Details })
class CRCaseClassLaborMatrix extends PXView {
	EarningType: PXFieldState;
	LabourItemID: PXFieldState;
}

class CSCalendar extends PXView {
	WorkdayTime: PXFieldState<PXFieldOptions.Disabled>;
}
