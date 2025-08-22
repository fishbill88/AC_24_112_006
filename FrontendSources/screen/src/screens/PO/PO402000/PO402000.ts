import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent,  CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, GridPreset } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.PO.POAccrualInquiry', primaryView: 'Filter', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class PO402000 extends PXScreen {
	ViewDocument: PXActionState;

	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(POAccrualInquiryFilter);
	@viewInfo({containerName: 'Documents'})
	ResultRecords = createCollection(POAccrualInquiryResult);
}

// Views

export class POAccrualInquiryFilter extends PXView  {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	AcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubCD: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowByLines: PXFieldState<PXFieldOptions.CommitChanges>;
	UnbilledAmt: PXFieldState;
	NotReceivedAmt: PXFieldState;
	NotInvoicedAmt: PXFieldState;
	NotAdjustedAmt: PXFieldState;
	Balance: PXFieldState;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	suppressNoteFiles: true,
	quickFilterFields: ["VendorID"]
})
export class POAccrualInquiryResult extends PXView  {
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	DocumentType: PXFieldState;
	@linkCommand('ViewDocument')
	DocumentNbr: PXFieldState;
	LineNbr: PXFieldState;
	DocDate: PXFieldState;
	@columnConfig({hideViewLink: true}) VendorID: PXFieldState;
	VendorName: PXFieldState;
	INDocType: PXFieldState;
	INRefNbr: PXFieldState;
	FinPeriodID: PXFieldState;
	PPVAdjRefNbr: PXFieldState;
	TaxAdjRefNbr: PXFieldState;
	@columnConfig({hideViewLink: true}) BranchID: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server, hideViewLink: true})	SiteID: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})	InventoryID: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})	TranDesc: PXFieldState;
	@columnConfig({hideViewLink: true}) AcctID: PXFieldState;
	@columnConfig({hideViewLink: true}) SubID: PXFieldState;
	UnbilledAmt: PXFieldState;
	NotReceivedAmt: PXFieldState;
	NotInvoicedAmt: PXFieldState;
	NotAdjustedAmt: PXFieldState;
	AccrualAmt: PXFieldState;
}
