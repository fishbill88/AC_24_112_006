import {
	createCollection, createSingle,
	PXScreen, PXActionState, PXView, PXFieldState,
	graphInfo, viewInfo, gridConfig, columnConfig, linkCommand,
	PXPageLoadBehavior, PXFieldOptions, TextAlign, GridColumnType
} from 'client-controls';

@graphInfo({
	graphType: "PX.Objects.AR.ARExpiringCardsProcess", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR512000 extends PXScreen {
	ViewCustomer: PXActionState;
	ViewPaymentMethod: PXActionState;

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(ARExpiringCardFilter);

   	@viewInfo({containerName: "Card List"})
	Cards = createCollection(CustomerPaymentMethod);
}

export class ARExpiringCardFilter extends PXView {
	BeginDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpireXDays: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ActiveOnly: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultOnly: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	mergeToolbarWith: "ScreenToolbar",
	quickFilterFields: ['BAccountID', 'Customer__AcctName', 'Customer__CustomerClassID', 'PaymentMethodID']
})
export class CustomerPaymentMethod extends PXView {
	@columnConfig({ allowNull: false, allowCheckAll: true, type: GridColumnType.CheckBox })
	Selected: PXFieldState;

	@linkCommand("ViewCustomer")
	BAccountID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Customer__AcctName: PXFieldState;

	@columnConfig({ allowUpdate: false, format: ">aaaaaaaaaa" })
	Customer__CustomerClassID: PXFieldState;

	@linkCommand("ViewPaymentMethod")
	PaymentMethodID: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	Descr: PXFieldState;

	@columnConfig({ allowNull: false, type: GridColumnType.CheckBox })
	IsActive: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ExpirationDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Contact__EMail: PXFieldState;

	@columnConfig({ allowUpdate: false, format: "CCCCCCCCCCCCCCCCCCCC" })
	Contact__Phone1: PXFieldState;

	@columnConfig({ allowUpdate: false, format: "CCCCCCCCCCCCCCCCCCCC" })
	Contact__Fax: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Contact__WebSite: PXFieldState;
}
