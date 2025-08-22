import {
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,

	createSingle,
	createCollection,

	graphInfo,
	viewInfo,
	gridConfig,
} from "client-controls";

@graphInfo({ graphType: 'PX.Data.Archiving.ArchivalPolicyMaint', primaryView: 'Setup' })
export class SM200400 extends PXScreen {
	@viewInfo({ containerName: "Setup Header" })
	Setup = createSingle(Setup);

	@viewInfo({ containerName: "Policies"})
	Policies = createCollection(Policies);
}

export class Setup extends PXView {
	ArchivingProcessDurationLimitInHours: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true,
	allowImport: false,
	suppressNoteFiles: true,
})
export class Policies extends PXView {
	TableName: PXFieldState<PXFieldOptions.CommitChanges>;
	RetentionPeriodInMonths: PXFieldState<PXFieldOptions.CommitChanges>;
}
