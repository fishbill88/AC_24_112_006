import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig,
	GridPagerMode,
} from "client-controls";

// Views
export class AccountsFilter extends PXView {
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountName: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountClass: PXFieldState<PXFieldOptions.CommitChanges>;
	LinkContactToAccount: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ContactFilter extends PXView {
	FirstName: PXFieldState<PXFieldOptions.CommitChanges>;
	LastName: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState<PXFieldOptions.CommitChanges>;
	Salutation: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2: PXFieldState<PXFieldOptions.CommitChanges>;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactClass: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	allowUpdate: false,
	fastFilterByAllFields: false,
	pagerMode: GridPagerMode.InfiniteScroll,
	autoAdjustColumns: true,
	showTopBar: false,
})
export class PopupAttributes extends PXView {
	DisplayName: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	fastFilterByAllFields: false,
	pagerMode: GridPagerMode.InfiniteScroll,
	autoAdjustColumns: true,
	showTopBar: false,
})
export class PopupUDFAttributes extends PXView {
	DisplayName: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}
