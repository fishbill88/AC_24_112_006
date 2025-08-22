import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
} from 'client-controls';

export class ScanHeader extends PXView {
	Barcode: PXFieldState;
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Message: PXFieldState<PXFieldOptions.Disabled>;
	Mode: PXFieldState;
}

export class ScanInfo extends PXView {
	Mode: PXFieldState;
	Message: PXFieldState;
	MessageSoundFile: PXFieldState;
	Prompt: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMMTran extends PXView {
	LaborType: PXFieldState;
	LaborCodeID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	InventoryID: PXFieldState;
	TranDesc: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	ExpireDate: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	EmployeeID: PXFieldState;
	ShiftCD: PXFieldState;
	LaborTime: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
})
export class ScanLog extends PXView {
	ScanTime: PXFieldState;
	Mode: PXFieldState;
	Prompt: PXFieldState;
	Scan: PXFieldState;
	Message: PXFieldState;
}

export class AMScanUserSetup extends PXView {
	DefaultWarehouse: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultLotSerialNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultExpireDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@graphInfo({ graphType: 'PX.Objects.AM.ScanLabor+Host', primaryView: 'HeaderView' })
export class AM302020 extends PXScreen {
	HeaderView = createSingle(ScanHeader);
	Info = createSingle(ScanInfo);
	transactions = createCollection(AMMTran);
	Logs = createCollection(ScanLog);
	UserSetupView = createSingle(AMScanUserSetup);
}
