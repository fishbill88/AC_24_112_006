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
using PX.Objects.IN;
using PX.Data.ReferentialIntegrity.Attributes;

namespace PX.Objects.FS
{
	[Serializable]
    [PXCacheName(TX.TableName.FSServiceTemplateDet)]
    public class FSServiceTemplateDet : PXBqlTable, PX.Data.IBqlTable
	{
        #region Keys
        public class PK : PrimaryKeyOf<FSServiceTemplateDet>.By<serviceTemplateID, serviceTemplateDetID>
        {
            public static FSServiceTemplateDet Find(PXGraph graph, int? serviceTemplateID, int? serviceTemplateDetID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, serviceTemplateID, serviceTemplateDetID, options);
        }

        public static class FK
        {
            public class ServiceTemplate : FSServiceTemplate.PK.ForeignKeyOf<FSServiceTemplateDet>.By<serviceTemplateID> { }
            public class InventoryItem : IN.InventoryItem.PK.ForeignKeyOf<FSServiceTemplateDet>.By<inventoryID> { }
        }
        #endregion

        #region ServiceTemplateID
            public abstract class serviceTemplateID : PX.Data.BQL.BqlInt.Field<serviceTemplateID> { }

        [PXDBInt(IsKey = true)]
        [PXParent(typeof(Select<FSServiceTemplate, Where<FSServiceTemplate.serviceTemplateID, Equal<Current<FSServiceTemplateDet.serviceTemplateID>>>>))]
        [PXDBDefault(typeof(FSServiceTemplate.serviceTemplateID))]
        [PXUIField(DisplayName = "Service Template ID")]
        public virtual int? ServiceTemplateID { get; set; }
        #endregion
        #region ServiceTemplateDetID
        public abstract class serviceTemplateDetID : PX.Data.BQL.BqlInt.Field<serviceTemplateDetID> { }

        [PXDBIdentity(IsKey = true)]
        [PXUIField(Enabled = false)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? ServiceTemplateDetID { get; set; }
        #endregion
        #region LineType
        public abstract class lineType : ListField_LineType_UnifyTabs
        {
        }

        [PXDBString(5, IsFixed = true)]
        [PXDefault(ID.LineType_ServiceTemplate.SERVICE)]
        [PXUIField(DisplayName = "Line Type")]
        [lineType.ListAtrribute]
        public virtual string LineType { get; set; }
        #endregion
        #region InventoryID
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<lineType>))]
        [InventoryIDByLineType(typeof(lineType), Filterable = true)]
        [PXRestrictor(typeof(
                        Where<
                            InventoryItem.itemType, NotEqual<INItemTypes.serviceItem>,
                            Or<FSxServiceClass.requireRoute, Equal<True>,
                            Or<Current<FSSrvOrdType.requireRoute>, Equal<False>>>>),
                TX.Error.NONROUTE_SERVICE_CANNOT_BE_HANDLED_WITH_ROUTE_SRVORDTYPE)]
        [PXRestrictor(typeof(
                        Where<
                            InventoryItem.itemType, NotEqual<INItemTypes.serviceItem>,
                            Or<FSxServiceClass.requireRoute, Equal<False>,
                            Or<Current<FSSrvOrdType.requireRoute>, Equal<True>>>>),
                TX.Error.ROUTE_SERVICE_CANNOT_BE_HANDLED_WITH_NONROUTE_SRVORDTYPE)]
        public virtual int? InventoryID { get; set; }
        #endregion
		#region UOM
		public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }

		/// <summary>
		/// The <see cref="INUnit">Unit of Measure</see> used as the sales unit for the Inventory Item.
		/// </summary>
		/// <value>
		/// Defaults to the <see cref="InventoryItem.SalesUnit">Sales Unit</see> associated with the <see cref="InventoryID">Inventory Item</see>.
		/// Corresponds to the <see cref="INUnit.FromUnit"/> field.
		/// </value>
		[PXDefault(typeof(Search<InventoryItem.salesUnit, Where<InventoryItem.inventoryID, Equal<Current<inventoryID>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		[INUnit(typeof(inventoryID), DisplayName = "UOM")]
		public virtual string UOM { get; set; }
		#endregion
        #region Qty
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }

        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "1.0")]
        [PXUIField(DisplayName = "Quantity")]
        public virtual decimal? Qty { get; set; }
        #endregion
        #region TranDesc
        public abstract class tranDesc : PX.Data.BQL.BqlString.Field<tranDesc> { }

        [PXDBString(Common.Constants.TranDescLength, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Selector<inventoryID, InventoryItem.descr>))]
        [PXUIField(DisplayName = "Transaction Description")]
        public virtual string TranDesc { get; set; }
        #endregion
        #region CreatedByID
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }

        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }
        #endregion
        #region CreatedByScreenID
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }

        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }
        #endregion
        #region CreatedDateTime
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }

        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion
        #region LastModifiedByID
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }

        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID { get; set; }
        #endregion
        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }

        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID { get; set; }
        #endregion
        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }

        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion
        #region tstamp
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

        [PXDBTimestamp(VerifyTimestamp = VerifyTimestampOptions.BothFromGraphAndRecord)]
        public virtual byte[] tstamp { get; set; }
        #endregion
    }
}
