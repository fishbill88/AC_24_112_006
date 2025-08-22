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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// API Exception
	/// </summary>
	public class ApiException : Exception
	{
		public string ResponseMessage;
		public string ResponseStatusCode;
		public string ResponseContent;
		private string _Message;
		private string _MessagePrefix;
		protected readonly IEnumerable<string> _errors;

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiException"/> class.
		/// </summary>
		public ApiException() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiException"/> class.
		/// </summary>
		/// <param name="errorCode">HTTP status code.</param>
		/// <param name="message">Error message.</param>
		public ApiException(IAmazonRestResponse response)
			: base(GetErrorMessage(response.RestError?.Message, response.StatusCode.ToString(), response.Content, GetErrorData(response)))
		{
			ResponseMessage = response.RestError.Message;
			ResponseStatusCode = response.StatusCode.ToString();
			ResponseContent = response.Content;

			_errors = GetErrorData(response);
		}

		public ApiException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_Message = info.GetString("_Message");
			_MessagePrefix = info.GetString("_MessagePrefix");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("_Message", _Message);
			info.AddValue("_MessagePrefix", _MessagePrefix);

			base.GetObjectData(info, context);
		}

		public override string ToString()
		{
			return GetErrorMessage(ResponseMessage, ResponseStatusCode, ResponseContent, _errors);
		}

		public static string GetErrorMessage(string message, string statusCode, string content, IEnumerable<string> restErrors)
		{
			StringBuilder sb = new StringBuilder();

			//Returned Errors
			bool errorsPersist = false;
			foreach (string error in restErrors)
			{
				errorsPersist = true;
				sb.AppendFormat("{0}", error);
				sb.AppendLine();
			}
			//if no errors parsed, display rough Content
			if (!errorsPersist)
			{
				//Content
				if (!string.IsNullOrWhiteSpace(content))
				{
					sb.AppendLine($"Content: {content}");
					sb.AppendLine();
				}
			}

			return sb.ToString();
		}

		public static IEnumerable<string> GetErrorData(IAmazonRestResponse response)
		{
			if (response.StatusCode == HttpStatusCode.OK ||
				response.StatusCode == HttpStatusCode.Created ||
				response.StatusCode == HttpStatusCode.Accepted ||
				response.StatusCode == HttpStatusCode.NoContent)
			{
				return Enumerable.Empty<string>();
			}

			if (string.IsNullOrWhiteSpace(response.Content))
				return Enumerable.Empty<string>();

			List<string> errorList = new List<string>();
			var jObject = IsJsonString(response.Content);

			if (jObject != null)
				DeserializeJson(jObject, string.Empty, errorList);

			return errorList;
		}

		private static void DeserializeJson(JToken content, string name, List<string> errorList)
		{
			if (content != null)
			{
				switch (content.Type)
				{
					case JTokenType.Object when content.HasValues:
						{
							foreach (var item in content.Children())
							{
								DeserializeJson(item, name, errorList);
							}
							break;
						}
					case JTokenType.Array when ((JArray)content)?.Count > 0:
						{
							foreach (var arr in ((JArray)content).Children())
							{
								DeserializeJson(arr, name, errorList);
							}
							break;
						}
					case JTokenType.Property:
						{
							var pContent = (JProperty)content;
							DeserializeJson(IsJsonString(pContent?.Value.ToString()) ?? pContent.Value, pContent?.Name ?? name, errorList);
							break;
						}
					default:
						{
							var value = ((JValue)content)?.ToString();
							if (!string.IsNullOrWhiteSpace(value))
							{
								errorList.Add(string.IsNullOrWhiteSpace(name) ? value : $"{name} : {value}");
							}
							break;
						}
				}
			}
		}

		private static JObject IsJsonString(string value)
		{
			try
			{
				return JObject.Parse(value);
			}
			catch
			{
				return null;
			}
		}
	}
}
