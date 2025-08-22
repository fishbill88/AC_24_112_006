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

using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PX.Objects.FS
{
    public class PXRedirectToBoardRequiredException : PXRedirectToUrlException
    {
		private static string BuildUrl(string baseBoardUrl, KeyValuePair<string, string>[] args)
        {
            StringBuilder boardUrl = new StringBuilder(@"~\");
            boardUrl.Append(baseBoardUrl);

            if (args != null && args.Length > 0)
            {
                boardUrl.Append("?");

                KeyValuePair<string, string> kvp;

                for (int i = 0; i < args.Length; i++)
                {
                    kvp = args[i];
                    boardUrl.Append(kvp.Key);
                    boardUrl.Append("=");
                    boardUrl.Append(kvp.Value);

                    if (i != args.Length - 1)
                    {
                        boardUrl.Append("&");
                    }
                }
            }

            return boardUrl.ToString();
        }

        public PXRedirectToBoardRequiredException(string baseBoardUrl, KeyValuePair<string, string>[] parameters, WindowMode windowMode = WindowMode.NewWindow, bool supressFrameset = true)
            : base(BuildUrl(baseBoardUrl, parameters), windowMode, supressFrameset, null)
        {
        }

		protected PXRedirectToBoardRequiredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public static PXBaseRedirectException GenerateMultiEmployeeRedirect(
			PXSiteMapProvider siteMapProvide,
			KeyValuePair<string, string>[] parameters,
			MainAppointmentFilter calendarFilter,
			WindowMode windowMode = WindowMode.NewWindow)
		{
			var siteMapNode = siteMapProvide.FindSiteMapNodeByScreenID(ID.ScreenID.MULTI_EMPLOYEE_CALENDAR);
			if (siteMapNode.Url.IndexOf(Paths.ScreenPaths.MULTI_EMPLOYEE_DISPATCH, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return new PXRedirectToBoardRequiredException(
					Paths.ScreenPaths.MULTI_EMPLOYEE_DISPATCH,
					parameters,
					windowMode);
			}
			else
			{
				var graphCalendar = PXGraph.CreateInstance<SchedulerMaint>();
				if (graphCalendar != null)
				{
					graphCalendar.MainAppointmentFilter.Current = calendarFilter;
					return new PXRedirectRequiredException(graphCalendar, null) { Mode = windowMode };
				}
			}
			return null;
		}
	}
}
