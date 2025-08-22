using System;
using PX.Web.UI;
using PX.Data;

public partial class Page_SM200578 : PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected void grid_RowDataBound(object sender, PXGridRowEventArgs e)
	{
		if (e.Row != null && e.Row.DataItem != null)
		{
			PXStringState state = this.ds.DataGraph.Caches[e.Row.DataItem.GetType()].GetStateExt(e.Row.DataItem, "Value") as PXStringState;
			if (state != null && state.InputMask == "*")
				e.Row.Cells["Value"].IsPassword = true;
		}
	}
}
