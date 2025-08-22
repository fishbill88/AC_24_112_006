import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, ICurrencyInfo, PXFieldOptions, columnConfig, linkCommand, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.TaxAdjustmentEntry', primaryView: 'Document', udfTypeField: 'DocType', showUDFIndicator: true })
export class TX301000 extends PXScreen {

	ViewBatch: PXActionState;
	ViewOriginalDocument: PXActionState;
	NewVendor: PXActionState;
	EditVendor: PXActionState;
	CurrencyView: PXActionState;

	Document = createSingle(TaxAdjustment);
	Transactions = createCollection(TaxTran);
	CurrentDocument = createSingle(TaxAdjustment);
	CurrencyInfo = createSingle(CurrencyInfo);
}

export class TaxAdjustment extends PXView {

	DocType: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxPeriod: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDesc: PXFieldState<PXFieldOptions.Multiline>;
	CuryDocBal: PXFieldState<PXFieldOptions.Disabled>;
	CuryOrigDocAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	BatchNbr: PXFieldState<PXFieldOptions.Disabled>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	AdjAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	AdjSubID: PXFieldState;

	@linkCommand('ViewOriginalDocument')
	OrigRefNbr: PXFieldState<PXFieldOptions.Disabled>;

}

export class TaxTran extends PXView {

	@columnConfig({hideViewLink: true})
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;

	TaxRate: PXFieldState;
	CuryTaxableAmt: PXFieldState;
	CuryTaxAmt: PXFieldState;

	@columnConfig({hideViewLink: true})
	TaxZoneID: PXFieldState;

	@columnConfig({hideViewLink: true})
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({hideViewLink: true})
	SubID: PXFieldState;

	Description: PXFieldState;

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
