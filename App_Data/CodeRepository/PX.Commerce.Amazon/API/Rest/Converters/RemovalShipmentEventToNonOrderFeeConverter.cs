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

using PX.Commerce.Amazon.API.Rest.Interfaces;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Commerce.Amazon.API.Rest.Converters
{
	public class RemovalShipmentEventToNonOrderFeeConverter : IConverter<RemovalShipmentEvent, IEnumerable<NonOrderFee>>
	{
		public IEnumerable<NonOrderFee> Convert(RemovalShipmentEvent removalShipmentEvent)
		{
			if (removalShipmentEvent is null)
				throw new PXArgumentException(nameof(removalShipmentEvent), ErrorMessages.ArgumentNullException);

			if (!removalShipmentEvent.IsValid)
				yield break;

			yield return new NonOrderFee
			{
				Amount = Math.Abs(removalShipmentEvent.RemovalShipmentItemList.Select(shipmentItem => System.Convert.ToDecimal(shipmentItem.Revenue.CurrencyAmount)).Sum()),
				FeeDescription = removalShipmentEvent.TransactionType.ToString(),
				PostedDate = removalShipmentEvent.PostedDate
			};
		}
	}
}
