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
using static PX.Data.BQL.BqlPlaceholder;

namespace PX.Commerce.BigCommerce.API.REST
{
	public class ProductVariantRestDataProviderFactory : IBCRestDataProviderFactory<IChildRestDataProvider<ProductsVariantData>>
	{
		public virtual IChildRestDataProvider<ProductsVariantData> CreateInstance(IBigCommerceRestClient restClient)
		{
			return new ProductVariantRestDataProvider(restClient); ;
		}
	}

	public class ProductVariantBatchRestDataProviderFactory : IBCRestDataProviderFactory<IChildUpdateAllRestDataProvider<ProductsVariantData>>
	{
		public virtual IChildUpdateAllRestDataProvider<ProductsVariantData> CreateInstance(IBigCommerceRestClient restClient)
		{
			return new ProductVariantBatchRestDataProvider(restClient);
		}
	}

	public class ProductVariantRestDataProvider : RestDataProviderV3, IChildRestDataProvider<ProductsVariantData>
	{
		protected override string GetListUrl { get; } = "v3/catalog/products/{parent_id}/variants";
		protected override string GetSingleUrl { get; } = "v3/catalog/products/{parent_id}/variants/{id}";
		protected string GetFullListUrl { get; } = "v3/catalog/products/{parent_id}/variants";

		public ProductVariantRestDataProvider(IBigCommerceRestClient restClient) : base()
		{
			_restClient = restClient;
		}

		public virtual async Task<ProductsVariantData> GetByID(string id, string parentId)
		{
			var segments = MakeUrlSegments(id, parentId);
			return (await GetByID<ProductsVariantData, ProductsVariant>(segments)).Data;
		}

		public virtual async IAsyncEnumerable<ProductsVariantData> GetAll(string parentId, CancellationToken cancellationToken = default)
		{
			var segments = MakeParentUrlSegments(parentId);
			await foreach (var data in GetAll<ProductsVariantData, ProductVariantList>(null, segments, cancellationToken))
				yield return data;

		}

		public virtual async Task<ProductsVariantData> Create(ProductsVariantData productsVariantData, string parentId)
		{
			var productsVariant = new ProductsVariant { Data = productsVariantData };
			var segments = MakeParentUrlSegments(parentId);
			return (await Create<ProductsVariantData, ProductsVariant>(productsVariant, segments)).Data;
		}

		public virtual async Task<ProductsVariantData> Update(ProductsVariantData productsVariantData, string id, string parentId)
		{
			var segments = MakeUrlSegments(id, parentId);
			var productVariant = new ProductsVariant { Data = productsVariantData };
			return (await Update<ProductsVariantData, ProductsVariant>(productVariant, segments)).Data;
		}

		public virtual async Task<bool> Delete(string id, string parentId)
		{
			var segments = MakeUrlSegments(id, parentId);
			return await base.Delete(segments);
		}
	}
	public class ProductVariantBatchRestDataProvider : ProductVariantRestDataProvider, IChildUpdateAllRestDataProvider<ProductsVariantData>
	{
		protected override string GetListUrl { get; } = "v3/catalog/variants";
		public ProductVariantBatchRestDataProvider(IBigCommerceRestClient restClient) : base(restClient)
		{
		}

		public virtual async Task UpdateAll(List<ProductsVariantData> productDatas, Func<ItemProcessCallback<ProductsVariantData>,Task> callback)
		{
			var product = new ProductVariantList { Data = productDatas };
			await UpdateAll<ProductsVariantData, ProductVariantList>(product, new UrlSegments(), callback);
		}

		/// <summary>
		/// Returns a list of product Variants own by the <paramref name="parentId"/>.<br/>
		/// <seealso href="https://developer.bigcommerce.com/api-reference/02db3ddfc6be7-get-all-product-variants">Link for more details.</seealso>
		/// </summary>
		/// <param name="parentId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public virtual async Task<List<ProductsVariantData>> GetVariants(string parentId, CancellationToken cancellationToken = default)
		{
			var segments = MakeParentUrlSegments(parentId);
			var request = BuildRequest(GetFullListUrl,urlSegments:segments);

			return (await _restClient.GetList<ProductsVariantData, ProductVariantList>(request))?.Data;
		}
		
	}
}
