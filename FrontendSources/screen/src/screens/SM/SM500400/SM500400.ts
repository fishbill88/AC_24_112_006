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
	GridColumnGeneration,
	//IGridColumn,
} from "client-controls";

@graphInfo({ graphType: 'PX.Data.Archiving.ArchiveProcess', primaryView: 'Header' })
export class SM500400 extends PXScreen {
	@viewInfo({ containerName: "Header" })
	Header = createSingle(Header);

	@viewInfo({ containerName: "Dates To Archive" })
	DatesToArchive = createCollection(DatesToArchive);

	// private AllToArchiveField = "AllToArchive";
	// private ToArchiveSuffix = "ToArchive";

	// onFilterColumns(col: IGridColumn) {
	// 	if (col.field !== this.AllToArchiveField && col.field.endsWith(this.ToArchiveSuffix)) {
	// 		col.linkCommand = col.field;
	// 	}
	// 	return true;
	// }
}

export class Header extends PXView {
	ArchivingProcessDurationLimitInHours: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	batchUpdate: true,
	syncPosition: true,
	adjustPageSize: true,
	suppressNoteFiles: true,
	allowInsert: false,
	allowDelete: false,
	allowImport: false,
	allowStoredFilters: false,
	generateColumns: GridColumnGeneration.AppendDynamic,
	mergeToolbarWith: 'ScreenToolbar'
})
export class DatesToArchive extends PXView {
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	Date: PXFieldState<PXFieldOptions.Disabled>;
}
