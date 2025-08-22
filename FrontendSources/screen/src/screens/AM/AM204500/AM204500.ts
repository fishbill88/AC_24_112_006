import {
	PXScreen,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from 'client-controls';

export class Machines extends PXView {
	MachID: PXFieldState;
	ActiveFlg: PXFieldState;
	Descr: PXFieldState;
	DownFlg: PXFieldState;
	AssetID: PXFieldState;
	CalendarID: PXFieldState;
	MachEff: PXFieldState;
	MachAcctID: PXFieldState;
	MachSubID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurySettings_AMMach extends PXView {
	StdCost: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.MachMaint', primaryView: 'Machines' })
export class AM204500 extends PXScreen {
	Machines = createSingle(Machines);
	CurySettings_AMMach = createSingle(CurySettings_AMMach);
}
