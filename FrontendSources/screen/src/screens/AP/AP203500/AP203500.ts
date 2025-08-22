import { createCollection, createSingle, graphInfo, PXActionState, PXScreen } from 'client-controls';
import { Schedule, APRegister } from './views';

@graphInfo({ graphType: 'PX.Objects.AP.APScheduleMaint', primaryView: 'Schedule_Header', showUDFIndicator: true })
export class AP203500 extends PXScreen {

	ViewDocument: PXActionState;
	ViewGenDocument: PXActionState;

	Schedule_Header = createSingle(Schedule);
	Document_Detail = createCollection(APRegister);
	Document_History = createCollection(APRegister);

}
