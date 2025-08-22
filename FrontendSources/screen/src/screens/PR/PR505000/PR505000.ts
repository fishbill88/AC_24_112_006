import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXPageLoadBehavior } from 'client-controls';
import { PrintChecksFilter, PRPayment } from './views';

@graphInfo({graphType: 'PX.Objects.PR.PRPrintChecks', primaryView: 'Filter', pageLoadBehavior: PXPageLoadBehavior.GoLastRecord})
export class PR505000 extends PXScreen {
	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(PrintChecksFilter);

	@viewInfo({ containerName: 'Payments' })
	PaymentList = createCollection(PRPayment);
}
