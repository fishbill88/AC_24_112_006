import {
	PXScreen,

	createSingle,
	createCollection,

	viewInfo,
	handleEvent,

	CustomEventType,
	RowCssHandlerArgs,
} from "client-controls";

import { ScanInfo, ScanLogs } from "./views";

export abstract class BarcodeProcessingScreen extends PXScreen {
	@viewInfo({ containerName: "Scan Information" })
	Info = createSingle(ScanInfo);

	@viewInfo({ containerName: "Scan Logs" })
	Logs = createCollection(ScanLogs);

	@handleEvent(CustomEventType.GetRowCss, { view: 'Logs' })
	getLogsRowCss(args: RowCssHandlerArgs<ScanLogs>) {
		if (args?.selector?.row?.MessageType.value === 'ERR') {
			return 'excessedLine startedLine';
		}
		else if (args?.selector?.row?.MessageType.value === 'WRN') {
			return 'startedLine';
		}

		return undefined;
	}
}