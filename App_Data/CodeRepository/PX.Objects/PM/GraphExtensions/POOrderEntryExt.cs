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

using CommonServiceLocator;
using PX.Data;
using PX.Objects.CN.Subcontracts.SC.Graphs;
using PX.Data.BQL.Fluent;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CM.Extensions;
using PX.Objects.CN.Common.Extensions;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM.TaxZoneExtension;
using PX.Objects.PO;
using PX.Web.UI.Frameset.Helpers;
using System;
using System.Collections;
using System.Linq;
using static PX.Objects.CT.ContractAction;
using static PX.Objects.PM.ChangeOrderEntry;
using static PX.Objects.PM.ProjectEntry;

namespace PX.Objects.PM
{
	public class POOrderEntryExt : CommitmentTracking<POOrderEntry>
	{
		[InjectDependency]
		public IProjectMultiCurrency MultiCurrencyService { get; set; }

		public static new bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.projectAccounting>();
		}

		public PXSetup<PMSetup> Setup;

		[Project]
		protected virtual void _(Events.CacheAttached<PMChangeOrderLine.projectID> e) { }

		public override void Initialize()
		{
			base.Initialize();
			Base.OnBeforePersist += SetBehaviorBasedOnLines;
		}

		public bool SkipProjectLockCommitmentsVerification;

		protected virtual void _(Events.FieldVerifying<POLine, POLine.projectID> e)
		{
			if (!SkipProjectLockCommitmentsVerification)
				VerifyProjectLockCommitments((int?)e.NewValue, e.Cache);

			VerifyExchangeRateExistsForProject((int?)e.NewValue);
		}

		protected virtual void _(Events.FieldVerifying<POOrder, POOrder.projectID> e)
		{
			if (!SkipProjectLockCommitmentsVerification)
				VerifyProjectLockCommitments((int?)e.NewValue,e.Cache);

			VerifyExchangeRateExistsForProject((int?)e.NewValue);
		}

