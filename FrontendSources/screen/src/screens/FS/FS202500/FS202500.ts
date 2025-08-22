import {
	graphInfo,
	gridConfig,
	linkCommand,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.BranchLocationMaint', primaryView: 'BranchLocationRecords' })
export class FS202500 extends PXScreen {
	OpenRoom: PXActionState;
	AddressLookup: PXActionState;
	ViewMainOnMap: PXActionState;
	BranchLocationRecords = createSingle(FSBranchLocation);
	BranchLocation_Contact = createSingle(FSContact);
	BranchLocation_Address = createSingle(FSAddress);
	CurrentBranchLocation = createSingle(FSBranchLocation);
	RoomRecords = createCollection(FSRoom);
}

export class FSBranchLocation extends PXView {
	BranchLocationCD: PXFieldState;
	Descr: PXFieldState;
	BranchID: PXFieldState;
	SubID: PXFieldState;
	DfltSiteID: PXFieldState;
	DfltUOM: PXFieldState;
	RoomFeatureEnabled: PXFieldState;
}

export class FSContact extends PXView {
	FullName: PXFieldState<PXFieldOptions.CommitChanges>;
	Attention: PXFieldState;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState;
	Phone1Type: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.NoLabel>;
	Phone1: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2Type: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.NoLabel>;
	Phone2: PXFieldState<PXFieldOptions.CommitChanges>;
	FaxType: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.NoLabel>;
	Fax: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSAddress extends PXView {
	AddressLine1: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine2: PXFieldState<PXFieldOptions.CommitChanges>;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSRoom extends PXView {
	@linkCommand("OpenRoom") RoomID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	FloorNbr: PXFieldState;
}
