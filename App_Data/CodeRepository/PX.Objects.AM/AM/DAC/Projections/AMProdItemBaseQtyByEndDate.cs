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
using PX.Objects.IN;
using System;

namespace PX.Objects.AM
{
	[Serializable]
	[PXProjection(typeof(Select<AMProdItem, Where<AMProdItem.canceled, Equal<False>>>))]
	[PXHidden]
	public class AMProdItemBaseQtyByEndDate : PXBqlTable, IBqlTable
	{
		#region OrderType
		/// <inheritdoc cref="AMProdItem.OrderType"/>
		public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }

		/// <inheritdoc cref="AMProdItem.OrderType"/>
		[AMOrderTypeField(IsKey = true, BqlField = typeof(AMProdItem.orderType))]
		public virtual String OrderType { get; set; }
		#endregion
		#region ProdOrdID
		/// <inheritdoc cref="AMProdItem.ProdOrdID"/>
		public abstract class prodOrdID : PX.Data.BQL.BqlString.Field<prodOrdID> { }

		/// <inheritdoc cref="AMProdItem.ProdOrdID"/>
		[ProductionNbr(IsKey = true, BqlField = typeof(AMProdItem.prodOrdID))]
		[PX.Data.EP.PXFieldDescription]
		public virtual String ProdOrdID { get; set; }
		#endregion
		#region InventoryID
		/// <inheritdoc cref="AMProdItem.InventoryID"/>
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

		/// <inheritdoc cref="AMProdItem.InventoryID"/>
		[PXDBInt(BqlField = typeof(AMProdItem.inventoryID))]
		[PXUIField(DisplayName = "Inventory ID")]
		public virtual Int32? InventoryID { get; set; }
		#endregion
		#region SubItemID
		/// <inheritdoc cref="AMProdItem.SubItemID"/>
		public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
		/// <inheritdoc cref="AMProdItem.SubItemID"/>
		[PXDBInt(BqlField = typeof(AMProdItem.subItemID))]
		[PXUIField(DisplayName = "Subitem", FieldClass = SubItemAttribute.DimensionName)]
		public virtual Int32? SubItemID { get; set; }
		#endregion
		#region SiteID
		/// <inheritdoc cref="AMProdItem.SiteID"/>
		public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }

		/// <inheritdoc cref="AMProdItem.SiteID"/>
		[PXDBInt(BqlField = typeof(AMProdItem.siteID))]
		[PXUIField(DisplayName = "Warehouse", FieldClass = SiteAttribute.DimensionName)]
		public virtual Int32? SiteID { get; set; }
		#endregion
		#region Hold
		public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }

		[PXDBBool(BqlField = typeof(AMProdItem.hold))]
		[PXUIField(DisplayName = "Hold")]
		public virtual Boolean? Hold { get; set; }
		#endregion
		#region EndDate
		/// <inheritdoc cref="AMProdItem.EndDate"/>
		public abstract class endDate : PX.Data.BQL.BqlDateTime.Field<endDate> { }

		/// <inheritdoc cref="AMProdItem.EndDate"/>
		[PXDBDate(BqlField = typeof(AMProdItem.endDate))]
		[PXUIField(DisplayName = "End Date")]
		public virtual DateTime? EndDate { get; set; }
		#endregion
		#region BaseQtytoProd
		/// <inheritdoc cref="AMProdItem.BaseQtytoProd"/>
		public abstract class baseQtytoProd : PX.Data.BQL.BqlDecimal.Field<baseQtytoProd> { }
		/// <inheritdoc cref="AMProdItem.BaseQtytoProd"/>
		[PXDBDecimal(6, BqlField = typeof(AMProdItem.baseQtytoProd))]
		[PXUIField(DisplayName = "Base Qty to Produce")]
		public virtual Decimal? BaseQtytoProd { get; set; }
		#endregion
	}
}
