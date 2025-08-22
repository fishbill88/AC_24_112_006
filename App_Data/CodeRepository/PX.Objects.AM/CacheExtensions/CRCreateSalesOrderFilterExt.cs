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
using PX.Objects.CR.Extensions.CRCreateSalesOrder;
using PX.Objects.CS;

namespace PX.Objects.AM.CacheExtensions
{
    /// <summary>
    /// Manufacturing cache extension on Opportunity Maint. - "Create Sales Order" process/panel for <see cref="CreateSalesOrderFilter"/>
    /// </summary>
    [Serializable]
    public sealed class CRCreateSalesOrderFilterExt : PXCacheExtension<CreateSalesOrderFilter>
    {
        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.manufacturing>();
        }

        #region AMIncludeEstimate
        public abstract class aMIncludeEstimate : PX.Data.BQL.BqlBool.Field<aMIncludeEstimate> { }
        /// <summary>
        /// Indicates if the estimates should be included in the create sales order process.
        /// When checked the current opp estimates will covert to sales order detail lines if valid.
        /// </summary>
        [PXBool]
        [PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Convert Estimates")]
        public bool? AMIncludeEstimate { get; set; }
        #endregion
        #region AMCopyConfigurations
        public abstract class aMCopyConfigurations : PX.Data.BQL.BqlBool.Field<aMCopyConfigurations> { }

        /// <summary>
        /// Indicates if the configurations should be included in the create sales order process.
        /// When checked the current opp configurations will copy to sales order.
        /// </summary>
        [PXBool]
        [PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Copy Configurations")]
        public bool? AMCopyConfigurations { get; set; }
        #endregion
    }
}