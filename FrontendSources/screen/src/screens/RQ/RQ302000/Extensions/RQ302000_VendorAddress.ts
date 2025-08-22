import {
	RQ302000
} from '../RQ302000';

import {
	PXView,
	createCollection,
	PXFieldState,
	PXFieldOptions,
	featureInstalled,
	createSingle,
	gridConfig,
	columnConfig,
	viewInfo
} from 'client-controls';

export interface RQ302000_VendorAddress extends RQ302000 { }
export class RQ302000_VendorAddress {
	@viewInfo({containerName: "Vendor Contact"})
	Bidding_Remit_Contact = createSingle(POContact);
   	@viewInfo({containerName: "Vendor Address"})
	Bidding_Remit_Address = createSingle(POAddress);
}

export class POContact extends PXView  {
	OverrideContact : PXFieldState<PXFieldOptions.CommitChanges>;
	FullName : PXFieldState;
	Attention : PXFieldState;
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


