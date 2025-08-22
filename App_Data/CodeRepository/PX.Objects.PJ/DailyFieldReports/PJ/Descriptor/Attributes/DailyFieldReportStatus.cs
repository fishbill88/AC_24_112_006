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
using PX.Objects.PJ.DailyFieldReports.Descriptor;

namespace PX.Objects.PJ.DailyFieldReports.PJ.Descriptor.Attributes
{
	public class DailyFieldReportStatus
	{
		public const string Hold = "H";
		public const string PendingApproval = "A";
		public const string Rejected = "R";
		public const string Completed = "F";

		public class ListAttribute : PXStringListAttribute
		{
			private static readonly string[] AllowedValues =
			{
				Hold,
				PendingApproval,
				Rejected,
				Completed
			};

			private static readonly string[] AllowedLabels =
			{
				DailyFieldReportMessages.HoldStatus,
				DailyFieldReportMessages.PendingApprovalStatus,
				DailyFieldReportMessages.RejectedStatus,
				DailyFieldReportMessages.CompletedStatus
			};

			public ListAttribute() : base(AllowedValues, AllowedLabels) {}
		}

		public sealed class hold : BqlString.Constant<hold>
		{
			public hold() : base(Hold) {}
		}

		public sealed class pendingApproval : BqlString.Constant<pendingApproval>
		{
			public pendingApproval() : base(PendingApproval) {}
		}

		public sealed class rejected : BqlString.Constant<rejected>
		{
			public rejected() : base(Rejected) {}
		}

		public sealed class completed : BqlString.Constant<completed>
		{
			public completed() : base(Completed) {}
		}
	}
}
