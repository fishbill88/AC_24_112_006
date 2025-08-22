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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
	// https://developer-docs.amazon.com/sp-api/docs/listings-items-api-v2021-08-01-reference
	public class ListingsDataProvider : RestDataProviderBase
	{
		private IEnumerable<string> _includedData = new List<string> { "summaries", "fulfillmentAvailability", "issues", "offers", "procurement" };

		protected override string GetListUrl => throw new System.NotImplementedException();

		protected override string GetSingleUrl => "/listings/2021-08-01/items/{sellerId}/{sku}";

		public ListingsDataProvider(IAmazonRestClient restClient)
			: base(restClient)
		{
		}

		public virtual async Task<ListingItem> GetListing(string sellerPartnerId, string SKU, string marketplaceId)
		{
			var marketplaceIds = new List<string> { marketplaceId };
			return await GetListing(sellerPartnerId, SKU, marketplaceIds);
		}

		public virtual async Task<int?> GetListingQuantity(string sellerPartnerId, string SKU, string marketplaceId)
		{
			var marketplaceIds = new List<string> { marketplaceId };
			ListingItem listingItem = await GetListing(sellerPartnerId, SKU, marketplaceIds);

			return listingItem
				.FulfillmentAvailability?
				.FirstOrDefault()?
				.Quantity;
		}

		protected virtual async Task<ListingItem> GetListing(string sellerPartnerId, string SKU, List<string> marketplaceIds)
		{
			FilterListings listingsfilter = new FilterListings(marketplaceIds, _includedData);
			UrlSegments segments = MakeListingSegments(SKU, sellerPartnerId);

			return await base.GetByID<ListingItem>(segments, filter: listingsfilter);
		}

		private UrlSegments MakeListingSegments(string SKU, string sellerId)
		{
			var segments = new UrlSegments();
			segments.Add("sellerId", sellerId);
			segments.Add("sku", SKU);
			return segments;
		}
	}
}
