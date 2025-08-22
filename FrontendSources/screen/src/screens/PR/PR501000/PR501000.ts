import { createCollection, createSingle, PXScreen, graphInfo, viewInfo } from 'client-controls';
import { PRDocumentProcessFilter, PRPayment } from './views';

@graphInfo({
	graphType: 'PX.Objects.PR.PRDocumentProcess',
	primaryView: 'Filter'
})
export class PR501000 extends PXScreen {
	@viewInfo({containerName: ''})
	Filter = createSingle(PRDocumentProcessFilter);

	@viewInfo({containerName: ''})
	DocumentList = createCollection(PRPayment);
}
