using System;
using PX.Web.UI;
using Messages = PX.Objects.DR.Messages;

public partial class Page_DR202000 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.IsCallback)
		{
			PXGroupBox box = this.form.FindControl("gbPeriodically") as PXGroupBox;
			if (box != null)
			{
				PXLabel lbl = box.FindControl("PXLabel1") as PXLabel;
				if (lbl != null)
					lbl.Text = Messages.DocumentDateSelection;
			}
		}
	}
}
