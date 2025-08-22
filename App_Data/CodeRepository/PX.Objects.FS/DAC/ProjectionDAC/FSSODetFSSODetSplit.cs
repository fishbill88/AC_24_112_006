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
using PX.Objects.IN;
using System;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Data.ReferentialIntegrity.Attributes;

namespace PX.Objects.FS
{
    [Serializable]
    [PXProjection(typeof(Select2<FSSODet,
                    LeftJoin<FSSODetSplit,
                        On<FSSODetSplit.srvOrdType, Equal<FSSODet.srvOrdType>,
                            And<FSSODetSplit.refNbr, Equal<FSSODet.refNbr>,
                            And<FSSODetSplit.lineNbr, Equal<FSSODet.lineNbr>>>>>>))]
    public class FSSODetFSSODetSplit : PXBqlTable, IBqlTable
    {
        #region Keys
        public class PK : PrimaryKeyOf<FSSODetFSSODetSplit>.By<srvOrdType, refNbr, lineNbr>
        {
            public static FSSODetFSSODetSplit Find(PXGraph graph, string srvOrdType, string refNbr, int? lineNbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, srvOrdType, refNbr, lineNbr, options);
        }

        public static class FK
        {
            public class ServiceOrderType : FSSrvOrdType.PK.ForeignKeyOf<FSSODetFSSODetSplit>.By<srvOrdType> { }
            public class ServiceOrder : FSServiceOrder.PK.ForeignKeyOf<FSSODetFSSODetSplit>.By<srvOrdType, refNbr> { }
            public class ServiceOrderLine : FSSODet.PK.ForeignKeyOf<FSSODetFSSODetSplit>.By<srvOrdType, refNbr, lineNbr> { }
            public class InventoryItem : IN.InventoryItem.PK.ForeignKeyOf<FSSODetFSSODetSplit>.By<inventoryID> { }
            public class ItemPlan : INItemPlan.PK.ForeignKeyOf<FSSODetFSSODetSplit>.By<planID> { }
        }
        #endregion

        #region SrvOrdType
        public abstract class srvOrdType : PX.Data.BQL.BqlString.Field<srvOrdType> { }

        [PXDBString(4, IsKey = true, IsFixed = true, BqlField = typeof(FSSODet.srvOrdType))]
        [PXUIField(DisplayName = "Service Order Type")]
        public virtual string SrvOrdType { get; set; }
        #endregion
        #region RefNbr
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }

        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "", BqlField = typeof(FSSODet.refNbr))]
        [PXUIField(DisplayName = "Service Order Nbr.")]
        public virtual string RefNbr { get; set; }
        #endregion
        #region LineNbr
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }

        [PXDBInt(IsKey = true, BqlField = typeof(FSSODet.lineNbr))]
        [PXUIField(DisplayName = "Line Nbr.")]
        public virtual int? LineNbr { get; set; }
        #endregion
        #region InventoryID
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

        [PXDBInt(BqlField = typeof(FSSODetSplit.inventoryID))]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual int? InventoryID { get; set; }
        #endregion
        #region PlanID
        public abstract class planID : PX.Data.BQL.BqlLong.Field<planID> { }

        [PXDBLong(IsImmutable = true, BqlField = typeof(FSSODetSplit.planID))]
        [PXUIField(DisplayName = "Plan ID")]
        public virtual Int64? PlanID { get; set; }
        #endregion
        #region UnitPrice
        public abstract class unitPrice : PX.Data.BQL.BqlDecimal.Field<unitPrice> { }

        [PXDBPriceCost(BqlField = typeof(FSSODet.unitPrice))]
        [PXUIField(DisplayName = "Base Unit Price")]
        public virtual Decimal? UnitPrice { get; set; }
        #endregion
        #region UOM
        public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }

        [PXDBString(BqlField = typeof(FSSODet.uOM))]
        [PXUIField(DisplayName = "UOM")]
        public virtual string UOM { get; set; }
        #endregion
    }

}
