import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo } from "client-controls";
import { EPTimeCard, CRActivity, EPTimeCardSummary, PMTimeActivity, EPTimeCardItem, EPApproval, ReasonApproveRejectFilter, ReassignApprovalFilter } from "./views";

@graphInfo({graphType: "PX.Objects.EP.TimeCardMaint", primaryView: "Document", bpEventsIndicator: true, showActivitiesIndicator: true, showUDFIndicator: true })
export class EP305000 extends PXScreen {
	PreloadFromTasks: PXActionState;
	preloadFromTasks: PXActionState;
	PreloadFromPreviousTimecard: PXActionState;
	PreloadHolidays: PXActionState;
	NormalizeTimecard: PXActionState;

   	@viewInfo({containerName: "Document Summary"})
	Document = createSingle(EPTimeCard);
   	@viewInfo({containerName: "Preload from Tasks"})
	Tasks = createCollection(CRActivity);
   	@viewInfo({containerName: "Summary"})
	Summary = createCollection(EPTimeCardSummary);
   	@viewInfo({containerName: "Details"})
	Activities = createCollection(PMTimeActivity);
   	@viewInfo({containerName: "Materials"})
	Items = createCollection(EPTimeCardItem);
   	@viewInfo({containerName: "Approval"})
	Approval = createCollection(EPApproval);
   	@viewInfo({containerName: "Enter Reason"})
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectFilter);
   	@viewInfo({containerName: "Reassign Approval"})
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}
