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
	public readonly struct WorkTimeInfo : IEquatable<WorkTimeInfo>
	{
		public WorkTimeInfo(int workdays, int hours, int minutes)
		{
			Workdays = workdays;
			Hours    = hours;
			Minutes  = minutes;
		}

		public int Workdays { get; }
		public int Hours { get; }
		public int Minutes { get; }

		public bool Equals(WorkTimeInfo other)
		{
			return Workdays == other.Workdays && Hours == other.Hours && Minutes == other.Minutes;
		}

		public override bool Equals(object obj)
		{
			return obj is WorkTimeInfo other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Workdays;
				hashCode = (hashCode * 397) ^ Hours;
				hashCode = (hashCode * 397) ^ Minutes;
				return hashCode;
			}
		}
	}
}
