using System;
using PX.BusinessProcess.UI;
using PX.Web.UI;

public partial class Page_SM302050 : PX.Web.UI.PXPage
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
	
	protected void edValue_RootFieldsNeeded(object sender, PXCallBackEventArgs e)
	{
		BusinessProcessEventMaint graph = this.ds.DataGraph as BusinessProcessEventMaint;
		if (graph != null)
		{
			e.Result = string.Join(";", graph.GetCurrentEventTriggerConditionsTablesWithFields());
		}
	}
}