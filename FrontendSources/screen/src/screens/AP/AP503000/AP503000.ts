import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, PXPageLoadBehavior } from 'client-controls';
import { PayBillsFilter, APAdjust, APAdjust2 } from './views';

@graphInfo({ graphType: 'PX.Objects.AP.APPayBills', primaryView: 'Filter', hideFilesIndicator: true, hideNotesIndicator: true, pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues })
export class AP503000 extends PXScreen {

	ViewInvoice: PXActionState;
	EditAmountPaid: PXActionState;
	ViewOriginalDocument: PXActionState;

	Filter = createSingle(PayBillsFilter);

	// DAC is duplicated here to allow different grids to show different columns
	APDocumentList = createCollection(APAdjust);
	APExceptionsList = createCollection(APAdjust2);

}
