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
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects.PO;
using System.Collections.Generic;
using System.Linq;
using PX.Objects.FS.DAC;

namespace PX.Objects.FS
{
    public class SM_ARReleaseProcess : PXGraphExtension<ARReleaseProcess>
    {
        #region ItemInfo
        public class ItemInfo
        {
            public virtual string LotSerialNbr { get; set; }
            public virtual string UOM { get; set; }
            public virtual decimal? Qty { get; set; }
            public virtual decimal? BaseQty { get; set; }

            #region Ctors
            public ItemInfo(SOShipLineSplit split)
            {
                LotSerialNbr = split.LotSerialNbr;
                UOM = split.UOM;
                Qty = split.Qty;
                BaseQty = split.BaseQty;
            }
            public ItemInfo(SOLineSplit split)
            {
                LotSerialNbr = split.LotSerialNbr;
                UOM = split.UOM;
                Qty = split.Qty;
                BaseQty = split.BaseQty;
            }
            public ItemInfo(ARTran arTran)
            {
                LotSerialNbr = arTran.LotSerialNbr;
                UOM = arTran.UOM;
                Qty = arTran.Qty;
                BaseQty = arTran.BaseQty;
            }
			public ItemInfo(POReceiptLine poRLine)
			{
				LotSerialNbr = poRLine.LotSerialNbr;
				UOM = poRLine.UOM;
				Qty = poRLine.Qty;
				BaseQty = poRLine.BaseQty;
			}
            #endregion
        }
		#endregion

		#region Views

		public PXSelectJoin<POReceiptLine,
				InnerJoin<POOrder, On<POOrder.orderType, Equal<POReceiptLine.pOType>, And<POOrder.orderNbr, Equal<POReceiptLine.pONbr>>>>,
				Where<POOrder.sOOrderType, Equal<Required<POOrder.sOOrderType>>,
					And<POOrder.sOOrderNbr, Equal<Required<POOrder.sOOrderNbr>>>>
			> POReceiptLines;

		#endregion

		public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.serviceManagementModule>();
        }

        public bool processEquipmentAndComponents = false;

        #region Overrides
        public delegate void PersistDelegate();

        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                if (SharedFunctions.isFSSetupSet(Base) == true)
                {
                    ARInvoice arInvoiceRow = Base.ARInvoice_DocType_RefNbr.Current;

                    if (arInvoiceRow != null)
                    {
                        if (arInvoiceRow.DocType == ARDocType.CreditMemo
                            && arInvoiceRow.CreatedByScreenID.Substring(0, 2) != "FS"
                            && arInvoiceRow.Released == true)
                        {
                            CleanPostingInfoCreditMemo(arInvoiceRow);
                        }

                        if (PXAccess.FeatureInstalled<FeaturesSet.equipmentManagementModule>() == true && processEquipmentAndComponents)
                        {
                            Dictionary<int?, int?> newEquiments = new Dictionary<int?, int?>();
                            SMEquipmentMaint graphSMEquipmentMaint = PXGraph.CreateInstance<SMEquipmentMaint>();

                            CreateEquipments(graphSMEquipmentMaint, arInvoiceRow, newEquiments);
                            ReplaceEquipments(graphSMEquipmentMaint, arInvoiceRow);
                            UpgradeEquipmentComponents(graphSMEquipmentMaint, arInvoiceRow, newEquiments);
                            CreateEquipmentComponents(graphSMEquipmentMaint, arInvoiceRow, newEquiments);
                            ReplaceComponents(graphSMEquipmentMaint, arInvoiceRow);
                        }
                    }
                }

                baseMethod();

