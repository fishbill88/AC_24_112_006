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
using PX.Objects.CR;
using PX.Objects.CS;

namespace PX.Objects.PJ.RequestsForInformation.CR.CacheExtensions
{
    public sealed class BAccountExt : PXCacheExtension<BAccount>
    {
        [PXBool]
        [PXUIField(DisplayName = "Is Related To Project Contact",
            Visibility = PXUIVisibility.SelectorVisible, Visible = false)]
        public bool? IsRelatedToProjectContact
        {
            get;
            set;
        }

        // PXSelector attribute is used for querying inner entity in Notification Templates.
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXSelector(typeof(Search<Address.addressID>))]
        public int? DefAddressID
        {
            get;
            set;
        }

        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.constructionProjectManagement>();
        }

        public abstract class defAddressID : IBqlField
        {
        }

        public abstract class isRelatedToProjectContact : IBqlField
        {
        }
    }
}