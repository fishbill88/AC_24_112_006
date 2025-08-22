import {PXView, PXFieldState, gridConfig, linkCommand, columnConfig, GridColumnType, PXActionState } from "client-controls";

// Views

@gridConfig({allowDelete: false, allowInsert: false, allowUpdate: false, adjustPageSize: true, syncPosition: true}) 
export class UPCompany extends PXView {
	InsertCompanyCommand: PXActionState;
	MoveCompanyUp: PXActionState;
	MoveCompanyDown: PXActionState;

	Current: PXFieldState;
	@linkCommand('UPCompany_View')
	CompanyID: PXFieldState;
	CompanyCD: PXFieldState;
	LoginName: PXFieldState;
	Status: PXFieldState;
}