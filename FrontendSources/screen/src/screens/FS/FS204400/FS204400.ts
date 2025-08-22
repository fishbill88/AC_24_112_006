import {
	graphInfo,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	createSingle
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.ManufacturerMaint', primaryView: 'ManufacturerRecords' })
export class FS204400 extends PXScreen {
	AddressLookup: PXActionState;
	ViewMainOnMap: PXActionState;
	ManufacturerRecords = createSingle(FSManufacturer);
	CurrentManufacturer = createSingle(FSManufacturer);
	Manufacturer_Address = createSingle(FSAddress);
	Manufacturer_Contact = createSingle(FSContact);
}

export class FSContact extends PXView {
	FullName: PXFieldState<PXFieldOptions.CommitChanges>;
	Attention: PXFieldState;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2: PXFieldState<PXFieldOptions.CommitChanges>;
	FaxType: PXFieldState<PXFieldOptions.CommitChanges>;
	Fax: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSAddress extends PXView {
	AddressLine1: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine2: PXFieldState<PXFieldOptions.CommitChanges>;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSManufacturer extends PXView {
	AllowOverrideContactAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	ManufacturerCD: PXFieldState;
	Descr: PXFieldState;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
}