		protected virtual void _(Events.FieldUpdated<POLine, POLine.lineType> e)
		{
			if (e.Row == null) return;

			POLine pOLine = e.Row;
			if (pOLine.LineType == POLineType.Description)
			{
				e.Cache.SetValueExt<POLine.projectID>(e.Row, ProjectDefaultAttribute.NonProject());
			}
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), nameof(PXUIFieldAttribute.DisplayName), "Change Order Description")]
		protected virtual void _(Events.CacheAttached<PMChangeOrder.description> e) {	}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), nameof(PXUIFieldAttribute.DisplayName), "Line Description")]
		protected virtual void _(Events.CacheAttached<PMChangeOrderLine.description> e) { }

		public virtual void VerifyProjectLockCommitments(int? newProjectID, PXCache cache)
		{
			if (!PXAccess.FeatureInstalled<FeaturesSet.changeOrder>())
				return;

			PMProject project;
			var graphType = cache.Graph.GetType().BaseType;

			if (ProjectDefaultAttribute.IsProject(Base, newProjectID, out project) && project.LockCommitments == true)
			{
				if (graphType == typeof(SubcontractEntry))
				{
					var ex = new PXSetPropertyException(PM.Messages.ProjectCommintmentsLockedForSubcontracts, project.ContractCD);
					ex.ErrorValue = project.ContractCD;

					throw ex;
				}
				else if (graphType == typeof(POOrderEntry))
				{
					var ex = new PXSetPropertyException(PM.Messages.ProjectCommintmentsLockedForPurchaseOrders, project.ContractCD);
					ex.ErrorValue = project.ContractCD;

					throw ex;
				}
				else
				{
					var ex = new PXSetPropertyException(PM.Messages.ProjectCommintmentsLocked);
					ex.ErrorValue = project.ContractCD;

					throw ex;
				}
			}
		}

		public virtual void VerifyExchangeRateExistsForProject(int? newProjectID)
		{
			if (!PXAccess.FeatureInstalled<FeaturesSet.projectMultiCurrency>())
				return;

			if (!IsCommitmentsEnabled())
				return;

			PMProject project;
			if (ProjectDefaultAttribute.IsProject(Base, newProjectID, out project) && Base.Document.Current != null)
			{
				//with throw PXException if conversion is required but rate is not defined.
				decimal result = MultiCurrencyService.GetValueInProjectCurrency(Base, project, Base.Document.Current.CuryID, Base.Document.Current.OrderDate, 100m);
			}
		}

		private bool IsCommitmentsEnabled()
		{
			return Setup.Current?.CostCommitmentTracking == true;
		}

		public virtual void SetBehaviorBasedOnLines(PXGraph obj)
		{
			if (Base.Document.Current != null && PXAccess.FeatureInstalled<FeaturesSet.changeOrder>())
			{
				bool changeOrderBehaviorIsRequired = false;

				var select = new PXSelect<PMChangeOrderLine,
					Where<PMChangeOrderLine.pOOrderType, Equal<Current<POOrder.orderType>>,
					And<PMChangeOrderLine.pOOrderNbr, Equal<Current<POOrder.orderNbr>>>>>(Base);

				PMChangeOrderLine link = select.SelectSingle();
				if (link != null)
				{
					changeOrderBehaviorIsRequired = true;
				}

				if (changeOrderBehaviorIsRequired && Base.Document.Current.Behavior != POBehavior.ChangeOrder)
				{
					Base.Document.Current.Behavior = POBehavior.ChangeOrder;
					Base.Document.Update(Base.Document.Current);
				}
				else if (!changeOrderBehaviorIsRequired && Base.Document.Current.Behavior == POBehavior.ChangeOrder)
				{
					Base.Document.Current.Behavior = POBehavior.Standard;
					Base.Document.Update(Base.Document.Current);
				}
			}
		}

		[PXOverride]
		public virtual IEnumerable CreateAPInvoice(PXAdapter adapter, Func<PXAdapter, IEnumerable> baseHandler)
		{
			ValidatePOLines();
			return baseHandler(adapter);
		}

		private void ValidatePOLines()
		{
			bool accountValidationFailed = false;
			foreach (POLine line in Base.Transactions.Select())
			{
				Account account = Account.PK.Find(Base, line.ExpenseAcctID);
				if (account?.AccountGroupID == null && line.LineType == POLineType.Service && ProjectDefaultAttribute.IsProject(Base, line.ProjectID))
				{
					InventoryItem item = InventoryItem.PK.Find(Base, line.InventoryID);
					if (item != null && item.PostToExpenseAccount == InventoryItem.postToExpenseAccount.Sales)
						continue;

					PXTrace.WriteError(Messages.NoAccountGroup2, account.AccountCD);
					accountValidationFailed = true;
				}
			}

			if (accountValidationFailed)
			{
				throw new PXException(Messages.FailedToAddLine);
			}
		}
	}

	/// <summary>
	/// This class implements graph extension to use change order extension
	/// </summary>
	public class POOrderEntry_ChangeOrderExt : ChangeOrderExt<POOrderEntry, POOrder>
	{
		public static new bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.changeOrder>();
		}

		public override PXSelectBase<PMChangeOrder> ChangeOrder => ChangeOrders;

		public override PMChangeOrder CurrentChangeOrder => ChangeOrders.Select(ChangeOrderDetails.Current.RefNbr);

		[PXFilterable]
		[PXViewName(PM.Messages.ChangeOrder)]
		[PXCopyPasteHiddenView()]
		public PXSelectJoin<PMChangeOrderLine,
			InnerJoin<PMChangeOrder, On<PMChangeOrderLine.refNbr, Equal<PMChangeOrder.refNbr>>>,
			Where<PMChangeOrderLine.pOOrderType, Equal<Current<POOrder.orderType>>,
			And<PMChangeOrderLine.pOOrderNbr, Equal<Current<POOrder.orderNbr>>>>> ChangeOrderDetails;

		[PXViewName(PM.Messages.ChangeOrder)]
		[PXCopyPasteHiddenView()]
		[PXHidden]
		public PXSelect<PMChangeOrder,
			Where<PMChangeOrder.refNbr, Equal<Current<PMChangeOrderLine.refNbr>>>> ChangeOrders;

		protected virtual void _(Events.RowSelected<POOrder> e)
		{
			ChangeOrderDetails.Cache.AllowSelect = IsCommitmentsEnabled();
		}

		protected virtual void _(Events.RowDeleting<POOrder> e)
		{
			POOrder doc = e.Row;
			if (doc.Hold != true && doc.Behavior == POBehavior.ChangeOrder)
			{
				throw new PXException(PX.Objects.PO.Messages.CanNotDeleteWithChangeOrderBehavior);
			}
		}

		private bool IsCommitmentsEnabled()
		{
			return Setup.Current?.CostCommitmentTracking == true;
		}
	}
}
