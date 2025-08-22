import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnType, TextAlign } from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.GenerationPeriods", primaryView: "Years", })
export class FA501000 extends PXScreen {

	@viewInfo({ containerName: "Parameters" })
	Years = createSingle(BoundaryYears);

	@viewInfo({ containerName: "Books" })
	Books = createCollection(FABook);
}

export class BoundaryYears extends PXView {

	FromYear: PXFieldState<PXFieldOptions.CommitChanges>;
	ToYear: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ syncPosition: true, mergeToolbarWith: "ScreenToolbar" })
export class FABook extends PXView {

	@columnConfig({ allowCheckAll: true, allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	Selected: PXFieldState;

	@columnConfig({ allowUpdate: false })
	OrganizationCD: PXFieldState;

	@columnConfig({ allowUpdate: false, format: ">CCCCCCCCCC" })
	BookCode: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Description: PXFieldState;

	@columnConfig({ allowUpdate: false, textAlign: TextAlign.Right })
	FirstCalendarYear: PXFieldState;

	@columnConfig({ allowUpdate: false, textAlign: TextAlign.Right })
	LastCalendarYear: PXFieldState;

}
