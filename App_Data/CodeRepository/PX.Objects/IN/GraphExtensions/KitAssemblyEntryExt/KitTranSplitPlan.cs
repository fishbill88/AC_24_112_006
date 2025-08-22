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

using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Objects.IN.GraphExtensions.INRegisterEntryBaseExt;

namespace PX.Objects.IN.GraphExtensions.KitAssemblyEntryExt
{
	public class KitTranSplitPlan : INTranSplitPlanBase<KitAssemblyEntry, INKitRegister, INKitTranSplit>
	{
		protected override void PrefetchDocumentPlansToCache()
		{
			PXSelect<INItemPlan, Where<INItemPlan.refNoteID, Equal<Current<INKitRegister.noteID>>>>.Select(Base).Consume();
		}

		protected override IEnumerable<INKitTranSplit> GetDocumentSplits()
		{
			return PXSelect<INKitTranSplit,
				Where<INKitTranSplit.docType, Equal<Current<INKitRegister.docType>>,
					And<INKitTranSplit.refNbr, Equal<Current<INKitRegister.refNbr>>>>>
				.Select(Base)
				.RowCast<INKitTranSplit>();
		}

		public override void _(Events.RowUpdated<INKitRegister> e)
		{
			base._(e);

			if (e.Row.KitRequestDate != e.OldRow.KitRequestDate)
			{
				PrefetchDocumentPlansToCache();

				foreach (INKitTranSplit split in GetDocumentSplits())
				{
					foreach (INItemPlan plan in PlanCache.Cached)
					{
						if (plan.PlanID == split.PlanID && PlanCache.GetStatus(plan).IsNotIn(PXEntryStatus.Deleted, PXEntryStatus.InsertedDeleted))
						{
							INItemPlan copy = PXCache<INItemPlan>.CreateCopy(plan);
							copy.PlanDate = e.Row.KitRequestDate;
							PlanCache.Update(copy);
						}
					}
				}
			}
		}

		public override INItemPlan DefaultValues(INItemPlan planRow, INKitTranSplit origRow)
		{
			planRow = base.DefaultValues(planRow, origRow);
			if (planRow == null)
				return null;

			INKitRegister parent = PXParentAttribute.SelectParent<INKitRegister>(ItemPlanSourceCache, origRow);
			planRow.ProjectID = parent.ProjectID;
			planRow.TaskID = parent.TaskID;
			planRow.CostCenterID = parent.CostCenterID;
			planRow.UOM = parent.UOM;
			planRow.PlanDate = parent.KitRequestDate;
			return planRow;
		}
	}
}
