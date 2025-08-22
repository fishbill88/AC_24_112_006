import {
	commitChanges,
	createCollection,
	createSingle,
	graphInfo,
	PXFieldState,
	PXView,
	PXScreen,
	PXFieldOptions,
	viewInfo,
	PXActionState,
	gridConfig,
} from "client-controls";

export class CarrierPlugin extends PXView {
	CarrierPluginID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	PluginTypeName: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitType: PXFieldState<PXFieldOptions.CommitChanges>;
	KilogramUOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CentimeterUOM: PXFieldState<PXFieldOptions.CommitChanges>;
	PoundUOM: PXFieldState<PXFieldOptions.CommitChanges>;
	InchUOM: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReturnLabelNotification: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	syncPosition: true
})
export class CarrierPluginDetail extends PXView {
	Certify: PXActionState;
	DetailID: PXFieldState;
	Descr: PXFieldState;
	Value: PXFieldState;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true
})
export class CarrierPluginCustomer extends PXView {
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID_description: PXFieldState;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CarrierAccount: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState;
	CarrierBillingType: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true
})
export class SETerritoriesMapping extends PXView {
	@commitChanges CountryID: PXFieldState;
	@commitChanges StateID: PXFieldState;
	StateName: PXFieldState;
	CountryName: PXFieldState;
}

@gridConfig({
	adjustPageSize: true,
	allowDelete: false,
	allowInsert: false,
	syncPosition: true
})
export class StampsAccountInfo extends PXView {
	BuyStampsPostage: PXActionState;
	GetStampsAccountInfo: PXActionState;

	AccountInfo: PXFieldState;
	AccountInfoValue: PXFieldState;
}

export class StampsBuyPostageParam extends PXView {
	BuyPostageAmount: PXFieldState;
}

@graphInfo({
	graphType: 'PX.Objects.CS.CarrierPluginMaint',
	primaryView: 'Plugin',
	bpEventsIndicator: false
})
export class CS207700 extends PXScreen {

	@viewInfo({containerName: 'Carrier Plug-in Summary'})
	Plugin = createSingle(CarrierPlugin);

	@viewInfo({containerName: 'Plug-in Parameters'})
	Details = createCollection(CarrierPluginDetail);

	@viewInfo({containerName: 'Customer Accounts'})
	CustomerAccounts = createCollection(CarrierPluginCustomer);

	@viewInfo({containerName: 'Stamps.com Account Info'})
	StampsAccountInfoRecord = createCollection(StampsAccountInfo);

	@viewInfo({containerName: 'Territories Mapping'})
	ShipEngineTerritoriesMappings = createCollection(SETerritoriesMapping);

	@viewInfo({ containerName: 'Specify Postage Amount' })
	BuyPostageParamsView = createSingle(StampsBuyPostageParam);
}
