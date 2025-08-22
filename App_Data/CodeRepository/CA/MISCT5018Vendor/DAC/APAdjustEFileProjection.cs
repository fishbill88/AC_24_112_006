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
using PX.Common.Serialization;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.Localizations.CA.AP;
using PX.Objects.Localizations.CA.AP.DAC;
using PX.Objects.Localizations.CA.MISCT5018Vendor;

namespace PX.Objects.Localizations.CA
{
	[PXHidden]
	[Serializable]
	[PXProjection(typeof(SelectFrom<APAdjustEFileRevision>.
		InnerJoin<APPayment>.On<APPayment.docType.IsEqual<APAdjustEFileRevision.adjgDocType>.
			And<APPayment.refNbr.IsEqual<APAdjustEFileRevision.adjgRefNbr>>>.
		LeftJoin<APAdjustEFileCalculation>.
			On<APAdjustEFileRevision.adjdDocType.IsEqual<APAdjustEFileCalculation.adjdDocType>.
			And<APAdjustEFileRevision.adjdRefNbr.IsEqual<APAdjustEFileCalculation.adjdRefNbr>.
			And<APAdjustEFileRevision.adjdLineNbr.IsEqual<APAdjustEFileCalculation.adjdLineNbr>>.
			And<APAdjustEFileRevision.adjgDocType.IsEqual<APAdjustEFileCalculation.adjgDocType>.
			And<APAdjustEFileRevision.adjgRefNbr.IsEqual<APAdjustEFileCalculation.adjgRefNbr>.
			And<APAdjustEFileRevision.adjNbr.IsEqual<APAdjustEFileCalculation.adjNbr>>>>>>.
		AggregateTo<
			GroupBy<APAdjustEFileRevision.adjgDocType>,
			GroupBy<APAdjustEFileRevision.adjgRefNbr>,
			GroupBy<APAdjustEFileRevision.organizationID>,
			GroupBy<APAdjustEFileRevision.year>,
			GroupBy<APAdjustEFileRevision.revision>,
			Max<APPayment.curyOrigDocAmt>,
			Sum<APAdjustEFileCalculation.amtAdjustment>>
		))]
	/// <exclude/>
	public class APAdjustEFileProjection : APAdjustEFileRevision
	{
		#region AdjgDocType
		public abstract class adjgDocType : BqlString.Field<adjgDocType> { }

		[PXDBString(IsKey = true, BqlField = typeof(APAdjustEFileRevision.adjgDocType))]
		public virtual string AdjgDocType { get; set; }
		#endregion

		#region AdjgRefNbr
		public abstract class adjgRefNbr : PX.Data.BQL.BqlString.Field<adjgRefNbr> { }

		[PXDBString(IsKey = true, BqlField = typeof(APAdjustEFileRevision.adjgRefNbr))]
		public virtual String AdjgRefNbr { get; set; }
		#endregion

		#region AdjNbr
		public abstract class adjNbr : PX.Data.BQL.BqlInt.Field<adjNbr> { }

		[PXDBInt(IsKey = true, BqlField = typeof(APAdjustEFileRevision.adjNbr))]
		public virtual Int32? AdjNbr { get; set; }
		#endregion

		#region AdjdDocType
		public abstract class adjdDocType : PX.Data.BQL.BqlString.Field<adjdDocType> { }

		[PXDBString(IsKey = true, BqlField = typeof(APAdjustEFileRevision.adjdDocType))]
		public virtual String AdjdDocType { get; set; }
		#endregion

		#region AdjdRefNbr
		public abstract class adjdRefNbr : PX.Data.BQL.BqlString.Field<adjdRefNbr> { }

		[PXDBString(IsKey = true, BqlField = typeof(APAdjustEFileRevision.adjdRefNbr))]
		public virtual string AdjdRefNbr { get; set; }
		#endregion

		#region AdjdLineNbr
		public abstract class adjdLineNbr : PX.Data.BQL.BqlInt.Field<adjdLineNbr> { }

		[PXDBInt(IsKey = true, BqlField = typeof(APAdjustEFileRevision.adjdLineNbr))]
		public virtual int? AdjdLineNbr { get; set; }
		#endregion

		#region Revision
		public abstract class revision : BqlString.Field<revision> { }

		[PXDBString(IsKey = true, BqlField = typeof(APAdjustEFileRevision.revision))]
		public virtual string Revision { get; set; }
		#endregion

		#region CuryOrigDocAmt
		public abstract class curyOrigDocAmt : PX.Data.BQL.BqlDecimal.Field<curyOrigDocAmt> { }

		[PXDBDecimal(BqlField = typeof(APPayment.curyOrigDocAmt))]
		public virtual Decimal? CuryOrigDocAmt { get; set; }
		#endregion

		#region AmtAdjustment
		public abstract class amtAdjustment : PX.Data.BQL.BqlDecimal.Field<amtAdjustment> { }

		[PXDBDecimal(BqlField = typeof(APAdjustEFileCalculation.amtAdjustment))]
		public virtual Decimal? AmtAdjustment { get; set; }
		#endregion

	}

	[PXHidden]
	[PXSerializable]
	[PXProjection(typeof(SelectFrom<APAdjust>))]
	public class APAdjustEFileCalculation : PXBqlTable, IBqlTable
	{
		#region AdjdDocType
		public abstract class adjdDocType : PX.Data.BQL.BqlString.Field<adjdDocType> { }

		[PXDBString(IsKey = true, BqlField = typeof(APAdjust.adjdDocType))]
		public virtual String AdjdDocType { get; set; }
		#endregion

		#region AdjdRefNbr
		public abstract class adjdRefNbr : PX.Data.BQL.BqlString.Field<adjdRefNbr> { }

		[PXDBString(IsKey = true, BqlField = typeof(APAdjust.adjdRefNbr))]
		public virtual string AdjdRefNbr { get; set; }
		#endregion

		#region AdjgDocType
		public abstract class adjgDocType : BqlString.Field<adjgDocType> { }

		[PXDBString(IsKey = true, BqlField = typeof(APAdjust.adjgDocType))]
		public virtual string AdjgDocType { get; set; }
		#endregion

		#region AdjgRefNbr
		public abstract class adjgRefNbr : PX.Data.BQL.BqlString.Field<adjgRefNbr> { }

		[PXDBString(IsKey = true, BqlField = typeof(APAdjust.adjgRefNbr))]
		public virtual String AdjgRefNbr { get; set; }
		#endregion

		#region AdjdLineNbr
		public abstract class adjdLineNbr : PX.Data.BQL.BqlInt.Field<adjdLineNbr> { }
		[PXDBInt(IsKey = true, BqlField = typeof(APAdjust.adjdLineNbr))]
		public virtual int? AdjdLineNbr
		{
			get;
			set;
		}
		#endregion

		#region AdjNbr
		public abstract class adjNbr : PX.Data.BQL.BqlInt.Field<adjNbr> { }
		[PXDBInt(IsKey = true, BqlField = typeof(APAdjust.adjNbr))]
		public virtual Int32? AdjNbr { get; set; }
		#endregion

		#region AmtAdjustment		

		public abstract class amtAdjustment : BqlDecimal.Field<amtAdjustment> { }
		[PXDecimal]
		[PXDBCalced(typeof(
			Switch<
			Case<Where<APAdjust.adjdDocType, Equal<APDocType.prepayment>>, APAdjust.curyAdjgAmt>,
			decimal0>), typeof(decimal) )]
		public virtual decimal? AmtAdjustment { get; set; }

		#endregion
	}
}
