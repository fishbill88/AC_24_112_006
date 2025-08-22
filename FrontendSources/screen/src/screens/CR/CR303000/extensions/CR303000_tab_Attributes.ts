import { CR303000 } from "../CR303000";
import {
	PXView,
	PXFieldState,
	createCollection,
	gridConfig,
	viewInfo,
	columnConfig,
	GridPreset,
} from "client-controls";

export interface CR303000_Attributes extends CR303000 {}
export class CR303000_Attributes {
	@viewInfo({ containerName: "Attributes" })
	Answers = createCollection(CSAnswers);
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	fastFilterByAllFields: false,
})
export class CSAnswers extends PXView {
	@columnConfig({ hideViewLink: true })
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
}
