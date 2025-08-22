import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState, PXActionState,
	graphInfo, viewInfo, gridConfig, columnConfig, linkCommand,
	PXFieldOptions, ICurrencyInfo
} from "client-controls";


@graphInfo({graphType: "PX.Objects.CA.CADepositEntry", primaryView: "Document", udfTypeField: "TranType", showUDFIndicator: true })
export class CA305000 extends PXScreen {
	ViewDocument: PXActionState;

   	@viewInfo({containerName: "Payment Selection"})
	filter = createSingle(PaymentFilter);

   	@viewInfo({containerName: "Add Payment to Deposit"})
	AvailablePayments = createCollection(PaymentInfo);

   	@viewInfo({containerName: "Deposit Summary"})
	Document = createSingle(CADeposit);

   	@viewInfo({containerName: "Payments"})
	DepositPayments = createCollection(CADepositDetail);

   	@viewInfo({containerName: "Charges"})
	Charges = createCollection(CADepositCharge);

   	@viewInfo({containerName: "Financial"})
    DocumentCurrent = createSingle(CADeposit2);

	@viewInfo({ containerName: "currencyinfo" })
	CurrencyInfo = createSingle(CurrencyInfo);
}

export class PaymentFilter extends PXView  {
	CashAccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID : PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate : PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate : PXFieldState<PXFieldOptions.CommitChanges>;
	SelectionTotal : PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfDocuments : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class PaymentInfo extends PXView  {
	@columnConfig({ allowNull: false })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	Module: PXFieldState;
	DocType : PXFieldState;
	RefNbr : PXFieldState;
	BAccountID : PXFieldState;
	BAccountID_BAccountR_acctName : PXFieldState;
	LocationID : PXFieldState;
	ExtRefNbr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DepositAfter: PXFieldState;

	CuryID: PXFieldState;

	@columnConfig({ allowNull: false })
	CuryOrigDocAmt: PXFieldState;

	CashAccountID : PXFieldState;
	CashAccountID_CashAccount_Descr: PXFieldState;
	PaymentMethodID: PXFieldState;
	PMInstanceID: PXFieldState;
	CuryChargeTotal: PXFieldState;
	CuryGrossPaymentAmount: PXFieldState;
}

export class CADeposit extends PXView  {
	TranType : PXFieldState;
	RefNbr : PXFieldState;
	CashAccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	Status : PXFieldState<PXFieldOptions.Disabled>;
	TranDesc : PXFieldState<PXFieldOptions.Multiline>;
	TranDate : PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID : PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr : PXFieldState;
	ExtraCashAccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtraCashTotal : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDetailTotal : PXFieldState<PXFieldOptions.Disabled>;
	CuryChargeTotal : PXFieldState<PXFieldOptions.Disabled>;
	CuryTranAmt : PXFieldState<PXFieldOptions.Disabled>;
	CuryControlAmt : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true,
	syncPosition: true
})
export class CADepositDetail extends PXView {
	AddPayment: PXActionState;

	OrigModule: PXFieldState;
	PaymentInfo__DocType: PXFieldState;

	@linkCommand("ViewDocument")
	OrigRefNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	PaymentInfo__BAccountID: PXFieldState;

	PaymentInfo__BAccountID_Description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	PaymentInfo__LocationID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CashAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	PaymentInfo__CuryID: PXFieldState;

	@columnConfig({ allowNull: false })
	CuryTranAmt: PXFieldState;

	@columnConfig({ allowNull: false })
	CuryOrigAmt: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	PaymentInfo__PaymentMethodID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	PaymentInfo__Status: PXFieldState;

	PaymentInfo__ExtRefNbr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	PaymentInfo__DocDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	PaymentInfo__DepositAfter: PXFieldState;

	ChargeEntryTypeID: PXFieldState;
}

export class CADepositCharge extends PXView  {
	@columnConfig({ hideViewLink: true })
	EntryTypeID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	PaymentMethodID: PXFieldState;

	ChargeRate: PXFieldState;

	@columnConfig({ allowNull: false })
	CuryChargeableAmt: PXFieldState;

	@columnConfig({ allowNull: false })
	CuryChargeAmt: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	SubID : PXFieldState;
}

export class CADeposit2 extends PXView  {
	TranID_CATran_batchNbr : PXFieldState<PXFieldOptions.Disabled>;
	WorkgroupID : PXFieldState;
	OwnerID : PXFieldState;
	ClearDate : PXFieldState;
	Cleared : PXFieldState<PXFieldOptions.NoLabel>;
	ChargesSeparate : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID : PXFieldState;
	BaseCuryID : PXFieldState;
	BaseCalc : PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID : PXFieldState;
	CuryRateTypeID : PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision : PXFieldState;
	CuryRate : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate : PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate : PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate : PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate : PXFieldState<PXFieldOptions.CommitChanges>;
}
