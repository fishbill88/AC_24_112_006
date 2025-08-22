import {
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,

	createSingle,

	graphInfo,
	viewInfo,
	columnConfig,
} from "client-controls";

@graphInfo({graphType: 'PX.Objects.SO.SOSetupMaint', primaryView: 'sosetup'})
export class SO101000 extends PXScreen {
	@viewInfo({containerName: "SO Preferences"})
	sosetup = createSingle(sosetup);
}

export class sosetup extends PXView {
	DefaultOrderType: PXFieldState;
	TransferOrderType: PXFieldState;
	ShipmentNumberingID: PXFieldState;

	@columnConfig({ allowNull: false })
	PickingWorksheetNumberingID: PXFieldState;

	AdvancedAvailCheck: PXFieldState;

	MinGrossProfitValidation: PXFieldState;
	UsePriceAdjustmentMultiplier: PXFieldState;
	IgnoreMinGrossProfitCustomerPrice: PXFieldState;
	IgnoreMinGrossProfitCustomerPriceClass: PXFieldState;
	IgnoreMinGrossProfitPromotionalPrice: PXFieldState;

	FreightAllocation: PXFieldState;

	FreeItemShipping: PXFieldState;
	HoldShipments: PXFieldState;
	RequireShipmentTotal: PXFieldState;
	AddAllToShipment: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateZeroShipments: PXFieldState<PXFieldOptions.CommitChanges>;

	CreditCheckError: PXFieldState;
	UseShipDateForInvoiceDate: PXFieldState;

	AutoReleaseIN: PXFieldState;

	SalesProfitabilityForNSKits: PXFieldState;

	DfltIntercompanyOrderType: PXFieldState;
	OrderType: PXFieldState;
	DfltIntercompanyRMAType: PXFieldState;
	DisableAddingItemsForIntercompany: PXFieldState;
	DisableEditingPricesDiscountsForIntercompany: PXFieldState<PXFieldOptions.CommitChanges>;

	ShowOnlyAvailableRelatedItems: PXFieldState;
	DefaultReturnOrderType: PXFieldState;

	OrderRequestApproval: PXFieldState<PXFieldOptions.Disabled | PXFieldOptions.Hidden>;
}
