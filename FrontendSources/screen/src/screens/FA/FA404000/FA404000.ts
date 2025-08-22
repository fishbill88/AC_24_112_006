import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, TextAlign, PXActionState } from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.FACostDetailsInq", primaryView: "Filter", })
export class FA404000 extends PXScreen {

	ViewDocument: PXActionState;
	ViewBatch: PXActionState;

	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(AccountFilter);

	@viewInfo({ containerName: "Transactions" })
	Transactions = createCollection(FATran);
}

export class AccountFilter extends PXView {

	AssetID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	EndPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	BookID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ syncPosition: true, mergeToolbarWith: "ScreenToolbar" })
export class FATran extends PXView {

	BookID: PXFieldState;

	@linkCommand("ViewDocument")
	RefNbr: PXFieldState;

	@linkCommand("ViewBatch")
	BatchNbr: PXFieldState;

	TranDate: PXFieldState;
	FinPeriodID: PXFieldState;
	BranchID: PXFieldState;
	TranType: PXFieldState;
	TranDesc: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	DebitAmt: PXFieldState;

	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	CreditAmt: PXFieldState;

	AccountID: PXFieldState;
	SubID: PXFieldState;

}
