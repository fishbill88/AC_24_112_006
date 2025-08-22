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
using PX.Objects.AR;
using ARRegisterAlias = PX.Objects.AR.Standalone.ARRegisterAlias;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using System;

namespace PX.Objects.SO.DAC.Unbound
{
	/// <exclude/>
	[PXCacheName(Messages.AddInvoiceFilter)]
	public partial class AddInvoiceFilter : PXBqlTable, IBqlTable
	{
		#region ARDocType
		public abstract class aRDocType : PX.Data.BQL.BqlString.Field<aRDocType> { }
		[PXString(3, IsFixed = true)]
		[PXDefault(AR.ARDocType.Invoice, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXStringList(
				new string[] { AR.ARDocType.Invoice, AR.ARDocType.CashSale, AR.ARDocType.DebitMemo, AR.ARDocType.CreditMemo },
				new string[] { AR.Messages.Invoice, AR.Messages.CashSale, AR.Messages.DebitMemo, AR.Messages.CreditMemo })]
		[PXUIField(DisplayName = "AR Doc. Type")]
		public virtual String ARDocType
		{
			get;
			set;
		}
		#endregion
		#region ARRefNbr
		public abstract class aRRefNbr : PX.Data.BQL.BqlString.Field<aRRefNbr> { }
		[PXString(15, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "AR Doc. Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[ARInvoiceType.RefNbr(typeof(Search2<AR.Standalone.ARRegisterAlias.refNbr,
			InnerJoinSingleTable<ARInvoice, On<ARInvoice.docType, Equal<AR.Standalone.ARRegisterAlias.docType>,
				And<ARInvoice.refNbr, Equal<AR.Standalone.ARRegisterAlias.refNbr>>>>,
			Where<AR.Standalone.ARRegisterAlias.docType, Equal<Optional<AddInvoiceFilter.aRDocType>>,
				And<AR.Standalone.ARRegisterAlias.released, Equal<boolTrue>,
				And<AR.Standalone.ARRegisterAlias.origModule, Equal<BatchModule.moduleSO>,
				And<AR.Standalone.ARRegisterAlias.customerID, Equal<Current<SOOrder.customerID>>>>>>,
			OrderBy<Desc<AR.Standalone.ARRegisterAlias.refNbr>>>), Filterable = true)]
		[PXRestrictor(typeof(Where<ARRegisterAlias.canceled, NotEqual<True>>),
			Messages.InvoiceCanceled, typeof(ARRegisterAlias.refNbr))]
		[PXRestrictor(typeof(Where<ARRegisterAlias.isUnderCorrection, NotEqual<True>>),
			Messages.InvoiceUnderCorrection, typeof(ARRegisterAlias.refNbr))]
		[PXFormula(typeof(Default<AddInvoiceFilter.aRDocType>))]
		public virtual String ARRefNbr
		{
			get;
			set;
		}
		#endregion
		#region OrderType
		public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
		[PXString(2, IsFixed = true, InputMask = ">aa")]
		[PXSelector(typeof(Search2<SOOrderType.orderType,
			InnerJoin<SOOrderTypeOperation, On2<SOOrderTypeOperation.FK.OrderType, And<SOOrderTypeOperation.operation, Equal<SOOrderType.defaultOperation>>>>,
			Where<SOOrderType.behavior.IsNotIn<SOBehavior.qT, SOBehavior.bL, SOBehavior.tR>.
				And<SOOrderType.behavior.IsNotEqual<SOBehavior.rM>.Or<SOOrderType.aRDocType.IsNotEqual<ARDocType.noUpdate>>>>>))]
		[PXRestrictor(typeof(Where<SOOrderType.requireAllocation, NotEqual<True>, Or<AllocationAllowed>>), ErrorMessages.ElementDoesntExist, typeof(SOOrderType.orderType))]
		[PXRestrictor(typeof(Where<SOOrderType.active, Equal<True>>), null)]
		[PXUIField(DisplayName = "Order Type", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String OrderType
		{
			get;
			set;
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
		[PXString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Order Nbr.")]
		[SO.RefNbr(typeof(Search<SOOrder.orderNbr,
			Where<SOOrder.orderType, Equal<Optional2<orderType>>,
				And<SOOrder.customerID, Equal<Current<SOOrder.customerID>>>>,
			OrderBy<Desc<SOOrder.orderNbr>>>), Filterable = true)]
		[PXRestrictor(typeof(Where<SOOrder.releasedCntr, Greater<int0>>), null)]
		public virtual String OrderNbr
		{
			get;
			set;
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
		[Inventory]
		public virtual Int32? InventoryID
		{
			get;
			set;
		}
		#endregion
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.BQL.BqlString.Field<lotSerialNbr> { }
		[PXString]
		[PXSelector(typeof(Search<INItemLotSerial.lotSerialNbr, Where<INItemLotSerial.inventoryID, Equal<Optional2<inventoryID>>>>))]
		[PXUIField(DisplayName = "Lot/Serial Nbr.", FieldClass = "LotSerial")]
		public virtual String LotSerialNbr
		{
			get;
			set;
		}
		#endregion
		#region StartDate
		public abstract class startDate : PX.Data.BQL.BqlDateTime.Field<startDate> { }
		[PXDate]
		[PXUIField(DisplayName = "Start Date")]
		public virtual DateTime? StartDate
		{
			get;
			set;
		}
		#endregion
		#region EndDate
		public abstract class endDate : PX.Data.BQL.BqlDateTime.Field<endDate> { }
		[PXDate]
		[PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "End Date")]
		public virtual DateTime? EndDate
		{
			get;
			set;
		}
		#endregion
		#region Expand
		public abstract class expand : PX.Data.BQL.BqlBool.Field<expand> { }
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Show Non-Stock Kits by Components")]
		public virtual Boolean? Expand
		{
			get; set;
		}
		#endregion
	}
}
