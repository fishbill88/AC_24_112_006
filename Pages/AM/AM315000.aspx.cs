using System;
using System.Web.UI.WebControls;
using PX.Objects.AM;

public partial class Page_AM315000 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		Style newStyle = new Style();
		newStyle.ForeColor = System.Drawing.Color.Red;
		newStyle.Font.Bold = true;
		this.Page.Header.StyleSheet.CreateStyleRule(newStyle, this, ".CssTimeError");
	}

	protected void Grid_RowDataBound(object sender, PX.Web.UI.PXGridRowEventArgs e)
	{
		var row = e.Row.DataItem as AMClockTran;
		if (row == null || ClockApprovalProcess.IsClockTranValid(row))
		{
			return;
		}

		e.Row.Style.CssClass = "CssTimeError";
	}
}
