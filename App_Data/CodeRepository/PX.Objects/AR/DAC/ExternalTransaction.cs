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
using PX.Objects.SO;
using PX.Objects.CS;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CC;

namespace PX.Objects.AR
{
	[PXCacheName("External Transaction")]
	public class ExternalTransaction : PXBqlTable, PX.Data.IBqlTable, IExternalTransaction
	{
		#region Keys
		public class PK : PrimaryKeyOf<ExternalTransaction>.By<transactionID>
		{
			public static ExternalTransaction Find(PXGraph graph, Int32? transactionID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, transactionID, options);
		}

		public static class FK
		{
			public class CustomerPaymentMethod : AR.CustomerPaymentMethod.PK.ForeignKeyOf<ExternalTransaction>.By<pMInstanceID> { }
			public class ARPayment : AR.ARPayment.PK.ForeignKeyOf<ExternalTransaction>.By<docType, refNbr> { }
			public class ProcessingCenter : CA.CCProcessingCenter.PK.ForeignKeyOf<ExternalTransaction>.By<processingCenterID> { }
			public class ParentExternalTransaction : AR.ExternalTransaction.PK.ForeignKeyOf<ExternalTransaction>.By<parentTranID> { }
			public class PayLinkID : CCPayLink.PK.ForeignKeyOf<ExternalTransaction>.By<payLinkID> { }
		}
		#endregion

		#region Selected
		public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Selected")]
		public bool? Selected
		{
			get;
			set;
		}
		#endregion

		#region TransactionID
		public abstract class transactionID : PX.Data.BQL.BqlInt.Field<transactionID> { }
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Ext. Tran. ID", Visibility = PXUIVisibility.SelectorVisible, Visible = false)]
		[PXSelector(typeof(Search<ExternalTransaction.transactionID>),
							typeof(ExternalTransaction.transactionID),
							typeof(ExternalTransaction.tranNumber), 
							typeof(ExternalTransaction.authNumber),
							typeof(ExternalTransaction.amount),
							typeof(ExternalTransaction.lastActivityDate), 
							typeof(ExternalTransaction.procStatus),
							typeof(ExternalTransaction.docType),
							typeof(ExternalTransaction.refNbr))]
		public virtual int? TransactionID { get;set; }
		#endregion

		#region PMInstanceID
		public abstract class pMInstanceID : PX.Data.BQL.BqlInt.Field<pMInstanceID> { }
		[PXDBInt]
		[PXUIField(DisplayName = "Card/Account No")]
		[PXSelector(typeof(Search<CustomerPaymentMethod.pMInstanceID,
			Where<CustomerPaymentMethod.isActive, Equal<boolTrue>>>), DescriptionField = typeof(CustomerPaymentMethod.descr), ValidateValue = false)]
		public virtual int? PMInstanceID { get; set; }
		#endregion

		#region PayLinkID
		public abstract class payLinkID : PX.Data.BQL.BqlInt.Field<payLinkID> { }
		/// <summary>
		/// Acumatica specific Payment Link Id.
		/// </summary>
		[PXDBInt]
		public virtual int? PayLinkID { get; set; }
		#endregion

		#region ProcessingCenterID
		public abstract class processingCenterID : PX.Data.BQL.BqlString.Field<processingCenterID> { }
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Proc. Center ID")]
		public virtual string ProcessingCenterID { get; set; }
		#endregion

		#region TerminalID
		public abstract class terminalID : PX.Data.BQL.BqlString.Field<terminalID> { }

		/// <summary>POS Terminal ID</summary>
		[PXDBString(36, IsUnicode = true)]
		[PXUIField(DisplayName = "POS Terminal ID")]
		public virtual string TerminalID
		{
			get;
			set;
		}
		#endregion

		#region DocType
		public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
		[PXDBString(3)]
		[PXUIField(DisplayName = "Doc. Type", Visibility = PXUIVisibility.SelectorVisible)]
		[ARDocType.List()]
		public virtual string DocType { get; set; }
		#endregion

