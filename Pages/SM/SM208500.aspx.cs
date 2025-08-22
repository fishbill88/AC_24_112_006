using PX.Data;
using System;

public partial class Page_SM208500 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
	}

	protected void grid_OnDataBound(object sender, EventArgs e)
	{
		LEPMaint graph = this.ds.DataGraph as LEPMaint;
		if (graph.IsSiteMapAltered)
			this.ds.CallbackResultArg = "RefreshSitemap";
	}
}
