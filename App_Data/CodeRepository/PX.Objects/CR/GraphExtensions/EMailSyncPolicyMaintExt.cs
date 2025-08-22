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

using PX.Data;
using PX.SM;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;

namespace PX.Objects.CR.GraphExtensions
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class EMailSyncPolicyMaintExt : PXGraphExtension<EMailSyncPolicyMaint>
	{
		protected virtual void _(Events.RowSelected<EMailSyncPolicy> e, PXRowSelected baseHandler)
		{
			baseHandler(e.Cache, e.Args);

			if (!(e.Args.Row is EMailSyncPolicy row))
			{
				return;
			}

			EMailSyncAccount syncAccountWithSynchronizedContacts =
				SelectFrom<EMailSyncAccount>
					.InnerJoin<EMailSyncServer>
						.On<EMailSyncAccount.serverID.IsEqual<EMailSyncServer.accountID>>
					.Where<Brackets<EMailSyncAccount.contactsExportDate.IsNotNull
							.Or<EMailSyncAccount.contactsImportDate.IsNotNull>>
						.And<Brackets<EMailSyncAccount.policyName.IsEqual<EMailSyncPolicy.policyName.FromCurrent>
							.Or<EMailSyncServer.defaultPolicyName.IsEqual<EMailSyncPolicy.policyName.FromCurrent>>>>>
					.View.SelectSingleBound(Base, new[] { row });

			PXUIFieldAttribute.SetEnabled<EMailSyncPolicy.contactsFilter>(e.Cache, row, (row.ContactsSync ?? false) && syncAccountWithSynchronizedContacts is null);
		}
	}
}