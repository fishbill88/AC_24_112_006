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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.CR;

namespace SP.Objects.CR.GraphExtensions
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class PortalCRDuplicateEntitiesForContactGraphExt : PXGraphExtension<ContactMaint.CRDuplicateEntitiesForContactGraphExt, ContactMaint>
	{
		public virtual void _(Events.RowSelected<Contact> e, PXRowSelected del)
		{
			del?.Invoke(e.Cache, e.Args);

			Base1.CheckForDuplicates.SetVisible(false);
			Base1.DuplicateMerge.SetVisible(false);
			Base1.DuplicateAttach.SetVisible(false);
			Base1.ViewMergingDuplicate.SetVisible(false);
			Base1.ViewMergingDuplicateBAccount.SetVisible(false);
			Base1.ViewLinkingDuplicate.SetVisible(false);
			Base1.ViewLinkingDuplicateBAccount.SetVisible(false);
			Base1.ViewLinkingDuplicateRefContact.SetVisible(false);
			Base1.MarkAsValidated.SetVisible(false);
			Base1.CloseAsDuplicate.SetVisible(false);
		}
	}

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class PortalCRDuplicateEntitiesForBAccountGraphExt : PXGraphExtension<BusinessAccountMaint.CRDuplicateEntitiesForBAccountGraphExt, BusinessAccountMaint>
	{
		public virtual void _(Events.RowSelected<BAccount> e, PXRowSelected del)
		{
			del?.Invoke(e.Cache, e.Args);

			Base1.CheckForDuplicates.SetVisible(false);
			Base1.DuplicateMerge.SetVisible(false);
			Base1.DuplicateAttach.SetVisible(false);
			Base1.ViewMergingDuplicate.SetVisible(false);
			Base1.ViewMergingDuplicateBAccount.SetVisible(false);
			Base1.ViewLinkingDuplicate.SetVisible(false);
			Base1.ViewLinkingDuplicateBAccount.SetVisible(false);
			Base1.ViewLinkingDuplicateRefContact.SetVisible(false);
			Base1.MarkAsValidated.SetVisible(false);
			Base1.CloseAsDuplicate.SetVisible(false);
		}
	}
}
