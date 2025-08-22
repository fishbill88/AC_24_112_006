import
{
	PXView, PXFieldState, PXFieldOptions, PXActionState, linkCommand, columnConfig, gridConfig
} from "client-controls";


export class FixedAsset extends PXView {
	AssetCD: PXFieldState;
	Description: PXFieldState;
	ParentAssetID: PXFieldState<PXFieldOptions.CommitChanges>;
	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	AssetTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsTangible: PXFieldState<PXFieldOptions.Disabled>;
	Qty: PXFieldState;
	Depreciable: PXFieldState<PXFieldOptions.CommitChanges>;
	UsefulLife: PXFieldState<PXFieldOptions.CommitChanges>;

	ConstructionAccountID: PXFieldState;
	ConstructionSubID: PXFieldState;
	FAAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FASubID: PXFieldState<PXFieldOptions.CommitChanges>;
	FAAccrualAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	FAAccrualSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccumulatedDepreciationAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccumulatedDepreciationSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DepreciatedExpenseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DepreciatedExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	GainAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	GainSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	LossAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	LossSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	RentAccountID: PXFieldState;
	RentSubID: PXFieldState;
	LeaseAccountID: PXFieldState;
	LeaseSubID: PXFieldState;
}

export class FADetails extends PXView {
	PropertyType: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	ReceiptDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DepreciateFromDate: PXFieldState<PXFieldOptions.CommitChanges>;
	AcquisitionCost: PXFieldState<PXFieldOptions.CommitChanges>;
	SalvageAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplacementCost: PXFieldState;
	BaseCuryID: PXFieldState;
	DisplayDisposalDate: PXFieldState<PXFieldOptions.Disabled>;
	DisplayDisposalMethodID: PXFieldState<PXFieldOptions.Disabled>;
	DisplaySaleAmount: PXFieldState<PXFieldOptions.Disabled>;
	TagNbr: PXFieldState;

	ReceiptType: PXFieldState;
	ReceiptNbr: PXFieldState;
	PONumber: PXFieldState;
	BillNumber: PXFieldState;
	Manufacturer: PXFieldState;
	Model: PXFieldState;
	SerialNumber: PXFieldState;
	WarrantyExpirationDate: PXFieldState;
	ReportingLineNbr: PXFieldState;
	Condition: PXFieldState;
	FairMarketValue: PXFieldState;
	LessorID: PXFieldState;
	LeaseRentTerm: PXFieldState;
	LeaseNumber: PXFieldState;
	RentAmount: PXFieldState;
	RetailCost: PXFieldState;
	ManufacturingYear: PXFieldState;
}

@gridConfig({ allowInsert: false, allowUpdate: false, allowDelete: false })
export class FALocationHistory extends PXView {

	TransactionDate: PXFieldState;
	PeriodID: PXFieldState;
	ClassID: PXFieldState;
	RefNbr: PXFieldState;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	BuildingID: PXFieldState<PXFieldOptions.CommitChanges>;
	Floor: PXFieldState<PXFieldOptions.CommitChanges>;
	Room: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID_EPEmployee_acctName: PXFieldState;
	Department: PXFieldState<PXFieldOptions.CommitChanges>;
	Reason: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	FAAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FASubID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccumulatedDepreciationAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccumulatedDepreciationSubID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DepreciatedExpenseAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DepreciatedExpenseSubID: PXFieldState;

	LastModifiedByID_Modifier_username: PXFieldState<PXFieldOptions.Disabled>;
	LastModifiedDateTime: PXFieldState<PXFieldOptions.Disabled>;

}

@gridConfig({ syncPosition: true })
export class FABookBalance extends PXView {
	BookID: PXFieldState<PXFieldOptions.CommitChanges>;
	DepreciationMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	UpdateGL: PXFieldState;
	Depreciate: PXFieldState<PXFieldOptions.CommitChanges>;
	DeprFromDate: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	DeprFromPeriod: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LastDeprPeriod: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DeprToPeriod: PXFieldState<PXFieldOptions.CommitChanges>;

	AcquisitionCost: PXFieldState;
	YtdAcquired: PXFieldState;
	BusinessUse: PXFieldState;
	YtdDeprBase: PXFieldState;
	SalvageAmount: PXFieldState;
	YtdDepreciated: PXFieldState;
	YtdBal: PXFieldState;
	YtdRGOL: PXFieldState;
	PercentPerYear: PXFieldState<PXFieldOptions.CommitChanges>;
	UsefulLife: PXFieldState<PXFieldOptions.CommitChanges>;
	ADSLife: PXFieldState;
	RecoveryPeriod: PXFieldState<PXFieldOptions.Hidden>;
	AveragingConvention: PXFieldState;
	MidMonthType: PXFieldState;
	MidMonthDay: PXFieldState;
	Tax179Amount: PXFieldState;
	YtdTax179Recap: PXFieldState;
	BonusID: PXFieldState<PXFieldOptions.CommitChanges>;
	BonusRate: PXFieldState<PXFieldOptions.CommitChanges>;
	BonusAmount: PXFieldState;
	YtdBonusRecap: PXFieldState;
}

