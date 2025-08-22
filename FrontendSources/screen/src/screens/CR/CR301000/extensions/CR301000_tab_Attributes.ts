import { CR301000 } from "../CR301000";
import {
	PXView,
	PXFieldState,
	createCollection,
	gridConfig,
	viewInfo,
	columnConfig,
	GridPreset,
} from "client-controls";

export interface CR301000_Attributes extends CR301000 {}
export class CR301000_Attributes {
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
