import { Messages as SysMessages } from "client-controls/services/messages";
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior } from "client-controls";
import { ClearDateFilter, GetLinkFilterType, FilesFilter, UploadFile } from "./views";

@graphInfo({graphType: "PX.SM.UploadFileInq", primaryView: "Filter", })
export class SM202520 extends PXScreen {

	@viewInfo({containerName: "Choose date"})
	ClearingFilter = createSingle(ClearDateFilter);
	@viewInfo({containerName: "File Link"})
	GetFileLinkFilter = createSingle(GetLinkFilterType);
	@viewInfo({containerName: "Selection"})
	Filter = createSingle(FilesFilter);
	Files = createCollection(UploadFile);
}