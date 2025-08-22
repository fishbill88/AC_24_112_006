import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	ExpenseClaim,
	ReceiptsForSubmit,
	CustomerUpdateAsk,
	ExpenseClaimDetailsCurrent,
	Tax_Rows,
	ExpenseClaimCurrent,
	ExpenseClaimDetails,
	Taxes,
	APDocuments,
	Approval,
	ReasonApproveRejectParams,
	ReassignApprovalFilter,
	CurrencyInfo
} from "./views";

@graphInfo({
	graphType: "PX.Objects.EP.ExpenseClaimEntry",
	primaryView: "ExpenseClaim",
	showUDFIndicator: true
})
export class EP301000 extends PXScreen {
	ViewUnsubmitReceipt: PXActionState;
	ViewTaxes: PXActionState;
	ViewProject: PXActionState;
	ViewInvoice: PXActionState;
	ViewAPInvoice: PXActionState;
	SubmitReceipt: PXActionState;
	CancelSubmitReceipt: PXActionState;
	ChangeOk: PXActionState;
	ChangeCancel: PXActionState;
	CommitTaxes: PXActionState;
	SaveTaxZone: PXActionState;
	CurrencyView: PXActionState;

	ExpenseClaim = createSingle(ExpenseClaim);
	ReceiptsForSubmit = createCollection(ReceiptsForSubmit);
	CustomerUpdateAsk = createSingle(CustomerUpdateAsk);
	ExpenseClaimDetailsCurrent = createSingle(ExpenseClaimDetailsCurrent);
	Tax_Rows = createCollection(Tax_Rows);
	ExpenseClaimCurrent = createSingle(ExpenseClaimCurrent);
	ExpenseClaimDetails = createCollection(ExpenseClaimDetails);
	Taxes = createCollection(Taxes);
	APDocuments = createCollection(APDocuments);
	Approval = createCollection(Approval);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
	CurrencyInfo = createSingle(CurrencyInfo);
}
