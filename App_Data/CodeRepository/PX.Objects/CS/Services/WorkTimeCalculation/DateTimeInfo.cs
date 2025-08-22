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
using PX.Common;

namespace PX.Objects.CS.Services.WorkTimeCalculation
{
	[PXInternalUseOnly]
	public readonly struct DateTimeInfo : IEquatable<DateTimeInfo>
	{
		public DateTimeInfo(PXTimeZoneInfo timeZoneInfo, DateTime dateTime)
		{
			TimeZoneInfo = timeZoneInfo ?? throw new ArgumentNullException(nameof(timeZoneInfo));
			DateTime     = dateTime;
		}

		public PXTimeZoneInfo TimeZoneInfo { get; }
		public DateTime DateTime { get; }

		public static DateTimeInfo FromLocalTimeZone(DateTime dateTime) => new (LocaleInfo.GetTimeZone(), dateTime);
		public static DateTimeInfo FromUtcTimeZone(DateTime dateTime) => new (PXTimeZoneInfo.Invariant, dateTime);

		public DateTimeInfo ToTimeZone(PXTimeZoneInfo timeZone)
		{
			if (TimeZoneInfo.Equals(timeZone))
				return this;

			var utc = PXTimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo, useDST: true);
			var tdt = PXTimeZoneInfo.ConvertTimeFromUtc(utc, timeZone, useDST: true);
			return new DateTimeInfo(timeZone, tdt);
		}

		public DateTimeInfo ToUtc() => ToTimeZone(PXTimeZoneInfo.Invariant);

		public bool Equals(DateTimeInfo other)
		{
			return TimeZoneInfo.Equals(other.TimeZoneInfo) && DateTime.Equals(other.DateTime);
		}

		public override bool Equals(object obj)
		{
			return obj is DateTimeInfo other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (TimeZoneInfo.GetHashCode() * 397) ^ DateTime.GetHashCode();
			}
		}
	}
}
