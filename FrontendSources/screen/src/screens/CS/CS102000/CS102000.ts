import {
	createCollection, createSingle,
	PXScreen, PXActionState, graphInfo, viewInfo,
	CALLBACK_COMPLETED_EVENT, CallbackCompletedEvent,
} from 'client-controls';

import {
	ChangeIDParam, BAccount, BAccount2,
	Contact, Contact2, Address, Address2, Location,
	EPEmployee, Ledger
} from './views';

@graphInfo({ graphType: 'PX.Objects.CS.BranchMaint', primaryView: 'BAccount' })
export class CS102000 extends PXScreen {
	AddressLookup: PXActionState;
	ViewMainOnMap: PXActionState;
	SetDefaultLocation: PXActionState;
	DefLocationAddressLookup: PXActionState;
	ViewDefLocationAddressOnMap: PXActionState;
	AddressLookupSelectAction: PXActionState;
	NewLocation: PXActionState;
	ViewContact: PXActionState;
	ViewLedger: PXActionState;

	@viewInfo({ containerName: 'Specify New ID' })
	ChangeIDDialog = createSingle(ChangeIDParam);

	@viewInfo({ containerName: 'Branch Summary' })
	BAccount = createSingle(BAccount);

	CurrentBAccount = createSingle(BAccount2);

	@viewInfo({ containerName: 'Main Contact' })
	DefContact = createSingle(Contact);

	@viewInfo({ containerName: 'Main Address' })
	DefAddress = createSingle(Address);

	@viewInfo({ containerName: 'Branch Details' })
	DefLocation = createSingle(Location);

	@viewInfo({ containerName: 'Delivery Settings' })
	DefLocationContact = createSingle(Contact2);

	@viewInfo({ containerName: 'Delivery Settings' })
	DefLocationAddress = createSingle(Address2);

	@viewInfo({ containerName: 'Employees' })
	Employees = createCollection(EPEmployee);

	@viewInfo({ containerName: 'Ledgers' })
	LedgersView = createCollection(Ledger);

	afterConstructor() {
		super.afterConstructor();
		this.eventAggregator.subscribe(CALLBACK_COMPLETED_EVENT, (event: CallbackCompletedEvent) => {
			// TODO: change to refreshMenu() from control-helper!
			if (event.command === "Save" || event.command === "Delete") (<any>window.top).frameHelper.refreshMenu();
		});
	}
}
