import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	viewInfo,
	gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CS.ShippingZoneMaint', primaryView: 'ShippingZones' })
export class CS207510 extends PXScreen {

	@viewInfo({ containerName: "Shipping Zones" })
	ShippingZones = createCollection(ShippingZones);
}

@gridConfig({
	mergeToolbarWith: 'ScreenToolbar'
})
export class ShippingZones extends PXView {
	ZoneID: PXFieldState;
	Description: PXFieldState;
}