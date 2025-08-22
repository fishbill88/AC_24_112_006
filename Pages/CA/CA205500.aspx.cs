using PX.Data;
using PX.Objects.CA;
using PX.Objects.CA.Descriptor;
using PX.Web.UI;
using System;

public partial class Page_CA205500 : PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.Page.IsCallback)
		{
			PXGrid gridImport = this.tabs.FindControl("gridFieldMapping") as PXGrid;
			if (gridImport != null)
			{
				this.Page.ClientScript.RegisterClientScriptBlock(GetType(), "gridID", "var gridID=\"" + gridImport.ClientID + "\";", true);
			}
		}
	}

	protected void edImportSourceField_ExternalFieldsNeeded(object sender, PXCallBackEventArgs e)
	{
		e.Result = GetFieldList();
	}

	private string GetFieldList()
	{
		var graph = (CABankFeedMaint)this.ds.DataGraph;
		if (graph.BankFeed.Current == null) { return string.Empty; }

		var bankFeed = graph.GetCurrentPrimaryObject() as CABankFeed;

		CABankFeedMappingSourceHelper cABankFeedMappingSourceHelper = new CABankFeedMappingSourceHelper(graph.BankFeed.Cache);
		return cABankFeedMappingSourceHelper.GetFieldsForFormula(bankFeed.Type);
	}
}
