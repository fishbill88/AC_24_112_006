import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnType, TextAlign } from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.AssetGLTransactions", primaryView: "Filter", })
export class FA504500 extends PXScreen {

	@viewInfo({ containerName: "Options" })
	Filter = createSingle(GLTranFilter);

	@viewInfo({ containerName: "GL Transactions" })
	GLTransactions = createCollection(FAAccrualTran);

	@viewInfo({ containerName: "Transaction Split Details" })
	FATransactions = createCollection(FATran);
}

export class GLTranFilter extends PXView {

	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowReconciled: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Department: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false })
export class FAAccrualTran extends PXView {

	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	Selected: PXFieldState;

	@columnConfig({ format: ">AAAAAA", hideViewLink: true })
	BranchID: PXFieldState;

	ClassID: PXFieldState;
	Department: PXFieldState;

	EmployeeID: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	Reconciled: PXFieldState;

	@columnConfig({ allowUpdate: false, format: ">AAAAAAAAAA", hideViewLink: true })
	GLTranBranchID: PXFieldState;

	@columnConfig({ allowUpdate: false, format: ">AAAAAAAAAAAA" })
	GLTranInventoryID: PXFieldState;

	GLTranUOM: PXFieldState;

	@columnConfig({ allowUpdate: false, allowFilter: false, textAlign: TextAlign.Right })
	SelectedQty: PXFieldState;

	@columnConfig({ allowUpdate: false, allowFilter: false, textAlign: TextAlign.Right })
	SelectedAmt: PXFieldState;

	@columnConfig({ allowUpdate: false, allowFilter: false, textAlign: TextAlign.Right })
	OpenQty: PXFieldState;

	@columnConfig({ allowUpdate: false, allowFilter: false, textAlign: TextAlign.Right })
	OpenAmt: PXFieldState;

	@columnConfig({ allowUpdate: false, textAlign: TextAlign.Right })
	GLTranQty: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false, textAlign: TextAlign.Right })
	UnitCost: PXFieldState;

	@columnConfig({ allowUpdate: false, textAlign: TextAlign.Right })
	GLTranAmt: PXFieldState;

	@columnConfig({ allowUpdate: false })
	GLTranDate: PXFieldState;

	GLTranModule: PXFieldState;
	GLTranBatchNbr: PXFieldState;

	@columnConfig({ allowNull: false })
	GLTranRefNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	GLTranReferenceID: PXFieldState;

	GLTranDesc: PXFieldState;

}

@gridConfig({ initNewRow: true })
export class FATran extends PXView {

	RefNbr: PXFieldState<PXFieldOptions.Disabled>;
	LineNbr: PXFieldState;
	BookID: PXFieldState;
	ClassID: PXFieldState;
	TargetAssetID: PXFieldState;
	EmployeeID: PXFieldState;
	Department: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	NewAsset: PXFieldState;

	AssetCD: PXFieldState;

	@columnConfig({ format: ">AAAAAA", hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	Component: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	Qty: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	TranAmt: PXFieldState;

	ReceiptDate: PXFieldState;
	DeprFromDate: PXFieldState;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranType: PXFieldState;
	TranDesc: PXFieldState;
	TranDate: PXFieldState;

}
