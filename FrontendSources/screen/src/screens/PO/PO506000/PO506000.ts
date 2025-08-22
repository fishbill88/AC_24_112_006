import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo,
	PXView, PXFieldState, GridColumnShowHideMode, gridConfig, columnConfig } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.PO.POLandedCostProcess', primaryView: 'landedCostDocsList' })
export class PO506000 extends PXScreen {
	@viewInfo({ containerName: 'Documents' })
	landedCostDocsList = createCollection(POLandedCostDoc);
}

@gridConfig({ syncPosition: true, adjustPageSize: true, batchUpdate: true, mergeToolbarWith: 'ScreenToolbar' })
export class POLandedCostDoc extends PXView {
	@columnConfig({ allowCheckAll: true, allowShowHide: GridColumnShowHideMode.False })
	Selected: PXFieldState;
	RefNbr: PXFieldState;
	DocType: PXFieldState;
	@columnConfig({ hideViewLink: true })
	VendorID: PXFieldState;
	VendorID_Vendor_acctName: PXFieldState;
	@columnConfig({ hideViewLink: true })
	VendorLocationID: PXFieldState;
	DocDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	CuryLineTotal: PXFieldState;
	CuryTaxTotal: PXFieldState;
}
