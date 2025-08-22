import { graphInfo, PXScreen, viewInfo, createSingle, createCollection} from "client-controls";

import { SynchronizationFilter,UploadFile } from './views';

@graphInfo({graphType: 'PX.SM.SynchronizationProcess', primaryView: 'filter'})
export class SM202530 extends PXScreen {


	@viewInfo({containerName: 'Operation'})
	filter = createSingle(SynchronizationFilter);
	@viewInfo({containerName: 'Available Files'})
	SelectedFiles = createCollection(UploadFile);

}