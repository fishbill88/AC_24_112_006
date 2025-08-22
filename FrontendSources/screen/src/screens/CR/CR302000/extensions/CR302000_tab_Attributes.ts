import { CR302000 } from "../CR302000";
import {
	PXView,
	PXFieldState,
	createCollection,
	gridConfig,
	viewInfo,
	columnConfig,
	GridPreset,
} from "client-controls";

export interface CR302000_Attributes extends CR302000 {}
export class CR302000_Attributes {
	@viewInfo({ containerName: "Attributes" })
	Answers = createCollection(CSAnswers);
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class CSAnswers extends PXView {
	@columnConfig({ hideViewLink: true })
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
}
