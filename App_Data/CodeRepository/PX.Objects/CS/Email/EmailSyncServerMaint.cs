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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.CR;
using PX.SM;

namespace PX.Objects.CS.Email
{
	public class EMailSyncServerMaintExt : PXGraphExtension<EMailSyncServerMaint>
	{
		public PXSelect<EMailSyncAccount, Where<EMailSyncAccount.serverID, Equal<Current<EMailSyncServer.accountID>>>, OrderBy<Desc<EMailSyncAccount.syncAccount>>> SyncAccounts;
		public IEnumerable syncAccounts()
		{
			List<EMailSyncAccount> rows = Base.SyncAccounts.Select().RowCast<EMailSyncAccount>().ToList();

			if (Base.Servers.Current == null || Base.Servers.Current.AccountCD == null) return new EMailSyncAccount[0];

			Boolean isDirty = Base.SyncAccounts.Cache.IsDirty;
			foreach (PXResult<EPEmployee, Contact> employee in PXSelectJoin<EPEmployee,
				InnerJoin<Contact, On<EPEmployee.defContactID, Equal<Contact.contactID>, And<EPEmployee.parentBAccountID, Equal<Contact.bAccountID>>>>,
				Where<Contact.eMail, IsNotNull, And<Contact.userID, IsNotNull>>>.Select(Base))
			{
				EMailSyncAccount row = new EMailSyncAccount();
				row.ServerID = Base.Servers.Current.AccountID;
				row.EmployeeID = ((EPEmployee)employee).BAccountID;
				row.Address = ((Contact)employee).EMail;
				row.EmployeeCD = ((EPEmployee)employee).AcctName;
				row.OwnerID = ((Contact)employee).ContactID;

				if (Base.SyncAccounts.Cache.Locate(row) == null)
				{
					row.IsVitrual = true;
					row.SyncAccount = false;

					row = (EMailSyncAccount)Base.SyncAccounts.Cache.Insert(row);
					if (row == null) continue;

					Base.SyncAccounts.Cache.SetStatus(row, PXEntryStatus.Held);

					rows.Add(row);
				}
			}
			Base.SyncAccounts.Cache.IsDirty = isDirty;

			return rows;
		}
		
		public override void Initialize()
		{
			Base.SyncAccounts.Cache.AllowSelect = true;
        }

		protected virtual void EMailSyncAccount_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			var row = e.Row as EMailSyncAccount;
			if (row == null) return;

			if (row.EmployeeID != null && row.Address == null && row.IsVitrual == false)
			{
				foreach (PXResult<EPEmployee, Contact> result in PXSelectJoin<EPEmployee,
					InnerJoin<Contact,
						On<EPEmployee.defContactID, Equal<Contact.contactID>,
						And<EPEmployee.parentBAccountID, Equal<Contact.bAccountID>>>>,
					Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>.Select(Base, row.EmployeeID))
				{
					row.EmployeeCD = ((EPEmployee) result).AcctName;
					row.OwnerID = ((Contact) result).ContactID;
					row.Address = ((Contact) result).EMail;
				}
			}




			// Inserting a new record when SyncAccount is updated

			if (!(row.SyncAccount ?? false)) return;

			if (row.EmailAccountID != null) return;
			EMailAccount account = null;

			UserPreferences prefs = null;
			if (row.OwnerID != null)
			{
				prefs = Base.UserSettings.Search<UserPreferences.userID>(PXAccess.GetUserID(row.OwnerID));
				if (prefs == null)
				{
					prefs = new UserPreferences();
					// Acuminator disable once PX1048 RowChangesInEventHandlersAllowedForArgsOnly [legacy]
					prefs.UserID = PXAccess.GetUserID(row.OwnerID);
					prefs = Base.UserSettings.Insert(prefs);
				}
			}

            using (new PXReadDeletedScope())
		    {
                account = PXSelectReadonly<EMailAccount, Where<EMailAccount.address, Equal<Required<EMailAccount.address>>,
                                           And<EMailAccount.emailAccountType, Equal<EmailAccountTypesAttribute.exchange>>>>.SelectSingleBound(Base, null, row.Address);
		        if (account == null)
		        {
		            account = new EMailAccount();
		            account.Description = row.EmployeeCD;
		            account.Address = row.Address;
		            account.EmailAccountType = EmailAccountTypesAttribute.Exchange;
		            account.IncomingHostProtocol = IncomingMailProtocolsAttribute._EXCHANGE;
		            account.IncomingProcessing = true;
		            account.ForbidRouting = true;
		            account.CreateActivity = true;
		            account.DefaultOwnerID = row.OwnerID;

		            account = Base.EmailAccounts.Insert(account);
		        }
                else
		        {
		            account.DeletedDatabaseRecord = false;
                    Base.EmailAccounts.Update(account);
                }
		    }
            row.EmailAccountID = account.EmailAccountID;


			if (prefs != null && Base.EmailAccounts.Cache.GetStatus(account) == PXEntryStatus.Inserted)
			{
				prefs.DefaultEMailAccountID = account.EmailAccountID;
				prefs = Base.UserSettings.Update(prefs) as UserPreferences;
			}

		}
	}

	[Serializable]
	public class EMailSyncAccountExt : PXCacheExtension<EMailSyncAccount>
	{
		#region EmployeeID
		public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Employee ID", Visibility = PXUIVisibility.SelectorVisible, IsReadOnly = true)]
		[PXSelector(typeof(EPEmployee.bAccountID), SubstituteKey = typeof(EPEmployee.acctCD), DescriptionField = typeof(EPEmployee.acctName))]
		public virtual Int32? EmployeeID { get; set; }
		#endregion
		#region EmployeeCD
		[PXUIField(DisplayName = "Employee Name", Visibility = PXUIVisibility.SelectorVisible, IsReadOnly = true)]
		[PXDBScalar(typeof(Search<EPEmployee.acctName, Where<EPEmployee.bAccountID, Equal<employeeID>>>))]
		public virtual String EmployeeCD { get; set; }
		#endregion
		#region OwnerID
		[PXUIField(DisplayName = "Owner ID", Visibility = PXUIVisibility.SelectorVisible, IsReadOnly = true)]
		[PXDBScalar(typeof(Search<EPEmployee.defContactID, Where<EPEmployee.bAccountID, Equal<employeeID>>>))]
		public virtual int? OwnerID { get; set; }
		#endregion
	}
}
