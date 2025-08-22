import { CR301000 } from "../CR301000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	PXActionState,
	gridConfig,
	columnConfig,
	GridColumnShowHideMode,
	viewInfo,
	createSingle,
	GridPagerMode,
	GridPreset,
} from "client-controls";

export interface CR301000_Duplicates extends CR301000 {}
export class CR301000_Duplicates {
	@viewInfo({ containerName: "Records for Merging" })
	DuplicatesForMerging = createCollection(CRDuplicateRecordForMerging);

	@viewInfo({ containerName: "Records for Association" })
	DuplicatesForLinking = createCollection(CRDuplicateRecordForLinking);

	@viewInfo({ containerName: "Select the Contact for the Lead" })
	Link_SelectEntityForLink = createSingle(ContactForLead);
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowInsert: false,
	allowDelete: false,
	pagerMode: GridPagerMode.InfiniteScroll,
})
export class CRDuplicateRecordForMerging extends PXView {
	DuplicateMerge: PXActionState;

	@linkCommand("ViewMergingDuplicate")
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	DuplicateContact__DisplayName: PXFieldState;

	DuplicateContact__FullName: PXFieldState;
	CRLead__Description: PXFieldState;
	@linkCommand("ViewMergingDuplicateRefContact")
	CRLead__RefContactID: PXFieldState;
	@linkCommand("ViewMergingDuplicateBAccount")
	DuplicateContact__BAccountID: PXFieldState;
	BAccountR__Type: PXFieldState<PXFieldOptions.Hidden>;
	CRLead__Status: PXFieldState;
	DuplicateContact__Source: PXFieldState;
	Phone1: PXFieldState;
	DuplicateContact__EMail: PXFieldState;
	DuplicateContact__OwnerID: PXFieldState;

	DuplicateContactID: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__WebSite: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__Phone2: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__Phone3: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__CampaignID: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine1: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine2: PXFieldState<PXFieldOptions.Hidden>;
	Address__State: PXFieldState<PXFieldOptions.Hidden>;
	Address__City: PXFieldState<PXFieldOptions.Hidden>;
	Address__CountryID: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__FirstName: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__LastName: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__ContactID: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastIncomingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastOutgoingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CanBeMerged: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	pagerMode: GridPagerMode.InfiniteScroll,
})
export class CRDuplicateRecordForLinking extends PXView {
	DuplicateAttach: PXActionState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	DuplicateContact__ContactType: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	@linkCommand("ViewLinkingDuplicate")
	DuplicateContact__DisplayName: PXFieldState;

	@linkCommand("ViewLinkingDuplicateBAccount")
	DuplicateContact__BAccountID: PXFieldState;
	BAccountR__AcctName: PXFieldState;
	Phone1: PXFieldState;
	DuplicateContact__EMail: PXFieldState;
	DuplicateContact__OwnerID: PXFieldState;

	DuplicateContactID: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__WebSite: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine1: PXFieldState<PXFieldOptions.Hidden>;
	Address__AddressLine2: PXFieldState<PXFieldOptions.Hidden>;
	Address__State: PXFieldState<PXFieldOptions.Hidden>;
	Address__City: PXFieldState<PXFieldOptions.Hidden>;
	Address__CountryID: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__FirstName: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__LastName: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastIncomingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	CRActivityStatistics__LastOutgoingActivityDate: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__ContactID: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__LastModifiedDateTime: PXFieldState<PXFieldOptions.Hidden>;

	BAccountR__Type: PXFieldState<PXFieldOptions.Hidden>;
	DuplicateContact__Phone2: PXFieldState<PXFieldOptions.Hidden>;
}

export class ContactForLead extends PXView {
	@columnConfig({ allowUpdate: false })
	DisplayName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Salutation: PXFieldState;
	@columnConfig({ allowUpdate: false })
	EMail: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Phone1: PXFieldState;
	@columnConfig({ allowUpdate: false })
	IsPrimary: PXFieldState;
}