		#region RefNbr
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Doc. Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<ARRegister.refNbr, Where<ARRegister.docType, Equal<Optional<ExternalTransaction.docType>>>>))]
		public virtual string RefNbr { get; set; }
		#endregion

		#region OrigDocType
		public abstract class origDocType : PX.Data.BQL.BqlString.Field<origDocType> { }
		[PXDBString(3)]
		[PXUIField(DisplayName = "Orig. Doc. Type")]
		[PXSelector(typeof(Search4<SOOrderType.orderType, Aggregate<GroupBy<SOOrderType.orderType>>>), DescriptionField = typeof(SOOrderType.descr))]
		public virtual string OrigDocType { get; set; }
		#endregion

		#region OrigRefNbr
		public abstract class origRefNbr : PX.Data.BQL.BqlString.Field<origRefNbr> { }
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Orig. Doc. Ref. Nbr.")]
		[PXSelector(typeof(Search<SOOrder.orderNbr, Where<SOOrder.orderType, Equal<Optional<ExternalTransaction.origDocType>>>>))]
		public virtual string OrigRefNbr { get; set; }
		#endregion

		#region VoidDocType
		public abstract class voidDocType : PX.Data.BQL.BqlString.Field<voidDocType> { }
		[PXDBString(3)]
		[ARDocType.List]
		public virtual string VoidDocType { get; set; }
		#endregion

		#region VoidRefNbr
		public abstract class voidRefNbr : PX.Data.BQL.BqlString.Field<voidRefNbr> { }
		[PXDBString(15, IsUnicode = true)]
		public virtual string VoidRefNbr { get; set; }
		#endregion

		#region TranNumber
		public abstract class tranNumber : PX.Data.BQL.BqlString.Field<tranNumber> { }
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Proc. Center Tran. Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string TranNumber { get; set; }
		#endregion

		#region AuthNumber
		public abstract class authNumber : PX.Data.BQL.BqlString.Field<authNumber> { }
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Proc. Center Auth. Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string AuthNumber { get; set; }
		#endregion

		#region Amount
		public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tran. Amount", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual decimal? Amount { get; set; }
		#endregion

		#region CardType
		public abstract class cardType : PX.Data.BQL.BqlString.Field<cardType> { }

		/// <summary>
		/// Type of a card associated with the document. 
		/// </summary>
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Card/Account Type", Enabled = false)]
		[CardType.List]
		public virtual string CardType
		{
			get;
			set;
		}
		#endregion
		#region ProcCenterCardTypeCode
		public abstract class procCenterCardTypeCode : PX.Data.BQL.BqlString.Field<procCenterCardTypeCode> { }

		/// <summary>
		/// Original card type value received from the processing center.
		/// </summary>
		[PXDBString(25, IsFixed = true)]
		[PXUIField(DisplayName = "Proc. Center Card Type", Enabled = false, Visible = false, Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string ProcCenterCardTypeCode
		{
			get;
			set;
		}
		#endregion

		#region ProcessingStatus
		public abstract class procStatus : PX.Data.BQL.BqlString.Field<procStatus> { }
		[PXDBString(3, IsFixed = true, DatabaseFieldName = "ProcessingStatus")]
		[ExtTransactionProcStatusCode.List()]
		[PXUIField(DisplayName = "Proc. Status", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string ProcStatus { get; set; }
		#endregion

		#region LastActivityDate
		public abstract class lastActivityDate : PX.Data.BQL.BqlDateTime.Field<lastActivityDate> { }
		[PXDBDate(PreserveTime = true, DisplayMask = "d")]
		[PXUIField(DisplayName = "Last Activity Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? LastActivityDate { get; set; }
		#endregion

		#region Direction
		public abstract class direction : PX.Data.BQL.BqlString.Field<direction> { }
		[PXDBString(1, IsFixed = true)]
		public virtual string Direction { get; set; }
		#endregion

		#region Active
		public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? Active { get; set; }
		#endregion

		#region SaveProfile
		public abstract class saveProfile : PX.Data.BQL.BqlBool.Field<saveProfile> { }
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Load Payment Profile")]
		public virtual bool? SaveProfile { get; set; }
		#endregion

		#region NeedSync
		public abstract class needSync : PX.Data.BQL.BqlBool.Field<needSync> { }
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Validation Is Required")]
		public virtual bool? NeedSync { get; set; }
		#endregion

		#region SyncStatus
		public abstract class syncStatus : PX.Data.BQL.BqlString.Field<syncStatus> { }
		[PXDBString(1, IsFixed = true)]
		[PXDefault(CCSyncStatusCode.None)]
		[CCSyncStatusCode.List]
		[PXUIField(DisplayName = "Validation Status")]
		public virtual string SyncStatus { get; set; }
		#endregion

		#region SyncMessage
		public abstract class syncMessage : PX.Data.BQL.BqlString.Field<syncMessage> { }
		[PXDBString(1024, IsUnicode = true)]
		public virtual string SyncMessage { get; set; }
		#endregion

		#region ExtProfileId
		public abstract class extProfileId : PX.Data.BQL.BqlString.Field<extProfileId> { }
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Ext. Profile ID")]
		public virtual string ExtProfileId { get; set; }
		#endregion

		#region Completed
		public abstract class completed : PX.Data.BQL.BqlBool.Field<completed> { }
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Completed")]
		public virtual bool? Completed { get; set; }
		#endregion

		#region ParentTranID
		public abstract class parentTranID : PX.Data.BQL.BqlInt.Field<parentTranID> { }
		[PXDBInt]
		public virtual int? ParentTranID { get; set; }
		#endregion

		#region ExpirationDate
		public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }
		[PXDBDate(PreserveTime = true, DisplayMask = "d")]
		[PXUIField(DisplayName = "Expiration Date")]
		public virtual DateTime? ExpirationDate { get; set; }
		#endregion

		#region CVVVerification
		public abstract class cVVVerification : PX.Data.BQL.BqlString.Field<cVVVerification> { }
		[PXDBString(3, IsFixed = true)]
		[PXDefault(CVVVerificationStatusCode.RequiredButNotVerified)]
		[CVVVerificationStatusCode.List()]
		[PXUIField(DisplayName = "CVV Verification")]
		public virtual string CVVVerification { get; set; }
		#endregion

		#region FundHoldExpDate

		public abstract class fundHoldExpDate : Data.BQL.BqlDateTime.Field<fundHoldExpDate> { }

		[PXDBDateAndTime]
		public virtual DateTime? FundHoldExpDate
		{
			get;
			set;
		}
		#endregion
		
		#region Settled
		public abstract class settled : PX.Data.BQL.BqlBool.Field<settled> { }
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Settled")]
		public virtual bool? Settled { get; set; }
		#endregion

		#region L3Status
		public abstract class l3Status : PX.Data.BQL.BqlString.Field<l3Status> { }
		/// <summary>
		/// Processing status of Level 3 Data.
		/// </summary>
		[PXDBString(3, IsFixed = true, DatabaseFieldName = "L3Status")]
		[ExtTransactionL3StatusCode.List]
		[PXDefault(typeof(ExtTransactionL3StatusCode.notApplicable), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Processing Status", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string L3Status { get; set; }
		#endregion

		#region L3Error
		public abstract class l3Error : PX.Data.BQL.BqlString.Field<l3Error> { }
		/// <summary>
		///	Processing error text of Level 3 Data.
		/// </summary>
		[PXDBString(255, IsUnicode = true, DatabaseFieldName = "L3Error")]
		[PXUIField(Visible = false, DisplayName = "Error Description")]
		public virtual string L3Error
		{
			get;
			set;
		}
		#endregion

		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
		[PXDBTimestamp()]
		public virtual byte[] tstamp { get; set; }
		#endregion
		
		#region NoteID
		public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }


		/// <summary>
		/// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the document.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
		/// </value>
		[PXNote()]
		public virtual Guid? NoteID
		{
			get;
			set;
		}
		#endregion

		public static class TransactionDirection
		{
			public const string Debet = "D";
			public const string Credit = "C";

			public class debetTransactionDirection : PX.Data.BQL.BqlString.Constant<debetTransactionDirection>
			{
				public debetTransactionDirection() : base(Debet) { }
			}
		}
	}
}
