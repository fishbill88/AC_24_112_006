// TODO: missing RefreshSitemap functionality (https://jira.acumatica.com/browse/AC-290458)

import { graphInfo, PXScreen, PXView, PXFieldState, PXFieldOptions, createSingle, createCollection, gridConfig, columnConfig, viewInfo } from "client-controls";

@graphInfo({graphType: 'PX.OAuthClient.ResourceMaint', primaryView: 'Resources'})
export class SM301010 extends PXScreen {
    Resources = createSingle(OAuthResource);
    @viewInfo({containerName: 'Visible to:'})
    Roles = createCollection(Roles);
}

// Views
export class OAuthResource extends PXView {
    ApplicationID: PXFieldState<PXFieldOptions.CommitChanges>;
    @columnConfig({nullText: "NEW"})
    ResourceCD: PXFieldState<PXFieldOptions.CommitChanges>;
    ResourceName: PXFieldState<PXFieldOptions.CommitChanges>;
    ResourceUrl: PXFieldState;
    SitemapTitle: PXFieldState<PXFieldOptions.CommitChanges>;
    WorkspaceID: PXFieldState;
    SubcategoryID: PXFieldState;
    SitemapScreenID: PXFieldState;
}

@gridConfig({adjustPageSize: true, allowInsert: false, allowDelete: false})
export class Roles extends PXView {
    @columnConfig({allowUpdate: false})
    AccessRights: PXFieldState<PXFieldOptions.CommitChanges>;
    @columnConfig({allowUpdate: false})
    Rolename: PXFieldState;
    @columnConfig({allowUpdate: false})
    Descr: PXFieldState;
}