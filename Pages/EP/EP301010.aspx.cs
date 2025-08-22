using System;
using PX.Objects.EP;

public partial class Page_EP301010 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		var graph = this.ds.DataGraph as ExpenseClaimDetailMaint;

		if (graph != null)
		{
			ExpenseClaimDetailEntry.ExpenseClaimDetailEntryExt.CheckAllowedUser(graph);
		}
	}
}
