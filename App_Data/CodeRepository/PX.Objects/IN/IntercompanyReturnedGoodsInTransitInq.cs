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
using PX.Objects.PO;
using PX.Objects.SO;

namespace PX.Objects.IN
{
	[GL.TableAndChartDashboardType]
	public class IntercompanyReturnedGoodsInTransitInq : PXGraph<IntercompanyReturnedGoodsInTransitInq>
	{
		public PXCancel<IntercompanyGoodsInTransitFilter> Cancel;

		public PXFilter<IntercompanyGoodsInTransitFilter> Filter;

		[PXFilterable]
		public SelectFrom<IntercompanyReturnedGoodsInTransitResult>
			.LeftJoin<SOShipLine>
				.On<SOShipLine.origOrderType.IsEqual<IntercompanyReturnedGoodsInTransitResult.sOType>
					.And<SOShipLine.origOrderNbr.IsEqual<IntercompanyReturnedGoodsInTransitResult.sONbr>
					.And<SOShipLine.origLineNbr.IsEqual<IntercompanyReturnedGoodsInTransitResult.sOLineNbr>
					.And<SOShipLine.confirmed.IsEqual<True>>>>>
			.LeftJoin<POReceipt>
				.On<POReceipt.receiptType.IsEqual<IntercompanyReturnedGoodsInTransitResult.origReceiptType>
					.And<POReceipt.receiptNbr.IsEqual<IntercompanyReturnedGoodsInTransitResult.origReceiptNbr>>>
			.Where<SOShipLine.shipmentNbr.IsNull
				.And<IntercompanyReturnedGoodsInTransitResult.stkItem.IsEqual<True>>
				.And<IntercompanyReturnedGoodsInTransitResult.returnReleased.IsEqual<True>>
				.And<IntercompanyReturnedGoodsInTransitResult.excludeFromIntercompanyProc.IsEqual<False>>
				.And<IntercompanyGoodsInTransitFilter.inventoryID.FromCurrent.IsNull
					.Or<IntercompanyReturnedGoodsInTransitResult.inventoryID.IsEqual<IntercompanyGoodsInTransitFilter.inventoryID.FromCurrent>>>
				.And<IntercompanyReturnedGoodsInTransitResult.returnDate.IsLessEqual<IntercompanyGoodsInTransitFilter.shippedBefore.FromCurrent>>
				.And<IntercompanyGoodsInTransitFilter.showItemsWithoutReceipt.FromCurrent.IsNotEqual<True>
					.Or<IntercompanyReturnedGoodsInTransitResult.shipmentNbr.IsNull>>
				.And<IntercompanyGoodsInTransitFilter.purchasingCompany.FromCurrent.IsNull
					.Or<IntercompanyReturnedGoodsInTransitResult.purchasingBranchBAccountID.IsEqual<IntercompanyGoodsInTransitFilter.purchasingCompany.FromCurrent>>>
				.And<IntercompanyGoodsInTransitFilter.purchasingSiteID.FromCurrent.IsNull
					.Or<IntercompanyReturnedGoodsInTransitResult.purchasingSiteID.IsEqual<IntercompanyGoodsInTransitFilter.purchasingSiteID.FromCurrent>>>
				.And<IntercompanyGoodsInTransitFilter.sellingCompany.FromCurrent.IsNull
					.Or<IntercompanyReturnedGoodsInTransitResult.sellingBranchBAccountID.IsEqual<IntercompanyGoodsInTransitFilter.sellingCompany.FromCurrent>>>
				.And<IntercompanyGoodsInTransitFilter.sellingSiteID.FromCurrent.IsNull
					.Or<IntercompanyReturnedGoodsInTransitResult.sellingSiteID.IsEqual<IntercompanyGoodsInTransitFilter.sellingSiteID.FromCurrent>>>
				.And<IntercompanyReturnedGoodsInTransitResult.sONbr.IsNull
					.Or<IntercompanyReturnedGoodsInTransitResult.sOBehavior.IsEqual<SOBehavior.rM>>>
				.And<Not<FeatureInstalled<FeaturesSet.multipleBaseCurrencies>>
					.Or<Where<IntercompanyReturnedGoodsInTransitResult.purchasingBranchID, InsideBranchesOf<Current<IntercompanyGoodsInTransitFilter.orgBAccountID>>>>>>
			.View.ReadOnly
			Results;

		public IntercompanyReturnedGoodsInTransitInq()
		{
			PXUIFieldAttribute.SetVisible<IntercompanyGoodsInTransitFilter.showItemsWithoutReceipt>(Filter.Cache, null, false);
			PXCache receiptCache = this.Caches[typeof(POReceipt)];
			PXUIFieldAttribute.SetVisible<POReceipt.receiptNbr>(receiptCache, null, false);
			PXUIFieldAttribute.SetVisible<POReceipt.receiptDate>(receiptCache, null, false);
			PXUIFieldAttribute.SetDisplayName<POReceipt.receiptDate>(receiptCache, Messages.ReceiptDate);

			bool mbcEnabled = PXAccess.FeatureInstalled<FeaturesSet.multipleBaseCurrencies>();
			PXUIFieldAttribute.SetVisible<IntercompanyGoodsInTransitFilter.purchasingCompany>(Filter.Cache, null, !mbcEnabled);
			if (mbcEnabled)
				PXUIFieldAttribute.SetDisplayName<IntercompanyGoodsInTransitFilter.orgBAccountID>(Filter.Cache, Messages.PurchasingCompanyBranch);
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
