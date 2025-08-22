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

namespace PX.Commerce.BigCommerce.API.REST
{
	public class ProductRestDataProviderFactory : IBCRestDataProviderFactory<IStockRestDataProvider<ProductData>>
	{
		public virtual IStockRestDataProvider<ProductData> CreateInstance(IBigCommerceRestClient restClient)
		{
			return new ProductRestDataProvider(restClient);
		}
	}

	public class ProductRestDataProvider : RestDataProviderV3, IStockRestDataProvider<ProductData>
	{
		private const string id_string = "id";
		protected override string GetListUrl { get; } = "v3/catalog/products";
		//protected override string GetFullListUrl { get; } = "v3/catalog/products?include=variants,images,custom_fields,primary_image,bulk_pricing_rules";
		protected override string GetSingleUrl { get; } = "v3/catalog/products/{id}";

		public ProductRestDataProvider(IBigCommerceRestClient restClient) : base()
		{
			_restClient = restClient;
		}

		#region IParentRestDataProvider
		public virtual async IAsyncEnumerable<ProductData> GetAll(IFilter filter = null, CancellationToken cancellationToken = default)
		{
			await foreach(var data in GetAll<ProductData, ProductList>(filter, cancellationToken: cancellationToken))
				yield return data;
		}
		public virtual async Task<ProductData> GetByID(string id)
		{
			return await GetByID(id, null);
		}

		public virtual async Task<ProductData> GetByID(string id, IFilter filter = null)
		{
			var segments = MakeUrlSegments(id);
			var result = await GetByID<ProductData, Product>(segments, filter);
			return result?.Data;
		}

		public virtual async Task<ProductData> Create(ProductData productData)
		{
				var product = new Product { Data = productData };
				var result =await base.Create<ProductData, Product>(product);
				return result?.Data;
		}

		public virtual async Task<bool> Delete(string id)
		{
			var segments = MakeUrlSegments(id.ToString());
			return await Delete(segments);
		}

		public virtual async Task<bool> Delete(string id, ProductData productData)
		{
			return await Delete(id);
		}

		public virtual async Task<ProductData> Update(ProductData productData, string id)
		{
			var segments = MakeUrlSegments(id);
			var result = await Update<ProductData, Product>(productData, segments);
			return result?.Data;
		}

		public virtual async Task UpdateAllQty(List<ProductQtyData> productDatas, Func<ItemProcessCallback<ProductQtyData>,Task> callback)
		{
			var product = new ProductQtyList { Data = productDatas };
			await UpdateAll<ProductQtyData, ProductQtyList>(product, new UrlSegments(), callback);
		}
		public virtual async Task UpdateAllRelations(List<RelatedProductsData> productDatas, Func<ItemProcessCallback<RelatedProductsData>,Task> callback)
		{
			var product = new RelatedProductsList { Data = productDatas };
			await UpdateAll<RelatedProductsData, RelatedProductsList>(product, new UrlSegments(), callback);
		}
		#endregion
	}
}
