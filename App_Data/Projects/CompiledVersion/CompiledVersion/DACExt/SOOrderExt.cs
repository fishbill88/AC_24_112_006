using PX.Data;
using PX.Objects.SO;
using PX.Objects.CM;
using System;

namespace CompiledVersion.DAC
{
    public sealed class SOOrderExt : PXCacheExtension<SOOrder>
    {
        
        public static bool IsActive() => true;
        #region UsrFreightPriceLimit
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Freight Limit")]
        [PXUIVisible(typeof(Where<Current<SOOrder.shipTermsID>, Equal<Current<SOSetupExt.usrNotToExceed>>>))]
        public decimal? UsrFreightPriceLimit { get; set; }
        public abstract class usrFreightPriceLimit : PX.Data.BQL.BqlDecimal.Field<usrFreightPriceLimit> { }
        #endregion

        #region UsrFreightTotal
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Freight Total", Enabled = false)]
        public decimal? UsrFreightTotal { get; set; }
        public abstract class usrFreightTotal : PX.Data.BQL.BqlDecimal.Field<usrFreightTotal> { }
        #endregion

        #region UsrShippingInstructions
        [PXDBString(500, IsUnicode = true)]
        [PXUIField(DisplayName = "Shipping Instructions")]
        public string UsrShippingInstructions { get; set; }
        public abstract class usrShippingInstructions : PX.Data.BQL.BqlString.Field<usrShippingInstructions> { }
        #endregion

        #region UsrShippingNotes
        [PXDBString(500, IsUnicode = true)]
        [PXUIField(DisplayName = "Internal Shipping Notes")]
        public string UsrShippingNotes { get; set; }
        public abstract class usrShippingNotes : PX.Data.BQL.BqlString.Field<usrShippingNotes> { }
        #endregion

        #region UsrCustomerAccount
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Customer Account")]
        [PXUIVisible(typeof(Where<Current<SOOrder.useCustomerAccount>, Equal<True>>))]
        public string UsrCustomerAccount { get; set; }
        public abstract class usrCustomerAccount : PX.Data.BQL.BqlString.Field<usrCustomerAccount> { }
        #endregion

        #region UsrRTHOrderQty
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "RTH Ordered Qty.", Enabled = false)]
        public decimal? UsrRTHOrderQty { get; set; }
        public abstract class usrRTHOrderQty : PX.Data.BQL.BqlDecimal.Field<usrRTHOrderQty> { }
        #endregion

        #region UsrRTHCuryDetaiExtPricelTotal
        public abstract class usrRTHCuryDetailExtPriceTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHCuryDetailExtPriceTotal> { }
        [PXCurrency(typeof(SOOrder.curyInfoID), typeof(SOOrderExt.usrRTHDetailExtPriceTotal))]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBCalced(typeof(Add<SOOrder.curyGoodsExtPriceTotal, SOOrder.curyMiscExtPriceTotal>), typeof(Decimal))]
        [PXFormula(typeof(Add<SOOrder.curyGoodsExtPriceTotal, SOOrder.curyMiscExtPriceTotal>))]
        [PXUIField(DisplayName = "RTH Detail Total", Enabled = false)]
        public Decimal? UsrRTHCuryDetailExtPriceTotal
        {
            get;
            set;
        }
        #endregion
        #region UsrRTHDetailExtPriceTotal

        public abstract class usrRTHDetailExtPriceTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHDetailExtPriceTotal> { }
        [PXDecimal(4)]
        [PXDBCalced(typeof(Add<SOOrder.goodsExtPriceTotal, SOOrder.miscExtPriceTotal>), typeof(Decimal))]
        public Decimal? UsrRTHDetailExtPriceTotal
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
        public Decimal? UsrRTHCuryLineDiscTotal
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
        public Decimal? UsrRTHLineDiscTotal
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
        public Decimal? UsrRTHCuryDiscTot
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
        public Decimal? UsrRTHDiscTot
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
        public Decimal? UsrRTHCuryFreightTot
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
        public Decimal? UsrRTHFreightTot
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
        public Decimal? UsrRTHCuryTaxTotal
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
        public Decimal? UsrRTHTaxTotal
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
        public Decimal? UsrRTHCuryOrderTotal
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
        public Decimal? UsrRTHOrderTotal
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

        #region UsrHubspotDealID    
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.HubspotDealID, Enabled = true)]
        public string UsrHubspotDealID { get; set; }
        public abstract class usrHubspotDealID : PX.Data.BQL.BqlString.Field<usrHubspotDealID> { }
        #endregion

        // Unbound display fields for SOCreateShipment grid
        #region UsrBillCompleteDisplay
        public abstract class usrBillCompleteDisplay : PX.Data.BQL.BqlBool.Field<usrBillCompleteDisplay> { }
        [PXBool]
        [PXUIField(DisplayName = "Bill Complete", Enabled = false)]
        public bool? UsrBillCompleteDisplay { get; set; }
        #endregion

        #region UsrFormTypeDisplay
        public abstract class usrFormTypeDisplay : PX.Data.BQL.BqlString.Field<usrFormTypeDisplay> { }
        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Form Type", Enabled = false)]
        public string UsrFormTypeDisplay { get; set; }
        #endregion

        // New: Bill-To Email (SOBillingContact.Email) for processing screens like SO501000
        #region UsrBillToEmail
        public abstract class usrBillToEmail : PX.Data.BQL.BqlString.Field<usrBillToEmail> { }
        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Bill-To Email", Enabled = false)]
        public string UsrBillToEmail { get; set; }
        #endregion
    }
}