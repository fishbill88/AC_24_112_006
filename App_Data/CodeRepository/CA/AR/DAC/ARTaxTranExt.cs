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
using PX.Data.BQL;
using PX.Objects.AR;
using PX.Objects.CS;

namespace PX.Objects.Localizations.CA.AR
{
	/// <summary>
	/// A cache extension for printing of <see cref="ARTaxTran"/>. The extension provides a way to calculate the document total for printing with Canadian tax calculation.
	/// </summary>
	public sealed class ARTaxTranExt : PXCacheExtension<ARTaxTran>
    {
        #region IsActive

        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.canadianLocalization>();
        }

        #endregion

        #region CuryTotalTaxAmount

        public abstract class curyTotalTaxAmount : BqlDecimal.Field<curyTotalTaxAmount>
        {
        }

		/// <summary>
		/// The tax total for printing that is calculated with Canadian taxes.
		/// </summary>
		[PXDecimal(2)]
        [PXFormula(typeof(
            Add<ARTaxTran.curyTaxAmt, ARTaxTran.curyExpenseAmt>))]
        public decimal? CuryTotalTaxAmount
        {
            get;
            set;
        }

        #endregion

    }
}
