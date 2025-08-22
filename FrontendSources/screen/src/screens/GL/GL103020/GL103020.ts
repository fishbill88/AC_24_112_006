import { PXView, createSingle, graphInfo, PXScreen, createCollection, columnConfig, TextAlign, gridConfig } from "client-controls";
import { PXFieldOptions, PXFieldState } from "client-controls/descriptors/fieldstate";

@graphInfo({ graphType: 'PX.Objects.GL.GLAccessByBranch', primaryView: 'Branch' })
export class GL103020 extends PXScreen {

	Branch = createSingle(Branch);
	Groups = createCollection(Groups);
}

@gridConfig({ allowInsert: false, allowDelete: false })
export class Groups extends PXView {

	@columnConfig({ textAlign: TextAlign.Center, allowCheckAll: true })
	Included: PXFieldState;
	@columnConfig({ hideViewLink: true })
	GroupName: PXFieldState;
	Description: PXFieldState;
	@columnConfig({})
	Active: PXFieldState;
	GroupType: PXFieldState;
}

export class Branch extends PXView {
	BranchCD: PXFieldState<PXFieldOptions.CommitChanges>;
	LedgerID: PXFieldState;
}
