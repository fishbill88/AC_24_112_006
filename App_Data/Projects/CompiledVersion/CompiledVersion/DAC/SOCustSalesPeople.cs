using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.DR;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.SO;
using PX.Objects.SO.DAC.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Objects.GL.ControlAccountModule;
using static PX.Objects.SO.SOBehavior;

namespace CompiledVersion.DAC
{
    [Serializable]
    [PXCacheName("Sales Order Sales People")]
    public class SOCustSalesPeople : PXBqlTable, IBqlTable
    {
        #region Keys
        public class PK : PrimaryKeyOf<SOCustSalesPeople>.By<orderType, orderNbr>
        {
            public static SOCustSalesPeople Find(PXGraph graph, string orderType, string orderNbr, int? customerID, PKFindOptions options = PKFindOptions.None) => 
                FindBy(graph, orderType, orderNbr, options);
        }
        public static class FK
        {
            public class Branch : PX.Objects.GL.Branch.PK.ForeignKeyOf<SOCustSalesPeople>.By<branchID> { }
            public class Order : SOOrder.PK.ForeignKeyOf<SOCustSalesPeople>.By<orderType, orderNbr> { }
            public class OrderType : SOOrderType.PK.ForeignKeyOf<SOCustSalesPeople>.By<orderType> { }
            public class SalesPerson : PX.Objects.AR.SalesPerson.PK.ForeignKeyOf<SOCustSalesPeople>.By<salesPersonID> { }
        }
        #endregion

        #region ID
        [PXDBIdentity(IsKey = true)]
        public virtual int? ID { get; set; }
        public abstract class id : PX.Data.BQL.BqlInt.Field<id> { }
        #endregion

        #region BranchID
        /// <inheritdoc cref="BranchID"/>
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        protected Int32? _BranchID;
        [Branch(typeof(SOOrder.branchID))]
        public virtual Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion

        #region OrderType
        /// <inheritdoc cref="OrderType"/>
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
        protected String _OrderType;
        [PXDBString(2, IsFixed = true)]
        [PXDefault(typeof(SOOrder.orderType))]
        [PXUIField(DisplayName = "Order Type", Visible = false, Enabled = false)]
        [PXSelector(typeof(Search<SOOrderType.orderType>), CacheGlobal = true)]
        public virtual String OrderType
        {
            get
            {
                return this._OrderType;
            }
            set
            {
                this._OrderType = value;
            }
        }
        #endregion
        #region OrderNbr
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        protected String _OrderNbr;
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXDBDefault(typeof(SOOrder.orderNbr), DefaultForUpdate = false)]
        [PXParent(typeof(FK.Order))]
        [PXUIField(DisplayName = "Order Nbr.", Visible = false, Enabled = false)]
        public virtual String OrderNbr
        {
            get
            {
                return this._OrderNbr;
            }
            set
            {
                this._OrderNbr = value;
            }
        }
        #endregion
        #region SalesPersonID
        public abstract class salesPersonID : PX.Data.BQL.BqlInt.Field<salesPersonID> { }
        protected Int32? _SalesPersonID;
        [SalesPerson()]
        public virtual Int32? SalesPersonID
        {
            get
            {
                return this._SalesPersonID;
            }
            set
            {
                this._SalesPersonID = value;
            }
        }
        #endregion


        #region IsDefault
        public abstract class isDefault : PX.Data.BQL.BqlBool.Field<isDefault> { }
        protected Boolean? _IsDefault;
        [PXDBBool()]
        [PXUIField(DisplayName = "Default", Enabled = false)]
        [PXDefault(false)]
        public virtual Boolean? IsDefault
        {
            get
            {
                return this._IsDefault;
            }
            set
            {
                this._IsDefault = value;
            }
        }
        #endregion

        #region CommisionPct
        public abstract class commisionPct : PX.Data.BQL.BqlDecimal.Field<commisionPct> { }
        protected Decimal? _CommisionPct;
        [PXDBDecimal(6)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Commission %")]
        public virtual Decimal? CommisionPct
        {
            get
            {
                return this._CommisionPct;
            }
            set
            {
                this._CommisionPct = value;
            }
        }
        #endregion

        #region Audit Fields
        #region NoteID
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : BqlGuid.Field<noteID> { }
        #endregion

        #region CreatedByID
        /// <summary>
        /// Audit Bql field.
        /// </summary>
        public abstract class createdByID : IBqlField { }
        /// <summary>
        /// Audit Bql property field.
        /// </summary>
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        #endregion

        #region CreatedByScreenID
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        protected String _CreatedByScreenID;
        [PXDBCreatedByScreenID()]
        public virtual String CreatedByScreenID
        {
            get
            {
                return this._CreatedByScreenID;
            }
            set
            {
                this._CreatedByScreenID = value;
            }
        }
        #endregion

        #region CreatedDateTime
        /// <summary>
        /// Audit Bql field.
        /// </summary>
        public abstract class createdDateTime : IBqlField { }
        /// <summary>
        /// Audit Bql property field.
        /// </summary>
        [PXDBCreatedDateTime()]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime, Enabled = false, IsReadOnly = true)]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion

        #region LastModifiedByID
        /// <summary>
        /// Audit Bql field.
        /// </summary>
        public abstract class lastModifiedByID : IBqlField { }
        /// <summary>
        /// Audit Bql property field.
        /// </summary>
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        #endregion

        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        protected String _LastModifiedByScreenID;
        [PXDBLastModifiedByScreenID()]
        public virtual String LastModifiedByScreenID
        {
            get
            {
                return this._LastModifiedByScreenID;
            }
            set
            {
                this._LastModifiedByScreenID = value;
            }
        }
        #endregion

        #region LastModifiedDateTime
        /// <summary>
        /// Audit Bql field.
        /// </summary>
        public abstract class lastModifiedDateTime : IBqlField { }
        /// <summary>
        /// Audit Bql property field.
        /// </summary>
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedDateTime, Enabled = false, IsReadOnly = true)]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion

        #region tstamp
        /// <summary>
        /// Audit Bql field.
        /// </summary>
        public abstract class Tstamp : IBqlField { }
        /// <summary>
        /// Audit Bql property.
        /// </summary>
        [PXDBTimestamp()]
        public virtual Byte[] tstamp { get; set; }
        #endregion
        #endregion
    }
}
