using System;
using PX.Web.UI;

public partial class Page_AR402000 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		this.grid.FilterShortCuts = true;
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		PXToolBar toolBar = this.ds.ToolBar;
		toolBar.Items.RemoveAt(0);
	}
}
