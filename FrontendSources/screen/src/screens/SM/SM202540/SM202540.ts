import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior } from "client-controls";
import { BlobStorageConfig, BlobProviderSettings, BlobStorageMessage } from "./views";

@graphInfo({ graphType: "PX.SM.BlobStorageMaint", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues })
export class SM202540 extends PXScreen {

   	@viewInfo({containerName: "Options"})
	Filter = createSingle(BlobStorageConfig);
   	@viewInfo({containerName: "Provider Settings"})
	Settings = createCollection(BlobProviderSettings);
   	@viewInfo({containerName: "Messages"})
	BlobStorageMessages = createSingle(BlobStorageMessage);
}
