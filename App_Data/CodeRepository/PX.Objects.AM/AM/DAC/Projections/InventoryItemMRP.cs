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

using System;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.AM.CacheExtensions;
using PX.Objects.IN;

namespace PX.Objects.AM
{
	/// <summary>
	/// Projection of <see cref="InventoryItem"/> for MRP only data required during the <see cref="MRPEngine"/> process.
	/// </summary>
	[PXProjection(typeof(Select<InventoryItem>), Persistent = false)]
	[Serializable]
	[PXHidden]
	public class InventoryItemMRP : PXBqlTable, IBqlTable
	{
		#region InventoryID (Key)
		/// <inheritdoc cref="InventoryID"/>
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

		/// <inheritdoc cref="InventoryItem.InventoryID"/>
		[PXDBInt(IsKey = true, BqlField = typeof(InventoryItem.inventoryID))]
		[PXUIField(DisplayName = "Inventory ID")]
		public virtual int? InventoryID { get; set; }

		#endregion
		#region BaseUnit
		/// <inheritdoc cref="BaseUnit"/>
		public abstract class baseUnit : BqlString.Field<baseUnit> { }

		/// <inheritdoc cref="InventoryItem.BaseUnit"/>
		[PXDBString(6, IsUnicode = true, InputMask = ">aaaaaa", BqlField = typeof(InventoryItem.baseUnit))]
		[PXUIField(DisplayName = "Base Unit")]
		public virtual String BaseUnit { get; set; }
		#endregion
		#region ItemClassID
		/// <inheritdoc cref="ItemClassID"/>
		public abstract class itemClassID : BqlInt.Field<itemClassID> { }
		/// <inheritdoc cref="InventoryItem.ItemClassID"/>
		[PXDBInt(BqlField = typeof(InventoryItem.itemClassID))]
		[PXUIField(DisplayName = "Item Class")]
		public virtual int? ItemClassID { get; set; }
		#endregion
		#region DefaultSubItemID
		/// <inheritdoc cref="DefaultSubItemID"/>
		public abstract class defaultSubItemID : BqlInt.Field<defaultSubItemID> { }
		/// <inheritdoc cref="InventoryItem.DefaultSubItemID"/>
		[PXDBInt(BqlField = typeof(InventoryItem.defaultSubItemID))]
		[PXUIField(DisplayName = "Default Subitem")]
		public virtual Int32? DefaultSubItemID { get; set; }
		#endregion
		#region StkItem
		/// <inheritdoc cref="StkItem"/>
		public abstract class stkItem : BqlBool.Field<stkItem> { }
		/// <inheritdoc cref="InventoryItem.StkItem"/>
		[PXDBBool(BqlField = typeof(InventoryItem.stkItem))]
		[PXUIField(DisplayName = "Stock Item")]
		public virtual Boolean? StkItem { get; set; }
		#endregion
		#region ItemStatus
		/// <inheritdoc cref="ItemStatus"/>
		public abstract class itemStatus : BqlString.Field<itemStatus> { }
		/// <inheritdoc cref="InventoryItem.ItemStatus"/>
		[PXDBString(2, IsFixed = true, BqlField = typeof(InventoryItem.itemStatus))]
		[PXUIField(DisplayName = "Item Status", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String ItemStatus { get; set; }
		#endregion
		#region PlanningMethod
		public abstract class planningMethod : BqlString.Field<planningMethod> { }

		/// <inheritdoc cref="InventoryItem.PlanningMethod"/>
		[PXDBString(1, IsFixed = true, BqlField = typeof(InventoryItem.planningMethod))]
		[PXUIField(DisplayName = "Planning Method")]
		public string PlanningMethod { get; set; }
		#endregion
		#region AMLowLevel (Extension Field)
		/// <inheritdoc cref="AMLowLevel"/>
		public abstract class aMLowLevel : PX.Data.BQL.BqlInt.Field<aMLowLevel> { }
		/// <inheritdoc cref="InventoryItemExt.AMLowLevel"/>
		[PXDBInt(BqlField = typeof(InventoryItemExt.aMLowLevel))]
		[PXUIField(DisplayName = "Low Level")]
		public Int32? AMLowLevel { get; set; }
		#endregion
	}
}
