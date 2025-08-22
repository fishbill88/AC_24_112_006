import {
	PXView,
	PXFieldState,
	PXFieldOptions,

	createCollection,

	viewInfo,
	columnConfig,
	gridConfig,
	handleEvent,

	CustomEventType,
	RowCssHandlerArgs,
	GridPreset,
} from "client-controls";

import { PO302020 } from '../PO302020';

export interface PO302020_Return extends PO302020 { }
export class PO302020_Return {
	@viewInfo({ containerName: "Receipt Splits for Returning" })
	Returned = createCollection(ReceiptSplitsForReturn);

	@viewInfo({ containerName: "Receipt Non-Zero Splits for Returning" })
	ReturnedNotZero = createCollection(ReceiptSplitsForReturn);

	@handleEvent(CustomEventType.GetRowCss, { view: 'Returned' })
	getReturnedRowCss(args: RowCssHandlerArgs<ReceiptSplitsForReturn>) {
		const split = args?.selector?.row;

		if (split == null) {
			return undefined;
		}
		else if (split.ReceivedQty.value > split.Qty.value) {
			return 'startedLine excessedLine';
		}
		else if (split.ReceivedQty.value === split.Qty.value) {
			return 'startedLine completedLine';
		}
		else if (split.ReceivedQty.value > 0) {
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
export class ReceiptSplitsForReturn extends PXView {
	LineNbr : PXFieldState<PXFieldOptions.Disabled>;
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

	@columnConfig({ hideViewLink: true })
	UOM : PXFieldState<PXFieldOptions.Disabled>;
}
