using System;
using PX.SM;
using PX.Web.UI;

public partial class Page_SM203000 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
	}

	protected void Page_Load(object sender, EventArgs e)
	{
        (sp1.FindControl("tree") as PXTreeView).DataBind();
	}

	protected void form_DataBound(object sender, EventArgs e)
	{
		Access graph = ds.DataGraph as Access;

		if (graph.IsSiteMapAltered)
		{
			ds.CallbackResultArg = "RefreshSitemap";
		}
	}
}
