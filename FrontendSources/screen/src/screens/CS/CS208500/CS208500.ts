import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	viewInfo,
	columnConfig,
	gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CS.FOBPointMaint', primaryView: 'FOBPoint', hideFilesIndicator: true, hideNotesIndicator: true })
export class CS208500 extends PXScreen {

	@viewInfo({ containerName: "FOB Points" })
	FOBPoint = createCollection(FOBPoint);
}

@gridConfig({
	mergeToolbarWith: 'ScreenToolbar'
})
export class FOBPoint extends PXView {
	@columnConfig({ hideViewLink: true })
	FOBPointID: PXFieldState;
	Description: PXFieldState;
}
