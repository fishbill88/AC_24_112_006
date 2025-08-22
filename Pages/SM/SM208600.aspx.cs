using System;
using PX.Dashboards;
using PX.Web.UI;

public partial class Page_SM208600 : PXPage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

	protected void frmHeader_DataBound(object sender, EventArgs e)
	{
		DashboardMaint graph = (DashboardMaint) this.ds.DataGraph;
		if (graph.IsSiteMapAltered)
			this.ds.CallbackResultArg = "RefreshSitemap";
	}

	protected void grd_EditorsCreated_RelativeDates(object sender, EventArgs e)
	{
		PXGrid grid = sender as PXGrid;
		if (grid != null)
		{
			PXDateTimeEdit de = grid.PrimaryLevel.GetStandardEditor(GridStandardEditor.Date) as PXDateTimeEdit;
			if (de != null)
			{
				de.ShowRelativeDates = true;
			}
		}
	}
}