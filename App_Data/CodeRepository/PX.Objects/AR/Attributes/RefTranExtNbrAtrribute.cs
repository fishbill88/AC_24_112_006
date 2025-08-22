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

using System.Collections;
using PX.Data;

namespace PX.Objects.AR
{
	public class RefTranExtNbrAttribute: PXCustomSelectorAttribute
	{
		public const string PaymentMethod = "PaymentMethodID";
		public const string CustomerIdField = "CustomerID";

		public RefTranExtNbrAttribute() : base(typeof(Search3<ExternalTransaction.tranNumber, 
			InnerJoin<ARPayment, On<ExternalTransaction.docType, Equal<ARPayment.docType>, And<ExternalTransaction.refNbr, Equal<ARPayment.refNbr>>>,
			LeftJoin<CustomerPaymentMethod, On<ExternalTransaction.pMInstanceID,Equal<CustomerPaymentMethod.pMInstanceID>>>>, 
			OrderBy<Desc<ExternalTransaction.tranNumber>>>), typeof(ExternalTransaction.docType),
			typeof(ExternalTransaction.refNbr), typeof(ARPayment.docDate), typeof(ExternalTransaction.amount), typeof(ExternalTransaction.tranNumber), typeof(CustomerPaymentMethod.descr))
		{
			ValidateValue = true;
		}

		public static ExternalTransaction GetStoredTran(string tranNbr, PXGraph graph, PXCache cache)
		{
			if (string.IsNullOrEmpty(tranNbr) || graph == null || cache == null)
			{
				return null;
			}

			int? customerID = cache.GetValue(cache.Current, CustomerIdField) as int?;
			string paymentMethod = cache.GetValue(cache.Current, PaymentMethod) as string;
			var query = new PXSelectJoin<ExternalTransaction,
				InnerJoin<ARPayment, On<ExternalTransaction.docType, Equal<ARPayment.docType>, And<ExternalTransaction.refNbr, Equal<ARPayment.refNbr>>>>,
				Where<ExternalTransaction.procStatus, Equal<ExtTransactionProcStatusCode.captureSuccess>,
					And<ExternalTransaction.tranNumber, Equal<Required<ExternalTransaction.tranNumber>>,
					And<ARPayment.customerID, Equal<Required<ARPayment.customerID>>,
					And<ARPayment.paymentMethodID, Equal<Required<ARPayment.paymentMethodID>>>>>>,
				OrderBy<Desc<ExternalTransaction.transactionID>>>(graph);
			ExternalTransaction extTran = query.SelectSingle(tranNbr, customerID, paymentMethod);
			return extTran;
		}

		protected virtual IEnumerable GetRecords()
		{
			var graph = this._Graph;
			PXCache cache = null;

			if (graph != null)
			{
				cache = _Graph.GetPrimaryCache();
			}

			if (cache == null)
			{
				yield break;
			}

			int? customerID = cache.GetValue(cache.Current, CustomerIdField) as int?;
			string paymentMethod = cache.GetValue(cache.Current, PaymentMethod) as string;
			var result = new PXSelectJoin<ExternalTransaction,
				InnerJoin<ARPayment, On<ExternalTransaction.docType, Equal<ARPayment.docType>, And<ExternalTransaction.refNbr, Equal<ARPayment.refNbr>>>,
				LeftJoin<CustomerPaymentMethod, On<ExternalTransaction.pMInstanceID, Equal<CustomerPaymentMethod.pMInstanceID>>>>,
				Where<ExternalTransaction.procStatus, Equal<ExtTransactionProcStatusCode.captureSuccess>,
					And<ARPayment.customerID, Equal<Required<ARPayment.customerID>>,
					And<ARPayment.paymentMethodID, Equal<Required<ARPayment.paymentMethodID>>>>>,
				OrderBy<Desc<ExternalTransaction.transactionID>>>(graph);

			foreach (PXResult<ExternalTransaction, ARPayment, CustomerPaymentMethod> item in result.SelectWithViewContext(customerID, paymentMethod))
			{
				yield return item;
			}
		}
	}
}
