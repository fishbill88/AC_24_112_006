import { CR304500 } from "../CR304500";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	gridConfig,
	viewInfo,
	PXActionState,
	GridPreset
} from "client-controls";

export interface CR304500_Estimates extends CR304500 {}
export class CR304500_Estimates {
	@viewInfo({ containerName: "Estimates" })
	OpportunityEstimateRecords = createCollection(AMEstimateReference);
}

@gridConfig({ 
	preset: GridPreset.Details,
	initNewRow: true,
	allowDelete: true
})
export class AMEstimateReference extends PXView {
	AddEstimate: PXActionState;
	QuickEstimate: PXActionState;
	RemoveEstimate: PXActionState;

	AMEstimateItem__BranchID: PXFieldState;
	AMEstimateItem__InventoryCD: PXFieldState;
	AMEstimateItem__ItemDesc: PXFieldState;
	AMEstimateItem__SiteID: PXFieldState;
	AMEstimateItem__UOM: PXFieldState;
	OrderQty: PXFieldState;
	CuryUnitPrice: PXFieldState;
	CuryExtPrice: PXFieldState;
	@linkCommand("ViewEstimate")
	EstimateID: PXFieldState;
	RevisionID: PXFieldState;
	TaxCategoryID: PXFieldState;
	AMEstimateItem__OwnerID: PXFieldState;
	AMEstimateItem__EngineerID: PXFieldState;
	AMEstimateItem__RequestDate: PXFieldState;
	AMEstimateItem__PromiseDate: PXFieldState;
	AMEstimateItem__EstimateClassID: PXFieldState;
}
