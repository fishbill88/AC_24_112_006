import {
	PXScreen, createCollection, graphInfo, PXView, PXFieldState, columnConfig, createSingle, PXFieldOptions, linkCommand, gridConfig
} from 'client-controls';


@graphInfo({ graphType: 'PX.Objects.AP.APGenerateIntercompanyBills', primaryView: 'Filter' })
export class AP503500 extends PXScreen {

	Filter = createSingle(APGenerateIntercompanyBillsFilter);
	Documents = createCollection(ARDocumentForAPDocument);

}

export class APGenerateIntercompanyBillsFilter extends PXView {

	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateOnHold: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateInSpecificPeriod: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyProjectInformation: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class ARDocumentForAPDocument extends PXView {

	@columnConfig({ allowNull: false, allowCheckAll: true, allowSort: false })
	Selected: PXFieldState;

	VendorCD: PXFieldState;
	CustomerCD: PXFieldState;
	DocType: PXFieldState;

	@linkCommand("ViewARDocument")
	RefNbr: PXFieldState;

	DocDate: PXFieldState;
	FinPeriodID: PXFieldState;
	Status: PXFieldState;
	CuryOrigDocAmt: PXFieldState;
	CuryID: PXFieldState;
	DocDesc: PXFieldState;
	ProjectCD: PXFieldState;
	InvoiceNbr: PXFieldState;
	TermsID: PXFieldState;
	DueDate: PXFieldState;
	DiscDate: PXFieldState;
	CuryDocBal: PXFieldState;
	CuryTaxTotal: PXFieldState;
	CuryOrigDiscAmt: PXFieldState;

	@linkCommand("ViewAPDocument")
	IntercompanyAPDocumentNoteID: PXFieldState;;

}
