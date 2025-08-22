import { Messages as SysMessages } from "client-controls/services/messages";
import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
	handleEvent,
	CustomEventType,
	RowSelectedHandlerArgs,
	PXViewCollection,
} from "client-controls";
import { INKitSpecHdr, INKitSpecStkDet, INKitSpecNonStkDet } from "./views";

@graphInfo({
	graphType: "PX.Objects.IN.INKitSpecMaint",
	primaryView: "Hdr",
	showUDFIndicator: true,
})
export class IN209500 extends PXScreen {
	@viewInfo({ containerName: "Kit Specification Summary" })
	Hdr = createSingle(INKitSpecHdr);
	@viewInfo({ containerName: "Stock Components" })
	StockDet = createCollection(INKitSpecStkDet);

	@viewInfo({ containerName: "Non Stock Components" })
	NonStockDet = createCollection(INKitSpecNonStkDet);
}
