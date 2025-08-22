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
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.CS.Services.WorkTimeCalculation
{
	[PXInternalUseOnly]
	public readonly struct TimeRange : IEquatable<TimeRange>
	{
		public static readonly TimeRange Zero = new(TimeSpan.Zero, TimeSpan.Zero);

		public TimeRange(TimeSpan start, TimeSpan end)
		{
			Start = start;
			End   = end;
		}

		public TimeSpan Start { get; }
		public TimeSpan End { get; }
		public TimeSpan Duration => End - Start;

		public bool IsWithinRange(TimeSpan time) => time >= Start && time < End;
		public bool IsWithinRange(TimeRange other) => IsWithinRange(other.Start) && IsWithinRange(other.End);

		public static TimeSpan GetDuration(IEnumerable<TimeRange> ranges)
		{
			return new TimeSpan(ranges.Select(d => d.Duration).Sum(r => r.Ticks));
		}

		public bool IntersectsWith(TimeRange other)
		{
			return IsWithinRange(other.Start) || other.IsWithinRange(End);
		}

		public TimeRange GetIntersection(TimeRange other)
		{
			if (IntersectsWith(other) is false)
				return TimeRange.Zero;
			return new TimeRange(
				new TimeSpan(Math.Max(Start.Ticks, other.Start.Ticks)),
				new TimeSpan(Math.Min(End.Ticks, other.End.Ticks)));
		}

		public TimeRange MergeWith(TimeRange other)
		{
			return new TimeRange(new TimeSpan(Math.Min(Start.Ticks, other.Start.Ticks)), new TimeSpan(Math.Max(End.Ticks, other.End.Ticks)));
		}

		public override string ToString()
		{
			return $"{Start} - {End}, Duration: {Duration}";
		}

		public bool Equals(TimeRange other)
		{
			return Start.Equals(other.Start) && End.Equals(other.End);
		}

		public override bool Equals(object obj)
		{
			return obj is TimeRange other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Start.GetHashCode() * 397) ^ End.GetHashCode();
			}
		}

		public static bool operator ==(TimeRange left, TimeRange right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(TimeRange left, TimeRange right)
		{
			return !left.Equals(right);
		}
	}
}