                ts.Complete();
            }
        }

        public delegate ARRegister OnBeforeReleaseDelegate(ARRegister ardoc);
        
        [PXOverride]
        public virtual ARRegister OnBeforeRelease(ARRegister ardoc, OnBeforeReleaseDelegate del)
        {
            ValidatePostBatchStatus(PXDBOperation.Update, ID.Batch_PostTo.AR, ardoc.DocType, ardoc.RefNbr);

            if (del != null)
            {
                return del(ardoc);
            }

            return null;
        }
        #endregion

        #region Methods
        public virtual void CleanPostingInfoCreditMemo(ARRegister arRegisterRow)
        {
            var anyARTranRowRelated = PXSelect<ARTran,
                                        Where<
                                            ARTran.tranType, Equal<Required<ARTran.tranType>>,
                                        And<
                                            ARTran.refNbr, Equal<Required<ARTran.refNbr>>>>>
                                        .Select(Base, arRegisterRow.DocType, arRegisterRow.RefNbr)
                                        .RowCast<ARTran>()
                                        .Where(_ => Base.Caches[typeof(ARTran)].GetExtension<FSxARTran>(_).IsFSRelated == true)
                                        .Any();

            if (anyARTranRowRelated == true)
            {
                SOInvoice crmSOInvoiceRow = PXSelect<SOInvoice,
                                            Where<
                                                SOInvoice.docType, Equal<Required<SOInvoice.docType>>,
                                            And<
                                                SOInvoice.refNbr, Equal<Required<SOInvoice.refNbr>>>>>
                                            .Select(Base, arRegisterRow.DocType, arRegisterRow.RefNbr);

                if (crmSOInvoiceRow != null)
                {
                    var SOInvoiceGraph = PXGraph.CreateInstance<SOInvoiceEntry>();
                    SM_SOInvoiceEntry extGraph = SOInvoiceGraph.GetExtension<SM_SOInvoiceEntry>();

                    // TODO: Add OrigDocAmt and CuryID validation with the invoice (parent document).

                    extGraph.CleanPostingInfoFromSOCreditMemo(Base, crmSOInvoiceRow);

                    // TODO: Which is the parent document?
                    extGraph.CreateBillHistoryRowsForDocument(Base,
                                FSEntityType.SOCreditMemo, arRegisterRow.DocType, arRegisterRow.RefNbr,
                                null, null, null);
                }
                else
                {
                    var ARInvoiceGraph = PXGraph.CreateInstance<ARInvoiceEntry>();
                    SM_ARInvoiceEntry extGraph = ARInvoiceGraph.GetExtension<SM_ARInvoiceEntry>();

                    ARInvoice origARInvoiceRow = PXSelect<ARInvoice,
                                                 Where<
                                                     ARInvoice.docType, Equal<Required<ARInvoice.docType>>,
                                                 And<
                                                     ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>
                                                 .Select(Base, arRegisterRow.OrigDocType, arRegisterRow.OrigRefNbr)
                                                 .FirstOrDefault();

                    // To unlink the FSDocuments from the invoice
                    // the credit memo must reverse the invoice completely
                    if (origARInvoiceRow.OrigDocAmt == arRegisterRow.OrigDocAmt
                        && origARInvoiceRow.CuryID == arRegisterRow.CuryID)
                    {
                        extGraph.CleanPostingInfoLinkedToDoc(origARInvoiceRow);
                        extGraph.CleanContractPostingInfoLinkedToDoc(origARInvoiceRow);

                        extGraph.CreateBillHistoryRowsForDocument(Base,
                                    FSEntityType.ARCreditMemo, arRegisterRow.DocType, arRegisterRow.RefNbr,
                                    FSEntityType.ARInvoice, arRegisterRow.OrigDocType, arRegisterRow.OrigRefNbr);
                    }
                }
            }
        }

        public virtual void CreateEquipments(SMEquipmentMaint graphSMEquipmentMaint,
                                             ARRegister arRegisterRow,
                                             Dictionary<int?, int?> newEquiments)
        {
            var inventoryItemSet = PXSelectJoin<InventoryItem,
                                   InnerJoin<ARTran,
                                            On<ARTran.inventoryID, Equal<InventoryItem.inventoryID>,
                                            And<ARTran.tranType, Equal<ARDocType.invoice>>>,
                                   LeftJoin<SOLine,
                                            On<SOLine.orderType, Equal<ARTran.sOOrderType>,
                                            And<SOLine.orderNbr, Equal<ARTran.sOOrderNbr>,
                                            And<SOLine.lineNbr, Equal<ARTran.sOOrderLineNbr>>>>,
                                   LeftJoin<FSServiceOrder,
                                            On<FSServiceOrder.srvOrdType, Equal<FSxARTran.srvOrdType>,
                                            And<FSServiceOrder.refNbr, Equal<FSxARTran.serviceOrderRefNbr>>>,
                                    LeftJoin<FSAppointment,
                                            On<FSAppointment.srvOrdType, Equal<FSxARTran.srvOrdType>,
                                            And<FSAppointment.refNbr, Equal<FSxARTran.appointmentRefNbr>>>>>>>,
                                   Where<
                                        ARTran.tranType, Equal<Required<ARInvoice.docType>>,
                                        And<ARTran.refNbr, Equal<Required<ARInvoice.refNbr>>,
                                        And<FSxEquipmentModel.eQEnabled, Equal<True>,
                                        And<FSxARTran.equipmentAction, Equal<ListField_EquipmentAction.SellingTargetEquipment>,
                                        And<FSxARTran.sMEquipmentID, IsNull,
                                        And<FSxARTran.newEquipmentLineNbr, IsNull,
                                        And<FSxARTran.componentID, IsNull>>>>>>>,
                                   OrderBy<
                                        Asc<ARTran.tranType,
                                        Asc<ARTran.refNbr,
                                        Asc<ARTran.lineNbr>>>>>
                                   .Select(Base, arRegisterRow.DocType, arRegisterRow.RefNbr);

            Create_Replace_Equipments(graphSMEquipmentMaint, inventoryItemSet, arRegisterRow, newEquiments, ID.Equipment_Action.SELLING_TARGET_EQUIPMENT);
        }

        public virtual void UpgradeEquipmentComponents(SMEquipmentMaint graphSMEquipmentMaint,
                                                       ARRegister arRegisterRow,
                                                       Dictionary<int?, int?> newEquiments)
        {
            var inventoryItemSet = PXSelectJoin<InventoryItem,
                                   InnerJoin<ARTran,
                                            On<ARTran.inventoryID, Equal<InventoryItem.inventoryID>,
                                            And<ARTran.tranType, Equal<ARDocType.invoice>>>,
                                    LeftJoin<SOLine,
                                            On<SOLine.orderType, Equal<ARTran.sOOrderType>,
                                            And<SOLine.orderNbr, Equal<ARTran.sOOrderNbr>,
                                            And<SOLine.lineNbr, Equal<ARTran.sOOrderLineNbr>>>>,
                                    LeftJoin<FSServiceOrder,
                                            On<FSServiceOrder.srvOrdType, Equal<FSxARTran.srvOrdType>,
                                            And<FSServiceOrder.refNbr, Equal<FSxARTran.serviceOrderRefNbr>>>,
                                    LeftJoin<FSAppointment,
                                            On<FSAppointment.srvOrdType, Equal<FSxARTran.srvOrdType>,
                                            And<FSAppointment.refNbr, Equal<FSxARTran.appointmentRefNbr>>>>>>>,
                                   Where<
                                        ARTran.tranType, Equal<Required<ARInvoice.docType>>,
                                        And<ARTran.refNbr, Equal<Required<ARInvoice.refNbr>>,
                                        And<FSxARTran.equipmentAction, Equal<ListField_EquipmentAction.UpgradingComponent>,
                                        And<FSxARTran.sMEquipmentID, IsNull,
                                        And<FSxARTran.newEquipmentLineNbr, IsNotNull,
                                        And<FSxARTran.componentID, IsNotNull,
                                        And<FSxARTran.equipmentComponentLineNbr, IsNull>>>>>>>,
                                   OrderBy<
                                        Asc<ARTran.tranType,
                                        Asc<ARTran.refNbr,
                                        Asc<ARTran.lineNbr>>>>>
                                   .Select(Base, arRegisterRow.DocType, arRegisterRow.RefNbr);

            foreach (PXResult<InventoryItem, ARTran, SOLine, FSServiceOrder, FSAppointment> bqlResult in inventoryItemSet)
            {
                ARTran arTranRow = (ARTran)bqlResult;
                SOLine soLineRow = (SOLine)bqlResult;
                InventoryItem inventoryItemRow = (InventoryItem)bqlResult;
				FSxARTran fsxARTranRow = Base.Caches[typeof(ARTran)].GetExtension<FSxARTran>(arTranRow);
                FSServiceOrder fsServiceOrderRow = (FSServiceOrder)bqlResult;
                FSAppointment fsAppointmentRow = (FSAppointment)bqlResult;

                int? smEquipmentID = -1;
                if (newEquiments.TryGetValue(fsxARTranRow.NewEquipmentLineNbr, out smEquipmentID))
                {
                    graphSMEquipmentMaint.EquipmentRecords.Current = graphSMEquipmentMaint.EquipmentRecords.Search<FSEquipment.SMequipmentID>(smEquipmentID);

                    FSEquipmentComponent fsEquipmentComponentRow = graphSMEquipmentMaint.EquipmentWarranties.Select().Where(x => ((FSEquipmentComponent)x).ComponentID == fsxARTranRow.ComponentID).FirstOrDefault();

                    if (fsEquipmentComponentRow != null)
                    {
                        fsEquipmentComponentRow.SalesOrderNbr = arTranRow.SOOrderNbr;
                        fsEquipmentComponentRow.SalesOrderType = arTranRow.SOOrderType;
                        fsEquipmentComponentRow.LongDescr = arTranRow.TranDesc;
                        fsEquipmentComponentRow.InvoiceRefNbr = arTranRow.RefNbr;
                        fsEquipmentComponentRow.InstallationDate = arTranRow.TranDate != null ? arTranRow.TranDate : arRegisterRow.DocDate;

                        if (fsxARTranRow != null)
                        {
                            if (string.IsNullOrEmpty(fsxARTranRow.AppointmentRefNbr) == false)
                            {
                                fsEquipmentComponentRow.InstSrvOrdType = fsxARTranRow.SrvOrdType;
                                fsEquipmentComponentRow.InstAppointmentRefNbr = fsxARTranRow.AppointmentRefNbr;
                                fsEquipmentComponentRow.InstallationDate = fsAppointmentRow?.ExecutionDate;
                            }
                            else if (string.IsNullOrEmpty(fsxARTranRow.ServiceOrderRefNbr) == false)
                            {
                                fsEquipmentComponentRow.InstSrvOrdType = fsxARTranRow.SrvOrdType;
                                fsEquipmentComponentRow.InstServiceOrderRefNbr = fsxARTranRow.ServiceOrderRefNbr;
                                fsEquipmentComponentRow.InstallationDate = fsServiceOrderRow?.OrderDate;
                            }

                            fsEquipmentComponentRow.Comment = fsxARTranRow.Comment;
                        }

                        // Component actions are assumed to always run for only one item per line (BaseQty == 1).
                        fsEquipmentComponentRow.SerialNumber = arTranRow.LotSerialNbr;

                        fsEquipmentComponentRow = graphSMEquipmentMaint.EquipmentWarranties.Update(fsEquipmentComponentRow);

                        graphSMEquipmentMaint.EquipmentWarranties.SetValueExt<FSEquipmentComponent.inventoryID>(fsEquipmentComponentRow, arTranRow.InventoryID);
                        graphSMEquipmentMaint.EquipmentWarranties.SetValueExt<FSEquipmentComponent.salesDate>(fsEquipmentComponentRow, soLineRow != null && soLineRow.OrderDate != null ? soLineRow.OrderDate : arTranRow.TranDate);
                        graphSMEquipmentMaint.Save.Press();
                    }
                }
            }
        }

        public virtual void CreateEquipmentComponents(SMEquipmentMaint graphSMEquipmentMaint,
                                                      ARRegister arRegisterRow,
                                                      Dictionary<int?, int?> newEquiments)
        {
            var inventoryItemSet = PXSelectJoin<InventoryItem,
                                   InnerJoin<ARTran,
                                            On<ARTran.inventoryID, Equal<InventoryItem.inventoryID>,
                                            And<ARTran.tranType, Equal<ARDocType.invoice>>>,
                                   LeftJoin<SOLine,
                                        On<SOLine.orderType, Equal<ARTran.sOOrderType>,
                                        And<SOLine.orderNbr, Equal<ARTran.sOOrderNbr>,
                                            And<SOLine.lineNbr, Equal<ARTran.sOOrderLineNbr>>>>,
                                   LeftJoin<FSServiceOrder,
                                            On<FSServiceOrder.srvOrdType, Equal<FSxARTran.srvOrdType>,
                                            And<FSServiceOrder.refNbr, Equal<FSxARTran.serviceOrderRefNbr>>>,
                                    LeftJoin<FSAppointment,
                                            On<FSAppointment.srvOrdType, Equal<FSxARTran.srvOrdType>,
                                            And<FSAppointment.refNbr, Equal<FSxARTran.appointmentRefNbr>>>>>>>,
                                   Where<
                                        ARTran.tranType, Equal<Required<ARInvoice.docType>>,
                                        And<ARTran.refNbr, Equal<Required<ARInvoice.refNbr>>,
                                        And<FSxARTran.equipmentAction, Equal<ListField_EquipmentAction.CreatingComponent>,
                                        And<FSxARTran.componentID, IsNotNull,
                                        And<FSxARTran.equipmentComponentLineNbr, IsNull>>>>>,
                                   OrderBy<
                                        Asc<ARTran.tranType,
                                        Asc<ARTran.refNbr,
                                        Asc<ARTran.lineNbr>>>>>
                                   .Select(Base, arRegisterRow.DocType, arRegisterRow.RefNbr);

			foreach (PXResult<InventoryItem, ARTran, SOLine, FSServiceOrder, FSAppointment> bqlResult in inventoryItemSet)
            {
                ARTran arTranRow = (ARTran)bqlResult;
				FSxARTran fsxARTranRow = Base.Caches[typeof(ARTran)].GetExtension<FSxARTran>(arTranRow);
                SOLine soLineRow = (SOLine)bqlResult;
                FSServiceOrder fsServiceOrderRow = (FSServiceOrder)bqlResult;
                FSAppointment fsAppointmentRow = (FSAppointment)bqlResult;

                int? smEquipmentID = -1;
                if (fsxARTranRow.NewEquipmentLineNbr != null && fsxARTranRow.SMEquipmentID == null)
                {
                    if (newEquiments.TryGetValue(fsxARTranRow.NewEquipmentLineNbr, out smEquipmentID))
                    {
                        graphSMEquipmentMaint.EquipmentRecords.Current = graphSMEquipmentMaint.EquipmentRecords.Search<FSEquipment.SMequipmentID>(smEquipmentID);
                    }
                }

                if (fsxARTranRow.NewEquipmentLineNbr == null && fsxARTranRow.SMEquipmentID != null)
                {
                    graphSMEquipmentMaint.EquipmentRecords.Current = graphSMEquipmentMaint.EquipmentRecords.Search<FSEquipment.SMequipmentID>(fsxARTranRow.SMEquipmentID);
                }

                if (graphSMEquipmentMaint.EquipmentRecords.Current != null)
                {
                    FSEquipmentComponent fsEquipmentComponentRow = new FSEquipmentComponent();
                    fsEquipmentComponentRow.ComponentID = fsxARTranRow.ComponentID;
                    fsEquipmentComponentRow = graphSMEquipmentMaint.EquipmentWarranties.Insert(fsEquipmentComponentRow);

                    fsEquipmentComponentRow.SalesOrderNbr = arTranRow.SOOrderNbr;
                    fsEquipmentComponentRow.SalesOrderType = arTranRow.SOOrderType;
                    fsEquipmentComponentRow.InvoiceRefNbr = arTranRow.RefNbr;
                    fsEquipmentComponentRow.InstallationDate = arTranRow.TranDate != null ? arTranRow.TranDate : arRegisterRow.DocDate;

                    if (string.IsNullOrEmpty(fsxARTranRow.AppointmentRefNbr) == false)
                    {
                        fsEquipmentComponentRow.InstSrvOrdType = fsxARTranRow.SrvOrdType;
                        fsEquipmentComponentRow.InstAppointmentRefNbr = fsxARTranRow.AppointmentRefNbr;
                        fsEquipmentComponentRow.InstallationDate = fsAppointmentRow?.ExecutionDate;
                    }
                    else if (string.IsNullOrEmpty(fsxARTranRow.ServiceOrderRefNbr) == false)
                    {
                        fsEquipmentComponentRow.InstSrvOrdType = fsxARTranRow.SrvOrdType;
                        fsEquipmentComponentRow.InstServiceOrderRefNbr = fsxARTranRow.ServiceOrderRefNbr;
                        fsEquipmentComponentRow.InstallationDate = fsServiceOrderRow?.OrderDate;
                    }

                    fsEquipmentComponentRow.Comment = fsxARTranRow.Comment;

                    // Component actions are assumed to always run for only one item per line (BaseQty == 1).
                    fsEquipmentComponentRow.SerialNumber = arTranRow.LotSerialNbr;

                    fsEquipmentComponentRow = graphSMEquipmentMaint.EquipmentWarranties.Update(fsEquipmentComponentRow);

                    graphSMEquipmentMaint.EquipmentWarranties.SetValueExt<FSEquipmentComponent.inventoryID>(fsEquipmentComponentRow, arTranRow.InventoryID);
                    graphSMEquipmentMaint.EquipmentWarranties.SetValueExt<FSEquipmentComponent.salesDate>(fsEquipmentComponentRow, soLineRow != null && soLineRow.OrderDate != null ? soLineRow.OrderDate : arTranRow.TranDate);
                    graphSMEquipmentMaint.Save.Press();
                }
            }
        }

        public virtual void ReplaceEquipments(SMEquipmentMaint graphSMEquipmentMaint, ARRegister arRegisterRow)
        {
            var inventoryItemSet = PXSelectJoin<InventoryItem,
                                   InnerJoin<ARTran,
                                            On<ARTran.inventoryID, Equal<InventoryItem.inventoryID>,
                                            And<ARTran.tranType, Equal<ARDocType.invoice>>>,
                                   LeftJoin<SOLine,
                                        On<SOLine.orderType, Equal<ARTran.sOOrderType>,
                                        And<SOLine.orderNbr, Equal<ARTran.sOOrderNbr>,
                                            And<SOLine.lineNbr, Equal<ARTran.sOOrderLineNbr>>>>,
                                   LeftJoin<FSServiceOrder,
                                            On<FSServiceOrder.srvOrdType, Equal<FSxARTran.srvOrdType>,
                                            And<FSServiceOrder.refNbr, Equal<FSxARTran.serviceOrderRefNbr>>>,
                                    LeftJoin<FSAppointment,
                                            On<FSAppointment.srvOrdType, Equal<FSxARTran.srvOrdType>,
                                            And<FSAppointment.refNbr, Equal<FSxARTran.appointmentRefNbr>>>>>>>,
                                   Where<
                                        ARTran.tranType, Equal<Required<ARInvoice.docType>>,
                                        And<ARTran.refNbr, Equal<Required<ARInvoice.refNbr>>,
                                        And<FSxEquipmentModel.eQEnabled, Equal<True>,
                                        And<FSxARTran.equipmentAction, Equal<ListField_EquipmentAction.ReplacingTargetEquipment>,
                                        And<FSxARTran.sMEquipmentID, IsNotNull,
                                        And<FSxARTran.newEquipmentLineNbr, IsNull,
                                        And<FSxARTran.componentID, IsNull>>>>>>>,
                                   OrderBy<
                                        Asc<ARTran.tranType,
                                        Asc<ARTran.refNbr,
                                        Asc<ARTran.lineNbr>>>>>
                                   .Select(Base, arRegisterRow.DocType, arRegisterRow.RefNbr);

            Create_Replace_Equipments(graphSMEquipmentMaint, inventoryItemSet, arRegisterRow, null, ID.Equipment_Action.REPLACING_TARGET_EQUIPMENT);
        }

        public virtual void ReplaceComponents(SMEquipmentMaint graphSMEquipmentMaint, ARRegister arRegisterRow)
        {
            var inventoryItemSet = PXSelectJoin<InventoryItem,
                                   InnerJoin<ARTran,
                                            On<ARTran.inventoryID, Equal<InventoryItem.inventoryID>,
                                            And<ARTran.tranType, Equal<ARDocType.invoice>>>,
                                   LeftJoin<SOLine,
                                        On<SOLine.orderType, Equal<ARTran.sOOrderType>,
                                        And<SOLine.orderNbr, Equal<ARTran.sOOrderNbr>,
                                            And<SOLine.lineNbr, Equal<ARTran.sOOrderLineNbr>>>>,
                                   LeftJoin<FSServiceOrder,
                                            On<FSServiceOrder.srvOrdType, Equal<FSxARTran.srvOrdType>,
                                            And<FSServiceOrder.refNbr, Equal<FSxARTran.serviceOrderRefNbr>>>,
                                    LeftJoin<FSAppointment,
                                            On<FSAppointment.srvOrdType, Equal<FSxARTran.srvOrdType>,
                                            And<FSAppointment.refNbr, Equal<FSxARTran.appointmentRefNbr>>>>>>>,
                                   Where<
                                        ARTran.tranType, Equal<Required<ARInvoice.docType>>,
                                        And<ARTran.refNbr, Equal<Required<ARInvoice.refNbr>>,
                                        And<FSxEquipmentModel.eQEnabled, Equal<True>,
                                        And<FSxARTran.equipmentAction, Equal<ListField_EquipmentAction.ReplacingComponent>,
                                        And<FSxARTran.sMEquipmentID, IsNotNull,
                                        And<FSxARTran.newEquipmentLineNbr, IsNull,
                                        And<FSxARTran.equipmentComponentLineNbr, IsNotNull>>>>>>>,
                                   OrderBy<
                                        Asc<ARTran.tranType,
                                        Asc<ARTran.refNbr,
                                        Asc<ARTran.lineNbr>>>>>
                                   .Select(Base, arRegisterRow.DocType, arRegisterRow.RefNbr);

            foreach (PXResult<InventoryItem, ARTran, SOLine, FSServiceOrder, FSAppointment> bqlResult in inventoryItemSet)
            {
                ARTran arTranRow = (ARTran)bqlResult;
                InventoryItem inventoryItemRow = (InventoryItem)bqlResult;
                SOLine soLineRow = (SOLine)bqlResult;
				FSxARTran fsxARTranRow = Base.Caches[typeof(ARTran)].GetExtension<FSxARTran>(arTranRow);
                FSServiceOrder fsServiceOrderRow = (FSServiceOrder)bqlResult;
                FSAppointment fsAppointmentRow = (FSAppointment)bqlResult;

                graphSMEquipmentMaint.EquipmentRecords.Current = graphSMEquipmentMaint.EquipmentRecords.Search<FSEquipment.SMequipmentID>(fsxARTranRow.SMEquipmentID);

                FSEquipmentComponent fsEquipmentComponentRow = graphSMEquipmentMaint.EquipmentWarranties.Select().Where(x => ((FSEquipmentComponent)x).LineNbr == fsxARTranRow.EquipmentComponentLineNbr).FirstOrDefault();

                FSEquipmentComponent fsNewEquipmentComponentRow = new FSEquipmentComponent();
                fsNewEquipmentComponentRow.ComponentID = fsxARTranRow.ComponentID;
                fsNewEquipmentComponentRow = graphSMEquipmentMaint.ApplyComponentReplacement(fsEquipmentComponentRow, fsNewEquipmentComponentRow);

                fsNewEquipmentComponentRow.SalesOrderNbr = arTranRow.SOOrderNbr;
                fsNewEquipmentComponentRow.SalesOrderType = arTranRow.SOOrderType;
                fsNewEquipmentComponentRow.InvoiceRefNbr = arTranRow.RefNbr;
                fsNewEquipmentComponentRow.InstallationDate = arTranRow.TranDate != null ? arTranRow.TranDate : arRegisterRow.DocDate;

                if (fsxARTranRow != null)
                {
                    if (string.IsNullOrEmpty(fsxARTranRow.AppointmentRefNbr) == false)
                    {
                        fsNewEquipmentComponentRow.InstSrvOrdType = fsxARTranRow.SrvOrdType;
                        fsNewEquipmentComponentRow.InstAppointmentRefNbr = fsxARTranRow.AppointmentRefNbr;
                        fsNewEquipmentComponentRow.InstallationDate = fsAppointmentRow?.ExecutionDate;
                    }
                    else if (string.IsNullOrEmpty(fsxARTranRow.ServiceOrderRefNbr) == false)
                    {
                        fsNewEquipmentComponentRow.InstSrvOrdType = fsxARTranRow.SrvOrdType;
                        fsNewEquipmentComponentRow.InstServiceOrderRefNbr = fsxARTranRow.ServiceOrderRefNbr;
                        fsNewEquipmentComponentRow.InstallationDate = fsServiceOrderRow?.OrderDate;
                    }

                    fsNewEquipmentComponentRow.Comment = fsxARTranRow.Comment;
                }

                fsNewEquipmentComponentRow.LongDescr = arTranRow.TranDesc;

                // Component actions are assumed to always run for only one item per line (BaseQty == 1).
                fsNewEquipmentComponentRow.SerialNumber = arTranRow.LotSerialNbr;

                fsNewEquipmentComponentRow = graphSMEquipmentMaint.EquipmentWarranties.Update(fsNewEquipmentComponentRow);

                graphSMEquipmentMaint.EquipmentWarranties.SetValueExt<FSEquipmentComponent.inventoryID>(fsNewEquipmentComponentRow, arTranRow.InventoryID);
                graphSMEquipmentMaint.EquipmentWarranties.SetValueExt<FSEquipmentComponent.salesDate>(fsNewEquipmentComponentRow, soLineRow != null && soLineRow.OrderDate != null ? soLineRow.OrderDate : arTranRow.TranDate);
                graphSMEquipmentMaint.Save.Press();
            }
        }

        // TODO: Change PXResultset<InventoryItem> to PXResult<InventoryItem, ARTran, SOLine>
        public virtual void Create_Replace_Equipments(
            SMEquipmentMaint graphSMEquipmentMaint,
            PXResultset<InventoryItem> arTranLines,
            ARRegister arRegisterRow,
            Dictionary<int?, int?> newEquiments,
            string action)
        {
            PXCache arTranCache = Base.Caches[typeof(ARTran)];
            bool needPersist = false;

            foreach (PXResult<InventoryItem, ARTran, SOLine, FSServiceOrder, FSAppointment> bqlResult in arTranLines)
            {
                ARTran arTranRow = (ARTran)bqlResult;

                //Fetching the cached data record for ARTran that will be updated later
                arTranRow = PXSelect<ARTran,
                            Where<
                                ARTran.tranType, Equal<Required<ARTran.tranType>>,
                                And<ARTran.refNbr, Equal<Required<ARTran.refNbr>>,
                                And<ARTran.lineNbr, Equal<Required<ARTran.lineNbr>>>>>>
                            .Select(Base, arTranRow.TranType, arTranRow.RefNbr, arTranRow.LineNbr);

                InventoryItem inventoryItemRow = (InventoryItem)bqlResult;
                SOLine soLineRow = (SOLine)bqlResult;
                FSServiceOrder fsServiceOrderRow = (FSServiceOrder)bqlResult;
                FSAppointment fsAppointmentRow = (FSAppointment)bqlResult;

                FSEquipment fsEquipmentRow = null;
                FSxEquipmentModel fsxEquipmentModelRow = PXCache<InventoryItem>.GetExtension<FSxEquipmentModel>(inventoryItemRow);
				FSxARTran fsxARTranRow = arTranCache.GetExtension<FSxARTran>(arTranRow);

				List<ItemInfo> items = GetDifferentItemList(Base, arTranRow, true);

				if (!items.Any())
					items = GetItemListFromPOReceipt(soLineRow);

				foreach (ItemInfo itemInfo in items)
                {
                    SoldInventoryItem soldInventoryItemRow = new SoldInventoryItem();

                    soldInventoryItemRow.CustomerID = arRegisterRow.CustomerID;
                    soldInventoryItemRow.CustomerLocationID = arRegisterRow.CustomerLocationID;
                    soldInventoryItemRow.InventoryID = inventoryItemRow.InventoryID;
                    soldInventoryItemRow.InventoryCD = inventoryItemRow.InventoryCD;
                    soldInventoryItemRow.InvoiceRefNbr = arTranRow.RefNbr;
                    soldInventoryItemRow.InvoiceLineNbr = arTranRow.LineNbr;
                    soldInventoryItemRow.DocType = arRegisterRow.DocType;
                    soldInventoryItemRow.DocDate = arTranRow.TranDate != null ? arTranRow.TranDate : arRegisterRow.DocDate;

                    if (string.IsNullOrEmpty(fsxARTranRow.AppointmentRefNbr) == false)
                    {
                        soldInventoryItemRow.DocDate = fsAppointmentRow.ExecutionDate;
                    }
                    else if (string.IsNullOrEmpty(fsxARTranRow.ServiceOrderRefNbr) == false)
                    {
                        soldInventoryItemRow.DocDate = fsServiceOrderRow.OrderDate;
                    }

                    soldInventoryItemRow.Descr = inventoryItemRow.Descr;
                    soldInventoryItemRow.SiteID = arTranRow.SiteID;
                    soldInventoryItemRow.ItemClassID = inventoryItemRow.ItemClassID;
                    soldInventoryItemRow.SOOrderType = arTranRow.SOOrderType;
                    soldInventoryItemRow.SOOrderNbr = arTranRow.SOOrderNbr;
                    soldInventoryItemRow.SOOrderDate = soLineRow.OrderDate;
                    soldInventoryItemRow.EquipmentTypeID = fsxEquipmentModelRow.EquipmentTypeID;

                    soldInventoryItemRow.LotSerialNumber = itemInfo.LotSerialNbr;

                    fsEquipmentRow = SharedFunctions.CreateSoldEquipment(graphSMEquipmentMaint, soldInventoryItemRow, arTranRow, fsxARTranRow, soLineRow, action, inventoryItemRow);
                }

                if (fsEquipmentRow != null)
                {
                    if (fsxARTranRow.ReplaceSMEquipmentID == null
                        && action == ID.Equipment_Action.REPLACING_TARGET_EQUIPMENT)
                    {
                        fsxARTranRow.ReplaceSMEquipmentID = fsxARTranRow.SMEquipmentID;
                    }

                    fsxARTranRow.SMEquipmentID = fsEquipmentRow.SMEquipmentID;
					arTranRow = (ARTran)arTranCache.Update(arTranRow);
                    needPersist = true;

                    if (action == ID.Equipment_Action.SELLING_TARGET_EQUIPMENT)
                    {
                        int? smEquipmentID = -1;
                        if (newEquiments.TryGetValue(arTranRow.LineNbr, out smEquipmentID) == false)
                        {
                            newEquiments.Add(
                                arTranRow.LineNbr,
                                fsEquipmentRow.SMEquipmentID);
                        }
                    }
                    else if (action == ID.Equipment_Action.REPLACING_TARGET_EQUIPMENT)
                    {
                        if (fsxARTranRow != null)
                        {
                            graphSMEquipmentMaint.EquipmentRecords.Current = graphSMEquipmentMaint.EquipmentRecords.Search<FSEquipment.SMequipmentID>(fsxARTranRow.ReplaceSMEquipmentID);
                            graphSMEquipmentMaint.EquipmentRecords.Current.ReplaceEquipmentID = fsEquipmentRow.SMEquipmentID;
                            graphSMEquipmentMaint.EquipmentRecords.Current.Status = ID.Equipment_Status.DISPOSED;
                            graphSMEquipmentMaint.EquipmentRecords.Current.DisposalDate = soLineRow.OrderDate != null ? soLineRow.OrderDate : arTranRow.TranDate;
                            graphSMEquipmentMaint.EquipmentRecords.Current.DispSrvOrdType = fsxARTranRow.SrvOrdType;
                            graphSMEquipmentMaint.EquipmentRecords.Current.DispServiceOrderRefNbr = fsxARTranRow.ServiceOrderRefNbr;
                            graphSMEquipmentMaint.EquipmentRecords.Current.DispAppointmentRefNbr = fsxARTranRow.AppointmentRefNbr;
                            graphSMEquipmentMaint.EquipmentRecords.Cache.SetStatus(graphSMEquipmentMaint.EquipmentRecords.Current, PXEntryStatus.Updated);
                            graphSMEquipmentMaint.Save.Press();
                        }
                    }
                }
            }

            if (needPersist==true) 
            {
                arTranCache.Persist(PXDBOperation.Update);
            }
        }

        public virtual List<ItemInfo> GetDifferentItemList(PXGraph graph, ARTran arTran, bool createDifferentEntriesForQtyGreaterThan1)
        {
            return SharedFunctions.GetDifferentItemList(graph, arTran, createDifferentEntriesForQtyGreaterThan1);
        }

        public virtual List<ItemInfo> GetVerifiedDifferentItemList(PXGraph graph, ARTran arTran, List<ItemInfo> lotSerialList)
        {
			return SharedFunctions.GetVerifiedDifferentItemList(graph, arTran, lotSerialList);
        }

		/// <summary>
		/// Workaround method to obtain serial numbers
		/// </summary>
		/// <param name="soLineRow"></param>
		/// <param name="arTran"></param>
		/// <returns></returns>
		private List<ItemInfo> GetItemListFromPOReceipt(SOLine soLineRow)
		{
			var receiptLines = POReceiptLines.Select(soLineRow.OrderType, soLineRow.OrderNbr).ToList();

			return receiptLines.Select(r => new ItemInfo(r)).ToList();

		}

		protected virtual PXResult<InventoryItem, INLotSerClass> ReadInventoryItem(PXCache sender, int? inventoryID)
        {
            if (inventoryID == null)
                return null;
            var inventory = InventoryItem.PK.Find(sender.Graph, inventoryID);
            if (inventory == null)
                throw new PXException(ErrorMessages.ValueDoesntExistOrNoRights, IN.Messages.InventoryItem, inventoryID);
            INLotSerClass lotSerClass;
            if (inventory.StkItem == true)
            {
                lotSerClass = INLotSerClass.PK.Find(sender.Graph, inventory.LotSerClassID);
                if (lotSerClass == null)
                    throw new PXException(ErrorMessages.ValueDoesntExistOrNoRights, IN.Messages.LotSerClass, inventory.LotSerClassID);
            }
            else
            {
                lotSerClass = new INLotSerClass();
            }
            return new PXResult<InventoryItem, INLotSerClass>(inventory, lotSerClass);
        }
        #endregion

        #region Validations
        public virtual void ValidatePostBatchStatus(PXDBOperation dbOperation, string postTo, string createdDocType, string createdRefNbr)
        {
            DocGenerationHelper.ValidatePostBatchStatus<ARRegister>(Base, dbOperation, postTo, createdDocType, createdRefNbr);
        }
        #endregion
    }
}
