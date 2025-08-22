import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
	handleEvent,
	CustomEventType,
	RowSelectedHandlerArgs,
	PXViewCollection,
	PXPageLoadBehavior,
	PXView,
	PXFieldState,
	gridConfig,
	headerDescription,
	ICurrencyInfo,
	disabled,
	selectorSettings,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	GridColumnShowHideMode,
	GridColumnType,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.SM.WikiSetupMaint",
	primaryView: "Wikis",
})
export class SM202005 extends PXScreen {
	importToDITA: PXActionState;

	@viewInfo({ containerName: "Choose date" })
	ClearingFilter = createSingle(ClearDateFilter);
	@viewInfo({ containerName: "Wiki" })
	Wikis = createSingle(WikiDescriptorExt);
	@viewInfo({ containerName: "Wiki" })
	SiteMapTree = createSingle(SiteMap);
	@viewInfo({ containerName: "" })
	CurrentWiki = createSingle(WikiDescriptorExt2);
	@viewInfo({ containerName: "Access Rights" })
	EntityRoles = createCollection(Role);

	@viewInfo({ containerName: "Tags" })
	Tags = createCollection(WikiTag);

	@viewInfo({ containerName: "Locales" })
	ReadLangs = createCollection(WikiReadLanguage);

	@viewInfo({ containerName: "File Paths" })
	SitePaths = createCollection(WikiSitePath);
}

// Views

export class ClearDateFilter extends PXView {
	Till: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class WikiDescriptorExt extends PXView {
	Name: PXFieldState;
	WikiTitle: PXFieldState;
	CreatedByID: PXFieldState<PXFieldOptions.Disabled>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Disabled>;
	HoldEntry: PXFieldState;
	RequestApproval: PXFieldState<PXFieldOptions.CommitChanges>;
	ApprovalGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	@selectorSettings("pkID", "")
	ApprovalUserID: PXFieldState;
	IsActive: PXFieldState;
	Category: PXFieldState<PXFieldOptions.CommitChanges>;
	Position: PXFieldState;
	DefaultUrl: PXFieldState;
	SitemapParent: PXFieldState<PXFieldOptions.CommitChanges>;
	SitemapTitle: PXFieldState;
}

export class SiteMap extends PXView {}

export class WikiDescriptorExt2 extends PXView {
	CssID: PXFieldState<PXFieldOptions.CommitChanges>;
	CssPrintID: PXFieldState;
	SPWikiArticleType: PXFieldState<PXFieldOptions.CommitChanges>;
	RootPageID: PXFieldState;
	RootPrintPageID: PXFieldState;
	HeaderPageID: PXFieldState;
	FooterPageID: PXFieldState;
	SiteMapTagID: PXFieldState;
	PubVirtualPath: PXFieldState;
	WikiDescription: PXFieldState<PXFieldOptions.Multiline>;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class Role extends PXView {
	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
	})
	ScreenID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
	})
	CacheName: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
	})
	MemberName: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false }) RoleName: PXFieldState;
	@columnConfig({ allowUpdate: false }) Guest: PXFieldState;
	@columnConfig({ allowUpdate: false }) RoleDescr: PXFieldState;
	RoleRight: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class WikiTag extends PXView {
	processTag: PXActionState;

	Description: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false
})
export class WikiReadLanguage extends PXView {
	Selected: PXFieldState;
	Language: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class WikiSitePath extends PXView {
	PageName: PXFieldState;
	Path: PXFieldState;
}
