import { BindingEngine } from 'aurelia-framework';
import { treeConfig } from 'client-controls';
import { autoinject } from 'client-controls/dependency-property-injection';
import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.WFStageMaint', primaryView: 'Filter' })
export class FS202100 extends PXScreen {
	Filter = createSingle(WFStageFilter);
	Nodes = createCollection(Nodes);
	Items = createCollection(FSWFStage);
}

@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: 'Nodes',
	idParent: 'ParentWFStageID',
	idName: 'WFStageID',
	description: 'WFStageCD',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
})
export class Nodes extends PXView {
	ParentWFStageID: PXFieldState;
	WFStageID: PXFieldState;
	WFStageCD: PXFieldState;
}

export class WFStageFilter extends PXView {
	WFID: PXFieldState;
	Descr: PXFieldState;
}

@gridConfig({
	suppressNoteFiles: true
})
export class FSWFStage extends PXView {
	Up: PXActionState;
	Down: PXActionState;
	WFStageCD: PXFieldState;
	AllowComplete: PXFieldState;
	AllowCancel: PXFieldState;
	AllowReopen: PXFieldState;
	AllowClose: PXFieldState;
	AllowModify: PXFieldState;
	AllowDelete: PXFieldState;
	Descr: PXFieldState;
}
