import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection } from "client-controls";
import { EPWeeklyCrewTimeActivity, EPWeeklyCrewTimeActivityFilter, PMTimeActivity, EPTimeActivitiesSummary, EPTimeActivitiesSummary2, PMTimeActivity2 } from "./views";

@graphInfo({ graphType: "PX.Objects.EP.EPWeeklyCrewTimeEntry", primaryView: "Document" })
export class EP307100 extends PXScreen {
	DeleteMember: PXActionState;
	InsertForBulkTimeEntry: PXActionState;

	Document = createSingle(EPWeeklyCrewTimeActivity);
	Filter = createSingle(EPWeeklyCrewTimeActivityFilter);

	@viewInfo({ containerName: "Time Activities" })
	TimeActivities = createCollection(PMTimeActivity);

	@viewInfo({ containerName: "Crew Members" })
	WorkgroupTimeSummary = createCollection(EPTimeActivitiesSummary);

	@viewInfo({ containerName: "Mass Enter Time" })
	CompanyTreeMembers = createCollection(EPTimeActivitiesSummary2);

	@viewInfo({ containerName: "Bulk Entry Time Activities" })
	BulkEntryTimeActivities = createCollection(PMTimeActivity2);

	@handleEvent(CustomEventType.RowSelected, { view: "TimeActivities" })
	onPMTimeActivityChanged(args: RowSelectedHandlerArgs<PXViewCollection<PMTimeActivity>>) {
		const model = (<any>args.viewModel as PMTimeActivity);
		const ar = args.viewModel.activeRow;

		/** Ensure that 'Copy Selected Line' action is enabled only when there is at least one row in Time Activities. */
		if (model.CopySelectedActivity) model.CopySelectedActivity.enabled = !!ar;
	}
}
