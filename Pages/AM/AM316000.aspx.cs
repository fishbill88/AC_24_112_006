using System;
using System.Web.UI.WebControls;
using PX.Objects.AM;
using PX.Objects.AM.Attributes;

public partial class  Page_AM316000 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		Style cellStyle = new Style();
		cellStyle.ForeColor = System.Drawing.Color.Green;
		cellStyle.Font.Bold = true;
		this.Page.Header.StyleSheet.CreateStyleRule(cellStyle, this, ".CssCellStyle");

        Style bgCellStyle = new Style();
        bgCellStyle.Font.Bold = true;
        this.Page.Header.StyleSheet.CreateStyleRule(bgCellStyle, this, ".CssBGCellStyle");
	}

	protected void Grid_RowDataBound(object sender, PX.Web.UI.PXGridRowEventArgs e)
	{
		var row = e.Row.DataItem as AMClockTran;
		Object value = e.Row.Cells["Status"].Value;
		if (value != null && ((string)value) == ClockTranStatus.ClockedIn)
		{
			//e.Row.Style.CssClass = "CssCellStyle";
			e.Row.Cells["TranDate"].Style.CssClass = "CssCellStyle";
			e.Row.Cells["StartTime_Time"].Style.CssClass = "CssCellStyle";
		}
	}

    protected void OperationsGrid_RowDataBound(object sender, PX.Web.UI.PXGridRowEventArgs e)
    {
        var row = e.Row.DataItem as ClockedInByOperation;
        Object value = e.Row.Cells["ClockedInByOperation__Active"].Value;
        if (value != null && ((bool)value) == true)
        {
            //e.Row.Style.CssClass = "CssCellStyle";
            e.Row.Cells["ClockedInByOperation__Active"].Style.CssClass = "CssBGCellStyle";
        }

    }
    
}
