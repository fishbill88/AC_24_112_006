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
using PX.Objects.IN;

namespace PX.Objects.FS
{
    #region PXProjection
    [Serializable]
    [PXProjection(typeof(
            Select2<FSAppointmentDet,
                InnerJoin<FSAppointment,
                    On<FSAppointment.appointmentID, Equal<FSAppointmentDet.appointmentID>>,
                InnerJoin<FSPostInfo,
                    On<FSAppointmentDet.postID, Equal<FSPostInfo.postID>>,
                InnerJoin<FSPostDet,
                    On<FSPostDet.postID, Equal<FSPostInfo.postID>>,
                InnerJoin<FSServiceOrder,
                    On<FSServiceOrder.sOID, Equal<FSAppointment.sOID>>,
                LeftJoin<Customer,
                    On<Customer.bAccountID, Equal<FSServiceOrder.billCustomerID>>,
                InnerJoin<FSAddress,
                    On<FSAddress.addressID, Equal<FSServiceOrder.serviceOrderAddressID>>,
                LeftJoin<FSGeoZonePostalCode,
                    On<FSGeoZonePostalCode.postalCode, Equal<FSAddress.postalCode>>,
                LeftJoin<FSGeoZone,
                    On<FSGeoZone.geoZoneID, Equal<FSGeoZonePostalCode.geoZoneID>>>>>>>>>>,
                Where<FSAppointmentDet.lineType, Equal<ListField_LineType_Pickup_Delivery.Pickup_Delivery>>>))]
    #endregion
    public class InventoryPostingBatchDetail : PostingBatchDetail
    {
        #region AppointmentInventoryItemID
        public abstract class appointmentInventoryItemID : PX.Data.BQL.BqlInt.Field<appointmentInventoryItemID> { }

        [PXDBInt(BqlField = typeof(FSAppointmentDet.appDetID))]
        public virtual int? AppointmentInventoryItemID { get; set; }
        #endregion
        #region SODetID
        public abstract class sODetID : PX.Data.BQL.BqlInt.Field<sODetID> { }

        [PXDBInt(BqlField = typeof(FSAppointmentDet.sODetID))]
        [PXUIField(DisplayName = "Service Order Detail Ref. Nbr.")]
        [PXSelector(typeof(Search<FSSODet.sODetID,
                       Where<
                           FSSODet.sOID, Equal<Current<InventoryPostingBatchDetail.appointmentID>>,
                           And<
                               FSSODet.status, NotEqual<FSSODet.status.Canceled>>>>), SubstituteKey = typeof(FSSODet.lineRef))]
        public virtual int? SODetID { get; set; }
        #endregion
        #region InventoryID
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

        [PXDBInt(BqlField = typeof(FSAppointmentDet.inventoryID))]
        [PXUIField(DisplayName = "Inventory ID")]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>), SubstituteKey = typeof(InventoryItem.inventoryCD))]
        public virtual int? InventoryID { get; set; }
        #endregion
    }
}
