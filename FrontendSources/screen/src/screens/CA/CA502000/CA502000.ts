import {
	PXScreen, createCollection, graphInfo, PXView, gridConfig,
	PXFieldState,
	PXActionState,
	columnConfig,
	linkCommand
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CA.CATrxRelease', primaryView: 'CARegisterList' })
export class CA502000 extends PXScreen {
	CARegisterList = createCollection(CARegister);
}

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class CARegister extends PXView {
	ViewCATrx: PXActionState;

	BranchID: PXFieldState;

	@columnConfig({ allowCheckAll: true, allowNull: false })
	Selected: PXFieldState;

	TranType: PXFieldState;

	@linkCommand("ViewCATrx")
	ReferenceNbr: PXFieldState

	@columnConfig({ hideViewLink: true })
	CashAccountID: PXFieldState

	CuryID: PXFieldState

	@columnConfig({ allowNull: false })
	CuryTranAmt: PXFieldState

	Description: PXFieldState

	DocDate: PXFieldState

	FinPeriodID: PXFieldState
}
