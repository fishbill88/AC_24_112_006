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
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.EP;
using PX.TM;
using static PX.Objects.EP.DAC.EPRuleApprover;

namespace PX.Objects.EP.DAC
{
	/// <summary>
	/// Represent table which connects Approver (Employee) with Rule
	/// Records of this type are created and edited on Approval Maps (EP205015) form
	/// which corresponds to the <see cref="EPApprovalMapMaint"/>
	/// </summary>
	[Serializable]
	[PXCacheName(Messages.RuleApprover)]
	public partial class EPRuleApprover : PXBqlTable, IBqlTable
	{
		#region keys
		/// <summary>
		/// Foreign Keys
		/// </summary>
		public static class FK
		{
			/// <summary>
			/// Contact
			/// </summary>
			public class Contact : CR.Contact.PK.ForeignKeyOf<EPRuleApprover>.By<ownerID> { }

			/// <summary>
			/// Rule
			/// </summary>
			public class Rule : EPRule.PK.ForeignKeyOf<EPRuleApprover>.By<ownerID> { }
		}

		#endregion

		#region RuleApproverID

		public abstract class ruleApproverID : Data.BQL.BqlInt.Field<ruleApproverID> { }

		/// <summary>
		/// ID for each row of RuleApprover entity
		/// </summary>
		[PXDBIdentity(IsKey = true)]
		public virtual int? RuleApproverID { get; set; }

		#endregion

		#region OwnerID

		public abstract class ownerID : Data.BQL.BqlInt.Field<ownerID> { }

		/// <summary>
		/// Represents ID of Approver
		/// </summary>
		[PXCheckUnique(typeof(EPRuleApprover.ruleID), typeof(EPRuleApprover.ownerID))]
		[Owner(null, null,
			headerList: new[] {
				"Employee Name",
				"Job Title",
				"Email",
				"Phone 1",
				"Department",
				"Employee ID",
				"Status",
				"Branch",
				"Reports To",
				"Type",
				"User ID"
			}, DisplayName = "Employee Name")]
		public virtual int? OwnerID { get; set; }

		#endregion

		#region RuleID

		public abstract class ruleID : Data.BQL.BqlGuid.Field<ruleID> { }

		/// <summary>
		/// Represents ID of Rule for which is Approver defined
		/// </summary>
		[PXDBGuid]
		[PXUIField(DisplayName = "Rule ID")]
		[PXParent(typeof(Select<EPRule, Where<EPRule.ruleID, Equal<Current<ruleID>>>>))]
		[PXDBDefault(typeof(EPRule.ruleID))]
		public virtual Guid? RuleID { get; set; }

		#endregion

		#region CreatedByScreenID

		public abstract class createdByScreenID : Data.BQL.BqlString.Field<createdByScreenID> { }

		[PXDBCreatedByScreenID]
		public virtual string CreatedByScreenID { get; set; }

		#endregion

		#region CreatedByID

		public abstract class createdByID : Data.BQL.BqlGuid.Field<createdByID> { }

		[PXDBCreatedByID]
		public virtual Guid? CreatedByID { get; set; }

		#endregion

		#region CreatedDateTime

		public abstract class createdDateTime : Data.BQL.BqlDateTime.Field<createdDateTime> { }

		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime { get; set; }

		#endregion

		#region LastModifiedByID

		public abstract class lastModifiedByID : Data.BQL.BqlGuid.Field<lastModifiedByID> { }

		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID { get; set; }

		#endregion

		#region LastModifiedByScreenID

		public abstract class lastModifiedByScreenID : Data.BQL.BqlString.Field<lastModifiedByScreenID> { }

		[PXDBLastModifiedByScreenID()]
		public virtual string LastModifiedByScreenID { get; set; }

		#endregion

		#region LastModifiedDateTime

		public abstract class lastModifiedDateTime : Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }

		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime { get; set; }

		#endregion

		#region tstamp

		public abstract class Tstamp : Data.BQL.BqlByteArray.Field<Tstamp> { }

		[PXDBTimestamp]
		public virtual byte[] tstamp { get; set; }

		#endregion
	}
}
