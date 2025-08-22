import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.TaxZoneMaint', primaryView: 'TxZone', showUDFIndicator: true })
export class TX206000 extends PXScreen {

	TxZone = createSingle(TaxZone);
	Details = createCollection(TaxZoneDet);
	TxZoneCurrent = createSingle(TaxZone);
	TaxZoneAddressMappings = createCollection(TaxZoneAddressMapping);

}

export class TaxZone extends PXView {

	TaxZoneID: PXFieldState;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltTaxCategoryID: PXFieldState;
	IsExternal: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxPluginID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternalAPTaxType: PXFieldState<PXFieldOptions.CommitChanges>;
	IsManualVATZone: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxID: PXFieldState;
	ShowTaxTabExpr: PXFieldState;
	ShowZipTabExpr: PXFieldState;
	MappingType: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class TaxZoneDet extends PXView {

	Tax__TaxType: PXFieldState;
	TaxID: PXFieldState;
	Tax__Descr: PXFieldState;
	Tax__TaxCalcRule: PXFieldState;
	Tax__TaxApplyTermsDisc: PXFieldState;
	Tax__DirectTax: PXFieldState;

}


@gridConfig({ syncPosition: true })
export class TaxZoneAddressMapping extends PXView {

	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	StateID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromPostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	ToPostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;

}
