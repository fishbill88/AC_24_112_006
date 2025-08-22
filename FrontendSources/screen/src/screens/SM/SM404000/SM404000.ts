import { autoinject } from 'aurelia-framework';
import {
	commitChanges,
	createSingle,
	createCollection,
	graphInfo,
	GridColumnShowHideMode,
	GridColumnType,
	PXFieldState,
	PXScreen,
	PXView,
	gridConfig,
	GridPagerMode,
	columnConfig
} from "client-controls";

@gridConfig({
	adjustPageSize: true,
	allowDelete: false,
	allowInsert: false,
	batchUpdate: true,
	pagerMode: GridPagerMode.NextPrevFirstLast,
	syncPosition: true
})
export class SharedFiles extends PXView {
	@columnConfig({ allowCheckAll: true, allowShowHide: GridColumnShowHideMode.False }) Selected: PXFieldState;
	@columnConfig({ hideViewLink: true }) FileName: PXFieldState;
	LastModifiedDateTime: PXFieldState;
	@columnConfig({ type: GridColumnType.Icon }) TeamPhoto: PXFieldState;
	@columnConfig({ hideViewLink: true }) SharedByName: PXFieldState;
}

export class Filter extends PXView {
	@commitChanges FileName: PXFieldState;
}

@graphInfo({
	graphType: 'PX.MSTeams.Graph.SM.SMImportSharedFilesInq',
	primaryView: 'SharedFiles',
	bpEventsIndicator: false
})
@autoinject
export class SM404000 extends PXScreen {
	Filter = createSingle(Filter);

	SharedFiles = createCollection(SharedFiles);
}
