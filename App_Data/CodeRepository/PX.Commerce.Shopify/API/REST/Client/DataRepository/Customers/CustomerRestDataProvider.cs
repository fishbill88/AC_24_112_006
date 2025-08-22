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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Shopify.API.REST
{
	public class CustomerRestDataProviderFactory : ISPRestDataProviderFactory<ICustomerRestDataProvider<CustomerData>>
	{
		public virtual ICustomerRestDataProvider<CustomerData> CreateInstance(IShopifyRestClient restClient) => new CustomerRestDataProvider(restClient);
	}

	public class CustomerRestDataProvider : RestDataProviderBase, ICustomerRestDataProvider<CustomerData>
	{
		protected override string GetListUrl { get; } = "customers.json";
		protected override string GetSingleUrl { get; } = "customers/{id}.json";
		protected override string GetSearchUrl { get; } = "customers/search.json";
		private string GetAccountActivationUrl { get; } = "customers/{id}/account_activation_url.json";
		private string GetSendInviteUrl { get; } = "customers/{id}/send_invite.json";
		private string GetMetafieldsUrl { get; } = "customers/{id}/metafields.json";
		private string GetAddressesUrl { get; } = "customers/{id}/addresses.json";

		public CustomerRestDataProvider(IShopifyRestClient restClient) : base()
		{
			ShopifyRestClient = restClient;
		}

		public virtual async Task<CustomerData> Create(CustomerData entity)
		{
			return await base.Create<CustomerData, CustomerResponse>(entity);
		}

		public virtual async Task<CustomerData> Update(CustomerData entity) => await Update(entity, entity.Id.ToString());
		public virtual async Task<CustomerData> Update(CustomerData entity, string customerId)
		{
			var segments = MakeUrlSegments(customerId);
			return await base.Update<CustomerData, CustomerResponse>(entity, segments);
		}

		public virtual async Task<bool> Delete(CustomerData entity, string customerId) => await Delete(customerId);

		public virtual async Task<bool> Delete(string customerId)
		{
			var segments = MakeUrlSegments(customerId);
			return await Delete(segments);
		}

		public virtual async IAsyncEnumerable<CustomerData> GetAll(IFilter filter = null, CancellationToken cancellationToken = default)
		{
			await foreach (var data in GetAll<CustomerData, CustomersResponse>(filter, cancellationToken: cancellationToken)
			   ) { yield return data; }
		}

		public virtual async Task<CustomerData> GetByID(string id) => await GetByID(id, true, false);

		public async Task<CustomerData> GetByID(string customerId, bool includedMetafields = true, bool includeAllAddresses = false, CancellationToken cancellationToken = default)
		{
			var segments = MakeUrlSegments(customerId);
			var entity = await base.GetByID<CustomerData, CustomerResponse>(segments);
			if (entity != null && includedMetafields == true)
			{
				entity.Metafields = await GetMetafieldsById(customerId, cancellationToken);
			}
			if (entity != null && includeAllAddresses == true)
			{
				entity.Addresses = await GetAddressesById(customerId, cancellationToken);
			}
			return entity;
		}

		public virtual async IAsyncEnumerable<CustomerData> GetByQuery(string fieldName, string value, bool includedMetafields = false, CancellationToken cancellationToken = default)
		{
			var url = GetSearchUrl;
			var property = typeof(CustomerData).GetProperty(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
			if (property != null)
			{
				var attr = property.GetCustomAttribute<JsonPropertyAttribute>();
				if (attr == null) throw new KeyNotFoundException();
				String key = attr.PropertyName;
				url += $"?query={attr.PropertyName}:{value}";
			}
			else
				throw new KeyNotFoundException();
			var request = BuildRequest(url, nameof(this.GetByQuery), null, null);
			await foreach (var result in ShopifyRestClient.GetAll<CustomerData, CustomersResponse>(request, cancellationToken))
			{
				if (includedMetafields == true && result != null)
				{
					result.Metafields = await GetMetafieldsById(result.Id.ToString(), cancellationToken);
				}
				yield return result;

			}
		}

		public virtual async Task<bool> ActivateAccount(string customerId)
		{
			var request = BuildRequest(GetAccountActivationUrl, nameof(this.ActivateAccount), MakeUrlSegments(customerId), null);
			return await ShopifyRestClient.Post(request);
		}

		public virtual async Task<List<MetafieldData>> GetMetafieldsById(string id, CancellationToken cancellationToken = default)
		{
			var metaFields = new List<MetafieldData>();
			var request = BuildRequest(GetMetafieldsUrl, nameof(GetMetafieldsById), MakeUrlSegments(id), null);
			 await foreach(var data in ShopifyRestClient.GetAll<MetafieldData, MetafieldsResponse>(request, cancellationToken))
				metaFields.Add(data);
			 return metaFields;
		}


		public async Task<List<CustomerAddressData>> GetAddressesById(string id, CancellationToken cancellationToken = default)
		{
			var segments = MakeUrlSegments(id);
			List<CustomerAddressData> customerAddressDatas = new List<CustomerAddressData>();
			var request = BuildRequest(GetAddressesUrl, nameof(GetAddressesById), segments, null);
			await foreach (var customerAddressData in ShopifyRestClient.GetAll<CustomerAddressData, CustomerAddressesResponse>(request, cancellationToken))
				customerAddressDatas.Add(customerAddressData);
			return customerAddressDatas;

		}
	}
}
