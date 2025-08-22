import { CR304500 } from "../CR304500";
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

export interface CR304500_Shipping extends CR304500 {}
export class CR304500_Shipping {
	@viewInfo({ containerName: "Shipping" })
	Shipping_Address = createSingle(ShippingCRAddress);

	@viewInfo({ containerName: "Shipping" })
	Shipping_Contact = createSingle(ShippingCRContact);
}

export class ShippingCRAddress extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class ShippingCRContact extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1Type: PXFieldState;
	Phone1: PXFieldState;
	Phone2Type: PXFieldState;
	Phone2: PXFieldState;
	Email: PXFieldState;
}
