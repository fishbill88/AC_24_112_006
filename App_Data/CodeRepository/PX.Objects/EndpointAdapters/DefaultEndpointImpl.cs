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
using System.Collections.Generic;
using System.Linq;

using PX.Api;
using PX.Api.ContractBased;
using PX.Api.ContractBased.Models;
using PX.Common;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CS;
using PX.Objects.EndpointAdapters;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.PO.DAC.Projections;
using PX.Objects.SO;

using Location = PX.Objects.CR.Standalone.Location;
using PX.Objects.SO.DAC.Projections;
using PX.Objects.SO.GraphExtensions.SOOrderEntryExt;

namespace PX.Objects
{
	[PXInternalUseOnly]
	public abstract class DefaultEndpointImpl
	{
		[FieldsProcessed(new[] { "DetailID", "Value" })]
		protected void BusinessAccountPaymentInstructionDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			IEnumerable<EntityValueField> valueFields = targetEntity.Fields.OfType<EntityValueField>();
			string targetID = valueFields.Single(f => f.Name.EndsWith("ID")).Value;
			string targetValue = valueFields.Single(f => f.Name.EndsWith("Value")).Value;

			switch (graph)
			{
				case VendorMaint vendorGraph:
					{
						var locationDetails = vendorGraph.GetExtension<VendorMaint.DefLocationExt>();
						locationDetails.DefLocation.View.Clear();
						Location location = locationDetails.DefLocation.Current = locationDetails.DefLocation.SelectSingle();

						var paymentDetails = vendorGraph.GetExtension<VendorMaint.PaymentDetailsExt>();
						paymentDetails.FillPaymentDetails(location);
						foreach (VendorPaymentMethodDetail paymentMethodDetail in paymentDetails.PaymentDetails.Select(location.BAccountID, location.LocationID, location.VPaymentMethodID))
							if (paymentMethodDetail.DetailID == targetID)
							{
								paymentDetails.PaymentDetails.Cache.SetValueExt<VendorPaymentMethodDetail.detailValue>(paymentMethodDetail, targetValue);
								paymentDetails.PaymentDetails.Cache.Update(paymentMethodDetail);
								return;
							}
						throw new PXException(Common.Messages.EntityWithIDDoesNotExist, Data.EntityHelper.GetFriendlyEntityName<VendorPaymentMethodDetail>(), targetID);
					}
				case EmployeeMaint employeeGraph:
					{
						CR.Location location = employeeGraph.DefLocation.Select();
						foreach (VendorPaymentMethodDetail paymentMethodDetail in employeeGraph.PaymentDetails.Select(location.BAccountID, location.LocationID, location.VPaymentMethodID))
							if (paymentMethodDetail.DetailID == targetID)
							{
								employeeGraph.PaymentDetails.Cache.SetValueExt<VendorPaymentMethodDetail.detailValue>(paymentMethodDetail, targetValue);
								employeeGraph.PaymentDetails.Cache.Update(paymentMethodDetail);
								return;
							}
						throw new PXException(Common.Messages.EntityWithIDDoesNotExist, Data.EntityHelper.GetFriendlyEntityName<VendorPaymentMethodDetail>(), targetID);
					}
				case CustomerMaint customerGraph:
					{
						var locationDetails = customerGraph.GetExtension<CustomerMaint.DefLocationExt>();
						Location location = locationDetails.DefLocation.Current = locationDetails.DefLocation.SelectSingle();

						var paymentDetails = customerGraph.GetExtension<CustomerMaint.PaymentDetailsExt>();
						foreach (CustomerPaymentMethodDetail paymentMethodDetail in paymentDetails.DefPaymentMethodInstanceDetails.Select(location.BAccountID, location.LocationID, location.VPaymentMethodID))
							if (paymentMethodDetail.DetailID == targetID)
							{
								paymentDetails.DefPaymentMethodInstanceDetails.Cache.SetValueExt<CustomerPaymentMethodDetail.value>(paymentMethodDetail, targetValue);
								paymentDetails.DefPaymentMethodInstanceDetails.Cache.Update(paymentMethodDetail);
								return;
							}
						throw new PXException(Common.Messages.EntityWithIDDoesNotExist, Data.EntityHelper.GetFriendlyEntityName<CustomerPaymentMethodDetail>(), targetID);
					}
				default: throw new InvalidOperationException("Not applicable for " + graph.GetType());
			}
		}

		[FieldsProcessed(new[] {
			"OrderType",
			"OrderNbr",
			"OrderLineNbr",
			"InventoryID",
			"LotSerialNbr",
			"ShippedQty"
		})]
		protected virtual void ShipmentDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var shipmentEntry = (SOShipmentEntry)graph;

			var filterCache = shipmentEntry.addsofilter.Cache;
			var filter = (AddSOFilter)filterCache.Current;

			var orderLineNbrField = targetEntity.Fields.SingleOrDefault(f => f.Name == "OrderLineNbr") as EntityValueField;
			var shippedQtyField = targetEntity.Fields.SingleOrDefault(f => f.Name == "ShippedQty") as EntityValueField;

			string orderType = (targetEntity.Fields.SingleOrDefault(f => f.Name == "OrderType") as EntityValueField)?.Value;
			string orderNbr = (targetEntity.Fields.SingleOrDefault(f => f.Name == "OrderNbr") as EntityValueField)?.Value;
			int? orderLineNbr = !string.IsNullOrEmpty(orderLineNbrField?.Value) ? (int?)int.Parse(orderLineNbrField.Value) : null;
			string inventoryId = (targetEntity.Fields.SingleOrDefault(f => f.Name == "InventoryID") as EntityValueField)?.Value;
			string lotSerialNbr = (targetEntity.Fields.SingleOrDefault(f => f.Name == "LotSerialNbr") as EntityValueField)?.Value;
			decimal? shippedQty = !string.IsNullOrEmpty(shippedQtyField?.Value) ? (decimal?)decimal.Parse(shippedQtyField.Value) : null;

			string createNewShipmentForEveryOrder = (entity.Fields.SingleOrDefault(f => f.Name == "CreateNewShipmentForEveryOrder") as EntityValueField)?.Value;
			bool insertOrdersSeparately = false;
			if (createNewShipmentForEveryOrder != null)
				Boolean.TryParse(createNewShipmentForEveryOrder, out insertOrdersSeparately);

			//setting Add Sales Order filter parameters
			string oldOrderNbr = filter.OrderNbr;
			filter.OrderType = orderType;
			filter.OrderNbr = orderNbr;
			filterCache.RaiseFieldUpdated<AddSOFilter.orderNbr>(filter, oldOrderNbr);
			filterCache.Update(filter);

			//forming selection criteria
			Func<PXResult<SOShipmentPlan>, bool> lineCriteria;
			SOShipmentPlan item = new SOShipmentPlan();
			if (orderLineNbr != null && orderLineNbr >= 0)
			{
				filter.OrderLineNbr = orderLineNbr;
				item = FindSalesOrderLine(shipmentEntry, _ => true);
			}
			else if (!string.IsNullOrWhiteSpace(inventoryId))
			{
				var inventoryItem = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>.Select(graph, inventoryId)?.FirstOrDefault()?.GetItem<InventoryItem>();
				if (inventoryItem == null)
				{
					throw new PXException("Inventory Item '{0}' was not found.", inventoryId);
				}
				lineCriteria = r => r.GetItem<SOLineSplit>().InventoryID == inventoryItem.InventoryID;

				if (!string.IsNullOrWhiteSpace(lotSerialNbr))
				{
					lineCriteria = PX.Common.Func.Conjoin(lineCriteria, t => string.Equals(t.GetItem<SOLineSplit>().LotSerialNbr, lotSerialNbr, StringComparison.OrdinalIgnoreCase));
				}
				if (shippedQty != null && shippedQty > 0)
				{
					lineCriteria = PX.Common.Func.Conjoin(lineCriteria, t => t.GetItem<SOLineSplit>().OpenQty >= shippedQty);
				}

				item = FindSalesOrderLine(shipmentEntry, lineCriteria);
			}

