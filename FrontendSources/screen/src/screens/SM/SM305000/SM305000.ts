import {
	createCollection,
	graphInfo,
	PXFieldState,
	PXView,
	PXScreen,
	GridColumnType,
	gridConfig,
	GridPagerMode,
	PXActionState,
	PXFieldOptions,
	columnConfig,
	TextAlign
} from "client-controls";

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	suppressNoteFiles: true,
	pagerMode: GridPagerMode.NextPrevFirstLast,
	autoRepaint: ["Teams", "Channels"]
})
export class SMTeamsTeam extends PXView {
	Active: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.Icon }) TeamPhoto: PXFieldState;
	DisplayName: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	suppressNoteFiles: true,
	pagerMode: GridPagerMode.NextPrevFirstLast,
})
export class SMTeamsChannel extends PXView {

	TestNotification: PXActionState;
	Active: PXFieldState;
	DisplayName: PXFieldState;
	NotificationUrl: PXFieldState;
	IsNotificationConfigured: PXFieldState<PXFieldOptions.Hidden>;
}

@graphInfo({
	graphType: 'PX.MSTeams.Graph.SM.TeamsChannelMaint',
	primaryView: 'Teams'
})
export class SM305000 extends PXScreen {

	Teams = createCollection(SMTeamsTeam);

	Channels = createCollection(SMTeamsChannel);
}
