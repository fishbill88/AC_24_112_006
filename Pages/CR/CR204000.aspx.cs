using System;
using CRHelpers;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CR.CRMarketingListMaint_Extensions;
using PX.Web.UI;

public partial class Page_CR204000 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		this.ds.SubscribeRedirectsToInquiries(
			typeof(CRMarketingList.gIDesignID),
			typeof(AddMembersFromGIFilter.gIDesignID));
	}


	protected void anySelector_OnEditRecord(object sender, PXNavigateEventArgs e)
	{
		this.ds.ChangeRedirectWindowMode(e, PXBaseRedirectException.WindowMode.New);
	}
}
