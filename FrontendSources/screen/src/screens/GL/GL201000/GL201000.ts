import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions 
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.MasterFinPeriodMaint', primaryView: 'FiscalYear' })
export class GL201000 extends PXScreen {

	FiscalYear = createSingle(FiscalYear);
	Periods = createCollection(Periods);
	GenerateParams = createSingle(GenerateParams);
	SaveDialog = createSingle(SaveDialog);

}

export class FiscalYear extends PXView {

	Year: PXFieldState;
	StartDate: PXFieldState;
	FinPeriods: PXFieldState;
	CustomPeriods: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class Periods extends PXView {

	FinPeriodID: PXFieldState;
	StartDateUI: PXFieldState;
	EndDateUI: PXFieldState<PXFieldOptions.CommitChanges>;
	Length: PXFieldState;
	Descr: PXFieldState;
	Status: PXFieldState;
	IsAdjustment: PXFieldState<PXFieldOptions.CommitChanges>;
	APClosed: PXFieldState;
	ARClosed: PXFieldState;
	INClosed: PXFieldState;
	Closed: PXFieldState;
	CAClosed: PXFieldState;
	FAClosed: PXFieldState;

}

export class GenerateParams extends PXView {

	FirstFinYear: PXFieldState;
	LastFinYear: PXFieldState;
	FromYear: PXFieldState<PXFieldOptions.CommitChanges>;
	ToYear: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class SaveDialog extends PXView {

	Message: PXFieldState;
	Method: PXFieldState<PXFieldOptions.CommitChanges>;
	MoveDayOfWeek: PXFieldState<PXFieldOptions.CommitChanges>;
	MethodDescription: PXFieldState;

}
