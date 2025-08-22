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
using PX.Objects.IN;
using System;

namespace PX.Objects.AM
{
	/// <summary>
	/// PXProjection of <see cref="AMRPDetail"/>.
	/// </summary>
	[Serializable]
	[PXHidden]
	[PXProjection(typeof(Select<AMRPDetail>))]
	public class AMRPDetailItemSite : PXBqlTable, IBqlTable
	{
		#region InventoryID (Key)
		/// <inheritdoc cref="AMRPDetail.InventoryID"/>
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

		/// <inheritdoc cref="AMRPDetail.InventoryID"/>
		[PXDBInt(IsKey = true, BqlField = typeof(AMRPDetail.inventoryID))]
		[PXUIField(DisplayName = "Inventory ID")]
		public virtual int? InventoryID { get; set; }

		#endregion
		#region SubItemID (Key)
		/// <inheritdoc cref="AMRPDetail.SubItemID"/>
		public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
		/// <inheritdoc cref="AMRPDetail.SubItemID"/>
		[PXDBInt(IsKey = true, BqlField = typeof(AMRPDetail.subItemID))]
		[PXUIField(DisplayName = "Subitem")]
		public virtual Int32? SubItemID { get; set; }
		#endregion
		#region SiteID (Key)
		/// <inheritdoc cref="AMRPDetail.SiteID"/>
		public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }

		/// <inheritdoc cref="AMRPDetail.SiteID"/>
		[PXDBInt(IsKey = true, BqlField = typeof(AMRPDetail.siteID))]
		[PXUIField(DisplayName = "Warehouse")]
		public virtual int? SiteID { get; set; }
		#endregion
		#region BaseQty
		/// <inheritdoc cref="AMRPDetail.BaseQty"/>
		public abstract class baseQty : PX.Data.BQL.BqlDecimal.Field<baseQty> { }

		/// <inheritdoc cref="AMRPDetail.BaseQty"/>
		[PXDBQuantity(BqlField = typeof(AMRPDetail.baseQty))]
		[PXUIField(DisplayName = "Base Qty")]
		public virtual decimal? BaseQty { get; set; }
		#endregion
		#region PromiseDate
		/// <inheritdoc cref="AMRPDetail.PromiseDate"/>
		public abstract class promiseDate : PX.Data.BQL.BqlDateTime.Field<promiseDate> { }

		/// <inheritdoc cref="AMRPDetail.PromiseDate"/>
		[PXDBDate(BqlField = typeof(AMRPDetail.promiseDate))]
		[PXUIField(DisplayName = "Promise Date")]
		public virtual DateTime? PromiseDate { get; set; }
		#endregion
		#region BranchID
		/// <inheritdoc cref="AMRPDetail.BranchID"/>
		public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }

		/// <inheritdoc cref="AMRPDetail.BranchID"/>
		[PXDBInt(BqlField = typeof(AMRPDetail.branchID))]
		[PXUIField(DisplayName = "Branch")]
		public virtual int? BranchID { get; set; }
		#endregion
		#region ItemClassID
		/// <inheritdoc cref="AMRPDetail.ItemClassID"/>
		public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }

		/// <inheritdoc cref="AMRPDetail.ItemClassID"/>
		[PXDBInt(BqlField = typeof(AMRPDetail.itemClassID))]
		[PXUIField(DisplayName = "Item Class")]
		public virtual int? ItemClassID { get; set; }
		#endregion
	}
}
