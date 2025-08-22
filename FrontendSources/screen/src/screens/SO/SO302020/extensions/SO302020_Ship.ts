import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,

	createSingle,
	createCollection,

	viewInfo,
	gridConfig,

	GridPagerMode,
	GridPreset,
} from "client-controls";

import { SO302020 } from '../SO302020';

export interface SO302020_Ship extends SO302020 { }
export class SO302020_Ship {
	@viewInfo({ containerName: "Shipping Address" })
	Shipping_Address = createSingle(ShippingAddress);

	@viewInfo({ containerName: "Shipment Totals" })
	CurrentDocument = createSingle(ShipmentTotals);

	@viewInfo({ containerName: "Carrier Rates" })
	CarrierRates = createCollection(CarrierRates);

	@viewInfo({ containerName: "Packages" })
	Packages = createCollection(ShipmentPackages);
}

export class ShippingAddress extends PXView {
	AddressLine1: PXFieldState<PXFieldOptions.Disabled>;
	AddressLine2: PXFieldState<PXFieldOptions.Disabled>;
	City: PXFieldState<PXFieldOptions.Disabled>;
	CountryID: PXFieldState<PXFieldOptions.Disabled>;
	State: PXFieldState<PXFieldOptions.Disabled>;
	PostalCode: PXFieldState<PXFieldOptions.Disabled>;
}

export class ShipmentTotals extends PXView {
	ShipmentQty: PXFieldState<PXFieldOptions.Disabled>;
	ShipmentWeight: PXFieldState<PXFieldOptions.Disabled>;
	ShipmentVolume: PXFieldState<PXFieldOptions.Disabled>;
	PackageCount: PXFieldState<PXFieldOptions.Disabled>;
	PackageWeight: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	pagerMode: GridPagerMode.InfiniteScroll,
	suppressNoteFiles: true,
	allowUpdate: false,
})
export class CarrierRates extends PXView {
	ScanRefreshRates: PXActionState;
	ScanGetLabels: PXActionState;

	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	Method: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState<PXFieldOptions.Disabled>;
	Amount: PXFieldState<PXFieldOptions.Disabled>;
	DaysInTransit: PXFieldState<PXFieldOptions.Disabled>;
	DeliveryDate: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	pagerMode: GridPagerMode.InfiniteScroll,
	suppressNoteFiles: true,
	allowUpdate: false,
})
export class ShipmentPackages extends PXView {
	BoxID: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState<PXFieldOptions.Disabled>;
	AllowOverrideDimension: PXFieldState<PXFieldOptions.Disabled>;
	LinearUOM: PXFieldState<PXFieldOptions.Disabled>;
	Length: PXFieldState<PXFieldOptions.Disabled>;
	Width: PXFieldState<PXFieldOptions.Disabled>;
	Height: PXFieldState<PXFieldOptions.Disabled>;
	WeightUOM: PXFieldState<PXFieldOptions.Disabled>;
	Weight: PXFieldState<PXFieldOptions.Disabled>;
	BoxWeight: PXFieldState<PXFieldOptions.Disabled>;
	NetWeight: PXFieldState<PXFieldOptions.Disabled>;
	MaxWeight: PXFieldState<PXFieldOptions.Disabled>;
	DeclaredValue: PXFieldState<PXFieldOptions.Disabled>;
	COD: PXFieldState<PXFieldOptions.Disabled>;
	TrackNumber: PXFieldState<PXFieldOptions.Disabled>;
	StampsAddOns: PXFieldState<PXFieldOptions.Disabled>;
}
