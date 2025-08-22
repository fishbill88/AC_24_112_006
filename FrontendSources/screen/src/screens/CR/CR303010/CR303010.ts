import {
	PXView,
	PXFieldState,
	graphInfo,
	PXScreen,
	createSingle,
	PXFieldOptions,
	viewInfo,
	PXActionState,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.AccountLocationMaint",
	primaryView: "Location",
	bpEventsIndicator: false,
	udfTypeField: "ClassID",
})
export class CR303010 extends PXScreen {
	AddressLookup: PXActionState;
	ViewOnMap: PXActionState;
	AddressLookupSelectAction: PXActionState;

	@viewInfo({ containerName: "Location Summary" })
	Location = createSingle(Location);
}

export class Location extends PXView {
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationCD: PXFieldState;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	IsDefault: PXFieldState;
}
