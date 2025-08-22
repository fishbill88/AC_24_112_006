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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Holds the values for processing statuses.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ProcessingStatus
	{
		/// <summary>
		/// Open processing status.
		/// </summary>
		[EnumMember(Value = "Open")]
		Open = 0,

		/// <summary>
		/// Closed processing status.
		/// </summary>
		[EnumMember(Value = "Closed")]
		Closed = 1
	}
}
