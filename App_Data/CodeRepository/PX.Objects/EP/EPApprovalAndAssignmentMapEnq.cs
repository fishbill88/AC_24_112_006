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
using System.Collections.Generic;
using PX.Data;
using PX.Web.UI;
using PX.Objects.CS;

namespace PX.Objects.EP
{
	public class EPAssignmentAndApprovalMapEnq : PXGraph<EPAssignmentAndApprovalMapEnq>
	{
		#region Select

		public	PXSelect<
					EPAssignmentMap,
				Where<EPAssignmentMap.mapType, Equal<EPMapType.assignment>,
					Or<FeatureInstalled<FeaturesSet.approvalWorkflow>>>,
				OrderBy<Desc<EPAssignmentMap.createdDateTime>>>
			Maps;

		public	PXSelect<
					EPAssignmentMap,
				Where2<
					Where<EPAssignmentMap.mapType, Equal<EPMapType.assignment>,
						Or<FeatureInstalled<FeaturesSet.approvalWorkflow>>>,
					And<EPAssignmentMap.assignmentMapID,
						Equal<Required<EPAssignmentMap.assignmentMapID>>>>>
			MapsForRedirect;
		#endregion

		#region Actions

		public PXCancel<EPAssignmentMap> Cancel;

		public PXAction<EPAssignmentMap> ViewDetails;
		[PXUIField(MapEnableRights = PXCacheRights.Select, DisplayName = "", Visible = false)]
		[PXButton(ImageKey = Sprite.Main.RecordEdit, Tooltip = Messages.NavigateToTheSelectedMap)]
		public virtual void viewDetails()
		{
			if (Maps.Current != null)
			{
				var mapId = Maps.Current.AssignmentMapID;
				Maps.Cache.Clear();
				var map = MapsForRedirect.SelectSingle(mapId);
				PXRedirectHelper.TryRedirect(this, map, PXRedirectHelper.WindowMode.InlineWindow);
			}
		}

		public PXAction<EPAssignmentMap> AddApprovalNew;
		[PXUIField(DisplayName = "Add Approval Map")]
		[PXInsertButton(Tooltip = Messages.AddNewApprovalMap, CommitChanges = true, ImageKey = "")]
		public void addApprovalNew()
		{
			EPApprovalMapMaint graph = CreateInstance<EPApprovalMapMaint>();
			graph.AssigmentMap.Current = graph.AssigmentMap.Insert();
			graph.AssigmentMap.Cache.IsDirty = false;
			PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.InlineWindow);
		}

		public PXAction<EPAssignmentMap> AddAssignmentNew;
		[PXUIField(DisplayName = "Add Assignment Map")]
		[PXInsertButton(Tooltip = Messages.AddNewAssignmentMap, CommitChanges = true, ImageKey = "")]
		protected void addAssignmentNew()
		{
			EPAssignmentMapMaint graph = CreateInstance<EPAssignmentMapMaint>();
			graph.AssigmentMap.Current = graph.AssigmentMap.Insert();
			graph.AssigmentMap.Cache.IsDirty = false;
			PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.InlineWindow);
		}
		#endregion

		#region Event Handlers

		protected virtual void EPAssignmentMap_EntityType_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			EPAssignmentMap row = e.Row as EPAssignmentMap;
			if (row != null)
				e.ReturnState = CreateFieldStateForEntity(e.ReturnValue, row.EntityType, row.GraphType);
		}

		private PXFieldState CreateFieldStateForEntity(object returnState, string entityType, string graphType)
		{
			List<string> allowedValues = new List<string>();
			List<string> allowedLabels = new List<string>();


			Type gType = null;
			if (graphType != null)
				gType = GraphHelper.GetType(graphType);
			else if (entityType != null)
			{
				Type eType = System.Web.Compilation.PXBuildManager.GetType(entityType, false);
				gType = (eType == null) ? null : EntityHelper.GetPrimaryGraphType(this, eType);
			}

			if (gType != null)
			{
				PXSiteMapNode node = PXSiteMap.Provider.FindSiteMapNodeUnsecure(gType);
				if (node != null)
				{
					allowedValues.Add(entityType);
					allowedLabels.Add(node.Title);
				}
			}

			return PXStringState.CreateInstance(returnState, 60, null, "Entity", false, 1, null,
												allowedValues.ToArray(), allowedLabels.ToArray(), true, null);
		}

		#endregion

		#region CacheAttached
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Entity Type")]
		protected virtual void EPAssignmentMap_EntityType_CacheAttached(PXCache sender)
		{
		}
		#endregion
	}
}
