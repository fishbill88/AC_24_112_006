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
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.Localizations.CA.AP;
using PX.Objects.Localizations.CA.AP.DAC;

namespace PX.Objects.Localizations.CA
{
	/// <summary>
	/// Projection DAC for aggregating payments for the purpose of T5018 calculations.
	/// </summary>
	[PXHidden]
	[Serializable]
	[PXProjection(typeof(SelectFrom<BAccountR>.
		InnerJoin<T5018Transactions>.On<T5018Transactions.vendorID.IsEqual<BAccountR.bAccountID>>.
		InnerJoin<APPayment>.On<APPayment.docType.IsEqual<T5018Transactions.docType>.
			And<APPayment.refNbr.IsEqual<T5018Transactions.refNbr>>>.

		LeftJoin<APAdjust>.On<APAdjust.adjgDocType.IsEqual<T5018Transactions.docType>.
			And<APAdjust.adjgRefNbr.IsEqual<T5018Transactions.refNbr>>>.

		LeftJoin<APRetainageInvoice>.On<APRetainageInvoice.refNbr.IsEqual<APAdjust.adjdRefNbr>.
			And<APRetainageInvoice.docType.IsEqual<APAdjust.adjdDocType>>>.

		LeftJoin<APTran>.
			On<APTran.tranType.IsEqual<APAdjust.adjdDocType>.
				And<APTran.refNbr.IsEqual<APAdjust.adjdRefNbr>>.
				And<Brackets<APTran.lineNbr.IsEqual<APAdjust.adjdLineNbr>.
					Or<APAdjust.adjdLineNbr.IsEqual<Zero>>>>>.

		LeftJoin<APTax>.
			On<APTax.refNbr.IsEqual<APTran.refNbr>.
				And<APTax.lineNbr.IsEqual<APTran.lineNbr>.
				And<APTax.tranType.IsEqual<APTran.tranType>>>>.

		Where<T5018VendorExt.vendorT5018.IsEqual<True>.
			And<Brackets<APAdjust.released.IsEqual<True>>.Or<APAdjust.released.IsNull>>.
			And<Brackets<Brackets<T5018Transactions.docType.IsEqual<APDocType.prepayment>.
				And<Brackets<APAdjust.adjdDocType.IsNotEqual<APDocType.prepayment>.
					Or<Brackets<APAdjust.adjdDocType.IsNull>>>>>.
				Or<Brackets<T5018Transactions.docType.IsEqual<APDocType.check>.
					And<APAdjust.adjdDocType.IsEqual<APDocType.invoice>.
					And<APTranExt.t5018Service.IsEqual<True>>>>>.
				Or<Brackets<T5018Transactions.docType.IsEqual<APDocType.check>.
					And<APAdjust.adjdDocType.IsEqual<APDocType.prepayment>>>>.
				Or<Brackets<T5018Transactions.docType.IsEqual<APDocType.quickCheck>>.
					And<APTranExt.t5018Service.IsEqual<True>>>>>>
		.AggregateTo<
			GroupBy<T5018Transactions.branchID>,
			GroupBy<T5018Transactions.vendorID>,
			GroupBy<T5018Transactions.docType>,
			GroupBy<T5018Transactions.refNbr>,
			GroupBy<APAdjust.adjdLineNbr>,
			GroupBy<APTran.tranID>,
			Sum<APTax.taxRate>>
		))]
	public class T5018VendorTransaction : BAccountR
	{

		#region AdjdDocType
		public abstract class adjdDocType : BqlString.Field<adjdDocType> { }
		/// <summary>
		/// The type of the adjusted document.
		/// </summary>
		[PXDBString(BqlField = typeof(APAdjust.adjdDocType))]
		public virtual String AdjdDocType { get; set; }
		#endregion

		#region AdjdRefNbr
		public new abstract class adjdRefNbr : PX.Data.BQL.BqlString.Field<adjdRefNbr> { }
		/// <summary>
		/// Reference number of the adjusted document.
		/// </summary>
		[PXDBString(BqlField = typeof(APAdjust.adjdRefNbr))]
		public virtual String AdjdRefNbr { get; set; }
		#endregion

		#region VendorID
		public abstract class vendorID : BqlInt.Field<vendorID> { }
		/// <summary>
		/// The vendor ID of associated payments.
		/// </summary>
		[PXDBInt(BqlField = typeof(bAccountID))]
		public virtual int? VendorID { get; set; }
		#endregion

		#region BranchID
		public abstract class branchID : BqlInt.Field<branchID> { }
		/// <summary>
		/// The branch ID of associated payments.
		/// </summary>
		[PXDBInt(BqlField = typeof(APPayment.branchID))]
		public virtual int? BranchID { get; set; }
		#endregion

