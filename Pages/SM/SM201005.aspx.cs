using System;
using PX.Data.Access.ActiveDirectory;

public partial class Page_SM202000 : PX.Web.UI.PXPage
{
	public void Page_Load(object sender, EventArgs e)
	{
		if (!((PX.SM.RoleAccess) ds.DataGraph).ActiveDirectoryProvider.IsEnabled())
			tab.Items["ad"].Visible = false;
	}
}
