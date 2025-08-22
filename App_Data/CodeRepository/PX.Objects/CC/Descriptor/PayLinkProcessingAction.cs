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
using System;

namespace PX.Objects.CC
{
	public static class PayLinkProcessingAction
	{
		public const string Create = "C";
		public const string Sync = "S";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute() : base(ValueLabelPairs())
			{

			}

			public static Tuple<string, string>[] ValueLabelPairs()
			{
				var arr = new Tuple<string, string>[]
				{
					new Tuple<string, string>(Create, Messages.CreateLink),
					new Tuple<string, string>(Sync, Messages.SyncLink),
				};
				return arr;
			}
		}
	}
}
