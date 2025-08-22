import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.SM.SPWikiCategoryMaint",
	primaryView: "WikiCategory",
})
export class SM200523 extends PXScreen {
	@viewInfo({ containerName: "Wiki Category" })
	WikiCategory = createSingle(SPWikiCategory);
	@viewInfo({ containerName: "" })
	WikiCategoryDetails = createCollection(SPWikiCategoryTags);
}

export class SPWikiCategory extends PXView {
	CategoryID: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class SPWikiCategoryTags extends PXView {
	PageName: PXFieldState<PXFieldOptions.CommitChanges>;
	PageTitle: PXFieldState;
}
