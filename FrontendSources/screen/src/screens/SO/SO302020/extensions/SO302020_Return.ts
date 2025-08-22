import {
	PXView,
	PXFieldState,
	PXFieldOptions,

	createCollection,

	viewInfo,
	columnConfig,
	gridConfig,
	handleEvent,
	linkCommand,

	CustomEventType,
	RowCssHandlerArgs,
	GridPreset,
} from "client-controls";

import { SO302020 } from '../SO302020';

export interface SO302020_Return extends SO302020 { }
export class SO302020_Return {
	@viewInfo({ containerName: "Shipment Splits for Returning" })
	Returned = createCollection(ShipmentSplitsForReturn);

	@handleEvent(CustomEventType.GetRowCss, { view: 'Returned' })
	getReturnedRowCss(args: RowCssHandlerArgs<ShipmentSplitsForReturn>) {
		const shipLine = args?.selector?.row;

		if (shipLine == null) {
			return undefined;
		}
		else if (shipLine.PickedQty.value > shipLine.Qty.value) {
			return 'startedLine excessedLine';
		}
		else if (shipLine.PickedQty.value === shipLine.Qty.value) {
			return 'startedLine completedLine';
		}
		else if (shipLine.PickedQty.value > 0) {
			return 'startedLine';
		}

		return undefined;
	}
}

@gridConfig({
	preset: GridPreset.Inquiry,
	suppressNoteFiles: true,
	allowUpdate: false,
})
export class ShipmentSplitsForReturn extends PXView {
	Fits: PXFieldState<PXFieldOptions.Disabled>;
	LineNbr: PXFieldState<PXFieldOptions.Disabled>;
	SOShipLine__OrigOrderType: PXFieldState;

	@linkCommand("ViewOrder")
	SOShipLine__OrigOrderNbr: PXFieldState;

	SiteID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState<PXFieldOptions.Disabled>;

	InventoryID: PXFieldState<PXFieldOptions.Disabled>;
	SOShipLine__TranDesc: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState<PXFieldOptions.Disabled>;

	ExpireDate: PXFieldState<PXFieldOptions.Disabled>;
	PickedQty: PXFieldState<PXFieldOptions.Disabled>;
	Qty: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.Disabled>;

	SOShipLine__IsFree: PXFieldState;
}
