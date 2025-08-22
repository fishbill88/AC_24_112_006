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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Shopify.API.REST
{
	public class ProductImageRestDataProviderFactory : ISPRestDataProviderFactory<IChildRestDataProvider<ProductImageData>>
	{
		public virtual IChildRestDataProvider<ProductImageData> CreateInstance(IShopifyRestClient restClient) => new ProductImageRestDataProvider(restClient);
	}

	public class ProductImageRestDataProvider : RestDataProviderBase, IChildRestDataProvider<ProductImageData>
	{
		protected override string GetListUrl { get; } = "products/{parent_id}/images.json";
		protected override string GetSingleUrl { get; } = "products/{parent_id}/images/{id}.json";
		protected override string GetSearchUrl => throw new NotImplementedException();
		private string GetMetafieldsUrl { get; } = "metafields.json?metafield[owner_id]={0}&metafield[owner_resource]=product_image";

		public ProductImageRestDataProvider(IShopifyRestClient restClient) : base()
		{
			ShopifyRestClient = restClient;
		}

		public virtual async Task<ProductImageData> Create(ProductImageData entity, string productId)
		{
			var segments = MakeParentUrlSegments(productId);
			return await base.Create<ProductImageData, ProductImageResponse>(entity, segments);
		}

		public virtual async Task<ProductImageData> Update(ProductImageData entity, string productId, string imageId)
		{
			var segments = MakeUrlSegments(imageId, productId);
			return await Update<ProductImageData, ProductImageResponse>(entity, segments);
		}

		public virtual async Task<bool> Delete(string productId, string imageId)
		{
			var segments = MakeUrlSegments(imageId, productId);
			return await Delete(segments);
		}

		public virtual async IAsyncEnumerable<ProductImageData> GetAll(string productId, IFilter filter = null, CancellationToken cancellationToken = default)
		{
			var segments = MakeParentUrlSegments(productId);
			await foreach (var oneItem in GetAll<ProductImageData, ProductImagesResponse>(filter, segments, cancellationToken))
			{
				oneItem.Metafields = await GetMetafieldsByImageId(oneItem.Id.ToString(), cancellationToken);
				yield return oneItem;
			}

			yield break;
		}

		public virtual async Task<ProductImageData> GetByID(string productId, string imageId)
		{
			var segments = MakeUrlSegments(imageId, productId);
			var image = await GetByID<ProductImageData, ProductImageResponse>(segments);
			if (image != null) image.Metafields = await GetMetafieldsByImageId(imageId);
			return image;
		}

		public virtual IAsyncEnumerable<ProductImageData> GetAllWithoutParent(IFilter filter = null)
		{
			throw new NotImplementedException();
		}

		public virtual async Task<List<MetafieldData>> GetMetafieldsByImageId(string imageId, CancellationToken cancellationToken = default)
		{
			List<MetafieldData> metafields = new List<MetafieldData>();
			var request = BuildRequest(string.Format(GetMetafieldsUrl, imageId), nameof(GetMetafieldsByImageId), null, null);
			await foreach (var data in ShopifyRestClient.GetAll<MetafieldData, MetafieldsResponse>(request, cancellationToken))
				metafields.Add(data);
			return metafields;
		}
	}
}
