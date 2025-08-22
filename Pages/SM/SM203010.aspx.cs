using System;
using System.Web.Compilation;
using PX.Data;
using PX.SM;
using PX.Web.UI;
using System.Web.UI.WebControls;

public partial class Page_SM201020 : PX.Web.UI.PXPage
{
	private const string _CALENDAR_SYNC_HANDLER_TYPE = "PX.Objects.EP.PXCalendarSyncHandler";
	private static readonly System.Reflection.MethodInfo _getSyncUrlMethod;

	static Page_SM201020()
	{
		Type syncHandlerType = PXBuildManager.GetType(_CALENDAR_SYNC_HANDLER_TYPE, false);
		if (syncHandlerType != null)
		{
			_getSyncUrlMethod = syncHandlerType.GetMethod("GetSyncUrl", new Type[] {typeof (System.Web.HttpContext), typeof (string)});
		}
	}

	protected void Page_Init(object sender, EventArgs e)
	{
		if (tab != null && _getSyncUrlMethod == null)
		{
			PXTabItem item = tab.Items["calendar"];
			if (item != null) item.Visible = false;
		}

		ShowHideOutlookManifest();
	}

	protected void tab_Init(object sender, EventArgs e)
	{
		bool existMyProfileMaint = System.String.Compare(ds.TypeName, "PX.SM.MyProfileMaint", StringComparison.OrdinalIgnoreCase) == 0;
		tab.Items.Remove(tab.Items[existMyProfileMaint ? "searchSettingsSimple" : "searchSettings"]);
		if (existMyProfileMaint == false)
			tab.Items.Remove(tab.Items["printingSettings"]);
	}

	protected void tab_DataBound(object sender, EventArgs e)
	{
		if (PX.Translation.ResourceCollectingManager.IsStringCollecting)
			return;

		PXButton button = (PXButton)this.tab.FindControl("btnChangePassword");
		if (button != null)
		{
			PXSmartPanel panel = (PXSmartPanel)this.tab.FindControl("pnlChangePassword");
			PXTextEdit edit = (PXTextEdit)panel.FindControl("edNewPassword");
			if (edit != null)
			{
				button.Enabled = edit.Enabled;
				button.Hidden = edit.Hidden;
			}
		}

		int index;
		if (int.TryParse(Request["tab"], out index) && index < this.tab.Items.Count)
			this.tab.SelectedIndex = index;

		var outlookLinkControl = (HyperLink)tab.Items["EmailSettings"].TemplateContainer.FindControl("form2").FindControl("OutlookAddin");
		outlookLinkControl.Text = PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.Controls.Messages.OutlookAddin);
	}

	protected void cmdCheckMailSettings_CallBack(object sender, PXCallBackEventArgs e)
	{
		try
		{
			((SMAccessPersonalMaint)ds.DataGraph).getCalendarSyncUrl(
				new PXAdapter(ds.DataGraph.Views[ds.PrimaryView]));
		}
		catch (PXDialogRequiredException ex)
		{
			if (_getSyncUrlMethod != null)
			{
				ex.SetMessage((string)_getSyncUrlMethod.Invoke(null,
						new object[] { System.Web.HttpContext.Current , ex.Message }));
				ex.DataSourceID = ds.ID;
			}
			throw ex;
		}
	}

	private void ShowHideOutlookManifest()
	{
		if (tab.Items != null
			&& tab.Items["EmailSettings"] != null
			&& tab.Items["EmailSettings"].TemplateContainer != null
			&& tab.Items["EmailSettings"].TemplateContainer.FindControl("form2") != null)
		{
			var graph = this.ds.DataGraph;
			var cache = (graph != null && graph.Views.ContainsKey("CustomerModule") && graph.Views["CustomerModule"] != null) ? graph.Views["CustomerModule"].Cache : null;
			var feature = (cache != null) ? cache.Current : null;
			var value = (cache != null && feature != null) ? cache.GetValue(feature, "IsOutlookIntegrationInstalled") : null;
			var outlookFeature = value != null && (bool?)value == true;

			var outlookLinkControl = tab.FindControl("form2").FindControl("OutlookAddin");
			var btnHelpControl = tab.FindControl("form2").FindControl("btnHelp");
			if (outlookLinkControl != null)
				outlookLinkControl.Visible = outlookFeature;
			if (btnHelpControl != null)
				btnHelpControl.Visible = outlookFeature;
		}
	}
}
