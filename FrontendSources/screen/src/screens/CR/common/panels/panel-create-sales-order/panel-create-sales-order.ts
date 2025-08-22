import {
	createSingle,
	createCollection,
	viewInfo,
	PXFieldState,
	PXView,
	PXFieldOptions,
} from "client-controls";
import { PopupAttributes, PopupUDFAttributes } from "../views";

export abstract class PanelCreateSalesOrderBase {
	@viewInfo({ containerName: "Create Sales Order" })
	CreateOrderParams = createSingle(CreateSalesOrderFilter);

	@viewInfo({ containerName: "Creation Dialog" })
	CustomerInfo = createSingle(CustomerFilter);

	@viewInfo({ containerName: "Create Sales Order" })
	CustomerInfoAttributes = createCollection(PopupAttributes);

	@viewInfo({ containerName: "Create Sales Order" })
	CustomerInfoUDF = createCollection(PopupUDFAttributes);
}

export class CreateSalesOrderFilter extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState;
	MakeQuotePrimary: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalculatePrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalculateDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDocGroupDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	ConfirmManualAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	AMIncludeEstimate: PXFieldState;
	AMCopyConfigurations: PXFieldState;
}

export class CustomerFilter extends PXView {
	AcctCD: PXFieldState;
	ClassID: PXFieldState;
	Email: PXFieldState;
	WarningMessage: PXFieldState;
}
