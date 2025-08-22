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
using PX.Objects.AR;

namespace PX.Objects.SO.DAC.Projections
{
	[PXHidden]
	[PXProjection(typeof(Select<ARTran>), Persistent = false)]
	public class SOLineInvoiced : PXBqlTable, IBqlTable
	{
		#region TranType
		public abstract class tranType : Data.BQL.BqlString.Field<tranType> { }
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(ARTran.tranType))]
		public virtual String TranType
		{
			get;
			set;
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : Data.BQL.BqlString.Field<refNbr> { }
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(ARTran.refNbr))]
		public virtual String RefNbr
		{
			get;
			set;
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : Data.BQL.BqlInt.Field<lineNbr> { }
		[PXDBInt(IsKey = true, BqlField = typeof(ARTran.lineNbr))]
		public virtual Int32? LineNbr
		{
			get;
			set;
		}
		#endregion
		#region SortOrder
		public abstract class sortOrder : Data.BQL.BqlInt.Field<sortOrder> { }
		[PXDBInt(BqlField = typeof(ARTran.sortOrder))]
		public virtual Int32? SortOrder
		{
			get;
			set;
		}
		#endregion
		#region SOOrderType
		public abstract class sOOrderType : Data.BQL.BqlString.Field<sOOrderType> { }
		[PXDBString(2, IsFixed = true, BqlField = typeof(ARTran.sOOrderType))]
		public virtual String SOOrderType
		{
			get;
			set;
		}
		#endregion
		#region SOOrderNbr
		public abstract class sOOrderNbr : Data.BQL.BqlString.Field<sOOrderNbr> { }
		[PXDBString(15, IsUnicode = true, BqlField = typeof(ARTran.sOOrderNbr))]
		public virtual String SOOrderNbr
		{
			get;
			set;
		}
		#endregion
		#region SOOrderLineNbr
		public abstract class sOOrderLineNbr : Data.BQL.BqlInt.Field<sOOrderLineNbr> { }
		[PXDBInt(BqlField = typeof(ARTran.sOOrderLineNbr))]
		public virtual Int32? SOOrderLineNbr
		{
			get;
			set;
		}
		#endregion
		#region SOOrderLineOperation
		public abstract class sOOrderLineOperation : Data.BQL.BqlString.Field<sOOrderLineOperation>
		{
		}
		[PXDBString(1, IsFixed = true, BqlField = typeof(ARTran.sOOrderLineOperation))]
		public virtual String SOOrderLineOperation
		{
			get;
			set;
		}
		#endregion
		#region SOOrderSortOrder
		public abstract class soOrderSortOrder : Data.BQL.BqlInt.Field<soOrderSortOrder> { }
		[PXDBInt(BqlField = typeof(ARTran.soOrderSortOrder))]
		public virtual Int32? SOOrderSortOrder
		{
			get;
			set;
		}
		#endregion
		#region SOOrderLineSign
		public abstract class sOOrderLineSign : Data.BQL.BqlShort.Field<sOOrderLineSign> { }
		[PXDBShort(BqlField = typeof(ARTran.sOOrderLineSign))]
		public virtual short? SOOrderLineSign
		{
			get;
			set;
		}
		#endregion
		#region SOShipmentType
		public abstract class sOShipmentType : Data.BQL.BqlString.Field<sOShipmentType> { }
		[PXDBString(1, IsFixed = true, BqlField = typeof(ARTran.sOShipmentType))]
		public virtual String SOShipmentType
		{
			get;
			set;
		}
		#endregion
		#region SOShipmentNbr
		public abstract class sOShipmentNbr : Data.BQL.BqlString.Field<sOShipmentNbr> { }
		[PXDBString(15, IsUnicode = true, BqlField = typeof(ARTran.sOShipmentNbr))]
		public virtual String SOShipmentNbr
		{
			get;
			set;
		}
		#endregion
		#region SOShipmentLineNbr
		public abstract class sOShipmentLineNbr : Data.BQL.BqlInt.Field<sOShipmentLineNbr> { }
		[PXDBInt(BqlField = typeof(ARTran.sOShipmentLineNbr))]
		public virtual Int32? SOShipmentLineNbr
		{
			get;
			set;
		}
		#endregion
	}
}
