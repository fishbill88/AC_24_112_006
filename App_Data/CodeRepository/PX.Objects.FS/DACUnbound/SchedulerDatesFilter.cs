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

namespace PX.Objects.FS
{
	public enum FilterPeriodKind
	{
		Day = 1,
		Week = 2,
		Month = 3,
	}

    [System.SerializableAttribute]
	[PXHidden]
	public class SchedulerDatesFilter : PXBqlTable, IBqlTable
	{
        #region DateSelected
        public abstract class dateSelected : PX.Data.BQL.BqlDateTime.Field<dateSelected> { }

		[PXDateAndTime(UseTimeZone = true)]
		[PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? DateSelected { get; set; }
		#endregion

		#region FilterBusinessHours
		public abstract class filterBusinessHours : PX.Data.BQL.BqlBool.Field<filterBusinessHours> { }

		[PXBool]
		public virtual bool? FilterBusinessHours { get; set; }
		#endregion

		#region PeriodKind
		public abstract class periodKind : PX.Data.BQL.BqlInt.Field<periodKind> { }

		[PXInt]
		public virtual int? PeriodKind { get; set; }
		#endregion

		#region DateBegin
		public abstract class dateBegin : PX.Data.BQL.BqlDateTime.Field<dateBegin> { }

		[PXDateAndTime(UseTimeZone = true)]
		public virtual DateTime? DateBegin { get; set; }
		#endregion

		#region DateEnd
		public abstract class dateEnd : PX.Data.BQL.BqlDateTime.Field<dateEnd> { }

		[PXDateAndTime(UseTimeZone = true)]
		public virtual DateTime? DateEnd { get; set; }
        #endregion
    }
}
