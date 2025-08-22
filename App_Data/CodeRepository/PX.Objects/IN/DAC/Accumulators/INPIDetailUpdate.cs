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

namespace PX.Objects.IN
{
	[PXHidden]
	[Serializable]
	[INPIDetailUpdate.Accumulator(BqlTable = typeof(INPIDetail))]
	public partial class INPIDetailUpdate : PXBqlTable, IBqlTable
	{
		#region PIID
		public abstract class pIID : Data.BQL.BqlString.Field<pIID>
		{
		}
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDefault]
		public virtual string PIID
		{
			get;
			set;
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : Data.BQL.BqlInt.Field<lineNbr> { }
		[PXDBInt(IsKey = true)]
		[PXDefault]
		public virtual int? LineNbr
		{
			get;
			set;
		}
		#endregion

		#region FinalExtVarCost
		public abstract class finalExtVarCost : Data.BQL.BqlDecimal.Field<finalExtVarCost>
		{
		}
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? FinalExtVarCost
		{
			get;
			set;
		}
		#endregion

		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : Data.BQL.BqlDateTime.Field<lastModifiedDateTime>
		{
		}
		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : Data.BQL.BqlGuid.Field<lastModifiedByID>
		{
		}
		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : Data.BQL.BqlString.Field<lastModifiedByScreenID>
		{
		}
		[PXDBLastModifiedByScreenID]
		public virtual string LastModifiedByScreenID
		{
			get;
			set;
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : Data.BQL.BqlByteArray.Field<Tstamp>
		{
		}
		[PXDBTimestamp]
		public virtual byte[] tstamp
		{
			get;
			set;
		}
		#endregion

		public class AccumulatorAttribute : PXAccumulatorAttribute
		{
			public AccumulatorAttribute()
			{
				this.SingleRecord = true;
			}

			protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
			{
				if (!base.PrepareInsert(sender, row, columns))
				{
					return false;
				}

				var detailRow = (INPIDetailUpdate)row;

				columns.UpdateOnly = true;
				columns.Update<INPIDetailUpdate.finalExtVarCost>(detailRow.FinalExtVarCost, PXDataFieldAssign.AssignBehavior.Replace);
				columns.Update<INPIDetailUpdate.lastModifiedByID>(detailRow.LastModifiedByID, PXDataFieldAssign.AssignBehavior.Replace);
				columns.Update<INPIDetailUpdate.lastModifiedDateTime>(detailRow.LastModifiedDateTime, PXDataFieldAssign.AssignBehavior.Replace);
				columns.Update<INPIDetailUpdate.lastModifiedByScreenID>(detailRow.LastModifiedByScreenID, PXDataFieldAssign.AssignBehavior.Replace);

				return true;
			}
		}
	}
}
