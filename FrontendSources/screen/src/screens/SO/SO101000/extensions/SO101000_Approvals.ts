import {
	PXView,
	PXFieldState,
	PXFieldOptions,

	createCollection,

	viewInfo,
	gridConfig,
	columnConfig,
	GridPreset
} from 'client-controls';

import { SO101000 } from '../SO101000';

export interface SO101000_Approvals extends SO101000 {}
export class SO101000_Approvals {
	@viewInfo({containerName: "Approval"})
	SetupApproval = createCollection(SetupApproval);

	@viewInfo({containerName: "Invoice Approval"})
	SetupInvoiceApproval = createCollection(SetupInvoiceApproval);
}

@gridConfig({ preset: GridPreset.Details })
export class SetupApproval extends PXView {
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ textField: "AssignmentMapID_EPAssignmentMap_Name" })
	AssignmentMapID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ textField: "Name" })
	AssignmentNotificationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ preset: GridPreset.Details })
export class SetupInvoiceApproval extends PXView {
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	DocType: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ textField: "AssignmentMapID_EPAssignmentMap_Name" })
	AssignmentMapID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ textField: "Name" })
	AssignmentNotificationID: PXFieldState<PXFieldOptions.CommitChanges>;
}
