import {
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({graphType: "PX.Objects.CS.ReasonCodeMaint", primaryView: "reasoncode"})
export class CS211000 extends PXScreen {

   	@viewInfo({containerName: "Reason Code"})
	reasoncode = createSingle(ReasonCode);
}

export class ReasonCode extends PXView  {
	ReasonCodeID : PXFieldState;
	Descr : PXFieldState;
	Usage : PXFieldState<PXFieldOptions.CommitChanges>;
	SubMaskInventory : PXFieldState;
	SubMask : PXFieldState;
	SubMaskFinance : PXFieldState;
	AccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	SubID : PXFieldState<PXFieldOptions.CommitChanges>;
	SalesAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	SalesSubID : PXFieldState<PXFieldOptions.CommitChanges>;
}