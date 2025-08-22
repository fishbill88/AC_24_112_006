using System;
using PX.Web.UI;

public partial class Page_GL201000 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		PXCommandInfo fc = this.grid.ActionBar.Actions.FilesMenu;
		fc.Enabled = false;
	}
}
