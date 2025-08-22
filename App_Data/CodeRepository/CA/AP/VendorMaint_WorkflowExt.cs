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
using PX.Objects.AP;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.Localizations.CA.AP
{
	public class VendorMaint_WorkflowExt : PXGraphExtension<VendorMaint_Workflow, VendorMaint>
	{
		#region IsActive

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.canadianLocalization>();
		}

		#endregion

		public override void Configure(PXScreenConfiguration config) => Configure(config.GetScreenConfigurationContext<VendorMaint, VendorR>());

		protected virtual void Configure(WorkflowContext<VendorMaint, VendorR> context)
		{
			var managementCategory = context.Categories.Get(categoryName: VendorMaint_Workflow.ActionCategoryNames.Management);
			
			context.UpdateScreenConfigurationFor(screen =>
			{
				return screen.WithActions(actions => actions.Add(g =>
					g.GetExtension<T5018VendorMaintExt>().enableT5018Vendor, a =>
					a.WithCategory(managementCategory)));
			});
		}
	}
}
