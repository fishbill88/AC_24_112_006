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

import { PO302020 } from '../PO302020';

export interface PO302020_Receive extends PO302020 { }
export class PO302020_Receive {
	@viewInfo({ containerName: "Receipt Splits for Receiving" })
	Received = createCollection(ReceiptSplitsForReceive);

	@viewInfo({ containerName: "Receipt Non-Zero Splits for Receiving" })
	ReceivedNotZero = createCollection(ReceiptSplitsForReceive);

	@handleEvent(CustomEventType.GetRowCss, { view: 'Received' })
	getReceivedRowCss(args: RowCssHandlerArgs<ReceiptSplitsForReceive>) {
		const split = args?.selector?.row;

		if (split == null) {
			return undefined;
		}
		else if (this.Document?.WMSSingleOrder?.value === true) {
			if (split.RestQty.value === 0) {
				return 'startedLine completedLine';
			}
			else if (split.ReceivedQty.value > 0) {
				return 'startedLine';
			}
		}
		else {
			if (split.ReceivedQty.value > split.Qty.value) {
				return 'startedLine excessedLine';
			}
			else if (split.ReceivedQty.value === split.Qty.value) {
				return 'startedLine completedLine';
			}
			else if (split.ReceivedQty.value > 0) {
				return 'startedLine';
			}
		}

		return undefined;
	}
}


@gridConfig({
	preset: GridPreset.Inquiry,
	suppressNoteFiles: true,
	allowUpdate: false,
})
export class ReceiptSplitsForReceive extends PXView {
	LineNbr : PXFieldState<PXFieldOptions.Disabled>;
	POReceiptLine__POType : PXFieldState<PXFieldOptions.Disabled>;

	@linkCommand("ViewOrder")
	POReceiptLine__PONbr : PXFieldState<PXFieldOptions.Disabled>;

	InventoryID : PXFieldState<PXFieldOptions.Disabled>;
	POReceiptLine__TranDesc : PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	LotSerialNbr : PXFieldState<PXFieldOptions.Disabled>;

	ExpireDate : PXFieldState<PXFieldOptions.Disabled>;
	SiteID : PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	LocationID : PXFieldState<PXFieldOptions.Disabled>;

	ReceivedQty : PXFieldState<PXFieldOptions.Disabled>;
	Qty : PXFieldState<PXFieldOptions.Disabled>;
	RestQty : PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	UOM : PXFieldState<PXFieldOptions.Disabled>;
}
