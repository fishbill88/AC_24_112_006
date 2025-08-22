import {
	createCollection,
	createSingle,
	graphInfo,
	handleEvent,
	CustomEventType,
	PXScreen,
	RowCssHandlerArgs
} from "client-controls";

import {
	Revisions,
	Filter,
	Project,
	Items,
	AddPeriodDialog,
	CopyDialog,
	DistributeDialog
} from "./views";

import {
	PMConstants
} from "../pm-constants";

@graphInfo({
	graphType: "PX.Objects.PM.ForecastMaint",
	primaryView: "Revisions"
})
export class PM209600 extends PXScreen {
	Revisions = createSingle(Revisions);
	Filter = createSingle(Filter);
	Project = createSingle(Project);
	Items = createCollection(Items);
	AddPeriodDialog = createSingle(AddPeriodDialog);
	CopyDialog = createSingle(CopyDialog);
	DistributeDialog = createSingle(DistributeDialog);

	@handleEvent(CustomEventType.GetRowCss, { view: Items.name })
	getItemsRowCss(args: RowCssHandlerArgs) {
		return args?.selector?.row?.FinPeriodID.value === PMConstants.TotalFinPeriod
			? PMConstants.BoldRowCssClass
			: undefined;
	}
}
