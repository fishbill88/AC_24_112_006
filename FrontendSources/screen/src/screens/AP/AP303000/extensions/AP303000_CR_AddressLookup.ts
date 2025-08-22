import { AP303000 } from '../AP303000';
import { PXView, PXFieldState, PXFieldOptions, createSingle } from 'client-controls';

export interface AP303000_CR_AddressLookup extends AP303000 { }
export class AP303000_CR_AddressLookup {

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
