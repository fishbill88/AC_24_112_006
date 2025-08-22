import {
	PXView,
	PXFieldState,
	PXFieldOptions,

	createSingle,

	viewInfo,
} from 'client-controls';

import { SO101000 } from '../SO101000';

export interface SO101000_PickPackShip extends SO101000 {}
export class SO101000_PickPackShip {
	@viewInfo({containerName: "Warehouse Management"})
	PickPackShipSetup = createSingle(PickPackShipSetup);
}

export class PickPackShipSetup extends PXView {
	ShowPickTab: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowPackTab: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowShipTab: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowReturningTab: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowScanLogTab: PXFieldState;

	ShortShipmentConfirmation: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipmentLocationOrdering: PXFieldState<PXFieldOptions.CommitChanges>;
	UseDefaultQty: PXFieldState<PXFieldOptions.CommitChanges>;
	ExplicitLineConfirmation: PXFieldState;
	UseCartsForPick: PXFieldState;
	DefaultLocationFromShipment: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultLotSerialFromShipment: PXFieldState<PXFieldOptions.CommitChanges>;
	EnterSizeForPackages: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintShipmentConfirmation: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintShipmentLabels: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintCommercialInvoices: PXFieldState<PXFieldOptions.CommitChanges>;
	ConfirmEachPackageWeight: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestLocationForEachItem: PXFieldState<PXFieldOptions.CommitChanges>;
	ConfirmToteForEachItem: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowMultipleTotesPerShipment: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintPickListsAndPackSlipsTogether: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowBidirectionalPickLists: PXFieldState<PXFieldOptions.CommitChanges>;
}
