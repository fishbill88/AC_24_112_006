import { CR304000 } from "../CR304000";
import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createCollection,
	gridConfig,
	viewInfo,
	PXActionState,
	columnConfig,
	GridPreset,
} from "client-controls";

export interface CR304000_Attributes extends CR304000 {}
export class CR304000_Attributes {
	@viewInfo({ containerName: "Attributes" })
	Answers = createCollection(CSAnswers);
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	fastFilterByAllFields: false,
})
export class CSAnswers extends PXView {
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
}
