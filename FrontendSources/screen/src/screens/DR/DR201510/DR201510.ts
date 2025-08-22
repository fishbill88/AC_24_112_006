import {
	PXScreen, createCollection, graphInfo,
	PXView, PXFieldState, PXActionState,
	PXFieldOptions,
	linkCommand, gridConfig, columnConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.DR.DRSchedulePrimary', primaryView: 'Items', })
export class DR201510 extends PXScreen {
	ViewSchedule: PXActionState
	ViewDoc: PXActionState

	Items = createCollection(DRSchedule);
}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false, mergeToolbarWith: 'ScreenToolbar' })
export class DRSchedule extends PXView {
	@linkCommand('ViewSchedule')
	ScheduleNbr: PXFieldState;

	Status: PXFieldState;

	BAccountType: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BAccountID: PXFieldState;

	DocumentTypeEx: PXFieldState;

	@linkCommand('ViewDoc')
	RefNbr: PXFieldState;
}
