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

namespace PX.Objects.AM.Attributes
{
	/// <summary>
	/// Actions for the Rough Cut Planning processing screen (<see cref="APSRoughCutProcess"/>)
	/// </summary>
	public class APSRoughCutProcessActions
	{
        public const string Schedule = "S";
		public const string ScheduleAndFirm = "A";
		public const string Firm = "F";
		public const string UndoFirm = "U";

		public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(new string[]{ Schedule, ScheduleAndFirm, Firm, UndoFirm }
                , new string[]{ Messages.Schedule, Messages.ScheduleAndFirm, Messages.Firm, Messages.UndoFirm }) { }
        }
	}
}
