using PX.Data;
using PX.Objects.CM;
using PX.Objects.SO;
using System;

namespace POInventoryCustomization
{
    public class SOOrderExt : PXCacheExtension<PX.Objects.SO.SOOrder>
    {
        #region UsrRTHOrderQty
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "RTH Ordered Qty.", Enabled = false)]
        public decimal? UsrRTHOrderQty { get; set; }
        public abstract class usrRTHOrderQty : PX.Data.BQL.BqlDecimal.Field<usrRTHOrderQty> { }
        #endregion

        #region UsrRTHCuryDetaiExtPricelTotal
        public abstract class usrRTHCuryDetailExtPriceTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHCuryDetailExtPriceTotal> { }
        [PXCurrency(typeof(SOOrder.curyInfoID), typeof(SOOrderExt.usrRTHDetailExtPriceTotal))]
        [PXDefault(TypeCode.Decimal, "0.0",PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBCalced(typeof(Add<SOOrder.curyGoodsExtPriceTotal, SOOrder.curyMiscExtPriceTotal>), typeof(Decimal))]
        [PXFormula(typeof(Add<SOOrder.curyGoodsExtPriceTotal, SOOrder.curyMiscExtPriceTotal>))]
        [PXUIField(DisplayName = "RTH Detail Total", Enabled = false)]
        public virtual Decimal? UsrRTHCuryDetailExtPriceTotal
        {
            get;
            set;
        }
        #endregion
        #region UsrRTHDetailExtPriceTotal

        public abstract class usrRTHDetailExtPriceTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHDetailExtPriceTotal> { }
        [PXDecimal(4)]
        [PXDBCalced(typeof(Add<SOOrder.goodsExtPriceTotal, SOOrder.miscExtPriceTotal>), typeof(Decimal))]
        public virtual Decimal? UsrRTHDetailExtPriceTotal
        {
            get;
            set;
        }
        #endregion

        #region UsrRTHCuryLineDiscTotal
        public abstract class usrRTHcuryLineDiscTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHcuryLineDiscTotal> { }
        [PXDBCurrency(typeof(SOOrder.curyInfoID), typeof(SOOrderExt.usrRTHLineDiscTotal))]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Line Discounts", Enabled = false)]
        public virtual Decimal? UsrRTHCuryLineDiscTotal
        {
            get;
            set;
        }
        #endregion
        #region UsrRTHLineDiscTotal
        public abstract class usrRTHLineDiscTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHLineDiscTotal> { }
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Line Discounts", Enabled = false)]
        public virtual Decimal? UsrRTHLineDiscTotal
        {
            get;
            set;
        }
        #endregion


        #region UsrRTHCuryDiscTot
        public abstract class usrRTHCuryDiscTot : PX.Data.BQL.BqlDecimal.Field<usrRTHCuryDiscTot> { }
        [PXDBCurrency(typeof(SOOrder.curyInfoID), typeof(SOOrderExt.usrRTHDiscTot))]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Document Discounts", Enabled = false)]
        public virtual Decimal? UsrRTHCuryDiscTot
        {
            get;
            set;
        }
        #endregion
        #region UsrRTHDiscTot
        public abstract class usrRTHDiscTot : PX.Data.BQL.BqlDecimal.Field<usrRTHDiscTot> { }
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Group and Document Discount Total")]
        public virtual Decimal? UsrRTHDiscTot
        {
            get;
            set;
        }
        #endregion

        #region UsrRTHCuryFreightTot
        public abstract class usrRTHCuryFreightTot : PX.Data.BQL.BqlDecimal.Field<usrRTHCuryFreightTot> { }
        protected Decimal? _UsrRTHCuryFreightTot;
        [PXDBCurrency(typeof(SOOrder.curyInfoID), typeof(SOOrderExt.usrRTHFreightTot))]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Add<SOOrder.curyPremiumFreightAmt, SOOrder.curyFreightAmt>))]
        [PXUIField(DisplayName = "RTH Freight Total", Enabled = false)]
        public virtual Decimal? UsrRTHCuryFreightTot
        {
            get
            {
                return this._UsrRTHCuryFreightTot;
            }
            set
            {
                this._UsrRTHCuryFreightTot = value;
            }
        }
        #endregion
        #region UsrRTHFreightTot
        public abstract class usrRTHFreightTot : PX.Data.BQL.BqlDecimal.Field<usrRTHFreightTot> { }
        protected Decimal? _UsrRTHFreightTot;
        [PXDBDecimal(4)]
        public virtual Decimal? UsrRTHFreightTot
        {
            get
            {
                return this._UsrRTHFreightTot;
            }
            set
            {
                this._UsrRTHFreightTot = value;
            }
        }
        #endregion

        #region UsrRTHCuryTaxTotal
        public abstract class usrRTHCuryTaxTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHCuryTaxTotal> { }
        protected Decimal? _UsrRTHCuryTaxTotal;
        [PXDBCurrency(typeof(SOOrder.curyInfoID), typeof(SOOrderExt.usrRTHTaxTotal))]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Tax Total", Enabled = false)]
        public virtual Decimal? UsrRTHCuryTaxTotal
        {
            get
            {
                return this._UsrRTHCuryTaxTotal;
            }
            set
            {
                this._UsrRTHCuryTaxTotal = value;
            }
        }
        #endregion
        #region UsrRTHTaxTotal
        public abstract class usrRTHTaxTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHTaxTotal> { }
        protected Decimal? _UsrRTHTaxTotal;
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? UsrRTHTaxTotal
        {
            get
            {
                return this._UsrRTHTaxTotal;
            }
            set
            {
                this._UsrRTHTaxTotal = value;
            }
        }
        #endregion

        #region UsrRTHCuryOrderTotal
        public abstract class usrRTHCuryOrderTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHCuryOrderTotal> { }
        protected Decimal? _UsrRTHCuryOrderTotal;
        [PXDBCurrency(typeof(SOOrder.curyInfoID), typeof(SOOrderExt.usrRTHOrderTotal))]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Order Total", Enabled = false)]
        public virtual Decimal? UsrRTHCuryOrderTotal
        {
            get
            {
                return this._UsrRTHCuryOrderTotal;
            }
            set
            {
                this._UsrRTHCuryOrderTotal = value;
            }
        }
        #endregion
        #region UsrRTHOrderTotal
        public abstract class usrRTHOrderTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHOrderTotal> { }
        protected Decimal? _UsrRTHOrderTotal;
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? UsrRTHOrderTotal
        {
            get
            {
                return this._UsrRTHOrderTotal;
            }
            set
            {
                this._UsrRTHOrderTotal = value;
            }
        }
        #endregion

    }
}