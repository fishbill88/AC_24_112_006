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

using Newtonsoft.Json;
using PX.Commerce.Core;
using PX.Commerce.Core.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Shopify.API.REST
{
    public class InventoryItemRestDataProvider : RestDataProviderBase,  IParentRestDataProvider<InventoryItemData>
    {
        protected override string GetListUrl   { get; } = "inventory_items.json";
        protected override string GetSingleUrl { get; } = "inventory_items/{id}.json";
		protected override string GetSearchUrl => throw new NotImplementedException();

		public InventoryItemRestDataProvider(IShopifyRestClient restClient) : base()
		{
            ShopifyRestClient = restClient;
		}

		public virtual async Task<InventoryItemData> Create(InventoryItemData entity) => throw new NotImplementedException();

		public virtual async  Task<InventoryItemData> Update(InventoryItemData entity) => await Update(entity, entity.Id.ToString());
		public virtual async Task<InventoryItemData> Update(InventoryItemData entity, string id)
		{
			var segments = MakeUrlSegments(id);
			return await base.Update<InventoryItemData, InventoryItemResponse>(entity, segments);
		}

		public virtual async Task<bool> Delete(InventoryItemData entity, string id) => await Delete(id);

		public virtual async Task<bool> Delete(string id)
		{
			var segments = MakeUrlSegments(id);
			return await Delete(segments);
		}

		public virtual async IAsyncEnumerable<InventoryItemData> GetAll(IFilter filter = null, CancellationToken cancellationToken = default)
		{
			await foreach(var data in GetAll<InventoryItemData, InventoryItemsResponse>(filter, cancellationToken: cancellationToken))
				yield return data;
		}

		public virtual async Task<InventoryItemData> GetByID(string id)
		{
			var segments = MakeUrlSegments(id);
			var entity = await base.GetByID<InventoryItemData, InventoryItemResponse>(segments);
			return entity;
		}
	}
}
