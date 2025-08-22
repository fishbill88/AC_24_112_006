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
using PX.Objects.CA;
using PX.CCProcessingBase.Interfaces.V2;

namespace PX.Objects.AR.CCPaymentProcessing.CardsSynchronization
{
	public class CreditCardReceiverFactory
	{
		CCSynchronizeCards.CreditCardsFilter filter;
		public CreditCardReceiverFactory(CCSynchronizeCards.CreditCardsFilter filter)
		{
			this.filter = filter;
		}

		public CreditCardReceiver GetCreditCardReceiver(ICCProfileProcessor profileProcessor, string customerProfileId)
		{
			CreditCardReceiver receiver = new CreditCardReceiver(profileProcessor, customerProfileId) { AttempsCnt = 3 };

			if (filter.LoadExpiredCards.GetValueOrDefault() != true)
			{
				receiver.ProcessFilter = (CreditCardData item) => {
					bool ret = true;

					if (item.CardExpirationDate.HasValue)
					{
						DateTime dt = item.CardExpirationDate.Value.Date.AddMonths(1);
						ret = DateTime.Now.Date < dt ? true : false;
					}
					return ret;
				};
			}
			return receiver;
		}
	}
}
