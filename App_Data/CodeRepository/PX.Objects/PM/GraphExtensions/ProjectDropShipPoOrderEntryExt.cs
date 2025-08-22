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

using CommonServiceLocator;

using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.PO;

namespace PX.Objects.PM.GraphExtensions
{
	public class ProjectDropShipPoOrderEntryExt : PXGraphExtension<POOrderEntry>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.projectAccounting>();

		#region DAC Attributes Override

		[PXRemoveBaseAttribute(typeof(POOrderType.ListAttribute))]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[POOrderTypeListProjectDropShip()]
		protected virtual void _(Events.CacheAttached<POOrder.orderType> e) {}

		#endregion

		protected void _(Events.FieldVerifying<POOrder, POOrder.projectID> e)
		{
			var projectID = e.NewValue as int?;
			if (projectID != null
				&& e.Row?.OrderType == POOrderType.ProjectDropShip
				&& Base.IsCopyPasteContext
				&& ProjectDefaultAttribute.IsNonProject(projectID))
			{
				e.NewValue = null;
			}
		}

		protected void _(Events.FieldVerifying<POLine, POLine.projectID> e)
		{
			var projectID = e.NewValue as int?;
			if (projectID != null
				&& e.Row?.OrderType == POOrderType.ProjectDropShip
				&& Base.IsCopyPasteContext
				&& ProjectDefaultAttribute.IsNonProject(projectID))
			{
				e.NewValue = null;
			}
		}
	}

	public class POOrderTypeListProjectDropShipAttribute : POOrderType.ListAttribute
	{
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			IProjectSettingsManager psm = ServiceLocator.Current.GetInstance<IProjectSettingsManager>();
			if (psm.IsPMVisible(BatchModule.PO) == false)
			{
				var projectDropshipIndex = Array.IndexOf(_AllowedValues, POOrderType.ProjectDropShip);
				if (projectDropshipIndex < 0) return;

				List<string> tmp = new List<string>(_AllowedValues);
				tmp.RemoveAt(projectDropshipIndex);
				_AllowedValues = tmp.ToArray();

				tmp = new List<string>(_AllowedLabels);
				tmp.RemoveAt(projectDropshipIndex);
				_AllowedLabels = tmp.ToArray();
			}
		}
	}
}
