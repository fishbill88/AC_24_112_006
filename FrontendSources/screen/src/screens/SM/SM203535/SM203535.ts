import { createCollection,createSingle,PXScreen,graphInfo,viewInfo,PXPageLoadBehavior } from 'client-controls';
import { SendTestMessageDialog,SmsPlugin,SmsPluginParameter } from './views';

@graphInfo({graphType: 'PX.SmsProvider.SM.SmsPluginMaint', primaryView: 'Plugin', pageLoadBehavior: PXPageLoadBehavior.GoLastRecord })
export class SM203535 extends PXScreen {


	@viewInfo({containerName: 'Send Test Message'})
	SendTestMessageDialogView = createSingle(SendTestMessageDialog);
	@viewInfo({containerName: 'Voice Plug-in Summary'})
	Plugin = createSingle(SmsPlugin);
	@viewInfo({containerName: 'Parameters'})
	Details = createCollection(SmsPluginParameter);

}