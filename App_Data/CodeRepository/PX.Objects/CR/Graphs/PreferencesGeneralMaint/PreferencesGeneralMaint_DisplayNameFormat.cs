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

namespace PX.Objects.CR
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class PreferencesGeneralMaint_DisplayNameFormat : PXGraphExtension<PX.SM.PreferencesGeneralMaint>
	{
		[PXOverride]
		public virtual void UpdatePersonDisplayNames(string PersonDisplayNameFormat)
		{
			using (var tran = new PXTransactionScope())
			{
				switch (PersonDisplayNameFormat)
				{
					case PersonNameFormatsAttribute.WESTERN:
						SetContactsWesternOrder();
						break;

					case PersonNameFormatsAttribute.EASTERN:
						SetContactsEasternOrder();
						break;

					case PersonNameFormatsAttribute.LEGACY:
						SetContactsLegacyOrder();
						break;

					case PersonNameFormatsAttribute.EASTERN_WITH_TITLE:
						SetContactsEasternWithTitleOrder();
						break;
				}

				UpdateBAccounts();

				tran.Complete();
			}

			PXDatabase.ClearCompanyCache();
		}

		protected virtual void SetContactsWesternOrder()
		{
			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName, IsNull<Contact.firstName.Concat<Space>.Concat<Contact.lastName>, Empty.Concat<Contact.lastName>>>,
				Select<Contact, Where<Contact.lastName, IsNotNull>>>());
		}
		protected virtual void SetContactsEasternOrder()
		{
			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName, IIf<Contact.firstName.IsNull, Contact.lastName, Contact.lastName.Concat<CommaSpace>.Concat<Contact.firstName>>>,
				Select<Contact, Where<Contact.lastName, IsNotNull>>>());
		}
		protected virtual void SetContactsLegacyOrder()
		{
			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.lastName>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
					And<Contact.title, IsNull,
					And<Contact.firstName, IsNull,
					And<Contact.midName, IsNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.lastName.Concat<CommaSpace>.Concat<Contact.midName>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
					And<Contact.title, IsNull,
					And<Contact.firstName, IsNull,
					And<Contact.midName, IsNotNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.lastName.Concat<CommaSpace>.Concat<Contact.firstName>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
					And<Contact.title, IsNull,
					And<Contact.firstName, IsNotNull,
					And<Contact.midName, IsNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.lastName.Concat<CommaSpace>.Concat<Contact.firstName>.Concat<Space>.Concat<Contact.midName>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
					And<Contact.title, IsNull,
					And<Contact.firstName, IsNotNull,
					And<Contact.midName, IsNotNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.lastName.Concat<CommaSpace>.Concat<Contact.title>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
					And<Contact.title, IsNotNull,
					And<Contact.firstName, IsNull,
					And<Contact.midName, IsNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.lastName.Concat<Space>.Concat<Contact.midName>.Concat<CommaSpace>.Concat<Contact.title>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
					And<Contact.title, IsNotNull,
					And<Contact.firstName, IsNull,
					And<Contact.midName, IsNotNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.lastName.Concat<Space>.Concat<Contact.firstName>.Concat<CommaSpace>.Concat<Contact.title>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
					And<Contact.title, IsNotNull,
					And<Contact.firstName, IsNotNull,
					And<Contact.midName, IsNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.lastName.Concat<Space>.Concat<Contact.firstName>.Concat<Space>.Concat<Contact.midName>.Concat<CommaSpace>.Concat<Contact.title>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
					And<Contact.title, IsNotNull,
					And<Contact.firstName, IsNotNull,
					And<Contact.midName, IsNotNull>>>>>>());
		}
		protected virtual void SetContactsEasternWithTitleOrder()
		{
			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
				Contact.lastName>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
				And<Contact.title, IsNull,
				And<Contact.firstName, IsNull,
				And<Contact.midName, IsNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.lastName.Concat<CommaSpace>.Concat<Contact.midName>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
				And<Contact.title, IsNull,
				And<Contact.firstName, IsNull,
				And<Contact.midName, IsNotNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.lastName.Concat<CommaSpace>.Concat<Contact.firstName>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
				And<Contact.title, IsNull,
				And<Contact.firstName, IsNotNull,
				And<Contact.midName, IsNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.lastName.Concat<CommaSpace>.Concat<Contact.firstName>.Concat<Space>.Concat<Contact.midName>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
				And<Contact.title, IsNull,
				And<Contact.firstName, IsNotNull,
				And<Contact.midName, IsNotNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.title.Concat<Space>.Concat<Contact.lastName>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
				And<Contact.title, IsNotNull,
				And<Contact.firstName, IsNull,
				And<Contact.midName, IsNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.title.Concat<Space>.Concat<Contact.lastName>.Concat<CommaSpace>.Concat<Contact.midName>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
				And<Contact.title, IsNotNull,
				And<Contact.firstName, IsNull,
				And<Contact.midName, IsNotNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.title.Concat<Space>.Concat<Contact.lastName>.Concat<CommaSpace>.Concat<Contact.firstName>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
				And<Contact.title, IsNotNull,
				And<Contact.firstName, IsNotNull,
				And<Contact.midName, IsNull>>>>>>());

			PXDatabase.Update(Base,
				new Update<Set<Contact.displayName,
					Contact.title.Concat<Space>.Concat<Contact.lastName>.Concat<CommaSpace>.Concat<Contact.firstName>.Concat<Space>.Concat<Contact.midName>>,
				Select<Contact, Where<Contact.lastName, IsNotNull,
				And<Contact.title, IsNotNull,
				And<Contact.firstName, IsNotNull,
				And<Contact.midName, IsNotNull>>>>>>());
		}
		protected virtual void UpdateBAccounts()
		{
			PXDatabase.Update(Base,
				new Update<Set<BAccount.acctName, Contact.displayName>,
				Select2<BAccount,
				InnerJoin<Contact, On<Contact.contactID, Equal<BAccount.defContactID>>>,
				Where<BAccount.type, Equal<BAccountType.employeeType>>>>());
		}
	}
}
