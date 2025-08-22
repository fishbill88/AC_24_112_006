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
using PX.Objects.FS.DAC;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.FS
{
    public class ConvertItemsToEquipmentProcess : PXGraph<ConvertItemsToEquipmentProcess>
    {
        public ConvertItemsToEquipmentProcess()
        {
            SMEquipmentMaint graphSMEquipmentMaint;

			// Acuminator disable once PX1008 LongOperationDelegateSynchronousExecution [compatibility with legacy code]
			InventoryItems.SetProcessDelegate(
                delegate(List<SoldInventoryItem> inventoryItemRows)
                {
					graphSMEquipmentMaint = CreateInstance<SMEquipmentMaint>();

					bool error = false;

					var groupedCustomerList = inventoryItemRows
										.Select((x, i) => new { Value = x, Index = i })
										.GroupBy(row => new { row.Value.DocType, row.Value.InvoiceRefNbr, row.Value.InvoiceLineNbr })
										.Select(grp => grp.ToList())
										.ToList();

					for (int i = 0; i < groupedCustomerList.Count; i++)
					{
						SoldInventoryItem soldInventoryItemRow = groupedCustomerList[i].First().Value;
						error = false;

						try
						{
							ARTran arTranRow = ARTran.PK.Find(graphSMEquipmentMaint, soldInventoryItemRow.DocType, soldInventoryItemRow.InvoiceRefNbr, soldInventoryItemRow.InvoiceLineNbr);

							InventoryItem inventoryItemRow = SharedFunctions.GetInventoryItemRow(graphSMEquipmentMaint, soldInventoryItemRow.InventoryID);

							foreach (SM_ARReleaseProcess.ItemInfo itemInfo in GetDifferentItemList(graphSMEquipmentMaint, arTranRow, true))
							{
								SoldInventoryItem newSoldInventoryItemRow = (SoldInventoryItem)InventoryItems.Cache.CreateCopy(soldInventoryItemRow);

								newSoldInventoryItemRow.LotSerialNumber = itemInfo.LotSerialNbr;

								SharedFunctions.CreateSoldEquipment(graphSMEquipmentMaint, newSoldInventoryItemRow, null, null, null, null, inventoryItemRow);
							}
						}
						catch (Exception e)
						{
							error = true;

							foreach (var row in groupedCustomerList[i])
							{
								PXProcessing<SoldInventoryItem>.SetError(row.Index, e.Message);
							}
						}

						if (error == false)
						{
							foreach (var row in groupedCustomerList[i])
							{
								PXProcessing<SoldInventoryItem>.SetInfo(row.Index, TX.Messages.RECORD_PROCESSED_SUCCESSFULLY);
							}
						}
					}
				});
        }

        #region DACFilter
        [Serializable]
        public partial class StockItemsFilter : PXBqlTable, IBqlTable
        {
            #region ItemClassID
            public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }

            [PXInt]
            [PXUIField(DisplayName = "Item Class ID")]
            [PXSelector(typeof(
                Search<INItemClass.itemClassID,
                Where<
                    FSxEquipmentModelTemplate.eQEnabled, Equal<True>>>), SubstituteKey = typeof(INItemClass.itemClassCD))]
            public virtual int? ItemClassID { get; set; }
            #endregion
            #region Date
            public abstract class date : PX.Data.BQL.BqlDateTime.Field<date> { }

            [PXDBDate]
            [PXUIField(DisplayName = "Sold After")]
            public virtual DateTime? Date { get; set; }
            #endregion
        }
        #endregion

        #region Select
        [PXHidden]
        public PXFilter<StockItemsFilter> Filter;
        public PXCancel<StockItemsFilter> Cancel;

        [PXFilterable]
        public
            PXFilteredProcessingJoin<SoldInventoryItem, StockItemsFilter,
                InnerJoinSingleTable<Customer,
                    On<Customer.bAccountID, Equal<SoldInventoryItem.customerID>,
                    And<Match<Customer, Current<AccessInfo.userName>>>>>,
            Where2<
                Where<
                    CurrentValue<StockItemsFilter.itemClassID>, IsNull,
                    Or<SoldInventoryItem.itemClassID, Equal<CurrentValue<StockItemsFilter.itemClassID>>>>,
                And<
                    Where<CurrentValue<StockItemsFilter.date>, IsNull,
                    Or<SoldInventoryItem.docDate, GreaterEqual<CurrentValue<StockItemsFilter.date>>>>>>,
            OrderBy<
                Asc<SoldInventoryItem.inventoryCD,
                Asc<SoldInventoryItem.invoiceRefNbr,
                Asc<SoldInventoryItem.invoiceLineNbr>>>>> InventoryItems;
        #endregion

        #region Actions
        #region OpenInvoice
        public PXAction<StockItemsFilter> openInvoice;
        [PXUIField(MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual void OpenInvoice()
        {
            SOInvoiceEntry graphSOInvoiceEntry = PXGraph.CreateInstance<SOInvoiceEntry>();

            if (InventoryItems.Current != null
                    && InventoryItems.Current.InvoiceRefNbr != null)
            {
                graphSOInvoiceEntry.Document.Current = graphSOInvoiceEntry.Document.Search<ARInvoice.refNbr>(InventoryItems.Current.InvoiceRefNbr, InventoryItems.Current.DocType);
                throw new PXRedirectRequiredException(graphSOInvoiceEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
        }
        #endregion
        #endregion

        #region Events
        protected virtual void _(Events.RowSelected<SoldInventoryItem> e)
        {
            if (e.Row == null)
            {
                return;
            }

            SoldInventoryItem inventoryItemRow = (SoldInventoryItem)e.Row;

            int components = PXSelect<FSModelComponent,
                             Where<
                                 FSModelComponent.modelID, Equal<Required<FSModelComponent.modelID>>>>
                             .SelectWindowed(this, 0, 1, inventoryItemRow.InventoryID).Count;

            if (components == 0)
            {
                e.Cache.RaiseExceptionHandling<SoldInventoryItem.inventoryCD>(inventoryItemRow,
                                                                              inventoryItemRow.InventoryCD,
                                                                              new PXSetPropertyException(TX.Warning.ITEM_WITH_NO_WARRANTIES_CONFIGURED,
                                                                                                         PXErrorLevel.RowWarning));
            }
        }
		#endregion

		#region Methods
		public virtual List<SM_ARReleaseProcess.ItemInfo> GetDifferentItemList(PXGraph graph, ARTran arTran, bool createDifferentEntriesForQtyGreaterThan1)
		{
			return SharedFunctions.GetDifferentItemList(graph, arTran, createDifferentEntriesForQtyGreaterThan1);
		}
		#endregion
	}
}
