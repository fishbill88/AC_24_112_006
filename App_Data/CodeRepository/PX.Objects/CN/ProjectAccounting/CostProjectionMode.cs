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
using Messages = PX.Objects.PM.Messages;

namespace PX.Objects.CN.ProjectAccounting
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public static class ProjectionMode
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Auto, Manual, ManualQuantity, ManualCost },
				new string[] { Messages.ProjectionModeAuto, Messages.ProjectionModeManual, Messages.ProjectionModeManualQuantity, Messages.ProjectionModeManualCost })
			{
			}
		}

		public class ShortListAttribute : PXStringListAttribute
		{
			public ShortListAttribute()
				: base(
				new string[] { Auto, Manual },
				new string[] { Messages.ProjectionModeAuto, Messages.ProjectionModeManual })
			{
			}
		}

		public const string Auto = "A";
		public const string Manual = "M";
		public const string ManualQuantity = "Q";
		public const string ManualCost = "C";
	}
}
