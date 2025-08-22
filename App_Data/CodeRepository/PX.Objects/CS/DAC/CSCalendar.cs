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
	[Serializable]
	[PXPrimaryGraph(
		new Type[] { typeof(CSCalendarMaint) },
		new Type[] { typeof(Select<CSCalendar,
			Where<CSCalendar.calendarID, Equal<Current<CSCalendar.calendarID>>>>)
		})]
	[PXCacheName(Messages.Calendar)]
	public partial class CSCalendar : PXBqlTable, PX.Data.IBqlTable
	{
		#region Keys

		public class PK : PrimaryKeyOf<CSCalendar>.By<calendarID>
		{
			public static CSCalendar Find(PXGraph graph, string calendarID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, calendarID, options);
		}

		#endregion

		#region CalendarID

		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Calendar ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<CSCalendar.calendarID>), DescriptionField = typeof(CSCalendar.description))]
		[PXReferentialIntegrityCheck]
		public virtual string CalendarID { get; set; }

		public abstract class calendarID : BqlString.Field<calendarID>
		{
			public const string DefaultCalendarID = "24H7WD";

			public class defaultCalendarID : BqlString.Constant<defaultCalendarID>
			{
				public defaultCalendarID() : base(DefaultCalendarID) { }
			}
		}

		#endregion

		#region Description

		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string Description { get; set; }
		public abstract class description : BqlString.Field<description> { }

		#endregion

		#region WorkdayTime

		/// <summary>
		/// Average amount of working time for this calendar.
		/// </summary>
		/// <remarks>
		/// Shown in UI as time: hh mm, persisted in database as minutes.
		/// </remarks>
		[CSDBTimeSpanShortWith24Hours]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Workday Hours")]
		[PXUIEnabled(typeof(workdayTimeOverride.IsEqual<True>))]
		public virtual int? WorkdayTime { get; set; }
		public abstract class workdayTime : BqlInt.Field<workdayTime> { }

		#endregion

		#region WorkdayTimeOverride

		/// <summary>
		/// This flag indicates whether the user can set <see cref="WorkdayTime">Workday Time</see> manually.
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override")]
		public virtual bool? WorkdayTimeOverride { get; set; }
		public abstract class workdayTimeOverride : BqlBool.Field<workdayTimeOverride> { }

		#endregion

		#region TimeZone

		[PXDBString(32)]
		[PXUIField(DisplayName = "Time Zone")]
		[PXTimeZone]
		public virtual string TimeZone { get; set; }
		public abstract class timeZone : BqlString.Field<timeZone> { }

		#endregion


		// Sunday

		#region SunWorkDay

		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Sunday")]
		public virtual bool? SunWorkDay { get; set; }
		public abstract class sunWorkDay : BqlBool.Field<sunWorkDay> { }

		#endregion

		#region SunStartTime

		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 09:00:00")]
		[PXUIField(DisplayName = "Sunday Start Time", Required = false)]
		public virtual DateTime? SunStartTime { get; set; }
		public abstract class sunStartTime : BqlDateTime.Field<sunStartTime> { }

		#endregion

		#region SunEndTime

		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 18:00:00")]
		[PXUIField(DisplayName = "Sunday End Time", Required = false)]
		public virtual DateTime? SunEndTime { get; set; }
		public abstract class sunEndTime : BqlDateTime.Field<sunEndTime> { }

		#endregion

		#region SunGoodsMoves

		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = " ")]
		public virtual bool? SunGoodsMoves { get; set; }
		public abstract class sunGoodsMoves : BqlBool.Field<sunGoodsMoves> { }

		#endregion

		#region SunUnpaidTime

		[PXDBTimeSpanLong(Format = TimeSpanFormatType.ShortHoursMinutesCompact)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Sun Unpaid Break Time", Enabled = false)]
		public virtual int? SunUnpaidTime { get; set; }
		public abstract class sunUnpaidTime : Data.BQL.BqlInt.Field<sunUnpaidTime> { }

		#endregion

		#region SunWorkTime

		/// <summary>
		/// Working time on Sunday.
		/// </summary>
		/// <value>
		/// <see cref="SunEndTime">End Time on Sunday</see> - <see cref="SunStartTime">Start Time on Sunday</see> - <see cref="SunUnpaidTime">Sum of Break Times on Sunday</see>.
		/// Value represented in minutes.
		/// </value>
		[CSTimeSpanShortWith24Hours]
		[PXUIField(DisplayName = "Sun Hours Worked", Enabled = false)]
		public virtual int? SunWorkTime { get; set; }
		public abstract class sunWorkTime : Data.BQL.BqlInt.Field<sunWorkTime> { }

		#endregion


		// Monday

		#region MonWorkDay

		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Monday")]
		public virtual bool? MonWorkDay { get; set; }
		public abstract class monWorkDay : BqlBool.Field<monWorkDay> { }

		#endregion

		#region MonStartTime
		
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 09:00:00")]
		[PXUIField(DisplayName = "Monday Start Time")]
		public virtual DateTime? MonStartTime { get; set; }
		public abstract class monStartTime : BqlDateTime.Field<monStartTime> { }

		#endregion

		#region MonEndTime
		
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 18:00:00")]
		[PXUIField(DisplayName = "Monday End Time")]
		public virtual DateTime? MonEndTime { get; set; }
		public abstract class monEndTime : BqlDateTime.Field<monEndTime> { }

		#endregion

		#region MonGoodsMoves
		
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = " ")]
		public virtual bool? MonGoodsMoves { get; set; }
		public abstract class monGoodsMoves : BqlBool.Field<monGoodsMoves> { }

		#endregion

		#region MonUnpaidTime

		[PXDBTimeSpanLong(Format = TimeSpanFormatType.ShortHoursMinutesCompact)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Mon Unpaid Break Time", Enabled = false)]
		public virtual int? MonUnpaidTime { get; set; }
		public abstract class monUnpaidTime : Data.BQL.BqlInt.Field<monUnpaidTime> { }

		#endregion

		#region MonWorkTime

		/// <summary>
		/// Working time on Monday.
		/// </summary>
		/// <value>
		/// <see cref="MonEndTime">End Time on Monday</see> - <see cref="MonStartTime">Start Time on Monday</see> - <see cref="MonUnpaidTime">Sum of Break Times on Monday</see>.
		/// Value represented in minutes.
		/// </value>
		[CSTimeSpanShortWith24Hours]
		[PXUIField(DisplayName = "Mon Hours Worked", Enabled = false)]
		public virtual int? MonWorkTime { get; set; }
		public abstract class monWorkTime : Data.BQL.BqlInt.Field<monWorkTime> { }

		#endregion


		// Tuesday

		#region TueWorkDay

		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Tuesday")]
		public virtual bool? TueWorkDay { get; set; }
		public abstract class tueWorkDay : BqlBool.Field<tueWorkDay> { }

		#endregion

		#region TueStartTime
		
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 09:00:00")]
		[PXUIField(DisplayName = "Tuesday Start Time")]
		public virtual DateTime? TueStartTime { get; set; }
		public abstract class tueStartTime : BqlDateTime.Field<tueStartTime> { }

		#endregion

		#region TueEndTime
		
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 18:00:00")]
		[PXUIField(DisplayName = "Tuesday End Time")]
		public virtual DateTime? TueEndTime { get; set; }
		public abstract class tueEndTime : BqlDateTime.Field<tueEndTime> { }

		#endregion

		#region TueGoodsMoves
		
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = " ")]
		public virtual bool? TueGoodsMoves { get; set; }
		public abstract class tueGoodsMoves : BqlBool.Field<tueGoodsMoves> { }

		#endregion

		#region TueUnpaidTime

		[PXDBTimeSpanLong(Format = TimeSpanFormatType.ShortHoursMinutesCompact)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Tue Unpaid Break Time", Enabled = false)]
		public virtual int? TueUnpaidTime { get; set; }
		public abstract class tueUnpaidTime : Data.BQL.BqlInt.Field<tueUnpaidTime> { }

		#endregion

		#region TueWorkTime

		/// <summary>
		/// Working time on Tuesday.
		/// </summary>
		/// <value>
		/// <see cref="TueEndTime">End Time on Tuesday</see> - <see cref="TueStartTime">Start Time on Tuesday</see> - <see cref="TueUnpaidTime">Sum of Break Times on Tuesday</see>.
		/// Value represented in minutes.
		/// </value>
		[CSTimeSpanShortWith24Hours]
		[PXUIField(DisplayName = "Tue Hours Worked", Enabled = false)]
		public virtual int? TueWorkTime { get; set; }
		public abstract class tueWorkTime : Data.BQL.BqlInt.Field<tueWorkTime> { }

		#endregion


		// Wednesday

		#region WedWorkDay

		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Wednesday")]
		public virtual bool? WedWorkDay { get; set; }
		public abstract class wedWorkDay : BqlBool.Field<wedWorkDay> { }

		#endregion

		#region WedStartTime
		
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 09:00:00")]
		[PXUIField(DisplayName = "Wednesday Start Time")]
		public virtual DateTime? WedStartTime { get; set; }
		public abstract class wedStartTime : BqlDateTime.Field<wedStartTime> { }

		#endregion

		#region WedEndTime
		
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 18:00:00")]
		[PXUIField(DisplayName = "Wednesday End Time")]
		public virtual DateTime? WedEndTime { get; set; }
		public abstract class wedEndTime : BqlDateTime.Field<wedEndTime> { }

		#endregion

		#region WedGoodsMoves
		
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = " ")]
		public virtual bool? WedGoodsMoves { get; set; }
		public abstract class wedGoodsMoves : BqlBool.Field<wedGoodsMoves> { }

		#endregion

		#region WedUnpaidTime

		[PXDBTimeSpanLong(Format = TimeSpanFormatType.ShortHoursMinutesCompact)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Wed Unpaid Break Time", Enabled = false)]
		public virtual int? WedUnpaidTime { get; set; }
		public abstract class wedUnpaidTime : Data.BQL.BqlInt.Field<wedUnpaidTime> { }

		#endregion

		#region WedWorkTime

		/// <summary>
		/// Working time on Wednesday.
		/// </summary>
		/// <value>
		/// <see cref="WedEndTime">End Time on Wednesday</see> - <see cref="WedStartTime">Start Time on Wednesday</see> - <see cref="WedUnpaidTime">Sum of Break Times on Wednesday</see>.
		/// Value represented in minutes.
		/// </value>
		[CSTimeSpanShortWith24Hours]
		[PXUIField(DisplayName = "Wed Hours Worked", Enabled = false)]
		public virtual int? WedWorkTime { get; set; }
		public abstract class wedWorkTime : Data.BQL.BqlInt.Field<wedWorkTime> { }

		#endregion


		// Thursday

		#region ThuWorkDay

		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Thursday")]
		public virtual bool? ThuWorkDay { get; set; }
		public abstract class thuWorkDay : BqlBool.Field<thuWorkDay> { }

		#endregion

		#region ThuStartTime
		
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 09:00:00")]
		[PXUIField(DisplayName = "Thursday Start Time")]
		public virtual DateTime? ThuStartTime { get; set; }
		public abstract class thuStartTime : BqlDateTime.Field<thuStartTime> { }

		#endregion

		#region ThuEndTime
		
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 18:00:00")]
		[PXUIField(DisplayName = "Thursday End Time")]
		public virtual DateTime? ThuEndTime { get; set; }
		public abstract class thuEndTime : BqlDateTime.Field<thuEndTime> { }

		#endregion

		#region ThuGoodsMoves
		
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = " ")]
		public virtual bool? ThuGoodsMoves { get; set; }
		public abstract class thuGoodsMoves : BqlBool.Field<thuGoodsMoves> { }

		#endregion

		#region ThuUnpaidTime

		[PXDBTimeSpanLong(Format = TimeSpanFormatType.ShortHoursMinutesCompact)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Thu Unpaid Break Time", Enabled = false)]
		public virtual int? ThuUnpaidTime { get; set; }
		public abstract class thuUnpaidTime : Data.BQL.BqlInt.Field<thuUnpaidTime> { }

		#endregion

		#region ThuWorkTime

		/// <summary>
		/// Working time on Thursday.
		/// </summary>
		/// <value>
		/// <see cref="ThuEndTime">End Time on Thursday</see> - <see cref="ThuStartTime">Start Time on Thursday</see> - <see cref="ThuUnpaidTime">Sum of Break Times on Thursday</see>.
		/// Value represented in minutes.
		/// </value>
		[CSTimeSpanShortWith24Hours]
		[PXUIField(DisplayName = "Thu Hours Worked", Enabled = false)]
		public virtual int? ThuWorkTime { get; set; }
		public abstract class thuWorkTime : Data.BQL.BqlInt.Field<thuWorkTime> { }

		#endregion


		// Friday

		#region FriWorkDay

		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Friday")]
		public virtual bool? FriWorkDay { get; set; }
		public abstract class friWorkDay : BqlBool.Field<friWorkDay> { }

		#endregion

		#region FriStartTime
		
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 09:00:00")]
		[PXUIField(DisplayName = "Friday Start Time")]
		public virtual DateTime? FriStartTime { get; set; }
		public abstract class friStartTime : BqlDateTime.Field<friStartTime> { }

		#endregion

		#region FriEndTime
		
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 18:00:00")]
		[PXUIField(DisplayName = "Friday End Time")]
		public virtual DateTime? FriEndTime { get; set; }
		public abstract class friEndTime : BqlDateTime.Field<friEndTime> { }

		#endregion

		#region FriGoodsMoves
		
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = " ")]
		public virtual bool? FriGoodsMoves { get; set; }
		public abstract class friGoodsMoves : BqlBool.Field<friGoodsMoves> { }

		#endregion

		#region FriUnpaidTime

		[PXDBTimeSpanLong(Format = TimeSpanFormatType.ShortHoursMinutesCompact)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Fri Unpaid Break Time", Enabled = false)]
		public virtual int? FriUnpaidTime { get; set; }
		public abstract class friUnpaidTime : Data.BQL.BqlInt.Field<friUnpaidTime> { }

		#endregion

		#region FriWorkTime

		/// <summary>
		/// Working time on Friday.
		/// </summary>
		/// <value>
		/// <see cref="FriEndTime">End Time on Friday</see> - <see cref="FriStartTime">Start Time on Friday</see> - <see cref="FriUnpaidTime">Sum of Break Times on Friday</see>.
		/// Value represented in minutes.
		/// </value>
		[CSTimeSpanShortWith24Hours]
		[PXUIField(DisplayName = "Fri Hours Worked", Enabled = false)]
		public virtual int? FriWorkTime { get; set; }
		public abstract class friWorkTime : Data.BQL.BqlInt.Field<friWorkTime> { }

		#endregion


		// Saturday

		#region SatWorkDay

		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Saturday")]
		public virtual bool? SatWorkDay { get; set; }
		public abstract class satWorkDay : BqlBool.Field<satWorkDay> { }

		#endregion

		#region SatStartTime
		
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 09:00:00")]
		[PXUIField(DisplayName = "Saturday Start Time", Required = false)]
		public virtual DateTime? SatStartTime { get; set; }
		public abstract class satStartTime : BqlDateTime.Field<satStartTime> { }

		#endregion

		#region SatEndTime

		public abstract class satEndTime : BqlDateTime.Field<satEndTime> { }
		
		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 18:00:00")]
		[PXUIField(DisplayName = "Saturday End Time", Required = false)]
		public virtual DateTime? SatEndTime { get; set; }

		#endregion

		#region SatGoodsMoves
		
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = " ")]
		public virtual bool? SatGoodsMoves { get; set; }
		public abstract class satGoodsMoves : BqlBool.Field<satGoodsMoves> { }

		#endregion

		#region SatUnpaidTime

		[PXDBTimeSpanLong(Format = TimeSpanFormatType.ShortHoursMinutesCompact)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Sat Unpaid Break Time", Enabled = false)]
		public virtual int? SatUnpaidTime { get; set; }
		public abstract class satUnpaidTime : Data.BQL.BqlInt.Field<satUnpaidTime> { }

		#endregion

		#region SatWorkTime

		/// <summary>
		/// Working time on Saturday.
		/// </summary>
		/// <value>
		/// <see cref="SatEndTime">End Time on Saturday</see> - <see cref="SatStartTime">Start Time on Saturday</see> - <see cref="SatUnpaidTime">Sum of Break Times on Saturday</see>.
		/// Value represented in minutes.
		/// </value>
		[CSTimeSpanShortWith24Hours]
		[PXUIField(DisplayName = "Sat Hours Worked", Enabled = false)]
		public virtual int? SatWorkTime { get; set; }
		public abstract class satWorkTime : Data.BQL.BqlInt.Field<satWorkTime> { }

		#endregion


		// Acuminator disable once PX1031 InstanceMethodInDac [not supposed to be extendable]
		public virtual bool IsWorkDay(DateTime date)
		{
			switch (date.DayOfWeek)
			{
				case DayOfWeek.Sunday:
					return (SunWorkDay == true);
				case DayOfWeek.Monday:
					return (MonWorkDay == true);
				case DayOfWeek.Tuesday:
					return (TueWorkDay == true);
				case DayOfWeek.Wednesday:
					return (WedWorkDay == true);
				case DayOfWeek.Thursday:
					return (ThuWorkDay == true);
				case DayOfWeek.Friday:
					return (FriWorkDay == true);
				case DayOfWeek.Saturday:
					return (SatWorkDay == true);
			}
			return false;
		}
	}
}
