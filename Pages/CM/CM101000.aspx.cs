using System;
using PX.Data;
using PX.Web.UI;

public partial class Page_CM101000 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.IsCallback)
		{
			PXLabel lbl = this.formSettings.FindControl("lblPeriodsNumberAfter") as PXLabel;
			if (lbl != null)
				lbl.Text = PXMessages.LocalizeNoPrefix(PX.Objects.CM.Messages.Periods);
		}
	}
}
