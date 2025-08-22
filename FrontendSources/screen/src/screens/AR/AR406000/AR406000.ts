import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState, PXActionState,
	graphInfo, viewInfo, gridConfig, linkCommand,
	PXFieldOptions
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.CCTransactionsHistoryEnq", primaryView: "Filter",
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR406000 extends PXScreen {
	viewCustomer: PXActionState;
	viewPaymentMethod: PXActionState;
	viewDocument: PXActionState;

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(CCTransactionsHistoryFilter);
	
   	@viewInfo({containerName: "Transaction List"})
	CCTrans = createCollection(CCProcTran);
}

export class CCTransactionsHistoryFilter extends PXView  {
	PaymentMethodID : PXFieldState<PXFieldOptions.CommitChanges>;
	CardType : PXFieldState<PXFieldOptions.CommitChanges>;
	PartialCardNumber : PXFieldState<PXFieldOptions.CommitChanges>;
	NameOnCard : PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfCards : PXFieldState;
	Descr : PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate : PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate : PXFieldState<PXFieldOptions.CommitChanges>;
	ProcessingCenterID : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class CCProcTran extends PXView  {
	@linkCommand("viewCustomer")
	Customer__AcctCD : PXFieldState;
	
	Customer__AcctName : PXFieldState;
	
	@linkCommand("viewPaymentMethod")
	CustomerPaymentMethod__PaymentMethodID : PXFieldState;
	
	CCProcTran__DocType : PXFieldState;
	
	@linkCommand("viewDocument")
	CCProcTran__RefNbr : PXFieldState;
	
	CCProcTran__OrigDocType : PXFieldState;
	CCProcTran__OrigRefNbr : PXFieldState;
	CustomerPaymentMethod__Descr : PXFieldState;
	TranNbr : PXFieldState;
	ProcessingCenterID : PXFieldState;
	TranType : PXFieldState;
	TranStatus : PXFieldState;
	Amount : PXFieldState;
	RefTranNbr : PXFieldState;
	PCTranNumber : PXFieldState;
	AuthNumber : PXFieldState;
	PCResponseReasonText : PXFieldState;
	StartTime : PXFieldState;
	ProcStatus : PXFieldState;
	CVVVerificationStatus : PXFieldState;
}
