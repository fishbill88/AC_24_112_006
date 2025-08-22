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
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;

using PX.Objects.EP;

namespace PX.Objects.SO
{
	/// <summary>
	/// The settings for approval of SO invoices.
	/// </summary>
	[PXCacheName(Messages.SOSetupInvoiceApproval)]
	public class SOSetupInvoiceApproval : PXBqlTable, IBqlTable, IAssignedMap
	{
		#region Keys
		/// <exclude/>
		public class PK : PrimaryKeyOf<SOSetupInvoiceApproval>.By<approvalID>
		{
			public static SOSetupInvoiceApproval Find(PXGraph graph, Int32? approvalID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, approvalID, options);
		}

		/// <exclude/>
		public static class FK
		{
			public class ApprovalMap : EPAssignmentMap.PK.ForeignKeyOf<SOSetupInvoiceApproval>.By<assignmentMapID> { }
			public class PendingApprovalNotification : PX.SM.Notification.PK.ForeignKeyOf<SOSetupInvoiceApproval>.By<assignmentNotificationID> { }
		}

		public class EPSettings : EPApprovalSettings<SOSetupInvoiceApproval, docType, AR.ARDocType, AR.ARDocStatus.hold, AR.ARDocStatus.pendingApproval, AR.ARDocStatus.rejected> { }
		#endregion

		#region IsActive
		/// <summary>
		/// Specifies (if set to <c>true</c>) that the approval map is applied to documents of the <see cref="DocType"/> type.
		/// </summary>
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? IsActive { get; set; }
		public abstract class isActive : BqlBool.Field<isActive> { }
		#endregion
		#region DocType
		/// <summary>
		/// Specifies the document type to which the approval map is applied.
		/// </summary>
		[PXDBString(3, IsFixed = true)]
		[PXDefault(AR.ARDocType.Invoice)]
		[AR.ARDocType.SOEntryList]
		[PXFieldDescription]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, TabOrder = 0)]
		public virtual string DocType { get; set; }
		public abstract class docType : BqlString.Field<docType> { }
		#endregion
		#region ApprovalID
		/// <summary>
		/// The surrogate identifier of the record.
		/// </summary>
		[PXDBIdentity(IsKey = true)]
		public virtual int? ApprovalID { get; set; }
		public abstract class approvalID : BqlInt.Field<approvalID> { }
		#endregion
		#region AssignmentMapID
		/// <summary>
		/// Specifies the assignment map that will be used to walk an SO Invoice through the approval process.
		/// </summary>
		[PXDBInt]
		[PXDefault]
		[PXCheckUnique(typeof(docType))]
		[PXSelector(typeof(
			SearchFor<EPAssignmentMap.assignmentMapID>.
			Where<
				EPAssignmentMap.entityType.IsEqual<FullNameOf<AR.ARInvoice>>.
				And<EPAssignmentMap.graphType.IsEqual<FullNameOf<SOInvoiceEntry>>>>),
			DescriptionField = typeof(EPAssignmentMap.name),
			SubstituteKey = typeof(EPAssignmentMap.name))]
		[PXUIField(DisplayName = "Approval Map")]
		public virtual int? AssignmentMapID { get; set; }
		public abstract class assignmentMapID : BqlInt.Field<assignmentMapID> { }
		#endregion
		#region AssignmentNotificationID
		/// <summary>
		/// Specifies the pending approval notification that will be send to an approver.
		/// </summary>
		[PXDBInt]
		[PXSelector(typeof(PX.SM.Notification.notificationID), DescriptionField = typeof(PX.SM.Notification.name))]
		[PXUIField(DisplayName = "Pending Approval Notification")]
		public virtual int? AssignmentNotificationID { get; set; }
		public abstract class assignmentNotificationID : BqlInt.Field<assignmentNotificationID> { }
		#endregion

		#region Audit Fields
		#region tstamp
		[PXDBTimestamp]
		public virtual byte[] tstamp { get; set; }
		public abstract class Tstamp : BqlByteArray.Field<Tstamp> { }
		#endregion
		#region CreatedByID
		[PXDBCreatedByID]
		public virtual Guid? CreatedByID { get; set; }
		public abstract class createdByID : BqlGuid.Field<createdByID> { }
		#endregion
		#region CreatedByScreenID
		[PXDBCreatedByScreenID]
		public virtual string CreatedByScreenID { get; set; }
		public abstract class createdByScreenID : BqlString.Field<createdByScreenID> { }
		#endregion
		#region CreatedDateTime
		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime { get; set; }
		public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
		#endregion
		#region LastModifiedByID
		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID { get; set; }
		public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
		#endregion
		#region LastModifiedByScreenID
		[PXDBLastModifiedByScreenID]
		public virtual string LastModifiedByScreenID { get; set; }
		public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }
		#endregion
		#region LastModifiedDateTime
		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
		#endregion
		#endregion
	}
}
