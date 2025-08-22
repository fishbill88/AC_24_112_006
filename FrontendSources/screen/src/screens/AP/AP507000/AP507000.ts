import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.AP1099SummaryEnq', primaryView: 'Year_Header' })
export class AP507000 extends PXScreen {

	Year_Header = createSingle(AP1099Year);
	Year_Summary = createCollection(AP1099Box);

}

export class AP1099Year extends PXView {
	OrganizationID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinYear: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, showTopBar: false })
export class AP1099Box extends PXView {
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	BoxNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Descr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AP1099History__HistAmt: PXFieldState;
}