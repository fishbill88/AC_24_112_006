using System;
using PX.Web.UI;
using PX.Data;
using PX.Objects.CT;

public partial class Page_CT202000 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected void Page_Init(object sender, EventArgs e)
	{
		if (PXPageCache.IsReloadPage)
		{
			ds.DataGraph.Caches<ContractDetail>().ClearQueryCache();
		}
	}
}
