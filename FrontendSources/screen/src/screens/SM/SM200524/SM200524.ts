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
	graphType: "PX.Objects.SM.SPWikiProductMaint",
	primaryView: "WikiProduct",
})
export class SM200524 extends PXScreen {
	@viewInfo({ containerName: "Wiki Product" })
	WikiProduct = createSingle(SPWikiProduct);
	@viewInfo({ containerName: "" })
	WikiProductDetails = createCollection(SPWikiProductTags);
}

export class SPWikiProduct extends PXView {
	ProductID: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class SPWikiProductTags extends PXView {
	PageName: PXFieldState<PXFieldOptions.CommitChanges>;
	PageTitle: PXFieldState;
}
