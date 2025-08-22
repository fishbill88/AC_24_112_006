import { autoinject } from 'aurelia-framework';
import {
	commitChanges,
	createCollection,
	createSingle,
	graphInfo,
	PXFieldState,
	PXView,
	PXScreen,
	PXFieldOptions,
	gridConfig,
	columnConfig,
} from "client-controls";

export class Filter extends PXView {
	@commitChanges CarrierID: PXFieldState;
	@commitChanges ShipDate: PXFieldState;
	@commitChanges PrintWithDeviceHub: PXFieldState;
	@commitChanges PrinterID: PXFieldState;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	mergeToolbarWith: 'ScreenToolbar'
})
export class SOShipment extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowCheckAll: true, allowNull: false }) Selected: PXFieldState;
	ShipmentNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) CustomerID: PXFieldState;
	CustomerID_description: PXFieldState;
	@columnConfig({ hideViewLink: true }) ShipVia: PXFieldState;
}

@graphInfo({
	graphType: 'PX.ExternalCarriersHelper.SOCreateShipmentManifestProcess',
	primaryView: 'Filter'
})
@autoinject
export class SO506000 extends PXScreen {

	Filter = createSingle(Filter);

	ShipmentList = createCollection(SOShipment);
}
