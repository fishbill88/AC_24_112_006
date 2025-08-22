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
using PX.Objects.AP;
using PX.Objects.GL;
using PX.Objects.IN;

namespace PX.Objects.PO
{
	/// <summary>
	/// This DAC is used for update of Unreceived Qty during PO Receipt Release process
	/// </summary>
	[PXProjection(typeof(Select<APTran>), Persistent = true)]
	[Serializable]
	[PXHidden]
	public partial class APTranReceiptUpdate : PXBqlTable, IBqlTable
	{
		#region TranType
		public abstract class tranType : PX.Data.BQL.BqlString.Field<tranType> { }
		[APDocType.List]
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(APTran.tranType))]
		public virtual string TranType
		{
			get;
			set;
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(APTran.refNbr))]
		public virtual string RefNbr
		{
			get;
			set;
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
		[PXDBInt(IsKey = true, BqlField = typeof(APTran.lineNbr))]
		public virtual int? LineNbr
		{
			get;
			set;
		}
		#endregion

		#region Released
		public abstract class released : PX.Data.BQL.BqlBool.Field<released> { }
		[PXDBBool(BqlField = typeof(APTran.released))]
		public virtual bool? Released
		{
			get;
			set;
		}
		#endregion
		#region POAccrualType
		public abstract class pOAccrualType : PX.Data.BQL.BqlString.Field<pOAccrualType> { }
		[PXDBString(1, IsFixed = true, BqlField = typeof(APTran.pOAccrualType))]
		[POAccrualType.List]
		public virtual string POAccrualType
		{
			get;
			set;
		}
		#endregion
		#region POOrderType
		public abstract class pOOrderType : PX.Data.BQL.BqlString.Field<pOOrderType> { }
		[PXDBString(2, IsFixed = true, BqlField = typeof(APTran.pOOrderType))]
		[POOrderType.List]
		public virtual string POOrderType
		{
			get;
			set;
		}
		#endregion
		#region PONbr
		public abstract class pONbr : PX.Data.BQL.BqlString.Field<pONbr> { }
		[PXDBString(15, IsUnicode = true, BqlField = typeof(APTran.pONbr))]
		public virtual string PONbr
		{
			get;
			set;
		}
		#endregion
		#region POLineNbr
		public abstract class pOLineNbr : PX.Data.BQL.BqlInt.Field<pOLineNbr> { }
		[PXDBInt(BqlField = typeof(APTran.pOLineNbr))]
		public virtual int? POLineNbr
		{
			get;
			set;
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
		[APTranInventoryItem(BqlField = typeof(APTran.inventoryID))]
		public virtual int? InventoryID
		{
			get;
			set;
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
		[INUnit(typeof(inventoryID), BqlField = typeof(APTran.uOM))]
		public virtual string UOM
		{
			get;
			set;
		}
		#endregion
		#region UnreceivedQty
		public abstract class unreceivedQty : PX.Data.BQL.BqlDecimal.Field<unreceivedQty> { }
		[PXDBQuantity(typeof(uOM), typeof(baseUnreceivedQty), HandleEmptyKey = true, BqlField = typeof(APTran.unreceivedQty))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual decimal? UnreceivedQty
		{
			get;
			set;
		}
		#endregion
		#region BaseUnreceivedQty
		public abstract class baseUnreceivedQty : PX.Data.BQL.BqlDecimal.Field<baseUnreceivedQty> { }
		[PXDBQuantity(BqlField = typeof(APTran.baseUnreceivedQty))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual decimal? BaseUnreceivedQty
		{
			get;
			set;
		}
		#endregion
		#region FinPeriodID
		[FinPeriodID(BqlField = typeof(APTran.finPeriodID))]
		public virtual string FinPeriodID { get; set; }
		public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
		#endregion

		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
		[PXDBLastModifiedByID(BqlField = typeof(APTran.lastModifiedByID))]
		public virtual Guid? LastModifiedByID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
		[PXDBLastModifiedByScreenID(BqlField = typeof(APTran.lastModifiedByScreenID))]
		public virtual string LastModifiedByScreenID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		[PXDBLastModifiedDateTime(BqlField = typeof(APTran.lastModifiedDateTime))]
		public virtual DateTime? LastModifiedDateTime
		{
			get;
			set;
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
		[PXDBTimestamp(BqlField = typeof(APTran.Tstamp))]
		public virtual byte[] tstamp
		{
			get;
			set;
		}
		#endregion
	}
}
