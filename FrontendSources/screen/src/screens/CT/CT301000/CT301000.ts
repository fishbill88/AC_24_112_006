import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CT.ContractMaint', primaryView: 'Contracts' })
export class CT301000 extends PXScreen {

	ShowContact: PXActionState;
	ViewInvoice: PXActionState;
	ViewContract: PXActionState;

	ChangeIDDialog = createSingle(ChangeIDParam);
	Contracts = createSingle(Contract);
	SetupSettings = createSingle(SetupSettingsFilter);
	ActivationSettings = createSingle(ActivationSettingsFilter);
	TerminationSettings = createSingle(TerminationSettingsFilter);
	OnDemandSettings = createSingle(BillingOnDemandSettingsFilter);
	RenewalSettings = createSingle(RenewalSettingsFilter);
	CurrentContract = createSingle(Contract);
	Billing = createSingle(ContractBillingSchedule);
	BillingLocation = createSingle(Location);

	// split DAC into 2 classes to allow different columns set for dirrerent grids
	ContractDetails = createCollection(ContractDetail);
	RecurringDetails = createCollection(ContractDetail2);

	ContractRates = createCollection(EPContractRate);
	RenewalHistory = createCollection(ContractRenewalHistory);
	Invoices = createCollection(ARInvoice);
	Answers = createCollection(CSAnswers);
	renewManualNumberingFilter = createSingle(RenewManualNumberingFilter);

}

export class ChangeIDParam extends PXView {

	CD: PXFieldState;

}

export class Contract extends PXView {

	ContractCD: PXFieldState;
	TemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Balance: PXFieldState;

	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ActivationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpireDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TerminationDate: PXFieldState;
	AutoRenew: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoRenewDays: PXFieldState;
	DaysBeforeExpiration: PXFieldState<PXFieldOptions.Disabled>;
	GracePeriod: PXFieldState;
	Days: PXFieldState<PXFieldOptions.Disabled>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState;
	SalesPersonID: PXFieldState;
	CaseItemID: PXFieldState;
	EffectiveFrom: PXFieldState;
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;
	PendingSetup: PXFieldState<PXFieldOptions.Disabled>;
	PendingRecurring: PXFieldState<PXFieldOptions.Disabled>;
	PendingRenewal: PXFieldState<PXFieldOptions.Disabled>;
	TotalPending: PXFieldState<PXFieldOptions.Disabled>;
	CurrentSetup: PXFieldState<PXFieldOptions.Disabled>;
	CurrentRecurring: PXFieldState<PXFieldOptions.Disabled>;
	CurrentRenewal: PXFieldState<PXFieldOptions.Disabled>;
	TotalRecurring: PXFieldState<PXFieldOptions.Disabled>;
	TotalUsage: PXFieldState<PXFieldOptions.Disabled>;
	TotalDue: PXFieldState<PXFieldOptions.Disabled>;

}

export class SetupSettingsFilter extends PXView {

	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class ActivationSettingsFilter extends PXView {

	ActivationDate: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class TerminationSettingsFilter extends PXView {

	TerminationDate: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class BillingOnDemandSettingsFilter extends PXView {

	BillingDate: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class RenewalSettingsFilter extends PXView {

	RenewalDate: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class ContractBillingSchedule extends PXView {

	StartBilling: PXFieldState;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	LastDate: PXFieldState;
	NextDate: PXFieldState;
	BillTo: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceFormula: PXFieldState;
	TranFormula: PXFieldState;

}

export class Location extends PXView {

	CBranchID: PXFieldState<PXFieldOptions.Disabled>;

}

export class ContractDetail extends PXView {

	ContractItemID: PXFieldState;
	Description: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	Change: PXFieldState;
	BasePriceVal: PXFieldState;
	BaseDiscountPct: PXFieldState;
	RecurringDiscountPct: PXFieldState;
	RenewalDiscountPct: PXFieldState;
	UsagePriceVal: PXFieldState;
	FixedRecurringPriceVal: PXFieldState;
	RenewalPriceVal: PXFieldState;

}

@gridConfig({ allowDelete: false, allowInsert: false })
export class ContractDetail2 extends PXView {

	ContractItemID: PXFieldState;
	Description: PXFieldState;
	InventoryItem__InventoryCD: PXFieldState;
	ContractItem__UOMForDeposits: PXFieldState;
	ContractItem__RecurringTypeForDeposits: PXFieldState;
	RecurringIncluded: PXFieldState;
	FixedRecurringPriceVal: PXFieldState;
	RecurringDiscountPct: PXFieldState;
	UsagePriceVal: PXFieldState;
	RecurringUsed: PXFieldState;
	RecurringUsedTotal: PXFieldState;

}

@gridConfig({ initNewRow: true })
export class EPContractRate extends PXView {

	EarningType: PXFieldState<PXFieldOptions.CommitChanges>;
	EPEarningType__Description: PXFieldState;
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ nullText: 'All Employees' })
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;

	EPEmployee__AcctName: PXFieldState;

}

@gridConfig({ allowDelete: false, allowInsert: false })
export class ContractRenewalHistory extends PXView {

	Action: PXFieldState;
	CreatedByID: PXFieldState;
	ChildContractID: PXFieldState;
	Date: PXFieldState;

}

export class ARInvoice extends PXView {

	DocType: PXFieldState;

	@linkCommand('ViewInvoice')
	RefNbr: PXFieldState;

	FinPeriodID: PXFieldState;
	DocDate: PXFieldState;
	DueDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;

	CuryOrigDocAmt: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CuryDocBal: PXFieldState;

	PaymentMethodID: PXFieldState;
}

@gridConfig({ allowDelete: false, allowInsert: false })
export class CSAnswers extends PXView {

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	AttributeID: PXFieldState;

	isRequired: PXFieldState;
	Value: PXFieldState;

}

export class RenewManualNumberingFilter extends PXView {

	ContractCD: PXFieldState<PXFieldOptions.CommitChanges>;

}
