import {
	PXScreen, createSingle, createCollection, graphInfo, autoRefresh, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, linkCommand, PXActionState
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CT.TemplateMaint', primaryView: 'Templates' })
export class CT202000 extends PXScreen {

	CRAttribute_ViewDetails: PXActionState;

	Templates = createSingle(ContractTemplate);
	CurrentTemplate = createSingle(ContractTemplate);
	Billing = createSingle(ContractBillingSchedule);
	ContractDetails = createCollection(ContractDetail);
	Contracts = createCollection(Contract);
	SLAMapping = createCollection(ContractSLAMapping);
	AttributeGroup = createCollection(CSAttributeGroupList);

}

export class ContractTemplate extends PXView {

	ContractCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Status: PXFieldState;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Duration: PXFieldState<PXFieldOptions.CommitChanges>;
	DurationType: PXFieldState<PXFieldOptions.CommitChanges>;
	Refundable: PXFieldState<PXFieldOptions.CommitChanges>;
	RefundPeriod: PXFieldState<PXFieldOptions.CommitChanges>;
	Days: PXFieldState<PXFieldOptions.Disabled>;
	AutoRenew: PXFieldState;
	AutoRenewDays: PXFieldState;
	DaysBeforeExpiration: PXFieldState<PXFieldOptions.Disabled>;
	GracePeriod: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	EffectiveFrom: PXFieldState;
	DiscontinueAfter: PXFieldState;
	ScheduleStartsOn: PXFieldState;
	DetailedBilling: PXFieldState;
	AllowOverrideFormulaDescription: PXFieldState;
	CaseItemID: PXFieldState;
	AllowOverride: PXFieldState;
	AutomaticReleaseAR: PXFieldState;

}

export class ContractBillingSchedule extends PXView {

	Type: PXFieldState;
	BillTo: PXFieldState;
	InvoiceFormula: PXFieldState;
	TranFormula: PXFieldState;

}

@gridConfig({ adjustPageSize: true })
export class ContractDetail extends PXView {

	@autoRefresh
	ContractItemID: PXFieldState<PXFieldOptions.CommitChanges>;

	Description: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePriceVal: PXFieldState;
	FixedRecurringPriceVal: PXFieldState;
	UsagePriceVal: PXFieldState;
	RenewalPriceVal: PXFieldState;

}

@gridConfig({ adjustPageSize: true })
export class Contract extends PXView {

	ContractCD: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;
	Status: PXFieldState;
	StartDate: PXFieldState;
	ExpireDate: PXFieldState;
	Description: PXFieldState;

}

@gridConfig({ adjustPageSize: true, allowDelete: false, allowInsert: false })
export class ContractSLAMapping extends PXView {

	Severity: PXFieldState;
	Period: PXFieldState;

}

@gridConfig({ adjustPageSize: true })
export class CSAttributeGroupList extends PXView {

	@columnConfig({ allowNull: false })
	IsActive: PXFieldState;

	@linkCommand("CRAttribute_ViewDetails")
	AttributeID: PXFieldState;

	Description: PXFieldState;
	SortOrder: PXFieldState;
	Required: PXFieldState;

	@columnConfig({ allowNull: true })
	CSAttribute__IsInternal: PXFieldState;

	@columnConfig({ allowNull: false })
	ControlType: PXFieldState;

	@columnConfig({ allowNull: true })
	DefaultValue: PXFieldState;

}
