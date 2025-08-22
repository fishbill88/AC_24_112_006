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
using System;

namespace PX.Objects.PR
{
	public static class PRUtils
	{
		public static decimal Round(decimal num, int precision = 2)
		{
			return Math.Round(num, precision, MidpointRounding.AwayFromZero);
		}

		public static decimal Round(decimal? num, int precision = 2)
		{
			return Round(num.GetValueOrDefault(), precision);
		}

		public static bool ParseBoolean(string value)
		{
			try
			{
				return bool.Parse(value);
			}
			catch (Exception exception)
			{
				throw new PXException(exception, Messages.BooleanParseError, value);
			}
		}

		public static bool ParseBoolean(string value, bool valueWhenEmpty)
		{
			return string.IsNullOrWhiteSpace(value) ? valueWhenEmpty : ParseBoolean(value);
		}

		public static bool? ParseNullableBoolean(string value)
		{
			return string.IsNullOrWhiteSpace(value) ? new bool?() : ParseBoolean(value);
		}

		public static int ParseInteger(string value)
		{
			try
			{
				return int.Parse(value);
			}
			catch (Exception exception)
			{
				throw new PXException(exception, Messages.IntegerParseError, value);
			}
		}

		public static int ParseInteger(string value, int valueWhenEmpty)
		{
			return string.IsNullOrWhiteSpace(value) ? valueWhenEmpty : ParseInteger(value);
		}

		public static int? ParseNullableInteger(string value)
		{
			return string.IsNullOrWhiteSpace(value) ? new int?() : ParseInteger(value);
		}

		public static decimal ParseDecimal(string value)
		{
			try
			{
				return decimal.Parse(value);
			}
			catch (Exception exception)
			{
				throw new PXException(exception, Messages.DecimalParseError, value);
			}
		}
	}
}
