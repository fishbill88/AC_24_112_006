using PX.SM;
using System;
using CRHelpers;
using PX.Data;
using PX.Metadata;
using PX.Web.UI;

public partial class Pages_SM_SM204003 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		this.ds.SubscribeRedirectsToScreens(typeof(Notification.screenID));
	}

	protected void edBody_BeforePreview(object src, PX.Web.UI.PXRichTextEdit.BeforePreviewArgs args)
	{
		var notification = (DefaultDataSource.DataGraph as SMNotificationMaint).Notifications.Current;
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

	protected void edScreenID_OnEditRecord(object sender, PXNavigateEventArgs e)
	{
		this.ds.ChangeRedirectWindowMode(e, PXBaseRedirectException.WindowMode.New);
	}
}
