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

namespace PX.Objects.AP
{
	public class APIntegrityCheckVisibilityRestriction : PXGraphExtension<APIntegrityCheck>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.visibilityRestriction>();
		}
		public override void Initialize()
		{
			base.Initialize();

			Base.APVendorList_ByVendorClassID.WhereAnd<Where<Vendor.vOrgBAccountID, RestrictByUserBranches<Current<AccessInfo.userName>>>>();
		}

		public PXFilteredProcessing<
			Vendor, APIntegrityCheckFilter,
			Where<Vendor.vOrgBAccountID, RestrictByUserBranches<Current<AccessInfo.userName>>,
				And<Match<Current<AccessInfo.userName>>>>>
			APVendorList;
	}

	public sealed class APIntegrityCheckFilterVisibilityRestriction : PXCacheExtension<APIntegrityCheckFilter>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.visibilityRestriction>();
		}

		#region VendorClassID
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[RestrictVendorClassByUserBranches]
		public string VendorClassID { get; set; }
		#endregion
	}
}
