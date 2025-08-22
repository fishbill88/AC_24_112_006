import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
	linkCommand,
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.AM.WCMaint', primaryView: 'WCRecords'})
export class AM207000 extends PXScreen {
	MassUpdate: PXActionState;
	CalendarInquiry: PXActionState;

	WCRecords = createSingle(AMWCHeader);
	WCRecordsSelected = createSingle(AMWCSelected);
	CurySettings_AMWC = createSingle(AMWCCury);
	WCShifts = createCollection(AMShift);
	WCOverheads = createCollection(AMWCOvhd);
	WCMachines = createCollection(AMWCMach);
	WCWhereUsed = createCollection(AMBomOper);
	SubstituteWorkCenters = createCollection(AMWCSubstitute);
	MassUpdateFilter = createSingle(WorkCenterUpdateFilter);
	CalendarInquiryFilter = createSingle(CalendarWeek);
}

export class AMWCHeader extends PXView {
	WcID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState;
	ActiveFlg: PXFieldState;
}

export class AMWCSelected extends PXView {
	WcBasis: PXFieldState;
	ScrapAction: PXFieldState;
	BflushMatl: PXFieldState;
	BflushLbr: PXFieldState;
	AllowMultiClockEntry: PXFieldState;
	ControlPoint: PXFieldState;
	OutsideFlg: PXFieldState;
	DefaultQueueTime: PXFieldState;
	DefaultFinishTime: PXFieldState;
	DefaultMoveTime: PXFieldState;
}

export class AMWCCury extends PXView {
	StdCost: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class AMShift extends PXView {
	@columnConfig({ hideViewLink: true }) ShiftCD: PXFieldState;
	CrewSize: PXFieldState;
	MachNbr: PXFieldState;
	ShftEff: PXFieldState;
	CalendarID: PXFieldState;
	EPShiftCode__DiffType: PXFieldState;
	EPShiftCode__ShftDiff: PXFieldState;
	LaborCodeID: PXFieldState;
	WcID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class AMWCOvhd extends PXView {
	OvhdID: PXFieldState;
	AMOverhead__Descr: PXFieldState;
	AMOverhead__OvhdType: PXFieldState;
	OFactor: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMWCMach extends PXView {
	MachID: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	StdCost: PXFieldState;
	@columnConfig({ hideViewLink: true }) MachAcctID: PXFieldState;
	@columnConfig({ hideViewLink: true }) MachSubID: PXFieldState;
	WcID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	adjustPageSize: true,
	syncPosition: true,
})
export class AMBomOper extends PXView {
	MassUpdate: PXActionState;

	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	@linkCommand("ViewBOM") AMBomItem__BOMID: PXFieldState;
	@columnConfig({ hideViewLink: true }) AMBomItem__RevisionID: PXFieldState;
	OperationCD: PXFieldState;
	AMBomItem__EffStartDate: PXFieldState;
	AMBomItem__EffEndDate: PXFieldState;
	AMBomItem__InventoryID: PXFieldState;
	AMBomItem__SiteID: PXFieldState;
	AMBomItem__Status: PXFieldState;
	Descr: PXFieldState;
	ScrapAction: PXFieldState;
	BFlush: PXFieldState;
	ControlPoint: PXFieldState;
	OutsideProcess: PXFieldState;
	QueueTime: PXFieldState;
	FinishTime: PXFieldState;
	MoveTime: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class AMWCSubstitute extends PXView {
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubstituteWcID: PXFieldState<PXFieldOptions.CommitChanges>;
	UpdateOperDesc: PXFieldState;
	WcID: PXFieldState;
}

export class WorkCenterUpdateFilter extends PXView {
	BFlush: PXFieldState<PXFieldOptions.CommitChanges>;
	ScrapAction: PXFieldState<PXFieldOptions.CommitChanges>;
	OperDescription: PXFieldState<PXFieldOptions.CommitChanges>;
	OutsideProcess: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CalendarWeek extends PXView {
	Day1Date: PXFieldState;
	Day2Date: PXFieldState;
	Day3Date: PXFieldState;
	Day4Date: PXFieldState;
	Day5Date: PXFieldState;
	Day6Date: PXFieldState;
	Day7Date: PXFieldState;
	Day1DayOfWeek: PXFieldState;
	Day2DayOfWeek: PXFieldState;
	Day3DayOfWeek: PXFieldState;
	Day4DayOfWeek: PXFieldState;
	Day5DayOfWeek: PXFieldState;
	Day6DayOfWeek: PXFieldState;
	Day7DayOfWeek: PXFieldState;
	Day1StartTime: PXFieldState;
	Day2StartTime: PXFieldState;
	Day3StartTime: PXFieldState;
	Day4StartTime: PXFieldState;
	Day5StartTime: PXFieldState;
	Day6StartTime: PXFieldState;
	Day7StartTime: PXFieldState;
	Day1EndTime: PXFieldState;
	Day2EndTime: PXFieldState;
	Day3EndTime: PXFieldState;
	Day4EndTime: PXFieldState;
	Day5EndTime: PXFieldState;
	Day6EndTime: PXFieldState;
	Day7EndTime: PXFieldState;
	Day1WorkTime: PXFieldState;
	Day2WorkTime: PXFieldState;
	Day3WorkTime: PXFieldState;
	Day4WorkTime: PXFieldState;
	Day5WorkTime: PXFieldState;
	Day6WorkTime: PXFieldState;
	Day7WorkTime: PXFieldState;
	Day1BreakTime: PXFieldState;
	Day2BreakTime: PXFieldState;
	Day3BreakTime: PXFieldState;
	Day4BreakTime: PXFieldState;
	Day5BreakTime: PXFieldState;
	Day6BreakTime: PXFieldState;
	Day7BreakTime: PXFieldState;
}
