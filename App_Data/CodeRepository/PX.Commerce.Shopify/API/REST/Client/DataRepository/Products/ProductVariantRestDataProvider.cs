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
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Shopify.API.REST
{
	public class ProductVariantRestDataProviderFactory : ISPRestDataProviderFactory<IChildRestDataProvider<ProductVariantData>>
	{
		public virtual IChildRestDataProvider<ProductVariantData> CreateInstance(IShopifyRestClient restClient) => new ProductVariantRestDataProvider(restClient);
	}

	public class ProductVariantRestDataProvider : RestDataProviderBase, IChildRestDataProvider<ProductVariantData>
	{
		protected override string GetListUrl { get; } = "products/{parent_id}/variants.json";
		protected override string GetSingleUrl { get; } = "products/{parent_id}/variants/{id}.json"; //The same API url : variants/{id}.json
		protected string GetAllUrl { get; } = "variants.json";
		protected override string GetSearchUrl => throw new NotImplementedException();

		public ProductVariantRestDataProvider(IShopifyRestClient restClient) : base()
		{
			ShopifyRestClient = restClient;
		}

		public virtual async Task<ProductVariantData> Create(ProductVariantData entity, string productId)
		{
			var segments = MakeParentUrlSegments(productId);
			return await base.Create<ProductVariantData, ProductVariantResponse>(entity, segments);
		}

		public virtual async Task<ProductVariantData> Update(ProductVariantData entity, string productId, string variantId)
		{
			var segments = MakeUrlSegments(variantId, productId);
			return await Update<ProductVariantData, ProductVariantResponse>(entity, segments);
		}

		public virtual async Task<bool> Delete(string productId, string variantId)
		{
			var segments = MakeUrlSegments(variantId, productId);
			return await Delete(segments);
		}

		public virtual async IAsyncEnumerable<ProductVariantData> GetAll(string productId, IFilter filter = null, CancellationToken cancellationToken = default)
		{
			var segments = MakeParentUrlSegments(productId);
			 await foreach(var data in GetAll<ProductVariantData, ProductVariantsResponse>(filter, segments, cancellationToken))
				yield return data;
		}

		public virtual async Task<ProductVariantData> GetByID(string productId, string variantId)
		{
			var segments = MakeUrlSegments(variantId, productId);
			return await GetByID<ProductVariantData, ProductVariantResponse>(segments);
		}

		public virtual async IAsyncEnumerable<ProductVariantData> GetAllWithoutParent(IFilter filter = null)
		{
			var request = BuildRequest(GetAllUrl, nameof(GetAllWithoutParent), null, filter);
			await  foreach(var data in ShopifyRestClient.GetAll<ProductVariantData, ProductVariantsResponse>(request))
				yield return data;
		}
	}
}
