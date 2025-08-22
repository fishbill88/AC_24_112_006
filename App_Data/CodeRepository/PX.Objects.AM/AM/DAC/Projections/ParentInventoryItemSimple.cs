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
using PX.Objects.AM.CacheExtensions;
using PX.Objects.IN;

namespace PX.Objects.AM
{
	/// <summary>
	/// Parent item inventory only with small set of columns
	/// </summary>
	[PXProjection(typeof(Select2<InventoryItem, InnerJoin<AMBomItemByInventoryID, On<AMBomItemByInventoryID.inventoryID, Equal<InventoryItem.inventoryID>>>>), Persistent = false)]
	[Serializable]
	[PXHidden]
	public class ParentInventoryItemSimple : PXBqlTable, IBqlTable
	{
		#region InventoryID
		/// <inheritdoc cref="InventoryItem.InventoryID"/>
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
		[PXDBInt(IsKey = true, BqlField = typeof(InventoryItem.inventoryID))]
		[PXUIField(DisplayName = "Inventory ID")]
		public virtual Int32? InventoryID { get; set; }
		#endregion
		#region InventoryCD
		/// <inheritdoc cref="InventoryItem.InventoryCD"/>
		[PXDBString(InputMask = "", IsUnicode = true, BqlField = typeof(InventoryItem.inventoryCD))]
		[PXUIField(DisplayName = "Inventory CD")]
		public virtual String InventoryCD { get; set; }
		public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
		#endregion
		#region AMLowLevel
		public abstract class aMLowLevel : PX.Data.BQL.BqlInt.Field<aMLowLevel> { }

		[PXDBInt(BqlField = typeof(InventoryItemExt.aMLowLevel))]
		[PXUIField(DisplayName = "Low Level")]
		public Int32? AMLowLevel { get; set; }
		#endregion

		#region IsKit
		public abstract class isKit : PX.Data.BQL.BqlBool.Field<isKit> { }
		[PXBool()]
		[PXUIField(DisplayName = "Is Kit")]
		[PXUnboundDefault(false)]
		public virtual Boolean? IsKit { get; set; }
		#endregion

		#region KitRevision
		public abstract class kitRevision : PX.Data.BQL.BqlBool.Field<kitRevision> { }
		[PXDBString]
		[PXUIField(DisplayName = "Kit Revision")]
		public virtual String KitRevision { get; set; }
		#endregion
	}
}
