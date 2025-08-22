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
	public class CustomerAddressRestDataProviderV3Factory : IBCRestDataProviderFactory<IParentRestDataProviderV3<CustomerAddressData, FilterAddresses>>
	{
		public virtual IParentRestDataProviderV3<CustomerAddressData, FilterAddresses> CreateInstance(IBigCommerceRestClient restClient)
		{
			return new CustomerAddressRestDataProviderV3(restClient);
		}
	}

	public class CustomerAddressRestDataProviderV3 : RestDataProviderV3, IParentRestDataProviderV3<CustomerAddressData, FilterAddresses>
	{
		protected override string GetListUrl { get; } = "v3/customers/addresses";

		protected override string GetSingleUrl { get; } = "v3/customers/addresses";

		public CustomerAddressRestDataProviderV3(IBigCommerceRestClient restClient) : base()
		{
			_restClient = restClient;
		}

		public virtual async IAsyncEnumerable<CustomerAddressData> GetAll(IFilter filter = null, CancellationToken cancellationToken = default)
		{
			await foreach (var data in GetAll<CustomerAddressData, CustomerAddressList>(filter, cancellationToken: cancellationToken))
				yield return data;
		}

		public virtual async Task<CustomerAddressData> Create(CustomerAddressData address)
		{
			CustomerAddressList response = await Create<CustomerAddressData, CustomerAddressList>(new CustomerAddressData[] { address }.ToList());
			return response?.Data?.FirstOrDefault();
		}

		public virtual async Task<CustomerAddressData> Update(CustomerAddressData address)
		{
			CustomerAddressList response = await Update<CustomerAddressData, CustomerAddressList>(new CustomerAddressData[] { address }.ToList());
			return response?.Data?.FirstOrDefault();
		}

		public virtual async Task<CustomerAddressData> GetByID(string id) => await GetByID(id, null);

		public virtual async Task<CustomerAddressData> GetByID(string adressId, FilterAddresses filter = null, CancellationToken cancellationToken = default)
		{
			if (filter == null) filter = new FilterAddresses();

			filter.Id = adressId;

			await foreach (var data in base.GetAll<CustomerAddressData, CustomerAddressList>(filter, cancellationToken: cancellationToken))
				return data;
			return null;
		}

		public virtual async Task<CustomerAddressData> Update(CustomerAddressData address, string id)
		{
			return await Update(address);
		}

		public virtual async Task<bool> Delete(string id) => throw new NotImplementedException();

		public virtual async Task<bool> Delete(string id, CustomerAddressData entity) => throw new NotImplementedException();
	}
}
