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
using PX.Data.BQL.Fluent;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.EP;
using System;

namespace PX.Objects.AR
{
	/// <summary>
	/// The settings for approval of AR documents.
	/// </summary>
	public class ARSetupApproval : PXBqlTable, IBqlTable, IAssignedMap
	{
		#region Keys
		/// <exclude/>
		public class PK : PrimaryKeyOf<ARSetupApproval>.By<approvalID>
		{
			public static ARSetupApproval Find(PXGraph graph, Int32? ApprovalID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, ApprovalID, options);
		}

		public class EPSetting : EPApprovalSettings<ARSetupApproval, docType, ARDocType, ARDocStatus.hold, ARDocStatus.pendingApproval, ARDocStatus.rejected> { }
		#endregion

		#region ApprovalID
		public abstract class approvalID : PX.Data.BQL.BqlInt.Field<approvalID> { }

		[PXDBIdentity(IsKey = true)]
		public virtual int? ApprovalID
		{
			get; set;
		}
		#endregion

		#region AssignmentMapID
		public abstract class assignmentMapID : PX.Data.BQL.BqlInt.Field<assignmentMapID> { }

		[PXDefault]
		[PXDBInt]
		[PXSelector(typeof(
			SearchFor<EPAssignmentMap.assignmentMapID>.
			Where<
				Brackets<
					docType.FromCurrent.IsEqual<ARDocType.refund>.
					And<EPAssignmentMap.entityType.IsEqual<FullNameOf<ARPayment>>>>.
				Or<
					docType.FromCurrent.IsEqual<ARDocType.cashReturn>.
					And<EPAssignmentMap.entityType.IsEqual<FullNameOf<Standalone.ARCashSale>>>>.
				Or<
					docType.FromCurrent.IsIn<ARDocType.invoice, ARDocType.prepaymentInvoice, ARDocType.creditMemo, ARDocType.debitMemo>.
					And<EPAssignmentMap.entityType.IsEqual<FullNameOf<ARInvoice>>>.
					And<EPAssignmentMap.graphType.IsNotEqual<FullNameOf<SO.SOInvoiceEntry>>>>>),
			DescriptionField = typeof(EPAssignmentMap.name),
			SubstituteKey = typeof(EPAssignmentMap.name))]
		[PXUIField(DisplayName = "Approval Map")]
		[PXCheckUnique(typeof(ARSetupApproval.docType))]
		public virtual int? AssignmentMapID
		{
			get; set;
		}
		#endregion

		#region AssignmentNotificationID
		public abstract class assignmentNotificationID : PX.Data.BQL.BqlInt.Field<assignmentNotificationID> { }

		[PXDBInt]
		[PXSelector(typeof(PX.SM.Notification.notificationID), DescriptionField = typeof(PX.SM.Notification.name))]
		[PXUIField(DisplayName = "Pending Approval Notification")]
		public virtual int? AssignmentNotificationID
		{
			get; set;
		}
		#endregion

		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

		[PXDBTimestamp()]
		public virtual byte[] tstamp
		{
			get; set;
		}
		#endregion

		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }

		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get; set;
		}
		#endregion

		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }

		[PXDBCreatedByScreenID()]
		public virtual string CreatedByScreenID
		{
			get; set;
		}
		#endregion

		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }

		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get; set;
		}
		#endregion

		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }

		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get; set;
		}
		#endregion

		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }

		[PXDBLastModifiedByScreenID()]
		public virtual string LastModifiedByScreenID
		{
			get; set;
		}
		#endregion

		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }

		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get; set;
		}
		#endregion

		#region IsActive
		public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }

		/// <summary>
		/// Specifies (if set to <c>true</c>) that the approval map is applied to documents of the <see cref="DocType"/> type.
		/// </summary>
		[PXDBBool]
		[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? IsActive
		{
			get; set;
		}
		#endregion

		#region DocType
		public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }

		/// <summary>
		/// Specifies the document type to which the approval map is applied.
		/// </summary>
		[PXDBString(3, IsFixed = true)]
		[PXDefault(ARDocType.Refund)]
		[ARDocType.ARApprovalDocTypeList]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, TabOrder = 0)]
		[PXFieldDescription]
		public virtual String DocType
		{
			get; set;
		} 
		#endregion
	}
}
