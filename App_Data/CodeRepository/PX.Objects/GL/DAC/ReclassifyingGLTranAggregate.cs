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
using PX.Objects.CM;

namespace PX.Objects.GL
{
    [PXProjection(typeof(Select4<GLTran,
							Where2<Where<GLTran.tranDate, GreaterEqual<CurrentValue<AccountByPeriodFilter.startDate>>, Or<CurrentValue<AccountByPeriodFilter.startDate>, IsNull>>,
							And2<Where<CurrentValue<AccountByPeriodFilter.endDate>, IsNull, Or<GLTran.tranDate, Less<CurrentValue<AccountByPeriodFilter.endDate>>>>, 
							And<GLTran.isReclassReverse, Equal<True>,
							And<GLTran.posted, Equal<IIf<
								Where<CurrentValue<AccountByPeriodFilter.includeUnposted>, Equal<True>>, GLTran.posted, True>>,
							And<GLTran.released, Equal<IIf<
								Where<CurrentValue<AccountByPeriodFilter.includeUnreleased>, Equal<True>>, GLTran.released, True>>,
							And<GLTran.origModule, IsNotNull,
							And<GLTran.origBatchNbr, IsNotNull,
							And<GLTran.origLineNbr, IsNotNull>>>>>>>>,
							Aggregate<Sum<GLTran.creditAmt,
								Sum<GLTran.debitAmt,
								Sum<GLTran.curyCreditAmt,
								Sum<GLTran.curyDebitAmt,
								GroupBy<GLTran.origModule,
								GroupBy<GLTran.origBatchNbr,
								GroupBy<GLTran.origLineNbr>>>>>>>>>))]
    public class ReclassifyingGLTranAggregate : PXBqlTable, IBqlTable
    {
		#region Module
		public abstract class module : PX.Data.BQL.BqlString.Field<module> { }

		[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(GLTran.origModule))]
		public virtual String Module { get; set; }
		#endregion
		#region BatchNbr
		public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }

		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(GLTran.origBatchNbr))]
		public virtual String BatchNbr { get; set; }
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }

		[PXDBInt(IsKey = true, BqlField = typeof(GLTran.origLineNbr))]
		public virtual Int32? LineNbr { get; set; }
		#endregion
		#region DebitAmt
		public abstract class debitAmt : PX.Data.BQL.BqlDecimal.Field<debitAmt> { }

		[PXDBBaseCury(typeof(GLTranR.ledgerID), BqlField = typeof(GLTran.debitAmt))]
		public Decimal? DebitAmt { get; set; }
		#endregion
		#region CreditAmt
		public abstract class creditAmt : PX.Data.BQL.BqlDecimal.Field<creditAmt> { }

		[PXDBBaseCury(typeof(GLTranR.ledgerID), BqlField = typeof(GLTran.creditAmt))]
		public Decimal? CreditAmt { get; set; }
		#endregion
		#region CuryDebitAmt
		public abstract class curyDebitAmt : PX.Data.BQL.BqlDecimal.Field<curyDebitAmt> { }

		[PXDBCury(typeof(GLTranR.curyID), BqlField = typeof(GLTran.curyDebitAmt))]
		public Decimal? CuryDebitAmt { get; set; }
		#endregion
		#region CuryCreditAmt
		public abstract class curyCreditAmt : PX.Data.BQL.BqlDecimal.Field<curyCreditAmt> { }

		[PXDBCury(typeof(GLTranR.curyID), BqlField = typeof(GLTran.curyCreditAmt))]
		public Decimal? CuryCreditAmt { get; set; }
		#endregion
    }
}
