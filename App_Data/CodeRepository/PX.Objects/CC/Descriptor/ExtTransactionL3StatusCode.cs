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

namespace PX.Objects.CC
{
	public static class ExtTransactionL3StatusCode
	{
		public const string NotApplicable = "NA ";
		public const string Pending = "PEN";
		public const string Sent = "SNT";
		public const string Failed = "FLD";
		public const string Rejected = "REJ";
		public const string ResendRejected = "RRJ";
		public const string ResendFailed = "RFL";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute() : base(
				new string[]
				{
					Pending,
					Sent,
					Failed,
					ResendFailed,
				},
				new string[]
				{
					Messages.L3Pending,
					Messages.L3Sent,
					Messages.L3Failed,
					Messages.L3ResendFailed,
				})
			{
			}
		}

		public class notApplicable : Data.BQL.BqlString.Constant<notApplicable>
		{
			public notApplicable() : base(NotApplicable) { }
		}

		public class pending : Data.BQL.BqlString.Constant<pending>
		{
			public pending() : base(Pending) { }
		}

		public class sent : Data.BQL.BqlString.Constant<sent>
		{
			public sent() : base(Sent) { }
		}

		public class failed : Data.BQL.BqlString.Constant<failed>
		{
			public failed() : base(Failed) { }
		}

		public class rejected : Data.BQL.BqlString.Constant<rejected>
		{
			public rejected() : base(Rejected) { }
		}

		public class resendRejected : Data.BQL.BqlString.Constant<resendRejected>
		{
			public resendRejected() : base(ResendRejected) { }
		}

		public class resendFailed : Data.BQL.BqlString.Constant<resendFailed>
		{
			public resendFailed() : base(ResendFailed) { }
		}
	}
}
