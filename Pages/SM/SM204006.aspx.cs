using PX.SM;
using System;
using PX.Metadata;

public partial class Pages_SM_SM204006 : PX.Web.UI.PXPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
	
	protected void edBody_BeforePreview(object src, PX.Web.UI.PXRichTextEdit.BeforePreviewArgs args)
	{
		var notification = (DefaultDataSource.DataGraph as SMTeamsNotificationMaint).Notifications.Current;
		if (null != notification)
		{
			var info = PX.Api.ScreenUtils.ScreenInfo.TryGet(notification.ScreenID);
			if (info != null)
			{
				args.GraphName = info.GraphName;
				args.ViewName = info.PrimaryView;
			}
		}
	}
	
	protected void edBody_BeforeFieldPreview(object src, PX.Web.UI.PXRichTextEdit.BeforeFieldPreviewArgs args)
	{
		if (args.Type == typeof(PX.SM.Users) && args.FieldName == "UserList.Password")
			args.Value = "*******";
	}
}