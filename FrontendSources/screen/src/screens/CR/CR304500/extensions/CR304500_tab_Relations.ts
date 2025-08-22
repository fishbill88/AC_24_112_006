import { CR304500 } from "../CR304500";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	viewInfo,
} from "client-controls";

export interface CR304500_Relations extends CR304500 {}
export class CR304500_Relations {
	@viewInfo({ containerName: "Relations" })
	Relations = createCollection(CRRelation);
}

@gridConfig({
	initNewRow: true,
	syncPosition: true,
	allowDelete: false,
	allowUpdate: false,
})
export class CRRelation extends PXView {
	Role: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPrimary: PXFieldState<PXFieldOptions.CommitChanges>;
	TargetType: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("RelationsViewTargetDetails")
	TargetNoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	OwnerID: PXFieldState;
	@linkCommand("RelationsViewEntityDetails")
	EntityID: PXFieldState<PXFieldOptions.CommitChanges>;
	Name: PXFieldState;
	@linkCommand("RelationsViewContactDetails") ContactID: PXFieldState;
	Email: PXFieldState;
	AddToCC: PXFieldState;

	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID: PXFieldState<PXFieldOptions.Hidden>;
}
