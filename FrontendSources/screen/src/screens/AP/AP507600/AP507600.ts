import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridPreset } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.Localizations.CA.T5018Fileprocessing', primaryView: 'MasterView' })
export class AP507600 extends PXScreen {

	MasterView = createSingle(T5018MasterTable);
	MasterViewSummary = createSingle(T5018MasterTable);
	DetailsView = createCollection(T5018EFileRow);

}

export class T5018MasterTable extends PXView {
	OrganizationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Year: PXFieldState<PXFieldOptions.CommitChanges>;
	Revision: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionSubmitted: PXFieldState<PXFieldOptions.CommitChanges>;
	FilingType: PXFieldState<PXFieldOptions.CommitChanges>;
	ThresholdAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate: PXFieldState;
	ToDate: PXFieldState;
	Language: PXFieldState;
	ProgramNumber: PXFieldState;
	TransmitterNumber: PXFieldState;
	AcctName: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	Province: PXFieldState;
	Country: PXFieldState;
	PostalCode: PXFieldState;
	Name: PXFieldState;
	AreaCode: PXFieldState;
	Phone: PXFieldState;
	ExtensionNbr: PXFieldState;
	EMail: PXFieldState;
	SecondEmail: PXFieldState;

}

@gridConfig({ preset: GridPreset.Inquiry, adjustPageSize: true })
export class T5018EFileRow extends PXView {
	@columnConfig({ allowUpdate: false })
	VAcctCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	VendorName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	OrganizationName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TotalServiceAmount: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Amount: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ReportType: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TaxRegistrationID: PXFieldState;
}