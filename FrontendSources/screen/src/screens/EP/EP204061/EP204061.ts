import { createCollection, createSingle, PXScreen, graphInfo,
	PXActionState, viewInfo, PXView, PXFieldState,
	PXFieldOptions, linkCommand, columnConfig, TextAlign,
	GridColumnType, gridConfig, GridPreset, treeConfig } from "client-controls";

@graphInfo({ graphType: "PX.TM.CompanyTreeMaint", primaryView: "SelectedFolders" })
export class EP204061 extends PXScreen {
	ViewEmployee: PXActionState;
	Up: PXActionState;
	Down: PXActionState;
	MoveWorkGroup: PXActionState;
	AddWorkGroup: PXActionState;
	DeleteWorkGroup: PXActionState;

	SelectedFolders = createSingle(SelectedNode);
	@viewInfo({ containerName: "Company Tree" })
	Folders = createCollection(EPCompanyTree);
	@viewInfo({ containerName: "Workgroup Details" })
	CurrentWorkGroup = createSingle(EPCompanyTree2);
	@viewInfo({ containerName: "Members" })
	Members = createCollection(EPCompanyTreeMember);
	@viewInfo({ containerName: "Move Workgroup" })
	SelectedParentFolders = createSingle(SelectedParentNode);
	@viewInfo({ containerName: "Move Workgroup" })
	ParentFolders = createSingle(EPCompanyTree3);
}

export class SelectedNode extends PXView {
	WorkGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: 'Folders',
	idParent: 'Key',
	idName: 'WorkGroupID',
	description: 'Description',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
	selectFirstNode: true,
	syncPosition: true,
	hideToolbarSearch: true,
	topBarItems: {
		Up: { config: { commandName: 'Up', text: '', images: { normal: 'main@ArrowUp' } } },
		Down: { config: { commandName: 'Down', text: '', images: { normal: 'main@ArrowDown' } } },
		Move: { config: { commandName: "MoveWorkGroup", images: {normal: "main@Roles"} } },
		AddWorkGroup: { config: { commandName: "AddWorkGroup", images: {normal: "main@AddNew"} } },
		DeleteWorkGroup: {config: {commandName: 'DeleteWorkGroup', images: {normal: 'main@RecordDel' } } }
	}
})
export class EPCompanyTree extends PXView {
	WorkGroupID: PXFieldState;
	Description: PXFieldState;
	Key: PXFieldState;
}

export class EPCompanyTree2 extends PXView {
	WorkGroupID: PXFieldState;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	fastFilterByAllFields: false,
	allowInsert: true,
	allowUpdate: true,
})
export class EPCompanyTreeMember extends PXView {
	@columnConfig({ hideViewLink: true })
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewEmployee")
	EPEmployee__AcctCD: PXFieldState;
	@columnConfig({ hideViewLink: true })
	EPEmployee__AcctName: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	EPEmployeePosition__PositionID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	EPEmployee__DepartmentID: PXFieldState;
	MembershipType: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsOwner: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	Active: PXFieldState;
}

export class SelectedParentNode extends PXView {}

export class EPCompanyTree3 extends PXView {
	WorkGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
}
