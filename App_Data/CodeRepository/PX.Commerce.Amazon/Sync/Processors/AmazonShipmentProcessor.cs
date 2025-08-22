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

using PX.Api;
using PX.Commerce.Amazon.API;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon
{
	public class AmazonShipmentEntityBucket : EntityBucketBase, IEntityBucket
	{
		public IMappedEntity Primary => Shipment;
		public IMappedEntity[] Entities => new IMappedEntity[] { Primary };

		public MappedShipment Shipment;
		public List<MappedFBMOrder> Orders = new List<MappedFBMOrder>();
	}

	public class AmazonShipmentsRestrictor : BCBaseRestrictor, IRestrictor
	{
		public FilterResult RestrictExport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			#region Shipments
			return base.Restrict<MappedShipment>(mapped, delegate (MappedShipment obj)
			{
				if (obj.Local != null)
				{
					// mark shipment as invalid when it's not confirmed
					if (obj.Local.Confirmed?.Value == false)
					{
						return new FilterResult(FilterStatus.Invalid,
								PXMessages.Localize(BCMessages.LogShipmentSkippedNotConfirmed));
					}

					// mark shipment as invalid when it's not of Issue type which is usual shipment
					// (there can be Dropship and Invoice types which are currently not supported)
					if (obj.Local.ShipmentType?.Value != null)
					{
						if (obj.Local.ShipmentType?.Value == SOShipmentType.Invoice)
							return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.NotSupportedInvoiceShipment, obj.Local.ShipmentNumber.Value));
						else if (obj.Local.ShipmentType?.Value == SOShipmentType.DropShip)
							return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.NotSupportedDropshipShipment, obj.Local.ShipmentNumber.Value));
					}

					// ignore shipment if order is not yet imported
					if (obj.Local?.OrderNoteIds != null)
					{
						Boolean anyFound = false;
						foreach (var orderNoteID in obj.Local?.OrderNoteIds)
						{
							if (processor.SelectStatus(BCEntitiesAttribute.Order, orderNoteID) == null) continue;

							anyFound = true;
						}
						if (!anyFound)
						{
							return new FilterResult(FilterStatus.Ignore,
								PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogShipmentSkippedNoOrder, obj.Local.ShipmentNumber?.Value ?? obj.Local.SyncID.ToString()));
						}
					}

				}

				return null;
			});
			#endregion
		}

		public FilterResult RestrictImport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return null;
		}
	}

	[BCProcessor(typeof(BCAmazonConnector), BCEntitiesAttribute.Shipment, BCCaptions.Shipment, 240,
		IsInternal = false,
		Direction = SyncDirection.Export,
		PrimaryDirection = SyncDirection.Export,
		PrimarySystem = PrimarySystem.Local,
		ExternTypes = new Type[] { typeof(Feed) },
		LocalTypes = new Type[] { typeof(BCShipments) },
		GIScreenID = BCConstants.GenericInquiryShipmentDetails,
		GIResult = typeof(BCShipmentsResult),
		AcumaticaPrimaryType = typeof(PX.Objects.SO.SOShipment),
		AcumaticaPrimarySelect = typeof(PX.Objects.SO.SOShipment.shipmentNbr),
		URL = "orders-v3/order/{0}",
		Requires = new string[] { BCEntitiesAttribute.Order }
	)]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.ShipmentLine, EntityName = BCCaptions.ShipmentLine, AcumaticaType = typeof(PX.Objects.SO.SOShipLine))]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.ShipmentBoxLine, EntityName = BCCaptions.ShipmentLineBox, AcumaticaType = typeof(PX.Objects.SO.SOPackageDetailEx))]
	public class AmazonShipmentProcessor : BCProcessorBulkBase<AmazonShipmentProcessor, AmazonShipmentEntityBucket, MappedShipment>, IProcessor
	{
		[InjectDependency]
		public Func<IAmazonRestClient, XMLFeedData, IXmlFeedDataProvider> XmlFeedDataProviderFactory { get; set; }
		public IXmlFeedDataProvider XmlFeedDataProvider;
		public AmazonHelper helper = PXGraph.CreateInstance<AmazonHelper>();

		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation);
			helper.Initialize(this);
			var client = ((BCAmazonConnector)iconnector).GetRestClient(GetBindingExt<BCBindingAmazon>(), GetBinding());
			XMLFeedData xMLFeedData = new()
			{
				MessageType = MessageTypes.ORDER_FULFILLMENT,
				FeedType = FeedTypes.POST_ORDER_FULFILLMENT_DATA,
				Marketplace =  GetBindingExt<BCBindingAmazon>().Marketplace ,
				SellerId = GetBindingExt<BCBindingAmazon>().SellerPartnerId
			};

			XmlFeedDataProvider = XmlFeedDataProviderFactory(client,xMLFeedData);
		}

		public override void NavigateLocal(IConnector connector, ISyncStatus status, ISyncDetail detail = null)
		{
			SOOrderShipment orderShipment = PXSelect<SOOrderShipment, Where<SOOrderShipment.shippingRefNoteID, Equal<Required<SOOrderShipment.shippingRefNoteID>>>>.Select(this, status?.LocalID);
			if (orderShipment.ShipmentType == SOShipmentType.DropShip)//dropshipment
			{
				POReceiptEntry extGraph = PXGraph.CreateInstance<POReceiptEntry>();
				EntityHelper helper = new EntityHelper(extGraph);
				helper.NavigateToRow(extGraph.GetPrimaryCache().GetItemType().FullName, status.LocalID, PXRedirectHelper.WindowMode.NewWindow);
			}
			if (orderShipment.ShipmentType == SOShipmentType.Issue && orderShipment.ShipmentNoteID == null) //Invoice
			{
				ARInvoiceEntry extGraph = PXGraph.CreateInstance<ARInvoiceEntry>();
				EntityHelper helper = new EntityHelper(extGraph);
				helper.NavigateToRow(extGraph.GetPrimaryCache().GetItemType().FullName, status.LocalID, PXRedirectHelper.WindowMode.NewWindow);
			}
			else//shipment
			{
				SOShipmentEntry extGraph = PXGraph.CreateInstance<SOShipmentEntry>();
				EntityHelper helper = new EntityHelper(extGraph);
				helper.NavigateToRow(extGraph.GetPrimaryCache().GetItemType().FullName, status.LocalID, PXRedirectHelper.WindowMode.NewWindow);
			}
		}

		#region Import

		public override Task FetchBucketsImport(CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public override Task<List<AmazonShipmentEntityBucket>> GetBucketsImport(List<BCSyncStatus> ids, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public override Task SaveBucketsImport(List<AmazonShipmentEntityBucket> buckets, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Export

		public override async Task FetchBucketsExport(CancellationToken cancellationToken = default)
		{
			BCBindingAmazon bindingAmazon = GetBindingExt<BCBindingAmazon>();
			var sellingPartnerId = bindingAmazon.SellerPartnerId;
			if (string.IsNullOrEmpty(sellingPartnerId))
				throw new PXException(PXMessages.Localize(AmazonMessages.SellingPartnerIdNotProvidedPrepare));

			IEnumerable<BCShipmentsResult> giResult = cbapi.GetGIResult<BCShipmentsResult>(new BCShipments()
			{
				BindingID = GetBinding().BindingID.ValueField()
			}, BCConstants.GenericInquiryShipment);

			foreach (var result in giResult)
			{
				if (result.NoteID?.Value == null)
					continue;
				BCShipments bCShipments = new BCShipments()
				{
					ShippingNoteID = result.NoteID,
					LastModified = result.LastModifiedDateTime,
					ExternalShipmentUpdated = result.ExternalShipmentUpdated,
					ShipmentNumber = result.ShipmentNumber,
					ShipmentType = result.ShipmentType,
					Confirmed = result.Confirmed
				};
				MappedShipment obj = new MappedShipment(bCShipments, bCShipments.ShippingNoteID.Value, bCShipments.LastModified.Value);
				EntityStatus status = EnsureStatus(obj, SyncDirection.Export);
			}
		}

		protected virtual void MapFilterFields(List<BCShipmentsResult> results, BCShipments impl)
		{
			impl.OrderNoteIds = new List<Guid?>();
			foreach (var result in results)
			{
				impl.ShippingNoteID = result.NoteID;
				impl.VendorRef = result.InvoiceNbr;
				impl.ShipmentNumber = result.ShipmentNumber;
				impl.ShipmentType = result.ShipmentType;
				impl.LastModified = result.LastModifiedDateTime;
				impl.Confirmed = result.Confirmed;
				impl.OrderNoteIds.Add(result.OrderNoteID.Value);
			}
		}

		public override async Task<List<AmazonShipmentEntityBucket>> GetBucketsExport(List<BCSyncStatus> ids, CancellationToken cancellationToken = default)
		{
			BCBindingAmazon bindingAmazon = GetBindingExt<BCBindingAmazon>();
			var sellingPartnerId = bindingAmazon.SellerPartnerId;
			if (string.IsNullOrEmpty(sellingPartnerId))
				throw new PXException(PXMessages.Localize(AmazonMessages.SellingPartnerIdNotProvidedProcess));

			List<AmazonShipmentEntityBucket> buckets = new List<AmazonShipmentEntityBucket>();

			foreach (var id in ids)
			{
				BCShipments bcShipments = (new BCShipments()
				{
					ShippingNoteID = id.LocalID.ValueField()
				});
				bcShipments.Results = cbapi.GetGIResult<BCShipmentsResult>(bcShipments, BCConstants.GenericInquiryShipmentDetails).ToList();

				if (bcShipments?.Results == null || bcShipments?.Results?.Any() != true) continue;

				MapFilterFields(bcShipments?.Results, bcShipments);

				AmazonShipmentEntityBucket bucket = new AmazonShipmentEntityBucket();
				EntityStatus shipmentTypeStatus;
				// only continue with shipments that are of type Issue
				if (bcShipments.ShipmentType.Value == SOShipmentType.Issue)
				{
					shipmentTypeStatus = GetShipment(bucket, bcShipments);
				}
				// skip those that are of other types, for example, Dropship or Invoice
				else
				{
					// TODO: should simplify this code by improving the querying of status in SynchronizeStatus method in the Core project
					MappedShipment obj = bucket.Shipment = bucket.Shipment.Set(bcShipments, bcShipments.ShippingNoteID.Value, bcShipments.LastModified.Value);
					BCSyncStatus status = SelectStatus(obj);
					obj.SyncID = status?.SyncID;
					if (bcShipments.ShipmentType.Value == SOShipmentType.DropShip)
						SynchronizeStatus(status, BCSyncOperationAttribute.Skipped, BCSyncStatusAttribute.Invalid,
							PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.NotSupportedDropshipShipment, bcShipments.ShipmentNumber.Value));
					else
						SynchronizeStatus(status, BCSyncOperationAttribute.Skipped, BCSyncStatusAttribute.Invalid,
							PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.NotSupportedInvoiceShipment, bcShipments.ShipmentNumber.Value));

					shipmentTypeStatus = EntityStatus.Skipped;
				}

				if (Operation.PrepareMode != PrepareMode.Reconciliation && shipmentTypeStatus != EntityStatus.Pending && Operation.SyncMethod != SyncMode.Force) continue;

				buckets.Add(bucket);
			}

			return buckets;
		}

		public override async Task MapBucketExport(AmazonShipmentEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			MappedShipment obj = bucket.Shipment;
			// throw error if shipment is not yet confirmed
			if (obj.Local?.Confirmed?.Value == false) throw new PXException(BCMessages.ShipmentNotConfirmed);
			if (string.IsNullOrWhiteSpace(obj.Local?.Shipment?.ShipVia?.Value))
			{
				throw new PXException(AmazonMessages.NoShipViaCodeHasBeen, obj.Local?.Shipment.ShipmentNbr.Value);
			}
			// only continue with shipments of type Issue
			if (obj.Local.ShipmentType.Value == SOShipmentType.Issue)
			{
				Shipment impl = obj.Local.Shipment;
				MapShipment(bucket, obj, impl);
			}
			else if (obj.Local.ShipmentType.Value == SOShipmentType.DropShip)
				throw new PXException(PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.NotSupportedDropshipShipment, obj.Local.ShipmentNumber.Value));
			else if (obj.Local.ShipmentType.Value == SOShipmentType.Invoice)
				throw new PXException(PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.NotSupportedInvoiceShipment, obj.Local.ShipmentNumber.Value));
		}
		public override async Task SaveBucketsExport(List<AmazonShipmentEntityBucket> buckets, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(GetBindingExt<BCBindingAmazon>().SellerPartnerId))
				throw new PXException(PXMessages.Localize(AmazonMessages.SellingPartnerIdNotProvidedProcess));

			List<ShipmentMessage> feedMessages = CreateListOfMessages(buckets);
			await ProcessFeed(buckets, feedMessages);
		}


		//TODO: Move ProcessFeed to a separate class

		/// <summary>
		/// Only proceed further to attempt exporting shipments if there's any feed message
		/// </summary>
		/// <param name="buckets"></param>
		/// <param name="feedMessages"></param>
		private async Task ProcessFeed(List<AmazonShipmentEntityBucket> buckets, List<ShipmentMessage> feedMessages)
		{
			if (!feedMessages.Any())
				return;

			Dictionary<Guid, MappedShipment> bucketDict = buckets.ToDictionary(keySelector: x => x.Shipment.LocalID.Value, elementSelector: x => x.Shipment);

			// create and submit feed to export shipment(s)
			var feedProcessingResults = await XmlFeedDataProvider.SendFeedAsync(feedMessages);

			// process export processing results
			// create a dictionary that has ShipmentNumber as key and errors (if there's any) as value
			// a record that has an empty list as value means shipment was processed sucessfully
			var shipmentProcessingResults = new Dictionary<string, List<string>>();
			foreach (FeedProcessingResult<ShipmentMessage> result in feedProcessingResults)
			{
				List<ShipmentMessage> processedMessages = result.FeedMessages;
				XmlProcessingReport processingReport = result.ProcessingReport;

				// get first result from processing report
				// when there's a document-level error there should be only error result returned, either by Amazon or by the data provider
				Result firstResult = processingReport.Result.FirstOrDefault();

				// when result has XmlDocumentIllformed error code, it means all messages of shipments will not be processed by Amazon
				// thus, mark all related shipments as failed
				if (firstResult != null && firstResult.ResultMessageCode == FeedErrorMessageCode.XmlDocumentIllformed)
				{
					foreach (var message in processedMessages)
					{
						MappedShipment shipment = bucketDict[message.OrderFulfillment.OriginalShipmentNoteId];
						if (shipmentProcessingResults.ContainsKey(shipment.Local.ShipmentNumber.Value))
							shipmentProcessingResults[shipment.Local.ShipmentNumber.Value].Add(AmazonMessages.FeedDocumentIllformed);
						else
							shipmentProcessingResults[shipment.Local.ShipmentNumber.Value] = new List<string> { AmazonMessages.FeedDocumentIllformed };
					}

					continue;
				}

				// similar behavior when there's a document-level error
				if (firstResult != null && firstResult.ResultMessageCode == FeedErrorMessageCode.XmlDocumentLevelError)
				{
					foreach (var message in processedMessages)
					{
						MappedShipment shipment = bucketDict[message.OrderFulfillment.OriginalShipmentNoteId];
						if (shipmentProcessingResults.ContainsKey(shipment.Local.ShipmentNumber.Value))
							shipmentProcessingResults[shipment.Local.ShipmentNumber.Value].Add(firstResult.ResultDescription);
						else
							shipmentProcessingResults[shipment.Local.ShipmentNumber.Value] = new List<string> { firstResult.ResultDescription };
					}

					continue;
				}

				// otherwise, process results individually
				// select errors from the processing results
				List<Result> errorResults = processingReport.Result.Where(x => x.ResultCode == FeedProcessingResultStatus.Error).ToList();

				foreach (var message in processedMessages)
				{
					MappedShipment shipment = bucketDict[message.OrderFulfillment.OriginalShipmentNoteId];
					if (!shipmentProcessingResults.ContainsKey(shipment.Local.ShipmentNumber.Value))
						shipmentProcessingResults[shipment.Local.ShipmentNumber.Value] = new List<string>();

					// select error messages (if any) and assign them (or append to an existing one) to corresponding shipment number 
					List<string> shipmentErrors = errorResults.Where(x => x.MessageID == message.MessageID).Select(x => x.ResultDescription).ToList();
					if (shipmentErrors.Count > 0)
						shipmentProcessingResults[shipment.Local.ShipmentNumber.Value].AddRange(shipmentErrors);
				}
			}

			bool hasErrors = false;
			foreach (var shipment in buckets)
			{
				MappedShipment obj = shipment.Shipment;

				if (obj.Extern.Fulfillments == null || obj.Extern.Fulfillments.Count() == 0) continue;

				string shipmentNbr = obj.Local.ShipmentNumber.Value;
				List<string> shipmentErrors = shipmentProcessingResults[shipmentNbr];

				// if the dictionary has no record of the shipment then it means, for some reason, the shipment has never been sent to Amazon
				// thus, continue without modifying sync status of the shipment
				if (shipmentErrors == null) continue;

				// add semi-colon character as a separator between AmazonOrderId and SyncID in order to cover the scenario where there are multiple shipments
				// for a same order and Amazon does not provide different IDs for shipments of an order
				obj.AddExtern(obj.Extern, new Object[] { obj.Extern.Fulfillments.FirstOrDefault()?.AmazonOrderID, obj.SyncID }.KeyCombine(), new Object[] { obj.Extern.Fulfillments.FirstOrDefault()?.AmazonOrderID }.KeyCombine(), DateTime.Now);

				if (shipmentErrors.Count == 0)
				{
					UpdateStatus(obj, BCSyncOperationAttribute.ExternUpdate);
				}
				else
				{
					Log(obj.SyncID, SyncDirection.Export, new Exception(string.Join(" ", shipmentErrors)));
					UpdateStatus(obj, BCSyncOperationAttribute.ExternFailed,
						PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.ShipmentExternError, shipmentNbr, string.Join(" ", shipmentErrors)));
					Operation.Callback?.Invoke(new SyncInfo(obj.SyncID ?? 0, SyncDirection.Export, SyncResult.Error, new Exception(string.Join(" ", shipmentErrors))));
				}
			}
		}

		// TODO: Move it to a separate class
		private List<ShipmentMessage> CreateListOfMessages(List<AmazonShipmentEntityBucket> buckets)
		{
			var feedMessages = new List<ShipmentMessage>();
			int messageIdInc = 1;
			foreach (var shipment in buckets)
			{
				MappedShipment obj = shipment.Shipment;
				var fulfillments = obj.Extern.Fulfillments;
				// mark shipment as invalid when there's nothing in Fulfillments
				if (fulfillments == null || fulfillments?.Count() == 0)
				{
					SynchronizeStatus(obj, BCSyncOperationAttribute.Skipped, BCSyncStatusAttribute.Invalid,
						PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.ShipmentIsEmpty, obj.Local.ShipmentNumber.Value));
					continue;
				}

				foreach (var fulfillment in fulfillments)
				{
					var message = new ShipmentMessage();
					message.MessageID = messageIdInc.ToString();
					message.OrderFulfillment = fulfillment;
					message.OrderFulfillment.OriginalShipmentNoteId = obj.LocalID.Value;
					feedMessages.Add(message);

					messageIdInc++;
				}
			}

			return feedMessages;
		}

		protected virtual EntityStatus GetShipment(AmazonShipmentEntityBucket bucket, BCShipments bCShipment)
		{
			if (bCShipment.ShippingNoteID == null || bCShipment.ShippingNoteID.Value == Guid.Empty) return EntityStatus.None;
			bCShipment.Shipment = cbapi.GetByID<Shipment>(bCShipment.ShippingNoteID.Value);
			if (bCShipment.Shipment == null) return EntityStatus.None;

			MappedShipment obj = bucket.Shipment = bucket.Shipment.Set(bCShipment, bCShipment.ShippingNoteID.Value, bCShipment.LastModified.Value);
			EntityStatus status = EnsureStatus(obj, SyncDirection.Export);

			IEnumerable<ShipmentDetail> lines = bCShipment.Shipment.Details
				.GroupBy(r => new { OrderType = r.OrderType.Value, OrderNbr = r.OrderNbr.Value })
				.Select(r => r.First());
			foreach (ShipmentDetail line in lines)
			{
				SalesOrder orderImpl = cbapi.Get<SalesOrder>(new SalesOrder() { OrderType = line.OrderType.Value.SearchField(), OrderNbr = line.OrderNbr.Value.SearchField() });
				if (orderImpl == null) throw new PXException(BCMessages.OrderNotFound, bCShipment.Shipment.ShipmentNbr.Value);
				MappedFBMOrder orderObj = new MappedFBMOrder(orderImpl, orderImpl.SyncID, orderImpl.SyncTime);
				EntityStatus orderStatus = EnsureStatus(orderObj);

				if (orderObj.ExternID == null) throw new PXException(BCMessages.OrderNotSyncronized, orderImpl.OrderNbr.Value);
				bucket.Orders.Add(orderObj);
			}
			return status;
		}

		protected virtual void MapShipment(AmazonShipmentEntityBucket bucket, MappedShipment obj, Shipment impl)
		{
			AmazonFulfillmentData amazonFulfillmentData = obj.Extern = new AmazonFulfillmentData();
			amazonFulfillmentData.Fulfillments = new List<OrderFulfillment>();
			List<AmazonShipmentLineDetails> shipmentLineDetails = new List<AmazonShipmentLineDetails>();

			var packageDetails = new List<(SOShipLineSplitPackage SOShipLineSplitPackage, InventoryItem InventoryItem, INUnit INUnit)>();
			foreach (PXResult<SOShipLineSplitPackage, InventoryItem> item in PXSelectJoin<SOShipLineSplitPackage,
				InnerJoin<InventoryItem, On<SOShipLineSplitPackage.inventoryID, Equal<InventoryItem.inventoryID>>,
				LeftJoin<INUnit, On<INUnit.inventoryID, Equal<InventoryItem.inventoryID>, And<INUnit.fromUnit, NotEqual<InventoryItem.baseUnit>, And<INUnit.toUnit, Equal<InventoryItem.baseUnit>,
				And<INUnit.fromUnit, In<Required<INUnit.fromUnit>>>>>>>>,
				Where<SOShipLineSplitPackage.shipmentNbr, Equal<Required<SOShipLineSplitPackage.shipmentNbr>>>>.
				Select(this, impl.Details.Select(detail => detail.UOM.Value).ToArray(), impl.ShipmentNbr?.Value))
			{
				packageDetails.Add((item.GetItem<SOShipLineSplitPackage>(), item.GetItem<InventoryItem>(), item.GetItem<INUnit>()));
			}

			// fail the shipment if it has no box
			var shipmentPackages = impl.Packages ?? new List<ShipmentPackage>();
			if (shipmentPackages.Count == 0)
			{
				throw new PXException(AmazonMessages.ShipmentWithoutBoxes, impl.ShipmentNbr.Value);
			}

			foreach (ShipmentPackage package in shipmentPackages)
			{
				var packageDetail = packageDetails.Where(x => x.SOShipLineSplitPackage.PackageLineNbr == package.LineNbr?.Value && x.SOShipLineSplitPackage.PackedQty != 0)?.ToList() ?? new List<(SOShipLineSplitPackage, InventoryItem, INUnit)>();
				// fail the shipment if it has any empty box
				if (packageDetail.Count == 0)
				{
					throw new PXException(AmazonMessages.BoxesWithoutItems, impl.ShipmentNbr.Value, package.TrackingNbr?.Value);
				}

				package.ShipmentPackageLineDetails.AddRange(packageDetail.Select(x => new ShipmentPackageLineDetail(x.Item1.ShipmentLineNbr, x.Item1.PackedQty, x.Item3, x.SOShipLineSplitPackage.InventoryID)));
				ValidateSplittedInventoryItems(package, impl);
			}

			ValidateShippedItemsQuantity(shipmentPackages, impl);

			foreach (MappedFBMOrder order in bucket.Orders)
			{
				var orderItemCDs = order.Local.Details.Select(x => x.InventoryID.Value).ToList();
				List<InventoryItem> orderItems = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, In<Required<InventoryItem.inventoryCD>>>>
					.Select(this, new object[] { orderItemCDs })
					.Select(x => x.GetItem<InventoryItem>()).ToList();

				foreach (ShipmentPackage package in shipmentPackages)
				{
					foreach (ShipmentPackageLineDetail packageDetail in package.ShipmentPackageLineDetails)
					{
						ShipmentDetail shipmentDetail = impl.Details.First(shipmentDetail => shipmentDetail.LineNbr.Value == packageDetail.LineNumber.Value);

						SalesOrderDetail orderLine = order.Local.Details.FirstOrDefault(d =>
							order.Local.OrderType.Value == shipmentDetail.OrderType.Value &&
							order.Local.OrderNbr.Value == shipmentDetail.OrderNbr.Value &&
							d.LineNbr.Value == shipmentDetail.OrderLineNbr.Value);

						//skip shipment line that is not from this order
						if (orderLine == null) continue;


						DetailInfo detailInfo = order.Details
							.FirstOrDefault(detailInfo => (detailInfo.EntityType == BCEntitiesAttribute.OrderLine || detailInfo.EntityType == BCEntitiesAttribute.GiftWrapOrderLine)
															&& detailInfo.LocalID == orderLine.NoteID.Value);

						if (detailInfo is null)
							continue;

						bool isNonStockKit = orderItems
							.Any(orderItem => orderItem.InventoryCD.Trim() == shipmentDetail.InventoryID.Value
							&& IsNonStockKit(orderItem));


						AmazonShipmentLineDetails shipmentLineDetail = new AmazonShipmentLineDetails();
						shipmentLineDetail.AmazonOrderID = order.ExternID;
						shipmentLineDetail.PackageId = package.NoteID.Value;
						shipmentLineDetail.AmazonOrderItemCode = detailInfo.ExternID;

						if (isNonStockKit)
						{
							//Skip shipping line if its ShippingQty is 0, because the non-stock kit has shipped in other package
							if (shipmentDetail.ShippedQty.Value == decimal.Zero) continue;

							// create a shipment to export to Amazon as if all non-stock kits are in a same box/package
							// thus, only first package is used
							shipmentLineDetail.Quantity = (int)(shipmentDetail.ShippedQty?.Value ?? decimal.Zero);
							shipmentDetail.ShippedQty = decimal.Zero.ValueField();
						}
						else
						{

							shipmentLineDetail.Quantity = (packageDetail.InUnit != null && packageDetail.InUnit.FromUnit == shipmentDetail.UOM.Value)
								? (int)INUnitAttribute.ConvertValue(packageDetail.Quantity.Value, packageDetail.InUnit, INPrecision.QUANTITY, true)
								: (int)packageDetail.Quantity.Value;
						}

						if (shipmentLineDetail.Quantity > 0)
							shipmentLineDetails.Add(shipmentLineDetail);
					}
				}

				// forming shipments in the format expected by Amazon
				// firstly, group shipment details by order
				var shipmentDetailsByOrders = shipmentLineDetails.GroupBy(x => x.AmazonOrderID).ToDictionary(x => x.Key, x => x.ToList());
				foreach (var shipmentDetailsByOrder in shipmentDetailsByOrders)
				{
					// secondly, group shipment details by package in each order
					var shipmentDetailsByPackages = shipmentDetailsByOrder.Value.GroupBy(x => x.PackageId).ToDictionary(x => x.Key, x => x.ToList());

					// thirdly, form an Amazon-formatted shipment for each package of each order
					foreach (var shipmentDetailsByPackage in shipmentDetailsByPackages)
					{
						var currentPackage = shipmentPackages.FirstOrDefault(x => x.NoteID?.Value == shipmentDetailsByPackage.Key);

						if (string.IsNullOrEmpty(currentPackage.TrackingNbr?.Value))
							throw new PXException(AmazonMessages.ShipmentWithoutBoxes, impl.ShipmentNbr.Value);

						var trackingNbr = currentPackage.TrackingNbr.Value;
						OrderFulfillment orderFulfillment = new OrderFulfillment();
						orderFulfillment.AmazonOrderID = shipmentDetailsByOrder.Key;
						// comment the assigning value for MerchantFulfillmentID out as seem not required for now
						//orderFulfillment.MerchantFulfillmentID = obj.SyncID.ToString() + currentPackage.LineNbr.Value;
						orderFulfillment.FulfillmentDate = GetShipmentDate(impl);
						var fulfillmentData = new FulfillmentData();
						fulfillmentData.ShipperTrackingNumber = currentPackage.TrackingNbr.Value;
						fulfillmentData.CarrierName = GetCarrierName(impl.ShipVia?.Value, GetBindingExt<BCBindingAmazon>().ShipViaCodesToCarriers);
						fulfillmentData.ShippingMethod = GetShippingMethod(impl.ShipVia?.Value, GetBindingExt<BCBindingAmazon>().ShipViaCodesToCarrierServices);

						var listOfItems = new List<Item>();
						var currentShipmentLineDetails = shipmentDetailsByPackage.Value;
						foreach (var shipmentLineDetail in currentShipmentLineDetails)
						{
							var item = new Item();
							item.AmazonOrderItemCode = shipmentLineDetail.AmazonOrderItemCode;
							item.Quantity = shipmentLineDetail.Quantity;
							listOfItems.Add(item);
						}

						orderFulfillment.Item = listOfItems;
						orderFulfillment.FulfillmentData = fulfillmentData;

						amazonFulfillmentData.Fulfillments.Add(orderFulfillment);
					}
				}
			}
		}

		protected virtual string GetShipmentDate(Shipment shipment)
		{
			DateTime? shipmentDate = shipment.ShipmentDate?.Value;

			if (!shipmentDate.HasValue)
				throw new PXException(AmazonMessages.ShipmentWithoutDate, shipment.ShipmentNbr.Value);

			//
			// The temporary solution how to set correct date at the Amazon shipment through the API.
			//
			DateTime correctedShipmentDate = shipmentDate.Value.AddSeconds(DateTime.UtcNow.TimeOfDay.TotalSeconds);

			return correctedShipmentDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
		}

		protected virtual string GetCarrierName(string shipVia, string ShipViaToCarrier)
		{
			if (string.IsNullOrEmpty(shipVia))
				throw new PXArgumentException(nameof(PX.Objects.SO.SOShipment.ShipVia), ErrorMessages.ArgumentNullException);

			if (string.IsNullOrEmpty(ShipViaToCarrier))
				throw new PXArgumentException(nameof(BCBindingAmazon.shipViaCodesToCarriers), ErrorMessages.ArgumentNullException);

			string substitutedShipVia = helper.GetSubstituteLocalByExtern(ShipViaToCarrier, shipVia, null);

			if (string.IsNullOrEmpty(substitutedShipVia))
			{
				throw new PXException(AmazonMessages.NoCarrierIsMapped, shipVia, ShipViaToCarrier);
			}

			return substitutedShipVia;
		}

		protected virtual string GetShippingMethod(string shipVia, string shipViaToCarrierServiceSubstitutionList)
		{
			if (string.IsNullOrEmpty(shipVia))
				throw new PXArgumentException(nameof(PX.Objects.SO.SOShipment.ShipVia), ErrorMessages.ArgumentNullException);

			if (string.IsNullOrEmpty(shipViaToCarrierServiceSubstitutionList))
				throw new PXArgumentException(nameof(BCBindingAmazon.shipViaCodesToCarrierServices), ErrorMessages.ArgumentNullException);

			string substitutedCarrierService = helper.GetSubstituteLocalByExtern(shipViaToCarrierServiceSubstitutionList, shipVia, null);
			if (string.IsNullOrEmpty(substitutedCarrierService))
			{
				throw new PXException(AmazonMessages.NoShippingServiceIsMapped, shipVia, shipViaToCarrierServiceSubstitutionList);
			}
			return substitutedCarrierService;
		}

		private void ValidateSplittedInventoryItems(ShipmentPackage package, Shipment shipment)
		{
			foreach (var packageDetals in package.ShipmentPackageLineDetails)
			{
				ShipmentDetail shipmentDetail = shipment.Details.First(shipmentDetail => shipmentDetail.LineNbr.Value == packageDetals.LineNumber.Value);
				decimal quantity = packageDetals.Quantity.Value;

				if (packageDetals.InUnit != null && packageDetals.InUnit.FromUnit == shipmentDetail.UOM.Value)
					quantity = INUnitAttribute.ConvertValue(quantity, packageDetals.InUnit, INPrecision.QUANTITY, true);

				if (quantity % 1 != 0)
					throw new PXException(AmazonMessages.ItemIsSplittedWithinDifferentPackages, shipment.ShipmentNbr.Value, shipmentDetail.InventoryID.Value);
			}
		}

		private void ValidateShippedItemsQuantity(List<ShipmentPackage> shipmentPackages, Shipment shipment)
		{
			var packageDetailsByInventoryItemGroups = shipmentPackages
				.SelectMany(shipmentPackage => shipmentPackage.ShipmentPackageLineDetails)
				.GroupBy(packageDetail => packageDetail.LineNumber.Value);

			foreach (var packageDetalsByInventoryItem in packageDetailsByInventoryItemGroups)
			{
				var firstPackageDetal = packageDetalsByInventoryItem.First();

				//
				// Get sum of all quantities of package details. They are stored in base UOM.
				//
				decimal quantity = packageDetalsByInventoryItem.Sum(package => package.Quantity.Value);
				ShipmentDetail shipmentDetail = shipment.Details.First(shipmentDetail => shipmentDetail.LineNbr.Value == firstPackageDetal.LineNumber.Value);

				//
				// Convert base UOM to the shipment line UOM.
				//
				if (firstPackageDetal.InUnit != null && firstPackageDetal.InUnit.FromUnit == shipmentDetail.UOM.Value)
					quantity = INUnitAttribute.ConvertValue(quantity, firstPackageDetal.InUnit, INPrecision.QUANTITY, true);

				if (quantity != shipmentDetail.ShippedQty.Value)
				{
					InventoryItem parentInventoryItem = SelectFrom<InventoryItem>.Where<InventoryItem.inventoryCD.IsEqual<@P.AsString>>.View.ReadOnly.SelectSingleBound(this, null, shipmentDetail.InventoryID.Value);
					if (IsNonStockKit(parentInventoryItem) == false)
						throw new PXException(AmazonMessages.ItemsWithoutBoxes, shipment.ShipmentNbr.Value, shipmentDetail.InventoryID.Value);
					ValidateNonStockKitsTrackingNumber(shipmentPackages, shipmentDetail, shipment.ShipmentNbr.Value);

					Dictionary<int, decimal> packageComponents = packageDetalsByInventoryItem
						.GroupBy(item => item.ItemID)
						.ToDictionary(
							keySelector: x => x.Key.Value,
							elementSelector: x => x.Sum(item => item.Quantity.Value));

					ValidateNonStockKitsQuantity(parentInventoryItem.InventoryID.Value, packageComponents, shipmentDetail, shipment.ShipmentNbr.Value);
				}
			}
		}

		protected virtual bool IsNonStockKit(InventoryItem orderItem) => orderItem?.KitItem == true && orderItem?.StkItem == false;

		/// <summary>
		/// Validates <paramref name="packageComponents"/> quantities according to <paramref name="shipmentDetail"/> Shipped Quantity.
		/// </summary>
		/// <param name="itemInventoryID">The referred Item Inventory ID</param>
		/// <param name="packageComponents">The dictionary components for all packages where item ID is the key and quantity is the value.</param>
		/// <param name="shipmentDetail">The Item's shipment detail which is being evaluated.</param>
		/// <param name="shipmentNumber">The shipment number which the <paramref name="shipmentDetail"/> is in.</param>
		/// <exception cref="PXException">Thrown when the number of items in all packages is invalid.</exception>
		protected virtual void ValidateNonStockKitsQuantity(int itemInventoryID, Dictionary<int, decimal> packageComponents, ShipmentDetail shipmentDetail, string shipmentNumber)
		{
			Dictionary<int, decimal> nonStockKitComponents = SelectFrom<INKitSpecStkDet>
						.Where<INKitSpecStkDet.kitInventoryID.IsEqual<@P.AsInt>>
						.View.ReadOnly
						.Select(this, itemInventoryID)
						.RowCast<INKitSpecStkDet>()
						.ToDictionary(
							keySelector: x => x.CompInventoryID.Value,
							elementSelector: x => x.DfltCompQty.Value);

			if (CalculateKitsQuantitiesPerPackage(nonStockKitComponents, packageComponents) != shipmentDetail.ShippedQty.Value)
				throw new PXException(AmazonMessages.StockKitsWithDifferentTrackingNumber, shipmentNumber, shipmentDetail.InventoryID.Value.Trim());
		}


		/// <summary>
		/// Validates Non Stock Kits tracking number for each package.
		/// </summary>
		/// <param name="shipmentPackages">List of packages for the shipment.</param>
		/// <param name="shipmentDetail">The Item's shipment detail which is being evaluated.</param>
		/// <param name="shipmentNumber">The shipment number which the <paramref name="shipmentDetail"/> is in.</param>
		/// <exception cref="PXException">Thrown when tracking numbers for the shipment is invalid.</exception>
		protected virtual void ValidateNonStockKitsTrackingNumber(List<ShipmentPackage> shipmentPackages, ShipmentDetail shipmentDetail, string shipmentNumber)
		{
			/*	All components of the non-stock kit to one package with the tracking number: Shipment export is successful.
				Some components of the non-stock kit are added to one package with the tracking number, and the rest are added to another box with the SAME tracking number as the first package: Shipment export is successful.
				Some components of the non-stock kit are added to one package with the tracking number, and the rest are added to another box with the different tracking number as the first package: Shipment export will fail.*/
			var listOfShipmentPackages = shipmentPackages
				.Where(package => package.ShipmentPackageLineDetails?.Any(shipmentLine => shipmentLine.LineNumber == shipmentDetail.LineNbr.Value) == true)
				.ToList();
			string trackingNumber = listOfShipmentPackages.FirstOrDefault(s => string.IsNullOrWhiteSpace(s?.TrackingNbr.Value) == false)?.TrackingNbr?.Value;

			if (string.IsNullOrWhiteSpace(trackingNumber) || listOfShipmentPackages.All(package => string.Equals(package.TrackingNbr.Value, trackingNumber)) == false)
				throw new PXException(AmazonMessages.StockKitsWithDifferentTrackingNumber, shipmentNumber, shipmentDetail.InventoryID.Value.Trim());
		}

		/// <summary>
		/// Calculates if one <paramref name="nonStockKitComponents"/> can be assembled by <paramref name="packageComponents"/>.
		/// </summary>
		/// <param name="nonStockKitComponents">The base composition of the kit</param>
		/// <param name="packageComponents">Current components in calculated package</param>
		/// <returns>True if can be assembled, otherwise false.</returns>
		protected virtual bool CanAssembleItem(Dictionary<int, decimal> nonStockKitComponents, Dictionary<int, decimal> packageComponents)
		{
			foreach (var nonStockKitcomponent in nonStockKitComponents)
			{
				if (packageComponents.TryGetValue(nonStockKitcomponent.Key, out decimal packageQty) == false || packageQty < nonStockKitcomponent.Value)
					return false;
			}

			return true;
		}

		/// <summary>
		/// Calculates the number of non stock kits in <paramref name="packageComponents"/>.
		/// </summary>
		/// <param name="nonStockKitComponents">The base composition of one kit.</param>
		/// <param name="packageComponents">Current components in calculated package.</param>
		/// <returns> Quantity of items calculated.</returns>
		protected virtual decimal CalculateKitsQuantitiesPerPackage(Dictionary<int, decimal> nonStockKitComponents, Dictionary<int, decimal> packageComponents)
		{
			decimal quantityPerPackage = decimal.Zero;
			Dictionary<int, decimal> packageComponentsCopy = new Dictionary<int, decimal>(packageComponents);

			while (CanAssembleItem(nonStockKitComponents, packageComponentsCopy))
			{
				foreach (var nonStockKitComponent in nonStockKitComponents)
					packageComponentsCopy[nonStockKitComponent.Key] -= nonStockKitComponent.Value;
				quantityPerPackage++;
			}

			return quantityPerPackage;
		}

		public override Task<MappedShipment> PullEntity(string externID, string externalInfo, CancellationToken cancellationToken = default)
		{
			return base.PullEntity(externID, externalInfo, cancellationToken);
		}

		public override Task<MappedShipment> PullEntity(Guid? localID, Dictionary<string, object> externalInfo, CancellationToken cancellationToken = default)
		{
			return base.PullEntity(localID, externalInfo, cancellationToken);
		}
		#endregion
	}
}
