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
using PX.Data.BQL;
using System;

namespace PX.Objects.PR
{
	[PXCacheName(Messages.PREmployeeClass)]
	[Serializable]
	public class PRSelectionPeriod : PXBqlTable, IBqlTable
	{
		#region Period
		public abstract class period : BqlString.Field<period> { }
		[PXString]
		[PXUIField(DisplayName = "Period")]
		[PXStringList(
			new string[] { PRSelectionPeriodIDs.LastMonth, PRSelectionPeriodIDs.Last12Months, PRSelectionPeriodIDs.CurrentQuarter, PRSelectionPeriodIDs.CurrentCalYear, PRSelectionPeriodIDs.CurrentFinYear },
			new string[] { Messages.LastMonth, Messages.Last12Months, Messages.CurrentQuarter, Messages.CurrentCalYear, Messages.CurrentFinYear })]
		public string Period { get; set; }
		#endregion
	}
}
