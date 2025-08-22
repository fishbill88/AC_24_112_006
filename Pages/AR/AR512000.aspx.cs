using System;

public partial class Page_AR512000 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		this.grid.FilterShortCuts = true;
	}
}
