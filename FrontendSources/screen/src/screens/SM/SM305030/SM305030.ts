import {
	createCollection,
	graphInfo,
	PXFieldState,
	PXView,
	PXScreen,
	GridColumnType,
	columnConfig,
	TextAlign,
	gridConfig
} from "client-controls";

@gridConfig({
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	suppressNoteFiles: true,
})
export class SMTeamsMember extends PXView {
	Active: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.Icon }) TeamPhoto: PXFieldState;
	UserPrincipalName: PXFieldState;
	DisplayName: PXFieldState;
	@columnConfig({ hideViewLink: true }) ContactID: PXFieldState;
}

@graphInfo({
	graphType: 'PX.MSTeams.Graph.SM.TeamsMemberMaint',
	primaryView: 'Members',
})
export class SM305030 extends PXScreen {

	Members = createCollection(SMTeamsMember);
}
