import
{
	PXView, PXFieldState, gridConfig, ICurrencyInfo,
	PXFieldOptions, columnConfig, GridPreset
} from "client-controls";

// Views

export class RQBiddingVendorHeader extends PXView {
	ReqNbr : PXFieldState;
	VendorID : PXFieldState;
	VendorLocationID : PXFieldState;
	EntryDate : PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	TotalQuoteQty : PXFieldState<PXFieldOptions.Disabled>;
	CuryTotalQuoteExtCost : PXFieldState<PXFieldOptions.Disabled>;
}

export class RQBiddingVendor extends PXView {
	ExpireDate : PXFieldState;
	PromisedDate : PXFieldState;
	FOBPoint : PXFieldState;
	ShipVia : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class RQRequisitionLineBidding extends PXView {
	@columnConfig({ hideViewLink: true })
	InventoryID : PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubItemID : PXFieldState;
	Description : PXFieldState;
	AlternateID : PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM : PXFieldState;
	@columnConfig({ allowNull: false })
	OrderQty : PXFieldState;
	@columnConfig({ allowNull: false })
	MinQty : PXFieldState;
	@columnConfig({ allowNull: false })
	QuoteQty : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	CuryQuoteUnitCost : PXFieldState<PXFieldOptions.CommitChanges>;
	QuoteNumber : PXFieldState;
	@columnConfig({ allowNull: false })
	CuryQuoteExtCost : PXFieldState;
}

export class POContact extends PXView {
	OverrideContact : PXFieldState<PXFieldOptions.CommitChanges>;
	FullName : PXFieldState;
	Attention : PXFieldState;
	Phone1 : PXFieldState;
	Email : PXFieldState;
}

export class POAddress extends PXView {
	OverrideAddress : PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1 : PXFieldState;
	AddressLine2 : PXFieldState;
	City : PXFieldState;
	CountryID : PXFieldState<PXFieldOptions.CommitChanges>;
	State : PXFieldState;
	PostalCode : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
}
