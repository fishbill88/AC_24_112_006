import { CR304000 } from "../CR304000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	viewInfo,
	createSingle,
} from "client-controls";

export interface CR304000_Contact extends CR304000 {}
export class CR304000_Contact {
	@viewInfo({ containerName: "Contact" })
	Opportunity_Contact = createSingle(CRContact);

	@viewInfo({ containerName: "Contact" })
	Opportunity_Address = createSingle(CRAddress);
}

export class CRContact extends PXView {
	FirstName: PXFieldState<PXFieldOptions.CommitChanges>;
	LastName: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState<PXFieldOptions.CommitChanges>;
	Salutation: PXFieldState<PXFieldOptions.CommitChanges>;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone3Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone3: PXFieldState;
	FaxType: PXFieldState<PXFieldOptions.CommitChanges>;
	Fax: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentAgreement: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ConsentExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CRAddress extends PXView {
	AddressLine1: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine2: PXFieldState<PXFieldOptions.CommitChanges>;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}
