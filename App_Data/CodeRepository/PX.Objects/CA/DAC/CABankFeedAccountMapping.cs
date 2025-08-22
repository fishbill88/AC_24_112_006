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
using PX.Data.ReferentialIntegrity.Attributes;
using System;

namespace PX.Objects.CA
{
	/// <summary>
	/// Defines the mapping between the bank feed account and the cash account.
	/// </summary>
	[PXCacheName("Bank Feed Account Mapping")]
	public class CABankFeedAccountMapping : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<CABankFeedAccountMapping>.By<bankFeedAccountMapID>
		{
			public static CABankFeedAccountMapping Find(PXGraph graph, Guid? bankFeedAccountMapID) => FindBy(graph, bankFeedAccountMapID);
		}

		public static class FK
		{
			public class BankFeed : CABankFeed.PK.ForeignKeyOf<CABankFeedAccountMapping>.By<bankFeedID> { }
			public class BankFeedDetail : CABankFeedDetail.PK.ForeignKeyOf<CABankFeedAccountMapping>.By<bankFeedID, lineNbr> { }
			public class CashAccount : CA.CashAccount.PK.ForeignKeyOf<CABankFeedAccountMapping>.By<cashAccountID> { }

		}
		#endregion

		#region BankFeedAccountMapID
		public abstract class bankFeedAccountMapID : PX.Data.BQL.BqlGuid.Field<bankFeedAccountMapID> { }
		/// <summary>
		/// The unique identifier of the record that represents the Acumatica specific bank feed account map id.
		/// </summary>
		[PXDBGuid(withDefaulting: true, IsKey = true)]
		public virtual Guid? BankFeedAccountMapID { get; set; }
		#endregion
		#region BankFeedID
		public abstract class bankFeedID : PX.Data.BQL.BqlString.Field<bankFeedID> { }
		/// <summary>
		/// The Bank feed id from the bank feed definition.
		/// </summary>
		[PXDBString(10, IsUnicode = true)]
		public virtual string BankFeedID { get; set; }
		#endregion

		#region LineNbr
		public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
		/// <summary>
		/// The line number from the bank feed detail.
		/// </summary>
		[PXDBInt]
		public virtual int? LineNbr { get; set; }
		#endregion

		#region BankFeedType
		public abstract class type : PX.Data.BQL.BqlString.Field<type> { }
		/// <summary>
		/// Bank Feed type (P - Plaid, M - MX, T - Test Plaid).
		/// </summary>
		[PXDBString]
		[PXDefault]
		[CABankFeedType.List]
		[PXUIField(DisplayName = "Bank Feed Type")]
		public virtual string Type { get; set; }
		#endregion

		#region InstitutionID
		public abstract class institutionID : PX.Data.BQL.BqlString.Field<institutionID> { }
		/// <summary>
		/// The bank feed specific bank identifier.
		/// </summary>
		[PXDBString(50, IsUnicode = true)]
		public virtual string InstitutionID { get; set; }
		#endregion

		#region AccountName
		public abstract class accountName : PX.Data.BQL.BqlString.Field<accountName> { }
		/// <summary>
		/// The bank feed specific account name.
		/// </summary>
		[PXDBString(250)]
		public virtual string AccountName { get; set; }
		#endregion

		#region Account Mask
		public abstract class accountMask : PX.Data.BQL.BqlString.Field<accountMask> { }
		/// <summary>
		/// The bank feed specific account mask.
		/// </summary>
		[PXDBString(50)]
		public virtual string AccountMask { get; set; }
		#endregion

		#region AccountNameMask
		public abstract class accountNameMask : PX.Data.BQL.BqlString.Field<accountNameMask> { }
		/// <summary>
		/// The combination of bank feed specific account name and bank feed specific account mask.
		/// </summary>
		[PXString]
		[PXDBCalced(typeof(accountName.Concat<Space>.Concat<accountMask>), typeof(string))]
		public string AccountNameMask
		{
			get
			;
			set;
		}
		#endregion

		#region CashAccountID
		public abstract class cashAccountID : PX.Data.BQL.BqlInt.Field<cashAccountID> { }
		/// <summary>
		/// The Acumatica specific cash account id which is linked to a bank feed account.
		/// </summary>
		[PXDBInt]
		[PXSelector(typeof(Search<CashAccount.cashAccountID>), SubstituteKey = typeof(CashAccount.cashAccountCD),
			DescriptionField = typeof(CashAccount.descr))]
		public virtual int? CashAccountID { get; set; }
		#endregion

		#region CreatedByID
		public abstract class createdByID : Data.BQL.BqlGuid.Field<createdByID> { }
		/// <exclude/>
		[PXDBCreatedByID]
		public virtual Guid? CreatedByID { get; set; }
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : Data.BQL.BqlString.Field<createdByScreenID> { }
		/// <exclude/>
		[PXDBCreatedByScreenID]
		public virtual string CreatedByScreenID { get; set; }
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : Data.BQL.BqlDateTime.Field<createdDateTime> { }
		/// <exclude/>
		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : Data.BQL.BqlGuid.Field<lastModifiedByID> { }
		/// <exclude/>
		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID { get; set; }
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
		/// <exclude/>
		[PXDBLastModifiedByScreenID]
		public virtual string LastModifiedByScreenID { get; set; }
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		/// <exclude/>
		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		#endregion

		#region tstamp
		public abstract class Tstamp : Data.BQL.BqlByteArray.Field<Tstamp> { }
		/// <exclude/>
		[PXDBTimestamp]
		public virtual byte[] tstamp { get; set; }
		#endregion
	}
}
