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
	public class MetafieldRestDataProvider : RestDataProviderBase, IParentRestDataProvider<MetafieldData>
	{
		protected override string GetListUrl { get; } = "metafields.json";
		protected override string GetSingleUrl { get; } = "metafields/{id}.json";
		protected override string GetSearchUrl => throw new NotImplementedException();

		public MetafieldRestDataProvider(IShopifyRestClient restClient) : base()
		{
			ShopifyRestClient = restClient;
		}

		#region IParentDataRestClient
		public virtual async Task<MetafieldData> Create(MetafieldData entity)
		{
			var result = await base.Create<MetafieldData, MetafieldResponse>(entity);
			return result;
		}

		public virtual async Task<MetafieldData> Update(MetafieldData entity, string id)
		{
			var segments = MakeUrlSegments(id);
			return await base.Update<MetafieldData, MetafieldResponse>(entity, segments);
		}

		public virtual async Task<bool> Delete(string id)
		{
			var segments = MakeUrlSegments(id);
			return await base.Delete(segments);
		}

		public virtual async IAsyncEnumerable<MetafieldData> GetAll(IFilter filter = null, CancellationToken cancellationToken = default)
		{
			 await foreach (var data in base.GetAll<MetafieldData, MetafieldsResponse>(filter, cancellationToken: cancellationToken))
				yield return data;
		}

		public virtual async Task<MetafieldData> GetByID(string id)
		{
			var segments = MakeUrlSegments(id);
			var result = await GetByID<MetafieldData, MetafieldResponse>(segments);
			return result;
		}

		public virtual async Task<MetafieldData> GetMetafieldBySpecifiedUrl(string url, string id)
		{
			var request = BuildRequest(url, nameof(GetMetafieldBySpecifiedUrl), MakeUrlSegments(id), null);
			return await ShopifyRestClient.Get<MetafieldData, MetafieldResponse>(request);
		}

		#endregion
	}
}
