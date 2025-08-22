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
	public class PTOAccrualMethod
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
			: base(
				new string[] { Percentage, TotalHoursPerYear, FrontLoading, FrontLoadingAndPercentage, FrontLoadingAndHoursPerYear },
				new string[] { Messages.Percentage, Messages.TotalHoursPerYear, Messages.FrontLoading, Messages.FrontLoadingAndPercentage, Messages.FrontLoadingAndHoursPerYear })
			{ }
		}

		public class percentage : PX.Data.BQL.BqlString.Constant<percentage>
		{
			public percentage() : base(Percentage) { }
		}

		public class totalHoursPerYear : PX.Data.BQL.BqlString.Constant<totalHoursPerYear>
		{
			public totalHoursPerYear() : base(TotalHoursPerYear) { }
		}

		public class frontLoading : PX.Data.BQL.BqlString.Constant<frontLoading>
		{
			public frontLoading() : base(FrontLoading) { }
		}

		public class frontLoadingAndPercentage : PX.Data.BQL.BqlString.Constant<frontLoadingAndPercentage>
		{
			public frontLoadingAndPercentage() : base(FrontLoadingAndPercentage) { }
		}

		public class frontLoadingAndHoursPerYear : PX.Data.BQL.BqlString.Constant<frontLoadingAndHoursPerYear>
		{
			public frontLoadingAndHoursPerYear() : base(FrontLoadingAndHoursPerYear) { }
		}


		public const string Percentage = "PCT";
		public const string TotalHoursPerYear = "HPY";
		public const string FrontLoading = "FLD";
		public const string FrontLoadingAndPercentage = "FAP";
		public const string FrontLoadingAndHoursPerYear = "FHP";
	}
}
