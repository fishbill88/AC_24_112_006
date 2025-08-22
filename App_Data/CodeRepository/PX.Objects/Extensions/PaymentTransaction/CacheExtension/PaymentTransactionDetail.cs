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
using PX.Objects.AR.CCPaymentProcessing.Interfaces;
namespace PX.Objects.Extensions.PaymentTransaction
{
	public class PaymentTransactionDetail : PXMappedCacheExtension, ICCPaymentTransaction
	{
		public abstract class tranNbr : PX.Data.BQL.BqlInt.Field<tranNbr> { }
		public virtual int? TranNbr { get; set; }
		public abstract class transactionID : PX.Data.BQL.BqlInt.Field<transactionID> { }
		public virtual int? TransactionID { get; set; }
		public abstract class pMInstanceID : PX.Data.BQL.BqlInt.Field<pMInstanceID> { }
		public virtual int? PMInstanceID { get; set; }
		public abstract class processingCenterID : PX.Data.BQL.BqlString.Field<processingCenterID> { }
		public virtual string ProcessingCenterID { get; set; }
		public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
		public virtual string DocType { get; set; }
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
		public virtual string RefNbr { get; set; }
		public abstract class refTranNbr : PX.Data.BQL.BqlInt.Field<refTranNbr> { }
		public int? RefTranNbr { get; set; }
		public abstract class origDocType : PX.Data.BQL.BqlString.Field<origDocType> { }
		public virtual string OrigDocType { get; set; }
		public abstract class origRefNbr : PX.Data.BQL.BqlString.Field<origRefNbr> { }
		public virtual string OrigRefNbr { get; set; }
		public abstract class tranType : PX.Data.BQL.BqlString.Field<tranType> { }
		public virtual string TranType { get; set; }
		public abstract class procStatus : PX.Data.BQL.BqlString.Field<procStatus> { }
		public virtual string ProcStatus { get; set; }
		public abstract class tranStatus : PX.Data.BQL.BqlString.Field<tranStatus> { }
		public virtual string TranStatus { get; set; }
		public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }
		public virtual DateTime? ExpirationDate { get; set; }
		public abstract class pCTranNumber : PX.Data.BQL.BqlString.Field<pCTranNumber> { }
		public string PCTranNumber { get; set; }
		public abstract class authNumber : PX.Data.BQL.BqlString.Field<authNumber> { }
		public string AuthNumber { get; set; }
		public abstract class pCResponseReasonText : PX.Data.BQL.BqlString.Field<pCResponseReasonText> { }
		public string PCResponseReasonText { get; set; }
		public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
		public decimal? Amount { get; set; }
		public abstract class imported : PX.Data.BQL.BqlBool.Field<imported> { }
		public bool? Imported { get; set; }
	}
}
