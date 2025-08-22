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
using PX.Objects.AM.Attributes;
using PX.Objects.CS;
using PX.Objects.IN;
using System;

namespace PX.Objects.AM
{
	/// <summary>
	/// PXProjection of <see cref="AMRPDetailFP"/> for supply with supplied quantity remaining.
	/// </summary>
	[Serializable]
	[PXHidden]
	[PXProjection(typeof(Select<AMRPDetailFP,
		Where<AMRPDetailFP.sDFlag, Equal<MRPSDFlag.supply>,
					And<AMRPDetailFP.suppliedQty, Greater<decimal0>>>>))]
	public class AMRPDetailFPOpenSupply : PXBqlTable, IBqlTable
	{
		#region InventoryID (Key)
		/// <inheritdoc cref="AMRPDetailFP.InventoryID"/>
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

		/// <inheritdoc cref="AMRPDetailFP.InventoryID"/>
		[PXDBInt(IsKey = true, BqlField = typeof(AMRPDetailFP.inventoryID))]
		[PXUIField(DisplayName = "Inventory ID")]
		public virtual int? InventoryID { get; set; }

		#endregion
		#region SubItemID (Key)
		/// <inheritdoc cref="AMRPDetailFP.SubItemID"/>
		public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
		/// <inheritdoc cref="AMRPDetailFP.SubItemID"/>
		[PXDBInt(IsKey = true, BqlField = typeof(AMRPDetailFP.subItemID))]
		[PXUIField(DisplayName = "Subitem")]
		public virtual Int32? SubItemID { get; set; }
		#endregion
		#region SiteID (Key)
		/// <inheritdoc cref="AMRPDetailFP.SiteID"/>
		public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }

		/// <inheritdoc cref="AMRPDetailFP.SiteID"/>
		[PXDBInt(IsKey = true, BqlField = typeof(AMRPDetailFP.siteID))]
		[PXUIField(DisplayName = "Warehouse")]
		public virtual int? SiteID { get; set; }
		#endregion
		#region SuppliedQty
		/// <inheritdoc cref="AMRPDetailFP.SuppliedQty"/>
		public abstract class suppliedQty : PX.Data.BQL.BqlDecimal.Field<suppliedQty> { }

		/// <inheritdoc cref="AMRPDetailFP.SuppliedQty"/>
		[PXDBQuantity(BqlField = typeof(AMRPDetailFP.suppliedQty))]
		[PXUIField(DisplayName = "Supplied Qty")]
		public virtual decimal? SuppliedQty { get; set; }
		#endregion
		#region PlanDate
		/// <inheritdoc cref="AMRPDetailFP.PlanDate"/>
		public abstract class planDate : PX.Data.BQL.BqlDateTime.Field<planDate> { }

		/// <inheritdoc cref="AMRPDetailFP.PlanDate"/>
		[PXDBDate(BqlField = typeof(AMRPDetailFP.planDate))]
		[PXUIField(DisplayName = "Plan Date")]
		public virtual DateTime? PlanDate { get; set; }
		#endregion
	}
}
