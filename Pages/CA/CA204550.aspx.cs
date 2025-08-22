using System;

public partial class Page_CA204550 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		this.Master.FindControl("usrCaption").Visible = false;
		this.Master.PopupHeight = 430;
		this.Master.PopupWidth = 760;
	}
}
