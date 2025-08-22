import {
	createCollection,
	createSingle,
	graphInfo,
	PXFieldState,
	PXView,
	PXScreen,
	PXFieldOptions,
	PXActionState,
	gridConfig,
} from "client-controls";

export class AddressValidatorPlugin extends PXView {
	AddressValidatorPluginID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	PluginTypeName: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	adjustPageSize: true
})
export class AddressValidatorPluginDetail extends PXView {
	SettingID: PXActionState;
	Description: PXFieldState;
	Value: PXFieldState;
}

@graphInfo({
	graphType: 'PX.Objects.CS.AddressValidatorPluginMaint',
	primaryView: 'Plugin'
})
export class CS103000 extends PXScreen {

	Plugin = createSingle(AddressValidatorPlugin);

	Details = createCollection(AddressValidatorPluginDetail);
}
