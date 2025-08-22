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
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Shopify.API.REST
{
	public class ProductRestDataProviderFactory : ISPRestDataProviderFactory<IProductRestDataProvider<ProductData>>
	{
		public virtual IProductRestDataProvider<ProductData> CreateInstance(IShopifyRestClient restClient) => new ProductRestDataProvider(restClient);
	}

	public class ProductRestDataProvider : RestDataProviderBase, IProductRestDataProvider<ProductData>
	{
		protected override string GetListUrl { get; } = "products.json";
		protected override string GetSingleUrl { get; } = "products/{id}.json";
		protected override string GetSearchUrl => throw new NotImplementedException();
		private string GetMetafieldsUrl { get; } = "products/{id}/metafields.json";
		private string GetVariantMetafieldsUrl { get; } = "products/{parent_id}/variants/{id}/metafields.json";

		public ProductRestDataProvider(IShopifyRestClient restClient) : base()
		{
			ShopifyRestClient = restClient;
		}

		public virtual async Task<ProductData> Create(ProductData entity)
		{
			return await base.Create<ProductData, ProductResponse>(entity);
		}

		public virtual async Task<ProductData> Update(ProductData entity) => await Update(entity, entity.Id.ToString());
		public virtual async Task<ProductData> Update(ProductData entity, string productId)
		{
			var segments = MakeUrlSegments(productId);
			return await base.Update<ProductData, ProductResponse>(entity, segments);
		}

		public virtual async Task<bool> Delete(ProductData entity, string productId) => await Delete(productId);

		public virtual async Task<bool> Delete(string productId)
		{
			var segments = MakeUrlSegments(productId);
			return await Delete(segments);
		}

		public virtual async IAsyncEnumerable<ProductData> GetAll(IFilter filter = null, CancellationToken cancellationToken = default)
		{
			await foreach(var data in GetAll<ProductData, ProductsResponse>(filter, cancellationToken: cancellationToken))
				yield return data;
		}

		public virtual async Task<ProductData> GetByID(string productId) => await GetByID(productId, false);

		public virtual async Task<ProductData> GetByID(string productId, bool includedMetafields = false, CancellationToken cancellationToken = default)
		{
			var segments = MakeUrlSegments(productId);
			var entity =await  base.GetByID<ProductData, ProductResponse>(segments);
			if (entity != null && includedMetafields == true)
			{
				entity.Metafields = await GetMetafieldsForProduct(productId, cancellationToken);
				foreach (var variant in entity.Variants)
				{
					variant.VariantMetafields = await GetMetafieldsForProductVariant(productId, variant.Id?.ToString(), cancellationToken);
				}
			}
			return entity;
		}

		public virtual async Task<List<MetafieldData>> GetMetafieldsForProduct(string productId, CancellationToken cancellationToken = default)
		{
			var metaFields = new List<MetafieldData>();
			var request = BuildRequest(GetMetafieldsUrl, nameof(GetMetafieldsForProduct), MakeUrlSegments(productId), null);
			 await foreach(var data in ShopifyRestClient.GetAll<MetafieldData, MetafieldsResponse>(request, cancellationToken))
				metaFields.Add(data);
			 return metaFields;
		}

		public virtual async Task<List<MetafieldData>> GetMetafieldsForProductVariant(string productId, string variantId, CancellationToken cancellationToken = default)
		{
			var metaFields = new List<MetafieldData>();
			var request = BuildRequest(GetVariantMetafieldsUrl, nameof(GetMetafieldsForProductVariant), MakeUrlSegments(variantId, productId), null);
			 await foreach(var data in ShopifyRestClient.GetAll<MetafieldData, MetafieldsResponse>(request, cancellationToken))
				metaFields.Add(data);
			 return metaFields;
		}
	}
}
