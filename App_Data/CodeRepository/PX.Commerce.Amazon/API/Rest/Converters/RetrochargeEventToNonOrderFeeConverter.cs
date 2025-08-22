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
using PX.Objects.FA;
using System;
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest.Converters
{
	public class RetrochargeEventToNonOrderFeeConverter : IConverter<RetrochargeEvent, IEnumerable<NonOrderFee>>
	{
		private readonly IConverter<ChargeComponent, NonOrderFee> chargeComponentConverter;

		public RetrochargeEventToNonOrderFeeConverter(IConverter<ChargeComponent, NonOrderFee> chargeComponentConverter) =>
			this.chargeComponentConverter = chargeComponentConverter;

		public IEnumerable<NonOrderFee> Convert(RetrochargeEvent retrochargeEvent)
		{
			if (retrochargeEvent is null)
				throw new PXArgumentException(nameof(retrochargeEvent), ErrorMessages.ArgumentNullException);

			if (!retrochargeEvent.IsValid)
				yield break;

			foreach (TaxWithheldComponent taxWithheldComponent in retrochargeEvent.RetrochargeTaxWithheldList)
			{
				if (!taxWithheldComponent.IsValid)
					continue;

				foreach (ChargeComponent chargeComponent in taxWithheldComponent.TaxesWithheld)
					yield return this.chargeComponentConverter.Convert(chargeComponent);
			}
		}
	}
}
