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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PX.Objects.CS.Services.WorkTimeCalculation
{
	internal class WorkTimeCalculator : IWorkTimeCalculator
	{
		private readonly CalendarInfo _calendar;

		public WorkTimeCalculator(CalendarInfo calendarInfo)
		{
			_calendar = calendarInfo;
		}

		public bool IsValid => DoesCalendarHaveCorrectWorkdayHours() && DoesCalendarHaveWorkingDays();

		public void Validate()
		{
			if (DoesCalendarHaveWorkingDays() is false)
				throw new PXInvalidOperationException(MessagesNoPrefix.CalendarIsNotFound, _calendar.CalendarID);
			if (DoesCalendarHaveCorrectWorkdayHours() is false)
				throw new PXInvalidOperationException(MessagesNoPrefix.CalendarDoesNotHaveWorkingDays, _calendar.CalendarID);
		}

		public WorkTimeSpan ToWorkTimeSpan(TimeSpan timeSpan)
		{
			Validate();

			return new WorkTimeSpan(_calendar.WorkdayHours, timeSpan);
		}

		public WorkTimeSpan ToWorkTimeSpan(WorkTimeInfo workTimeInfo)
		{
			Validate();

			return WorkTimeSpan.FromWorkdays(_calendar.WorkdayHours, workTimeInfo.Workdays, workTimeInfo.Hours, workTimeInfo.Minutes);
		}

		public DateTimeInfo AddWorkTime(DateTimeInfo startDateTime, WorkTimeSpan workTimeDiff)
		{
			Validate();
			AssertWorkTimeSpanIsPositive(workTimeDiff);

			var adder = new WorkTimeAdder(_calendar, startDateTime, workTimeDiff);
			while (!adder.IsFinished)
			{
				adder.MoveToClosesWorkday();
				adder.CalculateCurrentWorkday();
			}

			return adder.ResultDateTimeInfo.ToTimeZone(startDateTime.TimeZoneInfo);
		}

		private bool DoesCalendarHaveWorkingDays()
		{
			return _calendar.DaysOfWeek.Values.Any(d => d.IsWorkingDay);
		}

		private bool DoesCalendarHaveCorrectWorkdayHours()
		{
			return _calendar.WorkdayHours is > 0 and <= 24;
		}

		// todo: support time subtraction
		private void AssertWorkTimeSpanIsPositive(WorkTimeSpan workTime)
		{
			if (workTime.TimeSpan.Ticks < 0)
				// todo: message
				throw new PXNotSupportedException(Messages.NegativeWorkTime);
		}

		private static TimeSpan ToUtc(DateTime date, TimeSpan time, PXTimeZoneInfo originalTimeZone)
		{
			if (originalTimeZone == PXTimeZoneInfo.Invariant)
				return time;

			var resultDate = PXTimeZoneInfo.ConvertTimeToUtc(date.Add(time), originalTimeZone);
			return resultDate - date; // may be negative
		}

		private static TimeSpan FromUtc(DateTime date, TimeSpan time, PXTimeZoneInfo targetTimeZone)
		{
			if (targetTimeZone == PXTimeZoneInfo.Invariant)
				return time;

			var resultDate = PXTimeZoneInfo.ConvertTimeFromUtc(date.Add(time), targetTimeZone);

			return resultDate - date; // may be negative
		}

		private static TimeRange ToUtc(DateTime date, TimeRange timeRange, PXTimeZoneInfo originalTimeZone)
		{
			return new TimeRange(ToUtc(date, timeRange.Start, originalTimeZone), ToUtc(date, timeRange.End, originalTimeZone));
		}

		private struct WorkTimeAdder
		{
			private readonly CalendarInfo _calendar;
			private TimeSpan _time; // time in utc - adjusted from calendar timezone. may be < 0 and > 24h in that case
			private DayInfo _day;   // day in calendar timezone
			private TimeSpan _remainTime;

			public WorkTimeAdder(CalendarInfo calendar, DateTimeInfo startDateTime, WorkTimeSpan timeToAdd)
			{
				_calendar      = calendar;
				var utc        = startDateTime.ToUtc();
				var calendarDt = startDateTime.ToTimeZone(calendar.TimeZone);
				var date       = calendarDt.DateTime.Date;
				_time          = utc.DateTime - date;
				_day           = DayInfo.ConvertFrom(_calendar.TimeZone, date, _calendar.DaysOfWeek[date.DayOfWeek], _calendar.TryFindExceptionForDate(date));
				_remainTime    = timeToAdd.TimeSpan;
			}

			public DateTime ResultDateTime =>
				IsFinished
					? _day.Date.Add(FromUtc(_day.Date, _time, _calendar.TimeZone))
					// todo: message
					: throw new InvalidOperationException("ResultDateTime is not calculated.");

			public DateTimeInfo ResultDateTimeInfo => new (_calendar.TimeZone, ResultDateTime);

			public bool IsFinished => _remainTime <= TimeSpan.Zero;

			public void CalculateCurrentWorkday()
			{
				if (_remainTime <= TimeSpan.Zero || !_day.IsWorkingDay)
					return;

				MoveToWorkTime();

				if (IsCurrentDayOver())
					return;

				var remainWorkTime = GetRemainWorkTime();

				if (_remainTime >= remainWorkTime)
				{
					_time       =  GetEndOfDay();
					_remainTime -= remainWorkTime;
					return;
				}

				MoveToFinalTime();
			}

			public void MoveToClosesWorkday()
			{
				if (_remainTime <= TimeSpan.Zero)
					return;

				if (_day.IsWorkingDay && IsCurrentDayOver() is false)
					return;

				// little optimization to not create new DayInfo for not working days
				var date = _day.Date;

				DayOfWeekInfo          weekDay;
				CalendarExceptionInfo? exceptionInfo;
				do
				{
					date          = date.AddDays(1);
					weekDay       = _calendar.DaysOfWeek[date.DayOfWeek];
					exceptionInfo = _calendar.TryFindExceptionForDate(date);
				} while (!exceptionInfo?.IsWorkingDay ?? !weekDay.IsWorkingDay);

				_day  = DayInfo.ConvertFrom(_calendar.TimeZone, date, weekDay, exceptionInfo);
				_time = _day.TimeRange.Start;
			}

			private bool IsCurrentDayOver()
			{
				return _time >= GetEndOfDay();
			}

			private void MoveToWorkTime()
			{
				if (_time < _day.TimeRange.Start)
				{
					_time = _day.TimeRange.Start;
				}

				if (_day.BreakTimes.Count == 0)
					return;

				_time = GetTimeAfterBreakTime();
			}

			private void MoveToFinalTime()
			{
				if (_day.BreakTimes.Count != 0)
				{
					var time             = _time;
					var remainBreakTimes = _day.BreakTimes.Where(bt => bt.Start > time).OrderBy(bt => bt.Start);

					foreach (var breakTime in remainBreakTimes)
					{
						var diff = breakTime.Start - _time;
						if (_remainTime <= diff)
						{
							_time       += _remainTime;
							_remainTime =  TimeSpan.Zero;
							return;
						}

						_time       =  breakTime.End;
						_remainTime -= diff;
					}
				}

				_time       += _remainTime;
				_remainTime =  TimeSpan.Zero;
			}

			private TimeSpan GetTimeAfterBreakTime()
			{
				var time = _time; // lambda

				return
					_day
						.BreakTimes
						.Where(bt => bt.Start <= time && bt.End > time)
						.Select(bt => bt.End)
						.DefaultIfEmpty(time)
						.Max();
			}

			private TimeSpan GetRemainWorkTime()
			{
				if (_day.BreakTimes.Count == 0)
					return _day.TimeRange.End - _time;

				var remainBreakTime = GetRemainTimeBreakTimeDuration();

				return _day.TimeRange.End - _time - remainBreakTime;
			}

			private TimeSpan GetRemainTimeBreakTimeDuration()
			{
				if (_day.BreakTimes.Count == 0)
					return TimeSpan.Zero;

				var time = _time; // lambda
				var totalRemainBreakTime = TimeRange.GetDuration(_day.BreakTimes.Where(bt => bt.Start >= time));
				return totalRemainBreakTime - GetBreakTimeDurationAfterDayEnd();
			}

			private TimeSpan GetBreakTimeDurationAfterDayEnd()
			{
				var day = _day;
				return
					TimeRange.GetDuration(_day.BreakTimes.Where(bt => bt.Start >= day.TimeRange.End))
					+ new TimeSpan(
						_day.BreakTimes
							.Where(bt => bt.Start < day.TimeRange.End && bt.End > day.TimeRange.End)
							.Select(bt => bt.End - day.TimeRange.End)
							.Sum(t => t.Ticks));
			}

			private TimeSpan GetEndOfDay()
			{
				if (_day.BreakTimes.Count == 0)
					return _day.TimeRange.End;

				var end = _day.TimeRange.End;
				foreach (var breakTime in _day.BreakTimes)
				{
					if (breakTime.Start < _day.TimeRange.End
					 && breakTime.End >= _day.TimeRange.End
					 && breakTime.Start < end)
					{
						end = breakTime.Start;
					}
				}

				return end;
			}
		}

		private readonly struct DayInfo
		{
			public static DayInfo ConvertFrom(PXTimeZoneInfo originalTimeZone, DateTime date, DayOfWeekInfo dayOfWeek, CalendarExceptionInfo? exception)
			{
				if (exception is { IsWorkingDay: false })
				{
					return new(date, false, default, Array.Empty<TimeRange>());
				}

				var timeRange = exception?.TimeRange ?? dayOfWeek.TimeRange;
				var isWorkingDay = exception?.IsWorkingDay ?? dayOfWeek.IsWorkingDay;
				// todo: list creation optimization?
				return new DayInfo(
					date,
					isWorkingDay,
					ToUtc(date, timeRange, originalTimeZone),
					dayOfWeek.BreakTimes.Count == 0
						? (IReadOnlyList<TimeRange>) Array.Empty<TimeRange>()
						: dayOfWeek.BreakTimes.Select(bt => ToUtc(date, bt, originalTimeZone)).ToList()
				);
			}

			public DayInfo(DateTime date, bool isWorkingDay, TimeRange timeRange, IReadOnlyList<TimeRange> breakTimes)
			{
				Date         = date;
				IsWorkingDay = isWorkingDay;
				TimeRange    = timeRange;
				BreakTimes   = breakTimes;
			}

			public DateTime Date { get; }
			public bool IsWorkingDay { get; }
			public TimeRange TimeRange { get; }
			public IReadOnlyList<TimeRange> BreakTimes { get; }
		}
	}
}
