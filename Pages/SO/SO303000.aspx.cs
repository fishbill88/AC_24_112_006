using System;
using PX.Web.UI;

public partial class Page_SO303000 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
	}

	protected void grid_RowDataBound(object sender, PXGridRowEventArgs e)
	{
		e.Row.Cells["RelatedItems"].Style.CssClass = "RelatedItemsCell";
	}
}
