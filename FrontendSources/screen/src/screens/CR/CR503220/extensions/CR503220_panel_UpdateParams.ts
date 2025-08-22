import { CR503220 } from "../CR503220";
import {
	PXView,
	PXFieldState,
	gridConfig,
	columnConfig,
	createCollection,
	createSingle,
	PXActionState,
	GridPagerMode,
} from "client-controls";

import { IWizardConfig } from "client-controls/controls/container/wizard/qp-wizard";

export interface CR503220_panel_UpdateParams extends CR503220 {}
export class CR503220_panel_UpdateParams {
	Fields = createCollection(FieldValue);
	Attributes = createCollection(Attributes);
	wizardSummary = createSingle(UpdateSummary);
	wizardConfig: IWizardConfig = {
		nextCommand: "wizardNext",
	};
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	autoAdjustColumns: true,
	pagerMode: GridPagerMode.InfiniteScroll,
	fastFilterByAllFields: false,
	showTopBar: false,
})
export class FieldValue extends PXView {
	@columnConfig({ allowCheckAll: true, width: 35 })
	Selected: PXFieldState;
	DisplayName: PXFieldState;
	Value: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	autoAdjustColumns: true,
	pagerMode: GridPagerMode.InfiniteScroll,
	fastFilterByAllFields: false,
	showTopBar: false,
})
export class Attributes extends PXView {
	@columnConfig({ allowCheckAll: true, width: 35 })
	Selected: PXFieldState;
	DisplayName: PXFieldState;
	Value: PXFieldState;
	Required: PXFieldState;
}

export class UpdateSummary extends PXView {
	Summary: PXFieldState;
}
