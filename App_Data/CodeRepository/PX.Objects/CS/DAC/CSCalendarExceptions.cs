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

namespace PX.Objects.CS
{
	[PXCacheName(Messages.CalendarException)]
	public partial class CSCalendarExceptions : PXBqlTable, PX.Data.IBqlTable
	{
		#region CalendarID

		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Calendar ID")]
		public virtual string CalendarID { get; set; }
		public abstract class calendarID : BqlString.Field<calendarID> { }

		#endregion

		#region YearID

		[PXDBInt()]
		[PXDefault(2008)]
		[PXUIField(DisplayName = "Year", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<CSCalendarExceptions.yearID>))]
		public virtual Int32? YearID { get; set; }
		public abstract class yearID : BqlInt.Field<yearID> { }

		#endregion

		#region Date

		[PXDBDate(IsKey = true)]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Date")]
		public virtual DateTime? Date { get; set; }
		public abstract class date : BqlDateTime.Field<date> { }

		#endregion

		#region DayOfWeek

		[PXDBInt()]
		[PXDefault(1)]
		[PXUIField(DisplayName = "Day Of Week", Enabled = false)]
		[PXIntList(new int[] { 1, 2, 3, 4, 5, 6, 7}, new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"})]
		public virtual Int32? DayOfWeek { get; set; }
		public abstract class dayOfWeek : BqlInt.Field<dayOfWeek> { }

		#endregion

		#region Description

		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		public virtual string Description { get; set; }
		public abstract class description : BqlString.Field<description> { }

		#endregion

		#region WorkDay

		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Work Day")]
		public virtual bool? WorkDay { get; set; }
		public abstract class workDay : BqlBool.Field<workDay> { }

		#endregion

		#region StartTime

		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 09:00:00")]
		[PXUIField(DisplayName = "Start Time")]
		public virtual DateTime? StartTime { get; set; }
		public abstract class startTime : BqlDateTime.Field<startTime> { }

		#endregion

		#region EndTime

		[PXDBTime(DisplayMask = "t", UseTimeZone = false)]
		[PXDefault(TypeCode.DateTime, "01/01/2008 18:00:00")]
		[PXUIField(DisplayName = "End Time")]
		public virtual DateTime? EndTime { get; set; }
		public abstract class endTime : BqlDateTime.Field<endTime> { }

		#endregion

		#region GoodsMoved

		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Goods Are Moved")]
		public virtual bool? GoodsMoved { get; set; }
		public abstract class goodsMoved : BqlBool.Field<goodsMoved> { }

		#endregion

		#region UnpaidTime

		[PXDBTimeSpanLong(Format = TimeSpanFormatType.ShortHoursMinutesCompact)]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Break Duration")]
		public virtual int? UnpaidTime { get; set; }
		public abstract class unpaidTime : Data.BQL.BqlInt.Field<unpaidTime> { }

		#endregion
	}
}
