import {
	PXScreen,
	PXView,
	PXFieldState,
	graphInfo,
	createSingle,
	createCollection,
	gridConfig,
	viewInfo,
	PXFieldOptions,
	columnConfig,
	linkCommand,
	GridPreset
} from "client-controls";

@graphInfo({graphType: 'PX.Data.Maintenance.TenantShapshotDeletion.TenantSnapshotDeletionProcess', primaryView: 'Filter'})
export class SM503000 extends PXScreen {
	@viewInfo({containerName: 'Filter'})
	Filter = createSingle(DeletionAction);

	@viewInfo({containerName: 'Records to Delete'})
	Records = createCollection(TenantSnapshotDeletion);
}

export class DeletionAction extends PXView {
	Name: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({preset: GridPreset.Primary,
	actionsConfig: {
		adjust: { hidden: true },
		exportToExcel: { hidden: true }
	}})
export class TenantSnapshotDeletion extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	TenantId: PXFieldState;
	@linkCommand('NavigateToTenant')
	TenantName: PXFieldState;
	SnapshotName: PXFieldState;
	Description: PXFieldState;
	@linkCommand('NavigateToTenant')
	Visibility: PXFieldState;
	SizeMB: PXFieldState;
	CreatedOn: PXFieldState;
	Version: PXFieldState;
	ExportMode: PXFieldState;
	Status: PXFieldState;
	DeletionStatus: PXFieldState;
	DeletionProgress: PXFieldState;
	DeletionHeartbeat: PXFieldState;
	SnapshotId: PXFieldState;
}
