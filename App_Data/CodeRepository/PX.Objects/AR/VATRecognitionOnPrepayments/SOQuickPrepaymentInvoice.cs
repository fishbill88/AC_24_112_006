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
using PX.Objects.CM;
using PX.Objects.GL;
using System;

namespace PX.Objects.AR
{
	/// <summary>
	/// Parameters to create prepayment invoice
	/// </summary>
	[PXVirtual]
	[PXCacheName(SO.Messages.SOQuickPrepaymentInvoice)]
	public class SOQuickPrepaymentInvoice : PXBqlTable, IBqlTable
	{
		#region CuryID
		public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }

		/// <summary>
		/// The code of the <see cref="Currency"/> of the sales order.
		/// </summary>
		/// <value>
		/// Defaults to the <see cref="Company.BaseCuryID">base currency of the company</see>.
		/// Corresponds to the <see cref="CM.Currency.CuryID"/> field.
		/// </value>
		[PXString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXSelector(typeof(Currency.curyID))]
		public virtual string CuryID { get; set; }
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.BQL.BqlLong.Field<curyInfoID> { }

		/// <summary>
		/// The identifier of the <see cref="CurrencyInfo">CurrencyInfo</see> object associated
		/// with the sales order.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="CuryInfoID"/> field.
		/// </value>
		[PXLong()]
		[CurrencyInfo(ModuleCode = BatchModule.AR)]
		public virtual long? CuryInfoID { get; set; }
		#endregion
		#region CuryOrigDocAmt
		public abstract class curyOrigDocAmt : PX.Data.BQL.BqlDecimal.Field<curyOrigDocAmt> { }

		/// <summary>
		/// The unbilled balance of the sales order, which is used as a base for CuryPrepaymentAmt
		/// and PrepaymentPct calculation (in the currency of the document).
		/// </summary>
		[PXCurrency(typeof(curyInfoID), typeof(origDocAmt))]
		[PXUIField(DisplayName = "Payment Amount", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual decimal? CuryOrigDocAmt { get; set; }
		#endregion
		#region OrigDocAmt
		public abstract class origDocAmt : PX.Data.BQL.BqlDecimal.Field<origDocAmt> { }

		/// <summary>
		/// The unbilled balance of the sales order, which is used as a base for PrepaymentAmt
		/// and PrepaymentPct calculation (in the base currency).
		/// </summary>
		[PXBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual decimal? OrigDocAmt { get; set; }
		#endregion

		#region PrepaymentPct
		public abstract class prepaymentPct : PX.Data.BQL.BqlDecimal.Field<prepaymentPct> { }

		/// <summary>
		/// Percent of the Sales Order amount on which a PPI document will be created.
		/// </summary>
		[PXDBDecimal(6, MinValue = 0, MaxValue = 100)]
		[PXDefault(TypeCode.Decimal, "100.00")]
		[PXUIField(DisplayName = "Prepayment Percent", Visibility = PXUIVisibility.Invisible)]
		public virtual decimal? PrepaymentPct { get; set; }
		#endregion
		#region CuryPrepaymentAmt
		public abstract class curyPrepaymentAmt : PX.Data.BQL.BqlDecimal.Field<curyPrepaymentAmt> { }

		/// <summary>
		/// Part of the Sales Order amount on which a PPI document will be created.
		/// Given in the <see cref="curyID">currency of the document</see>.
		/// </summary>
		[PXDBCurrency(typeof(curyInfoID), typeof(prepaymentAmt))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Prepayment Amount", Visibility = PXUIVisibility.Invisible)]
		public decimal? CuryPrepaymentAmt { get; set; }
		#endregion
		#region PrepaymentAmt
		public abstract class prepaymentAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentAmt> { }

		/// <summary>
		/// Part of the sales order amount on which a PPI document will be created.
		/// </summary>
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public decimal? PrepaymentAmt { get; set; }
		#endregion
	}
}
