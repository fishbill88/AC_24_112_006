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
using PX.Common;
using PX.Data.BQL.Fluent;
using System.Collections.Generic;
using System.Linq;
using System;
using PX.Data.SQLTree;

namespace PX.Objects.CS.Services.WorkTimeCalculation
{
	[PXInternalUseOnly]
	public static class WorkTimeCalculatorProvider
	{
		public static IWorkTimeCalculator GetWorkTimeCalculator(string calendarId)
		{
			var container = PXDatabase.GetSlot<WorkTimeCalendarsContainer>(typeof(WorkTimeCalendarsContainer).FullName, WorkTimeCalendarsContainer.Tables);

			if (container?.Calculators is { } c)
			{
				if (c.TryGetValue(calendarId, out var calculator))
				{
					return calculator;
				}

				throw new PXArgumentException(nameof(calendarId), MessagesNoPrefix.CalendarIsNotFound, calendarId);
			}

			throw new PXInvalidOperationException(MessagesNoPrefix.CalendarSlotWasNotInitialized);
		}


		public static CalendarInfo ConvertToCalendarInfo(
			CSCalendar calendar,
			IReadOnlyCollection<CSCalendarExceptions> exceptions,
			IReadOnlyCollection<CSCalendarBreakTime> breakTimes)
		{
			var breakTimesDict = new Dictionary<DayOfWeek, List<TimeRange>>(7);
			foreach (var dayOfWeek in Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>())
			{
				breakTimesDict[dayOfWeek] =
					breakTimes
						.Where(c => c.DayOfWeek == (int)dayOfWeek || c.DayOfWeek == CSCalendarBreakTime.dayOfWeek.All)
						.Select
						(
							c => new TimeRange
							(
								DateTimeToTimeSpan(c.StartTime),
								DateTimeToTimeSpan(c.EndTime)
							)
						)
						.OrderBy(c => c.Start)
						.ToList();
			}

			var daysOfWeek = new Dictionary<DayOfWeek, DayOfWeekInfo>
			{
				[DayOfWeek.Sunday]    = GetDayOfWeek(calendar.SunWorkDay, calendar.SunStartTime, calendar.SunEndTime, breakTimesDict[DayOfWeek.Sunday]),
				[DayOfWeek.Monday]    = GetDayOfWeek(calendar.MonWorkDay, calendar.MonStartTime, calendar.MonEndTime, breakTimesDict[DayOfWeek.Monday]),
				[DayOfWeek.Tuesday]   = GetDayOfWeek(calendar.TueWorkDay, calendar.TueStartTime, calendar.TueEndTime, breakTimesDict[DayOfWeek.Tuesday]),
				[DayOfWeek.Wednesday] = GetDayOfWeek(calendar.WedWorkDay, calendar.WedStartTime, calendar.WedEndTime, breakTimesDict[DayOfWeek.Wednesday]),
				[DayOfWeek.Thursday]  = GetDayOfWeek(calendar.ThuWorkDay, calendar.ThuStartTime, calendar.ThuEndTime, breakTimesDict[DayOfWeek.Thursday]),
				[DayOfWeek.Friday]    = GetDayOfWeek(calendar.FriWorkDay, calendar.FriStartTime, calendar.FriEndTime, breakTimesDict[DayOfWeek.Friday]),
				[DayOfWeek.Saturday]  = GetDayOfWeek(calendar.SatWorkDay, calendar.SatStartTime, calendar.SatEndTime, breakTimesDict[DayOfWeek.Saturday]),
			};

			var calendarExceptions = exceptions
				.Select(e => new CalendarExceptionInfo(e.Date!.Value, DateTimesToTimeRange(e.StartTime, e.EndTime), e.WorkDay is true))
				.ToList(exceptions.Count);

			return new CalendarInfo(calendar.CalendarID, GetCalendarTimeZone(calendar.TimeZone), calendar.WorkdayTime!.Value / 60d, daysOfWeek, calendarExceptions);


			DayOfWeekInfo GetDayOfWeek(bool? isWorkingDay, DateTime? startTime, DateTime? endTime, IReadOnlyList<TimeRange> breakTimesRanges)
			{
				if (isWorkingDay is true)
					return new DayOfWeekInfo(DateTimesToTimeRange(startTime, endTime), breakTimesRanges);

				return DayOfWeekInfo.NotWorkingDay(breakTimesRanges);
			}

			PXTimeZoneInfo GetCalendarTimeZone(string timeZoneId)
			{
				return PXTimeZoneInfo.FindSystemTimeZoneById(timeZoneId) ?? PXTimeZoneInfo.Invariant;
			}
		}


		public static TimeSpan DateTimeToTimeSpan(DateTime? value) => value?.TimeOfDay ?? TimeSpan.Zero;

		public static TimeRange DateTimesToTimeRange(DateTime? start, DateTime? end)
		{
			var st = DateTimeToTimeSpan(start);
			var ed = DateTimeToTimeSpan(end);

			if (ed == TimeSpan.Zero)
				ed = TimeSpan.FromHours(24);

			return new TimeRange(st, ed);
		}

		internal class WorkTimeCalendarsContainer : IPrefetchable
		{
			public Dictionary<string, IWorkTimeCalculator> Calculators { get; private set; }

			public static Type[] Tables  => new[]
			{
				typeof(CSCalendar),
				typeof(CSCalendarExceptions),
				typeof(CSCalendarBreakTime)
			};

			public void Prefetch()
			{
				var calendars  = PXDatabase.Select<CSCalendar>().ToList();
				var exceptions = PXDatabase.Select<CSCalendarExceptions>().ToList();
				var breakTimes = PXDatabase.Select<CSCalendarBreakTime>().ToList();

				Calculators = new (calendars.Count);

				foreach (var calendar in calendars)
				{
					var exceptionsForCalendar = exceptions.Where(e => e.CalendarID == calendar.CalendarID).ToList();
					var breakTimesForCalendar = breakTimes.Where(b => b.CalendarID == calendar.CalendarID).ToList();
					var calculator            = new WorkTimeCalculator(ConvertToCalendarInfo(calendar, exceptionsForCalendar, breakTimesForCalendar));

					Calculators[calendar.CalendarID] = calculator;
				}
			}
		}
	}
}
