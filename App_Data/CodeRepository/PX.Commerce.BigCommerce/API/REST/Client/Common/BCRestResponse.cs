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

using System.Net;
using System.Net.Http.Headers;

namespace PX.Commerce.BigCommerce.API.REST
{
	public class BCRestResponse<TR> : BCRestResponse
	{
		public TR Data { get; set; }
		internal static BCRestResponse<TR> FromResponse(IBCRestResponse response)
		{
			return new BCRestResponse<TR>()
			{
				BCRestError = response.BCRestError,
				StatusCode = response.StatusCode,
				Headers = response.Headers,
				Content = response.Content,
				IsSuccessStatusCode = response.IsSuccessStatusCode
			};
		}
	}

	public class BCRestResponse : IBCRestResponse
	{
		public BCRestError BCRestError { get; set; }
		public HttpStatusCode StatusCode { get; set; }
		public HttpResponseHeaders Headers { get; set; }
		public string Content { get; set; }
		public bool IsSuccessStatusCode { get; set; }
		public object RequestBody { get; set; }
	}

	public interface IBCRestResponse
	{
		HttpStatusCode StatusCode { get; set; }
		BCRestError BCRestError { get; set; }
		object RequestBody { get; set; }
		public string Content { get; set; }
		bool IsSuccessStatusCode { get; set; }
		HttpResponseHeaders Headers { get; set; }

	}

	public class BCRestError
	{
		public string Message { get; set; }
		public string ExceptionMessage { get; set; }
	}
}
