import { createCollection, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, linkCommand, columnConfig, GridColumnType, TextAlign, PXActionState } from "client-controls";

@graphInfo({ graphType: "PX.Objects.CA.CABankFeedImport", primaryView: "BankFeeds", })
export class CA507500 extends PXScreen {

	ViewBankFeed: PXActionState;

	BankFeeds = createCollection(CABankFeed);

}

@gridConfig({ syncPosition: true, mergeToolbarWith: "ScreenToolbar" })
export class CABankFeed extends PXView {

	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;

	@linkCommand("ViewBankFeed")
	BankFeedID: PXFieldState;

	Type: PXFieldState;
	Descr: PXFieldState;
	Institution: PXFieldState;
	Status: PXFieldState;
	RetrievalStatus: PXFieldState;
	RetrievalDate: PXFieldState;
	ErrorMessage: PXFieldState;
	AccountQty: PXFieldState;
	UnmatchedAccountQty: PXFieldState;

}
