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
using System.Linq;
using PX.Data;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.AP.MigrationMode;
using PX.Objects.Common.Attributes;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;

namespace PX.Objects.PO.DAC.Projections
{
	/// <exclude/>
	[PXProjection(typeof(Select5<POLine,
		InnerJoin<POOrder,
			On<POLine.FK.Order>>,
		Where<POLine.pOType, IsNotNull>,
		Aggregate<
			GroupBy<POLine.pOType,
			GroupBy<POLine.pONbr,
			GroupBy<POLine.orderType,
			GroupBy<POLine.orderNbr,
			Sum<POLine.baseOrderQty>>>>>>>), Persistent = false)]
	[Serializable]
	[PXCacheName(Messages.POBlanketOrderPOOrder)]
	public partial class POBlanketOrderPOOrder : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<POBlanketOrderPOOrder>.By<pOType, pONbr, orderType, orderNbr>
		{
			public static POBlanketOrderPOOrder Find(PXGraph graph, string pOType, string pONbr, string orderType, string orderNbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, pOType, pONbr, orderType, orderNbr, options);
		}
		public static class FK
		{
			public class Order : POOrder.PK.ForeignKeyOf<POBlanketOrderPOOrder>.By<orderType, orderNbr> { }
		}
		#endregion

		#region OrderDate
		public abstract class orderDate : PX.Data.BQL.BqlDateTime.Field<orderDate> { }

		/// <summary>
		/// Date of the document.
		/// </summary>
		[PXDBDate(BqlField = typeof(POOrder.orderDate))]
		[PXUIField(DisplayName = "Order Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? OrderDate
		{
			get;
			set;
		}
		#endregion

		#region Status
		public abstract class status : PX.Data.BQL.BqlString.Field<status> { }

		[PXDBString(1, IsFixed = true, BqlField = typeof(POOrder.status))]
		[PXUIField(DisplayName = "Order Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[POOrderStatus.List]
		public virtual string Status
		{
			get;
			set;
		}
		#endregion

		#region TotalQty
		public abstract class totalQty : PX.Data.BQL.BqlDecimal.Field<totalQty> { }

		[PXDBQuantity(BqlField = typeof(POLine.baseOrderQty))]
		[PXUIField(DisplayName = "Ordered Qty.", Enabled = false)]
		public virtual Decimal? TotalQty
		{
			get;
			set;
		}
		#endregion

		#region OrderType
		public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }

		[PXDBString(2, IsFixed = true, IsKey = true, BqlField = typeof(POLine.orderType))]
		[POOrderType.List()]
		[PXUIField(DisplayName = "Order Type", Enabled = false, IsReadOnly = true, Visible = false)]
		public virtual String OrderType
		{
			get;
			set;
		}
		#endregion

		#region OrderNbr
		public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }

		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(POLine.orderNbr))]
		[PXUIField(DisplayName = "Order Nbr.", Enabled = false, IsReadOnly = true)]
		[PXSelector(typeof(Search<POOrder.orderNbr, Where<POOrder.orderType, Equal<Optional<orderType>>>>))]
		public virtual String OrderNbr
		{
			get;
			set;
		}
		#endregion

		#region POType
		public abstract class pOType : PX.Data.BQL.BqlString.Field<pOType> { }

		[PXDBString(2, IsFixed = true, IsKey = true, BqlField = typeof(POLine.pOType))]
		public virtual String POType
		{
			get;
			set;
		}
		#endregion

		#region PONbr
		public abstract class pONbr : PX.Data.BQL.BqlString.Field<pONbr> { }

		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(POLine.pONbr))]
		public virtual String PONbr
		{
			get;
			set;
		}
		#endregion

		#region NoteID
		public abstract class noteID : PX.Data.IBqlField { }

		[ProjectionNote(typeof(POOrder), BqlField = typeof(POOrder.noteID))]
		public virtual Guid? NoteID
		{
			get;
			set;
		}
		#endregion

		#region StatusText
		public abstract class statusText : PX.Data.BQL.BqlString.Field<statusText>
		{
		}
		[PXString]
		public virtual String StatusText
		{
			get;
			set;
		}
		#endregion
	}
}
