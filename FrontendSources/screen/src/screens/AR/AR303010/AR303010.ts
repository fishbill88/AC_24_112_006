import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs,
	PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings,
	PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.AR.CustomerPaymentMethodMaint", primaryView: "CustomerPaymentMethod", showUDFIndicator: true })
export class AR303010 extends PXScreen {

	CreateCCPaymentMethodHF: PXActionState;
	ManageCCPaymentMethodHF: PXActionState;
	AddressLookup: PXActionState;
	ViewBillAddressOnMap: PXActionState;
	AddressLookupSelectAction: PXActionState;

	@viewInfo({ containerName: "Payment Method Selection" })
	CustomerPaymentMethod = createSingle(CustomerPaymentMethod);

	CurrentCPM = createSingle(CustomerPaymentMethod);

	@viewInfo({ containerName: "Payment Method Details" })
	Details = createCollection(CustomerPaymentMethodDetail);

	@viewInfo({ containerName: "Billing Info" })
	BillContact = createSingle(Contact);

	@viewInfo({ containerName: "Billing Info" })
	BillAddress = createSingle(Address);

	@viewInfo({ containerName: "Address Lookup" })
	AddressLookupFilter = createSingle(AddressLookupFilter);
}

export class CustomerPaymentMethod extends PXView {

	BAccountID: PXFieldState;
	PMInstanceID: PXFieldState<PXFieldOptions.Hidden>;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	HasBillingInfo: PXFieldState;
	IsActive: PXFieldState;
	CCProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerCCPID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	ExpirationDate: PXFieldState;
	DisplayCardType: PXFieldState;

	IsBillContactSameAsMain: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	IsBillAddressSameAsMain: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowDelete: false
})
export class CustomerPaymentMethodDetail extends PXView {

	DetailID: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Contact extends PXView {

	FirstName: PXFieldState;
	LastName: PXFieldState;
	Phone1: PXFieldState;
	Phone1Type: PXFieldState;
	Phone2: PXFieldState;
	Phone2Type: PXFieldState;
	Fax: PXFieldState;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Address extends PXView {

	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class AddressLookupFilter extends PXView {

	SearchAddress: PXFieldState<PXFieldOptions.NoLabel>;
	ViewName: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	AddressLine3: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
}
