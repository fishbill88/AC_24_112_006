using System;
using PX.Web.UI;
using PX.Api;

public partial class Page_SM207036 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		PXGrid grid = this.tab.FindControl("gridPreparedData") as PXGrid;
		if (grid != null)
		{
			grid.RepaintColumns = true;
			grid.GenerateColumnsBeforeRepaint = true;
		}
	}

	protected void upl_FileUploaded(object sender, PXFileUploadEventArgs e)
	{
		((SYExportProcessSingle)this.ds.DataGraph).SaveNewFileVersion(e.UploadedFile);
	}
}
