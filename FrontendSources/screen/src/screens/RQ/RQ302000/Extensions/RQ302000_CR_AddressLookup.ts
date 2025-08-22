import { RQ302000 } from '../RQ302000';
import { PXView, PXFieldState, PXFieldOptions, createSingle, viewInfo } from 'client-controls';

export interface RQ302000_CR_AddressLookup extends RQ302000 { }
export class RQ302000_CR_AddressLookup {
	@viewInfo({containerName: "AddressLookupFilter"})
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
