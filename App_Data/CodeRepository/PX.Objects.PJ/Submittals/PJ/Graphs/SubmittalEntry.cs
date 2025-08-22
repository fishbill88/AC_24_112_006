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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CN.Common.Extensions;
using PX.Objects.Common.Labels;
using PX.Objects.CR;
using PX.Objects.PJ.Common.Actions;
using PX.Objects.PJ.Common.DAC;
using PX.Objects.PJ.Common.Descriptor;
using PX.Objects.PJ.ProjectManagement.Descriptor;
using PX.Objects.PJ.ProjectManagement.PJ.DAC;
using PX.Objects.PJ.Submittals.PJ.DAC;
using PX.Objects.PJ.Submittals.PJ.Descriptor;
using PX.Objects.PJ.Submittals.PJ.Services;
using PX.Objects.PM;

using StatusDefinition = PX.Objects.PJ.Submittals.PJ.DAC.PJSubmittal.status;

namespace PX.Objects.PJ.Submittals.PJ.Graphs
{
	public partial class SubmittalEntry : PXGraph<SubmittalEntry>
	{
		[PXCopyPasteHiddenFields(
			typeof(PJSubmittal.status),
			typeof(PJSubmittal.reason),
			typeof(PJSubmittal.submittalID),
			typeof(PJSubmittal.revisionID),
			typeof(PJSubmittal.dateCreated),
			typeof(PJSubmittal.ownerID),
			typeof(PJSubmittal.currentWorkflowItemContactID),
			typeof(PJSubmittal.currentWorkflowItemLineNbr),
			typeof(PJSubmittal.dateClosed),
			typeof(PJSubmittal.isLastRevision))]
		public SelectFrom<PJSubmittal>
			.LeftJoin<PMProject>
				.On<PMProject.contractID.IsEqual<PJSubmittal.projectId>>
			.Where<PMProject.contractID.IsNull.Or<MatchUserFor<PMProject>>>
			.OrderBy<PJSubmittal.submittalID.Asc, PJSubmittal.revisionID.Desc>
			.View Submittals;

		[PXCopyPasteHiddenFields(
			typeof(PJSubmittal.status),
			typeof(PJSubmittal.reason),
			typeof(PJSubmittal.submittalID),
			typeof(PJSubmittal.revisionID),
			typeof(PJSubmittal.dateCreated),
			typeof(PJSubmittal.ownerID),
			typeof(PJSubmittal.currentWorkflowItemContactID),
			typeof(PJSubmittal.currentWorkflowItemLineNbr),
			typeof(PJSubmittal.dateClosed),
			typeof(PJSubmittal.isLastRevision))]
		public PXSelect<PJSubmittal,
			Where<PJSubmittal.submittalID, Equal<Current2<PJSubmittal.submittalID>>,
				And<PJSubmittal.revisionID, Equal<Current2<PJSubmittal.revisionID>>>>> CurrentSubmittal;

		[PXCopyPasteHiddenFields(
			typeof(PJSubmittalWorkflowItem.status),
			typeof(PJSubmittalWorkflowItem.completionDate),
			typeof(PJSubmittalWorkflowItem.dueDate),
			typeof(PJSubmittalWorkflowItem.dateReceived),
			typeof(PJSubmittalWorkflowItem.dateSent),
			typeof(PJSubmittalWorkflowItem.startDate))]
		public PXSelectJoin<PJSubmittalWorkflowItem,
			LeftJoin<Contact,
				On<Contact.contactID, Equal<PJSubmittalWorkflowItem.contactID>>>,
			Where
				<PJSubmittalWorkflowItem.submittalID, Equal<Current<PJSubmittal.submittalID>>, And
				<PJSubmittalWorkflowItem.revisionID, Equal<Current<PJSubmittal.revisionID>>>>>
			SubmittalWorkflowItems;

		[PXHidden]
		[PXCheckCurrent]
		public PXSetup<ProjectManagementSetup> PJSetup;

		public PXFilter<CurrentProject> CurrentProject;

