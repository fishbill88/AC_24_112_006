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

using PX.Data.SQLTree;
using PX.Data;
using System.Collections.Generic;
using PX.Objects.AR;
using PX.Data.BQL;

namespace PX.Objects.SO.BQL
{
	public class SuggestRelatedItemsIsTrue<TranType, Released, CustomerID> : IBqlUnary
		where TranType : IBqlOperand
		where Released : IBqlOperand
		where CustomerID : IBqlOperand
	{
		private IBqlCreator formula = new Where2<Where<TranType, Equal<ARDocType.invoice>,
														Or<TranType, Equal<ARDocType.cashSale>>>,
												And<Released, NotEqual<True>,
												And<Use<Selector<CustomerID, Customer.suggestRelatedItems>>.AsBool, Equal<True>>>>();
		public bool AppendExpression(ref SQLExpression exp, PXGraph graph, BqlCommandInfo info, BqlCommand.Selection selection)
			=> formula.AppendExpression(ref exp, graph, info, selection);

		public void Verify(PXCache cache, object item, List<object> pars, ref bool? result, ref object value)
			=> formula.Verify(cache, item, pars, ref result, ref value);
	}
}
