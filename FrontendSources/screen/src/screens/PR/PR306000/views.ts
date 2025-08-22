import { PXView, PXFieldState, gridConfig, PXFieldOptions, GridPreset, columnConfig } from "client-controls";

export class PRPTOAdjustment extends PXView  {
	Type : PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr : PXFieldState;
	Status : PXFieldState;
	Date : PXFieldState;
	Description : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class PRPTOAdjustmentDetail extends PXView  {
	@columnConfig({ hideViewLink: true })
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	BAccountID_Description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BankID: PXFieldState<PXFieldOptions.CommitChanges>;

	InitialBalance : PXFieldState;
	AdjustmentHours : PXFieldState<PXFieldOptions.CommitChanges>;
	AdjustmentReason : PXFieldState<PXFieldOptions.CommitChanges>;
	ReasonDetails : PXFieldState<PXFieldOptions.CommitChanges>;
	NewBalance : PXFieldState;
	BalanceLimit : PXFieldState;
}
