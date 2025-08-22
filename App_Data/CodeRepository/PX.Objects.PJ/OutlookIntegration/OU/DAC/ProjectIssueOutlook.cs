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
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.TM;
using PX.Objects.PJ.Common.Descriptor.Attributes;
using PX.Objects.PJ.ProjectsIssue.Descriptor;

// TODO : common fields for ProjectIssueOutlook and ProjectIssue should be moved to a base class
namespace PX.Objects.PJ.OutlookIntegration.OU.DAC
{
    [Serializable]
    [PXHidden]
    public class ProjectIssueOutlook : PXBqlTable, IBqlTable
    {
		#region Summary
		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.summary"/>
		public abstract class summary : BqlString.Field<summary> { }

		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.Summary"/>
		[PXDefault(typeof(OUMessage.subject), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Summary")]
        public virtual string Summary
        {
            get;
            set;
        }
		#endregion

		#region ProjectId
		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.projectId"/>
		public abstract class projectId : BqlInt.Field<projectId> { }

		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.ProjectId"/>
		[PXDefault]
        [Project(typeof(Where<PMProject.nonProject, Equal<False>,
            And<PMProject.baseType, Equal<CTPRType.project>>>), DisplayName = "Project")]
        public virtual int? ProjectId
        {
            get;
            set;
        }
		#endregion

		#region ClassId
		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.classId"/>
		public abstract class classId : BqlString.Field<classId> { }

		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.ClassId"/>
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXString(10, InputMask = ">aaaaaaaaaa", IsUnicode = true)]
        [ResetOutlookResponseDueDate(typeof(dueDate), typeof(creationDate))]
        [PXSelector(typeof(Search<ProjectManagementClass.projectManagementClassId,
                Where<ProjectManagementClass.useForProjectIssue, Equal<True>>>),
            typeof(ProjectManagementClass.projectManagementClassId),
            DescriptionField = typeof(ProjectManagementClass.description))]
        [ClassPriorityDefaulting(nameof(PriorityId))]
        [PXUIField(DisplayName = "Class ID", Required = true)]
        public virtual string ClassId
        {
            get;
            set;
        }
		#endregion

		#region PriorityId
		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.priorityId"/>
		public abstract class priorityId : BqlInt.Field<priorityId> { }

		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.PriorityId"/>
		[PXInt]
        [ProjectManagementPrioritySelector(typeof(classId))]
        [PXUIField(DisplayName = "Priority")]
        public virtual int? PriorityId
        {
            get;
            set;
        }
		#endregion

		#region OwnerID
		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.ownerID"/>
		public abstract class ownerID : BqlString.Field<ownerID> { }

		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.OwnerID"/>
		[Owner(IsDBField = false)]
        [PXDefault(typeof(AccessInfo.contactID), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual int? OwnerID
        {
            get;
            set;
        }
		#endregion

		#region DueDate
		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.dueDate"/>
		public abstract class dueDate : BqlDateTime.Field<dueDate> { }

		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.DueDate"/>
		[PXDBDate(PreserveTime = false, InputMask = "d")]
        [PXUIField(DisplayName = "Due Date")]
        public virtual DateTime? DueDate
        {
            get;
            set;
        }
		#endregion

		#region CreationDate
		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.creationDate"/>
		public abstract class creationDate : BqlDateTime.Field<creationDate> { }

		/// <inheritdoc cref="PX.Objects.PJ.ProjectsIssue.PJ.DAC.ProjectIssue.CreationDate"/>
		[PXDBDateAndTime(DisplayNameDate = "Created On")]
		[DefaultWorkingTimeStart]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Created On", Required = false)]
		public DateTime? CreationDate
		{
			get;
			set;
		}
		#endregion
	}
}
