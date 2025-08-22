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
using System.Collections;
using PX.Data;
using PX.Objects.EP;
using PX.Objects.CT;
using System.Linq;
using PX.Common;
using PX.Objects.CS.Services.WorkTimeCalculation;

namespace PX.Objects.CS
{
	public class CSCalendarMaint : PXGraph<CSCalendarMaint, CSCalendar>
	{
		#region DACs

		[Serializable]
		[PXHidden]
		public partial class CSCalendarExceptionsParamsParameters : PXBqlTable, IBqlTable
		{
			#region YearID
			public abstract class yearID : PX.Data.BQL.BqlInt.Field<yearID> { }
			protected Int32? _YearID;
			[PXInt()]
			[PXUIField(DisplayName = "Year", Visibility = PXUIVisibility.SelectorVisible)]
			[PXSelector(typeof(Search4<CSCalendarExceptions.yearID,
				Where<CSCalendarExceptions.calendarID, Equal<Current<CSCalendar.calendarID>>>,
				Aggregate<GroupBy<CSCalendarExceptions.yearID>>>))]
			public virtual Int32? YearID
			{
				get
				{
					return this._YearID;
				}
				set
				{
					this._YearID = value;
				}
			}
			#endregion
		}

		#endregion

		#region Selects

		public PXSelect<CSCalendar> Calendar;
		public PXSelect<CSCalendar,
					Where<CSCalendar.calendarID, Equal<Current<CSCalendar.calendarID>>>> CalendarDetails;
		public PXFilter<CSCalendarExceptionsParamsParameters> Filter;

		public PXSelect<CSCalendarExceptions> CSCalendarExceptions;
		protected virtual IEnumerable cSCalendarExceptions()
		{
			CSCalendarExceptionsParamsParameters header = Filter.Current;
			if (header == null)
			{
				yield break;
			}

			foreach (CSCalendarExceptions calend in PXSelect<CSCalendarExceptions,
				Where<CSCalendarExceptions.calendarID, Equal<Current<CSCalendar.calendarID>>>>
				.Select(this))
			{
				if ((header.YearID.HasValue && header.YearID == calend.YearID) ||
					header.YearID.HasValue == false)
				{
					yield return calend;
				}
			}
		}

		public PXSelect<CSCalendarBreakTime, Where<CSCalendarBreakTime.calendarID, Equal<Current<CSCalendar.calendarID>>>> CalendarBreakTimes;

		#endregion

		#region Events

		#region CSCalendar

		protected virtual void CSCalendar_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null) return;

			CSCalendar row = (CSCalendar)e.Row;

			PXUIFieldAttribute.SetEnabled<CSCalendar.sunStartTime>(Calendar.Cache, row, row.SunWorkDay ?? false);
			PXUIFieldAttribute.SetEnabled<CSCalendar.sunEndTime>(Calendar.Cache, row, row.SunWorkDay ?? false);

			PXUIFieldAttribute.SetEnabled<CSCalendar.monStartTime>(Calendar.Cache, row, row.MonWorkDay ?? false);
			PXUIFieldAttribute.SetEnabled<CSCalendar.monEndTime>(Calendar.Cache, row, row.MonWorkDay ?? false);

			PXUIFieldAttribute.SetEnabled<CSCalendar.tueStartTime>(Calendar.Cache, row, row.TueWorkDay ?? false);
			PXUIFieldAttribute.SetEnabled<CSCalendar.tueEndTime>(Calendar.Cache, row, row.TueWorkDay ?? false);

			PXUIFieldAttribute.SetEnabled<CSCalendar.wedStartTime>(Calendar.Cache, row, row.WedWorkDay ?? false);
			PXUIFieldAttribute.SetEnabled<CSCalendar.wedEndTime>(Calendar.Cache, row, row.WedWorkDay ?? false);

