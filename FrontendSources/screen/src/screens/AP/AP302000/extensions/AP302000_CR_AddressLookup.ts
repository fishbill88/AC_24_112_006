import { AP302000 } from '../AP302000';
import { PXView, PXFieldState, createSingle } from 'client-controls';

export interface AP302000_CR_AddressLookup extends AP302000 { }
export class AP302000_CR_AddressLookup {

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
