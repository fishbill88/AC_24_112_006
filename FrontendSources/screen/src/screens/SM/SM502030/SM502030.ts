import {
	columnConfig,
	createCollection,
	createSingle,
	CustomEventType,
	graphInfo,
	GridColumnDisplayMode,
	GridColumnType,
	gridConfig,
	handleEvent,
	linkCommand,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXScreen,
	PXView,
	PXViewCollection,
	RowSelectedHandlerArgs,
	viewInfo
} from "client-controls";

@graphInfo({
	graphType: "PX.BusinessProcess.UI.BusinessProcessEventHistoryMaint",
	primaryView: "Filter",
	hideFilesIndicator: true,
	hideNotesIndicator: true
})
export class SM502030 extends PXScreen {
	ViewEventDetails: PXActionState;
	ViewBPEvent: PXActionState;
	@viewInfo({containerName: "Show Event Data"})
	EventDetails = createSingle(EventDetails);
	@viewInfo({containerName: "History Settings"})
	EventSettings = createSingle(BPEventSetting);
	Filter = createSingle(BPEventHistoryFilter);
	@viewInfo({containerName: "Business Events"})
	Events = createCollection(BPEventHistory);
	@viewInfo({containerName: "Subscribers"})
	EventSubscribers = createCollection(BPEventSubscriber);

	@handleEvent(CustomEventType.RowSelected, {view: "EventSubscribers"})
	onBPEventSubscriberChanged(args: RowSelectedHandlerArgs<PXViewCollection<BPEventSubscriber>>) {
		const model = (<any>args.viewModel as BPEventSubscriber);
		const ar = args.viewModel.activeRow;
		if (model.ExecuteSubscriber) model.ExecuteSubscriber.enabled = ar != null;
	}

	@handleEvent(CustomEventType.RowSelected, {view: "Events"})
	onBPEventHistoryChanged(args: RowSelectedHandlerArgs<PXViewCollection<BPEventHistory>>) {
		const scr = (<any>args.screenModel as SM502030);
		const ar = args.viewModel.activeRow;
		if (scr.ViewEventDetails) scr.ViewEventDetails.enabled = ar.Status?.value === 1;
	}
}

// Views

export class EventDetails extends PXView {
	Details: PXFieldState;
}

export class BPEventSetting extends PXView {
	DeleteHistoryAutomatically: PXFieldState;
	HistoryRetainCount: PXFieldState;
}

export class BPEventHistoryFilter extends PXView {
	DefinitionId: PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ToDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar",
	allowInsert: false,
	allowDelete: false,
	adjustPageSize: true,
	autoAdjustColumns: true,
	allowStoredFilters: true,
	preserveSortsAndFilters: true,
	autoRepaint: ["EventSubscribers"],
	actionsConfig: { insert: { hidden: true }, delete: { hidden: true } }
})
export class BPEventHistory extends PXView {
	@columnConfig({allowUpdate: false})
	Selected: PXFieldState;
	@columnConfig({allowUpdate: false, type: GridColumnType.Icon})
	LastRunStatus: PXFieldState;
	@columnConfig({allowUpdate: false})
	Status: PXFieldState;
	@linkCommand("ViewBPEvent")
	@columnConfig({allowUpdate: false})
	BPEvent__Name: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	BPEvent__Type: PXFieldState;
	@columnConfig({allowUpdate: false})
	BPEvent__Active: PXFieldState;
	@columnConfig({allowUpdate: false})
	BPEvent__ScreenIdValue: PXFieldState;
	@columnConfig({allowUpdate: false, format: "g"})
	CreatedDateTime: PXFieldState;
	@columnConfig({allowUpdate: false, format: "g"})
	LastModifiedDateTime: PXFieldState;
	@columnConfig({allowUpdate: false})
	ErrorText: PXFieldState;
	@linkCommand("ViewRelatedEntity")
	@columnConfig({allowUpdate: false})
	Source: PXFieldState;
}

@gridConfig({
	allowInsert: false, allowDelete: false, syncPosition: true, adjustPageSize: true, autoAdjustColumns: true
})
export class BPEventSubscriber extends PXView {
	ExecuteSubscriber: PXActionState;

	@columnConfig({allowUpdate: false})
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false, type: GridColumnType.Icon})
	LastRunStatus: PXFieldState;
	@linkCommand("ViewSubscriber")
	@columnConfig({allowUpdate: false, displayMode: GridColumnDisplayMode.Text})
	HandlerID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	Type: PXFieldState;
	@columnConfig({allowUpdate: false})
	ErrorText: PXFieldState;
}