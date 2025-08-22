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
using PX.Objects.Localizations.CA.MISCT5018Vendor;

namespace PX.Objects.Localizations.CA
{
	/// <exclude/>
	[PXHidden]
	[Serializable]
	[PXCacheName("T5018 Vendor Report Details Filter")]
	public class T5018VendorFilter : T5018EFileRow, IBqlTable
	{
		#region Report Fields

		#region BAccountID
		public abstract class bAccountID : BqlInt.Field<bAccountID> { }
		[PXDBInt(BqlField = typeof(T5018EFileRow.bAccountID))]
		[PXSelector(typeof(SearchFor<T5018EFileRow.bAccountID>.In<
				SelectFrom<T5018EFileRow>.
				AggregateTo<GroupBy<T5018EFileRow.bAccountID>>>),
			SubstituteKey = typeof(T5018EFileRow.vAcctCD))]
		public virtual int? BAccountID
		{
			get;
			set;
		}
		#endregion

		#endregion
	}
}
