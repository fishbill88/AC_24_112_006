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

using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PX.Commerce.Amazon.API.Rest
{
	public abstract class FeedDataProvider : RestDataProviderBase
	{
		public abstract string FeedContentType { get; }
		// max supported size for feed document can be found in this link
		// https://developer-docs.amazon.com/sp-api/docs/feeds-api-best-practices
		public const double feedSize = 9.5;

		public const int WAIT_DELAY = 5000;

		protected string CreateFeedDocumentUrl { get; } = "/feeds/2021-06-30/documents";

		protected string CreateFeedUrl { get; } = "/feeds/2021-06-30/feeds";

		protected string GetSingleFeedDocumentUrl { get; } = "/feeds/2021-06-30/documents/{id}";

		protected override string GetListUrl => throw new NotImplementedException();

		protected override string GetSingleUrl { get; } = "/feeds/2021-06-30/feeds/{id}";

		protected FeedDataProvider(IAmazonRestClient restClient)
			: base(restClient)
		{
		}

		protected CreateFeedSpecification CreateFeedSpecification(string feedType, string marketplace)
		{
			return new()
			{
				FeedType = feedType,
				MarketplaceIds = new List<string> { marketplace }
			};
		}

		public async Task<FeedDocument> SubmitAndProcessFeedAsync(CreateFeedSpecification specs, string feedData)
		{
			// Step 1: create a feed document
			CreateFeedDocumentResponse createFeedDocResult = await CreateFeedDocumentAsync();

			// Step 2: upload feed document data using pre-signed URL returned from step 1
			await base.UploadFeedAsync(feedData, createFeedDocResult.Url, FeedContentType);

			specs.InputFeedDocumentId = createFeedDocResult.FeedDocumentId;
			// Step 3: create a feed associating to the newly created feed document in step 2
			CreateFeedResponse createFeedResult = await CreateFeedAsync(specs);

			// Step 4: get the newly created feed in step 3 to check status of feed processing
			Feed feed = await GetFeedByIdAsync(createFeedResult.FeedId);
			// when feed is first submitted, before getting processed by Amazon, its status will be first InQueue and then InProgress
			// thus, repeatedly check processing status until the processing is finished
			while (feed.ProcessingStatus == FeedProcessingStatus.InProgress || feed.ProcessingStatus == FeedProcessingStatus.InQueue)
			{
				await Task.Delay(WAIT_DELAY);
				feed = await GetFeedByIdAsync(createFeedResult.FeedId);
			}

			// feed processing is finished but it can be FATAL or CANCELLED
			if (feed.ProcessingStatus != FeedProcessingStatus.Done)
				throw new PXException(PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.FeedProcessingUnexpectedStatus, feed.ProcessingStatus));

			// Step 5: get feed document which will be created to contain feed processing result once it's done
			return await GetFeedDocumentByIdAsync(feed.ResultFeedDocumentId);
		}

		private async Task<CreateFeedDocumentResponse> CreateFeedDocumentAsync()
		{
			CreateFeedDocumentSpecification data = new CreateFeedDocumentSpecification()
			{
				ContentType = FeedContentType
			};
			return await base.Post<CreateFeedDocumentSpecification, CreateFeedDocumentResponse>(data, CreateFeedDocumentUrl);
		}

		private async Task<CreateFeedResponse> CreateFeedAsync(CreateFeedSpecification specs)
		{
			return await base.Post<CreateFeedSpecification, CreateFeedResponse>(specs, CreateFeedUrl);
		}

		private async Task<Feed> GetFeedByIdAsync(string id)
		{
			return await base.GetByID<Feed>(MakeUrlSegments(id), GetSingleUrl);
		}

		private async Task<FeedDocument> GetFeedDocumentByIdAsync(string id)
		{
			return await base.GetByID<FeedDocument>(MakeUrlSegments(id), GetSingleFeedDocumentUrl);
		}
	}
}
