using System;
using PX.Web.UI;

public partial class Page_CR306010 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		this.Master.PopupWidth = 800;
		this.Master.PopupHeight = 600;
	}
	protected void edParentNoteID_OnEditRecord(object sender, PXNavigateEventArgs e)
	{
		var selector = (PXSelector)sender;
		if(selector.Value == null)
			e.NavigateUrl = null;
	}
}
