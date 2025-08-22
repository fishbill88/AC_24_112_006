using System;
using PX.Data;
using PX.Web.UI;
using Messages = PX.Objects.EP.Messages;

public partial class Page_EP101000 : PX.Web.UI.PXPage
{
    protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.IsCallback)
		{
			PXLabel lbl = this.tab.FindControl("lblPeriods") as PXLabel;
			if (lbl != null)
				lbl.Text = PXMessages.LocalizeNoPrefix(Messages.Periods);
		}
    }
}
