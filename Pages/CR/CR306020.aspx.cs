using System;
using PX.Web.UI;
using PX.Web.Controls.TitleModules;

[ReminderDisable]
public partial class Page_CR306020 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		Master.PopupWidth = 800;
		Master.PopupHeight = 650;
	}
	protected void edParentNoteID_OnEditRecord(object sender, PXNavigateEventArgs e)
	{
		var selector = (PXSelector)sender;
		if (selector.Value == null)
			e.NavigateUrl = null;
	}
}
