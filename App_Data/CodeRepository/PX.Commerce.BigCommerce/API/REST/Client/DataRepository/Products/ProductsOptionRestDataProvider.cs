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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce.API.REST
{
	public class ProductsOptionRestDataProviderFactory : IBCRestDataProviderFactory<IChildRestDataProvider<ProductsOptionData>>
	{
		public virtual IChildRestDataProvider<ProductsOptionData> CreateInstance(IBigCommerceRestClient restClient) => new ProductsOptionRestDataProvider(restClient);
	}

	public class ProductsOptionRestDataProvider : RestDataProviderV3, IChildRestDataProvider<ProductsOptionData>
    {
        protected override string GetListUrl { get; } = "v3/catalog/products/{parent_id}/options";
        protected override string GetSingleUrl { get; } = "v3/catalog/products/{parent_id}/options/{id}";

        public ProductsOptionRestDataProvider(IBigCommerceRestClient restClient) : base()
		{
            _restClient = restClient;
		}

		#region IChildRestDataProvider
		public virtual async Task<ProductsOptionData> Create(ProductsOptionData productsOptionData, string parentId)
        {
            var segments = MakeParentUrlSegments(parentId);
            var productsOption = new ProductsOption { Data = productsOptionData };
            return (await Create<ProductsOptionData, ProductsOption>(productsOption, segments)).Data;
        }

		public virtual async Task<ProductsOptionData> Update(ProductsOptionData productsOptionData, string id, string parentId)
        {
            var segments = MakeUrlSegments(id, parentId);
            return (await Update<ProductsOptionData, ProductsOption>(productsOptionData, segments)).Data;
        }

		public virtual async Task<bool> Delete(string id, string parentId)
        {
            var segments = MakeUrlSegments(id, parentId);
            return await base.Delete(segments);
        }

        public virtual async Task<ProductsOptionData> GetByID(string id, string parentId)
        {
            var segments = MakeUrlSegments(id, parentId);
            return (await GetByID<ProductsOptionData, ProductsOption>(segments)).Data;
        }

		public virtual async IAsyncEnumerable<ProductsOptionData> GetAll(string parentId, CancellationToken cancellationToken = default)
        {
            var segments = MakeParentUrlSegments(parentId);
            await  foreach(var data in GetAll<ProductsOptionData, ProductsOptionList>(null, segments, cancellationToken))
				yield return data;
        }
        #endregion
    }
}
