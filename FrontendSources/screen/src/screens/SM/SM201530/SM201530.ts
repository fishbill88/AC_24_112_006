import { Messages as SysMessages } from "client-controls/services/messages";
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, localizable } from "client-controls";
import { TaskManagerRunningProcessFilter, RowTaskInfo, RowActiveUserInfo, SystemEvent, SystemEvent2, SMPerformanceInfo, SMPerformanceFilterRow, SMPerformanceInfoSQL, SMPerformanceInfoTraceEvents, SMPerformanceInfoTraceEvents2, RowMemoryDumpOptions } from "./views";

@localizable
export class MemoryDumpOptions {
	static MiniDump = "Running Processes Only (Mini-Dump)";
	static FullDump = "Full Memory Dump";
}

@graphInfo({graphType: "PX.SM.TaskManager", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class SM201530 extends PXScreen {
	MemoryDumpMode = MemoryDumpOptions;

	@viewInfo({containerName: "RUNNING PROCESSES"})
	Filter = createSingle(TaskManagerRunningProcessFilter);
	@viewInfo({containerName: "Operations"})
	Items = createCollection(RowTaskInfo);
	@viewInfo({containerName: "Active Users"})
	ActiveUsers = createCollection(RowActiveUserInfo);
	@viewInfo({containerName: "SYSTEM EVENTS"})
	SystemEvents = createCollection(SystemEvent);
	@viewInfo({containerName: "SYSTEM EVENTS"})
	CurrentSystemEvent = createSingle(SystemEvent2);
	@viewInfo({containerName: "REQUESTS IN PROGRESS"})
	Samples = createCollection(SMPerformanceInfo);
	@viewInfo({containerName: "Active Threads"})
	CurrentThreadsPanel = createSingle(SMPerformanceFilterRow);
	@viewInfo({containerName: "View SQL"})
	Sql = createCollection(SMPerformanceInfoSQL);
	@viewInfo({containerName: "Exception Profiler"})
	TraceExceptions = createCollection(SMPerformanceInfoTraceEvents);
	@viewInfo({containerName: "View Event Log"})
	TraceEvents = createCollection(SMPerformanceInfoTraceEvents2);
	@viewInfo({containerName: "Create Memory Dump"})
	ViewMemoryDumpOptions = createSingle(RowMemoryDumpOptions);

	@handleEvent(CustomEventType.RowSelected, { view: 'Items' })
	onRowTaskInfoChanged(args: RowSelectedHandlerArgs<PXViewCollection<RowTaskInfo>>) {
		const model = (<any>args.viewModel as RowTaskInfo);
		const ar = args.viewModel.activeRow;

		if (model.actionStop) model.actionStop.enabled = !!ar;
		if (model.actionShow) model.actionShow.enabled = !!ar;
	}
}
