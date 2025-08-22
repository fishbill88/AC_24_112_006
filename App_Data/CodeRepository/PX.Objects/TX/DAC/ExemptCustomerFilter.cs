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

namespace PX.Objects.TX
{
	/// <summary>
	/// An unbound DAC that is used for the filtering parameters on the Manage Exempt Customers (TX505000) form.
	/// </summary>
	[Serializable]
	[PXHidden]
	public partial class ExemptCustomerFilter : PXBqlTable, IBqlTable
	{
		#region Action
		public abstract class action : PX.Data.BQL.BqlString.Field<action> { }
		/// <summary>
		/// The action that should be performed on customer records.
		/// </summary>
		[PXString()]
		[PXUIField(DisplayName = "Action", Visibility = PXUIVisibility.SelectorVisible)]
		[PXStringList(new string[] { AR.Messages.CreateCustomerInECM, AR.Messages.UpdateCustomerInECM },
			new string[] { AR.Messages.CreateCustomerInECM, AR.Messages.UpdateCustomerInECM })]
		public virtual string Action { get; set; }
		#endregion

		#region CompanyCode
		public abstract class companyCode : PX.Data.BQL.BqlString.Field<companyCode> { }
		/// <summary>
		/// The company codes for which customer records are processed in the exemption certificate management (ECM) system.
		/// </summary>
		[PXString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Company Code")]
		[PXSelector(typeof(Search5<TaxPluginMapping.companyCode,
			InnerJoin<TaxPlugin, On<TaxPluginMapping.taxPluginID, Equal<TaxPlugin.taxPluginID>>,
			InnerJoin<TXSetup, On<TaxPlugin.taxPluginID, Equal<TXSetup.eCMProvider>>>>,
			Aggregate<GroupBy<TaxPluginMapping.companyCode>>>), ValidateValue = false)]
		public virtual string CompanyCode
		{
			get;
			set;
		}
		#endregion
	}
}
