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

namespace PX.Objects.FS.Scheduler
{
    /// <summary>
    /// This class allows to map a FSSchedule in the Service Management module to a Schedule in the Scheduler module.
    /// </summary>
    public static class MapFSServiceContractToSchedule
    {
        /// <summary>
        /// This function converts a FSServiceContract to a List[Schedule].
        /// </summary>
        public static List<Schedule> convertFSServiceContractToSchedule(FSServiceContract fsServiceContractRow, DateTime? lastGeneratedElement, Period period = null)
        {
            List<Schedule> results = new List<Schedule>();
            switch (fsServiceContractRow.BillingPeriod)
            {
                #region ONETIME
                /*case ID.Contract_BillingPeriod.ONETIME:
                    //TODO future implementation
                    break;*/
                #endregion
                #region WEEK
                case ID.Contract_BillingPeriod.WEEK:
                    results.Add(mapDailyFrequency(fsServiceContractRow, lastGeneratedElement, 6, period));
                    break;
                #endregion
                #region MONTH
                case ID.Contract_BillingPeriod.MONTH:
                    results.Add(mapMonthlyFrequency(fsServiceContractRow, lastGeneratedElement, 1, period));
                    break;
                #endregion
                #region QUARTER
                case ID.Contract_BillingPeriod.QUARTER:
                    results.Add(mapMonthlyFrequency(fsServiceContractRow, lastGeneratedElement, 3, period));
                    break;
                #endregion
                #region HALFYEAR
                case ID.Contract_BillingPeriod.HALFYEAR:
                    results.Add(mapMonthlyFrequency(fsServiceContractRow, lastGeneratedElement, 6, period));
                    break;
                #endregion
                #region HALFYEAR
                case ID.Contract_BillingPeriod.YEAR:
                    results.Add(mapMonthlyFrequency(fsServiceContractRow, lastGeneratedElement, 12, period));
                    break;
                    #endregion
            }

            return results;
        }

        /// <summary>
        /// This function maps a FSServiceCOoract daily frequency to a DailySchedule in the Scheduler module.
        /// </summary>
        public static Schedule mapDailyFrequency(FSServiceContract fsServiceContractRow, DateTime? lastGeneratedElementDate, int frequency, Period period = null)
        {

            var dailySchedule = new DailySchedule
            {
                Name = TX.FrecuencySchedule.DAILY,
                LastGeneratedTimeSlotDate = lastGeneratedElementDate,
                SubScheduleID = 0,
                TimeOfDayBegin = new TimeSpan(5, 0, 0), //TODO SD-5493
                TimeOfDayEnd = new TimeSpan(11, 0, 0),
                SchedulingRange = period ?? new Period((DateTime)fsServiceContractRow.StartDate, fsServiceContractRow.EndDate),
                Frequency = frequency
            };
            return dailySchedule;
        }


        /// <summary>
        /// This function maps a FSServiceContract weekly frequency to a WeeklySchedule in the Scheduler module.
        /// </summary>
        public static Schedule mapWeeklyFrequency(FSServiceContract fsServiceContractRow, DateTime? lastGeneratedElementDate, Period period = null)
        {
            List<DayOfWeek> days = new List<DayOfWeek>();
            var weeklySchedule = new WeeklySchedule
            {
                Name = TX.FrecuencySchedule.WEEKLY,
                LastGeneratedTimeSlotDate = lastGeneratedElementDate,
                SubScheduleID = 0,
                TimeOfDayBegin = new TimeSpan(5, 0, 0),
                TimeOfDayEnd = new TimeSpan(11, 0, 0),
                SchedulingRange = period ?? new Period((DateTime)fsServiceContractRow.StartDate, fsServiceContractRow.EndDate),
                Frequency = 1,
            };
            #region SetDaysToList

            days.Add(fsServiceContractRow.StartDate.Value.DayOfWeek);
            weeklySchedule.SetDays(days.ToArray());
            #endregion

            return weeklySchedule;
        }

        public static Schedule mapMonthlyFrequency(FSServiceContract fsServiceContractRow, DateTime? lastGeneratedElementDate, int frequency, Period period = null)
        {
            Period schedulingRange = period ?? new Period((DateTime)fsServiceContractRow.StartDate, fsServiceContractRow.EndDate);

            var monthlyScheduleSpecificDay = new MonthlyScheduleSpecificDay
            {
                Name = TX.FrecuencySchedule.MONTHSPECIFICDATE,
                LastGeneratedTimeSlotDate = lastGeneratedElementDate,
                SubScheduleID = 1,
                TimeOfDayBegin = new TimeSpan(5, 0, 0), //TODO SD-5493
                TimeOfDayEnd = new TimeSpan(11, 0, 0),
                Frequency = frequency,
                SchedulingRange = schedulingRange
            };

            monthlyScheduleSpecificDay.DayOfMonth = fsServiceContractRow.StartDate.Value.Day;

            return monthlyScheduleSpecificDay;
        }
    }
}