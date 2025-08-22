/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using PX.Data;
using PX.Objects.PO;

namespace PX.Objects.RQ.GraphExtensions.RQRequisitionEntryExt
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class RQRequisitionSiteStatusLookupExt : RQSiteStatusLookupBaseExt<RQRequisitionEntry, RQRequisition, RQRequisitionLine>
	{
		protected override RQRequisitionLine CreateNewLine(RQSiteStatusSelected line)
		{
			RQRequisitionLine newline = new RQRequisitionLine();
			newline.SiteID = line.SiteID;
			newline.InventoryID = line.InventoryID;
			newline.SubItemID = line.SubItemID;
			newline.UOM = line.PurchaseUnit;
			newline.OrderQty = line.QtySelected;
			return Base.Lines.Insert(newline);
		}

		protected virtual void _(Events.RowInserted<POSiteStatusFilter> e)
		{
			if (e.Row != null && Base.Document.Current != null)
			{
				e.Row.OnlyAvailable = Base.Document.Current.VendorID != null;
				e.Row.VendorID = Base.Document.Current.VendorID;
			}
		}

		protected override void _(Events.RowSelected<POSiteStatusFilter> e)
		{
			base._(e);

			if (e.Row != null)
				PXUIFieldAttribute.SetEnabled<POSiteStatusFilter.onlyAvailable>(ItemFilter.Cache, e.Row, Base.Document.Current.VendorID != null);
		}

		protected virtual void _(Events.RowSelected<RQRequisition> e)
		{
			if (e.Row == null)
				return;

			showItems.SetEnabled(e.Row.Hold == true && !(bool)e.Row.Released);
		}
	}
}
