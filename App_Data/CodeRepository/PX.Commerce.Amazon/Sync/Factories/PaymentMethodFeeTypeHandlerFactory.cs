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

using PX.Commerce.Amazon.Sync.Helpers;
using PX.Commerce.Amazon.Sync.Interfaces;
using PX.Commerce.Objects;
using PX.Data;

namespace PX.Commerce.Amazon.Sync.Factories
{
	public class PaymentMethodFeeTypeHandlerFactory : IPaymentMethodFeeTypeHandlerFactory
	{
		public IPaymentMethodFeeTypeHandler CreateInstance(int bindingId, PXGraph graph, bool isOrderRelated)
		{
			if (isOrderRelated)
				return new PaymentMethodOrderFeeTypeHandler(bindingId, graph);
			else
				return new PaymentMethodNonOrderFeeTypeHandler(bindingId, graph);
		}
	}
}
