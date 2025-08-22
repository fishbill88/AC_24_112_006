import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, PXActionState } from "client-controls";

@graphInfo({ graphType: "PX.Objects.CA.CABankFeedMaint", primaryView: "BankFeed", })
export class CA205500 extends PXScreen {

	SetDefaultMapping: PXActionState;
	LoadTransactions: PXActionState;

	BankFeed = createSingle(CABankFeed);
	CurrentBankFeed = createSingle(CABankFeed2);

	@viewInfo({ containerName: "Cash Accounts" })
	BankFeedDetail = createCollection(CABankFeedDetail);

	@viewInfo({ containerName: "Corporate Cards" })
	BankFeedCorpCC = createCollection(CABankFeedCorpCard);

	@viewInfo({ containerName: "Expense Items" })
	BankFeedExpense = createCollection(CABankFeedExpense);

	@viewInfo({ containerName: "Custom Mapping Rules" })
	BankFeedFieldMapping = createCollection(CABankFeedFieldMapping);

	@viewInfo({ containerName: "Categories" })
	BankFeedCategories = createSingle(BankFeedCategory);

	@viewInfo({ containerName: "Load Transactions in Test Mode" })
	Filter = createSingle(TransactionsFilter);

	@viewInfo({ containerName: "Load Transactions in Test Mode" })
	BankFeedTransactions = createCollection(BankFeedTransaction);

}

export class CABankFeed extends PXView {

	BankFeedID: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	ImportStartDate: PXFieldState;
	Descr: PXFieldState<PXFieldOptions.Multiline>;
	Institution: PXFieldState;
	CreateExpenseReceipt: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateReceiptForPendingTran: PXFieldState;

}

export class CABankFeed2 extends PXView {

	DefaultExpenseItemID: PXFieldState;
	ExternalUserID: PXFieldState;
	ExternalItemID: PXFieldState;

}

export class CABankFeedDetail extends PXView {

	StatementStartDay: PXFieldState;
	Hidden: PXFieldState;
	AccountName: PXFieldState;
	AccountMask: PXFieldState;
	Descr: PXFieldState;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	StatementPeriod: PXFieldState<PXFieldOptions.CommitChanges>;
	ImportStartDate: PXFieldState;
	Currency: PXFieldState;
	AccountType: PXFieldState;
	AccountSubType: PXFieldState;
	RetrievalStatus: PXFieldState;
	RetrievalDate: PXFieldState;
	ErrorMessage: PXFieldState;
	AccountID: PXFieldState;

}

@gridConfig({ syncPosition: true })
export class CABankFeedCorpCard extends PXView {

	CorpCardID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	MatchField: PXFieldState<PXFieldOptions.CommitChanges>;
	MatchRule: PXFieldState<PXFieldOptions.CommitChanges>;
	MatchValue: PXFieldState<PXFieldOptions.CommitChanges>;
	CardNumber: PXFieldState;
	CardName: PXFieldState;
	EmployeeID: PXFieldState;
	EmployeeName: PXFieldState;

}

export class CABankFeedExpense extends PXView {

	showCategories: PXActionState;
	MatchField: PXFieldState;
	MatchRule: PXFieldState<PXFieldOptions.CommitChanges>;
	MatchValue: PXFieldState;
	InventoryItemID: PXFieldState;
	DoNotCreate: PXFieldState;

}

@gridConfig({ initNewRow: true })
export class CABankFeedFieldMapping extends PXView {

	@columnConfig({ allowNull: false })
	Active: PXFieldState<PXFieldOptions.CommitChanges>;

	TargetField: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceFieldOrValue: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class BankFeedCategory extends PXView {

	Category: PXFieldState;

}

export class TransactionsFilter extends PXView {

	Date: PXFieldState;
	DateTo: PXFieldState;
	MaxTransactions: PXFieldState<PXFieldOptions.CommitChanges>;
	LineNbr: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class BankFeedTransaction extends PXView {

	TransactionID: PXFieldState;
	Date: PXFieldState;
	Amount: PXFieldState;
	IsoCurrencyCode: PXFieldState;
	Name: PXFieldState;
	Category: PXFieldState;
	Pending: PXFieldState;
	PendingTransactionID: PXFieldState;
	Type: PXFieldState;

	AccountID: PXFieldState;
	AccountOwner: PXFieldState;
	CheckNumber: PXFieldState;
	Memo: PXFieldState;
	CreatedAt: PXFieldState;
	PostedAt: PXFieldState;
	TransactedAt: PXFieldState;
	UpdatedAt: PXFieldState;
	AccountStringId: PXFieldState;
	CategoryGuid: PXFieldState;
	ExtendedTransactionType: PXFieldState;
	Id: PXFieldState;
	IsBillPay: PXFieldState;
	IsDirectDeposit: PXFieldState;
	IsExpense: PXFieldState;
	IsFee: PXFieldState;
	IsIncome: PXFieldState;
	IsInternational: PXFieldState;
	IsOverdraftFee: PXFieldState;
	IsPayrollAdvance: PXFieldState;
	IsRecurring: PXFieldState;
	IsSubscription: PXFieldState;
	Latitude: PXFieldState;
	LocalizedDescription: PXFieldState;
	LocalizedMemo: PXFieldState;
	Longitude: PXFieldState;
	MemberIsManagedByUser: PXFieldState;
	MerchantCategoryCode: PXFieldState;
	MerchantGuid: PXFieldState;
	MerchantLocationGuid: PXFieldState;
	Metadata: PXFieldState;
	OriginalDescription: PXFieldState;
	UserId: PXFieldState;
	AuthorizedDate: PXFieldState;
	AuthorizedDatetime: PXFieldState;
	DatetimeValue: PXFieldState;
	Address: PXFieldState;
	City: PXFieldState;
	Country: PXFieldState;
	PostalCode: PXFieldState;
	Region: PXFieldState;
	StoreNumber: PXFieldState;
	MerchantName: PXFieldState;
	PaymentChannel: PXFieldState;
	ByOrderOf: PXFieldState;
	Payee: PXFieldState;
	Payer: PXFieldState;
	PaymentMethod: PXFieldState;
	PaymentProcessor: PXFieldState;
	PpdId: PXFieldState;
	Reason: PXFieldState;
	ReferenceNumber: PXFieldState;
	PersonalFinanceCategory: PXFieldState;
	TransactionCode: PXFieldState;
	UnofficialCurrencyCode: PXFieldState;
	PartnerAccountID: PXFieldState;

}
