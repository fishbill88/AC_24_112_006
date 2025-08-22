import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState, TextAlign } from "client-controls";

@graphInfo({ graphType: "PX.Objects.FA.BonusMaint", primaryView: "Bonuses", })
export class FA208000 extends PXScreen {

	@viewInfo({ containerName: "Bonus Summary" })
	Bonuses = createSingle(FABonus);

	@viewInfo({ containerName: "Details" })
	Details = createCollection(FABonusDetails);
}

export class FABonus extends PXView {

	BonusCD: PXFieldState;
	Description: PXFieldState;

}

export class FABonusDetails extends PXView {

	StartDate: PXFieldState;
	EndDate: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	BonusPercent: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	BonusMax: PXFieldState;

}
