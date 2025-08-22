import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
	PXPageLoadBehavior,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	linkCommand,
	GridPreset
} from "client-controls";

@graphInfo({graphType: "PX.Objects.SO.SOPickingEfficiencyEnq", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class SO402020 extends PXScreen {
	ViewDocument: PXActionState;

	@viewInfo({containerName: "Filter"})
	Filter = createSingle(EfficiencyFilter);
   	@viewInfo({containerName: "Efficiency"})
	Efficiency = createCollection(PickingEfficiency);
}

export class EfficiencyFilter extends PXView  {
	SiteID : PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate : PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate : PXFieldState<PXFieldOptions.CommitChanges>;
	ExpandByDay : PXFieldState<PXFieldOptions.CommitChanges>;
	ExpandByUser : PXFieldState<PXFieldOptions.CommitChanges>;
	UserID : PXFieldState<PXFieldOptions.CommitChanges>;
	ExpandByShipment : PXFieldState<PXFieldOptions.CommitChanges>;
	StandaloneShipmentNbr : PXFieldState<PXFieldOptions.CommitChanges>;
	DocType : PXFieldState<PXFieldOptions.CommitChanges>;
	ShipmentNbr : PXFieldState<PXFieldOptions.CommitChanges>;
	WorksheetNbr : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	mergeToolbarWith: "ScreenToolbar",
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false
})
export class PickingEfficiency extends PXView  {
	Day : PXFieldState;
	StartDate : PXFieldState;
	EndDate : PXFieldState;
	TotalTime : PXFieldState;
	JobType : PXFieldState;
	UserID : PXFieldState;
	SiteID : PXFieldState;
	@linkCommand("ViewDocument")
	PickListNbr : PXFieldState;
	NumberOfShipments : PXFieldState;
	NumberOfLines : PXFieldState;
	NumberOfPackages : PXFieldState;
	NumberOfInventories : PXFieldState;
	NumberOfLocations : PXFieldState;
	TotalQty : PXFieldState;
	NumberOfUsefulOperations : PXFieldState;
	NumberOfScans : PXFieldState;
	NumberOfFailedScans : PXFieldState;
	EffectiveTime : PXFieldState;
	Efficiency : PXFieldState;
}