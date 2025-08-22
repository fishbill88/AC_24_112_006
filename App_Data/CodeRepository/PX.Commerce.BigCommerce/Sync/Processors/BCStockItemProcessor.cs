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

using PX.Api.ContractBased.Models;
using PX.Commerce.BigCommerce.API.REST;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using Serilog.Context;
using PX.Objects.IN;
using PX.Objects.IN.RelatedItems;
using System;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using System.Threading.Tasks;
using System.Threading;


namespace PX.Commerce.BigCommerce
{
	public class BCStockItemEntityBucket : BCProductEntityBucket<MappedStockItem>
	{
		public override IMappedEntity[] PreProcessors { get => Categories.ToArray(); }
		public override IMappedEntity[] PostProcessors { get => StockItems.ToList<IMappedEntity>().Concat(Availabilities).ToArray(); }
		public List<MappedStockItem> StockItems = new List<MappedStockItem>();
		public List<MappedAvailability> Availabilities = new List<MappedAvailability>();
	}

	public class BCStockItemRestrictor : BCBaseRestrictor, IRestrictor
	{
		public virtual FilterResult RestrictExport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			#region StockItems
			return base.Restrict<MappedStockItem>(mapped, delegate (MappedStockItem obj)
			{
				if (obj.Local != null && obj.Local.TemplateItemID?.Value != null)
				{
					return new FilterResult(FilterStatus.Invalid,
						PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogStockSkippedVariant, obj.Local.InventoryID?.Value ?? obj.Local.SyncID.ToString()));
				}

				if (obj.Local != null && obj.Local.ExportToExternal?.Value == false)
				{
					return new FilterResult(FilterStatus.Invalid,
						PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogItemNoExport, obj.Local.InventoryID?.Value ?? obj.Local.SyncID.ToString()));
				}

