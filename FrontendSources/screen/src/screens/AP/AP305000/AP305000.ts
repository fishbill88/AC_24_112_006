import { CABatch, CABatchDetail, APPayment, AddPaymentsFilter, VoidFilter } from './views';
import { PXActionState, graphInfo, PXScreen, createSingle, createCollection } from "client-controls";

@graphInfo({ graphType: 'PX.Objects.CA.CABatchEntry', primaryView: 'Document', bpEventsIndicator: false, showUDFIndicator: true })
export class AP305000 extends PXScreen {

	ViewAPDocument: PXActionState;

	Document = createSingle(CABatch);
	BatchPayments = createCollection(CABatchDetail);
	AddendaInfo = createCollection(APPayment);
	filter = createSingle(AddPaymentsFilter);
	AvailablePayments = createCollection(APPayment);
	voidFilter = createSingle(VoidFilter);

}
