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
		InnerJoin<POReceiptLineSigned,
			On<POReceiptLineSigned.pOType, Equal<POLine.orderType>,
			And<POReceiptLineSigned.pONbr, Equal<POLine.orderNbr>,
			And<POReceiptLineSigned.pOLineNbr, Equal<POLine.lineNbr>>>>,
		InnerJoin<POReceipt,
			On<POReceiptLineSigned.receiptType, Equal<POReceipt.receiptType>,
				And<POReceiptLineSigned.receiptNbr, Equal<POReceipt.receiptNbr>>>>>,
		Where<POLine.pOType, IsNotNull>,
		Aggregate<
			GroupBy<POLine.pOType,
			GroupBy<POLine.pONbr,
			GroupBy<POLine.orderType,
			GroupBy<POLine.orderNbr,
			GroupBy<POReceiptLineSigned.receiptType,
			GroupBy<POReceiptLineSigned.receiptNbr,
			Sum<POReceiptLineSigned.signedBaseReceiptQty>>>>>>>>>), Persistent = false)]
	[Serializable]
	[PXCacheName(Messages.POBlanketOrderPOReceipt)]
	public partial class POBlanketOrderPOReceipt : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<POBlanketOrderPOReceipt>.By<pOType, pONbr, orderType, orderNbr, receiptType, receiptNbr>
		{
			public static POBlanketOrderPOReceipt Find(PXGraph graph, string pOType, string pONbr, string orderType, string orderNbr, string receiptType, string receiptNbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, pOType, pONbr, orderType, orderNbr, receiptType, receiptNbr, options);
		}
		public static class FK
		{
			public class Order : POOrder.PK.ForeignKeyOf<POBlanketOrderPOReceipt>.By<orderType, orderNbr> { }
			public class Receipt : POReceipt.PK.ForeignKeyOf<POBlanketOrderPOReceipt>.By<receiptType, receiptNbr> { }
		}
		#endregion

		#region ReceiptType
		public abstract class receiptType : PX.Data.BQL.BqlString.Field<receiptType> { }

		[PXDBString(2, IsFixed = true, InputMask = "", IsKey = true, BqlField = typeof(POReceiptLineSigned.receiptType))]
		[POReceiptType.List()]
		[PXUIField(DisplayName = "Receipt Type")]
		public virtual String ReceiptType
		{
			get;
			set;
		}
		#endregion

		#region ReceiptNbr
		public abstract class receiptNbr : PX.Data.BQL.BqlString.Field<receiptNbr> { }

		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(POReceiptLineSigned.receiptNbr))]
		[PXUIField(DisplayName = POOrderPOReceipt.receiptNbr.DisplayName, Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<POReceipt.receiptNbr>), Filterable = true)] // Should not contain Where<receiptTyp, Optional<ReceiptType>> because POBlanketOrderPOReceipt.ReceiptType is null, the DAC is in the join seciton of the view. NavigateParams section is used, the select will be executed by keys (searches).
		public virtual String ReceiptNbr
		{
			get;
			set;
		}
		#endregion

		#region OrderType
		public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }

		[PXDBString(2, IsFixed = true, IsKey = true, BqlField = typeof(POLine.orderType))]
		[POOrderType.List()]
		[PXUIField(DisplayName = "Order Type", Enabled = false, IsReadOnly = true)]
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

		#region ReceiptDate
		public abstract class receiptDate : PX.Data.BQL.BqlDateTime.Field<receiptDate> { }

		/// <summary>
		/// Date of the document.
		/// </summary>
		[PXDBDate(BqlField = typeof(POReceipt.receiptDate))]
		[PXUIField(DisplayName = "Receipt Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? ReceiptDate
		{
			get;
			set;
		}
		#endregion

		#region Status
		public abstract class status : PX.Data.BQL.BqlString.Field<status> { }

		[PXDBString(1, IsFixed = true, BqlField = typeof(POReceipt.status))]
		[PXUIField(DisplayName = "Receipt Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[POReceiptStatus.List]
		public virtual string Status
		{
			get;
			set;
		}
		#endregion

		#region TotalQty
		public abstract class totalQty : PX.Data.BQL.BqlDecimal.Field<totalQty> { }

		[PXDBQuantity(BqlField = typeof(POReceiptLineSigned.signedBaseReceiptQty))]
		[PXUIField(DisplayName = "Received Qty.", Enabled = false)]
		public virtual Decimal? TotalQty
		{
			get;
			set;
		}
		#endregion
	}
}
