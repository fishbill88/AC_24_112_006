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
using PX.Objects.AR;
using PX.Objects.CS;

namespace PX.Objects.FS
{
    public sealed class FSServiceContractVisibilityRestriction : PXCacheExtension<FSServiceContract>
    {
        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.visibilityRestriction>();
        }

        #region CustomerID
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [RestrictCustomerByUserBranches]
        public int? CustomerID { get; set; }
        #endregion

        #region BillCustomerID
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [RestrictCustomerByBranch(typeof(FSServiceContract.branchID), ResetCustomer = true)]
        public int? BillCustomerID { get; set; }
        #endregion

        #region VendorID
        [PXRemoveBaseAttribute(typeof(FSSelectorVendorAttribute))]
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [FSSelectorVendorRestrictVisibilityAttribute]
        public int? VendorID { get; set; }
        #endregion
    }
}
