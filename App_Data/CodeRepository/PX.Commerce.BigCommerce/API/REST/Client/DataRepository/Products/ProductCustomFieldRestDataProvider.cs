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
	public class ProductCustomFieldRestDataProviderFactory : IBCRestDataProviderFactory<IChildRestDataProvider<ProductsCustomFieldData>>
	{
		public virtual IChildRestDataProvider<ProductsCustomFieldData> CreateInstance(IBigCommerceRestClient restClient) => new ProductCustomFieldRestDataProvider(restClient);
	}

	public class ProductCustomFieldRestDataProvider : RestDataProviderV3, IChildRestDataProvider<ProductsCustomFieldData>
    {
        private const string id_string = "id";

        protected override string GetListUrl { get; } = "v3/catalog/products/{parent_id}/custom-fields";
        protected override string GetSingleUrl { get; } = "v3/catalog/products/{parent_id}/custom-fields/{id}";

        public ProductCustomFieldRestDataProvider(IBigCommerceRestClient restClient) : base()
		{
            _restClient = restClient;
		}

		#region IChildRestDataProvider
		public virtual async Task<ProductsCustomFieldData> Create(ProductsCustomFieldData productsCustomFieldData, string parentId)
        {
            var segments = MakeParentUrlSegments(parentId);
            return (await Create<ProductsCustomFieldData, ProductsCustomField>(productsCustomFieldData, segments)).Data;
        }

		public virtual async Task<ProductsCustomFieldData> Update(ProductsCustomFieldData productsCustomFieldData, string id, string parentId)
        {
            var segments = MakeUrlSegments(id, parentId);

            return (await Update<ProductsCustomFieldData, ProductsCustomField>(productsCustomFieldData, segments)).Data;
        }

		public virtual async Task<bool> Delete(string id, string parentId)
        {
            var segments = MakeUrlSegments(id, parentId);
            return await base.Delete(segments);
        }

        public virtual async Task<ProductsCustomFieldData> GetByID(string id, string parentId)
        {
            var segments = MakeUrlSegments(id, parentId);
            return (await GetByID<ProductsCustomFieldData, ProductsCustomField>(segments)).Data;
        }

		public virtual async IAsyncEnumerable<ProductsCustomFieldData> GetAll(string parentId, CancellationToken cancellationToken = default)
        {
            var segments = MakeParentUrlSegments(parentId);
            await foreach(var data in GetAll<ProductsCustomFieldData, ProductsCustomFieldList>(null, segments))
				yield return data;
        }
        #endregion
    }
}
