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
using PX.Objects.AR;

namespace PX.Objects.IN
{

	public class INPriceClassMaint : PXGraph<INPriceClassMaint>
	{
		public PXSelect<INPriceClass> Records;
        public PXSavePerRow<INPriceClass> Save;
		public PXCancel<INPriceClass> Cancel;

		protected virtual void INPriceClass_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			INPriceClass row = e.Row as INPriceClass;
			if (row != null)
			{
				PXSelectorAttribute.CheckAndRaiseForeignKeyException(sender, e.Row, typeof (DiscountInventoryPriceClass.inventoryPriceClassID));
				PXSelectorAttribute.CheckAndRaiseForeignKeyException(sender, e.Row, typeof(InventoryItem.priceClassID));
			}
		}
	}
}
