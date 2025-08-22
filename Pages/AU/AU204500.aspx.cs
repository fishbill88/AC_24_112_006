using System;
using PX.Data;

public partial class Page_AU204500 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
	    this.Master.FindControl("usrCaption").Visible = false;
		lblCaption.Text = PXMessages.LocalizeNoPrefix(PX.AscxControlsMessages.Messages.CustomFilesCaption);
	}
}
