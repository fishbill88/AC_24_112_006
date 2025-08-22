using System;
using PX.Web.UI;
using PX.Objects.IN.Matrix.DAC.Unbound;

public partial class Page_PO301000 : PX.Web.UI.PXPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
    }

	protected void MatrixAttributes_AfterSyncState(object sender, EventArgs e)
	{
		InventoryMatrixEntry.EnableCommitChangesAndMoveExtraColumnAtTheEnd(MatrixAttributes.Columns, 0);
	}

	protected void MatrixMatrix_AfterSyncState(object sender, EventArgs e)
	{
		InventoryMatrixEntry.EnableCommitChangesAndMoveExtraColumnAtTheEnd(MatrixMatrix.Columns);
	}

	protected void MatrixItems_AfterSyncState(object sender, EventArgs e)
	{
		InventoryMatrixEntry.InsertAttributeColumnsByTemplateColumn(
			MatrixItems.Columns, ds.DataGraph.Caches[typeof(MatrixInventoryItem)].Fields);
	}

	protected void MatrixItems_OnInit(object sender, EventArgs e)
	{
		InventoryMatrixEntry.InsertAttributeColumnsByTemplateColumn(
			MatrixItems.Columns, ds.DataGraph.Caches[typeof(MatrixInventoryItem)].Fields);
	}

	protected void MatrixMatrix_RowDataBound(object sender, PXGridRowEventArgs e)
	{
		InventoryMatrixEntry.SetUOMFormat(e.Row, MatrixMatrix.Columns);
	}
}
