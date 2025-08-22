import {
	RQ503000
} from '../RQ503000';

import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createSingle,
	viewInfo
} from 'client-controls';

export interface RQ302000_VendorAddress extends RQ503000 { }
export class RQ302000_VendorAddress {
	@viewInfo({containerName: "Vendor Contact"})
	Remit_Contact = createSingle(POContact);
   	@viewInfo({containerName: "Vendor Address"})
	Remit_Address = createSingle(POAddress);
}

export class POContact extends PXView  {
	OverrideContact : PXFieldState<PXFieldOptions.CommitChanges>;
	FullName : PXFieldState;
	Salutation : PXFieldState;
	Phone1 : PXFieldState;
	Email : PXFieldState;
}

export class POAddress extends PXView  {
	OverrideAddress : PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1 : PXFieldState;
	AddressLine2 : PXFieldState;
	City : PXFieldState;
	CountryID : PXFieldState<PXFieldOptions.CommitChanges>;
	State : PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode : PXFieldState<PXFieldOptions.CommitChanges>;
}
