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
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce.API.REST
{
    public class StoreTimeRestDataProvider :  IRestDataReader<StoreTime>
    {
        private readonly IBigCommerceRestClient _restClient;
        
        public StoreTimeRestDataProvider(IBigCommerceRestClient restClient)
        {
            _restClient = restClient;
		}

		public virtual async Task<StoreTime> Get()
        {
            const string resourceUrl = "v2/time";

            var request = _restClient.MakeHttpRequest(resourceUrl);
			StoreTime store = await _restClient.Get<StoreTime>(request);
            return store;
        }       
    }
}
