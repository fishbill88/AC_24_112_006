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
	/// The settings for approval of SO Orders.
	/// </summary>
	[PXCacheName(Messages.SOSetupApproval)]
	public class SOSetupApproval : PXBqlTable, IBqlTable, IAssignedMap
	{
		#region Keys
		public class PK: PrimaryKeyOf<SOSetupApproval>.By<approvalID>
		{
			public static SOSetupApproval Find(PXGraph graph, int? approvalID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, approvalID, options);
		}
		public static class FK
		{
			public class OrderType : SOOrderType.PK.ForeignKeyOf<SOSetupApproval>.By<orderType> { }
			public class ApprovalMap : EPAssignmentMap.PK.ForeignKeyOf<SOSetupApproval>.By<assignmentMapID> { }
			public class PendingApprovalNotification : PX.SM.Notification.PK.ForeignKeyOf<SOSetupApproval>.By<assignmentNotificationID> { }
		}
		#endregion

		#region IsActive
		/// <summary>
		/// Specifies (if set to <c>true</c>) that the approval map is applied to orders of the <see cref="OrderType"/> type.
		/// </summary>
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? IsActive { get; set; }
		public abstract class isActive : BqlBool.Field<isActive> { }
		#endregion
		#region OrderType
		/// <summary>
		/// Specifies the order type to which the approval map is applied.
		/// </summary>
		[PXDBString(2, IsFixed = true, InputMask = ">aa")]
		[PXDefault]
		[PXFieldDescription]
		[ApprovableOrderTypeSelector]
		[PXUIField(DisplayName = "SO Type", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string OrderType { get; set; }
		public abstract class orderType : BqlString.Field<orderType> { }
		public class ApprovableOrderTypeSelectorAttribute : PXCustomSelectorAttribute
		{
			public ApprovableOrderTypeSelectorAttribute() : base(typeof(
				Search2<SOOrderType.orderType,
				InnerJoin<SOOrderTypeOperation, On<
					SOOrderTypeOperation.FK.OrderType.
					And<SOOrderTypeOperation.operation.IsEqual<SOOrderType.defaultOperation>>>>>
			))
			{ }

			protected virtual System.Collections.IEnumerable GetRecords()
			{
				var command = new
					SelectFrom<SOOrderType>.
					InnerJoin<SOOrderTypeOperation>.On<
						SOOrderTypeOperation.FK.OrderType.
						And<SOOrderTypeOperation.operation.IsEqual<SOOrderType.defaultOperation>>>.
					View(_Graph);

				if (PXAccess.FeatureInstalled<CS.FeaturesSet.warehouse>() == false)
					command.WhereAnd<Where<SOOrderTypeOperation.iNDocType.IsNotEqual<IN.INTranType.transfer>>>();

				return command.Select();
			}
		}
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
		/// Specifies the assignment map that will be used to walk an SO Order through the approval process.
		/// </summary>
		[PXDBInt]
		[PXDefault]
		[PXCheckUnique(typeof(orderType))]
		[PXSelector(typeof(
			Search<EPAssignmentMap.assignmentMapID, 
			Where<
				EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeSalesOrder>,
				And<EPAssignmentMap.mapType, NotEqual<EPMapType.assignment>>>>), 
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
		[PXDBTimestamp(VerifyTimestamp = VerifyTimestampOptions.BothFromGraphAndRecord)]
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
