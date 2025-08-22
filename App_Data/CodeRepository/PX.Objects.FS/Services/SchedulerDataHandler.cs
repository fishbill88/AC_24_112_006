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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Common;
using PX.Data;
using PX.Objects.FS;
using PX.Objects.FS.Interfaces;
using static PX.Objects.FS.SchedulerDatesFilter;

namespace PX.Objects.FS.Services
{
	internal class SchedulerDataHandler : ISchedulerDataHandler 
	{
		public void StoreDatesFilter(SchedulerDatesFilter filter, string screenId)
		{
			string key = GetSessionKey(screenId);
			PXContext.SessionTyped<SchedulerSession>().PeriodInfos[key] = filter;
		}

		public SchedulerDatesFilter LoadDatesFilter(string screenId)
		{
			string key = GetSessionKey(screenId);
			var filter = PXContext.SessionTyped<SchedulerSession>().PeriodInfos[key];
			var initialDate = PXContext.GetBusinessDate() ?? PXTimeZoneInfo.Today; // Demo: new DateTime(2020, 8, 5)
			filter = (filter?.PeriodKind != null) ? filter : new SchedulerDatesFilter()
			{
				FilterBusinessHours = true,
				PeriodKind = (int?)FilterPeriodKind.Day,
				DateBegin = initialDate,
				DateEnd = initialDate.AddDays(1),
				DateSelected = initialDate,
			};
			return filter;
		}

		private static string GetSessionKey(string screenId)
		{
			return $"_scheduler";
			//return $"{screenId}_scheduler";
			//return $"{PXContext.GetScreenID()}_scheduler";
		}
	}
}
