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
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CR;

namespace PX.Objects.AR
{
	public sealed class ARPaymentVisibilityRestriction : PXCacheExtension<ARPayment>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.visibilityRestriction>();
		}
		#region BranchID
		/// <summary>
		/// The identifier of the <see cref="Branch">branch</see> to which the document belongs.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Branch.BranchID"/> field.
		/// </value>
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[Branch(typeof(AccessInfo.branchID), IsDetail = false, TabOrder = 0)]
		[PXFormula(typeof(Switch<
			Case<Where<PendingValue<ARPayment.branchID>, IsPending>, Null,
			Case<Where<ARPayment.customerLocationID, IsNotNull,
					And<Selector<ARPayment.customerLocationID, Location.cBranchID>, IsNotNull>>,
				Selector<ARPayment.customerLocationID, Location.cBranchID>,
			Case<Where<ARPayment.customerID, IsNotNull,
					And<Not<Selector<ARPayment.customerID, Customer.cOrgBAccountID>, RestrictByBranch<Current2<ARPayment.branchID>>>>>,
				Null,
			Case<Where<Current2<ARPayment.branchID>, IsNotNull>,
				Current2<ARPayment.branchID>>>>>,
			Current<AccessInfo.branchID>>))]
		public int? BranchID { get; set; }
		#endregion


		#region CustomerID
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[RestrictCustomerByBranch(branchID: typeof(ARPayment.branchID))]
		public int? CustomerID { get; set; }
		#endregion
	}
}
