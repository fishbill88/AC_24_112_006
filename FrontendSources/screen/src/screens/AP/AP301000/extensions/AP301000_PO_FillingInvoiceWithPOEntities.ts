import { AP301000 } from '../AP301000';
import { PXView, PXFieldState, PXFieldOptions, createSingle, createCollection, columnConfig, GridColumnShowHideMode, viewInfo, gridConfig, GridPreset } from 'client-controls';

export interface AP3010000_PO_FillingInvoiceWithPOEntities extends AP301000 { }
export class AP3010000_PO_FillingInvoiceWithPOEntities {

	// PX.Objects.PO.GraphExtensions.APInvoiceSmartPanel*

	filter = createSingle(POReceiptFilter);
	poreceiptslist = createCollection(POReceipt);
	poorderslist = createCollection(POReceipt);
	poReceiptLinesSelection = createCollection(POReceiptLineAdd);
	orderfilter = createSingle(POOrderFilter);
	subcontracts = createCollection(POOrderRS);
	poorderlineslist = createCollection(POOrderRS);
	landedCostFilter = createSingle(POLandedCostDetailFilter);
	LandedCostDetailsAdd = createCollection(POLandedCostDetailS);
	LinkLineLandedCostDetail = createCollection(POLandedCostDetailS2);
	linkLineFilter = createSingle(LinkLineFilter);
	linkLineReceiptTran = createCollection(POReceiptLineS);
	linkLineOrderTran = createCollection(POLineS);

	@viewInfo({ containerName: 'Add Subcontract Line' })
	SubcontractLines = createCollection(POLineRS2);
}

export class POReceiptFilter extends PXView {

	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ preset: GridPreset.Inquiry })
export class POReceipt extends PXView {

	@columnConfig({ allowCheckAll: true, allowSort: false })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	ReceiptNbr: PXFieldState;
	ReceiptType: PXFieldState;
	VendorID: PXFieldState;
	VendorLocationID: PXFieldState;
	ReceiptDate: PXFieldState;
	CuryID: PXFieldState;
	OrderQty: PXFieldState;
	UnbilledQty: PXFieldState;
	CuryOrderTotal: PXFieldState;

	OrderNbr: PXFieldState;
	OrderType: PXFieldState;
	OrderDate: PXFieldState;
	UnbilledOrderQty: PXFieldState;
	CuryUnbilledOrderTotal: PXFieldState;

}

@gridConfig({ preset: GridPreset.Inquiry })
export class POReceiptLineAdd extends PXView {

	@columnConfig({ allowCheckAll: true, allowSort: false, allowShowHide: GridColumnShowHideMode.False })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	PONbr: PXFieldState;
	POType: PXFieldState;
	ReceiptNbr: PXFieldState;
	POReceipt__InvoiceNbr: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	SiteID: PXFieldState;
	UOM: PXFieldState;
	LineNbr: PXFieldState;
	CuryID: PXFieldState;
	ReceiptQty: PXFieldState;
	CuryExtCost: PXFieldState;
	UnbilledQty: PXFieldState;
	TranDesc: PXFieldState;
	VendorID: PXFieldState;
	POReceipt__VendorLocationID: PXFieldState;

}

export class POOrderFilter extends PXView {

	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowBilledLines: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowUnbilledLines: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ preset: GridPreset.Inquiry })
export class POOrderRS extends PXView {

	@columnConfig({ allowCheckAll: true, allowSort: false, allowShowHide: GridColumnShowHideMode.False })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	SubcontractNbr: PXFieldState;
	POLine__ProjectID: PXFieldState;
	VendorID: PXFieldState;
	VendorLocationID: PXFieldState;
	OrderDate: PXFieldState;
	CuryID: PXFieldState;
	SubcontractTotal: PXFieldState;
	SubcontractBilledQty: PXFieldState;
	CurySubcontractBilledTotal: PXFieldState;

	ReceiptNbr: PXFieldState;
	ReceiptType: PXFieldState;
	ReceiptDate: PXFieldState;
	OrderQty: PXFieldState;
	UnbilledQty: PXFieldState;
	CuryOrderTotal: PXFieldState;

}

export class POLandedCostDetailFilter extends PXView {

	LandedCostDocRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	LandedCostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiptType: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiptNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ preset: GridPreset.Inquiry })
export class POLandedCostDetailS extends PXView {

	@columnConfig({ allowCheckAll: true, allowSort: false, allowShowHide: GridColumnShowHideMode.False })
	Selected: PXFieldState;

	DocType: PXFieldState;
	RefNbr: PXFieldState;
	VendorRefNbr: PXFieldState;
	LandedCostCodeID: PXFieldState;
	Descr: PXFieldState;
	CuryLineAmt: PXFieldState;
	CuryID: PXFieldState;
	TaxCategoryID: PXFieldState;
	LineNbr: PXFieldState;
	INDocType: PXFieldState;
	INRefNbr: PXFieldState;

}

@gridConfig({ preset: GridPreset.Details })
export class POLandedCostDetailS2 extends POLandedCostDetailS { }


export class LinkLineFilter extends PXView {

	POOrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState;
	SelectedMode: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ preset: GridPreset.Inquiry })
export class POReceiptLineS extends PXView {

	@columnConfig({ allowCheckAll: false, allowSort: false, allowShowHide: GridColumnShowHideMode.False })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	PONbr: PXFieldState;
	POType: PXFieldState;
	ReceiptNbr: PXFieldState;
	POReceipt__InvoiceNbr: PXFieldState;
	SubItemID: PXFieldState;
	SiteID: PXFieldState;
	LineNbr: PXFieldState;
	CuryID: PXFieldState;
	ReceiptQty: PXFieldState;
	CuryExtCost: PXFieldState;
	UnbilledQty: PXFieldState;
	TranDesc: PXFieldState;

}

@gridConfig({ preset: GridPreset.Inquiry })
export class POLineS extends PXView {

	@columnConfig({ allowCheckAll: false, allowSort: false, allowShowHide: GridColumnShowHideMode.False })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	POOrder__OrderNbr: PXFieldState;
	POOrder__OrderType: PXFieldState;
	LineNbr: PXFieldState;
	POOrder__VendorRefNbr: PXFieldState;
	SubItemID: PXFieldState;
	SiteID: PXFieldState;
	POOrder__CuryID: PXFieldState;
	OrderQty: PXFieldState;
	curyLineAmt: PXFieldState;
	UnbilledQty: PXFieldState;
	CuryUnbilledAmt: PXFieldState;
	TranDesc: PXFieldState;

}

@gridConfig({ preset: GridPreset.Inquiry })
export class POLineRS2 extends PXView {
	Selected: PXFieldState;
	SubcontractNbr: PXFieldState;
	ProjectID: PXFieldState;
	TaskID: PXFieldState;
	CostCodeID: PXFieldState;
	VendorID: PXFieldState;
	VendorLocationID: PXFieldState;
	SubcontractDate: PXFieldState;
	CuryID: PXFieldState;
	InventoryID: PXFieldState;
	CuryLineAmt: PXFieldState;
	UnbilledQty: PXFieldState;
	CuryUnbilledAmt: PXFieldState;
	BilledQty: PXFieldState;
	CuryBilledAmt: PXFieldState;
}
