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
using PX.Data.WorkflowAPI;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.PM
{
	/// <summary>
	/// This class implements graph extension to use change order extension
	/// </summary>
	public class ProjectEntry_ChangeOrderExt : ChangeOrderExt<ProjectEntry, PMProject>
	{
		public static new bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.changeOrder>();
		}

		public override PXSelectBase<PMChangeOrder> ChangeOrder => ChangeOrders;

		public override PMChangeOrder CurrentChangeOrder => ChangeOrders.Current;

		[PXFilterable]
		[PXCopyPasteHiddenView]
		[PXViewName(Messages.ChangeOrder)]
		public PXSelect<PMChangeOrder, Where<PMChangeOrder.projectID, Equal<Current<PMProject.contractID>>>> ChangeOrders;

		public PXAction<PMProject> createChangeOrder;
		[PXUIField(DisplayName = "Create Change Order", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable CreateChangeOrder(PXAdapter adapter)
		{
			if (Base.Project.Current != null)
			{
				ChangeOrderEntry target = PXGraph.CreateInstance<ChangeOrderEntry>();
				target.Document.Insert();
				target.Document.SetValueExt<PMChangeOrder.projectID>(target.Document.Current, Base.Project.Current.ContractID);

				throw new PXRedirectRequiredException(target, false, Messages.ChangeOrder) { Mode = PXBaseRedirectException.WindowMode.Same };
			}
			return adapter.Get();
		}

		protected virtual void _(Events.RowSelected<PMProject> e)
		{
			var changeOrderEnabled = ChangeOrderEnabled();
			var changeOrderVisible = ChangeOrderVisible();

			createChangeOrder.SetVisible(changeOrderVisible);
			createChangeOrder.SetEnabled(changeOrderEnabled);
		}

		public override bool ChangeOrderEnabled()
		{
			if (Base.Project.Current != null)
			{
				return Base.Project.Current.ChangeOrderWorkflow == true;
			}

			return ChangeOrderFeatureEnabled();
		}
	}

	/// <summary>
	/// Extensions that used to configure Workflow for <see cref="ProjectEntry"/> and <see cref="PMProject"/>.
	/// </summary>
	public class ProjectEntry_ChangeOrderExt_Workflow : PXGraphExtension<ProjectEntry_Workflow, ProjectEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.changeOrder>();
		}

		public sealed override void Configure(PXScreenConfiguration config) =>
			Configure(config.GetScreenConfigurationContext<ProjectEntry, PMProject>());

		protected static void Configure(WorkflowContext<ProjectEntry, PMProject> context)
		{
			context.UpdateScreenConfigurationFor(screen =>
				screen
					.UpdateDefaultFlow(flow => flow
						.WithFlowStates(fss =>
						{
							fss.Update<ProjectStatus.planned>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add<ProjectEntry_ChangeOrderExt>(g => g.createChangeOrder);
									});
							});
							fss.Update<ProjectStatus.active>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add<ProjectEntry_ChangeOrderExt>(g => g.createChangeOrder);
									});
							});
							fss.Update<ProjectStatus.pendingApproval>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add<ProjectEntry_ChangeOrderExt>(g => g.createChangeOrder);
									});
							});
						}))
					.WithActions(actions =>
					{
						actions.Add<ProjectEntry_ChangeOrderExt>(g => g.createChangeOrder,
							c => c.InFolder(context.Categories.Get(ToolbarCategory.ActionCategoryNames.ChangeManagement)));
					}));
		}
	}
}