		#region AdjdDocDate
		public abstract class adjdDocDate : BqlDateTime.Field<adjdDocDate> { }
		/// <summary>
		/// Either the date when the adjusted document was created or the date of the original vendor's document.
		/// </summary>
		[PXDBDate(BqlField = typeof(APAdjust.adjdDocDate))]
		public virtual DateTime? AdjdDocDate { get; set; }
		#endregion

		#region AdjgDocDate
		public abstract class adjgDocDate : BqlDateTime.Field<adjgDocDate> { }
		/// <summary>
		/// The date when the payment is applied.
		/// </summary>
		[PXDBDate(BqlField = typeof(APAdjust.adjgDocDate))]
		public virtual DateTime? AdjgDocDate { get; set; }
		#endregion

		#region Released
		public abstract class released : BqlBool.Field<released> { }
		/// <summary>
		/// When set to <c>true</c> indicates that the document was released.
		/// </summary>
		[PXDBBool(BqlField = typeof(APPayment.released))]
		public virtual bool? Released { get; set; }
		#endregion

		#region APAdjustReleased
		public abstract class aPAdjustReleased : BqlBool.Field<released> { }
		/// <summary>
		/// When set to <c>true</c> indicates that the adjustment was released.
		/// </summary>
		[PXDBBool(BqlField = typeof(APAdjust.released))]
		public virtual bool? APAdjustReleased { get; set; }
		#endregion

		#region Voided
		public abstract class voided : BqlBool.Field<voided> { }
		/// <summary>
		/// When set to <c>true</c> indicates that the document was voided.
		/// </summary>
		[PXDBBool(BqlField = typeof(APPayment.voided))]
		public virtual bool? Voided { get; set; }
		#endregion

		#region APAdjustVoided
		public abstract class aPAdjustVoided : BqlBool.Field<voided> { }
		/// <summary>
		/// When set to <c>true</c> indicates that the adjustment was voided.
		/// </summary>
		[PXDBBool(BqlField = typeof(APAdjust.voided))]
		public virtual bool? APAdjustVoided { get; set; }
		#endregion

		#region IncludeInT5018Report
		public abstract class includeInT5018Report : BqlString.Field<includeInT5018Report> { }
		/// <summary>
		/// The type of the adjusted document.
		/// </summary>
		[PXDBBool(BqlField = typeof(APPaymentExt.includeInT5018Report))]
		public virtual bool? IncludeInT5018Report { get; set; }
		#endregion

		#region DocDate
		public abstract class docDate : BqlDateTime.Field<docDate> { }
		/// <summary>
		/// Either the date when the adjusted document was created or the date of the original vendor's document.
		/// </summary>
		[PXDBDate(BqlField = typeof(APPayment.docDate))]
		public virtual DateTime? DocDate { get; set; }
		#endregion

		#region DocType
		public abstract class docType : BqlString.Field<docType> { }
		/// <summary>
		/// The type of the adjusted document.
		/// </summary>
		[PXDBString(3, IsKey = true, BqlField = typeof(APPayment.docType))]
		public virtual String DocType { get; set; }
		#endregion

		#region RefNbr
		public new abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
		/// <summary>
		/// Reference number of the document.
		/// </summary>
		[PXDBString(15, IsKey = true, BqlField = typeof(APPayment.refNbr))]
		public virtual String RefNbr { get; set; }
		#endregion

		#region CuryOrigDocAmt
		public abstract class curyOrigDocAmt : PX.Data.BQL.BqlDecimal.Field<curyOrigDocAmt> { }
		/// <summary>
		/// The amount to be paid for the document in the currency of the document. (See <see cref="CuryID"/>)
		/// </summary>
		[PXDBDecimal(BqlField = typeof(APPayment.curyOrigDocAmt))]
		public virtual Decimal? CuryOrigDocAmt { get; set; }
		#endregion

		#region CuryTranAmt
		public abstract class curyTranAmt : PX.Data.BQL.BqlDecimal.Field<curyTranAmt> { }
		/// <summary>
		/// The total amount for the specified quantity of items or services of this type (after discount has been taken),
		/// or the amount of debit adjustment or prepayment.
		/// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
		/// </summary>
		[PXDBDecimal(BqlField = typeof(APTran.curyTranAmt))]
		public virtual Decimal? CuryTranAmt { get; set; }
		#endregion

		#region CuryAdjgAmt
		public abstract class curyAdjgAmt : PX.Data.BQL.BqlDecimal.Field<curyAdjgAmt> { }
		/// <summary>
		/// The actual amount paid on the document.
		/// Presented in the currency of the document, see <see cref="APRegister.CuryID"/>.
		/// </summary>
		[PXDBDecimal(BqlField = typeof(APAdjust.curyAdjgAmt))]
		public virtual Decimal? CuryAdjgAmt { get; set; }
		#endregion

		#region T5018Service
		public abstract class t5018Service : PX.Data.BQL.BqlBool.Field<t5018Service> { }

