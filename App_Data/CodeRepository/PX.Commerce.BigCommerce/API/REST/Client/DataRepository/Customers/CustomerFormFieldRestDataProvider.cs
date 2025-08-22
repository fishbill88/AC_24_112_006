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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce.API.REST
{
	public class CustomerFormFieldRestDataProviderFactory : IBCRestDataProviderFactory<IUpdateAllParentRestDataProvider<CustomerFormFieldData>>
	{
		public virtual IUpdateAllParentRestDataProvider<CustomerFormFieldData> CreateInstance(IBigCommerceRestClient restClient) => new CustomerFormFieldRestDataProvider(restClient);
	}

	public class CustomerFormFieldRestDataProvider : RestDataProviderV3, IUpdateAllParentRestDataProvider<CustomerFormFieldData>
	{
		private const string id_string = "id";

		protected override string GetListUrl { get; } = "v3/customers/form-field-values";

		protected override string GetSingleUrl { get; } = "v3/customers/form-field-values";


		public CustomerFormFieldRestDataProvider(IBigCommerceRestClient restClient) : base()
		{
			_restClient = restClient;
		}

		public virtual async Task<CustomerFormFieldData> Create(CustomerFormFieldData customersCustomFieldData)
		{
			var newData = await Update<CustomerFormFieldData>(customersCustomFieldData, new UrlSegments());
			return newData;
		}

		public virtual async Task<CustomerFormFieldData> Update(CustomerFormFieldData customersCustomFieldData)
		{
			var updateData = await Update<CustomerFormFieldData>(customersCustomFieldData, new UrlSegments());
			return updateData;
		}

		public virtual async Task<List<CustomerFormFieldData>> UpdateAll(List<CustomerFormFieldData> customersCustomFieldDataList)
		{
			CustomerFormFieldList response = await Update<CustomerFormFieldData, CustomerFormFieldList>(customersCustomFieldDataList, new UrlSegments());
			return response?.Data;
		}

		public virtual async IAsyncEnumerable<CustomerFormFieldData> GetAll(IFilter filter = null, CancellationToken cancellationToken = default)
		{
			await foreach (var data in GetAll<CustomerFormFieldData, CustomerFormFieldList>(filter, cancellationToken: cancellationToken))
				yield return data;
		}

		public virtual async Task<CustomerFormFieldData> GetByID(string customerId)
		{
			await using (var enumerator = GetAll<CustomerFormFieldData, CustomerFormFieldList>(new FilterCustomerFormFieldData() { CustomerId = customerId }).GetAsyncEnumerator())
			{
				// If MoveNextAsync() returns false, the enumerator is empty.
				return await enumerator.MoveNextAsync() ? enumerator.Current : default;
			}
		}
	}
}