		public PXSave<PJSubmittal> Save;
		public CancelAction<PJSubmittal> Cancel;
		public PXInsertNoKeysFromUI<PJSubmittal> Insert;
		public PXDeleteNoKeysFromUI<PJSubmittal> Delete;
		public PXCopyPasteAction<PJSubmittal> CopyPaste;
		public PXFirstNoKeysFromUI<PJSubmittal> First;
		public PXPreviousNoKeysFromUI<PJSubmittal> Previous;
		public PXNextNoKeysFromUI<PJSubmittal> Next;
		public PXLastNoKeysFromUI<PJSubmittal> Last;

		public PXAction<PJSubmittal> SendEmail;
		public PXAction<PJSubmittal> DeleteWorkflowItem;

		public PXAction<PJSubmittal> PrintSubmittal;

		public SubmittalEntry()
		{
			PXUIFieldAttribute.SetDisplayName<Contact.phone1>(Caches[typeof(Contact)], ProjectManagementMessages.ContactPhone);
			PXUIFieldAttribute.SetDisplayName<PJSubmittalWorkflowItem.emailTo>(this.Caches<PJSubmittalWorkflowItem>(), ProjectManagementMessages.EmailTo);
		}

		#region Actions

		[PXUIField(DisplayName = "Email", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXButton(OnClosingPopup = PXSpecialButtonType.Refresh, CommitChanges = true)]
		protected virtual void sendEmail()
		{
			Actions.PressSave();

			var emailActivityService = new SubmittalEmailService(this);
			var graph = emailActivityService.GetEmailActivityGraph();

			if (graph is CREmailActivityMaint emailActivityGraph && emailActivityGraph.Message.Current != null)
			{
				new SubmittalEmailAttachService(emailActivityGraph)
				.AttachAllFilesLinkedToRelatedEntities(emailActivityGraph.Message.Cache, emailActivityGraph.Message.Current);
			}

			throw new PXRedirectRequiredException(graph, true, string.Empty)
			{
				RepaintControls = true,
				Mode = PXBaseRedirectException.WindowMode.NewWindow
			};
		}

		[PXButton(Tooltip = ActionsMessages.Delete)]
		[PXUIField(DisplayName = ActionsMessages.Delete,
			MapEnableRights = PXCacheRights.Delete,
			MapViewRights = PXCacheRights.Delete)]
		protected virtual IEnumerable deleteWorkflowItem(PXAdapter adapter)
		{
			PJSubmittalWorkflowItem item = SubmittalWorkflowItems.Current;

			if (item != null)
			{
				SubmittalWorkflowItems.Delete(item);
			}

			return adapter.Get();
		}

		public PXAction<PJSubmittal> CreateRevision;

		[PXButton]
		[PXUIField(DisplayName = "Create Revision")]
		protected virtual void createRevision()
		{
			Actions.PressSave();

			PJSubmittal currentSubmittal = Submittals.Current;
			PJSubmittal newSubmittal = (PJSubmittal)Submittals.Cache.CreateCopy(currentSubmittal);

			newSubmittal.RevisionID++;
			newSubmittal.IsLastRevision = true;
			newSubmittal.NoteID = null;
			newSubmittal.DateCreated = null;
			newSubmittal.DateClosed = null;

			var itemsCache = SubmittalWorkflowItems.Cache;

			var newItems = SubmittalWorkflowItems
				.Select()
				.FirstTableItems
				.Select(item =>
				{
					var copy = (PJSubmittalWorkflowItem)itemsCache.CreateCopy(item);
					copy.SubmittalID = null;
					copy.LineNbr = null;
					copy.DateSent = null;
					copy.DateReceived = null;
					copy.CompletionDate = null;
					copy.DueDate = null;
					copy.StartDate = null;
					copy.NoteID = null;
					copy.Status = PJSubmittalWorkflowItem.status.Planned;
					copy.RevisionID = newSubmittal.RevisionID;

					return copy;
				})
				.ToList();

			SubmittalEntry newSubmittalEntry = PXGraph.CreateInstance<SubmittalEntry>();
			newSubmittalEntry.Clear();

			PM.SubmittalAutoNumberAttribute.DisableAutonumbiring<PJSubmittal.submittalID>(newSubmittalEntry.Submittals.Cache);

			newSubmittal = newSubmittalEntry.Submittals.Insert(newSubmittal);
			newSubmittal.Reason = PJSubmittal.reason.Revision;

			foreach (var newItem in newItems)
			{
				newSubmittalEntry.SubmittalWorkflowItems.Insert(newItem);
			}

			currentSubmittal.IsLastRevision = false;
			Submittals.Update(currentSubmittal);
			using (var ts = new PXTransactionScope())
			{
				Actions.PressSave();
				newSubmittalEntry.SelectTimeStamp();
				newSubmittalEntry.Actions.PressSave();

				ts.Complete();
			}

			PXRedirectHelper.TryRedirect(newSubmittalEntry, PXRedirectHelper.WindowMode.Same);
		}

		[PXButton]
		[PXUIField(DisplayName = "Print")]
		protected virtual void printSubmittal()
		{
			Actions.PressSave();

			var parameters = GetReportParameters(Submittals.Current);
            throw new PXReportRequiredException(parameters, ScreenIds.SubmittalReport, null);
		}

		#endregion

		#region Event handlers
		protected virtual void _(Events.FieldDefaulting<PJSubmittal, PJSubmittal.currentWorkflowItemContactID> e)
		{
			e.NewValue = e.Row?.OwnerID;
		}

		protected virtual void _(Events.FieldUpdated<PJSubmittal, PJSubmittal.ownerID> e)
		{
			UpdateCurrentContact(e.Row);
		}

		protected virtual void _(Events.FieldUpdated<PJSubmittal, PJSubmittal.status> e)
		{
			UpdateCurrentContact(e.Row);
		}

		protected virtual void _(Events.FieldUpdated<PJSubmittalWorkflowItem.contactID> e)
		{
			UpdateCurrentContact(Submittals?.Current);
		}

		protected virtual void _(Events.FieldUpdated<PJSubmittalWorkflowItem.status> e)
		{
			UpdateCurrentContact(Submittals?.Current);
		}

		protected virtual void _(Events.RowInserted<PJSubmittalWorkflowItem> e)
		{
			UpdateCurrentContact(Submittals?.Current);
		}

		protected virtual void _(Events.RowSelected<PJSubmittal> e)
		{
			PJSubmittal submittal = e.Row;
			if (submittal != null)
			{
				CurrentProject.Current.ProjectID = submittal.ProjectId;

				PXCache cache = e.Cache;
				string status = submittal.Status;
				bool isNewStatus = status == StatusDefinition.New;
				bool isClosedStatus = status == StatusDefinition.Closed;

				cache
					.Adjust<PXUIFieldAttribute>(submittal)
					.For<PJSubmittal.description>(fieldAttribute => fieldAttribute.Enabled = isNewStatus)
					.SameFor<PJSubmittal.summary>()
					.SameFor<PJSubmittal.dateCreated>();

				cache
					.Adjust<PXUIFieldAttribute>(submittal)
					.For<PJSubmittal.reason>(fieldAttribute => fieldAttribute.Enabled = !isClosedStatus)
					.SameFor<PJSubmittal.typeID>()
					.SameFor<PJSubmittal.projectId>()
					.SameFor<PJSubmittal.projectTaskId>()
					.SameFor<PJSubmittal.costCodeID>()
					.SameFor<PJSubmittal.specificationSection>()
					.SameFor<PJSubmittal.specificationInfo>()
					.SameFor<PJSubmittal.dueDate>()
					.SameFor<PJSubmittal.dateOnSite>()
					.SameFor<PJSubmittal.ownerID>();

				SubmittalWorkflowItems.AllowInsert = !isClosedStatus;

				if (submittal.IsLastRevision != true)
				{
					cache.RaiseException<PJSubmittal.revisionID>(
						submittal, 
						SubmittalMessage.OldRevisionWarning, 
						submittal.RevisionID, 
						PXErrorLevel.Warning);
				}
			}
		}

		protected virtual void _(Events.RowSelected<PJSubmittalWorkflowItem> e)
		{
			PJSubmittalWorkflowItem item = e.Row;

			if (item == null) return;

			PXCache cache = e.Cache;
			string status = Submittals?.Current?.Status;
			bool isNewStatus = status == StatusDefinition.New;
			bool isClosedStatus = status == StatusDefinition.Closed;
			bool isRoleReviewer = item?.Role == PJSubmittalWorkflowItem.role.Reviewer;
			bool isInsertedLine = cache.GetStatus(item) == PXEntryStatus.Inserted;

			bool isFieldsEnabled = isNewStatus || isInsertedLine;

			cache
				.Adjust<PXUIFieldAttribute>(item)
				.For<PJSubmittalWorkflowItem.role>(fieldAttribute => fieldAttribute.Enabled = isFieldsEnabled)
				.SameFor<PJSubmittalWorkflowItem.contactID>();

			isFieldsEnabled = !isClosedStatus || isRoleReviewer || isInsertedLine;

			cache
				.Adjust<PXUIFieldAttribute>(item)
				.For<PJSubmittalWorkflowItem.completionDate>(fieldAttribue => fieldAttribue.Enabled = isFieldsEnabled)
				.SameFor<PJSubmittalWorkflowItem.completionDate>()
				.SameFor<PJSubmittalWorkflowItem.dueDate>()
				.SameFor<PJSubmittalWorkflowItem.dateReceived>()
				.SameFor<PJSubmittalWorkflowItem.dateSent>()
				.SameFor<PJSubmittalWorkflowItem.startDate>()
				.SameFor<PJSubmittalWorkflowItem.status>()
				.SameFor<PJSubmittalWorkflowItem.daysForReview>();

			item.CanDelete = isNewStatus || isInsertedLine;

			if(IsMobile && item.CanDelete != true)
			{
				DeleteWorkflowItem.SetEnabled(false);
			}

			if (item.Role == PJSubmittalWorkflowItem.role.Approver)
			{
				PXStringListAttributeHelper.SetList<PJSubmittalWorkflowItem.status>(cache, item, new PJSubmittalWorkflowItem.status.ApproverLabels());
			}
			else if (item.Role == PJSubmittalWorkflowItem.role.Submitter || item.Role == PJSubmittalWorkflowItem.role.Reviewer)
			{
				PXStringListAttributeHelper.SetList<PJSubmittalWorkflowItem.status>(cache, item, new PJSubmittalWorkflowItem.status.SubmitterReviewerLabels());
			}
		}

		protected virtual void _(Events.FieldUpdated<PJSubmittalWorkflowItem, PJSubmittalWorkflowItem.status> e)
		{
			if (e.Row != null)
			{
				if (e.Row.Status == PJSubmittalWorkflowItem.status.Planned)
				{
					e.Cache.SetValueExt<PJSubmittalWorkflowItem.startDate>(e.Row, null);
					e.Cache.SetValue<PJSubmittalWorkflowItem.completionDate>(e.Row, null);
				}

				if (e.Row.Status == PJSubmittalWorkflowItem.status.Pending)
				{
					if ((string)e.OldValue == PJSubmittalWorkflowItem.status.Planned)
					{
						e.Cache.SetValueExt<PJSubmittalWorkflowItem.startDate>(e.Row, Accessinfo.BusinessDate);
					}
					if ((string)e.OldValue == PJSubmittalWorkflowItem.status.Canceled)
					{
						e.Cache.SetValueExt<PJSubmittalWorkflowItem.startDate>(e.Row, Accessinfo.BusinessDate);
						e.Cache.SetValue<PJSubmittalWorkflowItem.completionDate>(e.Row, null);
					}
					if ((string)e.OldValue == PJSubmittalWorkflowItem.status.Completed || (string)e.OldValue == PJSubmittalWorkflowItem.status.Approved || (string)e.OldValue == PJSubmittalWorkflowItem.status.Rejected)
					{
						e.Cache.SetValue<PJSubmittalWorkflowItem.completionDate>(e.Row, null);
					}
				}

				if (e.Row.Status == PJSubmittalWorkflowItem.status.Completed || e.Row.Status == PJSubmittalWorkflowItem.status.Approved || e.Row.Status == PJSubmittalWorkflowItem.status.Rejected || e.Row.Status == PJSubmittalWorkflowItem.status.Canceled)
				{
					e.Cache.SetValue<PJSubmittalWorkflowItem.completionDate>(e.Row, Accessinfo.BusinessDate);
				}
			}
		}

		protected virtual void PJSubmittalWorkflowItem_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			PJSubmittalWorkflowItem item = (PJSubmittalWorkflowItem)e.Row;
			PJSubmittalWorkflowItem oldItem = (PJSubmittalWorkflowItem)e.OldRow;

			if (item == null) return;

			if (!sender.ObjectsEqual<PJSubmittalWorkflowItem.startDate, PJSubmittalWorkflowItem.daysForReview>(e.Row, e.OldRow))
			{
				if (item.StartDate != null && item.DaysForReview != null)
				{
					item.DueDate = item.StartDate.Value.AddDays((double)item.DaysForReview);
				}
			}

			if (!sender.ObjectsEqual<PJSubmittalWorkflowItem.dueDate>(e.Row, e.OldRow))
			{
				if (item.StartDate != null)
				{
					if (item.DueDate > item.StartDate || item.DueDate == item.StartDate)
					{
						item.DaysForReview = item.DueDate.Value.Date.Subtract(item.StartDate.Value.Date).Days;
					}
					else
					{
						item.DaysForReview = null;
					}
				}
			}

			if (!sender.ObjectsEqual<PJSubmittalWorkflowItem.role>(e.Row, e.OldRow))
			{
				if (oldItem.Role == PJSubmittalWorkflowItem.role.Approver && (oldItem.Status == PJSubmittalWorkflowItem.status.Approved || oldItem.Status == PJSubmittalWorkflowItem.status.Rejected))
				{
					if (item.Role == PJSubmittalWorkflowItem.role.Submitter || item.Role == PJSubmittalWorkflowItem.role.Reviewer)
					{
						item.Status = PJSubmittalWorkflowItem.status.Completed;
					}
				}
				else if ((oldItem.Role == PJSubmittalWorkflowItem.role.Submitter || oldItem.Role == PJSubmittalWorkflowItem.role.Reviewer) && oldItem.Status == PJSubmittalWorkflowItem.status.Completed)
				{
					if (item.Role == PJSubmittalWorkflowItem.role.Approver)
					{
						item.Status = PJSubmittalWorkflowItem.status.Approved;
					}
				}
			}
		}

