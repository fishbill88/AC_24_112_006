import { createCollection, PXScreen, graphInfo, viewInfo } from "client-controls";
import { EPShiftCode, EPShiftCodeRate } from "./views";

@graphInfo({
	graphType: 'PX.Objects.EP.EPShiftCodeSetup',
	primaryView: 'Codes'
})
export class EP103000 extends PXScreen {
	@viewInfo({ containerName: "Shift Codes" })
	Codes = createCollection(EPShiftCode);
	@viewInfo({ containerName: "Rates" })
	Rates = createCollection(EPShiftCodeRate);
}
