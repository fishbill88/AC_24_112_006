import { createCollection, createSingle, PXScreen, graphInfo, viewInfo } from 'client-controls';
import { PRPayment, PRTaxFormBatch, PayrollDocumentsFilter } from './views';

@graphInfo({graphType: 'PX.Objects.PR.PRPayStubInq', primaryView: 'Filter' })
export class PR405000 extends PXScreen {
	@viewInfo({containerName: 'Pay Stubs'})
	PayChecks = createCollection(PRPayment);

	@viewInfo({containerName: 'Annual Forms'})
	TaxForms = createCollection(PRTaxFormBatch);

	@viewInfo({containerName: ''})
	Filter = createSingle(PayrollDocumentsFilter);
}
