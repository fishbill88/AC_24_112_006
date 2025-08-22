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

using PX.Objects.AM.Attributes;
using PX.Data;
using PX.Objects.CS;

namespace PX.Objects.AM.GraphExtensions
{
    /// <summary>
    /// Manufacturing extension on standard Acumatica Work Calendars
    /// </summary>
    public class CSCalendarMaintAMExtension : PXGraphExtension<CSCalendarMaint>
    {
		public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.manufacturing>();

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXReferentialIntegrityCheck2]
		protected virtual void _(Events.CacheAttached<CSCalendar.calendarID> e)
        {
            // Add support for Referential Integrity check to prevent users from Deleting calendars that are used in Manufacturing
        }
    }
}
