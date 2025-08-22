import {
	PXView, createSingle, PXFieldState, graphInfo, PXScreen, createCollection, PXFieldOptions,
	PXActionState, columnConfig, gridConfig, GridPreset
} from "client-controls";
import { autoinject } from 'aurelia-framework';

@graphInfo({ graphType: 'PX.Objects.GL.AccountHistoryEnq', primaryView: 'Filter' })
@autoinject
export class GL401000 extends PXScreen {

	ViewDetails: PXActionState;
	Filter = createSingle(GLHistoryEnqFilter);

	EnqResult = createCollection(GLHistoryEnquiryResult);
}

export class GLHistoryEnqFilter extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LedgerID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubCD: PXFieldState<PXFieldOptions.CommitChanges>;
	UseMasterCalendar: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ preset: GridPreset.PrimaryInquiry })
export class GLHistoryEnquiryResult extends PXView {

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	AccountCD: PXFieldState;
	LedgerID: PXFieldState<PXFieldOptions.Hidden>;
	SubCD: PXFieldState<PXFieldOptions.Hidden>;
	Type: PXFieldState;
	Description: PXFieldState;
	LastActivityPeriod: PXFieldState;
	SignBegBalance: PXFieldState;
	PtdDebitTotal: PXFieldState;
	PtdCreditTotal: PXFieldState;
	SignEndBalance: PXFieldState;
	PtdSaldo: PXFieldState;
	ConsolAccountCD: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountClassID: PXFieldState;
}