		protected virtual void _(Events.FieldVerifying<PJSubmittalWorkflowItem, PJSubmittalWorkflowItem.daysForReview> e)
		{
			if (e.NewValue == null) return;

			if ((int)e.NewValue < 0)
			{
				throw new PXSetPropertyException<PJSubmittalWorkflowItem.daysForReview>(ProjectManagementMessages.DaysForReviewIsNegative);
			}
		}

		protected virtual void _(Events.RowDeleted<PJSubmittal> e)
		{
			if (e.Row.IsLastRevision == true)
			{
				if (e.Row.RevisionID > 0)
				{
					PJSubmittal previousRevision = PXSelect<PJSubmittal, Where<PJSubmittal.submittalID, Equal<Required<PJSubmittal.submittalID>>,
					And<PJSubmittal.revisionID, Equal<Required<PJSubmittal.revisionID>>>>>.Select(this, e.Row.SubmittalID, e.Row.RevisionID - 1);

					if (previousRevision != null)
					{
						previousRevision.IsLastRevision = true;
						Submittals.Update(previousRevision);
					}
				}
			}
		}

		#endregion

		public static Dictionary<string, string> GetReportParameters(PJSubmittal document)
		{
			return new Dictionary<string, string>
			{
				[SubmittalConstants.ReportParameters.SubmittalId] = document.SubmittalID,
				[SubmittalConstants.ReportParameters.RevisionId] = document.RevisionID.ToString(),
			};
		}

		private void UpdateCurrentContact(PJSubmittal submittal)
		{
			int? contactID = null;
			int? lineNbr = null;
			if (submittal != null && submittal.Status == StatusDefinition.Open)
			{
				foreach (PJSubmittalWorkflowItem line in SubmittalWorkflowItems.Select())
				{
					if (line.Status == PJSubmittalWorkflowItem.status.Pending)
					{
						contactID = line.ContactID;
						lineNbr = line.LineNbr;
						break;
					}
				}
			}

			if (contactID == null)
			{
				contactID = submittal.OwnerID;
			}

			submittal.CurrentWorkflowItemContactID = contactID;
			submittal.CurrentWorkflowItemLineNbr = lineNbr;
		}
	}
}
