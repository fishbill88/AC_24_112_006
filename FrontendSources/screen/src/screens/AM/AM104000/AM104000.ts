import {
	PXScreen,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from 'client-controls';

export class ConfigSetup extends PXView {
	ConfigNumberingID: PXFieldState;
	DfltRevisionNbr: PXFieldState;
	ConfigKeyFormat: PXFieldState;
	DefaultKeyNumberingID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsCompletionRequired: PXFieldState;

	HidePriceDetails: PXFieldState;
	Rollup: PXFieldState;
	AllowRollupOverride: PXFieldState;
	Calculate: PXFieldState;
	AllowCalculateOverride: PXFieldState;

	EnableWarehouse: PXFieldState;
	EnableSubItem: PXFieldState;
	EnableDiscount: PXFieldState;
	EnablePrice: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.ConfigSetup', primaryView: 'ConfigSetupRecord' })
export class AM104000 extends PXScreen {
	ConfigSetupRecord = createSingle(ConfigSetup);
}
