import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
	linkCommand,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.MRPDetail', primaryView: 'invlookup' })
export class AM404000 extends PXScreen {
	invlookup = createSingle(Invlookup);
	MRPRecs = createCollection(MRPRecs);
}

export class Invlookup extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	QtyOnHand: PXFieldState;
	UOM: PXFieldState;
	SafetyStock: PXFieldState;
	MinOrderQty: PXFieldState;
	MaxOrderQty: PXFieldState;
	LotQty: PXFieldState;
	AMGroupWindow: PXFieldState;
	ReplenishmentSource: PXFieldState;
	ReplenishmentSiteID: PXFieldState;
	TransferLeadTime: PXFieldState;
	LeadTime: PXFieldState;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class MRPRecs extends PXView {
	Type: PXFieldState;
	PromiseDate: PXFieldState;
	@linkCommand("AMRPPlan$RefNbr$Link") RefNbr: PXFieldState;
	BaseQty: PXFieldState;
	QtyOnHand: PXFieldState;
	@columnConfig({ hideViewLink: true }) ParentInventoryID: PXFieldState;
	ParentSubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) ProductInventoryID: PXFieldState;
	ProductSubItemID: PXFieldState;
	RefType: PXFieldState;
}
