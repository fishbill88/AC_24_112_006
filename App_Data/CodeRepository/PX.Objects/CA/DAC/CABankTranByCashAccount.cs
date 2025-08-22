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
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;

namespace PX.Objects.CA
{
	/// <summary>
	/// Contains totals of unprocessed Bank Transactions grouped by Cash Account
	/// </summary>
	[PXProjection(typeof(Select4<CABankTranSplitDebitCredit,
		Aggregate<
			GroupBy<CABankTranSplitDebitCredit.cashAccountID,
			Sum<CABankTranSplitDebitCredit.curyDebitAmount,
			Sum<CABankTranSplitDebitCredit.curyCreditAmount,
			Sum<CABankTranSplitDebitCredit.debitNumber,
			Sum<CABankTranSplitDebitCredit.creditNumber,
			Sum<CABankTranSplitDebitCredit.unprocessedNumber,
			Sum<CABankTranSplitDebitCredit.matchedNumber>>>>>>>>>))]
	[PXCacheName(Messages.BankTranByCashAccount)]
	public class CABankTranByCashAccount : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<CABankTranByCashAccount>.By<cashAccountID>
		{
			public static CABankTranByCashAccount Find(PXGraph graph, int? cashAccountID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, cashAccountID, options);
		}
		#endregion

		#region CashAccountID

		public abstract class cashAccountID : BqlInt.Field<cashAccountID> { }
		[PXDBInt(BqlField = typeof(CABankTranSplitDebitCredit.cashAccountID))]
		public virtual int? CashAccountID
		{
			get;
			set;
		}
		#endregion

		#region CuryDebitAmount
		public abstract class curyDebitAmount : IBqlField { }
		[PXDBDecimal(BqlField = typeof(CABankTranSplitDebitCredit.curyDebitAmount))]
		[PXUIField(DisplayName = "Unprocessed Receipts")]
		public virtual decimal? CuryDebitAmount
		{
			get;
			set;
		}
		#endregion

		#region CuryCreditAmount
		public abstract class curyCreditAmount : IBqlField { }
		[PXDBDecimal(BqlField = typeof(CABankTranSplitDebitCredit.curyCreditAmount))]
		[PXUIField(DisplayName = "Unprocessed Disb.")]
		public virtual decimal? CuryCreditAmount
		{
			get;
			set;
		}
		#endregion

		#region DebitNumber
		public abstract class debitNumber : IBqlField { }

		[PXDBInt(BqlField = typeof(CABankTranSplitDebitCredit.debitNumber))]
		[PXUIField(DisplayName = "Receipt Count")]
		public virtual int? DebitNumber
		{
			get;
			set;
		}
		#endregion

		#region CreditNumber
		public abstract class creditNumber : IBqlField { }

		[PXDBInt(BqlField = typeof(CABankTranSplitDebitCredit.creditNumber))]
		[PXUIField(DisplayName = "Disbursement Count")]
		public virtual int? CreditNumber
		{
			get;
			set;
		}
		#endregion

		#region UnprocessedNumber
		public abstract class unprocessedNumber : IBqlField { }

		[PXDBInt(BqlField = typeof(CABankTranSplitDebitCredit.unprocessedNumber))]
		[PXUIField(DisplayName = "Unmatched Count")]
		public virtual int? UnprocessedNumber
		{
			get;
			set;
		}
		#endregion

		#region MatchedNumber
		public abstract class matchedNumber : IBqlField { }

		[PXDBInt(BqlField = typeof(CABankTranSplitDebitCredit.matchedNumber))]
		[PXUIField(DisplayName = "Matched Count")]
		public virtual int? MatchedNumber
		{
			get;
			set;
		}
		#endregion
	}

	/// <summary>
	/// Contains unprocessed Bank Transactions with the Amount split into Debit and Credit depending on DrCr value
	/// </summary>
	[PXProjection(typeof(Select<CABankTran,
		Where<CABankTran.processed, Equal<boolFalse>, And<CABankTran.tranType, Equal<CABankTranType.statement>>>>))]
	[PXHidden]
	public class CABankTranSplitDebitCredit : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<CABankTranSplitDebitCredit>.By<cashAccountID>
		{
			public static CABankTranSplitDebitCredit Find(PXGraph graph, int? cashAccountID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, cashAccountID, options);
		}
		#endregion

		#region CashAccountID
		public abstract class cashAccountID : IBqlField { }

		/// <summary>
		/// The cash account specified on the bank statement for which you want to upload bank transactions.
		/// This field is a part of the compound key of the document.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="CashAccount.CashAccountID"/> field.
		/// </value>
		[PXDBInt(IsKey = true, BqlField = typeof(CABankTran.cashAccountID))]
		public virtual int? CashAccountID
		{
			get;
			set;
		}
		#endregion

		#region CuryDebitAmount
		public abstract class curyDebitAmount : IBqlField { }

		[PXDecimal]
		[PXDBCalced(typeof(IIf<Where<CABankTran.drCr, Equal<CADrCr.cADebit>>, CABankTran.curyTranAmt, decimal0>), typeof(decimal))]
		public virtual decimal? CuryDebitAmount
		{
			get;
			set;
		}
		#endregion

		#region CuryCreditAmount
		public abstract class curyCreditAmount : IBqlField { }

		[PXDecimal]
		[PXDBCalced(typeof(IIf<Where<CABankTran.drCr, Equal<CADrCr.cACredit>>, PX.Data.BQL.Minus<CABankTran.curyTranAmt>, decimal0>), typeof(decimal))]
		public virtual decimal? CuryCreditAmount
		{
			get;
			set;
		}
		#endregion

		#region DebitNumber
		public abstract class debitNumber : IBqlField { }

		[PXInt]
		[PXDBCalced(typeof(IIf<Where<CABankTran.drCr, Equal<CADrCr.cADebit>>, int1, int0>), typeof(int))]
		public virtual int? DebitNumber
		{
			get;
			set;
		}
		#endregion

		#region CreditNumber
		public abstract class creditNumber : IBqlField { }

		[PXInt]
		[PXDBCalced(typeof(IIf<Where<CABankTran.drCr, Equal<CADrCr.cACredit>>, int1, int0>), typeof(int))]
		public virtual int? CreditNumber
		{
			get;
			set;
		}
		#endregion

		#region UnprocessedNumber
		public abstract class unprocessedNumber : IBqlField { }

		[PXInt]
		[PXDBCalced(typeof(IIf<Where<CABankTran.documentMatched, NotEqual<True>, And<CABankTran.processed, NotEqual<True>>>, int1, int0>), typeof(int))]
		public virtual int? UnprocessedNumber
		{
			get;
			set;
		}
		#endregion

		#region MatchedNumber
		public abstract class matchedNumber : IBqlField { }

		[PXInt]
		[PXDBCalced(typeof(IIf<Where<CABankTran.documentMatched, Equal<True>>, int1, int0>), typeof(int))]
		public virtual int? MatchedNumber
		{
			get;
			set;
		}
		#endregion
	}
}
