import { createCollection, createSingle, PXScreen, graphInfo, viewInfo } from 'client-controls';
import { PRPayment, PayrollDocumentsFilter } from './views';

@graphInfo({graphType: 'PX.Objects.PR.PRPayStubInq', primaryView: 'PayChecks' })
export class PR405020 extends PXScreen {
	@viewInfo({containerName: 'Pay Stubs'})
	PayChecks = createCollection(PRPayment);

	@viewInfo({containerName: ''})
	Filter = createSingle(PayrollDocumentsFilter);
}
