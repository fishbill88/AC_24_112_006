using System;
using System.Web;
using PX.SM;
using PX.Web.Customization;
using PX.Web.UI;

public partial class Pages_AU_AU201050 : PX.Web.UI.PXPage
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        ProjectBrowserMaint.InitSessionFromQueryString(HttpContext.Current);
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        this.Master.FindControl("usrCaption").Visible = false;
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

    protected void edValue_RootFieldsNeeded(object sender, PXCallBackEventArgs e)
    {
        AUWorkflowActionsMaint graph = this.ds.DataGraph as AUWorkflowActionsMaint;
        if (graph != null)
        {
            var fields = graph.GetFieldsAndParams(true);
            e.Result = string.Join(";", fields);
        }
    }

    protected void edFormFieldValue_Params(object sender, PXCallBackEventArgs e)
    {
        AUWorkflowActionsMaint graph = this.ds.DataGraph as AUWorkflowActionsMaint;
        if (graph != null)
        {
            var fields = graph.GetFieldsAndParams(false);
            e.Result = string.Join(";", fields);
        }
    }

    protected void tabActionsDetails_OnPreRender(object sender, EventArgs e)
    {
        AUWorkflowActionsMaint graph = this.ds.DataGraph as AUWorkflowActionsMaint;
        var current = graph.FilterActionEdit.Current;
        if (current != null)
            (sender as PXTab).Items[0].Text = current.DisplayName;
    }

    protected void tabActionsDetails_OnSelectedTabChanged(object sender, PXTabChangedEventArgs e)
    {
        AUWorkflowActionsMaint graph = this.ds.DataGraph as AUWorkflowActionsMaint;
        var current = graph.SelectedActionSequence.Current;
        if (current != null)
            current.SelectedTabIndex = (sender as PXTab).SelectedItem.Index;
    }
}
