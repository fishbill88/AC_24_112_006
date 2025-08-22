import { CR303010 } from "../CR303010";
import {
	PXView,
	PXFieldState,
	createSingle,
	PXFieldOptions,
	viewInfo,
} from "client-controls";

export interface CR303010_General extends CR303010 {}
export class CR303010_General {
	@viewInfo({ containerName: "General" })
	Address = createSingle(Address);
	@viewInfo({ containerName: "General" })
	Contact = createSingle(Contact);

	@viewInfo({ containerName: "Address Lookup" })
	AddressLookupFilter = createSingle(AddressLookupFilter);
}

export class Address extends PXView {
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
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