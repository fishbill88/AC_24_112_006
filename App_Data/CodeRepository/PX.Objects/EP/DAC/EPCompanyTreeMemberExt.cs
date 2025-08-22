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
using PX.Objects.CR;
using PX.TM;

namespace PX.Objects.EP
{
	public sealed class EPCompanyTreeMemberExt : PXCacheExtension<EPCompanyTreeMember>
	{
		#region ContactID
		public abstract class contactID : PX.Data.BQL.BqlInt.Field<contactID> { }

		[PXDBInt(IsKey = true)]
		[PXSelector(typeof(Search<Contact.contactID, Where<Contact.contactType, Equal<ContactTypesAttribute.employee>>>), DescriptionField = typeof(Contact.displayName), SelectorMode = PXSelectorMode.DisplayModeText)]
		[PXUIField(DisplayName = "Contact", Visibility = PXUIVisibility.SelectorVisible)]
		public int? ContactID { get; set; }
		#endregion
	}
}