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
using System.Runtime.Serialization;

namespace PX.Objects.GL.Consolidation
{
	internal class BranchLedgerApiExceptionAPI
	{
		public virtual ApiProperty<string> BranchCD { get; set; }
		public virtual ApiProperty<string> LedgerCD { get; set; }
	}

	internal class CommonApiExceptionAPI
	{
		public virtual string message { get; set; }
		public virtual string exceptionMessage { get; set; }
	}

	internal class ApiException : Exception
	{
		public ApiException(string message) : base(message) { }

		protected ApiException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}

}
