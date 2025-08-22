import {
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,

	createSingle,
	createCollection,

	graphInfo,
	viewInfo,
	gridConfig,
	columnConfig,

	GridPagerMode,
	GridColumnGeneration,
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: 'PX.Data.Archiving.ArchivationHistoryEnq', primaryView: 'Header' })
export class SM400400 extends PXScreen {
	@viewInfo({ containerName: "Header" })
	Header = createSingle(Header);

	@viewInfo({ containerName: "Archiving Executions" })
	ArchivingExecutions = createCollection(ArchivingExecutions);

	@viewInfo({ containerName: "Archived Dates" })
	ArchivedDates = createCollection(ArchivedDates);
}

export class Header extends PXView {
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	suppressNoteFiles: true,
	allowUpdate: false,
	allowStoredFilters: false,
})
export class ArchivingExecutions extends PXView {
	ExecutionDate: PXFieldState<PXFieldOptions.Disabled>;
	ExecutionTime: PXFieldState<PXFieldOptions.Disabled>;
	CreatedByID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFilter: false })
	ArchivedRowsCount: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	suppressNoteFiles: true,
	allowUpdate: false,
	allowStoredFilters: false,
	pagerMode: GridPagerMode.NextPrevFirstLast,
	generateColumns: GridColumnGeneration.AppendDynamic,
})
export class ArchivedDates extends PXView {
	DateToArchive: PXFieldState<PXFieldOptions.Disabled>;
	ExecutionTime: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFilter: false })
	ArchivedRowsCount: PXFieldState<PXFieldOptions.Disabled>;
}
