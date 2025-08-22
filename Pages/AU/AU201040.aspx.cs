using System;
using System.Web;
using PX.SM;
using PX.Web.Customization;
using PX.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_AU_AU201040 : PX.Web.UI.PXPage
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        ProjectBrowserMaint.InitSessionFromQueryString(HttpContext.Current);
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        this.Master.FindControl("usrCaption").Visible = false;
    }

    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        if (PXDynamicFormGenerator.CreateChildControls(ds.DataGraph, this.FormPreview))
		{
			if (FormPreview.AutoSize.Enabled)
			{
				PanelDynamicForm.AutoSize.MinWidth = FormPreview.AutoSize.MinWidth;
				PanelDynamicForm.AutoSize.MinHeight = FormPreview.AutoSize.MinHeight +
					Convert.ToInt32(PXPanel1.Height.Value) +
					20 + // Dialog content padding
					40; // Dialog caption height
				PanelDynamicForm.Width = new Unit(PanelDynamicForm.AutoSize.MinWidth);
				PanelDynamicForm.Height = new Unit(PanelDynamicForm.AutoSize.MinHeight);
			}
		}
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
        AUWorkflowFormsMaint graph = this.ds.DataGraph as AUWorkflowFormsMaint;
        if (graph != null)
        {
            var fields = graph.GetFields();
            e.Result = string.Join(";", fields);
        }
    }
}
