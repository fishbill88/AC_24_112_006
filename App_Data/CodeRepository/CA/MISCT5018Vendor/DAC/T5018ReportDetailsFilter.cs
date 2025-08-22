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
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using static PX.Data.PXAccess;

namespace PX.Objects.Localizations.CA
{
	/// <exclude/>
	[PXHidden]
	[Serializable]
	[PXCacheName("T5018 Report Details Filter")]
	public class T5018ReportDetailsFilter : T5018MasterTable, IBqlTable
	{
		#region Report Fields

		#region OrganizationID
		public new abstract class organizationID : BqlInt.Field<organizationID> { }
		[PXDBInt]
		public override int? OrganizationID
		{
			get;
			set;
		}
		#endregion

		#region T5018Year
		public abstract class year : BqlString.Field<year> { }
		[PXDBString]
		[PXSelector(typeof(SearchFor<T5018MasterTable.year>.In<
				SelectFrom<T5018MasterTable>.
				Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>>.
				AggregateTo<GroupBy<T5018MasterTable.year>>>),
			typeof(T5018MasterTable.year))]
		public override string Year
		{
			get;
			set;
		}
		#endregion

		#region ReportRevision
		public abstract class revision : BqlString.Field<revision> { }
		[PXDBString]
		[PXSelector(typeof(SearchFor<T5018MasterTable.revision>.In<
				SelectFrom<T5018MasterTable>.
				Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.
					And<T5018MasterTable.year.IsEqual<year.AsOptional>>>>),
			typeof(T5018MasterTable.revision))]
		public override String Revision
		{
			get;
			set;
		}
		#endregion

		#endregion
	}
}
