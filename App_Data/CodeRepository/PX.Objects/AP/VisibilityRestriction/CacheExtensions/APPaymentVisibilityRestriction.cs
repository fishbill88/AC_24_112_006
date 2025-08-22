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
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;

namespace PX.Objects.AP
{
	public sealed class APPaymentVisibilityRestriction : PXCacheExtension<APPayment>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.visibilityRestriction>();
		}

		#region BranchID
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[Branch(typeof(AccessInfo.branchID), IsDetail = false, TabOrder = 0)]
		[PXFormula(typeof(Switch<
				Case<Where<PendingValue<APPayment.branchID>, IsPending>, Null,
				Case<Where<APPayment.vendorLocationID, IsNotNull,
						And<Selector<APPayment.vendorLocationID, Location.vBranchID>, IsNotNull>>,
					Selector<APPayment.vendorLocationID, Location.vBranchID>,
				Case<Where<APPayment.vendorID, IsNotNull,
						And<Not<Selector<APPayment.vendorID, Vendor.vOrgBAccountID>, RestrictByBranch<Current2<APPayment.branchID>>>>>,
					Null,
				Case<Where<Current2<APPayment.branchID>, IsNotNull>,
					Current2<APPayment.branchID>>>>>,
				Current<AccessInfo.branchID>>))]
		public int? BranchID { get; set; }
		#endregion

		#region VendorID
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[RestrictVendorByBranch(branchID: typeof(APPayment.branchID))]
		public int? VendorID { get; set; }
		#endregion
	}
}