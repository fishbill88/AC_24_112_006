import { EP203000 } from "../EP203000";
import {
	PXView,
	PXFieldState,
	featureInstalled,
	createCollection,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	viewInfo,
	GridPreset,
} from "client-controls";

export interface EP203000_History extends EP203000 {}

export class EP203000_History {
	@viewInfo({ containerName: "History" })
	EmployeePositions = createCollection(EPEmployeePosition);
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	fastFilterByAllFields: false,
	allowUpdate: false,
})
export class EPEmployeePosition extends PXView {
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	PositionID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	StartReason: PXFieldState<PXFieldOptions.CommitChanges>;
	ProbationPeriodEndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	IsTerminated: PXFieldState<PXFieldOptions.CommitChanges>;
	TermReason: PXFieldState<PXFieldOptions.CommitChanges>;
	IsRehirable: PXFieldState<PXFieldOptions.CommitChanges>;
	SettlementPaycheckRefNoteID: PXFieldState;
}
