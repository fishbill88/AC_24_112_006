using System;
using PX.Api;
using PX.Web.UI;

public partial class Page_SM206025 : PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.Page.IsCallback)
		{
			PXGrid grid = this.tab.FindControl("gridMapping") as PXGrid;
			if (grid != null)
				this.Page.ClientScript.RegisterClientScriptBlock(GetType(), "gridID", "var gridID=\"" + grid.ClientID + "\";", true);
		}
	}

    protected void edValue_InternalFieldsNeeded(object sender, PXCallBackEventArgs e)
    {
		var graph = (SYImportMaint)ds.DataGraph;
		e.Result = string.Join(";", graph.GetInternalFieldsNeeded());
    }

    protected void edValue_ExternalFieldsNeeded(object sender, PXCallBackEventArgs e)
	{
		var graph = (SYImportMaint)ds.DataGraph;
		e.Result = string.Join(";", graph.GetExternalFieldsNeeded());
	}

    protected void form_DataBound(object sender, EventArgs e)
    {
        var graph = this.ds.DataGraph as SYImportMaint;
        if (graph.IsSiteMapAltered)
            this.ds.CallbackResultArg = "RefreshSitemap";
    }

    protected void edValue_SubstitutionKeysNeeded(object sender, PXCallBackEventArgs e)
    {
		var graph = (SYImportMaint)ds.DataGraph;
		e.Result = string.Join(";", graph.GetSubstitutionKeysNeeded());
    }
}
