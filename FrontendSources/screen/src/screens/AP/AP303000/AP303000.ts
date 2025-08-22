import {
	ChangeIDParam, Vendor, VendorBalanceSummary, Address, ContactInfo, ContactInfoForGrid, Location, LocationForGrid, VendorPaymentMethodDetail,
	VendorBalanceSummaryByBaseCurrency, CSAnswers, CRActivity, NotificationSource, NotificationRecipient,
	ComplianceDocument
} from './views';
import { PXActionState, PXScreen, createSingle, createCollection, graphInfo, viewInfo } from "client-controls";

@graphInfo({ graphType: 'PX.Objects.AP.VendorMaint', primaryView: 'BAccount', bpEventsIndicator: true, udfTypeField: 'VendorClassID', showUDFIndicator: true })
export class AP303000 extends PXScreen {

	AddressLookup: PXActionState;
	ViewMainOnMap: PXActionState;
	NewMailActivity: PXActionState;
	ContactTeamsCardOffline: PXActionState;
	ContactTeamsCardAvailable: PXActionState;
	ContactTeamsCardBusy: PXActionState;
	ContactTeamsCardAway: PXActionState;
	RemitAddressLookup: PXActionState;
	ViewRemitOnMap: PXActionState;
	DefLocationAddressLookup: PXActionState;
	ViewDefLocationAddressOnMap: PXActionState;
	CreateContactCancel: PXActionState;
	CreateContactFinish: PXActionState;
	CreateContactFinishRedirect: PXActionState;
	AddressLookupSelectAction: PXActionState;
	StatusIconContactOffline: PXActionState;
	StatusIconContactAvailable: PXActionState;
	StatusIconContactBusy: PXActionState;
	StatusIconContactAway: PXActionState;
	ContactChat: PXActionState;
	ContactCall: PXActionState;
	ContactMeeting: PXActionState;
	StatusIconOwnerOffline: PXActionState;
	StatusIconOwnerAvailable: PXActionState;
	StatusIconOwnerBusy: PXActionState;
	StatusIconOwnerAway: PXActionState;
	OwnerChat: PXActionState;
	OwnerCall: PXActionState;
	OwnerMeeting: PXActionState;
	ViewBalanceDetails: PXActionState;

	@viewInfo({containerName: "Specify New ID"})
	ChangeIDDialog = createSingle(ChangeIDParam);

	@viewInfo({containerName: "Vendor Summary"})
	BAccount = createSingle(Vendor);

	@viewInfo({containerName: "Vendor Summary"})
	VendorBalance = createSingle(VendorBalanceSummary);

	CurrentVendor = createSingle(Vendor);

	@viewInfo({containerName: "General"})
	DefAddress = createSingle(Address);

	@viewInfo({containerName: "General"})
	DefContact = createSingle(ContactInfo);

	@viewInfo({containerName: "General"})
	PrimaryContactCurrent = createSingle(ContactInfo);

	@viewInfo({containerName: "Payment"})
	DefLocation = createSingle(Location);

	@viewInfo({containerName: "Payment"})
	RemitAddress = createSingle(Address);

	@viewInfo({containerName: "Payment"})
	RemitContact = createSingle(ContactInfo);

	@viewInfo({containerName: "Payment Instructions"})
	PaymentDetails = createCollection(VendorPaymentMethodDetail);

	@viewInfo({containerName: "Purchase Settings"})
	DefLocationAddress = createSingle(Address);

	@viewInfo({containerName: "Purchase Settings"})
	DefLocationContact = createSingle(ContactInfo);

	@viewInfo({containerName: "Balances"})
	VendorBalanceByBaseCurrency = createCollection(VendorBalanceSummaryByBaseCurrency);

	@viewInfo({containerName: "Attributes"})
	Answers = createCollection(CSAnswers);

	@viewInfo({containerName: "Locations"})
	Locations = createCollection(LocationForGrid);

	@viewInfo({containerName: "Contacts"})
	Contacts = createCollection(ContactInfoForGrid);

	@viewInfo({containerName: "Activities"})
	Activities = createCollection(CRActivity);

	@viewInfo({containerName: "Mailings"})
	NotificationSources = createCollection(NotificationSource);

	@viewInfo({containerName: "Recipients"})
	NotificationRecipients = createCollection(NotificationRecipient);

	@viewInfo({containerName: "Supplied-by Vendors"})
	SuppliedByVendors = createCollection(Vendor);

	@viewInfo({containerName: "Compliance"})
	ComplianceDocuments = createCollection(ComplianceDocument);

}
