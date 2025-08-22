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

using System.Collections.Generic;
using PX.Objects.AP;

namespace PX.Objects.CA
{
	public static class AddendaAliases
	{
		public static readonly Dictionary<string, string> Direct =
			new Dictionary<string, string> {
				{ nameof(APPayment), ACHPlugInBase.Messages.Payment },
				{ nameof(APAdjust), ACHPlugInBase.Messages.Adjustment },
				{ nameof(APInvoice), ACHPlugInBase.Messages.Bill },
				{ nameof(CashAccountPaymentMethodDetail), ACHPlugInBase.Messages.RemittanceSettings },
				{ nameof(VendorPaymentMethodDetail), ACHPlugInBase.Messages.APSettings },
			};

		public static readonly Dictionary<string, string> Reverse =
			new Dictionary<string, string> {
				{ ACHPlugInBase.Messages.Payment, nameof(APPayment) },
				{ ACHPlugInBase.Messages.Adjustment, nameof(APAdjust) },
				{ ACHPlugInBase.Messages.Bill, nameof(APInvoice) },
				{ ACHPlugInBase.Messages.RemittanceSettings, nameof(CashAccountPaymentMethodDetail) },
				{ ACHPlugInBase.Messages.APSettings, nameof(VendorPaymentMethodDetail) },
			};
	}
}
