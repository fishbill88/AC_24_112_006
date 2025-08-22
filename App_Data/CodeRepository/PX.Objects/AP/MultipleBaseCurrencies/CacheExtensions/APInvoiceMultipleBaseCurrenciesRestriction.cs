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
using PX.Data;
using PX.Objects.Common;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CR;

namespace PX.Objects.AP
{
	public sealed class APInvoiceMultipleBaseCurrenciesRestriction : PXCacheExtension<APInvoiceVisibilityRestriction,APInvoice>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.multipleBaseCurrencies>();
		}

		#region BranchID
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXFormula(typeof(Switch<
				Case<Where<PendingValue<APInvoice.branchID>, IsPending>, Null,
				Case<Where<APInvoice.vendorLocationID, IsNull, And<UnattendedMode, Equal<True>>>, Null,
				Case<Where<Selector<APInvoice.vendorLocationID, Location.vBranchID>, IsNotNull>,
					Selector<APInvoice.vendorLocationID, Location.vBranchID>,
				Case<Where<APInvoice.vendorID, IsNotNull,
						And<Not<Selector<APInvoice.vendorID, Vendor.vOrgBAccountID>, RestrictByBranch<Current2<APInvoice.branchID>>>>>,
					Null,
				Case<Where<Current2<APInvoice.branchID>, IsNotNull>,
					Current2<APInvoice.branchID>,
				Case<Where<APInvoice.vendorID, IsNotNull, 
						And<Selector<APInvoice.vendorID, Vendor.baseCuryID>, IsNotNull,
						And<Selector<APInvoice.vendorID, Vendor.baseCuryID>, NotEqual<Current<AccessInfo.baseCuryID>>>>>,
					Null>>>>>>, 
				Current<AccessInfo.branchID>>))]
		public Int32? BranchID{get;	set;}
		#endregion

		#region VendorID
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRestrictor(typeof(Where<Current2<branchBaseCuryID>, IsNull,
			Or<Vendor.baseCuryID, IsNull,
			Or<Vendor.baseCuryID, Equal<Current2<branchBaseCuryID>>>>>), null)]
		public int? VendorID { get; set; }
		#endregion

		#region BranchBaseCuryID
		public new abstract class branchBaseCuryID : PX.Data.BQL.BqlString.Field<branchBaseCuryID> { }

		[PXString]
		[PXFormula(typeof(Selector<APInvoice.branchID, Branch.baseCuryID>))]
		public string BranchBaseCuryID { get; set; }
		#endregion
		#region VendorBaseCuryID
		public new abstract class vendorBaseCuryID : PX.Data.BQL.BqlString.Field<vendorBaseCuryID> { }

		[PXString]
		[PXFormula(typeof(Selector<APInvoice.vendorID, Vendor.baseCuryID>))]
		public string VendorBaseCuryID { get; set; }
		#endregion
	}
}
