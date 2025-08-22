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

namespace PX.Objects.CR.Extensions.CRCreateActions
{
	/// <exclude/>
	public abstract class CRCreateBothContactAndAccountAction<TGraph, TMaster, TAccountExt, TContactExt>
		: PXGraphExtension<TGraph>
		where TGraph : PXGraph, new()
		where TAccountExt : CRCreateAccountAction<TGraph, TMaster>
		where TContactExt : CRCreateContactAction<TGraph, TMaster>
		where TMaster : class, IBqlTable, new()
	{
		public TAccountExt AccountExt { get; private set; }
		public TContactExt ContactExt { get; private set; }

		public override void Initialize()
		{
			base.Initialize();

			AccountExt = Base.GetExtension<TAccountExt>()
				?? throw new PXException(Messages.GraphHaveNoExt, typeof(TAccountExt).Name);
			ContactExt = Base.GetExtension<TContactExt>()
				?? throw new PXException(Messages.GraphHaveNoExt, typeof(TContactExt).Name);
		}

		public PXAction<TMaster> CreateBothContactAndAccount;
		[PXUIField(DisplayName = Messages.CreateAccount, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXButton(DisplayOnMainToolbar = false)]
		public virtual IEnumerable createBothContactAndAccount(PXAdapter adapter)
		{
			var existingContact = ContactExt.ExistingContact.SelectSingle();
			var existingAccount = AccountExt.ExistingAccount.SelectSingle();

			if (existingContact?.BAccountID != null && existingAccount == null)
			{
				AccountExt.Documents.Cache.SetValue<Document.bAccountID>(AccountExt.Documents.Current, existingContact.BAccountID);

				Base.Caches<TMaster>().Update(AccountExt.GetMainCurrent());

				PXGraph.ThrowWithoutRollback(new PXSetPropertyException(Messages.AccountAlreadyExists));
			}

			if (AccountExt.AskExtConvert(out bool redirect, ContactExt.PopupValidator))
			{
				var processingGraph = Base.CloneGraphState();

				PXLongOperation.StartOperation(Base, () =>
				{
					var extension = processingGraph.GetProcessingExtension<CRCreateBothContactAndAccountAction<TGraph, TMaster, TAccountExt, TContactExt>>();

					extension.DoConvert(redirect);
				});
			}

			return adapter.Get();
		}

		public virtual void DoConvert(bool redirect)
		{
			ConversionResult<BAccount> result;

			using (var ts = new PXTransactionScope())
			{
				result = AccountExt.Convert(new AccountConversionOptions
				{
					PreserveCachedRecordsFilters =
					{
						ContactExt.PopupValidator
					}
				});

				ContactExt.Convert(new ContactConversionOptions
				{
					GraphWithRelation = result.Graph
				});

				if (AccountExt.Documents.Current.RefContactID == null)
					throw new PXException(Messages.CannotCreateAccount);

				ts.Complete();
			}

			if(redirect)
				AccountExt.Redirect(result);
		}

		public virtual void _(Events.RowSelected<TMaster> e)
		{
			if (AccountExt?.CreateBAccount == null)
				return;

			CreateBothContactAndAccount.SetEnabled(AccountExt.CreateBAccount.GetEnabled());
		}


		public virtual void _(Events.RowSelected<ContactFilter> e)
		{
			if (e.Row == null)
				return;

			var existingAccount = AccountExt.ExistingAccount.SelectSingle();
			var existingContact = ContactExt.ExistingContact.SelectSingle();
			bool eitherBothExistOrNone = !(existingAccount == null ^ existingContact == null);
			PXUIFieldAttribute.SetEnabled<ContactFilter.fullName>(e.Cache, e.Row, eitherBothExistOrNone); //current contact
		}

		public virtual void _(Events.FieldUpdated<AccountsFilter.accountName> e)
		{

			var currentContact = ContactExt.ContactInfo.SelectSingle();
			if (currentContact != null)
				ContactExt.ContactInfo.Cache.SetValueExt<ContactFilter.fullName>(currentContact, e.NewValue);
		
		}
	}
}
