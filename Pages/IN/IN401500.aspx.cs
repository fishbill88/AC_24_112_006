using System;

public partial class Page_IN401500 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected void MatrixAttributes_AfterSyncState(object sender, EventArgs e)
	{
		InventoryMatrixEntry.EnableCommitChangesAndMoveExtraColumnAtTheEnd(MatrixAttributes.Columns, 0);
	}

	protected void MatrixMatrix_AfterSyncState(object sender, EventArgs e)
	{
		InventoryMatrixEntry.EnableCommitChangesAndMoveExtraColumnAtTheEnd(MatrixMatrix.Columns, null, "ViewAllocationDetails");
	}
}
