using System;
using System.Web.UI.WebControls;

using PX.Objects.PM;
using PX.Web.UI;

public partial class Page_PM301000 : PXPage
{
	private const string BoldClass = "BoldText";
	private const string BoldSelector = "." + BoldClass;

	protected void Page_Load(object sender, EventArgs e)
	{
		var style = new Style();
		style.Font.Bold = true;

		Page.Header.StyleSheet.CreateStyleRule(style, this, BoldSelector);

		Master.PopupWidth = 1000;
		Master.PopupHeight = 700;
	}

	protected void ProjectBalanceGrid_RowDataBound(object sender, PXGridRowEventArgs e)
	{
		var record = e.Row.DataItem as ProjectEntry.PMProjectBalanceRecord;
		if (record?.RecordID < 0)
		{
			e.Row.Style.CssClass = BoldClass;
		}
	}

	protected void BudgetGrid_RowDataBound(object sender, PXGridRowEventArgs e)
	{
		var budget = e.Row.DataItem as PMBudget;
		if (budget?.SortOrder == 1)
		{
			e.Row.Style.CssClass = BoldClass;
		}
	}
}
