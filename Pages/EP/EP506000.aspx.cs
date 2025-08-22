using System;
using PX.Web.UI;

public partial class Page_EP506000 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		this.Master.PopupHeight = 300;
		this.Master.PopupWidth = 700;
	}

	protected void grid_OnLoad(object sender, EventArgs e)
	{
		PXGrid grid = (PXGrid) sender;
		grid.ActionBar.Position = ActionsPosition.None;
	}
}
