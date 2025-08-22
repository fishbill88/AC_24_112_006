import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, linkCommand, columnConfig, PXActionState, ICurrencyInfo, viewInfo, gridConfig, GridPreset
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APPaymentEntry', primaryView: 'Document', udfTypeField: "DocType", showUDFIndicator: true })
export class AP302000 extends PXScreen {

	AddressLookup: PXActionState;
	ViewDocumentToApply: PXActionState;
	ViewCurrentBatch: PXActionState;
	ViewApplicationDocument: PXActionState;
	ViewPPDVATAdj: PXActionState;
	ViewVoucherBatch: PXActionState;
	ViewWorkBook: PXActionState;
	NewVendor: PXActionState;
	EditVendor: PXActionState;
	LoadPOOrders: PXActionState;

	Document = createSingle(Document);
	Adjustments = createCollection(Adjustments);
	APPost = createCollection(APPost);
	CurrentDocument = createSingle(CurrentDocument);
	Approval = createCollection(Approval);
	Remittance_Contact = createSingle(Remittance_Contact);
	Remittance_Address = createSingle(Remittance_Address);
	PaymentCharges = createCollection(PaymentCharges);

	@viewInfo({ containerName: "Currency rate" })
	CurrencyInfo = createSingle(CurrencyInfo);

}

export class Document extends PXView {

	DocType: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState;
	AdjDate: PXFieldState<PXFieldOptions.CommitChanges>;
	AdjFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	DepositAfter: PXFieldState;
	DocDesc: PXFieldState;
	CuryOrigDocAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPOApplAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnappliedBal: PXFieldState;
	CuryInitDocBal: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryApplAmt: PXFieldState;
	CuryChargeAmt: PXFieldState<PXFieldOptions.Disabled>;

}

@gridConfig({ preset: GridPreset.Details })
export class Adjustments extends PXView {

	LoadInvoices: PXActionState;
	AddJointPayee: PXActionState;

	@columnConfig({ hideViewLink: true })
	AdjdBranchID: PXFieldState;

	AdjdDocType: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ViewDocumentToApply")
	AdjdRefNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AdjdLineNbr: PXFieldState;

	APTran__InventoryID: PXFieldState;
	APTran__ProjectID: PXFieldState;
	APInvoice__ProjectID: PXFieldState;
	APTran__TaskID: PXFieldState;
	APTran__CostCodeID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	APTran__AccountID: PXFieldState;

	CuryAdjgAmt: PXFieldState;
	CuryAdjgPPDAmt: PXFieldState;
	CuryAdjgWhTaxAmt: PXFieldState;
	AdjdDocDate: PXFieldState;
	APInvoice__DueDate: PXFieldState;
	APInvoice__DiscDate: PXFieldState;
	AdjdCuryRate: PXFieldState;
	CuryDocBal: PXFieldState;
	CuryDiscBal: PXFieldState;
	CuryWhTaxBal: PXFieldState;
	APInvoice__DocDesc: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AdjdCuryID: PXFieldState;

	AdjdFinPeriodID: PXFieldState;
	APInvoice__InvoiceNbr: PXFieldState;
	APInvoice__SuppliedByVendorID: PXFieldState;
	HasExpiredComplianceDocuments: PXFieldState;
	StubNbr: PXFieldState;

}


@gridConfig({ preset: GridPreset.Details })
export class APPost extends PXView {

	ReverseApplication: PXActionState;

	@columnConfig({ hideViewLink: true })
	APRegister__BranchID: PXFieldState;

	@linkCommand("ViewCurrentBatch")
	BatchNbr: PXFieldState;

	SourceDocType: PXFieldState;

	@linkCommand("ViewApplicationDocument")
	SourceRefNbr: PXFieldState;

	LineNbr: PXFieldState;

	APTran__InventoryID: PXFieldState;
	APTran__ProjectID: PXFieldState;
	APRegister__ProjectID: PXFieldState;
	APTran__TaskID: PXFieldState;
	APTran__CostCodeID: PXFieldState;

	@columnConfig({hideViewLink: true})
	APTran__AccountID: PXFieldState;

	CuryAmt: PXFieldState;
	CuryPPDAmt: PXFieldState;
	CuryWhTaxAmt: PXFieldState;
	FinPeriodID: PXFieldState;
	ApplicationDate: PXFieldState;
	APRegister__DocDate: PXFieldState;
	APInvoice__DueDate: PXFieldState;
	APInvoice__DiscDate: PXFieldState;
	CuryBalanceAmt: PXFieldState;
	CuryDiscBalanceAmt: PXFieldState;
	APRegister__DocDesc: PXFieldState;

	@columnConfig({ hideViewLink: true })
	APRegister__CuryID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	APRegister__FinPeriodID: PXFieldState;

	APInvoice__InvoiceNbr: PXFieldState;
	APAdjust2__TaxInvoiceNbr: PXFieldState;
	APInvoice__SuppliedByVendorID: PXFieldState;
	APAdjust2__PendingPPD: PXFieldState;

	@linkCommand("ViewPPDVATAdj")
	APAdjust2__PPDVATAdjDescription: PXFieldState;

	APAdjust2__HasExpiredComplianceDocuments: PXFieldState;
	APAdjust2__StubNbr: PXFieldState;

}

export class CurrentDocument extends PXView {

	BatchNbr: PXFieldState;
	DisplayCuryInitDocBal: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	APAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	APSubID: PXFieldState;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	Cleared: PXFieldState<PXFieldOptions.CommitChanges>;
	ClearDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DepositAsBatch: PXFieldState<PXFieldOptions.CommitChanges>;
	Deposited: PXFieldState<PXFieldOptions.CommitChanges>;
	DepositDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DepositNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	OrigRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;

	PrintCheck: PXFieldState<PXFieldOptions.CommitChanges>;
	BatchPaymentRefNbr: PXFieldState;

}

@gridConfig({ preset: GridPreset.Details, allowInsert: false, allowDelete: false })
export class Approval extends PXView {

	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState;
	ApproveDate: PXFieldState;
	Status: PXFieldState;
	Reason: PXFieldState;
	AssignmentMapID: PXFieldState;
	RuleID: PXFieldState;
	StepID: PXFieldState;
	CreatedDateTime: PXFieldState;

}

export class Remittance_Contact extends PXView {

	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState;


}

export class Remittance_Address extends PXView {

	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState;

}

@gridConfig({ preset: GridPreset.Details })
export class PaymentCharges extends PXView {

	EntryTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDesc: PXFieldState;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState;
	CuryTranAmt: PXFieldState;

}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
}
