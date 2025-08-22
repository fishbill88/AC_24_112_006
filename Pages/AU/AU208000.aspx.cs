using System;
using PX.Data;

public partial class Page_AU208000 : PX.Web.UI.PXPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        this.Master.FindControl("usrCaption").Visible = false;

        this.lblSiteMapSectionHeader.Text = PXSiteMap.IsPortal
                                                ? PXMessages.LocalizeNoPrefix(PX.Web.Customization.Messages.CustomizationEditorSiteMapSectionHeaderForPortalMap)
                                                : PXMessages.LocalizeNoPrefix(PX.Web.Customization.Messages.CustomizationEditorSiteMapSectionHeaderForSiteMap);

        this.FilterSelectSiteMap.Caption = PXSiteMap.IsPortal
                                            ? PXMessages.LocalizeNoPrefix(PX.Web.Customization.Messages.CustomizationEditorAddPortalMapDialogHeader)
                                            : PXMessages.LocalizeNoPrefix(PX.Web.Customization.Messages.CustomizationEditorAddSiteMapDialogHeader);
    }
}
