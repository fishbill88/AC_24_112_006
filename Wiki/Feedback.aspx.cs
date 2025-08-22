using System;
using PX.Data;
using PX.Web.UI;

public partial class Pages_Feedback : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		this.Master.FindControl("usrCaption").Visible = false;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		this.Master.PopupWidth = 650;
		this.Master.PopupHeight = 350;
		
		PXLabel label = this.formview.FindControl("PXLabel") as PXLabel;
		if (label != null)
		{
			label.Text = PXSiteMap.IsPortal ? "Any comments and suggestions you would like to submit:" : "Other user comments and suggestions:";
		}
	}
}