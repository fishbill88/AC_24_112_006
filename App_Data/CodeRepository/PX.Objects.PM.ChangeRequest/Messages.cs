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

using PX.Common;

namespace PX.Objects.PM.ChangeRequest
{
	[PXLocalizable]
	public static class Messages
	{
		public const string ChangeRequest = "Change Request";
		public const string ChangeRequestLine = "Change Request Line";
		public const string Markup = "Markup";
		public const string ViewChangeRequest = "View Change Request";
				
		public const string CannotDeleteLinkedCR = "The change request cannot be deleted from the change order until this change request is deleted from the {0} revenue change order.";
		public const string RevenueTaskNotSpecified = "The revenue task is not specified.";
		public const string RevenueNotSpecified = "The revenue account group is not specified.";
		public const string RevenueTaskNotSpecifiedOnMarkup = "The revenue task is not specified for the change request on the Markups tab of the Change Requests (PM308500) form.";
		public const string RevenueNotSpecifiedOnMarkup = "The revenue account group is not specified for the change request on the Markups tab of the Change Requests (PM308500) form.";
		public const string ReferencedByCR = "The line cannot be deleted because it is associated with a change request.";
		public const string RemoveRequests = "To delete the change order, please remove all the change requests on the Change Requests tab first.";
        public const string InvalidDate = "The financial period for the {0} change date is not defined in the system. The change date, which is used for balance calculation, must belong to an existing financial period of the master calendar.";
		public const string InvlaidClass = "The default change order class specified on the Project Preferences (PM101000) form does not support 2-tier change management.";
		public const string ChangeRequestCannotBeAdded_Task = "The change request cannot be added to a change order because not all the project budget key attributes are specified on the Estimation tab (Revenue Task, Revenue Account Group) and the Markups tab (Account Group, Project Task) of the change request.";
		public const string ChangeRequestCannotBeAdded_TaskItem = "The change request cannot be added to a change order because not all the project budget key attributes are specified on the Estimation tab (Revenue Task, Revenue Account Group, Revenue Item) and the Markups tab (Account Group, Project Task, Inventory ID) of the change request.";
		public const string ChangeRequestCannotBeAdded_TaskCostCode = "The change request cannot be added to a change order because not all the project budget key attributes are specified on the Estimation tab (Revenue Task, Revenue Account Group, Revenue Code) and the Markups tab (Account Group, Project Task, Cost Code) of the change request.";
		public const string CreateCRProjectStatusError = "A change request cannot be created for the project with the {0} status.";
	}
}

