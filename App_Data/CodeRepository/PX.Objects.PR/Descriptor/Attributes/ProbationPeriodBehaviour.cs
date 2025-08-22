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

namespace PX.Objects.PR
{
	public class ProbationPeriodBehaviour
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { AccruedAndAvailable, AccruedButNotAvailable, NotAccrued },
				new string[] { Messages.AccruedAndAvailable, Messages.AccruedButNotAvailable, Messages.NotAccrued })
			{ }
		}

		public class accruedAndAvailable : PX.Data.BQL.BqlString.Constant<accruedAndAvailable>
		{
			public accruedAndAvailable() : base(AccruedAndAvailable) { }
		}

		public class accruedButNotAvailable : PX.Data.BQL.BqlString.Constant<accruedButNotAvailable>
		{
			public accruedButNotAvailable() : base(AccruedButNotAvailable) { }
		}

		public class notAccrued : PX.Data.BQL.BqlString.Constant<notAccrued>
		{
			public notAccrued() : base(NotAccrued) { }
		}

		public const string AccruedAndAvailable = "AAA";
		public const string AccruedButNotAvailable = "ANA";
		public const string NotAccrued = "NAC";
	}
}
