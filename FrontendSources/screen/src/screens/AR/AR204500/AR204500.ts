import {
	createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, TextAlign
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.AR.ARFinChargesMaint", primaryView: "ARFinChargesList", })
export class AR204500 extends PXScreen {

	ARFinChargesList = createSingle(ARFinCharge);
	PercentList = createCollection(ARFinChargePercent);
}

export class ARFinCharge extends PXView {

	FinChargeID: PXFieldState;
	FinChargeDesc: PXFieldState;
	CalculationMethod: PXFieldState;
	TermsID: PXFieldState;
	BaseCurFlag: PXFieldState;
	FinChargeAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinChargeSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategoryID: PXFieldState;
	FeeAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	FeeAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FeeSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	FeeDesc: PXFieldState;
	MinChargeDocumentAmt: PXFieldState;
	ChargingMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedAmount: PXFieldState;
	LineThreshold: PXFieldState;
	MinFinChargeAmount: PXFieldState;

}

@gridConfig({ initNewRow: true })
export class ARFinChargePercent extends PXView {

	@columnConfig({ textAlign: TextAlign.Right })
	FinChargePercent: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	BeginDate: PXFieldState<PXFieldOptions.CommitChanges>;

}
