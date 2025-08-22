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
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Objects.PM.ChangeRequest;

namespace PX.Objects.PJ.DrawingLogs.PJ.GraphExtensions
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class ChangeRequestEntry_ActivityDetailsExt : PMActivityDetailsExt<ChangeRequestEntry, PMChangeRequest, PMChangeRequest.noteID>
	{
		public override Type GetBAccountIDCommand() => typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<PMChangeRequest.customerID>>>>);

		public override Type GetEmailMessageTarget() => typeof(Select2<Contact,
			InnerJoin<Customer,
				On<Customer.bAccountID, Equal<Contact.bAccountID>,
				And<Customer.defContactID, Equal<Contact.contactID>>>>,
			Where<
				Customer.bAccountID, Equal<Current<PMChangeOrder.customerID>>>>);
	}
}
