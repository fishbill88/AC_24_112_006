import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Filter,
	Transactions
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.TransactionInquiry",
	primaryView: "Filter"
})
export class PM401000 extends PXScreen {
	ViewDocument: PXActionState;
	ViewInventory: PXActionState;
	ViewProforma: PXActionState;
	ViewInvoice: PXActionState;
	ViewCustomer: PXActionState;
	ViewOrigDocument: PXActionState;

	Filter = createSingle(Filter);
	Transactions = createCollection(Transactions);
}
