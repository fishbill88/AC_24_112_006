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

namespace PX.Commerce.Amazon.API.Rest.Converters
{
	public class FeeComponentToNonOrderFeeConverter : IConverter<FeeComponent, NonOrderFee>
	{
		public NonOrderFee Convert(FeeComponent feeComponent)
		{
			if (feeComponent is null)
				throw new PXArgumentException(nameof(feeComponent), ErrorMessages.ArgumentNullException);

			if (!feeComponent.IsValid)
				throw new PXArgumentException(nameof(feeComponent), ErrorMessages.FieldIsEmpty, nameof(feeComponent));

			return new NonOrderFee
			{
				Amount = Math.Abs(System.Convert.ToDecimal(feeComponent.FeeAmount.CurrencyAmount)),
				FeeDescription = feeComponent.FeeType
			};
		}
	}
}
