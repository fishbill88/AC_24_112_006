import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs,
	PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings,
	PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign
} from "client-controls";

@graphInfo({graphType: "PX.Objects.AR.CustomerLocationMaint", primaryView: "Location", showUDFIndicator: true })
export class AR303020 extends PXScreen {

	AddressLookup: PXActionState;
	ViewOnMap: PXActionState;
	AddressLookupSelectAction: PXActionState;

   	@viewInfo({containerName: "Location Summary"})
		  Location = createSingle(Location);

	LocationCurrent = createSingle(Location);

   	@viewInfo({containerName: "General"})
		  Address = createSingle(Address);

   	@viewInfo({containerName: "General"})
		  Contact = createSingle(Contact);

   	@viewInfo({containerName: "GL Accounts"})
		  ARAccountSubLocation = createSingle(LocationARAccountSub);

   	@viewInfo({containerName: "Contacts"})
		  RoleAssignments = createCollection(BCRoleAssignment);

   	@viewInfo({containerName: "Address Lookup"})
		  AddressLookupFilter = createSingle(AddressLookupFilter);

}

export class Location extends PXView  {

	BAccountID: PXFieldState;
	LocationCD : PXFieldState;
	Status : PXFieldState;
	IsDefault: PXFieldState;

	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	CBranchID: PXFieldState;
	CPriceClassID: PXFieldState;
	CDefProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRegistrationID: PXFieldState;
	CTaxZoneID: PXFieldState;
	CTaxCalcMode: PXFieldState;
	CAvalaraExemptionNumber: PXFieldState;
	CAvalaraCustomerUsageType: PXFieldState;
	CSiteID: PXFieldState;
	CCarrierID: PXFieldState<PXFieldOptions.CommitChanges>;
	CShipTermsID: PXFieldState;
	CShipZoneID: PXFieldState;
	CFOBPointID: PXFieldState;
	CResedential: PXFieldState;
	CSaturdayDelivery: PXFieldState;
	CInsurance: PXFieldState;
	CGroundCollect: PXFieldState;
	CShipComplete: PXFieldState;
	COrderPriority: PXFieldState;
	CLeadTime: PXFieldState;
	CCalendarID: PXFieldState;
	IsARAccountSameAsMain: PXFieldState<PXFieldOptions.CommitChanges>;
	CSalesAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	CSalesSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	CDiscountAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	CDiscountSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	CFreightAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	CFreightSubID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Address extends PXView  {

	AddressLine1: PXFieldState;
	AddressLine2 : PXFieldState;
	City : PXFieldState;
	State : PXFieldState;
	PostalCode : PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID : PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude : PXFieldState;
	Longitude : PXFieldState;
	IsValidated : PXFieldState<PXFieldOptions.Disabled>;
}

export class Contact extends PXView  {

	FullName: PXFieldState;
	Attention : PXFieldState;
	Phone1Type : PXFieldState<PXFieldOptions.NoLabel>;
	Phone1 : PXFieldState<PXFieldOptions.NoLabel>;
	Phone2Type : PXFieldState<PXFieldOptions.NoLabel>;
	Phone2 : PXFieldState<PXFieldOptions.NoLabel>;
	FaxType : PXFieldState<PXFieldOptions.NoLabel>;
	Fax : PXFieldState<PXFieldOptions.NoLabel>;
	EMail : PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class LocationARAccountSub extends PXView  {

	CARAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CARSubID : PXFieldState<PXFieldOptions.CommitChanges>;
	CRetainageAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	CRetainageSubID : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class BCRoleAssignment extends PXView  {

	@columnConfig({ hideViewLink: true })
	ContactID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	Role : PXFieldState;
}

export class AddressLookupFilter extends PXView  {

	SearchAddress: PXFieldState<PXFieldOptions.NoLabel>;
	ViewName : PXFieldState;
	AddressLine1 : PXFieldState;
	AddressLine2 : PXFieldState;
	AddressLine3 : PXFieldState;
	City : PXFieldState;
	CountryID : PXFieldState;
	State : PXFieldState;
	PostalCode : PXFieldState;
	Latitude : PXFieldState;
	Longitude : PXFieldState;
}
