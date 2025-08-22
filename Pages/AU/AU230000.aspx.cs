using System;
using PX.Web.UI;

public partial class Page_AU230000 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		this.Master.FindControl("usrCaption").Visible = false;
        PXGrid1.FeedbackMode = GridFeedbackModeOverride.DisableAll;
    }
}
