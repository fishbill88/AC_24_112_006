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

namespace PX.Commerce.Amazon.API.Rest.Constants
{
	/// <summary>
	/// Holds the values for Amazon API restrictions.
	/// </summary>
	public static class ApiRestrictions
	{
		/// <summary>
		/// Holds a constant value number of minutes from now we can fetch the financial event groups.
		/// </summary>
		public static int FinancialGroupMinutesLimit = -3;

		/// <summary>
		/// Holds a constant value for a number of days for finacial event group filtering.
		/// </summary>
		public static int FinancialGroupDaysLimit = 365;
	}
}
