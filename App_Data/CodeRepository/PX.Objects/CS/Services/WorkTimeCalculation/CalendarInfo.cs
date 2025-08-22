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
using System.Collections.Generic;
using PX.Common;

namespace PX.Objects.CS.Services.WorkTimeCalculation
{
	[PXInternalUseOnly]
	public readonly struct CalendarInfo
	{
		public CalendarInfo(
			string calendarID,
			PXTimeZoneInfo timeZone,
			double workdayHours,
			IReadOnlyDictionary<DayOfWeek, DayOfWeekInfo> daysOfWeek,
			IReadOnlyCollection<CalendarExceptionInfo> exceptions)
		{
			CalendarID   = calendarID;
			TimeZone     = timeZone;
			DaysOfWeek   = daysOfWeek;
			Exceptions   = exceptions;
			WorkdayHours = workdayHours;
		}

		public string CalendarID { get; }
		public PXTimeZoneInfo TimeZone { get; }
		public double WorkdayHours { get; }
		public IReadOnlyDictionary<DayOfWeek, DayOfWeekInfo> DaysOfWeek { get; }
		public IReadOnlyCollection<CalendarExceptionInfo> Exceptions { get; }

		public CalendarExceptionInfo? TryFindExceptionForDate(DateTime date)
		{
			date = date.Date;
			foreach (var exception in Exceptions)
			{
				if (exception.Date == date)
				{
					return exception;
				}
			}

			return null;
		}
	}

	[PXInternalUseOnly]
	public readonly struct CalendarExceptionInfo
	{
		public CalendarExceptionInfo(DateTime date, TimeRange timeRange, bool isWorkingDay)
		{
			Date = date.Date;
			TimeRange = timeRange;
			IsWorkingDay = isWorkingDay;
		}

		public DateTime Date { get; }
		public bool IsWorkingDay {get;}
		public TimeRange TimeRange { get; }
	}
}
