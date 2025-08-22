import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs,
	PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings,
	PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.AR.ARCashSaleEntry", primaryView: "Document", udfTypeField: "DocType", showUDFIndicator: true })
export class AR304000 extends PXScreen {

	SendARInvoiceMemo: PXActionState;
	AddressLookup: PXActionState;
	ShippingAddressLookup: PXActionState;
	AddressLookupSelectAction: PXActionState;

	@viewInfo({ containerName: "Invoice Summary" })
	Document = createSingle(ARRegister);

	CurrentDocument = createSingle(ARRegister2);

	@viewInfo({ containerName: "Details" })
	Transactions = createCollection(ARTran);

	@viewInfo({ containerName: "Bill-To Contact" })
	Billing_Contact = createSingle(ARContact);

	@viewInfo({ containerName: "Bill-To Address" })
	Billing_Address = createSingle(ARAddress);

	@viewInfo({ containerName: "Ship-To Contact" })
	Shipping_Contact = createSingle(ARShippingContact);

	@viewInfo({ containerName: "Ship-To Address" })
	Shipping_Address = createSingle(ARShippingAddress);

	@viewInfo({ containerName: "Taxes" })
	Taxes = createCollection(TaxTran);

	@viewInfo({ containerName: "Approvals" })
	Approval = createCollection(EPApproval);

	@viewInfo({ containerName: "Commissions" })
	salesPerTrans = createCollection(ARSalesPerTran);

	@viewInfo({ containerName: "Charges" })
	PaymentCharges = createCollection(ARPaymentChargeTran);

	@viewInfo({ containerName: "Card Processing" })
	ccProcTran = createCollection(CCProcTran);

	@viewInfo({ containerName: "Enter Reason" })
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectFilter);

	@viewInfo({ containerName: "Reassign Approval" })
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);

	@viewInfo({ containerName: "Address Lookup" })
	AddressLookupFilter = createSingle(AddressLookupFilter);

	@viewInfo({ containerName: "currencyinfo" })
	CurrencyInfo = createSingle(CurrencyInfo);

}

export class ARRegister extends PXView {

	DocType: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	AdjDate: PXFieldState<PXFieldOptions.CommitChanges>;
	AdjFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	DepositAfter: PXFieldState;
	DocDesc: PXFieldState<PXFieldOptions.Multiline>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	PMInstanceID: PXFieldState<PXFieldOptions.CommitChanges>;
	CCPaymentStateDescr: PXFieldState<PXFieldOptions.Disabled>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDetailExtPriceTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryLineDiscTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryRoundDiff: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	CuryOrigDocAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDocBal: PXFieldState<PXFieldOptions.Disabled>;
	CuryOrigDiscAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryChargeAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryConsolidateChargeTotal: PXFieldState<PXFieldOptions.Disabled>;
	RefTranExtNbr: PXFieldState;
	CuryID: PXFieldState;
}

export class ARRegister2 extends PXView {

	BatchNbr: PXFieldState<PXFieldOptions.Disabled>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARSubID: PXFieldState;
	OrigRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	Printed: PXFieldState<PXFieldOptions.Disabled>;
	DontPrint: PXFieldState;
	Emailed: PXFieldState<PXFieldOptions.Disabled>;
	DontEmail: PXFieldState;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	Cleared: PXFieldState<PXFieldOptions.CommitChanges>;
	ClearDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DepositAsBatch: PXFieldState<PXFieldOptions.CommitChanges>;
	Deposited: PXFieldState;
	DepositDate: PXFieldState<PXFieldOptions.Disabled>;
	DepositNbr: PXFieldState;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternalTaxExemptionNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	AvalaraCustomerUsageType: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryVatTaxableTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatExemptTotal: PXFieldState<PXFieldOptions.Disabled>;
	SalesPersonID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryCommnblAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryCommnAmt: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	initNewRow: true,
	syncPosition: true
})
export class ARTran extends PXView {

	ViewSchedule: PXActionState;
	InventoryID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;

	Qty: PXFieldState;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowNull: false })
	ManualPrice: PXFieldState<PXFieldOptions.CommitChanges>;

	CuryExtPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscPct: PXFieldState;
	CuryDiscAmt: PXFieldState;
	ManualDisc: PXFieldState;
	CuryTranAmt: PXFieldState;
	TranDesc: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SalesPersonID: PXFieldState;

	Commissionable: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	DefScheduleID: PXFieldState;
	DeferredCode: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TaxCategoryID: PXFieldState;

	TaskID: PXFieldState;
	CostCodeID: PXFieldState;
	SkipLineDiscounts: PXFieldState;

	DRTermStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DRTermEndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	LineNbr: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryUnitPriceDR: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	DiscPctDR: PXFieldState;
}

export class ARContact extends PXView {

	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState;
}

export class ARAddress extends PXView {

	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class ARShippingContact extends PXView {

	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState;
}

export class ARShippingAddress extends PXView {

	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class TaxTran extends PXView {

	@columnConfig({ allowUpdate: false })
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	TaxRate: PXFieldState;

	CuryTaxableAmt: PXFieldState;
	CuryTaxAmt: PXFieldState;
	CuryExemptedAmt: PXFieldState;
	TaxUOM: PXFieldState;
	TaxableQty: PXFieldState;
	Tax__TaxType: PXFieldState;
	Tax__PendingTax: PXFieldState;
	Tax__ReverseTax: PXFieldState;
	Tax__ExemptTax: PXFieldState;
	Tax__StatisticalTax: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false
})
export class EPApproval extends PXView {

	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;
	@columnConfig({ allowUpdate: false, allowNull: false })
	Status: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Reason: PXFieldState;

	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false
})
export class ARSalesPerTran extends PXView {

	CommnPct: PXFieldState;
	CommnAmt: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CuryCommnAmt: PXFieldState;

	CommnblAmt: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CuryCommnblAmt: PXFieldState;

	SalespersonID: PXFieldState;
	@columnConfig({ visible: false, allowShowHide: GridColumnShowHideMode.False })
	AdjdDocType: PXFieldState<PXFieldOptions.Hidden>;
}

export class ARPaymentChargeTran extends PXView {

	EntryTypeID: PXFieldState;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState;

	CuryTranAmt: PXFieldState;

	TranDesc: PXFieldState;
}

export class CCProcTran extends PXView {

	TranNbr: PXFieldState;

	ProcStatus: PXFieldState;
	ProcessingCenterID: PXFieldState;
	CVVVerificationStatus: PXFieldState;
	TranType: PXFieldState;
	TranStatus: PXFieldState;
	Amount: PXFieldState;
	RefTranNbr: PXFieldState;
	PCTranNumber: PXFieldState;
	AuthNumber: PXFieldState;
	PCResponseReasonText: PXFieldState;
	StartTime: PXFieldState;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {

	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState;
	CuryEffDate: PXFieldState;
	RecipRate: PXFieldState;
	SampleCuryRate: PXFieldState;
	SampleRecipRate: PXFieldState;
	CuryID: PXFieldState;
}

//<!--#include file = "~\Pages\Includes\CRApprovalReasonPanel.inc"-- >
export class ReasonApproveRejectFilter extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges>;
}

//<!--#include file = "~\Pages\Includes\EPReassignApproval.inc"-- >
export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

//<!--#include file = "~\Pages\Includes\AddressLookupPanel.inc"-- >
export class AddressLookupFilter extends PXView {

	SearchAddress: PXFieldState;
	ViewName: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	AddressLine3: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
}
