import { AP302000 } from '../AP302000';
import { PXView, createCollection, PXFieldState, PXFieldOptions, featureInstalled, createSingle, gridConfig, GridPreset } from 'client-controls';

export interface AP302000_PO_PurchaseOrders extends AP302000 { }

@featureInstalled("PX.Objects.CS.FeaturesSet+DistributionModule")
export class AP302000_PO_PurchaseOrders {

	// PX.Objects.PO.GraphExtensions.APPaymentEntryExt.POAdjustExtension

	POAdjustments = createCollection(POAdjustments);
	LoadOrders = createSingle(LoadOrders);

}

@gridConfig({ preset: GridPreset.Details })
export class POAdjustments extends PXView {

	AdjOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	AdjOrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	AdjRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	POOrder__Status: PXFieldState;
	CuryAdjgAmt: PXFieldState;
	CuryAdjgBilledAmt: PXFieldState;
	POOrder__OrderDate: PXFieldState;
	POOrder__CuryUnprepaidTotal: PXFieldState;
	POOrder__CuryLineTotal: PXFieldState;
	POOrder__CuryID: PXFieldState;
	Released: PXFieldState;
	IsRequest: PXFieldState;

}

export class LoadOrders extends PXView {

	BranchID: PXFieldState;
	FromDate: PXFieldState;
	StartOrderNbr: PXFieldState;
	MaxNumberOfDocuments: PXFieldState;
	OrderBy: PXFieldState;
	ToDate: PXFieldState;
	EndOrderNbr: PXFieldState;

}
