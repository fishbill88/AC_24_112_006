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
using PX.Objects.IN;
using System;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.GL;

namespace PX.Objects.AM.CacheExtensions
{
    [Serializable]
    public sealed class INPostClassExt : PXCacheExtension<INPostClass>
    {
        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<CS.FeaturesSet.manufacturing>();
        }

        #region AMWIPAccountDefault
        public abstract class aMWIPAccountDefault : PX.Data.BQL.BqlString.Field<aMWIPAccountDefault> { }

        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Use WIP Account from")]
        [INAcctSubDefault.ClassList]
        [PXDefault(INAcctSubDefault.MaskItem, PersistingCheck = PXPersistingCheck.Nothing)]
        public String AMWIPAccountDefault { get; set; }
        #endregion
        #region AMWIPSubMask
        public abstract class aMWIPSubMask : PX.Data.BQL.BqlString.Field<aMWIPSubMask> { }

        [SubAccountMask(DisplayName = "Combine WIP Account Sub from")]
        public String AMWIPSubMask { get; set; }
        #endregion
        #region AMWIPAcctID
        public abstract class aMWIPAcctID : PX.Data.BQL.BqlInt.Field<aMWIPAcctID> { }

        [Account(DisplayName = "Work in Process Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), AvoidControlAccounts = true)]
        [PXForeignReference(typeof(Field<INPostClassExt.aMWIPAcctID>.IsRelatedTo<Account.accountID>))]
        public Int32? AMWIPAcctID { get; set; }
        #endregion
        #region AMWIPSubID
        public abstract class aMWIPSubID : PX.Data.BQL.BqlInt.Field<aMWIPSubID> { }

        [SubAccount(typeof(INPostClassExt.aMWIPAcctID), DisplayName = "Work In Process Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
        [PXForeignReference(typeof(Field<INPostClassExt.aMWIPSubID>.IsRelatedTo<Sub.subID>))]
        public Int32? AMWIPSubID { get; set; }
        #endregion
        #region AMWIPVarianceAccountDefault
        public abstract class aMWIPVarianceAccountDefault : PX.Data.BQL.BqlString.Field<aMWIPVarianceAccountDefault> { }

        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Use WIP Variance Account from")]
        [INAcctSubDefault.ClassList]
        [PXDefault(INAcctSubDefault.MaskItem, PersistingCheck = PXPersistingCheck.Nothing)]
        public String AMWIPVarianceAccountDefault { get; set; }
        #endregion
        #region AMWIPVarianceSubMask
        public abstract class aMWIPVarianceSubMask : PX.Data.BQL.BqlString.Field<aMWIPVarianceSubMask> { }

        [SubAccountMask(DisplayName = "Combine WIP Variance Sub from")]
        public String AMWIPVarianceSubMask { get; set; }
        #endregion
        #region AMWIPVarianceAcctID
        public abstract class aMWIPVarianceAcctID : PX.Data.BQL.BqlInt.Field<aMWIPVarianceAcctID> { }

        [Account(DisplayName = "WIP Variance Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), AvoidControlAccounts = true)]
        [PXForeignReference(typeof(Field<INPostClassExt.aMWIPVarianceAcctID>.IsRelatedTo<Account.accountID>))]
        public Int32? AMWIPVarianceAcctID { get; set; }
        #endregion
        #region AMWIPVarianceSubID
        public abstract class aMWIPVarianceSubID : PX.Data.BQL.BqlInt.Field<aMWIPVarianceSubID> { }

        [SubAccount(typeof(INPostClassExt.aMWIPVarianceAcctID), DisplayName = "WIP Variance Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
        [PXForeignReference(typeof(Field<INPostClassExt.aMWIPVarianceSubID>.IsRelatedTo<Sub.subID>))]
        public Int32? AMWIPVarianceSubID { get; set; }
        #endregion
    }
}