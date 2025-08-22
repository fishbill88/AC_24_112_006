import { CR304000 } from "../CR304000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	viewInfo,
} from "client-controls";

export interface CR304000_Relations extends CR304000 {}
export class CR304000_Relations {
	@viewInfo({ containerName: "Relations" })
	Relations = createCollection(CRRelation);
}

@gridConfig({
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