			bool singleOrderLineAdded = false;
			if (item.InventoryID != null)
			{
				SelectLine(shipmentEntry, item);
				singleOrderLineAdded = true;
				shipmentEntry.addSO.Press();
			}
			else
			{
				if (insertOrdersSeparately)
				{
					filter.AddAllLines = true;
					shipmentEntry.addSO.Press();
					var shipLine = shipmentEntry.Caches[typeof(SOShipLine)].Current as SOShipLine;
					if (shipLine != null)
					{
						SOShipment origShipmentHeader = (SOShipment)shipmentEntry.Document.Cache.CreateCopy(shipmentEntry.Document.Current);
						shipmentEntry.Document.Current.ShipmentNbr = shipLine.ShipmentNbr;
					}
				}
				else
				{
					foreach (SOShipmentPlan row in shipmentEntry.soshipmentplan.Select())
					{
						SelectLine(shipmentEntry, row);
					}
					shipmentEntry.addSO.Press();
				}
			}

			var shipLineCurrent = shipmentEntry.Caches[typeof(SOShipLine)].Current as SOShipLine;
			if (shipLineCurrent == null)
				throw new InvalidOperationException(SO.Messages.CantAddShipmentDetail);

			var allocations = (targetEntity.Fields.SingleOrDefault(f => string.Equals(f.Name, "Allocations")) as EntityListField)?.Value ?? new EntityImpl[0];
			if (allocations.Any(a => a.Fields != null && a.Fields.Length > 0))
			{
				if (!singleOrderLineAdded)
					throw new InvalidOperationException("Allocations can be specified for unambiguously selected Sales Order line only.");

				// Clear auto-allocated splits in the current line, to replace them with ones specified in allocations.
				PXCache splitsCache = shipmentEntry.splits.Cache;
				foreach (SOShipLineSplit split in splitsCache.Inserted)
				{
					if (splitsCache.GetStatus(split) != PXEntryStatus.Inserted
						|| shipLineCurrent.ShipmentNbr != split.ShipmentNbr
						|| shipLineCurrent.LineNbr != split.LineNbr)
						continue;

					shipmentEntry.splits.Delete(split);
				}

				shipLineCurrent = (SOShipLine)shipmentEntry.Caches[typeof(SOShipLine)].CreateCopy(shipLineCurrent);
				shipLineCurrent.ShippedQty = 0m;
				shipmentEntry.Caches[typeof(SOShipLine)].Update(shipLineCurrent);
			}
			else if (shippedQty != null)
			{
				shipLineCurrent.ShippedQty = shippedQty;
				shipmentEntry.Caches[typeof(SOShipLine)].Update(shipLineCurrent);
			}
		}

		protected static SOShipmentPlan FindSalesOrderLine(SOShipmentEntry shipmentEntry, Func<PXResult<SOShipmentPlan>, bool> lineCriteria)
		{
			SOShipmentPlan item;
			try
			{
				item = shipmentEntry.soshipmentplan.Select().First(lineCriteria);
			}
			catch (InvalidOperationException ex)
			{
				throw new InvalidOperationException(SO.Messages.CannotSelectSpecificSOLine, ex);
			}

			return item;
		}

		protected void SelectLine(SOShipmentEntry shipmentEntry, SOShipmentPlan item)
		{
			item.Selected = true;
			item = shipmentEntry.soshipmentplan.Update(item);
			shipmentEntry.soshipmentplan.Cache.SetStatus(item, PXEntryStatus.Notchanged);
			AssertNoErrors(shipmentEntry.soshipmentplan.Cache, item);
		}

		protected void AssertNoErrors(PXCache cache, object current)
		{
			var errors = PXUIFieldAttribute.GetErrors(cache, current);
			if (errors.Count == 0)
				return;

			throw new InvalidOperationException(string.Join("\n", errors.Select(p => p.Key + ": " + p.Value)));
		}

		[FieldsProcessed(new[] {
			"ShipmentNbr",
			"OrderNbr",
			"OrderType"
		})]
		protected virtual void SalesInvoiceDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var maint = (SOInvoiceEntry)graph;

			var shipmentNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "ShipmentNbr") as EntityValueField;
			var orderNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "OrderNbr") as EntityValueField;
			var orderType = targetEntity.Fields.SingleOrDefault(f => f.Name == "OrderType") as EntityValueField;
			var calculateAutomaticDiscounts = targetEntity.Fields.SingleOrDefault(f => f.Name == "CalculateDiscountsOnImport") as EntityValueField;

			var shipment = shipmentNbr != null ? shipmentNbr.Value : null;
			var number = orderNbr != null ? orderNbr.Value : null;
			var type = orderType != null ? orderType.Value : null;
			bool? calculateDiscounts = ConvertToNullableBool(calculateAutomaticDiscounts);

			if (shipment != null && number != null && type != null)
			{
				var shipments = maint.shipmentlist.Select().AsEnumerable().Select(s => s.GetItem<SOOrderShipment>())
				.Where(s => (shipment == null || s.ShipmentNbr.OrdinalEquals(shipment))
							&& (number == null || s.OrderNbr.OrdinalEquals(number))
							&& (type == null || s.OrderType.OrdinalEquals(type)));

				if (!shipments.Any())
				{
					throw new PXException(SO.Messages.ShipmentsNotFound);
				}

				foreach (var item in shipments)
				{
					foreach (SOOrderShipment orderShipment in maint.shipmentlist.Select())
					{
						orderShipment.Selected = false;
					}

					item.Selected = true;
					maint.shipmentlist.Update(item);
					maint.Actions["AddShipment"].Press();
				}
			}
			else if (shipment == null)
			{
				var detailsCache = maint.Transactions.Cache;

				ARTran row = (ARTran)detailsCache.CreateInstance();
				row.SOShipmentNbr = shipment;
				row.SOOrderType = type;
				row.SOOrderNbr = number;
				row.CalculateDiscountsOnImport = calculateDiscounts;

				FillInvoiceRowFromEntiry(maint, targetEntity, row);
				maint.InsertInvoiceDirectLine(row);
			}
		}

		[FieldsProcessed(new[] {
			"Value",
			"Active",
			"SegmentID"
		})]
		protected void SubItemStockItem_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var subitemNameField = targetEntity.Fields.Single(f => f.Name == "Value") as EntityValueField;
			var activeField = targetEntity.Fields.Single(f => f.Name == "Active") as EntityValueField;
			var segmentIDField = targetEntity.Fields.Single(f => f.Name == "SegmentID") as EntityValueField;

			var view = graph.Views["SubItem_" + segmentIDField.Value];
			var cache = view.Cache;

			foreach (INSubItemSegmentValueList.SValue row in view.SelectMulti(segmentIDField.Value))
			{
				if (row.Value == subitemNameField.Value)
				{
					if (activeField.Value == "true")
					{
						row.Active = true;
						cache.Update(row);
					}
					else
						cache.Delete(row);
				}
			}
		}

		[FieldsProcessed(new[] {
			"Value",
			"Active",
			"SegmentID"
		})]
		protected void SubItemStockItem_Update(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var subitemNameField = targetEntity.Fields.Single(f => f.Name == "Value") as EntityValueField;
			var activeField = targetEntity.Fields.Single(f => f.Name == "Active") as EntityValueField;
			var segmentIDField = targetEntity.Fields.Single(f => f.Name == "SegmentID") as EntityValueField;

			var view = graph.Views["SubItem_" + segmentIDField.Value];
			var cache = view.Cache;

			foreach (INSubItemSegmentValueList.SValue row in view.SelectMulti(segmentIDField.Value))
			{
				if (row.Value == subitemNameField.Value)
				{
					if (activeField.Value == "true")
					{
						row.Active = true;
						cache.Update(row);
					}
					else
						cache.Delete(row);
				}
			}
		}

		[FieldsProcessed(new[] {
			"WarehouseID"
		})]
		protected void StockItemWarehouseDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var warehouseIDField = targetEntity.Fields.FirstOrDefault(f => f.Name == "WarehouseID") as EntityValueField;
			var defaultIssueLocationIDField = targetEntity.Fields.FirstOrDefault(f => f.Name == "DefaultIssueLocationID") as EntityValueField;
			var defaultReceiptLocationIDField = targetEntity.Fields.FirstOrDefault(f => f.Name == "DefaultReceiptLocationID") as EntityValueField;

			var view = graph.Views["itemsiterecords"];
			var cache = view.Cache;

			var site = (INSite)PXSelect<INSite, Where<INSite.siteCD, Equal<Required<INSite.siteCD>>>>.SelectSingleBound(graph, null, new object[] { warehouseIDField.Value });
			if (site == null)
			{
				throw new PXException("Site '{0}' is missing.", warehouseIDField.Value);
			}

			var rows = view.SelectMulti().Cast<PXResult<INItemSite, INSite, INSiteStatusSummary>>().ToArray();
			foreach (INItemSite row in rows)
			{
				if (row.SiteID == site.SiteID)
					return;
			}

			var itemsite = (INItemSite)cache.CreateInstance();

			var stockItemCache = graph.Caches[typeof(InventoryItem)];
			if (stockItemCache?.Current != null)
				itemsite.InventoryID = ((InventoryItem)stockItemCache.Current).InventoryID;
			itemsite.SiteID = site.SiteID;

			if (defaultIssueLocationIDField != null)
				cache.SetValueExt(itemsite, nameof(INItemSite.DfltShipLocationID), defaultIssueLocationIDField.Value);
			if (defaultReceiptLocationIDField != null)
				cache.SetValueExt(itemsite, nameof(INItemSite.DfltReceiptLocationID), defaultReceiptLocationIDField.Value);

			cache.Insert(itemsite);
		}

		[FieldsProcessed(new[] {
			"InventoryID",
			"Location",
			"LotSerialNbr",
			"Subitem"
		})]
		protected void PhysicalInventoryCountDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var InventoryIDField = targetEntity.Fields.Single(f => f.Name == "InventoryID") as EntityValueField;
			var LocationField = targetEntity.Fields.Single(f => f.Name == "Location") as EntityValueField;
			var LotSerialNumberField = targetEntity.Fields.FirstOrDefault(f => f.Name == "LotSerialNbr") as EntityValueField;
			var SubItemField = targetEntity.Fields.FirstOrDefault(f => f.Name == "Subitem") as EntityValueField;

			var maint = (INPICountEntry)graph;

			var view = maint.AddByBarCode;
			var cache = view.Cache;


			cache.Remove(view.Current);
			cache.Insert(new INBarCodeItem());
			var bci = (INBarCodeItem)cache.Current;

			cache.SetValueExt(bci, "InventoryID", InventoryIDField.Value);
			cache.SetValueExt(bci, "LocationID", LocationField.Value);
			if (LotSerialNumberField != null)
				cache.SetValueExt(bci, "LotSerialNbr", LotSerialNumberField.Value);
			if (SubItemField != null)
				cache.SetValueExt(bci, "SubItemID", SubItemField.Value);

			cache.Update(bci);

			maint.Actions["AddLine2"].Press();
		}

		private static EntityValueField GetEntityField(EntityImpl targetEntity, string fieldName)
		{
			return targetEntity.Fields.SingleOrDefault(f => f.Name == fieldName) as EntityValueField;
		}

		protected static decimal? ConvertToNullableDecimal(EntityValueField field)
		{
			return field != null ? (decimal?)Convert.ToDecimal(field.Value) : null;
		}

		protected static int? ConvertToNullableInt(EntityValueField field)
		{
			return field != null ? (int?)Convert.ToInt32(field.Value) : null;
		}

		protected static bool? ConvertToNullableBool(EntityValueField field)
		{
			return field != null ? (bool?)Convert.ToBoolean(field.Value) : null;
		}

		private static void FillInvoiceRowFromEntiry(SOInvoiceEntry graph, EntityImpl targetEntity, ARTran row)
		{
			row.TranDesc = GetEntityField(targetEntity, "TransactionDescr")?.Value;
			row.UnitPrice = ConvertToNullableDecimal(GetEntityField(targetEntity, "UnitPrice"));
			row.LineNbr = ConvertToNullableInt(GetEntityField(targetEntity, "LineNbr"));
			row.Qty = ConvertToNullableDecimal(GetEntityField(targetEntity, "Qty"));
			row.CuryTranAmt = ConvertToNullableDecimal(GetEntityField(targetEntity, "Amount"));
			row.UOM = GetEntityField(targetEntity, "UOM")?.Value;
			row.DiscAmt = ConvertToNullableDecimal(GetEntityField(targetEntity, "DiscountAmount"));
			row.DiscPct = ConvertToNullableDecimal(GetEntityField(targetEntity, "DiscountPercent"));
			row.LotSerialNbr = GetEntityField(targetEntity, "LotSerialNbr")?.Value;
			row.SOOrderLineNbr = ConvertToNullableInt(GetEntityField(targetEntity, "OrderLineNbr"));
			row.TaxCategoryID = GetEntityField(targetEntity, "TaxCategory")?.Value;
			row.OrigInvoiceType = GetEntityField(targetEntity, "OrigInvType")?.Value;
			row.OrigInvoiceNbr = GetEntityField(targetEntity, "OrigInvNbr")?.Value;
			row.OrigInvoiceLineNbr = ConvertToNullableInt(GetEntityField(targetEntity, "OrigInvLineNbr"));

			string inventoryID = GetEntityField(targetEntity, "InventoryID")?.Value;
			string branchID = GetEntityField(targetEntity, "BranchID")?.Value;
			EntityValueField fieldExpirationDate = GetEntityField(targetEntity, "ExpirationDate");
			DateTime expirationDate = fieldExpirationDate != null ? Convert.ToDateTime(fieldExpirationDate.Value) : default(DateTime);
			string location = GetEntityField(targetEntity, "Location")?.Value;
			string warehouseID = GetEntityField(targetEntity, "WarehouseID")?.Value;

			if (inventoryID != null)
			{
				row.InventoryID = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>.Select(graph, inventoryID).FirstOrDefault()?.GetItem<InventoryItem>().InventoryID;
			}
			if (branchID != null)
			{
				row.BranchID = PXSelect<GL.Branch, Where<GL.Branch.branchCD, Equal<Required<GL.Branch.branchCD>>>>.Select(graph, branchID).FirstOrDefault()?.GetItem<GL.Branch>().BranchID;
			}
			if (expirationDate != default(DateTime))
			{
				row.ExpireDate = expirationDate;
			}
			if (warehouseID != null)
			{
				row.SiteID = PXSelect<INSite, Where<INSite.siteCD, Equal<Required<INSite.siteCD>>>>.Select(graph, warehouseID).FirstOrDefault()?.GetItem<INSite>().SiteID;
				if (location != null)
				{
					row.LocationID = PXSelect<INLocation, Where<INLocation.siteID, Equal<Required<INLocation.siteID>>, And<INLocation.locationCD, Equal<Required<INLocation.locationCD>>>>>.Select(graph, row.SiteID, location).FirstOrDefault()?.GetItem<INLocation>().LocationID;
				}
			}
		}

		private protected void AttributeBase_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity, string attributeIdFieldName)
		{
			var attributeIdField = targetEntity.Fields.Single(f => f.Name == attributeIdFieldName) as EntityValueField;
			var valueField = targetEntity.Fields.Single(f => f.Name == "Value") as EntityValueField;

			var view = graph.Views[CS.Messages.CSAnswers];
			var cache = view.Cache;

			var rows = view.SelectMulti().OrderBy(row =>
			{
				var orderState = cache.GetStateExt(row, "Order") as PXFieldState;
				return orderState.Value;
			}).ToArray();

			foreach (var row in rows)
			{
				var attributeId = (cache.GetStateExt(row, "AttributeID") as PXFieldState).Value.ToString();
				if (attributeIdField.Value.OrdinalEquals(attributeId))
				{
					if (cache.GetStateExt<CSAnswers.value>(row) is PXStringState state)
					{
						if (state.Enabled is false)
						{
							continue;
						}

						if (state.ValueLabelDic != null)
						{
							foreach (var rec in state.ValueLabelDic)
							{
								if (rec.Value == valueField.Value)
								{
									valueField.Value = rec.Key;
									break;
								}
							}
						}
					}

					cache.SetValueExt(row, "Value", valueField.Value);
					cache.Update(row);
					break;
				}
			}
		}

		[FieldsProcessed(new[] {
			"POLineNbr",
			"POOrderType",
			"POOrderNbr",
			"TransferOrderType",
			"TransferOrderNbr",
			"TransferShipmentNbr"
		})]
		protected virtual void PurchaseReceiptDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var receiptEntry = (POReceiptEntry)graph;

			if (receiptEntry.Document.Current != null)
			{
				var detailsCache = receiptEntry.transactions.Cache;

				var allocations = (targetEntity.Fields.SingleOrDefault(f => string.Equals(f.Name, "Allocations")) as EntityListField)?.Value ?? new EntityImpl[0];
				bool hasAllocations = allocations.Any(a => a.Fields != null && a.Fields.Length > 0);

				if (receiptEntry.Document.Current.ReceiptType == POReceiptType.TransferReceipt)
				{
					var sOOrderType = targetEntity.Fields.SingleOrDefault(f => f.Name == "TransferOrderType") as EntityValueField;
					var sOOrderNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "TransferOrderNbr") as EntityValueField;
					var sOOrderLineNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "TransferOrderLineNbr") as EntityValueField;
					var sOShipmentNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "TransferShipmentNbr") as EntityValueField;

					if (sOOrderType != null && sOOrderNbr != null && sOShipmentNbr != null)
					{
						if (sOOrderLineNbr != null)
						{
							receiptEntry.filter.Cache.Remove(receiptEntry.filter.Current);
							receiptEntry.filter.Cache.Insert(new POReceiptEntry.POOrderFilter());
							var filter = receiptEntry.filter.Current;

							receiptEntry.filter.Cache.SetValueExt(filter, "SOOrderNbr", sOOrderNbr.Value);
							filter = receiptEntry.filter.Update(filter);

							Dictionary<string, string> filterErrors = PXUIFieldAttribute.GetErrors(receiptEntry.filter.Cache, filter);

							if (filterErrors.Count() > 0)
							{
								throw new PXException(string.Join(";", filterErrors.Select(x => x.Key + "=" + x.Value)));
							}

							INTran intran = null;
							foreach (INTran tran in receiptEntry.intranSelection.Select())
							{
								if (tran.SOOrderType == sOOrderType.Value && tran.SOOrderNbr == sOOrderNbr.Value && tran.SOOrderLineNbr.ToString() == sOOrderLineNbr.Value && tran.SOShipmentNbr == sOShipmentNbr.Value)
								{
									intran = tran;
									break;
								}
							}

							if (intran != null)
							{
								POReceiptLine line = PXSelect<POReceiptLine,
											Where<POReceiptLine.receiptType, Equal<Required<POReceipt.receiptType>>,
												And<POReceiptLine.receiptNbr, Equal<Required<POReceipt.receiptNbr>>,
												And<POReceiptLine.origDocType, Equal<Required<POReceiptLine.origDocType>>,
												And<POReceiptLine.origRefNbr, Equal<Required<POReceiptLine.origRefNbr>>,
												And<POReceiptLine.origLineNbr, Equal<Required<POReceiptLine.origLineNbr>>>>>>>>
												.Select(receiptEntry, receiptEntry.Document.Current.ReceiptType, receiptEntry.Document.Current.ReceiptNbr, intran.DocType, intran.RefNbr, intran.LineNbr); ;

								if (line == null)
								{
									line = receiptEntry.AddTransferLine(intran);
								}
							}
							else
							{
								throw new PXException(PO.Messages.TransferOrderLineNotFound);
							}
						}
						else
						{
							receiptEntry.filter.Cache.Remove(receiptEntry.filter.Current);
							receiptEntry.filter.Cache.Insert(new POReceiptEntry.POOrderFilter());

							var orders = receiptEntry.openTransfers.Select().Select(r => r.GetItem<SOOrderShipment>());
							var order = orders.FirstOrDefault(o => o.OrderType == sOOrderType.Value && o.OrderNbr == sOOrderNbr.Value && o.ShipmentNbr == sOShipmentNbr.Value);
							if (order == null)
							{
								throw new PXException(PO.Messages.TransferOrderNotFound);
							}

							order.Selected = true;
							receiptEntry.openTransfers.Update(order);
							receiptEntry.Actions["AddTransfer2"].Press();

							return;
						}
					}
				}
				else
				{
					var lineNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "POLineNbr") as EntityValueField;
					var orderType = targetEntity.Fields.SingleOrDefault(f => f.Name == "POOrderType") as EntityValueField;
					var orderNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "POOrderNbr") as EntityValueField;

					var unitCost = targetEntity.Fields.SingleOrDefault(f => f.Name == "UnitCost") as EntityValueField;
					var extCost = targetEntity.Fields.SingleOrDefault(f => f.Name == "ExtendedCost") as EntityValueField;

					bool insertViaAddPOLine = lineNbr != null && orderNbr != null && orderType != null;
					bool insertViaAddPO = orderNbr != null && orderType != null;
					bool setManualPrice = unitCost != null || extCost != null;

					if (!insertViaAddPO && (orderType != null || orderNbr != null))
					{
						throw new PXException(PO.Messages.POTypeNbrMustBeFilled);
					}

					if (insertViaAddPOLine)
					{
						FillInAddPOFilter(receiptEntry, orderType, orderNbr);

						var orders = receiptEntry.poLinesSelection.Select().Select(r => r.GetItem<POReceiptEntry.POLineS>());
						var order = orders.FirstOrDefault(o => o.LineNbr == int.Parse(lineNbr.Value));
						if (order == null)
						{
							throw new PXException(PO.Messages.PurchaseOrderLineNotFound);
						}
						order.Selected = true;
						receiptEntry.poLinesSelection.Update(order);
						receiptEntry.Actions["AddPOOrderLine2"].Press();

						if (setManualPrice && detailsCache.Current != null)
							detailsCache.SetValueExt<POReceiptLine.manualPrice>(detailsCache.Current, true);
					}
					else if (insertViaAddPO)
					{
						FillInAddPOFilter(receiptEntry, orderType, orderNbr);

						var orders = receiptEntry.openOrders.Select().Select(r => r.GetItem<POReceiptEntry.POOrderS>());
						var order = orders.FirstOrDefault(o => o.OrderNbr == orderNbr.Value);
						if (order == null)
						{
							throw new PXException(PO.Messages.PurchaseOrderNotFound, orderType.Value, orderNbr.Value);
						}

						order.Selected = true;
						receiptEntry.openOrders.Update(order);
						receiptEntry.Actions["AddPOOrder2"].Press();

						var ExpirationDateField = targetEntity.Fields.FirstOrDefault(f => f.Name == "ExpirationDate") as EntityValueField;

						if (ExpirationDateField != null && ExpirationDateField.Value != null)
						{
							var inserted = receiptEntry.transactions.Cache.Inserted;
							foreach (POReceiptLine line in inserted)
							{
								var state = receiptEntry.transactions.Cache.GetStateExt(line, "ExpireDate") as PXDateState;
								if (state != null && state.Enabled == true && line.PONbr == orderNbr.Value)
									receiptEntry.transactions.Cache.SetValueExt(line, "ExpireDate", ExpirationDateField.Value);
							}
						}
						return;
					}
					else
					{
						detailsCache.Current = detailsCache.Insert();
						var receiptLineCurrent = detailsCache.Current as POReceiptLine;

						if (setManualPrice && receiptLineCurrent != null)
							detailsCache.SetValueExt<POReceiptLine.manualPrice>(receiptLineCurrent, true);

						if (hasAllocations)
						{
							if (detailsCache.Current == null)
								throw new InvalidOperationException("Cannot insert Purchase Receipt detail.");

							SetFieldsNeedToInsertAllocations(targetEntity, receiptEntry, receiptLineCurrent);

							var QtyField = targetEntity.Fields.FirstOrDefault(f => f.Name == "ReceiptQty") as EntityValueField;
							var ExpirationDateField = targetEntity.Fields.FirstOrDefault(f => f.Name == "ExpirationDate") as EntityValueField;
							if (QtyField != null)
							{
								receiptLineCurrent.ReceiptQty = decimal.Parse(QtyField.Value);
								receiptLineCurrent = receiptEntry.transactions.Update(receiptLineCurrent);
							}
							if (ExpirationDateField != null)
								receiptEntry.transactions.Cache.SetValueExt(receiptLineCurrent, "ExpireDate", ExpirationDateField.Value);
						}
					}
				}

				//All the created splits will be deleted. New splits will be inserted later.
				if (hasAllocations && detailsCache.Current != null)
				{
					var inserted = receiptEntry.splits.Cache.Inserted;
					foreach (POReceiptLineSplit split in inserted)
					{
						if (split.LineNbr == (detailsCache.Current as POReceiptLine).LineNbr)
							receiptEntry.splits.Delete(split);
					}
				}

			}
		}

		public static void SetFieldsNeedToInsertAllocations(EntityImpl targetEntity, POReceiptEntry receiptEntry, POReceiptLine receiptLineCurrent)
		{
			var InventoryIDField = targetEntity.Fields.SingleOrDefault(f => f.Name == "InventoryID") as EntityValueField;
			var SiteField = targetEntity.Fields.SingleOrDefault(f => f.Name == "Warehouse") as EntityValueField;
			var LocationField = targetEntity.Fields.SingleOrDefault(f => f.Name == "Location") as EntityValueField;
			var LotSerialNumberField = targetEntity.Fields.FirstOrDefault(f => f.Name == "LotSerialNbr") as EntityValueField;
			var SubItemField = targetEntity.Fields.FirstOrDefault(f => f.Name == "Subitem") as EntityValueField;

			if (InventoryIDField != null)
				receiptEntry.transactions.Cache.SetValueExt(receiptLineCurrent, "InventoryID", InventoryIDField.Value);
			if (SiteField != null)
				receiptEntry.transactions.Cache.SetValueExt(receiptLineCurrent, "SiteID", SiteField.Value);
			if (LocationField != null)
				receiptEntry.transactions.Cache.SetValueExt(receiptLineCurrent, "LocationID", LocationField.Value);
			if (LotSerialNumberField != null)
				receiptEntry.transactions.Cache.SetValueExt(receiptLineCurrent, "LotSerialNbr", LotSerialNumberField.Value);
			if (SubItemField != null)
				receiptEntry.transactions.Cache.SetValueExt(receiptLineCurrent, "SubItemID", SubItemField.Value);
		}

		protected virtual void FillInAddPOFilter(POReceiptEntry receiptEntry, EntityValueField orderType, EntityValueField orderNbr)
		{
			receiptEntry.filter.Cache.Remove(receiptEntry.filter.Current);
			receiptEntry.filter.Cache.Insert(new POReceiptEntry.POOrderFilter());
			var filter = receiptEntry.filter.Current;

			var state = receiptEntry.filter.Cache.GetStateExt(filter, "OrderType") as PXStringState;
			if (state != null && state.AllowedLabels.Contains(orderType.Value))
			{
				orderType.Value = state.ValueLabelDic.Single(p => p.Value == orderType.Value).Key;
			}

			receiptEntry.filter.Cache.SetValueExt(filter, "OrderType", orderType.Value);
			receiptEntry.filter.Cache.SetValueExt(filter, "OrderNbr", orderNbr.Value);
			filter = receiptEntry.filter.Update(filter);

			Dictionary<string, string> filterErrors = PXUIFieldAttribute.GetErrors(receiptEntry.filter.Cache, filter);

			if (filterErrors.Count() > 0)
			{
				throw new PXException(string.Join(";", filterErrors.Select(x => x.Key + "=" + x.Value)));
			}
		}

		/// <summary>
		/// Adds all lines of a given order to the shipment.
		/// </summary>
		protected void Action_AddOrder(PXGraph graph, ActionImpl action)
		{
			SOShipmentEntry shipmentEntry = (SOShipmentEntry)graph;
			shipmentEntry.addsofilter.Current.OrderType = ((EntityValueField)action.Fields.Single(f => f.Name == "OrderType")).Value;
			shipmentEntry.addsofilter.Current.OrderNbr = ((EntityValueField)action.Fields.Single(f => f.Name == "OrderNbr")).Value;
			shipmentEntry.addsofilter.Update(shipmentEntry.addsofilter.Current);

			foreach (SOShipmentPlan line in shipmentEntry.soshipmentplan.Select())
			{
				SelectLine(shipmentEntry, line);
			}

			shipmentEntry.addSO.Press();
		}

		/// <summary>
		/// Handles creation of document details in the Bills and Adjustments (AP301000) screen
		/// for cases when po entities are specified
		/// using the <see cref="PO.GraphExtensions.APInvoiceSmartPanel.AddPOOrderExtension.addPOOrder2">Add PO action</see>,
		/// the <see cref="PO.GraphExtensions.APInvoiceSmartPanel.AddPOReceiptExtension.addPOReceipt2">Add PO Receipt action</see>.
		/// </summary>
		[FieldsProcessed(new[] {
			"POOrderType",
			"POOrderNbr",
			"POLine",
			"POReceiptType",
			"POReceiptNbr",
			"POReceiptLine"
		})]
		protected virtual void BillDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var invoiceEntry = (AP.APInvoiceEntry)graph;

			EntityValueField orderType = targetEntity.Fields.SingleOrDefault(f => f.Name == "POOrderType") as EntityValueField;
			EntityValueField orderNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "POOrderNbr") as EntityValueField;
			EntityValueField orderLineNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "POLine") as EntityValueField;
			EntityValueField receiptType = targetEntity.Fields.SingleOrDefault(f => f.Name == "POReceiptType") as EntityValueField;
			EntityValueField receiptNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "POReceiptNbr") as EntityValueField;
			EntityValueField receiptLineNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "POReceiptLine") as EntityValueField;

			if (orderType != null && orderNbr != null)
			{
				if (orderLineNbr != null)
					AddPOOrderLineToBill(invoiceEntry, orderType, orderNbr, orderLineNbr);
				else
					AddPOOrderToBill(invoiceEntry, orderType, orderNbr);
			}
			else if (receiptNbr != null)
			{
				if (receiptType == null)
				{
					receiptType = new EntityValueField
					{
						Value = invoiceEntry.Document.Current?.DocType == APDocType.Invoice
							? POReceiptType.POReceipt
							: POReceiptType.POReturn
					};
				}
				else
				{
					var receipTypeState = invoiceEntry.Transactions.Cache.GetStateExt<APTran.receiptType>(new APTran()) as PXStringState;

					if (receipTypeState != null && receipTypeState.AllowedLabels.Contains(receiptType.Value))
						receiptType.Value = receipTypeState.ValueLabelDic.Single(p => p.Value == receiptType.Value).Key;
				}

				if (receiptLineNbr != null)
					AddPOReceiptLineToBill(invoiceEntry, receiptType, receiptNbr, receiptLineNbr);
				else
					AddPOReceiptToBill(invoiceEntry, receiptType, receiptNbr);
			}
			else if (orderNbr != null || orderType != null)
			{
				// Acuminator disable once PX1051 NonLocalizableString as we do not translate messages in API responses
				throw new PXException(NonLocalizableMessages.MissingPOOrderReference);
			}
			else
			{
				var detailsCache = invoiceEntry.Transactions.Cache;
				var row = detailsCache.CreateInstance();
				row = detailsCache.Insert(row);
				detailsCache.Current = row;
			}
		}

		[FieldsProcessed(new[] {
			"Name",
			"Description",
			"Value"
		})]
		protected void CustomerPaymentMethodDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var maint = (CustomerPaymentMethodMaint)graph;

			var name = (EntityValueField)targetEntity.Fields.SingleOrDefault(f => f.Name.OrdinalEquals("Name"));
			var description = (EntityValueField)targetEntity.Fields.SingleOrDefault(f => f.Name.OrdinalEquals("Description"));
			var value = (EntityValueField)targetEntity.Fields.Single(f => f.Name.OrdinalEquals("Value"));

			var cache = maint.Details.Cache;
			foreach (CustomerPaymentMethodDetail detail in maint.Details.Select())
			{
				var selectorRow = PXSelectorAttribute.Select(cache, detail, "DetailID") as PaymentMethodDetail;
				if ((name != null && (selectorRow.Descr == name.Value || detail.DetailID == name.Value))
					|| (description != null && (selectorRow.Descr == description.Value || selectorRow.DetailID == description.Value)))
				{

					cache.SetValueExt(detail, "Value", value.Value);
					maint.Details.Update(detail);
					break;
				}
			}
		}

		[FieldsProcessed(new[] {
			"ParentCategoryID",
			"Description"
		})]
		protected void ItemSalesCategory_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var maint = (INCategoryMaint)graph;

			maint.Folders.Current = maint.Folders.SelectSingle();
			maint.Actions["AddCategory"].Press();

			var parentCategoryId = entity.Fields.SingleOrDefault(f => f.Name.Equals("ParentCategoryID", StringComparison.OrdinalIgnoreCase)) as EntityValueField;
			var description = entity.Fields.SingleOrDefault(f => f.Name.Equals("Description", StringComparison.OrdinalIgnoreCase)) as EntityValueField;

			var item = maint.Folders.Cache.ActiveRow as INCategory;

			var cache = maint.Folders.Cache;

			if (parentCategoryId != null && !string.IsNullOrEmpty(parentCategoryId.Value))
			{
				cache.SetValueExt(item, "ParentID", int.Parse(parentCategoryId.Value));
			}

			if (description != null && !string.IsNullOrEmpty(description.Value))
			{
				cache.SetValueExt(item, "Description", description.Value);
			}

			maint.Folders.Cache.Current = item;
		}

		[FieldsProcessed(new[] {
			"TypeID",
			"Description",
			"WarehouseID"
		})]
		protected void PhysicalInventoryReview_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var maint = (INPIReview)graph;

			var typeId = targetEntity.Fields.SingleOrDefault(f => f.Name == "TypeID") as EntityValueField;
			var description = targetEntity.Fields.SingleOrDefault(f => f.Name == "Description") as EntityValueField;
			var warehouseid = targetEntity.Fields.SingleOrDefault(f => f.Name == "WarehouseID") as EntityValueField;


			var cache = maint.GeneratorSettings.Cache;
			cache.Clear();
			cache.Insert(new PIGeneratorSettings());

			var updateDic = new Dictionary<string, object>();
			if (typeId != null)
			{
				updateDic.Add("PIClassID", typeId.Value);
			}
			maint.ExecuteUpdate(maint.GeneratorSettings.View.Name, new Dictionary<string, object>(), updateDic);

			updateDic = new Dictionary<string, object>();
			if (description != null)
			{
				updateDic.Add("Descr", description.Value);
			}
			if (warehouseid != null)
			{
				updateDic.Add("SiteID", warehouseid.Value);
			}
			maint.ExecuteUpdate(maint.GeneratorSettings.View.Name, new Dictionary<string, object>(), updateDic);

			maint.GeneratorSettings.View.SetAnswer(null, WebDialogResult.OK);
			maint.Insert.Press();
		}


		protected void ItemSalesCategory_Delete(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var maint = (INCategoryMaint)graph;
			maint.Actions["DeleteCategory"].Press();
			maint.Actions["Save"].Press();
		}

		[FieldsProcessed(new[] {
			"Description",
			"ParentCategoryID"
		})]
		protected void ItemSalesCategory_Update(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var maint = (INCategoryMaint)graph;

			var item = maint.Folders.Cache.Current as INCategory;

			var description = entity.Fields.SingleOrDefault(f => f.Name.Equals("Description", StringComparison.OrdinalIgnoreCase)) as EntityValueField;
			if (description != null && !string.IsNullOrEmpty(description.Value))
			{
				maint.Folders.Cache.SetValueExt<INCategory.description>(item, description.Value);
			}

			var parent = entity.Fields.SingleOrDefault(f => f.Name.Equals("ParentCategoryID", StringComparison.OrdinalIgnoreCase)) as EntityValueField;
			if (parent != null && !string.IsNullOrEmpty(parent.Value))
			{
				item.ParentID = int.Parse(parent.Value);
			}

			maint.Folders.Update(item);
		}

		protected INUnit GetINUnit(InventoryItemMaint maint, EntityValueField fromUnit, EntityValueField toUnit)
		{
			var conversions = maint.UnitsOfMeasureExt.itemunits.Select().AsEnumerable();
			return conversions.Select(c => c[typeof(INUnit)] as INUnit)
				.FirstOrDefault(c => c != null
									 && (toUnit == null || string.IsNullOrEmpty(toUnit.Value) || string.Equals(c.ToUnit, toUnit.Value))
									 && string.Equals(c.FromUnit, fromUnit.Value));
		}

		[FieldsProcessed(new[] {
			"ToUOM",
			"FromUOM"
		})]
		protected void InventoryItemUOMConversion_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var maint = (InventoryItemMaint)graph;

			var fromUnit = targetEntity.Fields.SingleOrDefault(f => f.Name == "FromUOM") as EntityValueField;
			var toUnit = targetEntity.Fields.SingleOrDefault(f => f.Name == "ToUOM") as EntityValueField;

			var conversion = GetINUnit(maint, fromUnit, toUnit);
			if (conversion == null)
			{
				conversion = maint.UnitsOfMeasureExt.itemunits.Insert(new INUnit()
				{
					ToUnit = toUnit != null && !string.IsNullOrEmpty(toUnit.Value) ? toUnit.Value : null,
					FromUnit = fromUnit.Value
				});
			}

			maint.UnitsOfMeasureExt.itemunits.Current = conversion;
		}


		protected void InventoryItemUOMConversion_Delete(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var maint = (InventoryItemMaint)graph;

			var fromUnit = targetEntity.Fields.SingleOrDefault(f => f.Name == "FromUOM") as EntityValueField;
			var toUnit = targetEntity.Fields.SingleOrDefault(f => f.Name == "ToUOM") as EntityValueField;

			var conversion = GetINUnit(maint, fromUnit, toUnit);
			if (conversion != null)
			{
				maint.UnitsOfMeasureExt.itemunits.Delete(conversion);
				maint.Save.Press();
			}
		}

		[FieldsProcessed(new string[0])]
		protected void FinancialSettings_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var salesOrderGraph = (SOOrderEntry)graph;

			if (targetEntity.Fields.Count() > 0)
			{
				var taxZoneField = targetEntity.Fields.SingleOrDefault(f => f.Name == "CustomerTaxZone") as EntityValueField;

				if (salesOrderGraph.Document.Current != null && taxZoneField != null && salesOrderGraph.Document.Current.TaxZoneID != taxZoneField.Value)
					salesOrderGraph.Document.Current.OverrideTaxZone = true;
			}
		}

		[FieldsProcessed(new string[0])]
		protected void FinancialSettings_Update(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var salesOrderGraph = (SOOrderEntry)graph;

			if (targetEntity.Fields.Count() > 0)
			{
				var taxZoneField = targetEntity.Fields.SingleOrDefault(f => f.Name == "CustomerTaxZone") as EntityValueField;

				if (salesOrderGraph.Document.Current != null && taxZoneField != null && salesOrderGraph.Document.Current.TaxZoneID != taxZoneField.Value)
					salesOrderGraph.Document.Current.OverrideTaxZone = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected void Action_SalesOrderAddInvoice(PXGraph graph, ActionImpl action)
		{
			var orderEntry = (SOOrderEntry)graph;
			var ext = orderEntry.GetExtension<SO.GraphExtensions.SOOrderEntryExt.AddInvoiceExt>();

			foreach (InvoiceSplit line in ext.invoiceSplits.Select())
			{
				line.QtyToReturn = line.QtyAvailForReturn;
				ext.invoiceSplits.Update(line);
			}

			ext.addInvoiceOK.Press();
		}

		/// <summary>
		/// 
		/// </summary>
		protected void Action_SalesInvoiceAddOrder(PXGraph graph, ActionImpl action)
		{
			var invoiceEntry = (SOInvoiceEntry)graph;
			string orderNbr = ((EntityValueField)action.Fields.Single(f => f.Name == "OrderNbr")).Value;
			string orderType = ((EntityValueField)action.Fields.Single(f => f.Name == "OrderType")).Value;
			string shipmentNbr = ((EntityValueField)action.Fields.Single(f => f.Name == "ShipmentNbr")).Value;

			foreach (SOOrderShipment line in invoiceEntry.shipmentlist.Select().Select<SOOrderShipment>().Where(_ =>
																				_.OrderType == orderType &&
																				_.OrderNbr == orderNbr &&
																				_.ShipmentNbr == shipmentNbr))
			{
				line.Selected = true;
				invoiceEntry.shipmentlist.Update(line);
			}

			invoiceEntry.addShipment.Press();
		}

		/// <summary>
		/// 
		/// </summary>
		protected void Action_SalesOrderAddStockItem(PXGraph graph, ActionImpl action)
		{
			var orderEntry = (SOOrderEntry)graph;
			var statusExt = orderEntry.GetExtension<SOOrderSiteStatusLookupExt>();
			foreach (SOOrderSiteStatusSelected line in statusExt.ItemInfo.Select())
			{
				line.Selected = true;
				statusExt.ItemInfo.Update(line);
			}

			statusExt.addSelectedItems.Press();
			statusExt.ItemFilter.Cache.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		protected void Action_ShipmentAddOrder(PXGraph graph, ActionImpl action)
		{
			var shipmentEntry = (SOShipmentEntry)graph;

			foreach (SOShipmentPlan line in shipmentEntry.soshipmentplan.Select())
			{
				SelectLine(shipmentEntry, line);
			}

			shipmentEntry.addSO.Press();
		}

		/// <summary>
		/// 
		/// </summary>
		protected void Action_PaymentLoadDocuments(PXGraph graph, ActionImpl action)
		{
			var paymentEntry = (ARPaymentEntry)graph;

			paymentEntry.LoadInvoicesProc(false, paymentEntry.loadOpts.Current);
		}

		/// <summary>
		/// 
		/// </summary>
		protected void Action_PaymentLoadOrders(PXGraph graph, ActionImpl action)
		{
			var paymentEntry = (ARPaymentEntry)graph;

			var ordersToApplyTabExt = paymentEntry.GetOrdersToApplyTabExtension(true);
			ordersToApplyTabExt.LoadOrdersProc(false, paymentEntry.loadOpts.Current);
		}



		private static void AddPOOrderToBill(APInvoiceEntry invoiceEntry, EntityValueField orderType, EntityValueField orderNbr)
		{
			var state = invoiceEntry.Transactions.Cache.GetStateExt<APTran.pOOrderType>(new APTran { }) as PXStringState;

			if (state != null && state.AllowedLabels.Contains(orderType.Value))
			{
				orderType.Value = state.ValueLabelDic.Single(p => p.Value == orderType.Value).Key;
			}

			if (orderType.Value == POOrderType.RegularSubcontract)
			{
				var constructionExt = invoiceEntry.GetExtension<CN.Subcontracts.AP.GraphExtensions.ApInvoiceEntryAddSubcontractsExtension>();
				POOrderRS line = (POOrderRS)(constructionExt.Subcontracts.Select().Where(x => (((POOrderRS)x).OrderType == orderType.Value && ((POOrderRS)x).OrderNbr == orderNbr.Value)).FirstOrDefault());
				if (line == null)
				{
					// Acuminator disable once PX1051 NonLocalizableString as we do not translate messages in API responses
					throw new PXException(NonLocalizableMessages.SubcontractNotFound, orderNbr.Value);
				}

				line.Selected = true;
				constructionExt.Subcontracts.Update(line);
				constructionExt.AddSubcontract.Press();
			}
			else
			{
				var orderExtension = invoiceEntry.GetExtension<PO.GraphExtensions.APInvoiceSmartPanel.AddPOOrderExtension>();
				POOrderRS line = (POOrderRS)(orderExtension.poorderslist.Select().Where(x => (((POOrderRS)x).OrderType == orderType.Value && ((POOrderRS)x).OrderNbr == orderNbr.Value)).FirstOrDefault());

				if (line == null)
				{
					// Acuminator disable once PX1051 NonLocalizableString as we do not translate messages in API responses
					throw new PXException(NonLocalizableMessages.PurchaseOrderDoesNotExist, orderType.Value, orderNbr.Value);
				}

				line.Selected = true;
				orderExtension.poorderslist.Update(line);
				orderExtension.addPOOrder2.Press();
			}
		}

		private static void AddPOOrderLineToBill(APInvoiceEntry invoiceEntry, EntityValueField orderType, EntityValueField orderNbr, EntityValueField orderLineNbr)
		{
			var state = invoiceEntry.Transactions.Cache.GetStateExt<APTran.pOOrderType>(new APTran { }) as PXStringState;

			if (state != null && state.AllowedLabels.Contains(orderType.Value))
			{
				orderType.Value = state.ValueLabelDic.Single(p => p.Value == orderType.Value).Key;
			}

			if (orderType.Value == POOrderType.RegularSubcontract)
			{
				var constructionExt = invoiceEntry.GetExtension<CN.Subcontracts.AP.GraphExtensions.ApInvoiceEntryAddSubcontractsExtension>();
				int poLineNbr = int.Parse(orderLineNbr.Value);
				POLineRS line = (POLineRS)(constructionExt.SubcontractLines.Select().Where(x => (((POLineRS)x).OrderType == orderType.Value && ((POLineRS)x).OrderNbr == orderNbr.Value && ((POLineRS)x).LineNbr == poLineNbr)).FirstOrDefault());

				if (line == null)
				{
					// Acuminator disable once PX1051 NonLocalizableString as we do not translate messages in API responses
					throw new PXException(NonLocalizableMessages.SubcontractLineNotFound, orderNbr.Value, orderLineNbr.Value);
				}

				line.Selected = true;
				constructionExt.SubcontractLines.Update(line);
				constructionExt.AddSubcontractLine.Press();
			}
			else
			{
				var orderLineExtension = invoiceEntry.GetExtension<PO.GraphExtensions.APInvoiceSmartPanel.AddPOOrderLineExtension>();
				int poLineNbr = int.Parse(orderLineNbr.Value);

				POLineRS line = (POLineRS)(orderLineExtension.poorderlineslist.Select().Where(x => (((POLineRS)x).OrderType == orderType.Value && ((POLineRS)x).OrderNbr == orderNbr.Value && ((POLineRS)x).LineNbr == poLineNbr)).FirstOrDefault());

				if (line == null)
				{
					// Acuminator disable once PX1051 NonLocalizableString as we do not translate messages in API responses
					throw new PXException(NonLocalizableMessages.PurchaseOrderLineNotFound, orderType.Value, orderNbr.Value, orderLineNbr.Value);
				}

				line.Selected = true;
				orderLineExtension.poorderlineslist.Update(line);
				orderLineExtension.addPOOrderLine2.Press();
			}
		}

		private static void AddPOReceiptToBill(APInvoiceEntry invoiceEntry, EntityValueField receiptType, EntityValueField receiptNbr)
		{
			var receiptExtension = invoiceEntry.GetExtension<PO.GraphExtensions.APInvoiceSmartPanel.AddPOReceiptExtension>();

			POReceipt line = (POReceipt)(receiptExtension.poreceiptslist.Select()
				.Where(x =>
					((POReceipt)x).ReceiptType == receiptType.Value
					&& ((POReceipt)x).ReceiptNbr == receiptNbr.Value)
				.FirstOrDefault());

			if (line == null)
			{
				// Acuminator disable once PX1051 NonLocalizableString as we do not translate messages in API responses
				throw new PXException(NonLocalizableMessages.PurchaseReceiptNotFound, receiptNbr.Value);
			}

			line.Selected = true;



			receiptExtension.poreceiptslist.Update(line);

			receiptExtension.addPOReceipt2.Press();
		}

		private static void AddPOReceiptLineToBill(APInvoiceEntry invoiceEntry, EntityValueField receiptType, EntityValueField receiptNbr, EntityValueField receiptLineNbr)
		{
			var receiptLineExtension = invoiceEntry.GetExtension<PO.GraphExtensions.APInvoiceSmartPanel.AddPOReceiptLineExtension>();

			POReceiptLineAdd line = receiptLineExtension.ReceipLineAdd.Select(receiptType.Value, receiptNbr.Value, receiptLineNbr.Value);

			if (line == null)
			{
				// Acuminator disable once PX1051 NonLocalizableString as we do not translate messages in API responses
				throw new PXException(NonLocalizableMessages.PurchaseReceiptLineNotFound, receiptNbr.Value, receiptLineNbr.Value);
			}

			line.Selected = true;

			receiptLineExtension.poReceiptLinesSelection.Update(line);

			receiptLineExtension.addReceiptLine2.Press();
		}

		[FieldsProcessed(new string[0])]
		protected void Payments_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			EntityValueField refNbr = targetEntity.Fields.SingleOrDefault(f => f.Name == "ReferenceNbr") as EntityValueField;
			if (refNbr != null)
			{
				SOAdjust adjust = (SOAdjust)((SOOrderEntry)graph).Adjustments.Cache.CreateInstance();
				adjust.AdjgRefNbr = refNbr.Value;
				((SOOrderEntry)graph).Adjustments.Insert(adjust);
				return;
			}
			EntityValueField docType = targetEntity.Fields.SingleOrDefault(f => f.Name == "DocType") as EntityValueField;
			EntityValueField appliedToOrderEntity = targetEntity.Fields.SingleOrDefault(f => f.Name == "AppliedToOrder") as EntityValueField;

			if (appliedToOrderEntity != null && appliedToOrderEntity.Value != null)
			{
				decimal appliedToOrder = decimal.Parse(appliedToOrderEntity.Value);

				SOOrderEntry orderEntry = (SOOrderEntry)graph;
				orderEntry.Save.Press();

				var createPaymentExtension = graph.GetExtension<SO.GraphExtensions.SOOrderEntryExt.CreatePaymentExt>();

				createPaymentExtension.CheckTermsInstallmentType();

				if (docType != null)
				{
					var state = graph.Caches[typeof(ARRegister)].GetStateExt(new ARRegister(), "DocType") as PXStringState;
					if (state != null && state.ValueLabelDic != null)
					{
						bool keyFound = false;
						foreach (var rec in state.ValueLabelDic)
						{
							if (rec.Value == docType.Value || rec.Key == docType.Value)
							{
								keyFound = true;
								docType.Value = rec.Key;
								break;
							}
						}
						if (!keyFound)
							docType = null;
					}
				}

				SOOrder order = orderEntry.Document.Current;

				var payment = createPaymentExtension.QuickPayment.Current;
				createPaymentExtension.SetDefaultValues(payment, order);

				PXCache paymentParameterCache = createPaymentExtension.QuickPayment.Cache;

				EntityValueField paymentMethodEntity = targetEntity.Fields.SingleOrDefault(f => f.Name == "PaymentMethod") as EntityValueField;
				if (paymentMethodEntity != null && paymentMethodEntity.Value != null)
				{
					paymentParameterCache.SetValueExt<SOQuickPayment.paymentMethodID>(payment, paymentMethodEntity.Value);
				}

				EntityValueField cashAccountEntity = targetEntity.Fields.SingleOrDefault(f => f.Name == "CashAccount") as EntityValueField;
				if (cashAccountEntity != null && cashAccountEntity.Value != null)
				{
					paymentParameterCache.SetValueExt<SOQuickPayment.cashAccountID>(payment, cashAccountEntity.Value);
				}

				EntityValueField paymentAmountEntity = targetEntity.Fields.SingleOrDefault(f => f.Name == "PaymentAmount") as EntityValueField;
				if (paymentAmountEntity != null && paymentAmountEntity.Value != null)
				{
					decimal paymentAmount = decimal.Parse(paymentAmountEntity.Value);
					paymentParameterCache.SetValueExt<SOQuickPayment.curyOrigDocAmt>(payment, paymentAmount);
				}
				else
				{
					paymentParameterCache.SetValueExt<SOQuickPayment.curyOrigDocAmt>(payment, appliedToOrder);
				}

				ARPaymentEntry paymentEntry = createPaymentExtension.CreatePayment(payment, order,
					docType != null ? docType.Value : ARPaymentType.Payment);

				EntityValueField paymentRefEntity = targetEntity.Fields.SingleOrDefault(f => f.Name == "PaymentRef") as EntityValueField;
				if (paymentRefEntity != null && paymentRefEntity.Value != null)
				{
					paymentEntry.Document.Cache.SetValueExt<ARPayment.extRefNbr>(paymentEntry.Document.Current, paymentRefEntity.Value);
				}

				paymentEntry.Save.Press();
				orderEntry.Cancel.Press();
				orderEntry.Document.Current = orderEntry.Document.Search<SOOrder.orderNbr>(
					orderEntry.Document.Current.OrderNbr,
					orderEntry.Document.Current.OrderType);
				try
				{
					orderEntry.Adjustments.Current = orderEntry.Adjustments.Select().Where(x => (((SOAdjust)x).AdjgDocType == paymentEntry.Document.Current.DocType && ((SOAdjust)x).AdjgRefNbr == paymentEntry.Document.Current.RefNbr)).First();
				}
				catch
				{
					// Acuminator disable once PX1051 NonLocalizableString as we do not translate messages in API responses
					throw new PXException(NonLocalizableMessages.PaymentInSOOrderNotFound, paymentEntry.Document.Current.DocType, paymentEntry.Document.Current.RefNbr);
				}
			}
		}

		[FieldsProcessed(new[] {
			"AttributeID",
			"Value"
		})]
		protected void AttributeValue_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			AttributeBase_Insert(graph, entity, targetEntity, "AttributeID");
		}

		[FieldsProcessed(new[] {
			"Attribute",
			"Value"
		})]
		protected void AttributeDetail_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			// TODO: merge AttributeDetail and AttributeValue entities in new endpoint version (2019r..)
			AttributeBase_Insert(graph, entity, targetEntity, "Attribute");
		}
	}
}
