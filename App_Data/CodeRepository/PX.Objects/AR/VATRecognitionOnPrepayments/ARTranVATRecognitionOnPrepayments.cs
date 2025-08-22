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
using PX.Objects.CM.Extensions;
using PX.Objects.CS;
using System;

namespace PX.Objects.AR
{
	/// <exclude/>
	public sealed class ARTranVATRecognitionOnPrepayments : PXCacheExtension<ARTran>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.vATRecognitionOnPrepayments>();
		}

		#region CuryPrepaymentAmt
		public abstract class curyPrepaymentAmt : PX.Data.BQL.BqlDecimal.Field<curyPrepaymentAmt> { }

		/// <summary>
		/// Part of sales order amount which shall be paid in PPI document
		/// </summary>
		[PXDBCurrency(typeof(ARTran.curyInfoID), typeof(ARTranVATRecognitionOnPrepayments.prepaymentAmt))]
		[PXFormula(typeof(Mult<
			Sub<ARTran.curyExtPrice, ARTran.curyDiscAmt>,
			Div<ARTranVATRecognitionOnPrepayments.prepaymentPct, decimal100>>))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Prepayment Amount", Visibility = PXUIVisibility.Invisible)]
		public decimal? CuryPrepaymentAmt { get; set; }
		#endregion
		#region PrepaymentAmt
		public abstract class prepaymentAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentAmt> { }

		/// <summary>
		/// Part of sales order amount which shall be paid in PPI document
		/// </summary>
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public decimal? PrepaymentAmt { get; set; }
		#endregion

		#region PrepaymentPct
		public abstract class prepaymentPct : PX.Data.BQL.BqlDecimal.Field<prepaymentPct> { }

		/// <summary>
		/// Percent of the Sales Order amount which shall be paid in PPI document
		/// </summary>
		[PXDBDecimal(6, MinValue = 0, MaxValue = 100)]
		[PXFormula(typeof(CalculatePrepaymentPercent<prepaymentPct, curyPrepaymentAmt, ARTran.curyExtPrice, ARTran.curyDiscAmt>))]
		[PXDefault(TypeCode.Decimal, "100.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Prepayment Percent", Visibility = PXUIVisibility.Invisible)]
		public decimal? PrepaymentPct { get; set; }
		#endregion
	}
}
