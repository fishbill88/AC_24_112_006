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

using System.Linq;
using PX.Data;
using PX.Objects.CS;

namespace PX.Objects.GL.Reclassification.UI
{
	public class ReclassifyTransactionsProcessMultipleBaseCurrencies : PXGraphExtension<ReclassifyTransactionsProcess>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.multipleBaseCurrencies>();
		}

		protected void _(Events.FieldUpdated<ReclassifyTransactionsProcess.ReplaceOptions, ReclassifyTransactionsProcess.ReplaceOptions.withBranchID> e)
		{
			ReclassifyTransactionsProcess.ReplaceOptions row = e.Row;
			bool hasSameBaseCurrency = PXSelect<Branch, Where<Branch.baseCuryID, EqualBaseCuryID<ReclassifyTransactionsProcess.ReplaceOptions.withBranchID.FromCurrent>,
				And<Branch.branchID.IsEqual<ReclassifyTransactionsProcess.ReplaceOptions.newBranchID.FromCurrent>>>>.SelectSingleBound(Base, null).Any();

			if (!hasSameBaseCurrency)
			{
				e.Cache.SetValueExt<ReclassifyTransactionsProcess.ReplaceOptions.newBranchID>(row, null);
			}
		}
	}
}
