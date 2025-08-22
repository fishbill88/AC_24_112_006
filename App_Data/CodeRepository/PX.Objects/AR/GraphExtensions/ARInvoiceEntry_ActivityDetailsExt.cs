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
using PX.Data;
using PX.Objects.PM;

namespace PX.Objects.AR
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class ARInvoiceEntry_ActivityDetailsExt : PMActivityDetailsExt<ARInvoiceEntry, ARInvoice, ARInvoice.noteID>
	{
		public override Type GetBAccountIDCommand() => typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<ARInvoice.customerID>>>>);

		public override Type GetEmailMessageTarget() => typeof(Select<ARContact, Where<ARContact.contactID, Equal<Current<ARInvoice.billContactID>>>>);
	}
}
