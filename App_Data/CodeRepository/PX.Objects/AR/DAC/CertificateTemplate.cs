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
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.TX;
using System;

namespace PX.Objects.AR
{
	/// <summary>
	/// An unbound DAC that is used to collect certificate templates.
	/// </summary>
	[PXHidden]
	public partial class CertificateTemplate : PXBqlTable, IBqlTable
	{
		#region TemplateID
		/// <summary>
		/// The certificate template ID.
		/// </summary>
		[PXDBString(15, IsKey = true, IsUnicode = true)]
		public virtual string TemplateID { get; set; }
		public abstract class templateID : PX.Data.BQL.BqlString.Field<templateID> { }
		#endregion

		#region TemplateName
		/// <summary>
		/// A description of the certificate template.
		/// </summary>
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Template Name")]
		public virtual string TemplateName { get; set; }
		public abstract class templateName : PX.Data.BQL.BqlString.Field<templateName> { }
		#endregion
	}
}
