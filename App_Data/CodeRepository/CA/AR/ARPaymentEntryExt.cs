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

using PX.CCProcessingUtility;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CC.GraphExtensions;
using PX.Objects.CS;
using PX.Objects.Localizations.CA.CS;

namespace PX.Objects.Localizations.CA.AR
{
	/// <summary>
	/// Localization extension for ARCashSaleEntry.
	/// </summary>
	public class ARPaymentEntryExt : PXGraphExtension<ARPaymentEntryLevel3, ARPaymentEntry>
	{
		#region IsActive

		/// <summary>
		/// This extension is only available for Canadian Localization.
		/// </summary>
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.canadianLocalization>();
		}

		#endregion

		/// <summary>
		/// Retrieving L3 Code by UoM name.
		/// </summary>
		/// <param name="uomName">Unit of Measure name.</param>
		/// <returns>Level 3 Code.</returns>
		[PXOverride]
		public string GetL3Code(string uomName)
		{
			return UnitOfMeasure.PK.Find(Base, uomName)?.L3Code ?? UnitOfMeasureL3Codes.DefaultL3Code;
		}
	}
}
