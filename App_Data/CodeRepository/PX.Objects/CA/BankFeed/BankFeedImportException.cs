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

using PX.Common;
using PX.Data;
using System;
using System.Runtime.Serialization;
using PX.BankFeed.Common;

namespace PX.Objects.CA
{
	public class BankFeedImportException : PXException
	{
		public enum ExceptionReason { Error, LoginFailed }
		public ExceptionReason Reason { get; private set; } = ExceptionReason.Error;

		public DateTime ErrorTime { get; private set; }

		public BankFeedImportException(SerializationInfo info, StreamingContext context)
				: base(info, context)
		{

		}

		public BankFeedImportException(string message) : base(message)
		{
			ErrorTime = PXTimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, LocaleInfo.GetTimeZone());
		}

		public BankFeedImportException(string message, BankFeedException ex) : base(message, ex)
		{
			if (ex.Reason == BankFeedException.ExceptionReason.LoginFailed)
			{
				Reason = ExceptionReason.LoginFailed;
			}
			ErrorTime = PXTimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, LocaleInfo.GetTimeZone());
		}

		public BankFeedImportException(string message, ExceptionReason reason) : base(message)
		{
			if (reason == ExceptionReason.LoginFailed)
			{
				Reason = ExceptionReason.LoginFailed;
			}
			ErrorTime = PXTimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, LocaleInfo.GetTimeZone());
		}
	}
}
