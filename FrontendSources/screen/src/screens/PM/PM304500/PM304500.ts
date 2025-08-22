import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Quote,
	QuoteCurrent,
	Products,
	Tasks,
	Taxes,
	Answers,
	Approval,
	Quote_Contact,
	Quote_Address,
	Shipping_Address,
	CopyQuoteInfo,
	ConvertQuoteInfo,
	recalcdiscountsfilter,
	TasksForAddition,
	CurrencyInfo,
	Activities,
	ReasonApproveRejectParams,
	ReassignApprovalFilter,
	AddressLookupFilter
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.PMQuoteMaint",
	primaryView: "Quote", showUDFIndicator: true
})
export class PM304500 extends PXScreen {
	AddCommonTasks: PXActionState;
	AddressLookup: PXActionState;
	ViewMainOnMap: PXActionState;
	ShippingAddressLookup: PXActionState;
	ViewShippingOnMap: PXActionState;
	ViewActivity: PXActionState;
	OpenActivityOwner: PXActionState;
	AddressLookupSelectAction: PXActionState;

	Quote = createSingle(Quote);
	QuoteCurrent = createSingle(QuoteCurrent);
	Products = createCollection(Products);
	Tasks = createCollection(Tasks);
	Taxes = createCollection(Taxes);
	Answers = createCollection(Answers);
	Approval = createCollection(Approval);
	Quote_Contact = createSingle(Quote_Contact);
	Quote_Address = createSingle(Quote_Address);
	Shipping_Address = createSingle(Shipping_Address);
	CopyQuoteInfo = createSingle(CopyQuoteInfo);
	ConvertQuoteInfo = createSingle(ConvertQuoteInfo);
	recalcdiscountsfilter = createSingle(recalcdiscountsfilter);
	TasksForAddition = createCollection(TasksForAddition);
	CurrencyInfo = createSingle(CurrencyInfo);
	Activities = createCollection(Activities);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
	AddressLookupFilter = createSingle(AddressLookupFilter);
}
