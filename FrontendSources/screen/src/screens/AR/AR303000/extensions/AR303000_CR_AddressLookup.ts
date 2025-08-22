import { AR303000 } from '../AR303000';
import { PXView, PXFieldState, PXFieldOptions, createSingle } from 'client-controls';

export interface AR303000_CR_AddressLookup extends AR303000 { }
export class AR303000_CR_AddressLookup {

	AddressLookupFilter = createSingle(AddressLookupFilter);

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
