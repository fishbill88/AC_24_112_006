import {
	PXView,
	PXFieldState,
	linkCommand,
	columnConfig,
	graphInfo,
	PXScreen,
	createCollection,
	gridConfig,
	PXFieldOptions,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.CR.CRGrammProcess', primaryView: 'Items' })
export class CR503400 extends PXScreen {
	Items = createCollection(ProcessingItem);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	suppressNoteFiles: true,
})
class ProcessingItem extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState
	BAccountID: PXFieldState;
	FullName: PXFieldState;
	ContactType: PXFieldState;
	@linkCommand("Items_ViewDetails") DisplayName: PXFieldState;
}
