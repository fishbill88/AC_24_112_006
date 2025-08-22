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
using PX.Objects.CS;
using PX.Objects.PJ.ProjectAccounting.PM.Services;
using PX.Objects.PJ.RequestsForInformation.PM.DAC;
using PX.Objects.PM;

namespace PX.Objects.PJ.RequestsForInformation.PM.GraphExtensions
{
	public class ProjectEntryExt : PXGraphExtension<ProjectEntry>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.constructionProjectManagement>();

		public PXSelect<ProjectContact,
			Where<ProjectContact.projectId, Equal<Current<PMProject.contractID>>>> ProjectContacts;

		public virtual void _(Events.FieldUpdated<ProjectContact.contactId> args)
		{
			if (args.Row is ProjectContact projectContact)
			{
				args.Cache.SetDefaultExt<ProjectContact.email>(projectContact);
				args.Cache.SetDefaultExt<ProjectContact.phone>(projectContact);
			}
		}

		protected virtual void _(Events.RowPersisting<PMTask> args)
		{
			var projectTask = args.Row;
			if (projectTask != null)
			{
				var projectTaskTypeUsageService = new ProjectTaskTypeUsageInProjectManagementValidationService();
				projectTaskTypeUsageService.ValidateProjectTaskType(args.Cache, projectTask);
			}
		}
	}
}
