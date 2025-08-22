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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.AR
{
	/// <summary>
	/// An unbound DAC that is used to collect certificate exemption reasons.
	/// </summary>
	[PXHidden]
	public partial class CertificateReason : PXBqlTable, IBqlTable
	{
		#region ReasonID
		public abstract class reasonID : PX.Data.BQL.BqlString.Field<reasonID> { }
		/// <summary>
		/// The exemption reason ID.
		/// </summary>
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		public virtual string ReasonID { get; set; }
		#endregion

		#region ReasonName
		public abstract class reasonName : PX.Data.BQL.BqlString.Field<reasonName> { }
		/// <summary>
		/// A description of the exemption reason.
		/// </summary>
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Exemption Reason")]
		public virtual string ReasonName { get; set; }
		#endregion
	}
}
