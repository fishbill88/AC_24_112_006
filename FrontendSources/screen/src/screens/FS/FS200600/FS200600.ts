import {
	graphInfo,
	gridConfig,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.SkillMaint', primaryView: 'SkillRecords' })
export class FS200600 extends PXScreen {
	SkillRecords = createCollection(FSSkill);
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar"
})
export class FSSkill extends PXView {
	SkillCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	IsDriverSkill: PXFieldState;
}