		[PXDBBool(BqlField = typeof(APTranExt.t5018Service))]
		public bool? T5018Service { get; set; }
		#endregion

		#region TranID
		public abstract class tranID : PX.Data.BQL.BqlInt.Field<tranID> { }

		/// <summary>
		/// Internal unique identifier of the transaction line.
		/// </summary>
		/// <value>
		/// The value is an auto-generated database identity.
		/// </value>
		[PXDBInt(IsKey = true, BqlField = typeof(APTran.tranID))]
		public virtual Int32? TranID { get; set; }
		#endregion

		#region CuryDocBal
		public new abstract class curyDocBal : PX.Data.BQL.BqlDecimal.Field<curyDocBal> { }
		[PXDBDecimal(BqlField = typeof(APPayment.curyDocBal))]
		public virtual Decimal? CuryDocBal { get; set; }
		#endregion

		#region InvoiceTotal
		public abstract class invoiceTotal : BqlDecimal.Field<invoiceTotal> { }
		/// <summary>
		/// The amount to be paid for the document in the currency of the document. (See <see cref="CuryID"/>)
		/// </summary>
		[PXDBDecimal(BqlField = typeof(APRegister.curyOrigDocAmt))]
		public virtual decimal? InvoiceTotal { get; set; }
		#endregion

		#region OrigRefNbr
		public abstract class origRefNbr : BqlString.Field<origRefNbr> { }
		[PXDBString(BqlField = typeof(APRetainageInvoice.origRefNbr))]
		public virtual string OrigRefNbr { get; set; }
		#endregion

		#region OrigDocType
		public abstract class origDocType : BqlString.Field<origDocType> { }
		[PXDBString(BqlField = typeof(APRetainageInvoice.origDocType))]
		public virtual string OrigDocType { get; set; }
		#endregion

		#region CuryTaxableAmt
		public abstract class curyTaxableAmt : PX.Data.BQL.BqlDecimal.Field<curyTaxableAmt> { }
		[PXDBDecimal(BqlField = typeof(APTax.taxableAmt))]
		public virtual Decimal? CuryTaxableAmt { get; set; }
		#endregion

		#region AdjdLineNbr
		public abstract class adjdLineNbr : PX.Data.BQL.BqlInt.Field<adjdLineNbr> { }
		[PXDBInt(IsKey = true, BqlField = typeof(APAdjust.adjdLineNbr))]
		public virtual int? AdjdLineNbr { get; set; }
		#endregion

		#region RetainageApply
		public abstract class retainageApply : BqlDecimal.Field<retainageApply> { }
		/// <summary>
		/// If the document is the parent for a retainage bill.
		/// </summary>
		[PXDBBool(BqlField = typeof(APRegister.retainageApply))]
		public virtual bool? RetainageApply { get; set; }
		#endregion

		#region CuryLineAmt
		public abstract class curyLineAmt : PX.Data.BQL.BqlInt.Field<curyLineAmt> { }
		/// <summary>
		/// The extended cost of the item or service associated with the line, which is the unit price multiplied by the quantity.
		/// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
		/// </summary>
		[PXDBDecimal(BqlField = typeof(APTran.curyLineAmt))]
		public virtual Decimal? CuryLineAmt { get; set; }
		#endregion

		#region TranType
		public new abstract class tranType : PX.Data.BQL.BqlString.Field<tranType> { }
		/// <summary>
		/// The type of the transaction.
		/// </summary>
		/// <value>
		/// The field is determined by the type of the parent <see cref="APRegister">document</see>.
		/// For the list of possible values see <see cref="APRegister.DocType"/>.
		/// </value>
		[PXDBString(BqlField = typeof(APRegister.docType))]
		public virtual String TranType { get; set; }
		#endregion

		#region CuryDiscTot
		public abstract class curyDiscTot : BqlDecimal.Field<APRegister.curyDiscTot> { }
		/// <summary>
		/// Total discount associated with the document in the currency of the document. (See <see cref="CuryID"/>)
		/// </summary>
		[PXDBDecimal(BqlField = typeof(APRegister.curyDiscTot))]
		public virtual Decimal? CuryDiscTot { get; set; }
		#endregion

		#region LineType
		public abstract class lineType : PX.Data.BQL.BqlString.Field<lineType> { }
		/// <summary>
		/// The type of the transaction line. This field is used to distinguish Discount lines from other ones.
		/// </summary>
		/// <value>
		/// Equals <c>"DS"</c> for discounts, <c>"LA"</c> for landed-cost transactions created in AP, 
		/// <c>"LP"</c> for landed-cost transactions created from PO, and empty string for common lines.
		/// </value>
		[PXDBString(BqlField = typeof(APTran.lineType))]
		public virtual String LineType { get; set; }
		#endregion
	}
}
