import { createCollection, createSingle, PXScreen, graphInfo, viewInfo } from 'client-controls';
import {
	PushNotificationsHook,
	PushNotificationsSourceGI,
	PushNotificationsTrackingFieldGI,
	PushNotificationsSourceIC,
	PushNotificationsTrackingFieldIC
} from './views';

@graphInfo({graphType: 'PX.PushNotifications.UI.PushNotificationMaint', primaryView: 'Hooks' })
export class SM302000 extends PXScreen {

	Hooks = createSingle(PushNotificationsHook);
	SourcesGI = createCollection(PushNotificationsSourceGI);
	@viewInfo({containerName: 'Fields'})
	TrackingFieldsGI = createCollection(PushNotificationsTrackingFieldGI);
	@viewInfo({containerName: 'Definitions'})
	SourcesInCode = createCollection(PushNotificationsSourceIC);
	@viewInfo({containerName: 'Fields'})
	TrackingFieldsIC = createCollection(PushNotificationsTrackingFieldIC);
}