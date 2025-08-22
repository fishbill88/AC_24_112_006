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
	///<inheritdoc/>
	public class PriceListRestDataProviderFactory : IBCRestDataProviderFactory<IPriceListRestDataProvider>
	{
		///<inheritdoc/>
		public virtual IPriceListRestDataProvider CreateInstance(IBigCommerceRestClient restClient) => new PriceListRestDataProvider(restClient);
	}

	///<inheritdoc/>
	public class PriceListRecordRestDataProviderFactory : IBCRestDataProviderFactory<IPriceListRecordRestDataProvider>
	{
		///<inheritdoc/>
		public virtual IPriceListRecordRestDataProvider CreateInstance(IBigCommerceRestClient restClient) => new PriceListRecordRestDataProvider(restClient);
	}

	///<inheritdoc/>
	public class PriceListRestDataProvider : RestDataProviderV3, IPriceListRestDataProvider
	{
		///<inheritdoc/>		
		protected override string GetListUrl { get; } = "/v3/pricelists";
		///<inheritdoc/>
		protected override string GetSingleUrl { get; } = "v3/pricelists/{id}";
		///<inheritdoc/>
		public PriceListRestDataProvider(IBigCommerceRestClient restClient) : base()
		{
			_restClient = restClient;
		}

		///<inheritdoc/>
		public virtual async Task<PriceList> Create(PriceList priceList)
		{
			return (await Create<PriceList, PriceListResponse>(priceList))?.Data;
		}

		///<inheritdoc/>
		public virtual async IAsyncEnumerable<PriceList> GetAll(CancellationToken cancellationToken = default)
		{
			await foreach (var data in
				GetAll<PriceList, PriceListsResponse>(cancellationToken: cancellationToken))
				yield return data;
		}

		///<inheritdoc/>
		public virtual async Task<bool> DeletePriceList(string priceListId)
		{
			var segment = MakeUrlSegments(priceListId);
			return (await Delete(segment));
		}
	}

	///<inheritdoc/>
	public class PriceListRecordRestDataProvider : PriceListRestDataProvider, IPriceListRecordRestDataProvider
	{

		protected const int PRICELIST_BATCH_SIZE = 1000;

		///<inheritdoc/>
		protected override string GetListUrl { get; } = "v3/pricelists/{parent_id}/records";
		///<inheritdoc/>
		protected override string GetSingleUrl { get; } = "v3/pricelists/{parent_id}/records/{id}/{other_param}";

		/// <summary>
		/// Price List Upsert accepts up to 1000 records per batch
		/// </summary>
		protected override int BatchSize
		{
			get { return PRICELIST_BATCH_SIZE; }
		}

		///<inheritdoc/>
		public PriceListRecordRestDataProvider(IBigCommerceRestClient restClient) : base(restClient) { }

		///<inheritdoc/>
		public virtual async Task Upsert(List<PriceListRecord> priceListRecords, string priceListId, Func<ItemProcessCallback<PriceListRecord>,Task> callback)
		{
			var segment = MakeParentUrlSegments(priceListId);
			await UpdateAll<PriceListRecord, PriceListRecordResponse>(new PriceListRecordResponse() { Data = priceListRecords }, segment, callback);
		}
		///<inheritdoc/>
		public virtual async IAsyncEnumerable<PriceListRecord> GetAllRecords(string priceListId, IFilter filter = null, CancellationToken cancellationToken = default)
		{
			var segment = MakeParentUrlSegments(priceListId);
			await foreach(var data in GetAll<PriceListRecord, PriceListRecordResponse>(filter: filter, urlSegments: segment, cancellationToken: cancellationToken))
				yield return data;
		}
		///<inheritdoc/>		
		public virtual async Task<bool> DeleteRecords(string priceListId, string id, string currency)
		{
			var segment = MakeUrlSegments(id, priceListId, currency);
			return (await Delete(segment));
		}
	}
}
