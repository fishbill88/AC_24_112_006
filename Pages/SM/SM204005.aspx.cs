using System;
using System.Linq;
using PX.Api;
using PX.Metadata;
using PX.SM;
using PX.Web.UI;

public partial class Pages_SM_SM204005 : PXPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
		if (!this.Page.IsCallback)
		{
			if (this.tab.FindControl("gridSettings") is PXGrid grid)
				this.Page.ClientScript.RegisterClientScriptBlock(GetType(), "gridID", "var gridID=\"" + grid.ClientID + "\";", true);
		}
    }
	protected void edBody_BeforePreview(object src, PXRichTextEdit.BeforePreviewArgs args)
	{
		var screenID = (DefaultDataSource.DataGraph as TaskTemplateMaint).CurrentScreenID;
		if (screenID != null)
		{
			var info = ScreenUtils.ScreenInfo.TryGet(screenID);
			if (info != null)
			{
				args.GraphName = info.GraphName;
				args.ViewName = info.PrimaryView;
			}
		}
	}
	protected void edBody_BeforeFieldPreview(object src, PXRichTextEdit.BeforeFieldPreviewArgs args)
	{
		if (args.Type == typeof(Users) && args.FieldName == "UserList.Password")
			args.Value = "*******";
	}
	protected void edValue_InternalFieldsNeeded(object sender, PXCallBackEventArgs e)
    {
		if (((TaskTemplateMaint)ds.DataGraph).TryGetInternalFields(out var fields))
		{
			e.Result = string.Join(";", fields);
		}
    }
    protected void edValue_ExternalFieldsNeeded(object sender, PXCallBackEventArgs e)
	{
		e.Result = null;
	}
}
