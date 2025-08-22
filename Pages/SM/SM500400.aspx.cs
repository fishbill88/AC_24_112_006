using PX.Web.UI;
using PX.Data.Archiving;

public partial class Page_SM500400 : PX.Web.UI.PXPage
{
	protected void grid_ColumnsGenerated(object sender, System.EventArgs e)
	{
		foreach (PXGridColumn column in ((PXGrid)sender).Columns)
			if (column.DataField != ArchiveProcess.AllToArchiveField && column.DataField.EndsWith(ArchiveProcess.ToArchiveSuffix))
				column.LinkCommand = column.DataField;
	}
}
