import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	Document,
	CurrentDocument,
	Address,
	StatutoryHolidays,
	OtherMonies,
	InsurableEarnings,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PR.PRRecordOfEmploymentMaint',
	primaryView: 'Document'
})
export class PR303000 extends PXScreen {
	AddressLookup: PXActionState;

	Document = createSingle(Document);
	CurrentDocument = createSingle(CurrentDocument);
	Address = createSingle(Address);
	StatutoryHolidays = createCollection(StatutoryHolidays);
	OtherMonies = createCollection(OtherMonies);
	InsurableEarnings = createCollection(InsurableEarnings);
}

