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
	linkCommand,
	GridPreset,
} from "client-controls";

export interface CR304000_Estimates extends CR304000 {}
export class CR304000_Estimates {
	@viewInfo({ containerName: "Estimates" })
	OpportunityEstimateRecords = createCollection(AMEstimateReference);
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	topBarItems: {
		AddEstimate: {
			index: 2,
			config: {
				commandName: "AddEstimate",
				text: "Add",
			},
		},
		QuickEstimate: {
			index: 3,
			config: {
				commandName: "QuickEstimate",
				text: "Quick Estimate",
			},
		},
		RemoveEstimate: {
			index: 4,
			config: {
				commandName: "RemoveEstimate",
				text: "Remove",
			},
		},
	},
})
export class AMEstimateReference extends PXView {
	AddEstimate: PXActionState;
	QuickEstimate: PXActionState;
	RemoveEstimate: PXActionState;

	@columnConfig({ allowUpdate: false })
	AMEstimateItem__BranchID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AMEstimateItem__InventoryCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AMEstimateItem__ItemDesc: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AMEstimateItem__SiteID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AMEstimateItem__UOM: PXFieldState;
	@columnConfig({ allowUpdate: false })
	OrderQty: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryUnitPrice: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryExtPrice: PXFieldState;
	@linkCommand("ViewEstimate")
	@columnConfig({ allowUpdate: false })
	EstimateID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	RevisionID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TaxCategoryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AMEstimateItem__OwnerID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AMEstimateItem__EngineerID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AMEstimateItem__RequestDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AMEstimateItem__PromiseDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AMEstimateItem__EstimateClassID: PXFieldState;
}
