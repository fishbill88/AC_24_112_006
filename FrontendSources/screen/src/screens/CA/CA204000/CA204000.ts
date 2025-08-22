import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, selectorSettings, PXFieldOptions, columnConfig, GridColumnShowHideMode, GridColumnType } from "client-controls";

@graphInfo({graphType: "PX.Objects.CA.PaymentMethodMaint", primaryView: "PaymentMethod", })
export class CA204000 extends PXScreen {

	@viewInfo({containerName: "Payment Method Summary"})
	PaymentMethod = createSingle(PaymentMethod);

	PaymentMethodCurrent = createSingle(PaymentMethod2);

	@viewInfo({containerName: "Allowed Cash Accounts"})
	CashAccounts = createCollection(PaymentMethodAccount);

	@viewInfo({containerName: "Payment Method Details"})
	DetailsForReceivable = createCollection(PaymentMethodDetail);

	@viewInfo({containerName: "Payment Method Details"})
	DetailsForVendor = createCollection(PaymentMethodDetail2);

	@viewInfo({containerName: "Remittance Settings"})
	DetailsForCashAccount = createCollection(PaymentMethodDetail3);

	@viewInfo({containerName: "Processing Centers"})
	ProcessingCenters = createCollection(CCProcessingCenterPmntMethod);

	@viewInfo({containerName: "Selection"})
	PlugInFilter = createSingle(PlugInFilter);

	@viewInfo({containerName: "Plug-In Settings"})
	aCHPlugInParameters = createCollection(ACHPlugInParameter);

	@viewInfo({containerName: "Plug-In Settings"})
	PlugInParameters = createCollection(ACHPlugInParameter2);
}

// Views

export class PaymentMethod extends PXView  {
	PaymentMethodID : PXFieldState;
	HasProcessingCenters : PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.CommitChanges>;
	IsUsingPlugin : PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.CommitChanges>;
	IsActive : PXFieldState;
	ContainsPersonalData : PXFieldState;
	PaymentType : PXFieldState<PXFieldOptions.CommitChanges>;
	DirectDepositFileFormat : PXFieldState<PXFieldOptions.CommitChanges>;
	Descr : PXFieldState;
	UseForAP : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	UseForAR : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	UseForPR : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	PaymentDateToBankDate : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	UseForCA : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
}

export class PaymentMethod2 extends PXView  {
	ARIsProcessingRequired : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	IsAccountNumberRequired : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	ARVoidOnDepositAccount : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	ARHasBillingInfo : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	ARDefaultVoidDateToDocumentDate : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	APAdditionalProcessing : PXFieldState<PXFieldOptions.CommitChanges>;
	APCheckReportID : PXFieldState<PXFieldOptions.CommitChanges>;
	APStubLines : PXFieldState<PXFieldOptions.CommitChanges>;
	APPrintRemittance : PXFieldState<PXFieldOptions.CommitChanges>;
	@selectorSettings("ScreenID", "")
	APRemittanceReportID : PXFieldState<PXFieldOptions.CommitChanges>;
	APBatchExportMethod : PXFieldState<PXFieldOptions.CommitChanges>;
	APBatchExportSYMappingID : PXFieldState<PXFieldOptions.CommitChanges>;
	APBatchExportPlugInTypeName : PXFieldState<PXFieldOptions.CommitChanges>;
	SkipPaymentsWithZeroAmt : PXFieldState<PXFieldOptions.CommitChanges>;
	RequireBatchSeqNum : PXFieldState<PXFieldOptions.CommitChanges>;
	SkipExport : PXFieldState<PXFieldOptions.CommitChanges>;
	APRequirePaymentRef : PXFieldState<PXFieldOptions.CommitChanges>;
	PRProcessing : PXFieldState<PXFieldOptions.CommitChanges>;
	PRCheckReportID : PXFieldState<PXFieldOptions.CommitChanges>;
	PRBatchExportSYMappingID : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true
})
export class PaymentMethodAccount extends PXView  {
	CashAccountID : PXFieldState;
	CashAccountID_CashAccount_Descr : PXFieldState;
	CashAccount__BranchID : PXFieldState;
	UseForAP : PXFieldState;
	UseForPR : PXFieldState;
	APIsDefault : PXFieldState;
	APAutoNextNbr : PXFieldState;
	APLastRefNbr : PXFieldState;
	APBatchLastRefNbr : PXFieldState;
	APQuickBatchGeneration : PXFieldState;
	UseForAR : PXFieldState;
	ARIsDefault : PXFieldState;
	ARIsDefaultForRefund : PXFieldState;
	ARAutoNextNbr : PXFieldState;
	ARLastRefNbr : PXFieldState;
}

@gridConfig({
	initNewRow: true
})
export class PaymentMethodDetail extends PXView  {
	DetailID : PXFieldState;
	EntryMask : PXFieldState;
	ValidRegexp : PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})	PaymentMethodID : PXFieldState;
	Descr : PXFieldState;
	IsRequired : PXFieldState;
	IsEncrypted : PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})	IsIdentifier : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})	IsExpirationDate : PXFieldState<PXFieldOptions.CommitChanges>;
	IsCVV : PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})	IsOwnerName : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})	IsCCProcessingID : PXFieldState<PXFieldOptions.CommitChanges>;
	OrderIndex : PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})	DisplayMask : PXFieldState;
}

@gridConfig({
	initNewRow: true
})
export class PaymentMethodDetail2 extends PXView  {
	DetailID : PXFieldState;
	Descr : PXFieldState;
	IsRequired : PXFieldState;
	OrderIndex : PXFieldState;
	EntryMask : PXFieldState;
	ValidRegexp : PXFieldState;
	ControlType : PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultValue : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class PaymentMethodDetail3 extends PXView  {
	DetailID : PXFieldState;
	Descr : PXFieldState;
	IsRequired : PXFieldState;
	OrderIndex : PXFieldState;
	EntryMask : PXFieldState;
	ValidRegexp : PXFieldState;
	ControlType : PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultValue : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CCProcessingCenterPmntMethod extends PXView  {
	ProcessingCenterID : PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive : PXFieldState;
	IsDefault : PXFieldState;
	FundHoldPeriod : PXFieldState;
	ReauthDelay : PXFieldState;
}

export class PlugInFilter extends PXView  {
	ShowAllSettings : PXFieldState<PXFieldOptions.CommitChanges>;
	ShowOffsetSettings : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ACHPlugInParameter extends PXView  {

	ParameterID : PXFieldState;
	ParameterCode : PXFieldState;
	Description : PXFieldState;

	@columnConfig({width: 1})
	IsFormula : PXFieldState;

	@columnConfig({allowSort: false, fullState: true})
	Value : PXFieldState<PXFieldOptions.CommitChanges>;

}

export class ACHPlugInParameter2 extends PXView  {
	ParameterID : PXFieldState;
	ParameterCode : PXFieldState;
	Value : PXFieldState;
	Order : PXFieldState;
	UsedIn : PXFieldState;
	Type : PXFieldState;
	Required : PXFieldState;
	Visible : PXFieldState;
	IsGroupHeader : PXFieldState;
	IsAvailableInShortForm : PXFieldState;
	IsFormula : PXFieldState;
}