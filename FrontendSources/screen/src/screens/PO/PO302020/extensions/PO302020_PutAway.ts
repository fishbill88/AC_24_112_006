import {
	PXView,
	PXActionState,
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

export interface PO302020_PutAway extends PO302020 { }
export class PO302020_PutAway {
	@viewInfo({ containerName: "Receipt Splits for Put Away" })
	PutAway = createCollection(ReceiptSplitsForPutAway);

	@viewInfo({ containerName: "Transfer Allocations" })
	TransferSplitLinks = createCollection(TransferSplitLinks);

	@viewInfo({ containerName: "Related Transfers" })
	RelatedTransfers = createCollection(RelatedTransfers);

	@handleEvent(CustomEventType.GetRowCss, { view: 'PutAway' })
	getPutAwayRowCss(args: RowCssHandlerArgs<ReceiptSplitsForPutAway>) {
		const split = args?.selector?.row;

		if (split == null) {
			return undefined;
		}
		else if (split.PutAwayQty.value > split.Qty.value) {
			return 'startedLine excessedLine';
		}
		else if (split.PutAwayQty.value === split.Qty.value) {
			return 'startedLine completedLine';
		}
		else if (split.PutAwayQty.value > 0) {
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
export class ReceiptSplitsForPutAway extends PXView {
	ViewTransferInfo: PXActionState;

	Fits: PXFieldState<PXFieldOptions.Disabled>;
	LineNbr: PXFieldState<PXFieldOptions.Disabled>;
	POReceiptLine__POType: PXFieldState<PXFieldOptions.Disabled>;

	@linkCommand("ViewOrder")
	POReceiptLine__PONbr: PXFieldState<PXFieldOptions.Disabled>;

	InventoryID: PXFieldState<PXFieldOptions.Disabled>;
	POReceiptLine__TranDesc: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState<PXFieldOptions.Disabled>;

	ExpireDate: PXFieldState<PXFieldOptions.Disabled>;
	SiteID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	ToLocationID: PXFieldState<PXFieldOptions.Disabled>;

	PutAwayQty: PXFieldState<PXFieldOptions.Disabled>;
	CartQty: PXFieldState<PXFieldOptions.Disabled>;
	OverallCartQty: PXFieldState<PXFieldOptions.Disabled>;
	Qty: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	suppressNoteFiles: true,
	allowUpdate: false,
})
export class TransferSplitLinks extends PXView {
	TransferRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	INTran__InventoryID: PXFieldState<PXFieldOptions.Disabled>;
	INTran__SubItemID: PXFieldState<PXFieldOptions.Disabled>;
	INTranSplit__LocationID: PXFieldState<PXFieldOptions.Disabled>;
	INTran__ToLocationID: PXFieldState<PXFieldOptions.Disabled>;
	INTranSplit__LotSerialNbr: PXFieldState<PXFieldOptions.Disabled>;
	Qty: PXFieldState<PXFieldOptions.Disabled>;
	INTranSplit__UOM: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	suppressNoteFiles: true,
	allowUpdate: false,
})
export class RelatedTransfers extends PXView {
	RefNbr: PXFieldState<PXFieldOptions.Disabled>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	TransferType: PXFieldState<PXFieldOptions.Disabled>;
	TranDate: PXFieldState<PXFieldOptions.Disabled>;
	FinPeriodID: PXFieldState<PXFieldOptions.Disabled>;
	SiteID: PXFieldState<PXFieldOptions.Disabled>;
	TotalQty: PXFieldState<PXFieldOptions.Disabled>;
	BatchNbr: PXFieldState<PXFieldOptions.Disabled>;
}
