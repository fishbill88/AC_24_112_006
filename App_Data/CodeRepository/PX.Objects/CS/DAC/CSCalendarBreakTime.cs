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
using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;

namespace PX.Objects.CS
{
	/// <summary>
	/// The table for the data that ia shown on the "Break Times" tab of the Work Calendar (CS209000) form.
	/// The data drives break time information for use in production scheduling.
	/// A work calendar can have zero to many break time records.
	/// </summary>
	/// <remarks>
	///	The parent is <see cref = "CSCalendar"/>.
	/// </remarks>
	[Serializable]
	[PXCacheName("Calendar Break Time")]
	public class CSCalendarBreakTime : PXBqlTable, IBqlTable
	{
		#region Keys

		public class PK : PrimaryKeyOf<CSCalendarBreakTime>.By<calendarID, dayOfWeek, startTime>
		{
			public static CSCalendarBreakTime Find(PXGraph graph, string calendarID, int? dayOfWeek, DateTime? startTime, PKFindOptions options = PKFindOptions.None) => FindBy(graph, calendarID, dayOfWeek, startTime, options);
		}

		public static class FK
		{
			public class CSCalendar : CS.CSCalendar.PK.ForeignKeyOf<CSCalendarBreakTime>.By<calendarID> { }
		}

		#endregion

		#region CalendarID (key)

		/// <summary>
		/// The work calendar identifier.
		/// This field is a key field.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="CSCalendar.CalendarID"/> field.
		/// </value>
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(CSCalendar.calendarID))]
		[PXUIField(DisplayName = "Calendar ID", Enabled = false, Visible = false)]
		[PXParent(typeof(Select<CSCalendar, Where<CSCalendar.calendarID, Equal<Current<CSCalendarBreakTime.calendarID>>>>))]
		public virtual String CalendarID { get; set; }
		public abstract class calendarID : BqlString.Field<calendarID> { }

		#endregion

		#region DayOfWeek (key)

		/// <summary>
		/// The field contains an option how the break period is applied to the calendar.
		/// This field is a key field.
		/// </summary>
		/// <value>
		/// The field can have one of the values listed in the <see cref="dayOfWeek.List"/> class.
		/// The default value is <see cref="dayOfWeek.All"/>.
		/// </value>
		[PXDBInt(IsKey = true)]
		[PXDefault(dayOfWeek.All)]
		[PXUIField(DisplayName = "Day Of Week")]
		[dayOfWeek.List]
		public virtual Int32? DayOfWeek { get; set; }

		public abstract class dayOfWeek : BqlInt.Field<dayOfWeek>
		{
			/// <summary>
			/// Indicates all day of week days apply (value = 10)
			/// </summary>
			public const int All = 10;
			public class all : BqlInt.Constant<all>
			{
				public all() : base(All) { }
			}

			public const int Sunday = 0;
			public class sunday : BqlInt.Constant<sunday>
			{
				public sunday() : base(Sunday) { }
			}

			public const int Monday = 1;
			public class monday : BqlInt.Constant<monday>
			{
				public monday() : base(Monday) { }
			}

			public const int Tuesday = 2;
			public class tuesday : BqlInt.Constant<tuesday>
			{
				public tuesday() : base(Tuesday) { }
			}

			public const int Wednesday = 3;
			public class wednesday : BqlInt.Constant<wednesday>
			{
				public wednesday() : base(Wednesday) { }
			}

			public const int Thursday = 4;
			public class thursday : BqlInt.Constant<thursday>
			{
				public thursday() : base(Thursday) { }
			}

			public const int Friday = 5;
			public class friday : BqlInt.Constant<friday>
			{
				public friday() : base(Friday) { }
			}

			public const int Saturday = 6;
			public class saturday : BqlInt.Constant<saturday>
			{
				public saturday() : base(Saturday) { }
			}


			/// <summary>
			/// List following standard date enum DayOfWeek (0-6 for Sunday-Saturday) plus an "All" for al day of weeks.
			/// </summary>
			public class List : PXIntListAttribute
			{
				public List() : base(
					new int[] { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, All },
					new string[] { EP.Messages.Sunday, EP.Messages.Monday, EP.Messages.Tuesday, EP.Messages.Wednesday, EP.Messages.Thursday, EP.Messages.Friday, EP.Messages.Saturday, Messages.All })
				{ }
			}
		}

		#endregion

		#region StartTime (key)

		/// <summary>
		/// Start time of break period.
		/// </summary>
		[PXDBTime(DisplayMask = "t", UseTimeZone = false, IsKey = true)]
		[PXUIField(DisplayName = "Start Time")]
		[PXDefault]
		public virtual DateTime? StartTime { get; set; }
		public abstract class startTime : BqlDateTime.Field<startTime> { }

		#endregion

		#region EndTime

		/// <summary>
		/// End time of the break period.
		/// </summary>
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXUIField(DisplayName = "End Time")]
		[PXDefault]
		public virtual DateTime? EndTime { get; set; }
		public abstract class endTime : BqlDateTime.Field<endTime> { }

		#endregion

		#region BreakTime

		/// <summary>
		/// Amount of time (in minutes) of the break period.
		/// </summary>
		/// <value>
		/// <see cref="EndTime">End Time</see> - <see cref="StartTime">Start Time</see>.
		/// </value>
		[PXDBTimeSpanLong(Format = TimeSpanFormatType.ShortHoursMinutesCompact)]
		[breakTime.Default]
		[PXUIField(DisplayName = "Break Duration", Enabled = false)]
		[PXFormula(typeof(Default<CSCalendarBreakTime.startTime, CSCalendarBreakTime.endTime>))]
		public virtual Int32? BreakTime { get; set; }

		public abstract class breakTime : BqlInt.Field<breakTime>
		{
			public class DefaultAttribute : PXDefaultAttribute
			{
				public override void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
				{
					var row = (CSCalendarBreakTime)e.Row;
					if (row == null)
					{
						e.NewValue = 0;
						return;
					}

					e.NewValue = Convert.ToInt32(GetTotalMinutes(row.StartTime, row.EndTime));
				}

				public static double GetTotalMinutes(DateTime? startDateTime, DateTime? endDatetime)
				{
					if (startDateTime == null || endDatetime == null)
					{
						return 0;
					}

					return Math.Max((endDatetime.GetValueOrDefault() - startDateTime.GetValueOrDefault()).TotalMinutes, 0);
				}

			}
		}

		#endregion

		#region Description

		/// <summary>
		/// Description of the break period.
		/// </summary>
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		public virtual String Description { get; set; }
		public abstract class description : BqlString.Field<description> { }

		#endregion

		#region LastModifiedByID

		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID { get; set; }
		public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }

		#endregion

		#region LastModifiedByScreenID

		public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }

		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID { get; set; }

		#endregion

		#region LastModifiedDateTime

		public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }

		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime { get; set; }

		#endregion

		#region tstamp

		[PXDBTimestamp()]
		public virtual Byte[] tstamp { get; set; }
		public abstract class Tstamp : BqlByteArray.Field<Tstamp> { }

		#endregion
	}
}
