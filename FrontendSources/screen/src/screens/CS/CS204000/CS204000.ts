import {
	PXScreen,
	graphInfo,
	PXView,
	createCollection,
	PXFieldState,
	createSingle,
	PXFieldOptions,
	gridConfig,
	GridPreset
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CS.CountryMaint",
	primaryView: "Country",
})
export class CS204000 extends PXScreen {
	Country = createSingle(Country);
	CountryStates = createCollection(CountryStates);
}

export class Country extends PXView {
	CountryID: PXFieldState;
	Description: PXFieldState;
	AddressValidatorPluginID: PXFieldState;
	AutoOverrideAddress: PXFieldState;
	SalesTerritoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryValidationMethod: PXFieldState;
	CountryRegexp: PXFieldState;
	StateValidationMethod: PXFieldState;
	ZipCodeMask: PXFieldState;
	ZipCodeRegexp: PXFieldState;
	LanguageID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
	suppressNoteFiles: true,
	quickFilterFields: ["StateID", "Name"],
})
export class CountryStates extends PXView {
	StateID: PXFieldState;
	Name: PXFieldState;
	SalesTerritoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesTerritoryID_Description: PXFieldState;
	StateRegexp: PXFieldState;
	NonTaxable: PXFieldState;
	LocationCode: PXFieldState;
}
