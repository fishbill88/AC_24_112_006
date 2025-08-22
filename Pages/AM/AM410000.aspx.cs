using System;
using PX.Web.UI;

public partial class Page_AM410000 : PX.Web.UI.PXPage
{
    const string HighlightTextStyle = "HighlightText";

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        this.Master.PopupWidth = 960;
        this.Master.PopupHeight = 600;
    }

    protected void AMBomMatl_RowDataBound(object sender, PX.Web.UI.PXGridRowEventArgs e)
    {
        HighlightDiff(e.Row);
    }

    private void HighlightDiff(PXGridRow row)
    {
        if (row.PrevVisibleRow != null)
        {
            for (var i = 0; i < row.Cells.Count; i++)
            {
                PXGridCell cell = row.Cells[i];
                PXGridCell prevCell = row.PrevVisibleRow.Cells[i];
                if (!cell.ValueText.Equals(prevCell.ValueText))
                {
                    cell.Style.CssClass = HighlightTextStyle;
                    prevCell.Style.CssClass = HighlightTextStyle;
                }
            }
        }
    }

    protected void AMBomStep_RowDataBound(object sender, PXGridRowEventArgs e)
    {
        HighlightDiff(e.Row);
    }

    protected void AMBomTool_RowDataBound(object sender, PXGridRowEventArgs e)
    {
        HighlightDiff(e.Row);
    }

    protected void AMBomOvhd_RowDataBound(object sender, PXGridRowEventArgs e)
    {
        HighlightDiff(e.Row);
    }
}
