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

namespace PX.Commerce.BigCommerce.API.REST
{
	public class ProductBulkPricingRestDataProviderFactory : IBCRestDataProviderFactory<IChildRestDataProvider<ProductsBulkPricingRules>>
	{
		public virtual IChildRestDataProvider<ProductsBulkPricingRules> CreateInstance(IBigCommerceRestClient restClient)
		{
			return new ProductBulkPricingRestDataProvider(restClient);
		}
	}
	public class ProductBatchBulkRestDataProviderFactory : IBCRestDataProviderFactory<IChildUpdateAllRestDataProvider<BulkPricingWithSalesPrice>>
	{
		public virtual IChildUpdateAllRestDataProvider<BulkPricingWithSalesPrice> CreateInstance(IBigCommerceRestClient restClient)
		{
			return new ProductBatchBulkRestDataProvider(restClient);
		}
	}

	public class ProductBulkPricingRestDataProvider : RestDataProviderV3, IChildRestDataProvider<ProductsBulkPricingRules>
	{
		protected override string GetListUrl { get; } = "v3/catalog/products/{parent_id}/bulk-pricing-rules";

		protected override string GetSingleUrl { get; } = "v3/catalog/products/{parent_id}/bulk-pricing-rules/{id}";

		public ProductBulkPricingRestDataProvider(IBigCommerceRestClient client)
		{
			_restClient = client;
		}
		public virtual async Task<ProductsBulkPricingRules> Create(ProductsBulkPricingRules pricingRules, string parentId)
		{
			var segments = MakeParentUrlSegments(parentId);
			return (await Create<ProductsBulkPricingRules, BulkPricing>(pricingRules, segments)).Data;
		}
		public virtual async IAsyncEnumerable<ProductsBulkPricingRules> GetAll(string parentId, CancellationToken cancellationToken = default)
		{
			var segments = MakeParentUrlSegments(parentId);
			await foreach (var data in GetAll<ProductsBulkPricingRules, BulkPricingList>(urlSegments: segments, cancellationToken: cancellationToken))
				yield return data;
		}

		public virtual async Task<bool> Delete(string id, string parentId)
		{
			var segments = MakeUrlSegments(id, parentId);
			return (await Delete(urlSegments: segments));
		}

		public virtual async Task<ProductsBulkPricingRules> Update(ProductsBulkPricingRules productData, string id, string parentId)
		{
			var segments = MakeUrlSegments(id, parentId);
			var result = (await Update<ProductsBulkPricingRules, BulkPricing>(productData, segments));
			return result.Data;
		}

		public async Task<ProductsBulkPricingRules> GetByID(string id, string parentId)
		{
			throw new NotImplementedException();
		}
	}

	public class ProductBatchBulkRestDataProvider : ProductBulkPricingRestDataProvider, IChildUpdateAllRestDataProvider<BulkPricingWithSalesPrice>
	{
		protected override string GetListUrl { get; } = "v3/catalog/products?include=bulk_pricing_rules";
		public ProductBatchBulkRestDataProvider(IBigCommerceRestClient restClient) : base(restClient)
		{
		}

		public virtual async Task UpdateAll(List<BulkPricingWithSalesPrice> productDatas, Func<ItemProcessCallback<BulkPricingWithSalesPrice>,Task> callback)
		{
			var product = new BulkPricingListWithSalesPrice { Data = productDatas };
			await UpdateAll<BulkPricingWithSalesPrice, BulkPricingListWithSalesPrice>(product, new UrlSegments(), callback);
		}

		public Task<List<BulkPricingWithSalesPrice>> GetVariants(string parentId, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}
