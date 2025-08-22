using System;
using PX.Objects.PM;
using PX.Web.UI;

public partial class Page_PM307000 : PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		Master.PopupWidth = 1000;
		Master.PopupHeight = 700;

		var proformaEntry = ds.DataGraph as ProformaEntry;
		ds.PageLoadBehavior = proformaEntry?.IsMigrationMode() == true
			? PXPageLoadBehavior.InsertRecord
			: PXPageLoadBehavior.GoLastRecord;
	}
}
