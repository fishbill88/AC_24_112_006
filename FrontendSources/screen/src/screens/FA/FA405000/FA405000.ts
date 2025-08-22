import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState, TextAlign } from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.FASplitsInq", primaryView: "Filter", })
export class FA405000 extends PXScreen {

	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(AccountFilter);

	@viewInfo({ containerName: "Split Transactions" })
	Transactions = createCollection(FATran);
}

export class AccountFilter extends PXView {

	AssetID: PXFieldState<PXFieldOptions.CommitChanges>;
	BookID: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ mergeToolbarWith: "ScreenToolbar" })
export class FATran extends PXView {

	AssetID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BookID: PXFieldState;

	RefNbr: PXFieldState;
	TranDate: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	TranType: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	DebitAmt: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	CreditAmt: PXFieldState;

}
