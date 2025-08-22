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
using PX.Objects.IN;

namespace PX.Objects.PO
{
	[Serializable]
	[PXCacheName(Messages.POReceiptPOReturn)]
	public partial class POReceiptPOOriginal : PXBqlTable, IBqlTable
	{
		#region ReceiptType
		public abstract class receiptType : PX.Data.BQL.BqlString.Field<receiptType> { }
		[PXDBString(2, IsFixed = true, IsKey = true, InputMask = "", BqlField = typeof(POReceipt.receiptType))]
		[POReceiptType.List()]
		[PXUIField(DisplayName = "PR Type")]
		public virtual String ReceiptType
		{
			get;
			set;
		}
		#endregion

		#region ReceiptNbr
		public abstract class receiptNbr : PX.Data.BQL.BqlString.Field<receiptNbr>
		{
			public const string DisplayName = "PR Nbr.";
		}
		protected String _ReceiptNbr;
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(POReceipt.receiptNbr))]
		[PXUIField(DisplayName = receiptNbr.DisplayName, Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<POReceipt.receiptNbr, Where<POReceipt.receiptType, Equal<Optional<receiptType>>>>), Filterable = true)]
		public virtual String ReceiptNbr
		{
			get;
			set;
		}
		#endregion

		#region DocDate
		public abstract class docDate : PX.Data.BQL.BqlDateTime.Field<docDate> { }

		/// <summary>
		/// Date of the document.
		/// </summary>
		[PXDBDate(BqlField = typeof(POReceipt.receiptDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? DocDate
		{
			get;
			set;
		}
		#endregion

		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }

		[FinPeriodID]
		[PXUIField(DisplayName = "Post Period", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String FinPeriodID
		{
			get;
			set;
		}
		#endregion

		#region Status
		public abstract class status : PX.Data.BQL.BqlString.Field<status> { }

		[PXDBString(1, IsFixed = true, BqlField = typeof(POReceipt.status))]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[POReceiptStatus.List]
		public virtual string Status
		{
			get;
			set;
		}
		#endregion

		#region TotalQty
		public abstract class totalQty : PX.Data.BQL.BqlDecimal.Field<totalQty> { }
		[PXDBQuantity(BqlField = typeof(POReceiptLine.baseReceiptQty))]
		[PXUIField(DisplayName = "Total Qty.", Enabled = false)]
		public virtual Decimal? TotalQty
		{
			get;
			set;
		}
		#endregion

		#region InvtDocType
		public abstract class invtDocType : PX.Data.BQL.BqlString.Field<invtDocType> { }

		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "IN Doc. Type", Enabled = false)]
		[INDocType.List()]
		public virtual String InvtDocType
		{
			get;
			set;
		}
		#endregion

		#region InvtRefNbr
		public abstract class invtRefNbr : PX.Data.BQL.BqlString.Field<invtRefNbr> { }

		[PXDBString(15, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "IN Ref. Nbr.", Enabled = false)]
		[PXSelector(typeof(Search<INRegister.refNbr, Where<INRegister.docType, Equal<Current<invtDocType>>>>))]
		public virtual String InvtRefNbr
		{
			get;
			set;
		}
		#endregion

		#region StatusText
		public abstract class statusText : PX.Data.BQL.BqlString.Field<statusText> { }
		[PXString]
		public virtual String StatusText
		{
			get;
			set;
		}
		#endregion
	}
}
