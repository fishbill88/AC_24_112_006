import {
	PXScreen,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from 'client-controls';

export class ProdSetup extends PXView {
	MoveNumberingID: PXFieldState;
	LaborNumberingID: PXFieldState;
	MaterialNumberingID: PXFieldState;
	WipAdjustNumberingID: PXFieldState;
	ProdCostNumberingID: PXFieldState;
	DisassemblyNumberingID: PXFieldState;
	VendorShipmentNumberingID: PXFieldState;

	FMLTMRPOrdorOP: PXFieldState;
	FMLTime: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineScheduling: PXFieldState;
	ToolScheduling: PXFieldState;
	UseShiftCrewSize: PXFieldState;
	FixMfgCalendarID: PXFieldState;
	FMLTimeUnits: PXFieldState;
	SchdBlockSize: PXFieldState<PXFieldOptions.CommitChanges>;

	DfltLbrRate: PXFieldState;
	DefaultOrderType: PXFieldState;
	DefaultDisassembleOrderType: PXFieldState;
	CTPOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	InclScrap: PXFieldState;
	SummPost: PXFieldState;
	HoldEntry: PXFieldState;
	RequireControlTotal: PXFieldState;
	DefaultEmployee: PXFieldState;
	RestrictClockCurrentUser: PXFieldState;
	LockWorkflowEnabled: PXFieldState<PXFieldOptions.CommitChanges>;

	HoldShipmentsOnEntry: PXFieldState;
	ValidateShipmentTotalOnConfirm: PXFieldState;
}

export class ScanSetup extends PXView {
	UseDefaultQtyInMaterials: PXFieldState<PXFieldOptions.CommitChanges>;
	UseDefaultQtyInMove: PXFieldState<PXFieldOptions.CommitChanges>;
	UseRemainingQtyInMaterials: PXFieldState<PXFieldOptions.CommitChanges>;
	UseRemainingQtyInMove: PXFieldState<PXFieldOptions.CommitChanges>;
	UseDefaultOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestLocationForEachItemInMaterials: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestLocationForEachItemInMove: PXFieldState<PXFieldOptions.CommitChanges>;
	ExplicitLineConfirmation: PXFieldState;
	DefaultWarehouse: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultLotSerialNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultExpireDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@graphInfo({ graphType: 'PX.Objects.AM.ProdSetup', primaryView: 'ProdSetupRecord' })
export class AM102000 extends PXScreen {
	ProdSetupRecord = createSingle(ProdSetup);
	ScanSetup = createSingle(ScanSetup);
}
