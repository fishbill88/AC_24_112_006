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
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO.DAC.Unbound;
using System;

namespace PX.Objects.SO.Attributes
{
	[PX.Common.PXInternalUseOnly]
	public class InvoiceSplitProjectionAttribute : PXProjectionAttribute
	{
		public InvoiceSplitProjectionAttribute(bool expandByFilter)
			: base(GetSelect(expandByFilter))
		{
		}

		private static Type GetSelect(bool expandByFilter)
		{
			return BqlTemplate.OfCommand<
				SelectFrom<ARTran>.
				LeftJoin<SOLine>.On<ARTran.FK.SOOrderLine>.
				LeftJoin<INTran>.On<INTran.FK.ARTran.
					And<Where<BqlPlaceholder.E, Equal<True>,Or<ARTran.inventoryID.IsEqual<INTran.inventoryID>>>>.
					And<INTran.docType.IsNotEqual<INDocType.adjustment>>>.
				LeftJoin<INTranSplit>.On<INTranSplit.FK.Tran>.
				Where<
					ARTran.released.IsEqual<True>.
					And<ARTran.invtReleased.IsEqual<True>.
						Or<
							INTran.released.IsNull.
							And<ARTran.lineType.IsIn<SOLineType.miscCharge, SOLineType.nonInventory>>>>.
					And<
						ARTran.qty.IsEqual<decimal0>.
						Or<
							ARTran.qty.IsGreater<decimal0>.
							And<ARTran.tranType.IsIn<ARDocType.debitMemo, ARDocType.cashSale, ARDocType.invoice>>>.
						Or<
							ARTran.qty.IsLess<decimal0>.
							And<ARTran.tranType.IsIn<ARDocType.creditMemo, ARDocType.cashReturn>>>>>.
				OrderBy<
					ARTran.inventoryID.Asc,
					INTranSplit.subItemID.Asc>>
				.Replace<BqlPlaceholder.E>(expandByFilter ? typeof(CurrentValue<AddInvoiceFilter.expand>) : typeof(True))
				.ToType();
		}
	}
}