@gridConfig({ allowInsert: false, allowUpdate: false, allowDelete: false })
export class FAComponent extends PXView {
	@columnConfig({ hideViewLink: true })
	AssetCD: PXFieldState;

	Description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ClassID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AssetTypeID: PXFieldState;

	Status: PXFieldState;
	FADetails__AcquisitionCost: PXFieldState;
	UsefulLife: PXFieldState;
	Active: PXFieldState;
	FADetails__PropertyType: PXFieldState;
	FADetails__Condition: PXFieldState;
	FADetails__ReceiptDate: PXFieldState;
	FADetails__ReceiptType: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FADetails__ReceiptNbr: PXFieldState;

	FADetails__PONumber: PXFieldState;
	FADetails__BillNumber: PXFieldState;
	FADetails__Manufacturer: PXFieldState;
	FADetails__SerialNumber: PXFieldState;
	FADetails__TagNbr: PXFieldState;
}

@gridConfig({ allowInsert: false, allowUpdate: false, allowDelete: false, generateColumns: 1, adjustPageSize: true })
export class FAHistory extends PXView {
	FinPeriodID: PXFieldState;
}

@gridConfig({ allowInsert: false, allowUpdate: false, allowDelete: false, generateColumns: 1, pagerMode: 0 })
export class FASheetHistory extends PXView {
	AssetID: PXFieldState<PXFieldOptions.Hidden>;
	PeriodNbr: PXFieldState;
}

export class DeprBookFilter extends PXView {
	BookID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FASetup extends PXView {
	//DeprHistoryView: PXFieldState;
	ShowBookSheet: PXFieldState;
	ShowSideBySide: PXFieldState;
}

export class TranBookFilter extends PXView {
	BookID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true })
export class FATran extends PXView {
	@columnConfig({ hideViewLink: true })
	BookID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	TranDate: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	TranType: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DebitAccountID: PXFieldState;

	DebitAccountID_Account_description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DebitSubID: PXFieldState;

	DebitSubID_Sub_description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CreditAccountID: PXFieldState;

	CreditAccountID_Account_description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CreditSubID: PXFieldState;

	CreditSubID_Sub_description: PXFieldState;
	TranAmt: PXFieldState;

	@linkCommand("ViewDocument")
	RefNbr: PXFieldState;

	@linkCommand("ViewBatch")
	BatchNbr: PXFieldState;

	Released: PXFieldState;
	MethodDesc: PXFieldState;
	TranDesc: PXFieldState;
}

export class GLTranFilter extends PXView {
	ReconType: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDate: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowReconciled: PXFieldState<PXFieldOptions.CommitChanges>;

	AcquisitionCost: PXFieldState<PXFieldOptions.Disabled>;
	CurrentCost: PXFieldState<PXFieldOptions.Disabled>;
	AccrualBalance: PXFieldState<PXFieldOptions.Disabled>;
	UnreconciledAmt: PXFieldState<PXFieldOptions.Disabled>;

	SelectionAmt: PXFieldState<PXFieldOptions.Disabled>;
	ExpectedCost: PXFieldState<PXFieldOptions.Disabled>;
	ExpectedAccrualBal: PXFieldState<PXFieldOptions.Disabled>;

}

export class DsplFAATran extends PXView {
	ProcessAdditions: PXActionState;

	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	Component: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;

	Reconciled: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	FAAccrualTran__GLTranBranchID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	FAAccrualTran__GLTranInventoryID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ hideViewLink: true })
	FAAccrualTran__GLTranUOM: PXFieldState<PXFieldOptions.Disabled>;

	SelectedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	SelectedAmt: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowFilter: false, allowSort: false })
	OpenQty: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFilter: false, allowSort: false })
	OpenAmt: PXFieldState<PXFieldOptions.Disabled>;

	GLTranQty: PXFieldState<PXFieldOptions.Disabled>;
	UnitCost: PXFieldState<PXFieldOptions.Disabled>;
	GLTranAmt: PXFieldState<PXFieldOptions.Disabled>;
	FAAccrualTran__GLTranDate: PXFieldState<PXFieldOptions.Disabled>;
	FAAccrualTran__GLTranDesc: PXFieldState<PXFieldOptions.Disabled>;
	FAAccrualTran__GLTranModule: PXFieldState<PXFieldOptions.Disabled>;
	FAAccrualTran__GLTranBatchNbr: PXFieldState<PXFieldOptions.Disabled>;
	FAAccrualTran__GLTranRefNbr: PXFieldState<PXFieldOptions.Disabled>;
}

export class DisposeParams extends PXView {
	DisposalDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ActionBeforeDisposal: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	Reason: PXFieldState;
}

export class SuspendParameters extends PXView {
	CurrentPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReverseDisposalInfo extends PXView {
	DisposalDate: PXFieldState<PXFieldOptions.Disabled>;
	DisposalPeriodID: PXFieldState<PXFieldOptions.Disabled>;
	DisposalAmt: PXFieldState<PXFieldOptions.Disabled>;
	DisposalMethodID: PXFieldState<PXFieldOptions.Disabled>;
	DisposalAccountID: PXFieldState<PXFieldOptions.Disabled>;
	DisposalSubID: PXFieldState<PXFieldOptions.Disabled>;
	ReverseDisposalDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ReverseDisposalPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
}
