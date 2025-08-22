import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, GridColumnShowHideMode, gridConfig, GridPreset } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.SalesTaxMaint', primaryView: 'Tax', })
export class TX205000 extends PXScreen {

	Tax = createSingle(Tax);
	TaxRevisions = createCollection(TaxRev);
	Categories = createCollection(TaxCategoryDet);
	Zones = createCollection(TaxZoneDet);
	CurrentTax = createSingle(Tax);
	TaxForPrintingParametersTab = createSingle(Tax);

}

export class Tax extends PXView {

	TaxID: PXFieldState;
	Descr: PXFieldState;
	TaxType: PXFieldState<PXFieldOptions.CommitChanges>;
	DeductibleVAT: PXFieldState<PXFieldOptions.CommitChanges>;
	ReverseTax: PXFieldState<PXFieldOptions.CommitChanges>;
	PendingTax: PXFieldState<PXFieldOptions.CommitChanges>;
	StatisticalTax: PXFieldState<PXFieldOptions.CommitChanges>;
	DirectTax: PXFieldState<PXFieldOptions.CommitChanges>;
	ExemptTax: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeInTaxable: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcRule: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxApplyTermsDisc: PXFieldState;
	TaxCalcLevel2Exclude: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	OutDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxUOM: PXFieldState<PXFieldOptions.CommitChanges>;
	PerUnitTaxPostMode: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesTaxAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesTaxAcctIDOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesTaxSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesTaxSubIDOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchTaxAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchTaxAcctIDOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchTaxSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchTaxSubIDOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	ReportExpenseToSingleAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PendingSalesTaxAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PendingSalesTaxSubID: PXFieldState;
	PendingPurchTaxAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PendingPurchTaxSubID: PXFieldState;
	RetainageTaxPayableAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageTaxPayableSubID: PXFieldState;
	RetainageTaxClaimableAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageTaxClaimableSubID: PXFieldState;

	ShortPrintingLabel: PXFieldState;
	LongPrintingLabel: PXFieldState;
	PrintingSequence: PXFieldState;

}

@gridConfig({ preset: GridPreset.Details })
export class TaxRev extends PXView {

	StartDate: PXFieldState;
	TaxBucketID: PXFieldState;
	TaxRate: PXFieldState;
	NonDeductibleTaxRate: PXFieldState;
	TaxableMin: PXFieldState;
	TaxableMax: PXFieldState;
	TaxableMaxQty: PXFieldState;
	TaxType: PXFieldState;

}

@gridConfig({ preset: GridPreset.Details })
export class TaxCategoryDet extends PXView {

	@columnConfig({ allowUpdate: false })
	TaxCategory__TaxCatFlag: PXFieldState;

	@columnConfig({ allowUpdate: false })
	TaxCategory__Descr: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	TaxCategoryID: PXFieldState;

}

@gridConfig({ preset: GridPreset.Details })
export class TaxZoneDet extends PXView {

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	TaxZoneID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	TaxZone__DfltTaxCategoryID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	TaxZone__Descr: PXFieldState;

}
