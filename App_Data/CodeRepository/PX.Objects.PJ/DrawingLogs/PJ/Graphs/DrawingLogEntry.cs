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

using PX.Api.Models;
using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CN.Common.Descriptor;
using PX.Objects.CN.Common.Services.DataProviders;
using PX.Objects.Common.Extensions;
using PX.Objects.CR;
using PX.Objects.PJ.DrawingLogs.PJ.DAC;
using PX.Objects.PJ.DrawingLogs.PJ.Services;
using PX.Objects.PJ.ProjectManagement.PJ.Graphs;
using PX.Objects.PM;
using PX.SM;

using Constants = PX.Objects.PJ.DrawingLogs.PJ.Descriptor.Constants;

namespace PX.Objects.PJ.DrawingLogs.PJ.Graphs
{
	public class DrawingLogEntry : ProjectManagementBaseMaint<DrawingLogEntry, DrawingLog>
	{
		[PXHidden]
		public PXSelect<DrawingLog,
			Where<DrawingLog.drawingLogId,
				Equal<Current<DrawingLog.drawingLogId>>>> CurrentDrawingLog;

		[PXHidden]
		[PXCheckCurrent]
		public PXSetup<DrawingLogSetup> Setup;

		[PXCopyPasteHiddenView]
		public SelectFrom<UploadFileRevision>.View Drawings;

		public SelectFrom<DrawingLog>
			.LeftJoin<PMProject>
				.On<PMProject.contractID.IsEqual<DrawingLog.projectId>>
			.Where<
				PMProject.contractID.IsNull
				.Or<MatchUserFor<PMProject>>>
			.View DrawingLog;

		public PXAction<DrawingLog> CreateIssue;
		public PXAction<DrawingLog> CreateNewRequestForInformation;
		public PXAction<DrawingLog> CreateNewProjectIssue;
		public PXAction<DrawingLog> DownloadZip;
		public PXAction<DrawingLog> EmailDrawingLog;
		public PXAction<DrawingLog> ViewAttachment;

		private readonly DrawingLogZipService drawingLogZipService;

		public DrawingLogEntry()
		{
			InitalizeCreateMenu();
			drawingLogZipService = new DrawingLogZipService(DrawingLog.Cache, DrawingLog.Current?.IsCurrent);
		}

		[InjectDependency]
		public IProjectDataProvider ProjectDataProvider
		{
			get;
			set;
		}

		public IEnumerable drawings()
		{
			return SelectFrom<UploadFileRevision>
				.InnerJoin<UploadFile>.On<UploadFileRevision.fileID.IsEqual<UploadFile.fileID>>
				.InnerJoin<NoteDoc>.On<NoteDoc.fileID.IsEqual<UploadFile.fileID>>
				.Where<NoteDoc.noteID.IsEqual<DrawingLog.noteID.FromCurrent>>.View.Select(this)
				.Distinct(result => result.GetItem<UploadFileRevision>().FileID);
		}

		[PXUIField(DisplayName = "Email",
			MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(CommitChanges = true, Category = PX.Objects.PM.Messages.PrintingAndEmailing, DisplayOnMainToolbar = true)]
		public virtual void emailDrawingLog()
		{
			Persist();
			var emailActivityService = new DrawingLogEmailActivityService(this);
			var graph = emailActivityService.GetEmailActivityGraph();
			PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);
		}

