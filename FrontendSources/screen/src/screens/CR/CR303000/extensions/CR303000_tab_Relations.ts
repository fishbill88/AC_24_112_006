import { CR303000 } from "../CR303000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	viewInfo,
} from "client-controls";

export interface CR303000_Relations extends CR303000 {}
export class CR303000_Relations {
	@viewInfo({ containerName: "Relations" })
	Relations = createCollection(CRRelation);
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true,
	fastFilterByAllFields: false,
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
