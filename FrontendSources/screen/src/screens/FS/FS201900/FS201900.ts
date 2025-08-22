import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.GeoZoneMaint', primaryView: 'GeoZoneRecords' })
export class FS201900 extends PXScreen {
	GeoZoneRecords = createSingle(FSGeoZone)
	GeoZoneEmpRecords = createCollection(FSGeoZone_Employee);
	GeoZonePostalCodeRecords = createCollection(FSGeoZone_PostalCode);
}

export class FSGeoZone extends PXView {
	GeoZoneCD: PXFieldState;
	Descr: PXFieldState;
	CountryID: PXFieldState;
}

export class FSGeoZone_Employee extends PXView {
	EmployeeID: PXFieldState;
	EmployeeID_EPEmployee_acctName: PXFieldState;
}

export class FSGeoZone_PostalCode extends PXView {
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
}
