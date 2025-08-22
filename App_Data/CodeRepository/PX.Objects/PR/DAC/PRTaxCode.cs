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

namespace PX.Objects.PR.Standalone
{
	[PXCacheName("Payroll Tax Code")]
	[Serializable]
	public partial class PRTaxCode : PXBqlTable, IBqlTable
	{
		#region TaxID
		public abstract class taxID : PX.Data.BQL.BqlInt.Field<taxID> { }
		[PXDBIdentity]
		public int? TaxID { get; set; }
		#endregion
		#region CountryID
		public abstract class countryID : PX.Data.BQL.BqlString.Field<countryID> { }
		[PXDBString(2, IsFixed = true)]
		public virtual string CountryID { get; set; }
		#endregion
		#region IsDeleted
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that the tax code was soft deleted.
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		public virtual bool? IsDeleted { get; set; }
		public abstract class isDeleted : PX.Data.BQL.BqlBool.Field<isDeleted> { }
		#endregion
	}
}
