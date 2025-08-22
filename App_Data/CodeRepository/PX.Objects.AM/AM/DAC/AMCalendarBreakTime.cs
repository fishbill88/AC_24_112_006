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
using System.Diagnostics;
using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;

namespace PX.Objects.AM
{
	/// <summary>
	/// The DAC whose data is shown on the Break Times tab of the Work Calendar (CS209000) form via the <see cref="GraphExtensions.CSCalendarMaintAMExtension"/> graph extension. The data drives break time information for use in production scheduling. A work calendar can have zero to many break time records.
	///	Parent: <see cref = "CSCalendar"/>
	/// </summary>
	[Serializable]
	[PXCacheName("Calendar Break Time")]
#if DEBUG
	[DebuggerDisplay(
		"DayOfWeek={DayOfWeek}, StartTime={StartTime.GetValueOrDefault().ToShortTimeString()}, EndTime={EndTime.GetValueOrDefault().ToShortTimeString()}, CalendarID={CalendarID}")]
#endif
	public class AMCalendarBreakTime : CSCalendarBreakTime
	{
		#region Keys

		public new class PK : PrimaryKeyOf<AMCalendarBreakTime>.By<calendarID, dayOfWeek, startTime>
		{
			public static AMCalendarBreakTime Find(PXGraph graph, string calendarID, int? dayOfWeek, DateTime? startTime, PKFindOptions options = PKFindOptions.None) =>
				FindBy(graph, calendarID, dayOfWeek, startTime, options);
		}

		public new static class FK
		{
			public class CSCalendar : CS.CSCalendar.PK.ForeignKeyOf<AMCalendarBreakTime>.By<calendarID>
			{
			}
		}

		#endregion

		#region Fields

		public new abstract class calendarID : BqlString.Field<calendarID> { }
		public new abstract class dayOfWeek : BqlInt.Field<dayOfWeek> {}
		public new abstract class startTime : BqlDateTime.Field<startTime> { }
		public new abstract class endTime : BqlDateTime.Field<endTime> { }
		public new abstract class breakTime : BqlInt.Field<breakTime> {}
		public new abstract class description : BqlString.Field<description> { }

		#endregion
	}
}
