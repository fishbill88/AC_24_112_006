using System;
using PX.Web.UI;

public partial class Pages_SM_SM204007 : PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{

	}
	
	 protected void grid_EditorsCreated(object sender, EventArgs e)
    {
        var grid = sender as PXGrid;
        if (grid != null)
        {
            var de = grid.PrimaryLevel.GetStandardEditor(GridStandardEditor.Date) as PXDateTimeEdit;
            if (de != null)
            {
                de.ShowRelativeDates = true;
            }
        }
    }
}