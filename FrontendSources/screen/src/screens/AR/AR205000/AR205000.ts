import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, columnConfig, PXActionState } from "client-controls";

@graphInfo({ graphType: "PX.Objects.AR.SalesPersonMaint", primaryView: "Salesperson", })
export class AR205000 extends PXScreen {

	@viewInfo({ containerName: "Salesperson Info" })
	Salesperson = createSingle(SalesPerson);

	@viewInfo({ containerName: "Customers" })
	SPCustomers = createCollection(CustSalesPeople);

	@viewInfo({ containerName: " Commission History" })
	CommissionsHistory = createCollection(ARSPCommnHistory);

}

export class SalesPerson extends PXView {

	SalesPersonCD: PXFieldState;
	IsActive: PXFieldState;
	Descr: PXFieldState;
	CommnPct: PXFieldState;
	SalesSubID: PXFieldState;

}

export class CustSalesPeople extends PXView {

	@columnConfig({ hideViewLink: true })
	BAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState;
	CommisionPct: PXFieldState;
	BAccountID_Customer_acctName: PXFieldState;
	LocationID_Location_descr: PXFieldState;
	IsDefault: PXFieldState;

}

export class ARSPCommnHistory extends PXView {

	ViewDetails: PXActionState;
	CommnPeriod: PXFieldState;
	CommnblAmt: PXFieldState;
	CommnAmt: PXFieldState;
	BaseCuryID: PXFieldState;
	PRProcessedDate: PXFieldState;

}
