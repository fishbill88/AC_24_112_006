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
using System;

namespace PX.Objects.Common.GraphExtensions.Abstract.DAC
{
	[PXHidden]
	public class TransactionLineQty : PXMappedCacheExtension
	{
		public virtual decimal? Qty { get; set; }
		public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }

		public virtual decimal? BaseQty { get; set; }
		public abstract class baseQty : PX.Data.BQL.BqlDecimal.Field<baseQty> { }

		public virtual Int32? InventoryID { get; set; }
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
	}
}
