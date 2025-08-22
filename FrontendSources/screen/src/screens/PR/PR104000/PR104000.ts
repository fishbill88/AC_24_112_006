import { autoinject } from 'aurelia-framework';
import {
	PXScreen,
	graphInfo,
	PXFieldState,
	createCollection,
	PXView,
	PXFieldOptions,
	columnConfig,
	gridConfig,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.PR.PROvertimeRuleMaint', primaryView: 'OvertimeRules' })
@autoinject
export class PR104000 extends PXScreen {
	OvertimeRules = createCollection(OvertimeRules);
}

@gridConfig({ initNewRow: true, syncPosition: true, wrapToolbar: true, mergeToolbarWith: "ScreenToolbar" })
export class OvertimeRules extends PXView {
	IsActive: PXFieldState;
	OvertimeRuleID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	DisbursingTypeCD: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeMultiplier: PXFieldState;
	RuleType: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeThreshold: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 110 })
	WeekDay: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 105})
	NumberOfConsecutiveDays: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 150, hideViewLink: true })
	UnionID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 150, hideViewLink: true })
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
}
