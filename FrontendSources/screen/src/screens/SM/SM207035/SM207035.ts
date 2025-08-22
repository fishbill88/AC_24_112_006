import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo } from "client-controls";
import { SYHistory, SYData, SYImportOperation, SYMapping } from "./views";

@graphInfo({ graphType: "PX.Api.SYExportProcess", primaryView: "Operation", })
export class SM207035 extends PXScreen {
	viewHistory: PXActionState;
	viewPreparedData: PXActionState;
	Save: PXActionState;
	viewReplacement: PXActionState;
	replaceOneValue: PXActionState;
	replaceAllValues: PXActionState;
	addSubstitutions: PXActionState;
	savePreparedData: PXActionState;

	@viewInfo({ containerName: "Parameters" })
	Operation = createSingle(SYImportOperation);

	@viewInfo({ containerName: "Scenarios" })
	Mappings = createCollection(SYMapping);

	@viewInfo({ containerName: "History" })
	History = createCollection(SYHistory);

	@viewInfo({ containerName: "Prepared Data" })
	PreparedData = createCollection(SYData);

}