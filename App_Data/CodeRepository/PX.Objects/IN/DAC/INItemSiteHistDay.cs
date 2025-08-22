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

namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Data.ReferentialIntegrity.Attributes;

	[Serializable]
	[PXCacheName(Messages.INItemSiteHistDay)]
	public partial class INItemSiteHistDay : PXBqlTable, PX.Data.IBqlTable
    {
		#region Keys
		public class PK : PrimaryKeyOf<INItemSiteHistDay>.By<inventoryID, subItemID, siteID, locationID, sDate>
		{
			public static INItemSiteHistDay Find(PXGraph graph, int? inventoryID, int? subItemID, int? siteID, int? locationID, DateTime? sDate, PKFindOptions options = PKFindOptions.None)
				=> FindBy(graph, inventoryID, subItemID, siteID, locationID, sDate, options);
		}
		public static class FK
		{
			public class InventoryItem : IN.InventoryItem.PK.ForeignKeyOf<INItemSiteHistDay>.By<inventoryID> { }
			public class SubItem : INSubItem.PK.ForeignKeyOf<INItemSiteHistDay>.By<subItemID> { }
			public class Site : INSite.PK.ForeignKeyOf<INItemSiteHistDay>.By<siteID> { }
			public class Location : INLocation.PK.ForeignKeyOf<INItemSiteHistDay>.By<locationID> { }
			public class ItemSiteReplenishment : INItemSiteReplenishment.PK.ForeignKeyOf<INItemSiteHistDay>.By<inventoryID, siteID, subItemID> { }
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID>
        {
        }
        protected Int32? _InventoryID;
        [StockItem(IsKey = true)]
        [PXDefault()]
        public virtual Int32? InventoryID
        {
            get
            {
                return this._InventoryID;
            }
            set
            {
                this._InventoryID = value;
            }
        }
        #endregion
        #region SubItemID
        public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID>
        {
        }
        protected Int32? _SubItemID;
        [SubItem(IsKey = true)]
        [PXDefault()]
        public virtual Int32? SubItemID
        {
            get
            {
                return this._SubItemID;
            }
            set
            {
                this._SubItemID = value;
            }
        }
        #endregion
        #region SiteID
        public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID>
        {
        }
        protected Int32? _SiteID;
        [Site(IsKey = true)]
        [PXDefault()]
        public virtual Int32? SiteID
        {
            get
            {
                return this._SiteID;
            }
            set
            {
                this._SiteID = value;
            }
        }
		#endregion
		#region LocationID
	    public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID>
        {
	    }
	    protected Int32? _LocationID;
	    [IN.Location(typeof(siteID), IsKey = true)]
	    [PXDefault()]
	    public virtual Int32? LocationID
	    {
		    get
		    {
			    return this._LocationID;
		    }
		    set
		    {
			    this._LocationID = value;
		    }
	    }
	    #endregion
		#region SDate
		public abstract class sDate : PX.Data.BQL.BqlDateTime.Field<sDate>
        {
        }
        protected DateTime? _SDate;
        [PXDBDate(IsKey = true)]
        public virtual DateTime? SDate
        {
            get
            {
                return this._SDate;
            }
            set
            {
                this._SDate = value;
            }
        }
	    #endregion
		#region QtyReceived
		public abstract class qtyReceived : PX.Data.BQL.BqlDecimal.Field<qtyReceived>
        {
        }
        protected Decimal? _QtyReceived;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Received")]
        public virtual Decimal? QtyReceived
        {
            get
            {
                return this._QtyReceived;
            }
            set
            {
                this._QtyReceived = value;
            }
        }
        #endregion
        #region QtyIssued
        public abstract class qtyIssued : PX.Data.BQL.BqlDecimal.Field<qtyIssued>
        {
        }
        protected Decimal? _QtyIssued;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Issued")]
        public virtual Decimal? QtyIssued
        {
            get
            {
                return this._QtyIssued;
            }
            set
            {
                this._QtyIssued = value;
            }
        }
        #endregion
        #region QtySales
        public abstract class qtySales : PX.Data.BQL.BqlDecimal.Field<qtySales>
        {
        }
        protected Decimal? _QtySales;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Sales")]
        public virtual Decimal? QtySales
        {
            get
            {
                return this._QtySales;
            }
            set
            {
                this._QtySales = value;
            }
        }
        #endregion
        #region QtyCreditMemos
        public abstract class qtyCreditMemos : PX.Data.BQL.BqlDecimal.Field<qtyCreditMemos>
        {
        }
        protected Decimal? _QtyCreditMemos;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Credit Memos")]
        public virtual Decimal? QtyCreditMemos
        {
            get
            {
                return this._QtyCreditMemos;
            }
            set
            {
                this._QtyCreditMemos = value;
            }
        }
        #endregion
        #region QtyDropShipSales
        public abstract class qtyDropShipSales : PX.Data.BQL.BqlDecimal.Field<qtyDropShipSales>
        {
        }
        protected Decimal? _QtyDropShipSales;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Drop Ship Sales")]
        public virtual Decimal? QtyDropShipSales
        {
            get
            {
                return this._QtyDropShipSales;
            }
            set
            {
                this._QtyDropShipSales = value;
            }
        }
        #endregion
        #region QtyTransferIn
        public abstract class qtyTransferIn : PX.Data.BQL.BqlDecimal.Field<qtyTransferIn>
        {
        }
        protected Decimal? _QtyTransferIn;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Transfer In")]
        public virtual Decimal? QtyTransferIn
        {
            get
            {
                return this._QtyTransferIn;
            }
            set
            {
                this._QtyTransferIn = value;
            }
        }
        #endregion
        #region QtyTransferOut
        public abstract class qtyTransferOut : PX.Data.BQL.BqlDecimal.Field<qtyTransferOut>
        {
        }
        protected Decimal? _QtyTransferOut;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Transfer Out")]
        public virtual Decimal? QtyTransferOut
        {
            get
            {
                return this._QtyTransferOut;
            }
            set
            {
                this._QtyTransferOut = value;
            }
        }
        #endregion
        #region QtyAssemblyIn
        public abstract class qtyAssemblyIn : PX.Data.BQL.BqlDecimal.Field<qtyAssemblyIn>
        {
        }
        protected Decimal? _QtyAssemblyIn;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Assembly In")]
        public virtual Decimal? QtyAssemblyIn
        {
            get
            {
                return this._QtyAssemblyIn;
            }
            set
            {
                this._QtyAssemblyIn = value;
            }
        }
        #endregion
        #region QtyAssemblyOut
        public abstract class qtyAssemblyOut : PX.Data.BQL.BqlDecimal.Field<qtyAssemblyOut>
        {
        }
        protected Decimal? _QtyAssemblyOut;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Assembly Out")]
        public virtual Decimal? QtyAssemblyOut
        {
            get
            {
                return this._QtyAssemblyOut;
            }
            set
            {
                this._QtyAssemblyOut = value;
            }
        }
        #endregion
        #region QtyAdjusted
        public abstract class qtyAdjusted : PX.Data.BQL.BqlDecimal.Field<qtyAdjusted>
        {
        }
        protected Decimal? _QtyAdjusted;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Adjusted")]
        public virtual Decimal? QtyAdjusted
        {
            get
            {
                return this._QtyAdjusted;
            }
            set
            {
                this._QtyAdjusted = value;
            }
        }
		#endregion
		#region QtyDebit
		public abstract class qtyDebit : PX.Data.BQL.BqlDecimal.Field<qtyDebit>
		{
		}
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Debit")]
		public virtual decimal? QtyDebit
		{
			get;
			set;
		}
		#endregion
		#region QtyCredit
		public abstract class qtyCredit : PX.Data.BQL.BqlDecimal.Field<qtyCredit>
		{
		}
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Credit")]
		public virtual decimal? QtyCredit
		{
			get;
			set;
		}
		#endregion

		#region BegQty
		public abstract class begQty : PX.Data.BQL.BqlDecimal.Field<begQty>
        {
		}
		protected Decimal? _BegQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Beginning Qty.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? BegQty
		{
			get
			{
				return this._BegQty;
			}
			set
			{
				this._BegQty = value;
			}
		}
		#endregion
		#region QtyIn
		public abstract class qtyIn : PX.Data.BQL.BqlDecimal.Field<qtyIn>
        {
		}
		protected Decimal? _QtyIn;
		[PXQuantity]
	    [PXFormula(typeof(Add<INItemSiteHistDay.qtyReceived, Add<INItemSiteHistDay.qtyTransferIn, INItemSiteHistDay.qtyAssemblyIn>>))]
		[PXUIField(DisplayName = "Qty. In", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? QtyIn
		{
			get
			{
				return this._QtyIn;
			}
			set
			{
				this._QtyIn = value;
			}
		}
		#endregion
		#region QtyOut
		public abstract class qtyOut : PX.Data.BQL.BqlDecimal.Field<qtyOut>
        {
		}
		protected Decimal? _QtyOut;
		[PXQuantity]
	    [PXFormula(typeof(Add<INItemSiteHistDay.qtyIssued, Add<INItemSiteHistDay.qtySales, Add<INItemSiteHistDay.qtyCreditMemos, Add<INItemSiteHistDay.qtyTransferOut, INItemSiteHistDay.qtyAssemblyOut>>>>))]
		[PXUIField(DisplayName = "Qty. Out", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? QtyOut
		{
			get
			{
				return this._QtyOut;
			}
			set
			{
				this._QtyOut = value;
			}
		}
		#endregion
		#region EndQty
		public abstract class endQty : PX.Data.BQL.BqlDecimal.Field<endQty>
        {
		}
		protected Decimal? _EndQty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Ending Qty.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? EndQty
		{
			get
			{
				return this._EndQty;
			}
			set
			{
				this._EndQty = value;
			}
		}
		#endregion

		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp>
        {
        }
        protected Byte[] _tstamp;
        [PXDBTimestamp(VerifyTimestamp = VerifyTimestampOptions.BothFromGraphAndRecord)]
        public virtual Byte[] tstamp
        {
            get
            {
                return this._tstamp;
            }
            set
            {
                this._tstamp = value;
            }
        }
        #endregion
    }
}
