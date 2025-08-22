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
	public class ProductImagesDataProviderFactory : IBCRestDataProviderFactory<IChildRestDataProvider<ProductsImageData>>
	{
		public virtual IChildRestDataProvider<ProductsImageData> CreateInstance(IBigCommerceRestClient restClient)
		{
			return new ProductImagesDataProvider(restClient); ;
		}
	}

	public class ProductImagesDataProvider : RestDataProviderV3, IChildRestDataProvider<ProductsImageData>
	{
		protected override string GetListUrl { get; } = "v3/catalog/products/{parent_id}/images";
		protected override string GetSingleUrl { get; } = "v3/catalog/products/{parent_id}/images/{id}";

		public ProductImagesDataProvider(IBigCommerceRestClient restClient) : base()
		{
			_restClient = restClient;
		}

		public virtual async Task<ProductsImageData> Create(ProductsImageData productsImageData, string parentId)
		{
			var segments = MakeParentUrlSegments(parentId);
			return (await Create<ProductsImageData, ProductsImage>(productsImageData, segments))?.Data;
		}

		public virtual async Task<ProductsImageData> Update(ProductsImageData productsImageData, string id,string parentId)
		{
			var segments = MakeUrlSegments(id, parentId);
			return (await Update<ProductsImageData, ProductsImage>(productsImageData, segments))?.Data;
		}
		public virtual async Task<bool> Delete(string id, string parentId)
		{
			var segments = MakeUrlSegments(id, parentId);
			return await base.Delete(segments);
		}

		public virtual async IAsyncEnumerable<ProductsImageData> GetAll(string parentId, CancellationToken cancellationToken = default)
		{
			var segments = MakeParentUrlSegments(parentId);
			await foreach(var data in GetAll<ProductsImageData, ProductsImageList>(null, segments, cancellationToken))
				yield return data;
		}

		public virtual async Task<ProductsImageData> GetByID(string id, string parentId)
		{
			var segments = MakeUrlSegments(id, parentId);
			return (await GetByID<ProductsImageData, ProductsImage>(segments)).Data;
		}

		#region Not implemented 

		public virtual async Task<int> Count(string parentId)
		{
			throw new System.NotImplementedException();
		}
		#endregion
	}
}
