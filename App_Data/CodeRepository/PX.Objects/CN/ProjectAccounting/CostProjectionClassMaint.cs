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
using PX.Objects.CN.ProjectAccounting.Descriptor;
using PX.Objects.PM;

namespace PX.Objects.CN.ProjectAccounting
{
	public class CostProjectionClassMaint : PXGraph<CostProjectionClassMaint>
	{
		[PXImport(typeof(PMCostProjectionClass))]
		public PXSelect<PMCostProjectionClass> Items;
		public PXSavePerRow<PMCostProjectionClass> Save;
		public PXCancel<PMCostProjectionClass> Cancel;

		protected virtual void _(Events.FieldVerifying<PMCostProjectionClass, PMCostProjectionClass.accountGroupID> e)
		{
			if (e.Row.AccountGroupID != (bool?)e.NewValue)
			{
				VerifyAndRaiseException();
			}
		}

		protected virtual void _(Events.FieldVerifying<PMCostProjectionClass, PMCostProjectionClass.taskID> e)
		{
			if (e.Row.TaskID != (bool?)e.NewValue)
			{
				VerifyAndRaiseException();
			}
		}

		protected virtual void _(Events.FieldVerifying<PMCostProjectionClass, PMCostProjectionClass.inventoryID> e)
		{
			if (e.Row.InventoryID != (bool?)e.NewValue)
			{
				VerifyAndRaiseException();
			}
		}

		protected virtual void _(Events.FieldVerifying<PMCostProjectionClass, PMCostProjectionClass.costCodeID> e)
		{
			if (e.Row.CostCodeID != (bool?)e.NewValue)
			{
				VerifyAndRaiseException();
			}
		}

		protected virtual void VerifyAndRaiseException()
		{
			var select = new PXSelect<PMCostProjection, Where<PMCostProjection.classID, Equal<Current<PMCostProjectionClass.classID>>>>(this);

			PMCostProjection first = select.SelectWindowed(0, 1);

			if (first != null)
			{
				throw new PXSetPropertyException(ProjectAccountingMessages.CostProjectionClassIsNotValid);
			}
		}
	}
}
