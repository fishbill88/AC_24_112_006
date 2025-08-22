import { CR302000 } from "../CR302000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	PXActionState,
	viewInfo,
	GridPreset,
} from "client-controls";

export interface CR302000_Notifications extends CR302000 {}
export class CR302000_Notifications {
	@viewInfo({ containerName: "Notifications" })
	NWatchers = createCollection(NotificationRecipient);
}

@gridConfig({
	preset: GridPreset.Details,
	allowUpdate: false,
	allowInsert: false,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class NotificationRecipient extends PXView {
	//convertToOpportunityAll: PXActionState;

	NotificationSetup__Module: PXFieldState<PXFieldOptions.Disabled>;
	NotificationSetup__SourceCD: PXFieldState<PXFieldOptions.Disabled>;
	NotificationSetup__NotificationCD: PXFieldState;
	ClassID: PXFieldState<PXFieldOptions.Disabled>;
	EntityDescription: PXFieldState<PXFieldOptions.Disabled>;
	ReportID: PXFieldState;
	NotificationID: PXFieldState;
	Format: PXFieldState; // AllowNull="False"
	Hidden: PXFieldState; //AllowNull="False"
	Active: PXFieldState;
}
