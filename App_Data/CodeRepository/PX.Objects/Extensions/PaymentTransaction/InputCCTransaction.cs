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

namespace PX.Objects.Extensions.PaymentTransaction
{
	[PXHidden]
	public class InputCCTransaction : PXBqlTable, IBqlTable
	{
		#region TranType
		public abstract class tranType : PX.Data.BQL.BqlString.Field<tranType> { }
		[PXString(IsUnicode = true)]
		[TranTypeList]
		[PXUIField(DisplayName = "Transaction type", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string TranType { get; set; }
		#endregion

		#region PCTranNumber
		public abstract class pCTranNumber : PX.Data.BQL.BqlString.Field<pCTranNumber> { }
		[PXString(50, IsUnicode = true)]
		[PXDefault]
		public virtual string PCTranNumber { get; set; }
		#endregion

		#region OrigPCTranNumber
		public abstract class origPCTranNumber : PX.Data.BQL.BqlString.Field<origPCTranNumber> { }
		[PXString(50, IsUnicode = true)]
		[PXDefault]
		public virtual string OrigPCTranNumber { get; set; }
		#endregion

		#region AuthNumber
		public abstract class authNumber : PX.Data.BQL.BqlString.Field<authNumber> { }
		[PXString(50, IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Proc. Center Auth. Nbr.")]
		public virtual string AuthNumber { get; set; }
		#endregion

		#region TransactionDate
		public abstract class tranDate : PX.Data.BQL.BqlString.Field<tranDate> { }
		[PXDate]
		[PXUIField(DisplayName = "Transaction Date")]
		public virtual DateTime? TranDate { get; set; }
		#endregion

		#region ExtProfileId
		public abstract class extProfileId : PX.Data.BQL.BqlString.Field<extProfileId> { }
		[PXString(255, IsUnicode = true)]
		public virtual string ExtProfileId { get; set; }
		#endregion

		#region NeedSync
		public abstract class needValidation : PX.Data.BQL.BqlBool.Field<needValidation> { }
		[PXBool]
		[PXDefault(false)]
		public virtual bool? NeedValidation { get; set; }
		#endregion

		#region Amount
		public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
		[PXDecimal(4)]
		[PXDefault]
		public virtual decimal? Amount { get; set; }
		#endregion

		#region ExpirationDate
		public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }
		[PXDate]
		public virtual DateTime? ExpirationDate { get; set; }
		#endregion

		#region CardType
		public abstract class cardType : PX.Data.BQL.BqlString.Field<cardType> { }

		/// <summary>
		/// Type of a card associated with the document. 
		/// </summary>
		[PXString]
		[PXUIField(DisplayName = "Card Type")]
		public virtual string CardType
		{
			get;
			set;
		}
		#endregion
	}
}
