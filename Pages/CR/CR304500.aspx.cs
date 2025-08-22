using System;
using PX.Objects.CR;
using PX.Objects.CR.QuoteMaint_Extensions;
using PX.Objects.IN.Matrix.DAC.Unbound;
using PX.Web.UI;

public partial class Page_CR304500 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		this.Master.PopupHeight = 700;
		this.Master.PopupWidth = 1100;
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

	protected void edContactID_EditRecord(object sender, PX.Web.UI.PXNavigateEventArgs e)
	{
		QuoteMaint quoteMaint = this.ds.DataGraph as QuoteMaint;
		if (quoteMaint != null)
		{
			var ext = quoteMaint.GetExtension<QuoteMaint_CRCreateContactAction>();

			CRQuote currentQuote = this.ds.DataGraph.Views[this.ds.DataGraph.PrimaryView].Cache.Current as CRQuote;
			if (currentQuote.ContactID == null && currentQuote.BAccountID != null)
			{
				try
				{
					ext.CreateContactRedirect.Press();
				}
				catch (PX.Data.PXRedirectRequiredException e1)
				{
					PX.Web.UI.PXBaseDataSource ds = this.ds as PX.Web.UI.PXBaseDataSource;
					PX.Web.UI.PXBaseDataSource.RedirectHelper helper = new PX.Web.UI.PXBaseDataSource.RedirectHelper(ds);
					helper.TryRedirect(e1);
				}
			}
		}
	}
}
