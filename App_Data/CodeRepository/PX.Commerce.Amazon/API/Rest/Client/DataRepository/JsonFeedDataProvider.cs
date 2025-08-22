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

#nullable enable

using Newtonsoft.Json;
using PX.Commerce.Amazon.API.Rest.Client.Common;
using PX.Commerce.Amazon.API.Rest.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
	internal class JsonFeedDataProvider : FeedDataProvider, IJsonFeedDataProvider
	{
		IAmazonReportReader _reportReader;
		JsonFeedData _jsonFeedData;
		const string ContentType = "application/json";
		const string Version = "2.0";

		public override string FeedContentType => ContentType;
		public JsonFeedDataProvider(IAmazonRestClient restClient, JsonFeedData jsonFeedData, IAmazonReportReader reportReader) : base(restClient)
		{
			_jsonFeedData = jsonFeedData;
			_reportReader = reportReader;
		}

		public async Task<IEnumerable<JsonFeedProcessingResult<TMessage>>> SendFeedAsync<TMessage>(IEnumerable<TMessage> messages) where TMessage : class
		{
			List<JsonFeedProcessingResult<TMessage>> feedProcessingResults = new();
			string jsonString = null;
			List<TMessage> currentListOfMessages = new();
			JsonFeedDocument<TMessage> document = new();
			document.Messages = new();

			double megaBytesLength = 0.0d;
			document.Header = new(_jsonFeedData.SellerId, Version);
			CreateFeedSpecification feedSpecification = CreateFeedSpecification(_jsonFeedData.FeedType, _jsonFeedData.Marketplace);

			foreach (TMessage message in messages)
			{
				document.Messages.Add(message);
				jsonString = JsonConvert.SerializeObject(document);

				// calculating size of json string before appending it
				var byteData = Encoding.UTF8.GetBytes(jsonString);
				megaBytesLength = (byteData.Length / 1024f) / 1024f;

				// if size of the json string will be larger than the max size allowed then submit it without appending current message
				if (megaBytesLength >= feedSize)
				{
					document.Messages.Remove(message);
					string trimmedJsonString = JsonConvert.SerializeObject(document);
					feedProcessingResults.Add(await SendFeedAsync(feedSpecification, currentListOfMessages, trimmedJsonString));

					document.Messages = new () { message };
					jsonString = JsonConvert.SerializeObject(document);
					currentListOfMessages = new();
				}

				// when size of json string is still less than max size then append current message
				currentListOfMessages.Add(message);
			}

			// submit any pending messages that have not yet been submitted
			feedProcessingResults.Add(await SendFeedAsync(feedSpecification, currentListOfMessages, jsonString));
			return feedProcessingResults;
		}

		private async Task<JsonFeedProcessingResult<TMessage>> SendFeedAsync<TMessage>(CreateFeedSpecification specs, List<TMessage> currentListOfMessages, string currentJsonFeedString) where TMessage : class
		{
			JsonProcessingReport currentFeedProcessingResult = null;
			try
			{
				FeedDocument feedDocument = await SubmitAndProcessFeedAsync(specs, currentJsonFeedString);
				string jsonStr = Encoding.UTF8.GetString(await _reportReader.ReadAsync(feedDocument.Url, feedDocument.CompressionAlgorithm));
				currentFeedProcessingResult = JsonConvert.DeserializeObject<JsonProcessingReport>(jsonStr);
			}
			catch (Exception ex)
			{
				currentFeedProcessingResult = new()
				{
					Issues = new() {
									new () {
										Severity = Severity.Error,
										Message = !string.IsNullOrEmpty(ex.Message) ? ex.Message : ex.GetType() == typeof(ApiException) ? ((ApiException)ex).ResponseMessage : string.Empty
									}
								}
				};
			}
			return new JsonFeedProcessingResult<TMessage>()
			{
				ProcessingReport = currentFeedProcessingResult,
				FeedMessages = currentListOfMessages
			};
		}
	}
}
