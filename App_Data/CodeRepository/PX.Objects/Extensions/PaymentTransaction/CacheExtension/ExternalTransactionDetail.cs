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
	[PXHidden]
	public class ExternalTransactionDetail : PXMappedCacheExtension, IExternalTransaction
	{
		public abstract class transactionID : PX.Data.BQL.BqlInt.Field<transactionID> { }
		public int? TransactionID { get; set; }
		public abstract class pMInstanceID : PX.Data.BQL.BqlInt.Field<pMInstanceID> { }
		public int? PMInstanceID { get; set; }
		public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
		public string DocType { get; set; }
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
		public string RefNbr { get; set; }
		public abstract class origDocType : PX.Data.BQL.BqlString.Field<origDocType> { }
		public string OrigDocType { get; set; }
		public abstract class origRefNbr : PX.Data.BQL.BqlString.Field<origRefNbr> { }
		public string OrigRefNbr { get; set; }
		public abstract class voidDocType : PX.Data.BQL.BqlString.Field<voidDocType> { }
		public string VoidDocType { get; set; }
		public abstract class voidRefNbr : PX.Data.BQL.BqlString.Field<voidRefNbr> { }
		public string VoidRefNbr { get; set; }
		public abstract class tranNumber : PX.Data.BQL.BqlString.Field<tranNumber> { }
		public string TranNumber { get; set; }
		public abstract class authNumber : PX.Data.BQL.BqlString.Field<authNumber> { }
		public string AuthNumber { get; set; }
		public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
		public decimal? Amount { get; set; }
		public abstract class procStatus : PX.Data.BQL.BqlDecimal.Field<procStatus> { }
		public string ProcStatus { get; set; }
		public abstract class lastActivityDate : PX.Data.BQL.BqlDateTime.Field<lastActivityDate> { }
		public DateTime? LastActivityDate { get; set; }
		public abstract class direction : PX.Data.BQL.BqlString.Field<direction> { }
		public string Direction { get; set; }
		public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
		public bool? Active { get; set; }
		public abstract class completed : PX.Data.BQL.BqlBool.Field<completed> { }
		public bool? Completed { get; set; }
		public abstract class needSync : PX.Data.BQL.BqlBool.Field<needSync> { }
		public bool? NeedSync { get; set; }
		public abstract class saveProfile : PX.Data.BQL.BqlBool.Field<saveProfile> { }
		public bool? SaveProfile { get; set; }
		public abstract class parentTranID : PX.Data.BQL.BqlInt.Field<parentTranID> { }
		public int? ParentTranID { get; set; }
		public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }
		public DateTime? ExpirationDate { get; set; }
		public abstract class cVVVerification : PX.Data.BQL.BqlString.Field<cVVVerification> { }
		public string CVVVerification { get; set; }
		public abstract class processingCenterID : PX.Data.BQL.BqlString.Field<processingCenterID> { }
		public string ProcessingCenterID { get; set; }
		public abstract class syncStatus : PX.Data.BQL.BqlString.Field<syncStatus> { }
		public string SyncStatus { get; set; }
		public abstract class syncMessage : PX.Data.BQL.BqlString.Field<syncMessage> { }
		public string SyncMessage { get; set; }
		public abstract class noteID : PX.Data.BQL.BqlString.Field<noteID> { }
		public Guid? NoteID { get; set; }
	}
}
