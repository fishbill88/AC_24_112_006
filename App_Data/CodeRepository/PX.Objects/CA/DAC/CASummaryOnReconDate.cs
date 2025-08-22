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
using PX.Objects.GL;

namespace PX.Objects.CA
{
	[Serializable]
	[PXProjection(typeof(Select5<CARecon,
		InnerJoin<CADailySummary,
			On<CARecon.cashAccountID, Equal<CADailySummary.cashAccountID>,
			And<CARecon.reconDate, GreaterEqual<CADailySummary.tranDate>>>>,
		Aggregate<
			GroupBy<CARecon.cashAccountID,
			GroupBy<CARecon.reconNbr,
			GroupBy<CARecon.reconDate,
			Sum<CADailySummary.amtReleasedClearedDr,
			Sum<CADailySummary.amtReleasedClearedCr,
			Sum<CADailySummary.amtReleasedUnclearedDr,
			Sum<CADailySummary.amtReleasedUnclearedCr>>>>>>>>>))]
	[PXCacheName("Aggregated CA Daily Summary until Reconciliation Date")]
	public partial class CASummaryOnReconDate : PXBqlTable, IBqlTable
	{
		#region CashAccountID
		/// <summary>
		/// The <see cref="CashAccount">cash account</see> under reconciliation.
		/// </summary>
		public abstract class cashAccountID : PX.Data.BQL.BqlInt.Field<cashAccountID> { }

		/// <summary>
		/// The <see cref="CashAccount">cash account</see> under reconciliation.
		/// </summary>
		[CashAccount(IsKey = true, BqlField = typeof(CARecon.cashAccountID))]
		public virtual int? CashAccountID
		{
			get;
			set;
		}
		#endregion
		#region ReconNbr
		/// <summary>
		/// The identification number of the reconciliation statement, 
		/// which the system assigns when the user saves the statement.
		/// </summary>
		public abstract class reconNbr : PX.Data.BQL.BqlString.Field<reconNbr> { }

		/// <summary>
		/// The identification number of the reconciliation statement, 
		/// which the system assigns when the user saves the statement.
		/// </summary>
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(CARecon.reconNbr))]
		[PXSelector(typeof(Search<CARecon.reconNbr, Where<CARecon.cashAccountID, Equal<Optional<CARecon.cashAccountID>>>>))]
		public virtual string ReconNbr
		{
			get;
			set;
		}
		#endregion
		#region ReconDate
		/// <summary>
		/// The date when the reconciliation statement was released and closed. A user can change the date up to the release.
		/// </summary>
		public abstract class reconDate : PX.Data.BQL.BqlDateTime.Field<reconDate> { }

		/// <summary>
		/// The date when the reconciliation statement was released and closed. A user can change the date up to the release.
		/// </summary>
		[PXDBDate(BqlField = typeof(CARecon.reconDate))]
		[PXUIField(DisplayName = "Reconciliation Date")]
		public virtual DateTime? ReconDate
		{
			get;
			set;
		}
		#endregion
		#region AmtReleasedClearedDr
		public abstract class amtReleasedClearedDr : PX.Data.BQL.BqlDecimal.Field<amtReleasedClearedDr> { }

		[PXDBDecimal(2, BqlField = typeof(CADailySummary.amtReleasedClearedDr))]
		public virtual decimal? AmtReleasedClearedDr
		{
			get;
			set;
		}
		#endregion
		#region AmtReleasedClearedCr
		public abstract class amtReleasedClearedCr : PX.Data.BQL.BqlDecimal.Field<amtReleasedClearedCr> { }

		[PXDBDecimal(2, BqlField = typeof(CADailySummary.amtReleasedClearedCr))]
		public virtual decimal? AmtReleasedClearedCr
		{
			get;
			set;
		}
		#endregion
		#region AmtReleasedUnclearedDr
		public abstract class amtReleasedUnclearedDr : PX.Data.BQL.BqlDecimal.Field<amtReleasedUnclearedDr> { }

		[PXDBDecimal(2, BqlField = typeof(CADailySummary.amtReleasedUnclearedDr))]
		public virtual decimal? AmtReleasedUnclearedDr
		{
			get;
			set;
		}
		#endregion
		#region AmtReleasedUnclearedCr
		public abstract class amtReleasedUnclearedCr : PX.Data.BQL.BqlDecimal.Field<amtReleasedUnclearedCr> { }

		[PXDBDecimal(2, BqlField = typeof(CADailySummary.amtReleasedUnclearedCr))]
		public virtual decimal? AmtReleasedUnclearedCr
		{
			get;
			set;
		}
		#endregion
	}
}