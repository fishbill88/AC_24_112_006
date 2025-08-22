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
	public readonly struct DayOfWeekInfo
	{
		public static DayOfWeekInfo NotWorkingDay(IReadOnlyCollection<TimeRange> breakTimes = null)
			=> new (false, TimeRange.Zero, breakTimes ?? Array.Empty<TimeRange>());

		private DayOfWeekInfo(bool isWorkingDay, TimeRange timeRange, IReadOnlyCollection<TimeRange> breakTimes)
		{
			IsWorkingDay      = isWorkingDay;
			TimeRange         = timeRange;
			BreakTimes        = MergeTimeRanges(breakTimes);
		}

		public DayOfWeekInfo(TimeRange timeRange, IReadOnlyCollection<TimeRange> breakTimes)
			: this (true, timeRange, breakTimes)
		{
		}


		public bool IsWorkingDay { get; }
		public TimeRange TimeRange { get; }
		public IReadOnlyCollection<TimeRange> BreakTimes { get; }

		// BreakTimes.Duration that intersects with TimeRange
		public TimeSpan GetBreakTimeDuration()
		{
			var duration = TimeSpan.Zero;

			foreach (var breakTime in BreakTimes)
			{
				duration += TimeRange.GetIntersection(breakTime).Duration;
			}

			return duration;
		}

		public static IReadOnlyCollection<TimeRange> MergeTimeRanges(IReadOnlyCollection<TimeRange> timeRanges)
		{
			var result = new List<TimeRange>(timeRanges);
			result.Sort((a, b) => a.Start.CompareTo(b.Start));
			for (int i = 0; i < result.Count - 1; i++)
			{
				if (result[i].IntersectsWith(result[i + 1]))
				{
					result[i] = result[i].MergeWith(result[i + 1]);
					result.RemoveAt(i + 1);
					i--;
				}
			}

			return result;
		}

	}
}