				return null;
			});
			#endregion
		}

		public virtual FilterResult RestrictImport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return base.Restrict<MappedStockItem>(mapped, delegate(MappedStockItem obj)
			{
				ProductData external = obj.Extern;
				if (!external.BaseVariantId.HasValue)
				{
					return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogProductItemImportSkippedTemplateItem, external.Name));
				}

				if (mode != FilterMode.Merge && external.Type == ProductTypes.digital.ToString())
				{
					return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogStockImportSkippedNonStockItem, external.Name));
				}

				return null;
			});
		}
	}

	[BCProcessor(typeof(BCConnector), BCEntitiesAttribute.StockItem, BCCaptions.StockItem, 50,
		IsInternal = false,
		Direction = SyncDirection.Bidirect,
		PrimaryDirection = SyncDirection.Export,
		PrimarySystem = PrimarySystem.Local,
		PrimaryGraph = typeof(PX.Objects.IN.InventoryItemMaint),
		ExternTypes = new Type[] { typeof(ProductData) },
		LocalTypes = new Type[] { typeof(StockItem) },
		AcumaticaPrimaryType = typeof(PX.Objects.IN.InventoryItem),
		AcumaticaPrimarySelect = typeof(Search<PX.Objects.IN.InventoryItem.inventoryCD, Where<PX.Objects.IN.InventoryItem.stkItem, Equal<True>>>),
		URL = "products/{0}/edit",
		Requires = new string[] { }
	)]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.ProductVideo, EntityName = BCCaptions.ProductVideo, AcumaticaType = typeof(BCInventoryFileUrls))]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.RelatedItem, EntityName = BCCaptions.RelatedItem, AcumaticaType = typeof(PX.Objects.IN.InventoryItem))]
	[BCProcessorRealtime(PushSupported = true, HookSupported = false,
		PushSources = new String[] { "BC-PUSH-Stocks" }, PushDestination = BCConstants.PushNotificationDestination,
		WebHookType = typeof(WebHookProduct),
		WebHooks = new String[]
		{
			"store/product/created",
			"store/product/updated",
			"store/product/deleted"
		})]
	[BCProcessorExternCustomField(BCConstants.CustomFields, BigCommerceCaptions.CustomFields, nameof(ProductData.CustomFields), typeof(ProductData))]
	public class BCStockItemProcessor : BCProductProcessor<BCStockItemProcessor, BCStockItemEntityBucket, MappedStockItem, ProductData, StockItem>
	{
		#region Constructor
		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation, cancellationToken);
		}
		#endregion

		#region Common
		public override async Task<MappedStockItem> PullEntity(Guid? localID, Dictionary<string, object> externalInfo, CancellationToken cancellationToken = default)
		{
			StockItem impl = cbapi.GetByID(localID,
				new StockItem()
				{
					ReturnBehavior = ReturnBehavior.OnlySpecified,
					Attributes = new List<AttributeValue>() { new AttributeValue() },
					Categories = new List<CategoryStockItem>() { new CategoryStockItem() },
					CrossReferences = new List<InventoryItemCrossReference>() { new InventoryItemCrossReference() },
					VendorDetails = new List<ProductItemVendorDetail>() { new ProductItemVendorDetail() },
					FileURLs = new List<InventoryFileUrls>() { new InventoryFileUrls() }

				});
			if (impl == null) return null;

			MappedStockItem obj = new MappedStockItem(impl, impl.SyncID, impl.SyncTime);

			return obj;
		}
		public override async Task<MappedStockItem> PullEntity(String externID, String externalInfo, CancellationToken cancellationToken = default)
		{
			ProductData data = await productDataProvider.GetByID(externID, new FilterProducts { Include = "variants,options" });
			if (data == null) return null;

			MappedStockItem obj = new MappedStockItem(data, data.Id?.ToString(), data.Name, data.DateModifiedUT.ToDate());

			return obj;
		}

		/// <summary>
		/// Initialize a new object of the entity to be used to Fetch bucket
		/// </summary>
		/// <returns>The initialized entity</returns>
		protected override StockItem CreateEntityForFetch()
		{
			StockItem entity = new StockItem();

			entity.InventoryID = new StringReturn();
			entity.TemplateItemID = new StringReturn();
			entity.Categories = new List<CategoryStockItem>() { new CategoryStockItem() { CategoryID = new IntReturn() } };
			entity.ExportToExternal = new BooleanReturn();

			return entity;
		}

		/// <summary>
		/// Initialize a new object of the entity to be used to Get bucket
		/// </summary>
		/// <returns>The initialized entity</returns>
		protected override StockItem CreateEntityForGet()
		{
			StockItem entity = new StockItem();

			entity.ReturnBehavior = ReturnBehavior.OnlySpecified;
			entity.Attributes = new List<AttributeValue>() { new AttributeValue() };
			entity.Categories = new List<CategoryStockItem>() { new CategoryStockItem() };
			entity.CrossReferences = new List<InventoryItemCrossReference>() { new InventoryItemCrossReference() };
			entity.VendorDetails = new List<ProductItemVendorDetail>() { new ProductItemVendorDetail() };
			entity.FileURLs = new List<InventoryFileUrls>() { new InventoryFileUrls() };

			return entity;
		}

		/// <summary>
		/// Creates a mapped entity for the passed entity
		/// </summary>
		/// <param name="entity">The entity to create the mapped entity from</param>
		/// <param name="syncId">The sync id of the entity</param>
		/// <param name="syncTime">The timestamp of the last modification</param>
		/// <returns>The mapped entity</returns>
		protected override MappedStockItem CreateMappedEntity(StockItem entity, Guid? syncId, DateTime? syncTime)
		{
			return new MappedStockItem(entity, syncId, syncTime);
		}

		#endregion	

		#region Import

		public override async Task<PullSimilarResult<MappedStockItem>> PullSimilar(IExternEntity entity, CancellationToken cancellationToken = default)
		{

			string uniqueStr = ((ProductData)entity)?.Sku;

			if (string.IsNullOrEmpty(uniqueStr))
				return null;

			var products = ProductImportService.FindSimilarProductForSku<StockItem>(uniqueStr).ToList();
			if (products == null || !products.Any())
				return null;

			return new PullSimilarResult<MappedStockItem>
			{
				UniqueField = uniqueStr,
				Entities = products.Select(item => new MappedStockItem((StockItem)item, item.Id, item.LastModifiedDateTime.Value)).ToList()
			};
		}
		public override async Task FetchBucketsForImport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			FilterProducts filter = new FilterProducts
			{
				Type = ProductTypes.physical.ToString(),
				Include = "variants,options",
				MinDateModified = !minDateTime.HasValue ? null : minDateTime.Value.AddDays(-1),
				MaxDateModified = !maxDateTime.HasValue ? null : maxDateTime.Value.AddDays(1)
			};

			// to force the code to run asynchronously and keep UI responsive.
			//In some case it runs synchronously especially when using IAsyncEnumerable
			await Task.Yield();
			await foreach (ProductData data in productDataProvider.GetAll(filter, cancellationToken))
			{

				if (data.BaseVariantId == null) continue; 
				BCStockItemEntityBucket bucket = CreateBucket();

				MappedStockItem obj = bucket.Product = bucket.Product.Set(data, data.Id?.ToString(), data.Name, data.DateModifiedUT.ToDate());
				EntityStatus status = EnsureStatus(obj, SyncDirection.Import);
			}
		}
		public override async Task<EntityStatus> GetBucketForImport(BCStockItemEntityBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			FilterProducts filter = new FilterProducts { Include = "images,modifiers" };
			ProductData data = await productDataProvider.GetByID(syncstatus.ExternID, filter);
			if (data == null) return EntityStatus.None;

			MappedStockItem obj = bucket.Product = bucket.Product.Set(data, data.Id?.ToString(), data.Name, data.DateModifiedUT.ToDate());
			EntityStatus status = EnsureStatus(obj, SyncDirection.Import);

			await base.AddSalesCategoryBucketForImport(bucket);

			return status;
		}

		public override async Task MapBucketImport(BCStockItemEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			ProductData external = bucket.Product.Extern;
			if (external == null && bucket.Product.Local != null)
				return;

			MappedStockItem existingMapped = existing as MappedStockItem;
			StockItem existingData = existingMapped?.Local;

			Objects.BCBindingExt bindingExt = GetBindingExt<Objects.BCBindingExt>();
			StockItem local = MapProductImport<StockItem>(external, existingData, bindingExt.StockItemClassID);
			local.ItemStatus = GetItemStatus(external).ValueField();

			if (existingData == null)
			{
				local.NotAvailable = BCItemNotAvailModes.StoreDefault.ValueField();
				local.Availability = GetProductItemAvailabilityForStockItems(external).ValueField();
			}

			local.DimensionWeight = new DecimalValue() { Value = external.Weight };
			MapSalesCategories(bucket, existingData, external, local);

			GetHelper<BCHelper>().AddExternalSku(existingData, local, external.Sku);
			GetHelper<BCHelper>().AddGITN(existingData, local, external.GTIN);

			bucket.Product.Local = local;
		}

		/// <summary>
		/// The method checks the availability of the external product and returns the status of the local product.
		/// The local product status will be set to inactive if the external product is disabled. In other cases, the status will be set to active.
		/// </summary>
		/// <param name="external"></param>
		/// <returns></returns>
		public virtual string GetItemStatus(ProductData external) =>
			external.Availability switch
			{
				BigCommerceConstants.AvailabilityDisabled => PX.Objects.IN.Messages.Inactive,
				_ => PX.Objects.IN.Messages.Active,
			};

		/// <inheritdoc/>
		public override async Task SaveBucketImport(BCStockItemEntityBucket bucket, IMappedEntity existing, String operation, CancellationToken cancellationToken = default)
		{
			MappedStockItem obj = bucket.Product;
			var external = bucket.Product.Extern;
			
			StockItem impl = cbapi.Put<StockItem>(obj.Local, obj.LocalID);

			bucket.Product.AddLocal(impl, impl.SyncID, impl.SyncTime);
			UpdateStatus(obj, operation);
		}
		#endregion

		#region Export
		public override async Task<PullSimilarResult<MappedStockItem>> PullSimilar(ILocalEntity entity, CancellationToken cancellationToken = default)
		{
			List<ProductData> datas = new List<ProductData>();
			var uniqueField = await PullSimilar(((StockItem)entity)?.Description?.Value, ((StockItem)entity)?.InventoryID?.Value, datas, cancellationToken);
			return new PullSimilarResult<MappedStockItem>() { UniqueField = uniqueField, Entities = datas == null ? null : datas.Select(data => new MappedStockItem(data, data.Id.ToString(), data.Name, data.DateModifiedUT.ToDate())) };
		}

		public override async Task MapBucketExport(BCStockItemEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			StockItem local = bucket.Product.Local;
			ProductData external = bucket.Product.Extern = new ProductData();

			MappedStockItem existingMapped = existing as MappedStockItem;
			ProductData existingData = existingMapped?.Extern;

			await MapInventoryItem(bucket, local, external, existingData);
			MapCustomFields(local, external);
			bucket.Product.Extern.RelatedProducts = MapRelatedItems(bucket.Product);
			MapVisibility(local, external);
			MapAvailability(local, external, existingData);
			MapCategories(local, external);
		}

		public virtual async Task MapInventoryItem(BCStockItemEntityBucket bucket, StockItem local, ProductData external, ProductData existingData)
		{
			external.Name = local.Description?.Value;
			external.Description = GetHelper<BCHelper>().ClearHTMLContent(local.Content?.Value);
			external.Type = ProductsType.Physical.ToEnumMemberAttrValue();
			external.Price = await GetHelper<BCHelper>().RoundToStoreSetting(local.CurySpecificPrice?.Value);
			external.Weight = local.DimensionWeight.Value;
			external.CostPrice = local.CurrentStdCost.Value;
			external.RetailPrice = await GetHelper<BCHelper>().RoundToStoreSetting(local.CurySpecificMSRP?.Value);
			external.BinPickingNumber = local.DefaultIssueLocationID?.Value;
			external.Sku = this.GetProductSku(bucket.Product.Local, bucket.ExternalProduct);
			external.TaxClassId = taxClasses?.Find(i => string.Equals(i.Name, GetHelper<BCHelper>().GetSubstituteLocalByExtern(GetBindingExt<Objects.BCBindingExt>().TaxCategorySubstitutionListID, local.TaxCategory?.Value, String.Empty)))?.Id;
		}

		public virtual void MapCustomFields(StockItem local, ProductData external)
		{
			external.PageTitle = local.PageTitle?.Value;
			external.MetaDescription = local.MetaDescription?.Value;
			external.MetaKeywords = local.MetaKeywords?.Value != null ? local.MetaKeywords?.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null;
			external.SearchKeywords = local.SearchKeywords?.Value;
			var vendor = local.VendorDetails?.FirstOrDefault(v => v.Default?.Value == true);
			if (vendor != null)
				external.MPN = local.CrossReferences?.FirstOrDefault(x => x.AlternateType?.Value == BCCaptions.VendorPartNumber && x.VendorOrCustomer?.Value == vendor.VendorID?.Value)?.AlternateID?.Value;
			if (local.CustomURL?.Value != null) external.CustomUrl = new ProductCustomUrl() { Url = local.CustomURL?.Value, IsCustomized = true };

			external.Upc = GetCrossReferenceValueWithFallBack(local, local.SalesUOM, local.BaseUOM, PX.Objects.IN.Messages.Barcode);
			external.GTIN = GetCrossReferenceValueWithFallBack(local, local.SalesUOM, local.BaseUOM, PX.Objects.IN.Messages.GIN);		
		}

		public virtual void MapVisibility(StockItem local, ProductData external)
		{
			string visibility = local?.Visibility?.Value;
			if (visibility == null || visibility == BCCaptions.StoreDefault) visibility = BCItemVisibility.Convert(GetBindingExt<Objects.BCBindingExt>().Visibility);
			switch (visibility)
			{
				case BCCaptions.Visible:
					{
						external.IsVisible = true;
						external.IsFeatured = false;
						break;
					}
				case BCCaptions.Featured:
					{
						external.IsVisible = true;
						external.IsFeatured = true;
						break;
					}
				case BCCaptions.Invisible:
				default:
					{
						external.IsFeatured = false;
						external.IsVisible = false;
						break;
					}
			}
		}

		public virtual void MapAvailability(StockItem local, ProductData external, ProductData existingProduct)
		{
			string availability = local.Availability?.Value;

			if (availability == null || availability == BCCaptions.StoreDefault)
			{
				availability = BCItemAvailabilities.Convert(GetBindingExt<Objects.BCBindingExt>().Availability);
			}
			string notAvailable = local.NotAvailable?.Value;

			if (notAvailable == null || notAvailable == BCCaptions.StoreDefault)
			{
				notAvailable = BCItemNotAvailModes.Convert(GetBindingExt<Objects.BCBindingExt>().NotAvailMode);
			}

			if (local.ItemStatus?.Value == PX.Objects.IN.Messages.Active || local.ItemStatus?.Value == PX.Objects.IN.Messages.NoPurchases || local.ItemStatus?.Value == PX.Objects.IN.Messages.NoRequest)
			{
				if (availability == BCCaptions.AvailableTrack)
				{
					external.Availability = BigCommerceConstants.AvailabilityAvailable;
					external.InventoryTracking = BigCommerceConstants.InventoryTrackingProduct;

					//If there is no existing product default to enable.
					if (existingProduct?.InventoryLevel <= 0)
					{
						if (notAvailable == BCCaptions.DisableItem)
						{
							external.Availability = BigCommerceConstants.AvailabilityDisabled;
						}
						else if (notAvailable == BCCaptions.PreOrderItem || notAvailable == BCCaptions.ContinueSellingItem || notAvailable == BCCaptions.EnableSellingItem)
						{
							external.Availability = BigCommerceConstants.AvailabilityPreOrder;
							external.InventoryTracking = BigCommerceConstants.InventoryTrackingNone;
						}
						else if (notAvailable == BCCaptions.DoNothing || notAvailable == BCCaptions.DoNotUpdate)
						{
							//If there is no existing product default to available
							external.Availability = existingProduct?.Availability ?? BigCommerceConstants.AvailabilityAvailable;
						}
					}
				}
				else if (availability == BCCaptions.AvailableSkip)
				{
					external.Availability = BigCommerceConstants.AvailabilityAvailable;
					external.InventoryTracking = BigCommerceConstants.InventoryTrackingNone;
				}
				else if (availability == BCCaptions.PreOrder)
				{
					external.Availability = BigCommerceConstants.AvailabilityPreOrder;
					external.InventoryTracking = BigCommerceConstants.InventoryTrackingNone;
				}
				else if (availability == BCCaptions.DisableItem)
				{
					external.Availability = BigCommerceConstants.AvailabilityDisabled;
					external.InventoryTracking = BigCommerceConstants.InventoryTrackingNone;
				}
				else if (availability == BCCaptions.DoNotUpdate)
				{
					external.Availability = existingProduct?.Availability ?? BigCommerceConstants.AvailabilityAvailable;
					external.InventoryTracking = existingProduct?.InventoryTracking ?? BigCommerceConstants.InventoryTrackingNone;
				}
			}
			else if (local.ItemStatus?.Value == PX.Objects.IN.Messages.Inactive || local.ItemStatus?.Value == PX.Objects.IN.Messages.NoSales || local.ItemStatus?.Value == PX.Objects.IN.Messages.ToDelete)
			{
				external.Availability = BigCommerceConstants.AvailabilityDisabled;
				external.InventoryTracking = BigCommerceConstants.InventoryTrackingNone;
			}
		}

		public virtual void MapCategories(StockItem local, ProductData external)
		{
			if (GetEntity(BCEntitiesAttribute.SalesCategory)?.IsActive == true)
			{
				if (external.Categories == null) external.Categories = new List<int>();

				foreach (PXResult<PX.Objects.IN.INCategory, PX.Objects.IN.INItemCategory, PX.Objects.IN.InventoryItem, BCSyncStatus> result in PXSelectJoin<PX.Objects.IN.INCategory,
						InnerJoin<PX.Objects.IN.INItemCategory, On<PX.Objects.IN.INItemCategory.categoryID, Equal<PX.Objects.IN.INCategory.categoryID>>,
						InnerJoin<PX.Objects.IN.InventoryItem, On<PX.Objects.IN.InventoryItem.inventoryID, Equal<PX.Objects.IN.INItemCategory.inventoryID>>,
						LeftJoin<BCSyncStatus, On<PX.Objects.IN.INCategory.noteID, Equal<BCSyncStatus.localID>>>>>,
						Where<BCSyncStatus.connectorType, Equal<Current<BCEntity.connectorType>>,
							And<BCSyncStatus.bindingID, Equal<Current<BCEntity.bindingID>>,
							And<BCSyncStatus.entityType, Equal<Required<BCEntity.entityType>>,
							And<PX.Objects.IN.InventoryItem.noteID, Equal<Required<PX.Objects.IN.InventoryItem.noteID>>>>>>>
								.Select(this, BCEntitiesAttribute.SalesCategory, local.SyncID))
				{
					BCSyncStatus status = result.GetItem<BCSyncStatus>();
					if (status == null || status.ExternID == null) continue;

					external.Categories.Add(status.ExternID.ToInt().Value);
				}
				if ((external.Categories ?? Enumerable.Empty<int>()).Empty_())
				{
					String categories = GetBindingExt<Objects.BCBindingExt>().StockSalesCategoriesIDs;
					if (!String.IsNullOrEmpty(categories))
					{
						Int32?[] categoriesArray = categories.Split(',').Select(c => { return Int32.TryParse(c, out Int32 i) ? (int?)i : null; }).Where(i => i != null).ToArray();

						foreach (BCSyncStatus status in PXSelectJoin<BCSyncStatus,
							LeftJoin<PX.Objects.IN.INCategory, On<PX.Objects.IN.INCategory.noteID, Equal<BCSyncStatus.localID>>>,
							Where<BCSyncStatus.connectorType, Equal<Current<BCEntity.connectorType>>,
								And<BCSyncStatus.bindingID, Equal<Current<BCEntity.bindingID>>,
								And<BCSyncStatus.entityType, Equal<Required<BCEntity.entityType>>,
								And<PX.Objects.IN.INCategory.categoryID, In<Required<PX.Objects.IN.INCategory.categoryID>>>>>>>
								.Select(this, BCEntitiesAttribute.SalesCategory, categoriesArray))
						{
							if (status?.ExternID == null)
							{
								continue;
							}

							external.Categories.Add(status.ExternID.ToInt().Value);
						}
					}
				}
			}
		}



		public override object GetAttribute(BCStockItemEntityBucket bucket, string attributeID)
		{
			return GetAttribute(bucket.Product?.Local, attributeID);
		}

		public override void AddAttributeValue(BCStockItemEntityBucket bucket, string attributeID, object attributeValue)
		{
			MappedStockItem obj = bucket.Product;
			StockItem impl = obj.Local;
			impl.Attributes = impl.Attributes ?? new List<AttributeValue>();
			AttributeValue attributeDetail = new AttributeValue();
			attributeDetail.AttributeID = new StringValue() { Value = attributeID };
			attributeDetail.Value = new StringValue() { Value = attributeValue?.ToString() };
			attributeDetail.ValueDescription = new StringValue() { Value = attributeValue.ToString() };
			impl.Attributes.Add(attributeDetail);
		}

		public override async Task SaveBucketExport(BCStockItemEntityBucket bucket, IMappedEntity existing, String operation, CancellationToken cancellationToken = default)
		{
			MappedStockItem obj = bucket.Product;

			ProductData data = null;

			obj.Extern.CustomFieldsData = await ExportCustomFields(obj, obj.Extern.CustomFields, data, cancellationToken);

			if (obj.ExternID == null)
				data = await productDataProvider.Create(obj.Extern);
			else
				data = await productDataProvider.Update(obj.Extern, obj.ExternID);

			obj.AddExtern(data, data.Id?.ToString(), data.Name, data.DateModifiedUT.ToDate());

			await SaveImages(obj, obj.Local?.FileURLs, cancellationToken);
			await SaveVideos(obj, obj.Local?.FileURLs);

			UpdateStatus(obj, operation);
			if (data != null)
				await UpdateRelatedItems(obj);
		}

		#endregion
	}
}
