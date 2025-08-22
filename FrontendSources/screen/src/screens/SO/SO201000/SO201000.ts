import
{
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	headerDescription,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
} from "client-controls";

@graphInfo({graphType: 'PX.Objects.SO.SOOrderTypeMaint', primaryView: 'soordertype'})
export class SO201000 extends PXScreen {

	@viewInfo({containerName: "Order Type Header"})
	soordertype = createSingle(SOOrderTypeHeader);

	@viewInfo({containerName: "Order Type Settings"})
	currentordertype = createSingle(SOOrderTypeHeaderSettings);

	@viewInfo({containerName: "Operations"})
	operations = createCollection(operations);

	@viewInfo({containerName: "Quick Processing"})
	quickProcessPreset = createSingle(quickProcessPreset);
}

export class SOOrderTypeHeader extends PXView {
	OrderType: PXFieldState;
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
	@headerDescription Descr: PXFieldState;
	Template: PXFieldState<PXFieldOptions.CommitChanges>;

	AllowQuickProcess: PXFieldState;
}

export class SOOrderTypeHeaderSettings extends PXView {
	OrderNumberingID: PXFieldState;
	DaysToKeep: PXFieldState;
	HoldEntry: PXFieldState;
	CreditHoldEntry: PXFieldState<PXFieldOptions.CommitChanges>;
	RemoveCreditHoldByPayment: PXFieldState;
	RequireControlTotal: PXFieldState;
	BillSeparately: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipSeparately: PXFieldState<PXFieldOptions.CommitChanges>;
	CalculateFreight: PXFieldState;
	ShipFullIfNegQtyAllowed: PXFieldState;
	DisableAutomaticDiscountCalculation: PXFieldState;
	RecalculateDiscOnPartialShipment: PXFieldState;
	DisableAutomaticTaxCalculation: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowRefundBeforeReturn: PXFieldState;
	CustomerOrderIsRequired: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerOrderValidation: PXFieldState;
	EncryptAndPseudonymizePII: PXFieldState;

	CopyNotes: PXFieldState;
	CopyFiles: PXFieldState;
	CopyHeaderNotesToShipment: PXFieldState;
	CopyHeaderFilesToShipment: PXFieldState;
	CopyHeaderNotesToInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyHeaderFilesToInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyLineNotesToShipment: PXFieldState;
	CopyLineFilesToShipment: PXFieldState;
	CopyLineNotesToInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyLineNotesToInvoiceOnlyNS: PXFieldState;
	CopyLineFilesToInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyLineFilesToInvoiceOnlyNS: PXFieldState;
	CopyLineNotesToChildOrder: PXFieldState;
	CopyLineFilesToChildOrder: PXFieldState;

	InvoiceNumberingID: PXFieldState;
	MarkInvoicePrinted: PXFieldState;
	MarkInvoiceEmailed: PXFieldState;
	InvoiceHoldEntry: PXFieldState;
	UseCuryRateFromSO: PXFieldState;

	SalesAcctDefault: PXFieldState;
	SalesSubMask: PXFieldState;
	FreightAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	FreightAcctDefault: PXFieldState;
	FreightSubID: PXFieldState;
	FreightSubMask: PXFieldState;
	DiscountAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscAcctDefault: PXFieldState;
	DiscountSubID: PXFieldState;
	DiscSubMask: PXFieldState;
	UseShippedNotInvoiced: PXFieldState<PXFieldOptions.CommitChanges>;
	ShippedNotInvoicedAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShippedNotInvoicedSubID: PXFieldState;
	PostLineDiscSeparately: PXFieldState<PXFieldOptions.CommitChanges>;
	UseDiscountSubFromSalesSub: PXFieldState<PXFieldOptions.CommitChanges>;
	COGSAcctDefault: PXFieldState;
	COGSSubMask: PXFieldState;
	AutoWriteOff: PXFieldState<PXFieldOptions.CommitChanges>;

	IntercompanySalesAcctDefault: PXFieldState;
	IntercompanyCOGSAcctDefault: PXFieldState;

	ValidateCCRefundsOrigTransactions: PXFieldState;

	DfltChildOrderType: PXFieldState;
	UseCuryRateFromBL: PXFieldState;

	EnableFSIntegration: PXFieldState<PXFieldOptions.CommitChanges>;

	AMProductionOrderEntry: PXFieldState<PXFieldOptions.CommitChanges>;
	AMProductionOrderEntryOnHold: PXFieldState<PXFieldOptions.CommitChanges>;
	AMEstimateEntry: PXFieldState<PXFieldOptions.CommitChanges>;
	AMConfigurationEntry: PXFieldState;
	AMEnableWarehouseLinkedProduction: PXFieldState;
	AMMTOOrder: PXFieldState;

	Behavior: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultOperation: PXFieldState<PXFieldOptions.CommitChanges>;
	ARDocType: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireShipping: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireLotSerial: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireAllocation: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowQuickProcess: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.ShortList,
	syncPosition: true,
	allowInsert: false,
	allowDelete: false
})
export class operations extends PXView {
	Operation: PXFieldState;
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
	INDocType: PXFieldState;
	@columnConfig({ hideViewLink: true, textField: "OrderPlanType_INPlanType_LocalizedDescr" })
	OrderPlanType: PXFieldState;
	@columnConfig({ hideViewLink: true, textField: "ShipmentPlanType_INPlanType_LocalizedDescr" })
	ShipmentPlanType: PXFieldState;
	OrderType: PXFieldState<PXFieldOptions.Disabled>;
	AutoCreateIssueLine: PXFieldState;
	RequireReasonCode: PXFieldState;
}

export class quickProcessPreset extends PXView {
	AutoRedirect: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoDownloadReports: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateShipment: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintPickList: PXFieldState<PXFieldOptions.CommitChanges>;
	ConfirmShipment: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintLabels: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintConfirmation: PXFieldState<PXFieldOptions.CommitChanges>;
	UpdateIN: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepareInvoiceFromShipment: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepareInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	EmailInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	ReleaseInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintWithDeviceHub: PXFieldState<PXFieldOptions.CommitChanges>;
	DefinePrinterManually: PXFieldState<PXFieldOptions.CommitChanges>;
	PrinterID: PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfCopies: PXFieldState<PXFieldOptions.CommitChanges>;
}
