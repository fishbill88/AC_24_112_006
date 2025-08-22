import { Messages as SysMessages } from "client-controls/services/messages";
import { createCollection, createSingle, PXScreen, graphInfo, viewInfo,
	PXPageLoadBehavior, PXView, PXFieldState, PXFieldOptions, gridConfig } from "client-controls";

@graphInfo({ graphType: "PX.Objects.SO.SOOrderInvoicesSP", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues })
export class SO4050SP extends PXScreen {
   	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(SOOrderInvoicesSPFilter);
	Invoices = createCollection(SOOrderInvoicesSPInqResult);
}

export class SOOrderInvoicesSPFilter extends PXView {
	OrderType : PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ mergeToolbarWith: "ScreenToolbar" })
export class SOOrderInvoicesSPInqResult extends PXView {
	DocType : PXFieldState;
	RefNbr : PXFieldState;
	Status : PXFieldState;
	DocDate : PXFieldState;
	DueDate : PXFieldState;
	FinPeriodID : PXFieldState;
	CuryOrigDocAmt : PXFieldState;
	CuryDocBal : PXFieldState;
	CuryID : PXFieldState;
	OrderNbr : PXFieldState;
	CustomerID : PXFieldState;
}