			PXUIFieldAttribute.SetEnabled<CSCalendar.thuStartTime>(Calendar.Cache, row, row.ThuWorkDay ?? false);
			PXUIFieldAttribute.SetEnabled<CSCalendar.thuEndTime>(Calendar.Cache, row, row.ThuWorkDay ?? false);

			PXUIFieldAttribute.SetEnabled<CSCalendar.friStartTime>(Calendar.Cache, row, row.FriWorkDay ?? false);
			PXUIFieldAttribute.SetEnabled<CSCalendar.friEndTime>(Calendar.Cache, row, row.FriWorkDay ?? false);

			PXUIFieldAttribute.SetEnabled<CSCalendar.satStartTime>(Calendar.Cache, row, row.SatWorkDay ?? false);
			PXUIFieldAttribute.SetEnabled<CSCalendar.satEndTime>(Calendar.Cache, row, row.SatWorkDay ?? false);

			CalculateWorkTime(row);
		}

		protected virtual void CSCalendar_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			CSCalendar calendar = (CSCalendar)e.NewRow;

			if (calendar == null)
				return;

			CalculateWorkTime(calendar);
		}

		protected virtual void CSCalendar_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			CSCalendar row = (CSCalendar)e.Row;
			PXDefaultAttribute.SetPersistingCheck<CSCalendar.sunStartTime>(sender, e.Row, row.SunWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CSCalendar.sunEndTime>(sender, e.Row, row.SunWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			PXDefaultAttribute.SetPersistingCheck<CSCalendar.monStartTime>(sender, e.Row, row.MonWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CSCalendar.monEndTime>(sender, e.Row, row.MonWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			PXDefaultAttribute.SetPersistingCheck<CSCalendar.tueStartTime>(sender, e.Row, row.TueWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CSCalendar.tueEndTime>(sender, e.Row, row.TueWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			PXDefaultAttribute.SetPersistingCheck<CSCalendar.wedStartTime>(sender, e.Row, row.WedWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CSCalendar.wedEndTime>(sender, e.Row, row.WedWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			PXDefaultAttribute.SetPersistingCheck<CSCalendar.thuStartTime>(sender, e.Row, row.ThuWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CSCalendar.thuEndTime>(sender, e.Row, row.ThuWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			PXDefaultAttribute.SetPersistingCheck<CSCalendar.friStartTime>(sender, e.Row, row.FriWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CSCalendar.friEndTime>(sender, e.Row, row.FriWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			PXDefaultAttribute.SetPersistingCheck<CSCalendar.satStartTime>(sender, e.Row, row.SatWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CSCalendar.satEndTime>(sender, e.Row, row.SatWorkDay == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			if (row.SunWorkDay is false
				&& row.MonWorkDay is false
				&& row.TueWorkDay is false
				&& row.WedWorkDay is false
				&& row.ThuWorkDay is false
				&& row.FriWorkDay is false
				&& row.SatWorkDay is false)
			{
				throw new PXException(MessagesNoPrefix.CalendarMustHaveWorkingDays, row.CalendarID);
			}

			if (row.WorkdayTime is <= 0 or > 24 * 60)
			{
				throw new PXException(MessagesNoPrefix.WorkdayTimeMustBeBetweenZeroAndOneDay);
			}
		}

		protected virtual void CSCalendar_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			CSCalendar row = e.Row as CSCalendar;
			if (row != null)
			{
				EPEmployeeClass refEmpClass = PXSelect<EPEmployeeClass, Where<EPEmployeeClass.calendarID, Equal<Current<CSCalendar.calendarID>>>>.SelectWindowed(this, 0, 1);
				if (refEmpClass != null)
				{
					e.Cancel = true;
					throw new PXException(Messages.ReferencedByEmployeeClass, refEmpClass.VendorClassID);
				}

				Carrier refCarrier = PXSelect<Carrier, Where<Carrier.calendarID, Equal<Current<CSCalendar.calendarID>>>>.SelectWindowed(this, 0, 1);
				if (refCarrier != null)
				{
					e.Cancel = true;
					throw new PXException(Messages.ReferencedByCarrier, refCarrier.CarrierID);
				}

				Contract refContract = PXSelect<Contract, Where<Contract.calendarID, Equal<Current<CSCalendar.calendarID>>>>.SelectWindowed(this, 0, 1);
				if (refContract != null)
				{
					e.Cancel = true;
					throw new PXException(Messages.ReferencedByContract, refContract.ContractID);
				}

				EPEmployee refEmployee = PXSelect<EPEmployee, Where<EPEmployee.calendarID, Equal<Current<CSCalendar.calendarID>>>>.SelectWindowed(this, 0, 1);
				if (refEmployee != null)
				{
					e.Cancel = true;
					throw new PXException(Messages.ReferencedByEmployee, refEmployee.ClassID);
				}
			}
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.sunWorkDay> e)
		{
			if (e.Row == null) return;

			CSCalendar calendar = e.Row;

			if (calendar.SunWorkDay is true)
			{
				if (calendar.SunStartTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.sunStartTime>(calendar);
				}

				if (calendar.SunEndTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.sunEndTime>(calendar);
				}
			}
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.monWorkDay> e)
		{
			if (e.Row == null) return;

			CSCalendar calendar = e.Row;

			if (calendar.MonWorkDay is true)
			{
				if (calendar.MonStartTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.monStartTime>(calendar);
				}

				if (calendar.MonEndTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.monEndTime>(calendar);
				}
			}
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.tueWorkDay> e)
		{
			if (e.Row == null) return;

			CSCalendar calendar = e.Row;

			if (calendar.TueWorkDay is true)
			{
				if (calendar.TueStartTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.tueStartTime>(calendar);
				}

				if (calendar.TueEndTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.tueEndTime>(calendar);
				}
			}
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.wedWorkDay> e)
		{
			if (e.Row == null) return;

			CSCalendar calendar = e.Row;

			if (calendar.WedWorkDay is true)
			{
				if (calendar.WedStartTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.wedStartTime>(calendar);
				}

				if (calendar.WedEndTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.wedEndTime>(calendar);
				}
			}
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.thuWorkDay> e)
		{
			if (e.Row == null) return;

			CSCalendar calendar = e.Row;

			if (calendar.ThuWorkDay is true)
			{
				if (calendar.ThuStartTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.thuStartTime>(calendar);
				}

				if (calendar.ThuEndTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.thuEndTime>(calendar);
				}
			}
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.friWorkDay> e)
		{
			if (e.Row == null) return;

			CSCalendar calendar = e.Row;

			if (calendar.FriWorkDay is true)
			{
				if (calendar.FriStartTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.friStartTime>(calendar);
				}

				if (calendar.FriEndTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.friEndTime>(calendar);
				}
			}
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.satWorkDay> e)
		{
			if (e.Row == null) return;

			CSCalendar calendar = e.Row;

			if (calendar.SatWorkDay is true)
			{
				if (calendar.SatStartTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.satStartTime>(calendar);
				}

				if (calendar.SatEndTime is null)
				{
					e.Cache.SetDefaultExt<CSCalendar.satEndTime>(calendar);
				}
			}
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.sunStartTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Sunday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.sunEndTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Sunday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.monStartTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Monday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.monEndTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Monday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.tueStartTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Tuesday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.tueEndTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Tuesday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.wedStartTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Wednesday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.wedEndTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Wednesday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.thuStartTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Thursday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.thuEndTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Thursday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.friStartTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Friday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.friEndTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Friday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.satStartTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Saturday);
		}

		protected virtual void _(Events.FieldUpdated<CSCalendar, CSCalendar.satEndTime> e)
		{
			if (e.NewValue != null && Equals(e.NewValue, e.OldValue) is false)
				RecalculateBreakTimes(CSCalendarBreakTime.dayOfWeek.Saturday);
		}



		#endregion

		#region CSCalendarBreakTime

		protected virtual void _(Events.RowInserted<CSCalendarBreakTime> e)
		{
			if (e.Row == null)
				return;

			RecalculateBreakTimes(e.Row.DayOfWeek!.Value);
		}

		protected virtual void _(Events.RowUpdated<CSCalendarBreakTime> e)
		{
			if (e.Row == null)
				return;

			RecalculateBreakTimes(e.Row.DayOfWeek!.Value);
		}

		protected virtual void _(Events.RowDeleted<CSCalendarBreakTime> e)
		{
			if (e.Row == null)
				return;

			RecalculateBreakTimes(e.Row.DayOfWeek!.Value);
		}

		#endregion

		#region CSCalendarExceptions

		protected virtual void CSCalendarExceptions_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			CSCalendarExceptions row = (CSCalendarExceptions)e.Row;
			if (row.CalendarID == null && Calendar.Current != null)
			{
				row.CalendarID = Calendar.Current.CalendarID;
			}
			if (row.Date.HasValue)
			{
				row.YearID = row.Date.Value.Year;
				row.DayOfWeek = (int)row.Date.Value.DayOfWeek + 1;
			}
			else
			{
				row.YearID = Accessinfo.BusinessDate.Value.Year;
				row.DayOfWeek = (int)Accessinfo.BusinessDate.Value.DayOfWeek + 1;
			}
		}

		protected virtual void CSCalendarExceptions_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			CSCalendarExceptions row = (CSCalendarExceptions)e.NewRow;

			if (row.Date.HasValue)
			{
				row.YearID = row.Date.Value.Year;
				row.DayOfWeek = (int)row.Date.Value.DayOfWeek + 1;
			}
			else
			{
				row.YearID = Accessinfo.BusinessDate.Value.Year;
				row.DayOfWeek = (int)Accessinfo.BusinessDate.Value.DayOfWeek + 1;
			}
		}

		#endregion

		#endregion

		#region Methods

		public virtual void RecalculateBreakTimes(int dayOfWeek)
		{
			CSCalendar calendar = CalendarDetails.Current;

			CalendarInfo calendarInfo = WorkTimeCalculatorProvider.ConvertToCalendarInfo(
				calendar,
				CSCalendarExceptions.Select<CSCalendarExceptions>().ToList(),
				CalendarBreakTimes.Select<CSCalendarBreakTime>().ToList());

			switch (dayOfWeek)
			{
				case CSCalendarBreakTime.dayOfWeek.All:
					calendar.SunUnpaidTime = CalculateDayBreakTime(calendarInfo, CSCalendarBreakTime.dayOfWeek.Sunday);
					calendar.MonUnpaidTime = CalculateDayBreakTime(calendarInfo, CSCalendarBreakTime.dayOfWeek.Monday);
					calendar.TueUnpaidTime = CalculateDayBreakTime(calendarInfo, CSCalendarBreakTime.dayOfWeek.Tuesday);
					calendar.WedUnpaidTime = CalculateDayBreakTime(calendarInfo, CSCalendarBreakTime.dayOfWeek.Wednesday);
					calendar.ThuUnpaidTime = CalculateDayBreakTime(calendarInfo, CSCalendarBreakTime.dayOfWeek.Thursday);
					calendar.FriUnpaidTime = CalculateDayBreakTime(calendarInfo, CSCalendarBreakTime.dayOfWeek.Friday);
					calendar.SatUnpaidTime = CalculateDayBreakTime(calendarInfo, CSCalendarBreakTime.dayOfWeek.Saturday);
					break;
				case CSCalendarBreakTime.dayOfWeek.Sunday:
					calendar.SunUnpaidTime = CalculateDayBreakTime(calendarInfo, dayOfWeek);
					break;
				case CSCalendarBreakTime.dayOfWeek.Monday:
					calendar.MonUnpaidTime = CalculateDayBreakTime(calendarInfo, dayOfWeek);
					break;
				case CSCalendarBreakTime.dayOfWeek.Tuesday:
					calendar.TueUnpaidTime = CalculateDayBreakTime(calendarInfo, dayOfWeek);
					break;
				case CSCalendarBreakTime.dayOfWeek.Wednesday:
					calendar.WedUnpaidTime = CalculateDayBreakTime(calendarInfo, dayOfWeek);
					break;
				case CSCalendarBreakTime.dayOfWeek.Thursday:
					calendar.ThuUnpaidTime = CalculateDayBreakTime(calendarInfo, dayOfWeek);
					break;
				case CSCalendarBreakTime.dayOfWeek.Friday:
					calendar.FriUnpaidTime = CalculateDayBreakTime(calendarInfo, dayOfWeek);
					break;
				case CSCalendarBreakTime.dayOfWeek.Saturday:
					calendar.SatUnpaidTime = CalculateDayBreakTime(calendarInfo, dayOfWeek);
					break;
				default:
					break;
			}

			CalculateWorkTime(calendar);
		}

		[Obsolete]
		public virtual int? CalculateDayBreakTime(int dayOfWeek, DateTime? startDateTime, DateTime? endDateTime) => null;


		public virtual int? CalculateDayBreakTime(CalendarInfo calendarInfo, int dayOfWeek)
		{
			var dayOfWeekEnum = (DayOfWeek)dayOfWeek;

			DayOfWeekInfo dayInfo = calendarInfo.DaysOfWeek[dayOfWeekEnum];

			return (int)dayInfo.GetBreakTimeDuration().TotalMinutes;
		}

		public virtual void CalculateWorkTime(CSCalendar calendar)
		{
			calendar.SunWorkTime =
				CalculateWorkTimeByDay(
					calendar.SunStartTime,
					calendar.SunEndTime,
					calendar.SunUnpaidTime ?? 0);

			calendar.MonWorkTime =
				CalculateWorkTimeByDay(
					calendar.MonStartTime,
					calendar.MonEndTime,
					calendar.MonUnpaidTime ?? 0);

			calendar.TueWorkTime =
				CalculateWorkTimeByDay(
					calendar.TueStartTime,
					calendar.TueEndTime,
					calendar.TueUnpaidTime ?? 0);

			calendar.WedWorkTime =
				CalculateWorkTimeByDay(
					calendar.WedStartTime,
					calendar.WedEndTime,
					calendar.WedUnpaidTime ?? 0);

			calendar.ThuWorkTime =
				CalculateWorkTimeByDay(
					calendar.ThuStartTime,
					calendar.ThuEndTime,
					calendar.ThuUnpaidTime ?? 0);

			calendar.FriWorkTime =
				CalculateWorkTimeByDay(
					calendar.FriStartTime,
					calendar.FriEndTime,
					calendar.FriUnpaidTime ?? 0);

			calendar.SatWorkTime =
				CalculateWorkTimeByDay(
					calendar.SatStartTime,
					calendar.SatEndTime,
					calendar.SatUnpaidTime ?? 0);

			if (calendar.WorkdayTimeOverride is false)
			{
				calendar.WorkdayTime = CalculateWorkdayTime(calendar);
			}
		}

		public virtual int CalculateWorkTimeByDay(DateTime? startTime, DateTime? endTime, int breakTimeMinutes)
		{
			if (startTime is null || endTime is null) return 0;

			var realEndTime = (DateTime.Compare(startTime.Value, endTime.Value) >= 0) ? endTime!.Value.AddDays(1) : endTime.Value;

			return (int)(realEndTime - startTime.Value).TotalMinutes - breakTimeMinutes;
		}

		public virtual int CalculateWorkdayTime(CSCalendar calendar)
		{
			if (calendar == null) return 0;

			var totalMinutes =
				(calendar.SunWorkDay is true ? calendar.SunWorkTime.GetValueOrDefault() : 0)
				+ (calendar.MonWorkDay is true ? calendar.MonWorkTime.GetValueOrDefault() : 0)
				+ (calendar.TueWorkDay is true ? calendar.TueWorkTime.GetValueOrDefault() : 0)
				+ (calendar.WedWorkDay is true ? calendar.WedWorkTime.GetValueOrDefault() : 0)
				+ (calendar.ThuWorkDay is true ? calendar.ThuWorkTime.GetValueOrDefault() : 0)
				+ (calendar.FriWorkDay is true ? calendar.FriWorkTime.GetValueOrDefault() : 0)
				+ (calendar.SatWorkDay is true ? calendar.SatWorkTime.GetValueOrDefault() : 0);

			var totalWorkDays =
				(calendar.SunWorkDay is true ? 1 : 0)
				+ (calendar.MonWorkDay is true ? 1 : 0)
				+ (calendar.TueWorkDay is true ? 1 : 0)
				+ (calendar.WedWorkDay is true ? 1 : 0)
				+ (calendar.ThuWorkDay is true ? 1 : 0)
				+ (calendar.FriWorkDay is true ? 1 : 0)
				+ (calendar.SatWorkDay is true ? 1 : 0);

			return (totalWorkDays == 0) ? 0 : (int)decimal.Round((decimal)totalMinutes / totalWorkDays, 2, MidpointRounding.AwayFromZero);
		}

		public virtual DateTime? GetStartDateTimeOfDay(int dayOfWeek)
		{
			DateTime? dayStartDateTime = null;

			switch (dayOfWeek)
			{
				case CSCalendarBreakTime.dayOfWeek.Sunday:
					dayStartDateTime = CalendarDetails.Current.SunStartTime;
					break;
				case CSCalendarBreakTime.dayOfWeek.Monday:
					dayStartDateTime = CalendarDetails.Current.MonStartTime;
					break;
				case CSCalendarBreakTime.dayOfWeek.Tuesday:
					dayStartDateTime = CalendarDetails.Current.TueStartTime;
					break;
				case CSCalendarBreakTime.dayOfWeek.Wednesday:
					dayStartDateTime = CalendarDetails.Current.WedStartTime;
					break;
				case CSCalendarBreakTime.dayOfWeek.Thursday:
					dayStartDateTime = CalendarDetails.Current.ThuStartTime;
					break;
				case CSCalendarBreakTime.dayOfWeek.Friday:
					dayStartDateTime = CalendarDetails.Current.FriStartTime;
					break;
				case CSCalendarBreakTime.dayOfWeek.Saturday:
					dayStartDateTime = CalendarDetails.Current.SatStartTime;
					break;
				default:
					break;
			}

			return dayStartDateTime;
		}

		public virtual DateTime? GetEndDateTimeOfDay(int dayOfWeek)
		{
			DateTime? dayEndDateTime = null;

			switch (dayOfWeek)
			{
				case CSCalendarBreakTime.dayOfWeek.Sunday:
					dayEndDateTime = CalendarDetails.Current.SunEndTime;
					break;
				case CSCalendarBreakTime.dayOfWeek.Monday:
					dayEndDateTime = CalendarDetails.Current.MonEndTime;
					break;
				case CSCalendarBreakTime.dayOfWeek.Tuesday:
					dayEndDateTime = CalendarDetails.Current.TueEndTime;
					break;
				case CSCalendarBreakTime.dayOfWeek.Wednesday:
					dayEndDateTime = CalendarDetails.Current.WedEndTime;
					break;
				case CSCalendarBreakTime.dayOfWeek.Thursday:
					dayEndDateTime = CalendarDetails.Current.ThuEndTime;
					break;
				case CSCalendarBreakTime.dayOfWeek.Friday:
					dayEndDateTime = CalendarDetails.Current.FriEndTime;
					break;
				case CSCalendarBreakTime.dayOfWeek.Saturday:
					dayEndDateTime = CalendarDetails.Current.SatEndTime;
					break;
				default:
					break;
			}

			return dayEndDateTime;
		}

		#endregion
	}
}
