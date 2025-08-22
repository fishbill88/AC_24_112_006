import { CR304000 } from "../CR304000";
import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createCollection,
	gridConfig,
	viewInfo,
	PXActionState,
	columnConfig,
	createSingle,
} from "client-controls";

export interface CR304000_Shipping extends CR304000 {}
export class CR304000_Shipping {
	@viewInfo({ containerName: "Shipping" })
	Shipping_Address = createSingle(CRAddress3);
	@viewInfo({ containerName: "Shipping" })
	Shipping_Contact = createSingle(CRContact3);
}

export class CRAddress3 extends PXView {
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

export class CRContact3 extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1Type: PXFieldState;
	Phone1: PXFieldState;
	Phone2Type: PXFieldState;
	Phone2: PXFieldState;
	Email: PXFieldState;
}
