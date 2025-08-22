import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
	PXPageLoadBehavior,
} from "client-controls";
import {
	AMEstimateItem,
	AMEstimateItem2,
	AddressLookupFilter,
	CRQuote,
	CRQuote2,
	CurrencyInfo,
	ReasonApproveRejectFilter,
	ReassignApprovalFilter,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.CR.QuoteMaint",
	primaryView: "Quote",
	pageLoadBehavior: PXPageLoadBehavior.SearchSavedKeys,
})
export class CR304500 extends PXScreen {
	ShowMatrixPanel: PXActionState;
	AddEstimate: PXActionState;
	RemoveEstimate: PXActionState;
	AddressLookup: PXActionState;
	ViewMainOnMap: PXActionState;
	BillingAddressLookup: PXActionState;
	ViewBillingOnMap: PXActionState;
	ShippingAddressLookup: PXActionState;
	ViewShippingOnMap: PXActionState;
	CreateContactCancel: PXActionState;
	CreateContactFinish: PXActionState;
	CreateContactFinishRedirect: PXActionState;
	AddressLookupSelectAction: PXActionState;
	CreateCustomerInPanel: PXActionState;
	CreateSalesOrderInPanel: PXActionState;

	@viewInfo({ containerName: "Quote Summary" })
	Quote = createSingle(CRQuote);
	@viewInfo({ containerName: "" })
	QuoteCurrent = createSingle(CRQuote2);

	@viewInfo({ containerName: "Enter Reason" })
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectFilter);
	@viewInfo({ containerName: "Reassign Approval" })
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
	@viewInfo({ containerName: "Address Lookup" })
	AddressLookupFilter = createSingle(AddressLookupFilter);
	@viewInfo({ containerName: "Add Matrix Item: Table View" })
	@viewInfo({ containerName: "Add Estimate" })
	OrderEstimateItemFilter = createSingle(AMEstimateItem);
	@viewInfo({ containerName: "Quick Estimate" })
	SelectedEstimateRecord = createSingle(AMEstimateItem2);
	@viewInfo({ containerName: "currencyinfo" })
	currencyinfo = createSingle(CurrencyInfo);
}
