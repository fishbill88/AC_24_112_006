import { CR503021 } from "../CR503021";
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

export interface CR503021_panel_UpdateParams extends CR503021 {}
export class CR503021_panel_UpdateParams {
	wizardNext: PXActionState;

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
