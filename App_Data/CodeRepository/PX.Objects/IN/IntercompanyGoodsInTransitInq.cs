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
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.SO;

namespace PX.Objects.IN
{
	[GL.TableAndChartDashboardType]
	public class IntercompanyGoodsInTransitInq : PXGraph<IntercompanyGoodsInTransitInq>
	{
		public PXCancel<IntercompanyGoodsInTransitFilter> Cancel;

		public PXFilter<IntercompanyGoodsInTransitFilter> Filter;

		[PXFilterable]
		public SelectFrom<IntercompanyGoodsInTransitResult>
			.Where<IntercompanyGoodsInTransitResult.stkItem.IsEqual<True>
				.And<IntercompanyGoodsInTransitResult.operation.IsEqual<SOOperation.issue>>
				.And<IntercompanyGoodsInTransitResult.shipmentConfirmed.IsEqual<True>>
				.And<IntercompanyGoodsInTransitResult.pOReceiptNbr.IsNull
					.Or<IntercompanyGoodsInTransitResult.receiptReleased.IsEqual<False>>>
				.And<IntercompanyGoodsInTransitResult.excludeFromIntercompanyProc.IsNotEqual<True>>
				.And<IntercompanyGoodsInTransitFilter.inventoryID.FromCurrent.IsNull
					.Or<IntercompanyGoodsInTransitResult.inventoryID.IsEqual<IntercompanyGoodsInTransitFilter.inventoryID.FromCurrent>>>
				.And<IntercompanyGoodsInTransitResult.shipDate.IsLessEqual<IntercompanyGoodsInTransitFilter.shippedBefore.FromCurrent>>
				.And<IntercompanyGoodsInTransitFilter.showOverdueItems.FromCurrent.IsNotEqual<True>
					.Or<IntercompanyGoodsInTransitResult.daysOverdue.IsNotNull>>
				.And<IntercompanyGoodsInTransitFilter.showItemsWithoutReceipt.FromCurrent.IsNotEqual<True>
					.Or<IntercompanyGoodsInTransitResult.pOReceiptNbr.IsNull>>
				.And<IntercompanyGoodsInTransitFilter.sellingCompany.FromCurrent.IsNull
					.Or<IntercompanyGoodsInTransitResult.sellingBranchBAccountID.IsEqual<IntercompanyGoodsInTransitFilter.sellingCompany.FromCurrent>>>
				.And<IntercompanyGoodsInTransitFilter.sellingSiteID.FromCurrent.IsNull
					.Or<IntercompanyGoodsInTransitResult.sellingSiteID.IsEqual<IntercompanyGoodsInTransitFilter.sellingSiteID.FromCurrent>>>
				.And<IntercompanyGoodsInTransitFilter.purchasingCompany.FromCurrent.IsNull
					.Or<IntercompanyGoodsInTransitResult.purchasingBranchBAccountID.IsEqual<IntercompanyGoodsInTransitFilter.purchasingCompany.FromCurrent>>>
				.And<IntercompanyGoodsInTransitFilter.purchasingSiteID.FromCurrent.IsNull
					.Or<IntercompanyGoodsInTransitResult.purchasingSiteID.IsEqual<IntercompanyGoodsInTransitFilter.purchasingSiteID.FromCurrent>>>
				.And<Not<FeatureInstalled<FeaturesSet.multipleBaseCurrencies>>
					.Or<Where<IntercompanyGoodsInTransitResult.sellingBranchID, InsideBranchesOf<Current<IntercompanyGoodsInTransitFilter.orgBAccountID>>>>>>
			.View.ReadOnly
			Results;

		public IntercompanyGoodsInTransitInq()
		{
			bool mbcEnabled = PXAccess.FeatureInstalled<FeaturesSet.multipleBaseCurrencies>();
			PXUIFieldAttribute.SetVisible<IntercompanyGoodsInTransitFilter.sellingCompany>(Filter.Cache, null, !mbcEnabled);
			if (mbcEnabled)
				PXUIFieldAttribute.SetDisplayName<IntercompanyGoodsInTransitFilter.orgBAccountID>(Filter.Cache, Messages.SellingCompanyBranch);
		}

		protected virtual IEnumerable results()
		{
			using (new PXReadBranchRestrictedScope())
			{
				PXView query = new PXView(this, true, Results.View.BqlSelect);
				int startRow = PXView.StartRow;
				int totalRows = 0;

				var res = query.Select(PXView.Currents, PXView.Parameters,
					PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters,
					ref startRow, PXView.MaximumRows, ref totalRows);

				PXView.StartRow = 0;

				return res;
			}
		}
	}
}
