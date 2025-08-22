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
	public readonly struct WorkTimeSpan : IEquatable<WorkTimeSpan>
	{
		private const int MinutesInHour = 60;
		public TimeSpan TimeSpan { get; }
		public double HoursInWorkday { get; }

		public double TotalWorkdays => TotalHours / HoursInWorkday;
		public double TotalHours => TimeSpan.TotalHours;
		public double TotalMinutes => TimeSpan.TotalMinutes;

		public double Workdays => TotalWorkdays;
		public double Hours => TotalHours % HoursInWorkday;
		public double Minutes => Hours % 1 * MinutesInHour;

		public int RoundWorkdays => GetRoundTime().workdays;
		public int RoundHours => GetRoundTime().hours;
		public int RoundMinutes => GetRoundTime().minutes;

		private int LastHourMinutes => (int)(HoursInWorkday % 1 * MinutesInHour);

		private (int workdays, int hours, int minutes) GetRoundTime()
		{
			var minutes = (int) Math.Round(Minutes, 0, MidpointRounding.AwayFromZero);
			var hours    = (int) Hours;
			var workdays = (int) Workdays;
			bool isLastHour = Hours >= (int)HoursInWorkday;
			bool isLastMinuteOfLastHour = LastHourMinutes != 0 && isLastHour && minutes == LastHourMinutes;
			if (minutes < MinutesInHour && !isLastMinuteOfLastHour)
			{
				return (workdays, hours, minutes);
			}

			// round up
			minutes = 0;
			hours++;
			if (hours >= HoursInWorkday)
			{
				hours = 0;
				workdays++;
			}

			return (workdays, hours, minutes);
		}

		public WorkTimeSpan Duration()
		{
			if (TimeSpan.Ticks >= 0)
				return this;
			return Negate();
		}

		public WorkTimeSpan Negate()
		{
			return new WorkTimeSpan(HoursInWorkday, TimeSpan.Negate());
		}
		
		public override string ToString()
		{
			return $"{RoundWorkdays}d {RoundHours}h {RoundMinutes}m, {HoursInWorkday:0.00}h/d";
		}

		#region Constructors

		public WorkTimeSpan(double hoursInWorkday, TimeSpan timeSpan)
		{
			if (hoursInWorkday is <= 0 or > 24)
				throw new ArgumentOutOfRangeException(
					nameof(hoursInWorkday),
					hoursInWorkday,
					// todo: message
					"Hours in workday must be in range (0, 24]");

			HoursInWorkday = hoursInWorkday;
			TimeSpan       = timeSpan;
		}


		public static WorkTimeSpan FromWorkdays(double hoursInWorkday, int workdays, int hours, int minutes)
		{
			return new WorkTimeSpan(hoursInWorkday, new TimeSpan(0, hours, minutes, 0, 0) + TimeSpan.FromHours(workdays * hoursInWorkday));
		}

		public static WorkTimeSpan FromWorkdays(double hoursInWorkday, double businessDays)
		{
			return new WorkTimeSpan(hoursInWorkday, TimeSpan.FromHours(businessDays * hoursInWorkday));
		}

		public static WorkTimeSpan FromRealDays(double hoursInWorkday, int realDays, int hours, int minutes)
		{
			return new WorkTimeSpan(hoursInWorkday, new TimeSpan(realDays, hours, minutes, 0, 0));
		}

		public static WorkTimeSpan FromRealDays(double hoursInWorkday, double realDays)
		{
			return new WorkTimeSpan(hoursInWorkday, TimeSpan.FromDays(realDays));
		}

		public static WorkTimeSpan FromHours(double hoursInWorkday, double hours)
		{
			return new WorkTimeSpan(hoursInWorkday, TimeSpan.FromHours(hours));
		}

		public static WorkTimeSpan FromMinutes(double hoursInWorkday, double minutes)
		{
			return new WorkTimeSpan(hoursInWorkday, TimeSpan.FromMinutes(minutes));
		}

		#endregion

		#region Equality

		public static IEqualityComparer<WorkTimeSpan> EqualityComparer { get; } = new WorkTimeSpanComparer();

		public bool Equals(WorkTimeSpan other)
		{
			return EqualityComparer.Equals(this, other);
		}

		public override bool Equals(object obj)
		{
			return obj is WorkTimeSpan other && Equals(other);
		}

		public override int GetHashCode()
		{
			return EqualityComparer.GetHashCode(this);
		}

		public static bool operator ==(WorkTimeSpan left, WorkTimeSpan right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(WorkTimeSpan left, WorkTimeSpan right)
		{
			return !left.Equals(right);
		}

		private sealed class WorkTimeSpanComparer : IEqualityComparer<WorkTimeSpan>
		{
			public bool Equals(WorkTimeSpan x, WorkTimeSpan y)
			{
				return x.TimeSpan.Equals(y.TimeSpan) && x.HoursInWorkday.Equals(y.HoursInWorkday);
			}

			public int GetHashCode(WorkTimeSpan obj)
			{
				unchecked
				{
					return (obj.TimeSpan.GetHashCode() * 397) ^ obj.HoursInWorkday.GetHashCode();
				}
			}
		}

		#endregion
	}
}
