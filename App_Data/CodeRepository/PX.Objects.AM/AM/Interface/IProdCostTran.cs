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

namespace PX.Objects.AM
{
	public interface IProdCostTran : IProdOper
	{
		string DocType { get; set; }
		string TranType { get; set; }
		int? SubcontractSource { get; set; }
		bool? IsScrap { get; set; }
		bool? IsByproduct { get; set; }
		bool? LastOper { get; set; }
		Decimal? TranAmt { get; set; }
		Int32? LaborTime { get; set; }
		Decimal? BaseQty { get; set; }
		Int32? LineNbr { get; set; }
	}
}
