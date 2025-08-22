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
	public class InventoryLocationRestDataProviderFactory : ISPRestDataProviderFactory<IParentRestDataProvider<InventoryLocationData>>
	{
		/// <summary>
		/// Creates a <see cref="InventoryLocationRestDataProvider"/>.
		/// </summary>
		/// <param name="restClient"></param>
		/// <returns>A new a <see cref="InventoryLocationRestDataProvider"/>.</returns>
		public IParentRestDataProvider<InventoryLocationData> CreateInstance(IShopifyRestClient restClient) => new InventoryLocationRestDataProvider(restClient);
	}

	public class InventoryLocationRestDataProvider : RestDataProviderBase, IParentRestDataProvider<InventoryLocationData>
	{
		protected override string GetListUrl { get; } = "locations.json";
		protected override string GetSingleUrl { get; } = "locations/{id}.json";
		protected override string GetSearchUrl => throw new NotImplementedException();
		protected string GetLevelsUrl { get; } = "locations/{id}/inventory_levels.json";

		public InventoryLocationRestDataProvider(IShopifyRestClient restClient) : base()
		{
			ShopifyRestClient = restClient;
		}

		public virtual async Task<InventoryLocationData> Create(InventoryLocationData entity) => throw new NotImplementedException();

		public virtual async Task<InventoryLocationData> Update(InventoryLocationData entity) => throw new NotImplementedException();
		public virtual async Task<InventoryLocationData> Update(InventoryLocationData entity, string id) => throw new NotImplementedException();

		public virtual async Task<bool> Delete(InventoryLocationData entity, string id) => throw new NotImplementedException();

		public virtual async Task<bool> Delete(string id) => throw new NotImplementedException();

		public virtual async IAsyncEnumerable<InventoryLocationData> GetAll(IFilter filter = null, CancellationToken cancellationToken = default)
		{
			await foreach (var data in GetAll<InventoryLocationData, InventoryLocationsResponse>(filter, cancellationToken: cancellationToken))
				yield return data;
		}

		public virtual async Task<InventoryLocationData> GetByID(string id)
		{
			var segments = MakeUrlSegments(id);
			var entity = await base.GetByID<InventoryLocationData, InventoryLocationResponse>(segments);
			return entity;
		}
	}
}
