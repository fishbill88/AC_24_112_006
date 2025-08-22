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
using PX.Commerce.Core;
using PX.Data;
using System;
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest.Converters
{
	public class RentalTransactionEventToNonOrderFeeConverter : IConverter<RentalTransactionEvent, IEnumerable<NonOrderFee>>
	{
		private readonly IConverter<FeeComponent, NonOrderFee> feeComponentConverter;

		private readonly IConverter<ChargeComponent, NonOrderFee> chargeComponentConverter;

		public RentalTransactionEventToNonOrderFeeConverter(IConverter<FeeComponent, NonOrderFee> feeComponentConverter, IConverter<ChargeComponent, NonOrderFee> chargeComponentConverter)
		{
			this.feeComponentConverter = feeComponentConverter;
			this.chargeComponentConverter = chargeComponentConverter;
		}

		public IEnumerable<NonOrderFee> Convert(RentalTransactionEvent rentalTransactionEvent)
		{
			if (rentalTransactionEvent is null)
				throw new PXArgumentException(nameof(rentalTransactionEvent), ErrorMessages.ArgumentNullException);

			if (rentalTransactionEvent.RentalFeeList != null)
			{
				foreach (FeeComponent feeComponent in rentalTransactionEvent.RentalFeeList)
					yield return this.feeComponentConverter.Convert(feeComponent);
			}

			if (rentalTransactionEvent.RentalChargeList != null)
			{
				foreach (ChargeComponent chargeComponent in rentalTransactionEvent.RentalChargeList)
					yield return this.chargeComponentConverter.Convert(chargeComponent);
			}
		}
	}
}
