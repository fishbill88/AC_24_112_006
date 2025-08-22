import { createCollection, createSingle, PXScreen, graphInfo } from "client-controls";
import { PRPTOAdjustment, PRPTOAdjustmentDetail } from "./views";

@graphInfo({graphType: "PX.Objects.PR.PRPTOAdjustmentMaint", primaryView: "Document" })
export class PR306000 extends PXScreen {
	Document = createSingle(PRPTOAdjustment);
	PTOAdjustmentDetails = createCollection(PRPTOAdjustmentDetail);
}
