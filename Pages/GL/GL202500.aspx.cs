using PX.Web.UI;
using System;
using PX.Objects.GL;

public partial class Page_GL202500 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
	}

    protected void messageLabel_DataBinding(object sender, EventArgs e)
    {
		AccountMaint graph = (AccountMaint)this.ds.DataGraph;
		((PXLabel)sender).Text = graph.AccountTypeChangePrepare.Current.Message;
	}
}
