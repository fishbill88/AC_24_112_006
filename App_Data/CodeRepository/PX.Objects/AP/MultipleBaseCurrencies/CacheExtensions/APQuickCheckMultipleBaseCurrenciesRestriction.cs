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
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.AP.Standalone;

namespace PX.Objects.AP
{
	/// <summary>
	/// Graph extension to restrict vendors selection based on base currency when MBC is on.
	/// </summary>
	public sealed class APQuickCheckMultipleBaseCurrenciesRestriction : PXCacheExtension<APQuickCheckVisibilityRestriction, APQuickCheck>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.multipleBaseCurrencies>();
		}

		#region VendorID
		/// <summary>
		/// Vendor ID based on selected branch base currency.
		/// </summary>
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRestrictor(typeof(Where<Current2<branchBaseCuryID>, IsNull,
			Or<Vendor.baseCuryID, IsNull,
			Or<Vendor.baseCuryID, Equal<Current2<branchBaseCuryID>>>>>), null)]
		public int? VendorID { get; set; }
		#endregion

		#region BranchBaseCuryID
		public new abstract class branchBaseCuryID : PX.Data.BQL.BqlString.Field<branchBaseCuryID> { }

		/// <summary>
		/// Branch base currency ID.
		/// </summary>
		[PXString]
		[PXFormula(typeof(Selector<APQuickCheck.branchID, Branch.baseCuryID>))]
		public string BranchBaseCuryID { get; set; }
		#endregion
		#region VendorBaseCuryID
		public new abstract class vendorBaseCuryID : PX.Data.BQL.BqlString.Field<vendorBaseCuryID> { }

		/// <summary>
		/// Vendor base currency ID.
		/// </summary>
		[PXString]
		[PXFormula(typeof(Selector<APQuickCheck.vendorID, Vendor.baseCuryID>))]
		public string VendorBaseCuryID { get; set; }
		#endregion
	}
}
