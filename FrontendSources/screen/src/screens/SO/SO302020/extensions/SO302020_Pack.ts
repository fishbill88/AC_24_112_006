import {
	PXView,
	PXViewCollection,
	PXFieldState,
	PXFieldOptions,
	PXActionState,

	createSingle,
	createCollection,

	viewInfo,
	columnConfig,
	gridConfig,
	handleEvent,
	linkCommand,

	GridPagerMode,
	CustomEventType,
	RowCssHandlerArgs,
	RowSelectedHandlerArgs,
	GridPreset,
} from "client-controls";

import { SO302020 } from '../SO302020';

export interface SO302020_Pack extends SO302020 { }
export class SO302020_Pack {
	@viewInfo({ containerName: "Shipment Splits for Packing" })
	PickedForPack = createCollection(ShipmentSplitsForPack);

	@viewInfo({ containerName: "Selected Package" })
	ShownPackage = createSingle(SelectedPackageForPack);

	@viewInfo({ containerName: "Package Content" })
	Packed = createCollection(PackageContent);

	@handleEvent(CustomEventType.GetRowCss, { view: 'PickedForPack' })
	getPickedForPackRowCss(args: RowCssHandlerArgs<ShipmentSplitsForPack>) {
		const shipLine = args?.selector?.row;

		if (shipLine == null) {
			return undefined;
		}
		else if (shipLine.PickedQty.value > 0 && shipLine.PackedQty.value > ((<SO302020>args.screenModel).Setup.ShowPickTab.value === false ? shipLine.Qty.value : shipLine.PickedQty.value)) {
			return 'startedLine excessedLine';
		}
		else if (shipLine.PickedQty.value > 0 && shipLine.PackedQty.value === ((<SO302020>args.screenModel).Setup.ShowPickTab.value === false ? shipLine.Qty.value : shipLine.PickedQty.value)) {
			return 'startedLine completedLine';
		}
		else if (shipLine.PackedQty.value > 0) {
			return 'startedLine';
		}

		return undefined;
	}

	@handleEvent(CustomEventType.RowSelected, { view: 'PickedForPack' })
	onPickedForPackSelected(args: RowSelectedHandlerArgs<PXViewCollection<ShipmentSplitsForPack>>) {
		const model = (<any>args.viewModel as ShipmentSplitsForPack); // todo: think about a better solution

		if (model?.ReopenLineQty) {
			model.ReopenLineQty.enabled = !!args.viewModel.activeRow?.RelatedPickListSplitForceCompleted.value;
		}
	}
}

@gridConfig({
	preset: GridPreset.Inquiry,
	suppressNoteFiles: true,
	allowUpdate: false,
})
export class ShipmentSplitsForPack extends PXView {
	ReopenLineQty: PXActionState;

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
	CartQty: PXFieldState<PXFieldOptions.Disabled>;
	OverAllCartQty: PXFieldState<PXFieldOptions.Disabled>;
	PickedQty: PXFieldState<PXFieldOptions.Disabled>;
	PackedQty: PXFieldState<PXFieldOptions.Disabled>;
	Qty: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.Disabled>;

	SOShipLine__IsFree: PXFieldState;
	RelatedPickListSplitForceCompleted: PXFieldState<PXFieldOptions.Disabled>;
}

export class SelectedPackageForPack extends PXView {
	Confirmed: PXFieldState<PXFieldOptions.Disabled>;
	Weight: PXFieldState<PXFieldOptions.Disabled>;
	MaxWeight: PXFieldState<PXFieldOptions.Disabled>;
	WeightUOM: PXFieldState<PXFieldOptions.Disabled>;
	PackageDimensionsCombined: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	pagerMode: GridPagerMode.InfiniteScroll,
	suppressNoteFiles: true,
	allowUpdate: false,
})
export class PackageContent extends PXView {
	LineNbr: PXFieldState<PXFieldOptions.Disabled>;
	InventoryID: PXFieldState<PXFieldOptions.Disabled>;
	SOShipLine__TranDesc: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState<PXFieldOptions.Disabled>;

	PackedQtyPerBox: PXFieldState<PXFieldOptions.Disabled>;
	Qty: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.Disabled>;

	RelatedPickListSplitForceCompleted: PXFieldState<PXFieldOptions.Disabled>;
}
