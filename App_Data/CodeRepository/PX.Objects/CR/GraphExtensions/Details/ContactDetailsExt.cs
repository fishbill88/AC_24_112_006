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
using PX.Objects.CR.Extensions.CRCreateActions;

namespace PX.Objects.CR.Extensions
{
	/// <summary>
	/// Represents the Contacts grid
	/// </summary>
	public abstract class BusinessAccountContactDetailsExt<TGraph, TCreateContactExt, TMaster, FBAccountIDField>
		: ContactDetailsExt<TGraph, TCreateContactExt, TMaster, Contact.bAccountID, FBAccountIDField>
		where TGraph : PXGraph, new()
		where TCreateContactExt : CRCreateContactActionBase<TGraph, TMaster>	//no usages, just need to be deaclared in the graph
		where TMaster : BAccount, IBqlTable, new()
		where FBAccountIDField : class, IBqlField
	{
		#region Events

		[PXOverride]
		public virtual void Persist(Action del)
		{
			using (PXTransactionScope ts = new PXTransactionScope())
			{
				InactivateActiveContacts();

				del();

				ts.Complete();
			}
		}

		#endregion

		#region Methods

		public virtual void InactivateActiveContacts()
		{
			var masterCache = Base.Caches[typeof(TMaster)];
			TMaster acct = masterCache.Current as TMaster;

			if (acct != null
				&& acct.Status == CustomerStatus.Inactive
				&& !CustomerStatus.Inactive.Equals(masterCache.GetValueOriginal<BAccount.status>(acct)))
			{
				ContactMaint graph = PXGraph.CreateInstance<ContactMaint>();

				foreach (Contact contact in this.Contacts.Select())
				{
					if (contact.Status == ContactStatus.Inactive)
						continue;

					contact.Status = ContactStatus.Inactive;
					graph.ContactCurrent.Cache.Update(contact); 
				}

				if (graph.IsDirty)
				{
					graph.Save.Press();
					Base.SelectTimeStamp();
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// Represents the Contacts grid
	/// </summary>
	public abstract class ContactDetailsExt<TGraph, TCreateContactExt, TMaster, FContactField, FMasterField> : PXGraphExtension<TGraph>
		where TGraph : PXGraph, new()
		where TCreateContactExt : CRCreateContactActionBase<TGraph, TMaster>	//no usages, just need to be deaclared in the graph
		where TMaster : class, IBqlTable, new()
		where FContactField : class, IBqlField
		where FMasterField : class, IBqlField
	{
		#region Views

		[PXViewName(Messages.Contacts)]
		[PXFilterable]
		public PXSelectJoin<
				Contact,
			LeftJoin<Address,
				On<Address.addressID, Equal<Contact.defAddressID>>>,
			Where<
				FContactField, Equal<Current<FMasterField>>,
				And<Contact.contactType, Equal<ContactTypesAttribute.person>>>>
			Contacts;

		#endregion

		#region Actions
		public PXAction<TMaster> RefreshContact;
		[PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual void refreshContact()
		{
			Base.SelectTimeStamp();
			Base.Caches<Contact>().ClearQueryCache();
		}

		public PXAction<TMaster> ViewContact;
		[PXUIField(DisplayName = Messages.ViewContact, Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual void viewContact()
		{
			if (this.Contacts.Current == null)
				return;

			if (Base.Caches[typeof(TMaster)].GetStatus(Base.Caches[typeof(TMaster)].Current) == PXEntryStatus.Inserted)
				return;

			Contact current = this.Contacts.Current;

			ContactMaint graph = PXGraph.CreateInstance<ContactMaint>();

			graph.Contact.Current = current;

			PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);
		}

		#endregion
	}
}
