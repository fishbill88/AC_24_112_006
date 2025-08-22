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

using PX.Common;
using PX.Data;
using System;

namespace PX.Objects.CS
{
	[PXInternalUseOnly]
	public class CSTimeSpanShortWith24HoursAttribute : PXTimeSpanLongAttribute
	{
		public CSTimeSpanShortWith24HoursAttribute()
		{
			Format = TimeSpanFormatType.ShortHoursMinutesCompact;
		}

		public static readonly int MinutesInDay = (int)TimeSpan.FromDays(1).TotalMinutes;
		public const string StringValueFor24Hours = "2400";
		public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			bool use24Hours = e.ReturnValue is int value && value == MinutesInDay;

			base.FieldSelecting(sender, e);

			if (use24Hours && e.ReturnValue is string)
			{
				e.ReturnValue = StringValueFor24Hours;
			}
		}
	}

	[PXInternalUseOnly]
	public class CSDBTimeSpanShortWith24HoursAttribute : PXDBTimeSpanLongAttribute
	{
		public CSDBTimeSpanShortWith24HoursAttribute()
		{
			Format = TimeSpanFormatType.ShortHoursMinutesCompact;
		}

		public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			bool use24Hours = e.ReturnValue is int value && value == CSTimeSpanShortWith24HoursAttribute.MinutesInDay;

			base.FieldSelecting(sender, e);

			if (use24Hours && e.ReturnValue is string)
			{
				e.ReturnValue = CSTimeSpanShortWith24HoursAttribute.StringValueFor24Hours;
			}
		}
	}
}
