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
	columnConfig,
	GridColumnShowHideMode,
	GridPreset,
} from "client-controls";

export interface CR302000_Leads extends CR302000 {}
export class CR302000_Leads {
	@viewInfo({ containerName: "Leads" })
	Leads = createCollection(CRLeads);
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowDelete: false,
	allowUpdate: false,
	adjustPageSize: true,
	fastFilterByAllFields: false,
	topBarItems: {
		CreateLead: {
			index: 2,
			config: {
				commandName: "CreateLead",
				images: { normal: "main@RecordAdd" },
			},
		},
	},
})
export class CRLeads extends PXView {
	CreateLead: PXActionState;

	@linkCommand("Leads_ViewDetails")
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	MemberName: PXFieldState;
	Salutation: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState;
	Address__City: PXFieldState<PXFieldOptions.Hidden>;
	Address__State: PXFieldState<PXFieldOptions.Hidden>;
	Address__CountryID: PXFieldState<PXFieldOptions.Hidden>;
	EMail: PXFieldState;
	Phone1: PXFieldState;
	Phone2: PXFieldState<PXFieldOptions.Hidden>;
	Phone3: PXFieldState<PXFieldOptions.Hidden>;
	Source: PXFieldState<PXFieldOptions.Hidden>;
	CampaignID: PXFieldState;
	Status: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OwnerID: PXFieldState;
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastIncomingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastOutgoingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
}
