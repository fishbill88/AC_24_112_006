import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo
} from 'client-controls';

import {
	ChangeIDParam,
	INSite,
	INSiteAccounts,
	INLocation,
	INCart,
	INTote,
	Contact,
	Address,
	INToteCart,
	AddressLookupFilter
} from './views';

@graphInfo({
	graphType: 'PX.Objects.IN.INSiteMaint', 
	primaryView: 'site', 
})
export class IN204000 extends PXScreen {
	AddressLookup: PXActionState;

	@viewInfo({containerName: 'Specify New ID'})
	ChangeIDDialog = createSingle(ChangeIDParam);

	@viewInfo({containerName: 'Warehouse Summary'})
	site = createSingle(INSite);

	@viewInfo({containerName: 'Accounts'})
	siteaccounts = createSingle(INSiteAccounts);

	@viewInfo({containerName: 'Location Table'})
	location = createCollection(INLocation);

	@viewInfo({containerName: 'Carts'})
	carts = createCollection(INCart);

	@viewInfo({containerName: 'Totes'})
	totes = createCollection(INTote);

	@viewInfo({containerName: 'Address'})
	Contact = createSingle(Contact);

	@viewInfo({containerName: 'Address'})
	Address = createSingle(Address);

	@viewInfo({containerName: 'Assigned Totes'})
	totesInCart = createCollection(INToteCart);

	@viewInfo({containerName: 'Address Lookup'})
	AddressLookupFilter = createSingle(AddressLookupFilter);
}