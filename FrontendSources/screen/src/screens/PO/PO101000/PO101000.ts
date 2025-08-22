import
{
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo
} from 'client-controls';
import { POSetup, POSetupApproval, Notifications,
	Recipients, POReceivePutAwaySetup } from './views';

@graphInfo({graphType: 'PX.Objects.PO.POSetupMaint', primaryView: 'Setup' })
export class PO101000 extends PXScreen {
	@viewInfo({containerName: 'General Settings'})
	Setup = createSingle(POSetup);

	@viewInfo({containerName: 'Approval'})
	SetupApproval = createCollection(POSetupApproval);

	@viewInfo({containerName: 'Default Sources'})
	Notifications = createCollection(Notifications);

	@viewInfo({containerName: 'Default Recipients'})
	Recipients = createCollection(Recipients);

	@viewInfo({containerName: 'Warehouse Management'})
	ReceivePutAwaySetup = createSingle(POReceivePutAwaySetup);
}
