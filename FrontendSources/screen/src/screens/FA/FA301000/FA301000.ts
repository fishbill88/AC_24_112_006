import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, TextAlign, PXActionState } from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.TransactionEntry", primaryView: "Document", showUDFIndicator: true })
export class FA301000 extends PXScreen {

	ViewBatch: PXActionState;
	ViewAsset: PXActionState;
	ViewBook: PXActionState;

	@viewInfo({ containerName: "Transaction Summary" })
	Document = createSingle(FARegister);

	@viewInfo({ containerName: "Transaction Details" })
	Trans = createCollection(FATran);
}

export class FARegister extends PXView {

	RefNbr: PXFieldState;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	Origin: PXFieldState<PXFieldOptions.Disabled>;
	Reason: PXFieldState<PXFieldOptions.Disabled>;
	DocDesc: PXFieldState;
	TranAmt: PXFieldState<PXFieldOptions.Disabled>;

}

@gridConfig({ initNewRow: true, syncPosition: true })
export class FATran extends PXView {

	AssetID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	AssetID_FixedAsset_description: PXFieldState;

	BookID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	TranType: PXFieldState;

	@columnConfig({ format: ">######", hideViewLink: true })
	DebitAccountID: PXFieldState;

	DebitAccountID_Account_description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DebitSubID: PXFieldState;

	DebitSubID_Sub_description: PXFieldState;

	@columnConfig({ format: ">######", hideViewLink: true })
	CreditAccountID: PXFieldState;

	CreditAccountID_Account_description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CreditSubID: PXFieldState;

	CreditSubID_Sub_description: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	TranAmt: PXFieldState;

	BatchNbr: PXFieldState;
	TranDesc: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	SrcBranchID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	MethodDesc: PXFieldState;

}
