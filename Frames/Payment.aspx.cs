using System;
using PX.Web.UI;

public partial class Page_Show : PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		ShowRouter.Instance.TryRedirect(Request, Context);
	}
}
