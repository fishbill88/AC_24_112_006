import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand,
	columnConfig, GridColumnType, PXActionState, TextAlign
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.AR.ARFinChargesApplyMaint", primaryView: "Filter", })
export class AR507000 extends PXScreen {

	ViewDocument: PXActionState;

	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(ARFinChargesApplyParameters);

	ARFinChargeRecords = createCollection(ARFinChargesDetails);
}

export class ARFinChargesApplyParameters extends PXView {
	StatementCycle: PXFieldState<PXFieldOptions.CommitChanges>;
	FinChargeDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, mergeToolbarWith: "ScreenToolbar" })
export class ARFinChargesDetails extends PXView {

	@columnConfig({ allowCheckAll: true, allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	Selected: PXFieldState;

	BranchID: PXFieldState;
	DocType: PXFieldState;

	@linkCommand("ViewDocument")
	RefNbr: PXFieldState;

	DocDate: PXFieldState;
	DueDate: PXFieldState;
	CustomerID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CustomerID_BAccountR_acctName: PXFieldState;

	CuryID: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	CuryOrigDocAmt: PXFieldState;

	@columnConfig({ allowUpdate: false, textAlign: TextAlign.Right })
	CuryDocBal: PXFieldState;

	LastPaymentDate: PXFieldState;
	LastChargeDate: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	OverdueDays: PXFieldState;

	FinChargeCuryID: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Right })
	FinChargeAmt: PXFieldState;

	@columnConfig({ allowUpdate: false, format: ">AAAAAAAAAA" })
	ARAccountID: PXFieldState;

	@columnConfig({ allowUpdate: false, format: ">AAAA-AA-AA-AAAA" })
	ARSubID: PXFieldState;

	FinChargeID: PXFieldState;
}
