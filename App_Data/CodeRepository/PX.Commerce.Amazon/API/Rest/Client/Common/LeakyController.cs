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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
    public class LeakyController: IDisposable
    {
        private const string HEADER_LIMIT = "x-amzn-RateLimit-Limit";

        private readonly SemaphoreSlim semaphore;

        // API Limit of the given API. All API have their own limits, that is way we need to store them separatly 
        // defaultRate is 0.0167, taken from the SalesOrder API
        private double defaultRate = 0.0167;

        public double Rate
        {
            get { return defaultRate; }
            set { defaultRate = value; }
        }

        public LeakyController()
        {
            this.semaphore = new SemaphoreSlim(1, 1);
        }

        public TimeSpan GetDelayTime(int requestAttemptNumber)
        {
			return TimeSpan.FromSeconds(1 / Rate + requestAttemptNumber);
		}

        public void ReleaseSlot()
        {
            semaphore.Release();
        }

        public async Task GetSlotAsync()
        {
            await semaphore.WaitAsync();
        }

        public void UpdateRate(HttpResponseHeaders headers)
        {
            double newLimitRate = 0;
            if (TryGetRateLimitRate(headers, out newLimitRate))
            {
                Rate = newLimitRate;
            }
        }

        private bool TryGetRateLimitRate(HttpResponseHeaders headers, out double rate)
        {
            var header = headers?.FirstOrDefault(header => string.Equals(header.Key, HEADER_LIMIT, StringComparison.OrdinalIgnoreCase));
            string valueFromHeader = header?.Value?.ToString();
            return double.TryParse(valueFromHeader, out rate);
        }

        void IDisposable.Dispose()
        {
            semaphore.Dispose();
        }
    }
}
