import { createCollection,createSingle,PXScreen,graphInfo } from 'client-controls';
import { PreferencesGeneral,UploadAllowedFileTypes } from './views';

@graphInfo({graphType: 'PX.SM.UploadAllowedFileTypesMaint', primaryView: 'Prefs', })
export class SM202550 extends PXScreen {
	Prefs = createSingle(PreferencesGeneral);
	PrefsDetail = createCollection(UploadAllowedFileTypes);
}