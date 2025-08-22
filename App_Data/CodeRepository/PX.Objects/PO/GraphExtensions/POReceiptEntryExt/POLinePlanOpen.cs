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

namespace PX.Objects.PO.GraphExtensions.POReceiptEntryExt
{
	public class POLinePlanOpen : POLinePlanOpen<POReceiptEntry>
	{
	}

	public abstract class POLinePlanOpen<TGraph> : POLinePlanBase<TGraph, POLine>
		where TGraph : PXGraph
	{
		public override void _(Events.RowUpdated<POOrder> e)
		{
			base._(e);

			if (e.Row != null && e.OldRow != null && !e.Cache.ObjectsEqual<POOrder.hold>(e.Row, e.OldRow))
			{
				foreach (POLine split in PXSelect<POLine,
					Where<POLine.orderType, Equal<Current<POOrder.orderType>>,
						And<POLine.orderNbr, Equal<Current<POOrder.orderNbr>>>>>
					.SelectMultiBound(Base, new[] { e.Row }))
				{
					Base.Caches<POLine>().RaiseRowUpdated(split, PXCache<POLine>.CreateCopy(split));
				}
			}
		}
	}
}
