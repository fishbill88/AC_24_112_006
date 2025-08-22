import {
	PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnShowHideMode
} from "client-controls";

// Views

export class Location extends PXView {
	BAccountID: PXFieldState;
	LocationCD: PXFieldState;
	Status: PXFieldState;
	IsDefault: PXFieldState;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	VBranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	VPrintOrder: PXFieldState;
	VEmailOrder: PXFieldState;
	OverrideRemitAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideRemitContact: PXFieldState<PXFieldOptions.CommitChanges>;
	IsAPPaymentInfoSameAsMain: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRegistrationID: PXFieldState;
	VTaxZoneID: PXFieldState;
	VTaxCalcMode: PXFieldState;
	VAllowAPBillBeforeReceipt: PXFieldState;
	VRcptQtyMin: PXFieldState;
	VRcptQtyMax: PXFieldState;
	VRcptQtyThreshold: PXFieldState;
	VRcptQtyAction: PXFieldState;
	VSiteID: PXFieldState;
	VCarrierID: PXFieldState;
	VShipTermsID: PXFieldState;
	VFOBPointID: PXFieldState;
	VLeadTime: PXFieldState;
	IsAPAccountSameAsMain: PXFieldState<PXFieldOptions.CommitChanges>;
	VExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	VExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	VDiscountAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	VDiscountSubID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Address extends PXView {
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class Contact extends PXView {
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1Type: PXFieldState;
	Phone1: PXFieldState;
	Phone2Type: PXFieldState;
	Phone2: PXFieldState;
	FaxType: PXFieldState;
	Fax: PXFieldState;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class LocationAPPaymentInfo extends PXView {
	VPaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	VCashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	VPaymentByType: PXFieldState;
	VPaymentLeadTime: PXFieldState;
	VSeparateCheck: PXFieldState;
}

@gridConfig({ allowDelete: false, allowInsert: false, allowUpdate: false, adjustPageSize: true })
export class VendorPaymentMethodDetail extends PXView {
	@columnConfig({ allowUpdate: false })
	PaymentMethodDetail__descr: PXFieldState;
	@columnConfig({ allowUpdate: false, allowShowHide: GridColumnShowHideMode.False })
	DetailValue: PXFieldState;
}

export class LocationAPAccountSub extends PXView {
	VAPAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	VAPSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	VRetainageAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	VRetainageSubID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class AddressLookupFilter extends PXView {
	SearchAddress: PXFieldState;
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

export class LocationBranchSettings extends PXView {
	VSiteID: PXFieldState;
}