		[PXUIField(DisplayName = "Export Drawing Log",
			MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(CommitChanges = true, Category = PX.Objects.PM.Messages.Processing, DisplayOnMainToolbar = true)]
		public virtual void downloadZip()
		{
			Persist();
			drawingLogZipService.DownloadZipFile(DrawingLog.Current);
		}

		[PXUIField(DisplayName = "Create")]
		[PXButton]
		public virtual void createIssue()
		{
		}

		[PXUIField(DisplayName = "Create RFI",
			MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(CommitChanges = true, Category = PX.Objects.PM.Messages.Processing)]
		public virtual void createNewRequestForInformation()
		{
			var service = new RequestForInformationRedirectionService();
			CreateNewEntity(service);
		}

		[PXUIField(DisplayName = "Create Project Issue",
			MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(CommitChanges = true, Category = PX.Objects.PM.Messages.Processing)]
		public virtual void createNewProjectIssue()
		{
			var service = new ProjectIssueRedirectionService();
			CreateNewEntity(service);
		}

		[PXButton(CommitChanges = true)]
		[PXUIField]
		public virtual void viewAttachment()
		{
			if (Drawings.Current != null)
			{
				var graph = CreateInstance<UploadFileMaintenance>();
				var fileInfo = graph.GetFile(Drawings.Current.FileID.GetValueOrDefault());
				throw new PXRedirectToFileException(fileInfo.UID, true);
			}
		}

		public virtual void InitalizeCreateMenu()
		{
			CreateIssue.MenuAutoOpen = true;
			CreateIssue.AddMenuAction(CreateNewRequestForInformation);
			CreateIssue.AddMenuAction(CreateNewProjectIssue);
		}

		public virtual void _(Events.RowSelected<DrawingLog> args)
		{
			var drawingLog = args.Row;
			if (drawingLog != null)
			{
				SetFieldsEnabled(args.Cache, drawingLog);
				SetActionsEnabled(drawingLog);
			}
		}

		public virtual void _(Events.RowSelected<CRPMTimeActivity> args)
		{
			PXUIFieldAttribute.SetEnabled(args.Cache, null, false);
		}

		public WebDialogResult ShowConfirmationDialog(string message)
		{
			return DrawingLog.Ask(SharedMessages.Warning, message, MessageButtons.OKCancel);
		}

		public override void CopyPasteGetScript(bool isImportSimple, List<Command> script, List<Container> containers)
		{
			if (DrawingLog.Current?.ProjectId == null || DoesProjectHaveActiveOrPlanningStatus())
			{
				base.CopyPasteGetScript(isImportSimple, script, containers);
			}
			else
			{
				var projectTaskField = script.Single(command => command.FieldName == nameof(DAC.DrawingLog.ProjectTaskId));
				script.Remove(projectTaskField);
			}
		}

		public override IEnumerable ExecuteSelect(string viewName, object[] parameters, object[] searches,
			string[] sortColumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows,
			ref int totalRows)
		{
			OverrideDisciplinesSelectIfRequired(viewName, sortColumns, ref searches);
			return base.ExecuteSelect(viewName, parameters, searches, sortColumns,
				descendings, filters, ref startRow, maximumRows, ref totalRows);
		}

		public bool IsDrawingLogSaved(DrawingLog drawingLog)
		{
			return DrawingLog.Cache.GetStatus(drawingLog) != PXEntryStatus.Inserted;
		}

		private bool DoesProjectHaveActiveOrPlanningStatus()
		{
			var project = ProjectDataProvider.GetProject(this, DrawingLog.Current.ProjectId);
			return project != null && project.Status.IsIn(ProjectStatus.Active, ProjectStatus.Planned);
		}

		private void CreateNewEntity<TRedirectionService>(TRedirectionService service)
			where TRedirectionService : class, IRedirectionService, new()
		{
			Persist();
			service.RedirectToEntity(DrawingLog.Current);
		}

		private static void SetFieldsEnabled(PXCache cache, DrawingLog drawingLog)
		{
			PXUIFieldAttribute.SetEnabled<DrawingLog.projectId>(cache, drawingLog,
				drawingLog.OriginalDrawingId == null);
		}

		private void SetActionsEnabled(DrawingLog drawingLog)
		{
			var isDrawingLogSaved = IsDrawingLogSaved(drawingLog);
			CreateNewProjectIssue.SetEnabled(isDrawingLogSaved);
			CreateNewRequestForInformation.SetEnabled(isDrawingLogSaved);
			DownloadZip.SetEnabled(isDrawingLogSaved);
			EmailDrawingLog.SetEnabled(isDrawingLogSaved);
			var isActiveProject = ProjectDataProvider.IsActiveProject(this, drawingLog.ProjectId);
			DrawingLog.AllowDelete = isActiveProject;
			Delete.SetEnabled(isActiveProject);
		}

		private static void OverrideDisciplinesSelectIfRequired(string viewName, string[] sortColumns, ref object[] searches)
		{
			if (viewName == Constants.DisciplineViewName &&
				sortColumns.Contains(Constants.DisciplineNameField))
			{
				var index = sortColumns.FindIndex(column => column == Constants.DisciplineNameField);
				sortColumns[index] = Constants.DisciplineSortOrderField;
				searches = new[]
				{
					(object) null
				};
			}
		}
	}
}
