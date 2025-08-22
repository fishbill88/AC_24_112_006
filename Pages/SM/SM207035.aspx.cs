using System;

public partial class Page_SM207035 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		if (!this.Page.IsCallback)
		{
			this.Page.ClientScript.RegisterClientScriptBlock(GetType(), "gridPreparedID", "var gridPreparedID=\"" + this.gridPreparedData.ClientID + "\";", true);
			this.Page.ClientScript.RegisterClientScriptBlock(GetType(), "pnlPreparedDataID", "var pnlPreparedDataID=\"" + this.pnlPreparedData.ClientID + "\";", true);
		}
		this.gridPreparedData.RepaintColumns = true;
	}
}
