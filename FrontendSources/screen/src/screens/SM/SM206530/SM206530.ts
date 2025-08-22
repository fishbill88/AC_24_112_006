import {
	createCollection,
	PXScreen,
	graphInfo,
	gridConfig,
	PXView,
	PXFieldState
} from "client-controls";

@graphInfo({graphType: "PX.SM.ScaleMaint", primaryView: "Scale"})
export class SM206530 extends PXScreen {

	Scale = createCollection(SMScale);
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	adjustPageSize: true
})
export class SMScale extends PXView  {
	DeviceHubID : PXFieldState;
	ScaleID : PXFieldState;
	Descr : PXFieldState;
	CompanyLastWeight : PXFieldState;
	CompanyUOM : PXFieldState;
	LastWeight : PXFieldState;
	UOM : PXFieldState;
	LastModifiedDateTime : PXFieldState;
}
