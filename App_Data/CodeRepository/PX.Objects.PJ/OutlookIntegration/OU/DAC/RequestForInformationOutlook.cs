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
using PX.Objects.PJ.OutlookIntegration.OU.Descriptor.Attributes;
using PX.Objects.PJ.ProjectManagement.PJ.DAC;
using PX.Objects.PJ.ProjectManagement.PJ.Descriptor.Attributes;
using PX.Objects.PJ.RequestsForInformation.Descriptor;
using PX.Objects.PJ.RequestsForInformation.PJ.Descriptor.Attributes;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.TM;

// TODO : common fields for RequestForInformationOutlook and RequestForInformation should be moved to a base class
namespace PX.Objects.PJ.OutlookIntegration.OU.DAC
{
	[Serializable]
	[PXHidden]
	public class RequestForInformationOutlook : PXBqlTable, IBqlTable
	{
		#region Summary
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.summary"/>
		public abstract class summary : BqlString.Field<summary> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.Summary"/>
		[PXDefault(typeof(OUMessage.subject), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXString(255, IsUnicode = true)]
		[PXUIField(DisplayName = RequestForInformationLabels.Summary)]
		public virtual string Summary
		{
			get;
			set;
		}
		#endregion

		#region ClassId
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.classId"/>
		public abstract class classId : BqlString.Field<classId> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.ClassId"/>
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXString(10, InputMask = ">aaaaaaaaaa", IsUnicode = true)]
		[ResetOutlookResponseDueDate(typeof(dueResponseDate), typeof(creationDate))]
		[PXUIField(DisplayName = RequestForInformationLabels.ClassId, Required = true)]
		[PXSelector(typeof(Search<ProjectManagementClass.projectManagementClassId,
				Where<ProjectManagementClass.useForRequestForInformation, Equal<True>>>),
			typeof(ProjectManagementClass.projectManagementClassId),
			DescriptionField = typeof(ProjectManagementClass.description))]
		[ClassPriorityDefaulting(nameof(PriorityId))]
		public virtual string ClassId
		{
			get;
			set;
		}
		#endregion

		#region Incoming
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.incoming"/>
		public abstract class priorityId : BqlInt.Field<priorityId> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.Incoming"/>
		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Incoming")]
		public virtual bool? Incoming
		{
			get;
			set;
		}
		#endregion

		#region Status
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.status"/>
		public abstract class projectId : BqlInt.Field<projectId> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.Status"/>
		[PXString(1, IsFixed = true)]
		[RequestForInformationStatus]
		[PXDefault(RequestForInformationStatusAttribute.NewStatus, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = RequestForInformationLabels.Status, Required = true)]
		public virtual string Status
		{
			get;
			set;
		}
		#endregion

		#region PriorityId
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.priorityId"/>
		public abstract class incoming : BqlBool.Field<incoming> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.PriorityId"/>
		[PXInt]
		[ProjectManagementPrioritySelector(typeof(classId))]
		[PXUIField(DisplayName = "Priority")]
		public virtual int? PriorityId
		{
			get;
			set;
		}
		#endregion

		#region ProjectId
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.projectId"/>
		public abstract class status : BqlString.Field<status> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.ProjectId"/>
		[PXDefault]
		[Project(typeof(Where<PMProject.nonProject, Equal<False>, And<PMProject.baseType, Equal<CTPRType.project>>>),
			DisplayName = RequestForInformationLabels.ProjectId)]
		public virtual int? ProjectId
		{
			get;
			set;
		}
		#endregion

		#region ContactId
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.contactId"/>
		public abstract class contactId : BqlInt.Field<contactId> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.ContactId"/>
		[PXInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<Contact.contactID,
				Where<Contact.contactType,
					In3<ContactTypesAttribute.person, ContactTypesAttribute.employee>,
					And<Contact.isActive, Equal<True>>>>),
			SubstituteKey = typeof(Contact.displayName))]
		[PXRestrictor(typeof(Where<Contact.isActive, Equal<True>>),
			RequestForInformationMessages.OnlyActiveContactsAreAllowed)]
		[PXUIField(DisplayName = RequestForInformationLabels.ContactId, Required = true)]
		public virtual int? ContactId
		{
			get;
			set;
		}
		#endregion

		#region OwnerId
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.ownerId"/>
		public abstract class ownerID : BqlString.Field<ownerID> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.OwnerId"/>
		[Owner(IsDBField = false)]
		[PXDefault(typeof(AccessInfo.contactID), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual int? OwnerId
		{
			get;
			set;
		}
		#endregion

		#region DueResponseDate
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.dueResponseDate"/>
		public abstract class dueResponseDate : BqlDateTime.Field<dueResponseDate> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.DueResponseDate"/>
		[PXDBDate(PreserveTime = false, InputMask = "d")]
		[PXUIField(DisplayName = "Answer Date")]
		public virtual DateTime? DueResponseDate
		{
			get;
			set;
		}
		#endregion

		#region IsScheduleImpact
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.isScheduleImpact"/>
		public abstract class isScheduleImpact : BqlBool.Field<isScheduleImpact> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.IsScheduleImpact"/>
		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Schedule Impact (days)")]
		public virtual bool? IsScheduleImpact
		{
			get;
			set;
		}
		#endregion

		#region ScheduleImpact
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.scheduleImpact"/>
		public abstract class scheduleImpact : BqlInt.Field<scheduleImpact> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.ScheduleImpact"/>
		[PXInt]
		[PXUIField(DisplayName = "Schedule Impact (days)")]
		public virtual int? ScheduleImpact
		{
			get;
			set;
		}
		#endregion

		#region IsCostImpact
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.isCostImpact"/>
		public abstract class isCostImpact : BqlBool.Field<isCostImpact> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.IsCostImpact"/>
		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Cost Impact")]
		public virtual bool? IsCostImpact
		{
			get;
			set;
		}
		#endregion

		#region CostImpact
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.costImpact"/>
		public abstract class costImpact : BqlDecimal.Field<costImpact> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.CostImpact"/>
		[PXDecimal]
		[PXUIField(DisplayName = "Cost Impact")]
		public virtual decimal? CostImpact
		{
			get;
			set;
		}
		#endregion

		#region DesignChange
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.designChange"/>
		public abstract class designChange : BqlBool.Field<designChange> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.DesignChange"/>
		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Design Change")]
		public virtual bool? DesignChange
		{
			get;
			set;
		}
		#endregion

		#region CreationDate
		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.creationDate"/>
		public abstract class creationDate : BqlDateTime.Field<creationDate> { }

		/// <inheritdoc cref="PX.Objects.PJ.RequestsForInformation.PJ.DAC.RequestForInformation.CreationDate"/>
		[PXDBDate(PreserveTime = false, InputMask = "d")]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Creation Date")]
		public DateTime? CreationDate
		{
			get;
			set;
		}
		#endregion
	}
}
