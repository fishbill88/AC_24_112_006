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
using PX.Objects.Localizations.CA.Messages;

namespace PX.Objects.Localizations.CA
{
	[PXHidden]
	public class T5018ReportFilter : PXBqlTable, IBqlTable
	{
		#region Report Fields

		#region Transmitter
		public abstract class transmitter : BqlInt.Field<transmitter> { }
		[PXUIField(DisplayName = T5018Messages.Transmitter)]
		[PXSelector(typeof(SearchFor<T5018MasterTable.organizationID>.In<
				SelectFrom<T5018MasterTable>.
				AggregateTo<GroupBy<T5018MasterTable.organizationID>>>),
			typeof(T5018MasterTable.organizationID))]
		[PXDBInt]
		public virtual int? Transmitter
		{
			get;
			set;
		}
		#endregion

		#region T5018Year
		public abstract class t5018Year : BqlString.Field<t5018Year> { }
		[PXSelector(typeof(SearchFor<T5018MasterTable.year>.In<
				SelectFrom<T5018MasterTable>.
				Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>>.
				AggregateTo<GroupBy<T5018MasterTable.year>>>),
			typeof(T5018MasterTable.year))]
		[PXUIField(DisplayName = T5018Messages.T5018Year)]
		[PXDBString]
		public virtual string T5018Year
		{
			get;
			set;
		}
		#endregion

		#region ReportRevision
		public abstract class reportRevision : BqlString.Field<reportRevision> { }
		[PXDBString]
		[PXSelector(typeof(SearchFor<T5018MasterTable.revision>.In<
				SelectFrom<T5018MasterTable>.
				Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.
					And<T5018MasterTable.year.IsEqual<t5018Year.AsOptional>>>>),
			typeof(T5018MasterTable.revision))]
		[PXUIField(DisplayName = T5018Messages.Revision)]
		public virtual String ReportRevision
		{
			get;
			set;
		}
		#endregion

		#endregion
	}
}
