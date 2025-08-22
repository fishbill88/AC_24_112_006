using System;
using PX.Objects.EP;

public partial class Page_EP305000 : PX.Web.UI.PXPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
		this.Master.PopupHeight = 700;
		this.Master.PopupWidth = 900;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		TimeCardMaint graph = this.ds.DataGraph as TimeCardMaint;
		if (graph != null)
			graph.CheckAllowedUser();
	}

}
