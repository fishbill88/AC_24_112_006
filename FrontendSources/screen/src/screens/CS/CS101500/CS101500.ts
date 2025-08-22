import {
	createCollection, createSingle,
	PXScreen, PXActionState, CallbackCompletedEvent,
	graphInfo, viewInfo,
	CALLBACK_COMPLETED_EVENT, refreshMenu
} from 'client-controls';

import {
	BAccount, BAccount2, State, LedgerCreateParameters, ChangeIDParam, CommonSetup,
	Contact, Contact2, Address, Address2, Location,
	Organization, Company, Branch, EPEmployee, OrganizationLedgerLink,
	GroupOrganizationLink, AddressLookupFilter
} from './views';

@graphInfo({ graphType: 'PX.Objects.CS.OrganizationMaint', primaryView: 'BAccount', })
export class CS101500 extends PXScreen {
	AddressLookupSelectAction: PXActionState;
	DefLocationAddressLookup: PXActionState;
	ViewDefLocationAddressOnMap: PXActionState;
	SetDefaultLocation: PXActionState;
	DeleteOrganizationLedgerLink: PXActionState;
	ViewMainOnMap: PXActionState;
	NewLocation: PXActionState;
	AddLedger: PXActionState;
	ViewBranch: PXActionState;
	ViewContact: PXActionState;
	ViewLedger: PXActionState;

	@viewInfo({ containerName: 'Company Summary' })
	BAccount = createSingle(BAccount);

	@viewInfo({ containerName: 'Hidden Form needed for VisibleExp of TabItems' })
	StateView = createSingle(State);

	@viewInfo({ containerName: 'Main Address' })
	OrganizationView = createSingle(Organization);

	@viewInfo({ containerName: 'Main Contact' })
	DefContact = createSingle(Contact);

	@viewInfo({ containerName: 'Create Ledger' })
	CreateLedgerView = createSingle(LedgerCreateParameters);

	@viewInfo({ containerName: 'Specify New ID' })
	ChangeIDDialog = createSingle(ChangeIDParam);

	CurrentBAccount = createSingle(BAccount2);

	@viewInfo({ containerName: 'Main Address' })
	DefAddress = createSingle(Address);

	@viewInfo({ containerName: 'Company Details' })
	DefLocation = createSingle(Location);

	@viewInfo({ containerName: 'Company Details' })
	commonsetup = createSingle(CommonSetup);

	@viewInfo({ containerName: 'Company Details' })
	Company = createSingle(Company);

	@viewInfo({ containerName: 'Branches' })
	BranchesView = createCollection(Branch);

	@viewInfo({ containerName: 'Delivery Settings' })
	DefLocationContact = createSingle(Contact2);

	@viewInfo({ containerName: 'Delivery Settings' })
	DefLocationAddress = createSingle(Address2);

	@viewInfo({ containerName: 'Employees' })
	Employees = createCollection(EPEmployee);

	@viewInfo({ containerName: 'Ledgers' })
	OrganizationLedgerLinkWithLedgerSelect = createCollection(OrganizationLedgerLink);

    @viewInfo({ containerName: 'Company Groups' })
	Groups = createCollection(GroupOrganizationLink);

	@viewInfo({ containerName: 'Address Lookup' })
	AddressLookupFilter = createSingle(AddressLookupFilter);

	afterConstructor() {
		super.afterConstructor();
		this.eventAggregator.subscribe(CALLBACK_COMPLETED_EVENT, (event: CallbackCompletedEvent) => {
			if (event.command === "Save" || event.command === "Delete") refreshMenu();
		});
	}
}
