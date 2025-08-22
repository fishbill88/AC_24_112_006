using System;
using PX.SM;

[Customization.CstDesignMode(Disabled=true)]
public partial class Page_SM204510 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		this.Master.FindControl("usrCaption").Visible = false;
		//this.Master.CustomizationAvailable = false;
	}

	protected void gridVersions_RowDataBound(object sender, PX.Web.UI.PXGridRowEventArgs e)
	{
		//DummyProjRevision item = e.Row.DataItem as DummyProjRevision;
		//if (item == null) return;
		//if(item.ID == Guid.Empty)
		//    e.Row.Style.CssClass = "CssCurrent";
	}

	public void uploadPanel_Upload(PX.Web.UI.UserControls.PXUploadFilePanel.PXFileUploadedEventArgs args)
	{
		ProjectMaintenance.OnUploadPackage(args.FileName, args.BinData);

	}

}
