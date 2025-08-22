import {
	createCollection,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	gridConfig,
	columnConfig
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.CS.CSBoxMaint', primaryView: 'Records'})
export class CS207600 extends PXScreen {

	@viewInfo({containerName: 'Boxes'})
	Records = createCollection(CSBox);
}

@gridConfig({
	adjustPageSize: true,
	mergeToolbarWith: 'ScreenToolbar'
})
export class CSBox extends PXView  {
	BoxID: PXFieldState;
	Description: PXFieldState;
	BoxWeight: PXFieldState;
	MaxWeight: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CommonSetup__WeightUOM: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CommonSetup__VolumeUOM: PXFieldState;
	Length: PXFieldState;
	Width: PXFieldState;
	Height: PXFieldState;
	MaxVolume: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CommonSetup__LinearUOM: PXFieldState;
	CarrierBox: PXFieldState;
	AllowOverrideDimension: PXFieldState;
	ActiveByDefault: PXFieldState;
}