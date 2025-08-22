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
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.CM.Extensions;
using PX.Objects.GL;
using PX.Objects.IN;

namespace PX.Objects.PO
{
	[PXProjection(typeof(Select<APTran>), Persistent = false)]
	[Serializable]
	[PXHidden]
	public partial class APTranSigned : PXBqlTable, IBqlTable
	{
		#region TranType
		public abstract class tranType : PX.Data.BQL.BqlString.Field<tranType>
		{
		}
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(APTran.tranType))]
		public virtual string TranType
		{
			get;
			set;
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr>
		{
		}
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(APTran.refNbr))]
		public virtual string RefNbr
		{
			get;
			set;
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr>
		{
		}
		[PXDBInt(IsKey = true, BqlField = typeof(APTran.lineNbr))]
		public virtual int? LineNbr
		{
			get;
			set;
		}
		#endregion
		#region POOrderType
		public abstract class pOOrderType : PX.Data.BQL.BqlString.Field<pOOrderType>
		{
		}
		[PXDBString(2, IsFixed = true, BqlField = typeof(APTran.pOOrderType))]
		public virtual string POOrderType
		{
			get;
			set;
		}
		#endregion
		#region PONbr
		public abstract class pONbr : PX.Data.BQL.BqlString.Field<pONbr>
		{
		}
		[PXDBString(15, IsUnicode = true, BqlField = typeof(APTran.pONbr))]
		public virtual string PONbr
		{
			get;
			set;
		}
		#endregion
		#region POLineNbr
		public abstract class pOLineNbr : PX.Data.BQL.BqlInt.Field<pOLineNbr> { }
		[PXDBInt(BqlField = typeof(APTran.pOLineNbr))]
		public virtual int? POLineNbr
		{
			get;
			set;
		}
		#endregion
		#region SignedBaseQty
		public abstract class signedBaseQty : PX.Data.BQL.BqlDecimal.Field<signedBaseQty>
		{
		}
		[PXDBCalced(typeof(Switch<Case<Where<APTran.drCr, Equal<DrCr.debit>>, APTran.baseQty>, Minus<APTran.baseQty>>), typeof(decimal))]
		[PXQuantity]
		public virtual decimal? SignedBaseQty
		{
			get;
			set;
		}
		#endregion
		#region SignedCuryTranAmt
		public abstract class signedCuryTranAmt : PX.Data.BQL.BqlDecimal.Field<signedCuryTranAmt>
		{
		}
		[PXDBCalced(typeof(Switch<Case<Where<APTran.drCr, Equal<DrCr.debit>>, APTran.curyTranAmt>, Minus<APTran.curyTranAmt>>), typeof(decimal))]
		[CM.PXCury(typeof(POOrderAPDoc.curyID))]
		public virtual decimal? SignedCuryTranAmt
		{
			get;
			set;
		}
		#region SignedCuryRetainageAmt
		public abstract class signedCuryRetainageAmt : PX.Data.BQL.BqlDecimal.Field<signedCuryRetainageAmt>
		{
		}
		[PXDBCalced(typeof(Switch<Case<Where<APTran.drCr, Equal<DrCr.debit>>, APTran.curyRetainageAmt>, Minus<APTran.curyRetainageAmt>>), typeof(decimal))]
		[CM.PXCury(typeof(POOrderAPDoc.curyID))]
		public virtual decimal? SignedCuryRetainageAmt
		{
			get;
			set;
		}
		#endregion
		#endregion
		#region POPPVAmt
		public abstract class pOPPVAmt : PX.Data.BQL.BqlDecimal.Field<pOPPVAmt>
		{
		}
		[PXDBBaseCury(BqlField = typeof(APTran.pOPPVAmt))]
		public virtual decimal? POPPVAmt
		{
			get;
			set;
		}
		#endregion
	}

	[PXProjection(typeof(Select5<APTranSigned,
		InnerJoin<APInvoice, On<APInvoice.docType, Equal<APTranSigned.tranType>, And<APInvoice.refNbr, Equal<APTranSigned.refNbr>>>>,
		Where<APTranSigned.tranType, NotEqual<APDocType.prepayment>>,
		Aggregate<
			GroupBy<APTranSigned.pOOrderType,
			GroupBy<APTranSigned.pONbr,
			GroupBy<APInvoice.docType,
			GroupBy<APInvoice.refNbr,
			Sum<APTranSigned.signedBaseQty,
			Sum<APTranSigned.signedCuryTranAmt,
			Sum<APTranSigned.signedCuryRetainageAmt,
			Sum<APTranSigned.pOPPVAmt>>>>>>>>>>), Persistent = false)]
	[Serializable]
	[PXCacheName(Messages.POOrderAPDoc)]
	public partial class POOrderAPDoc : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<POOrderAPDoc>.By<docType, refNbr, pOOrderType, pONbr>
		{
			public static POOrderAPDoc Find(PXGraph graph, string docType, string refNbr, string pOOrderType, string pONbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, docType, refNbr, pOOrderType, pONbr, options);
		}
		public static class FK
		{
			public class APInvoice : AP.APInvoice.PK.ForeignKeyOf<POOrderAPDoc>.By<docType, refNbr> { }
			public class Order : POOrder.PK.ForeignKeyOf<POOrderAPDoc>.By<pOOrderType, pONbr> { }
			public class Currency : CM.Currency.PK.ForeignKeyOf<POOrderAPDoc>.By<curyID> { }
		}
		#endregion

		#region DocType
		public abstract class docType : PX.Data.BQL.BqlString.Field<docType>
		{
		}

		/// <summary>
		/// Type of the document.
		/// </summary>
		/// <value>
		/// Possible values are: "INV" - Invoice, "ACR" - Credit Adjustment, "ADR" - Debit Adjustment, 
		/// "CHK" - Payment, "VCK" - Voided Payment, "PPM" - Prepayment, "REF" - Refund,
		/// "QCK" - Cash Purchase, "VQC" - Voided Cash Purchase.
		/// </value>
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(APInvoice.docType))]
		[APDocType.List()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true)]
		public virtual String DocType
		{
			get;
			set;
		}
		#endregion

		#region RefNbr
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr>
		{
		}

		/// <summary>
		/// Reference number of the document.
		/// </summary>
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(APInvoice.refNbr))]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[PXSelector(typeof(Search<APInvoice.refNbr, Where<APInvoice.docType, Equal<Optional<docType>>>>), Filterable = true)]
		public virtual String RefNbr
		{
			get;
			set;
		}
		#endregion
		
		#region DocDate
		public abstract class docDate : PX.Data.BQL.BqlDateTime.Field<docDate>
		{
		}

		/// <summary>
		/// Date of the document.
		/// </summary>
		[PXDBDate(BqlField = typeof(APInvoice.docDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? DocDate
		{
			get;
			set;
		}
		#endregion

		#region Status
		public abstract class status : PX.Data.BQL.BqlString.Field<status> { }

		/// <summary>
		/// Status of the document. The field is calculated based on the values of status flag. It can't be changed directly.
		/// The fields tht determine status of a document are: <see cref="Hold"/>, <see cref="Released"/>, <see cref="Voided"/>, 
		/// <see cref="Scheduled"/>, <see cref="Prebooked"/>, <see cref="Printed"/>, <see cref="Approved"/>, <see cref="Rejected"/>.
		/// </summary>
		/// <value>
		/// Possible values are: 
		/// <c>"H"</c> - Hold, <c>"B"</c> - Balanced, <c>"V"</c> - Voided, <c>"S"</c> - Scheduled, 
		/// <c>"N"</c> - Open, <c>"C"</c> - Closed, <c>"P"</c> - Printed, <c>"K"</c> - Prebooked,
		/// <c>"E"</c> - Pending Approval, <c>"R"</c> - Rejected, <c>"Z"</c> - Reserved.
		/// Defaults to Hold.
		/// </value>
		[PXDBString(1, IsFixed = true, BqlField = typeof(APInvoice.status))]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[APDocStatus.List]
		public virtual string Status
		{
			get;
			set;
		}
		#endregion

		#region TotalQty
		public abstract class totalQty : PX.Data.BQL.BqlDecimal.Field<totalQty>
		{
		}
		[PXDBQuantity(BqlField = typeof(APTranSigned.signedBaseQty))]
		[PXUIField(DisplayName = "Billed Qty.", Enabled = false)]
		public virtual Decimal? TotalQty
		{
			get;
			set;
		}
		#endregion

		#region TotalTranAmt
		public abstract class totalTranAmt : PX.Data.BQL.BqlDecimal.Field<totalTranAmt>
		{
		}

		[PXDBBaseCury(BqlField = typeof(APTranSigned.signedCuryTranAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TotalTranAmt
		{
			get;
			set;
		}
		#endregion

		#region TotalRetainageAmt
		public abstract class totalRetainageAmt : PX.Data.BQL.BqlDecimal.Field<totalRetainageAmt>
		{
		}

		[PXDBBaseCury(BqlField = typeof(APTranSigned.signedCuryRetainageAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TotalRetainageAmt
		{
			get;
			set;
		}
		#endregion

		#region TotalAmt
		public abstract class totalAmt : PX.Data.BQL.BqlDecimal.Field<totalAmt>
		{
		}

		/// <summary>
		/// Billed Amount of the item or service associated with the line.
		/// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
		/// </summary>
		[PXBaseCury]
		[PXFormula(typeof(Add<totalTranAmt, totalRetainageAmt>))]
		[PXUIField(DisplayName = "Billed Amt.")]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? TotalAmt
		{
			get;
			set;
		}
		#endregion

		#region TotalPPVAmt
		public abstract class totalPPVAmt : PX.Data.BQL.BqlDecimal.Field<totalPPVAmt>
		{
		}

		/// <summary>
		/// Purchase price variance amount associated with the line.
		/// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
		/// </summary>
		/// <seealso cref="PX.Objects.PO.POReceiptLine.BillPPVAmt"/>
		[PXDBBaseCury(BqlField = typeof(APTranSigned.pOPPVAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "PPV Amt")]
		public virtual Decimal? TotalPPVAmt
		{
			get;
			set;
		}
		#endregion

		#region CuryID
		public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID>
		{
		}
		protected String _CuryID;

		/// <summary>
		/// Code of the <see cref="PX.Objects.CM.Currency">Currency</see> of the document.
		/// </summary>
		/// <value>
		/// Defaults to the <see cref="Company.BaseCuryID">company's base currency</see>.
		/// </value>
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(APInvoice.curyID))]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion

		#region POOrderType
		public abstract class pOOrderType : PX.Data.BQL.BqlString.Field<pOOrderType>
		{
		}

		/// <summary>
		/// The type of the corresponding <see cref="PX.Objects.PO.POOrder">PO Order</see>.
		/// Together with <see cref="PONbr"/> and <see cref="POLineNbr"/> links APTrans to the PO Orders and their lines.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="PX.Objects.PO.POOrder.OrderType">POOrder.OrderType</see> field.
		/// See its description for the list of allowed values.
		/// </value>
		[PXDBString(2, IsFixed = true, IsKey = true, BqlField = typeof(APTranSigned.pOOrderType))]
		[POOrderType.List()]
		[PXUIField(DisplayName = "PO Type", Enabled = false, IsReadOnly = true)]
		public virtual String POOrderType
		{
			get;
			set;
		}
		#endregion

		#region PONbr
		public abstract class pONbr : PX.Data.BQL.BqlString.Field<pONbr>
		{
		}

		/// <summary>
		/// The reference number of the corresponding <see cref="PX.Objects.PO.POOrder">PO Order</see>.
		/// Together with <see cref="POOrderType"/> and <see cref="POLineNbr"/> links APTrans to the PO Orders and their lines.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="PX.Objects.PO.POOrder.OrderNbr">POOrder.OrderNbr</see> field.
		/// </value>
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(APTranSigned.pONbr))]
		[PXUIField(DisplayName = "PO Number", Enabled = false, IsReadOnly = true)]
		[PXSelector(typeof(Search<POOrder.orderNbr, Where<POOrder.orderType, Equal<Optional<pOOrderType>>>>))]
		public virtual String PONbr
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
