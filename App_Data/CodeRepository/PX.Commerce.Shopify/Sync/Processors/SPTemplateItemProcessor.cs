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
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Commerce.Shopify.API.REST;
using PX.Commerce.Shopify.Sync.Processors.Utility;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Shopify
{
	public class SPTemplateItemEntityBucket : SPProductEntityBucket<MappedTemplateItem>
	{
		public new Dictionary<string, Tuple<long?, string, InventoryPolicy?>> VariantMappings = new Dictionary<string, Tuple<long?, string, InventoryPolicy?>>();
		public List<MappedStockItemVariant> StockItemVariants = new List<MappedStockItemVariant>();
		public List<MappedNonStockItemVariant> NonStockItemVariants = new List<MappedNonStockItemVariant>();

		/// <summary>
		/// Gets the external variants by variant id.
		/// </summary>
		public Dictionary<long, ProductVariantData> ExternalVariants { get; } = new Dictionary<long, ProductVariantData>();
	}

	public class SPTemplateItemRestrictor : BCBaseRestrictor, IRestrictor
	{
		public virtual FilterResult RestrictExport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return base.Restrict<MappedTemplateItem>(mapped, delegate (MappedTemplateItem obj)
			{
				if (obj.Local != null && obj.Local.ExportToExternal?.Value == false)
				{
					return new FilterResult(FilterStatus.Invalid,
						PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogItemNoExport, obj.Local.InventoryID?.Value ?? obj.Local.SyncID.ToString()));
				}

				if (mode != FilterMode.Merge
					&& obj.Local != null && (obj.Local.Matrix == null || obj.Local.Matrix?.Count == 0))
				{
					return new FilterResult(FilterStatus.Invalid,
						PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogTemplateSkippedNoMatrix, obj.Local.InventoryID?.Value ?? obj.Local.SyncID.ToString()));
				}

				return null;
			});
		}

		public virtual FilterResult RestrictImport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return base.Restrict<MappedTemplateItem>(mapped, delegate (MappedTemplateItem obj)
			{
				ProductData external = obj.Extern;

				if (external.Variants.Count == 1 && external.Options!=null && external.Options.Count == 1 && external.Options[0].Name == ShopifyConstants.RegularProductDefaultOptionName)
				{

					if (external.Variants.Any(v => v.RequiresShipping == false))
					{
						return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogTemplateImportSkippedNonStockItem, external.Title));
					}

					if (external.Variants.Any(v => v.RequiresShipping == true))
					{
						return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogTemplateImportSkippedStockItem, external.Title));
					}					
				}

				if (external.Status == ProductStatus.Archived)
				{
					return new FilterResult(FilterStatus.Filtered, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.PIInvalidProductStatus, external.Title));
				}

				if (obj.Local != null && obj.OriginalStatus != null
					&& obj.Local.NoteID?.Value == obj.OriginalStatus.LocalID
					&& obj.OriginalStatus.ExternID != external.Id.ToString())
				{
					var sku = obj.Extern.Variants.FirstOrDefault()?.Sku;
					var localInventoryCD = obj.Local;
					return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.PICannotImportBecauseOfDuplicateSku, external.Id, sku, localInventoryCD));
				}

				return null;
			});	
		}
	}

	[BCProcessor(typeof(SPConnector), BCEntitiesAttribute.ProductWithVariant, BCCaptions.TemplateItem, 50,
		IsInternal = false,
		Direction = SyncDirection.Bidirect,
		PrimaryDirection = SyncDirection.Export,
		PrimarySystem = PrimarySystem.Local,
		PrimaryGraph = typeof(PX.Objects.IN.InventoryItemMaint),
		ExternTypes = new Type[] { typeof(ProductData) },
		LocalTypes = new Type[] { typeof(TemplateItems) },
		AcumaticaPrimaryType = typeof(PX.Objects.IN.InventoryItem),
		AcumaticaPrimarySelect = typeof(Search<PX.Objects.IN.InventoryItem.inventoryCD, Where<PX.Objects.IN.InventoryItem.isTemplate, Equal<True>>>),
		AcumaticaFeaturesSet = typeof(FeaturesSet.matrixItem),
		URL = "products/{0}",
		Requires = new string[] { }
	)]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.ProductVideo, EntityName = BCCaptions.ProductVideo, AcumaticaType = typeof(BCInventoryFileUrls))]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.RelatedItem, EntityName = BCCaptions.RelatedItem, AcumaticaType = typeof(PX.Objects.IN.InventoryItem))]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.ProductOption, EntityName = BCCaptions.ProductOption, AcumaticaType = typeof(PX.Objects.CS.CSAttribute))]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.ProductOptionValue, EntityName = BCCaptions.ProductOption, AcumaticaType = typeof(PX.Objects.CS.CSAttributeDetail))]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.Variant, EntityName = BCCaptions.Variant, AcumaticaType = typeof(PX.Objects.IN.InventoryItem))]
	[BCProcessorRealtime(PushSupported = true, HookSupported = false,
		PushSources = new String[] { "BC-PUSH-Variants" },
		PushDestination = BCConstants.PushNotificationDestination,
		WebHookType = typeof(WebHookMessage),
		WebHooks = new String[]
		{
			"products/create",
			"products/delete",
			"products/update"
		})]
	[BCProcessorExternCustomField(BCConstants.ShopifyMetaFields, ShopifyCaptions.Metafields, nameof(ProductData.Metafields), typeof(ProductData), writeAsCollection: false)]
	[BCProcessorExternCustomField(BCConstants.ShopifyMetaFields, ShopifyCaptions.Metafields, nameof(ProductVariantData.VariantMetafields), typeof(ProductVariantData), writeAsCollection: true)]
	[BCProcessorExternCustomField(BCAPICaptions.Matrix, BCAPICaptions.Matrix, nameof(TemplateItems.Matrix), typeof(TemplateItems), readAsCollection: true, writeAsCollection: true)]
	public class SPTemplateItemProcessor : SPProductProcessor<SPTemplateItemProcessor, SPTemplateItemEntityBucket, MappedTemplateItem, ProductData, TemplateItems>
	{
		private IAvailabilityProvider _stockItemAvailabilityProvider;
		private IAvailabilityProvider _nonStockItemAvailabilityProvider;

		protected TemplateItemsMappingService templateItemsMappingService { get; set; }

		#region Constructor
		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation, cancellationToken);
			templateItemsMappingService = GetHelper<CommerceHelper>().GetExtension<TemplateItemsMappingService>();
			_stockItemAvailabilityProvider = this.AvailabilityProviderFactory.CreateInstance<StockItemAvailabilityProvider>();
			_nonStockItemAvailabilityProvider = this.AvailabilityProviderFactory.CreateInstance<NonStockItemAvailabilityProvider>();
		}
		#endregion

		#region Common
		public override async Task<MappedTemplateItem> PullEntity(Guid? localID, Dictionary<string, object> externalInfo, CancellationToken cancellationToken = default)
		{
			TemplateItems impl = cbapi.GetByID(localID,
				new TemplateItems()
				{
					ReturnBehavior = ReturnBehavior.OnlySpecified,
					Attributes = new List<AttributeValue>() { new AttributeValue() },
					Categories = new List<CategoryStockItem>() { new CategoryStockItem() },
					FileURLs = new List<InventoryFileUrls>() { new InventoryFileUrls() },
					Matrix = new List<ProductItem>() { new ProductItem() }
				});
			if (impl == null) return null;

			MappedTemplateItem obj = new MappedTemplateItem(impl, impl.SyncID, impl.SyncTime);

			return obj;
		}
		public override async Task<MappedTemplateItem> PullEntity(String externID, String externalInfo, CancellationToken cancellationToken = default)
		{
			ProductData data = await productDataProvider.GetByID(externID);
			if (data == null) return null;

			MappedTemplateItem obj = new MappedTemplateItem(data, data.Id?.ToString(), data.Title, data.DateModifiedAt.ToDate(false));

			return obj;
		}

		/// <summary>
		/// Initialize a new object of the entity to be used to Fetch bucket
		/// </summary>
		/// <returns>The initialized entity</returns>
		protected override TemplateItems CreateEntityForFetch()
		{
			TemplateItems entity = new TemplateItems();

			entity.InventoryID = new StringReturn();
			entity.IsStockItem = new BooleanReturn();
			entity.Matrix = new List<ProductItem>() { new ProductItem() { InventoryID = new StringReturn() } };
			entity.Categories = new List<CategoryStockItem>() { new CategoryStockItem() { CategoryID = new IntReturn() } };
			entity.ExportToExternal = new BooleanReturn();
			entity.VendorDetails = new List<ProductItemVendorDetail>() { new ProductItemVendorDetail() };
			entity.ItemType = new StringReturn();

			return entity;
		}

		/// <summary>
		/// Initialize a new object of the entity to be used to Get bucket
		/// </summary>
		///<remarks><see cref="TemplateItems.Matrix"/> field are fetched separately.</remarks>
		/// <returns>The initialized entity</returns>
		protected override TemplateItems CreateEntityForGet()
		{
			TemplateItems entity = new TemplateItems();

			entity.ReturnBehavior = ReturnBehavior.OnlySpecified;
			entity.Attributes = new List<AttributeValue>() { new AttributeValue() };
			entity.Categories = new List<CategoryStockItem>() { new CategoryStockItem() };
			entity.FileURLs = new List<InventoryFileUrls>() { new InventoryFileUrls() };
			entity.VendorDetails = new List<ProductItemVendorDetail>() { new ProductItemVendorDetail() };
			entity.ItemType = new StringReturn();

			return entity;
		}

		/// <summary>
		/// Creates a mapped entity for the passed entity
		/// </summary>
		/// <param name="entity">The entity to create the mapped entity from</param>
		/// <param name="syncId">The sync id of the entity</param>
		/// <param name="syncTime">The timestamp of the last modification</param>
		/// <returns>The mapped entity</returns>
		protected override MappedTemplateItem CreateMappedEntity(TemplateItems entity, Guid? syncId, DateTime? syncTime)
		{
			return new MappedTemplateItem(entity, entity.SyncID, entity.SyncTime);
		}

		/// <inheritdoc/>
		public override object GetTargetObjectExport(object currentSourceObject, IExternEntity data, BCEntityExportMapping mapping)
		{
			ProductData productData = data as ProductData;
			ProductItem productItem = currentSourceObject as ProductItem;
			//Flag that indicates if we are trying to find a matching matrix to a defaultVariant.
			bool isMatrixToVariants = mapping.SourceObject.Contains(BCConstants.Matrix) && mapping.TargetObject.Contains(BCConstants.Variants);

			return (isMatrixToVariants) ?
				productData?.Variants?.FirstOrDefault(variant => variant.Sku == productItem.InventoryID.Value) :
				base.GetTargetObjectExport(currentSourceObject, data, mapping);
		}
		#endregion

		#region Import
		public override async Task FetchBucketsForImport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			//No DateTime filtering for Category
			FilterProducts filter = new FilterProducts
			{
				UpdatedAtMin = minDateTime == null ? (DateTime?)null : minDateTime.Value.ToLocalTime().AddSeconds(-GetBindingExt<BCBindingShopify>().ApiDelaySeconds ?? 0),
				UpdatedAtMax = maxDateTime == null ? (DateTime?)null : maxDateTime.Value.ToLocalTime()
			};

            // to force the code to run asynchronously and keep UI responsive.
            //In some case it runs synchronously especially when using IAsyncEnumerable
            await Task.Yield();
            await foreach (ProductData product in productDataProvider.GetAll(filter, cancellationToken))
			{
				if (!base.IsTemplateItem(product))
					continue;

				SPTemplateItemEntityBucket bucket = CreateBucket();
				MappedTemplateItem obj = bucket.Product = bucket.Product.Set(product, product.Id?.ToString(), product.Title, product.DateModifiedAt.ToDate(false));
				EntityStatus status = EnsureStatus(obj, SyncDirection.Import);
				AddOptionsConfigurationForImport(product, obj.SyncID, obj.ExternID);

				if (product.Variants?.Count > 0)
				{
					product.Variants.ForEach(x => { bucket.VariantMappings[x.Sku ?? x.Id?.ToString()] = Tuple.Create(x.Id, x.InventoryManagement, x.InventoryPolicy); });
				}
			}
		}
		public override async Task<EntityStatus> GetBucketForImport(SPTemplateItemEntityBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			ProductData data = await productDataProvider.GetByID(syncstatus.ExternID, includedMetafields: true, cancellationToken);
			if (data == null) return EntityStatus.None;

			if (data.Variants?.Count > 0)
			{
				data.Variants.ForEach(x =>
				{
					var key = String.IsNullOrEmpty(x.Sku) ? x.Id?.ToString() : x.Sku;
					if (bucket.VariantMappings.ContainsKey(key))
						bucket.VariantMappings[key] = Tuple.Create(x.Id, x.InventoryManagement, x.InventoryPolicy);
					else
						bucket.VariantMappings.Add(key, Tuple.Create(x.Id, x.InventoryManagement, x.InventoryPolicy));
				});
			}

			MappedTemplateItem obj = bucket.Product = bucket.Product.Set(data, data.Id?.ToString(), data.Title, data.DateModifiedAt.ToDate(false));
			EntityStatus status = EnsureStatus(obj, SyncDirection.Import);
			return status;
		}

		public override async Task<PullSimilarResult<MappedTemplateItem>> PullSimilar(IExternEntity entity, CancellationToken cancellationToken = default)
		{
			var externalTemplate = entity as ProductData;

			if (externalTemplate == null)
				return null;

			var uniqueSKUs = externalTemplate.Variants
				.Select(variant => variant.Sku)
				.Where(sku => !string.IsNullOrWhiteSpace(sku))
				.Distinct()
				.ToList();

			var uniqueNames = externalTemplate.Variants.Select(x => x.Title).Distinct().ToList();

			var templates = ProductImportService.FindSimilarTemplateItems(uniqueSKUs, uniqueNames, externalTemplate.Title);
			
			if (templates == null || !templates.Any())
				return null;

			return new PullSimilarResult<MappedTemplateItem>
			{
				UniqueField = externalTemplate.Title,
				Entities = templates.Select(item => new MappedTemplateItem((TemplateItems)item, item.Id, item.LastModifiedDateTime.Value)).ToList()
			};
		}

		public override async Task MapBucketImport(SPTemplateItemEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			ProductData external = bucket.Product.Extern;

			if (external == null && bucket.Product.Local != null)
				return;

			var syncid = bucket.Primary.SyncID;

			//Options may have changed since the prepare.
			AddOptionsConfigurationForImport(bucket.Product.Extern, syncid, bucket.Product.ExternID);

			TemplateItems existingTemplateItem = existing?.Local as TemplateItems;

			var validationResult = templateItemsMappingService.ValidateOptionsMappings(syncid.Value);

			//get the current store
			var optionsMappingValidationErrorMessage = validationResult.Status switch
			{
				OptionMappingStatus.HasDuplicates => PXMessages.LocalizeFormat(BCMessages.PITemplateHasDuplicatedAttributes, bucket.Product.ExternDescription),
				OptionMappingStatus.NotOrPartiallyMapped => PXMessages.LocalizeFormat(BCMessages.PITemplateNotReadyToBeImported, GetBindingExt<BCBinding>().BindingName),
				OptionMappingStatus.Valid => string.Empty,
				_ => string.Empty
			};

			if (!string.IsNullOrEmpty(optionsMappingValidationErrorMessage))
			{
				var syncStatus = SelectStatus(bucket.Product);
				syncStatus.ExternDescription = bucket.Product.ExternDescription;
				UpdateSyncStatusFailed(bucket.Product, BCSyncOperationAttribute.ExternChanged, optionsMappingValidationErrorMessage);
				throw new PXException(optionsMappingValidationErrorMessage);
			}
				
			//Map the template fields
			TemplateItems localTemplateItems = MapProductTemplateItem(external, existingTemplateItem, syncid);

			bucket.Product.Local = localTemplateItems;
						
			#region "Map Variants to Stock/Non Stock Items"
			if (localTemplateItems.Matrix is null)
				localTemplateItems.Matrix = new List<ProductItem>();

			var itemClass = templateItemsMappingService.GetTemplateMappedItemClass(syncid);

			foreach (var variant in external.Variants)
			{
				ProductItem existingMatrixItem = null;
				//Check for existingTemplateItems Matrix Item.
				if (existingTemplateItem != null)
				{
					var syncedNoteId = templateItemsMappingService.GetSyncedMatrixItemNoteID(syncid, variant.Id.ToString());

					if (syncedNoteId != null)
					{
						existingMatrixItem = existingTemplateItem.Matrix.FirstOrDefault(x => x.NoteID?.Value == syncedNoteId);
					}
					else
					{
						var similarItems = ProductImportService.FindSimilarProductForSku<ProductItem>(variant.Sku, existingTemplateItem.InventoryID.Value).Where(x=>x.TemplateItemID!=null).ToList();
						var templateId = similarItems?.FirstOrDefault()?.TemplateItemID;

						if (similarItems!=null && similarItems.Count() == 1 && templateId != null && templateId.Value == existingTemplateItem.InventoryID.Value)
						{
							var inventoryId = similarItems.FirstOrDefault()?.InventoryID?.Value?.Trim() ;
							existingMatrixItem  = existingTemplateItem.Matrix.FirstOrDefault(x => x.InventoryID.Value == inventoryId);
						}
					}

					if (existingMatrixItem is null)
						existingMatrixItem = this.GetSimilarLocalProductItem(existingTemplateItem.Matrix, variant, syncid);
				}

				if (localTemplateItems.IsStockItem.Value == true)
				{
					StockItem item = GetMappedVariant<StockItem>(externalVariant: variant, exernalTemplateItems: external,
																localTemplateItems: localTemplateItems, existingTemplateVariant: existingMatrixItem, syncid);

					item.SalesUOM = localTemplateItems.SalesUOM;
					item.NotAvailable = localTemplateItems.NotAvailable;
					
					var mappedStockItem = new MappedStockItemVariant(item, existingMatrixItem?.NoteID.Value, external.DateModifiedAt.ToDate());
					mappedStockItem.ExternID = variant.Id.ToString();
					mappedStockItem.ExternDescription = variant.Sku;
					mappedStockItem.Local = item;
					mappedStockItem.Extern = variant;
					mappedStockItem.LocalID = existingMatrixItem?.NoteID?.Value;
					this.UpdateVendorDetails(item, existingMatrixItem, external.Vendor);
					item.Availability = _stockItemAvailabilityProvider.GetAvailablity(variant.InventoryManagement);

					GetHelper<SPHelper>().AddExternalSku(existingMatrixItem, item, variant.Sku);
					GetHelper<SPHelper>().AddBarCode(existingMatrixItem, item, variant.Barcode);

					bucket.StockItemVariants.Add(mappedStockItem);
					localTemplateItems.Matrix.Add(item);
				}
				else
				{
					NonStockItem item = GetMappedVariant<NonStockItem>(externalVariant: variant, exernalTemplateItems: external,
																		localTemplateItems: localTemplateItems, existingTemplateVariant: existingMatrixItem, syncid);
			
					var mappedNonStockItem = new MappedNonStockItemVariant(item, existingMatrixItem?.NoteID.Value, external.DateModifiedAt.ToDate());

					item.SalesUnit = itemClass?.SalesUnit?.ValueField();
					item.PurchaseUnit = itemClass?.PurchaseUnit?.ValueField();
					item.BaseUnit = itemClass?.BaseUnit?.ValueField();
					item.RequireShipment = new BooleanValue() { Value = false };
					mappedNonStockItem.ExternID = variant.Id.ToString();
					mappedNonStockItem.ExternDescription = variant.Sku;
					mappedNonStockItem.Local = item;
					mappedNonStockItem.Extern = variant;
					mappedNonStockItem.LocalID = existingMatrixItem?.NoteID?.Value;
					this.UpdateVendorDetails(item, existingMatrixItem, external.Vendor);
					item.Availability = _nonStockItemAvailabilityProvider.GetAvailablity(variant.InventoryManagement);

					GetHelper<SPHelper>().AddExternalSku(existingMatrixItem, item, variant.Sku);
					GetHelper<SPHelper>().AddBarCode(existingMatrixItem, item, variant.Barcode);

					bucket.NonStockItemVariants.Add(mappedNonStockItem);
					localTemplateItems.Matrix.Add(item);
				}
			}
			#endregion
			bucket.Product.Local = localTemplateItems;
		}

		private ProductItem GetSimilarLocalProductItem(IEnumerable<ProductItem> localProductItems, ProductVariantData externalVariantData, int? syncid)
		{
			IEnumerable<AttributeValue> mappedAttributes = this.GetMappedAttributesForVariant(externalVariantData, syncid);

			foreach (ProductItem productItem in localProductItems)
			{
				var variantOptions = new string[] { externalVariantData.Option1, externalVariantData.Option2, externalVariantData.Option3 }
				.Where(option => !string.IsNullOrWhiteSpace(option))
				.ToList();

				if (productItem.Attributes.SequenceEqual(variantOptions, IsAttributeEqualVariantOption) || productItem.Attributes.SequenceEqual(mappedAttributes))
					return productItem;
			}

			return null;

			bool IsAttributeEqualVariantOption(AttributeValue attribute, string option) => option.Equals(attribute.Value.Value, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Map a externalVariant to a stock or non stock item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="externalVariant"></param>
		/// <param name="exernalTemplateItems"></param>
		/// <param name="localTemplateItems">The current TemplateItems (parent) being synced</param>
		/// <param name="existingTemplateVariant">Existing TemplateItems in the ERP (if first sync, null)</param>
		/// <param name="syncid">The SyncID of the current operation</param>
		/// <returns></returns>
		public virtual T GetMappedVariant<T>(ProductVariantData externalVariant, ProductData exernalTemplateItems,
											 ProductItem localTemplateItems, ProductItem existingTemplateVariant,
											 int? syncid) where T : ProductItem, new()
		{
			var item = new T();

			//Validate that we can merge with the existingTemplateItems product item.
			//If we can't merge and an existingTemplateItems product exists with the same SKU, the method throws an exception.
			if (existingTemplateVariant != null)
				ProductImportService.ValidateMergingWithExistingItem(existingTemplateVariant, exernalTemplateItems.Id.ToString(), Operation.EntityType);

			item.InventoryID = existingTemplateVariant != null ? existingTemplateVariant.InventoryID :
								ProductImportService.GetNewProductInventoryID(null, exernalTemplateItems.Title).ValueField();
			item.TemplateItemID = localTemplateItems.InventoryID.Value.SearchField();

			item.NoteID = existingTemplateVariant?.NoteID;
			item.ExternalSku = externalVariant.Sku?.ValueField();
			item.Description = $"{externalVariant.Sku} ({exernalTemplateItems.Title})".ValueField();
			item.Content = exernalTemplateItems.BodyHTML.ValueField();
			item.Visibility = (exernalTemplateItems.DataPublishedAt is null ? BCItemVisibility.Invisible : BCItemVisibility.Visible).ValueField();

			//Price and MSRP should be mapped only for the first import.
			//First time the template is imported (therefore, the first time the defaultVariant is imported):
			//		- If the defaultVariant has a price, then the defaultVariant price is mapped to the template price.
			//		- If the defaultVariant has no price, then the template price is mapped to the defaultVariant price.
			//Template already imported but first time we import the defaultVariant (it has been added after the first import for instance)
			//		- If the defaultVariant has a price, then the defaultVariant price is mapped to the defaultVariant price.
			//		- If the defaultVariant has no price, then the existingTemplateItems template in the ERP price is mapped to the defaultVariant price.
			if (existingTemplateVariant == null)
			{
				item.DefaultPrice = externalVariant.Price?.ValueField();
				item.MSRP = externalVariant.OriginalPrice?.ValueField();
			}

			item.ItemClass = localTemplateItems.ItemClass;
			item.PostingClass = localTemplateItems.PostingClass;
			item.ItemClass = localTemplateItems.ItemClass;
			item.TaxCategory = localTemplateItems.TaxCategory;
			item.WeightUOM = this.GetLocalWeightUnit(externalVariant.WeightUnit).ValueField();
			item.DimensionWeight = this.GetWeight(externalVariant.Weight, externalVariant.WeightUnit)?.ValueField();

			if (item.Attributes == null) item.Attributes = new List<AttributeValue>();
		
			var rowNumber = 1;

			var attributes = GetMappedAttributesForVariant(externalVariant, syncid);
			foreach(var attr in attributes)
			{
				if (attr == null)
					continue;
				attr.RowNumber = rowNumber++;
				attr.InventoryID = localTemplateItems.InventoryID;
				item.Attributes.Add(attr);
			}

			this.templateItemsMappingService.UpdateAttributeActivity(localTemplateItems, item);

			return item;
		}

		/// <summary>
		/// Parses the externalTemplateItems externalVariant options and map them to ERP attributes/values
		/// </summary>
		/// <param name="externalVariant"></param>
		/// <param name="syncid"></param>
		/// <returns></returns>
		public virtual IEnumerable<AttributeValue> GetMappedAttributesForVariant(ProductVariantData externalVariant, int? syncid)
		{
			if (externalVariant == null)
				yield break;

			var mappedOptions = templateItemsMappingService.GetMappedOptionsForTemplate(syncid);
			
			var mappedOption1 = externalVariant.Option1 != null ? mappedOptions.FirstOrDefault(o => o.OptionSortOrder == 1 && o.OptionValue == externalVariant.Option1) : null;
			yield return GetAttributeValueFromMappedOption(mappedOption1);

			var mappedOption2 = externalVariant.Option2 != null ? mappedOptions.FirstOrDefault(o => o.OptionSortOrder == 2 && o.OptionValue == externalVariant.Option2) : null;
			yield return GetAttributeValueFromMappedOption(mappedOption2);

			var mappedOption3 = externalVariant.Option3 != null ? mappedOptions.FirstOrDefault(o => o.OptionSortOrder == 3 && o.OptionValue == externalVariant.Option3) : null;
			yield return GetAttributeValueFromMappedOption(mappedOption3);
		}

		/// <summary>
		/// Get a local representation of Attribute/Value from a mapped option
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public virtual AttributeValue GetAttributeValueFromMappedOption(TemplateMappedOptions option)
		{
			if (option == null)
				return null;

			var attributeValue = new AttributeValue()
			{
				AttributeID = option.AttributeID.ValueField(),
				AttributeDescription = option.AttributeDescription.ValueField(),
				Value = option.Value.ValueField(),
				ValueDescription = option.Value.ValueField(),
				IsActive = true.ValueField(),
			};

			return attributeValue;
		}

		/// <summary>
		/// Map fields of the template items
		/// </summary>
		/// <param name="externalTemplateItems"></param>
		/// <param name="existingTemplateItems">null if it is the first sync</param>
		/// <param name="syncid"></param>
		/// <returns></returns>
		/// <exception cref="PXInvalidOperationException"></exception>
		public virtual TemplateItems MapProductTemplateItem(ProductData externalTemplateItems, ProductItem existingTemplateItems, int? syncid) 
		{
			ProductVariantData defaultVariant = externalTemplateItems.Variants.FirstOrDefault();

			if (defaultVariant is null)
			{	
				throw new PXInvalidOperationException(BCMessages.PIProductHasNoVariant, externalTemplateItems.Id);
			}

			var local = new TemplateItems();

			//Validate that we can merge with the existingTemplateItems product item.
			//If we can't merge and an existingTemplateItems product exists with the same SKU, the method throws an exception.
			if (existingTemplateItems != null)
				ProductImportService.ValidateMergingWithExistingItem(existingTemplateItems, externalTemplateItems.Id.ToString(), Operation.EntityType);
			
			local.Description = externalTemplateItems.Title.ValueField();
			local.Content = externalTemplateItems.BodyHTML.ValueField();
			local.ItemStatus = GetItemStatus(externalTemplateItems).ValueField();

			local.InventoryID = existingTemplateItems?.InventoryID?.Value != null	
					? existingTemplateItems?.InventoryID.Value.SearchField() : ProductImportService.GetNewProductInventoryID(null, local.Description?.Value).ValueField();

			var itemClass = templateItemsMappingService.GetTemplateMappedItemClass(syncid);
			local.ItemClass = itemClass?.ItemClassCD.ValueField();

			local.Attributes = new List<AttributeValue>();
			local.Attributes = templateItemsMappingService.GetAttributesDefitionsForClassID(itemClass.ItemClassID.Value)?.ToList();

			var isDigitalTemplate = IsDigitalTemplate(externalTemplateItems);
			local.IsStockItem = local.RequireShipment = (!isDigitalTemplate).ValueField();
			string subEntityType = isDigitalTemplate ? BCEntitiesAttribute.NonStockItem : BCEntitiesAttribute.StockItem;
			base.ProductImportService.Settings.SubEntityTypeForTemplate = subEntityType;

			local.Visibility = (externalTemplateItems.DataPublishedAt is null ? BCItemVisibility.Invisible : BCItemVisibility.Visible).ValueField();

			this.SetTaxCaterogyForProductItem(local, externalTemplateItems, itemClass);

			if (existingTemplateItems == null)
			{
				ProductImportService.ValidateItemClass(itemClass, itemClass?.ItemClassCD, externalTemplateItems?.Title, local.Attributes.Count());
				string inventoryManagement = externalTemplateItems.Variants.Select(variant => variant.InventoryManagement).FirstOrDefault();
				local.Availability = local.IsStockItem.Value == true
					? _stockItemAvailabilityProvider.GetAvailablity(inventoryManagement)
					: _nonStockItemAvailabilityProvider.GetAvailablity(inventoryManagement);
				local.NotAvailable = BCItemNotAvailModes.StoreDefault.ValueField();
				
				local.DefaultPrice = 0M.ValueField();
				local.MSRP = 0M.ValueField();
			}

			local.PostingClass = itemClass?.PostClassID?.ValueField();

			if (existingTemplateItems == null)
			{
				if (defaultVariant.InventoryManagement == null)
				{
					local.NotAvailable = BCItemNotAvailModes.StoreDefault.ValueField();
				}
				else if (defaultVariant.InventoryManagement == ShopifyConstants.InventoryManagement_Shopify)
				{
					string notAvailableBehavior = defaultVariant.InventoryPolicy == InventoryPolicy.Continue
												? BCItemNotAvailModes.DoNothing : BCItemNotAvailModes.DisableItem;

					local.NotAvailable = notAvailableBehavior.ValueField();
				}
			}

			local.WeightUOM = GetLocalWeightUnit(defaultVariant.WeightUnit)?.ValueField();
			var weight = GetWeight(defaultVariant.Weight, defaultVariant.WeightUnit);
			local.DimensionWeight = weight == null ? null : weight.ValueField();

			this.UpdateVendorDetails(local, existingTemplateItems, externalTemplateItems.Vendor);

			return local;
		}

		/// <summary>
		/// Checks whether the template is considered as a digital (Non Stock) or physical (Stock) template.
		/// If all variants are digital, then the template is Non Stock
		/// If all variants are physical, the the template is Stock
		/// If there is a mix, an error is thrown.
		/// </summary>
		/// <param name="product"></param>
		/// <returns></returns>
		/// <exception cref="PXException"></exception>
		public virtual bool IsDigitalTemplate(ProductData product)
		{
			if (product.Variants.Any(x => x.RequiresShipping == false) && product.Variants.Any(x => x.RequiresShipping == true))
			{
				throw new PXException(BCMessages.PITemplateHasMixOfVariantsTypes, product.Title);
			}

			return product.Variants.Any(x => x.RequiresShipping == false);
		}

		public override async Task SaveBucketImport(SPTemplateItemEntityBucket bucket, IMappedEntity existing, String operation, CancellationToken cancellationToken = default)
		{
			MappedTemplateItem obj = bucket.Product;
			var external = bucket.Product.Extern;

			//
			// Since we do not use MatrixItems in TemplateItems endpoint, we have t clear data in TemplateItems::Matrix to avoid mapping issues.
			// TemplateItems::Matrix initially filled with the same values as bucket.StockItemVariants/bucket.NonStockItemVariants in order to get correct user mapping.
			//
			obj.Local.Matrix = new List<ProductItem>();

			TemplateItems impl = cbapi.Put<TemplateItems>(obj.Local, obj.LocalID);
			bucket.Product.AddLocal(impl, impl.SyncID, impl.SyncTime);
			bucket.Product.ClearDetails();

			this.SaveStockDetails(impl, bucket);
			this.SaveNonStockDetails(impl, bucket);

			base.UpdateMappedEntityTimestamp(bucket.Product);
			this.AddOptionValuesDetails(bucket);
			base.UpdateStatus(obj, operation);
		}

		private void SaveStockDetails(TemplateItems impl, SPTemplateItemEntityBucket bucket)
		{
			if (impl is null || impl.IsStockItem.Value != true)
				return;

			foreach (var item in bucket.StockItemVariants)
			{
				item.Local.TemplateItemID = bucket.Product.Local.InventoryID;
				var detail = cbapi.Put<StockItem>(item.Local, item.LocalID);
				bucket.Product.AddDetail(BCEntitiesAttribute.Variant, detail.NoteID.Value, item.ExternID);
			}

			this.MarkRemovedProductVariants<StockItem>(impl.Matrix, bucket.StockItemVariants.Select(stockItem => stockItem.Local));
		}

		private void SaveNonStockDetails(TemplateItems impl, SPTemplateItemEntityBucket bucket)
		{
			if (impl is null || impl.IsStockItem.Value != false)
				return;

			foreach (var item in bucket.NonStockItemVariants)
			{
				item.Local.TemplateItemID = bucket.Product.Local.InventoryID;
				var detail = cbapi.Put<NonStockItem>(item.Local, item.LocalID);
				bucket.Product.AddDetail(BCEntitiesAttribute.Variant, detail.NoteID.Value, item.ExternID);
			}

			this.MarkRemovedProductVariants<NonStockItem>(impl.Matrix, bucket.NonStockItemVariants.Select(nonStockItem => nonStockItem.Local));
		}

		/// <summary>
		/// Add sync details for options and options' values
		/// </summary>
		/// <param name="bucket"></param>
		public virtual void AddOptionValuesDetails(SPTemplateItemEntityBucket bucket)
		{
			var details = templateItemsMappingService.GetAttributesDetails(bucket.Product.SyncID);
			if (details == null || details.Count == 0) return;

			var byOption = details.GroupBy(x => x.OptionExternId).ToDictionary(x => x.Key, x => x.ToList());

			foreach (var option in byOption.Keys)
			{
				if (bucket.Product.Details.Any(x => x.ExternID == option))
					continue;
				var attribute = byOption[option].FirstOrDefault();
				bucket.Product.AddDetail(BCEntitiesAttribute.ProductOption, attribute.AttributeNoteId, option);

				//add values
				foreach (var value in byOption[option])
				{
					var key = new object[] { option, value.OptionValueExternId }.KeyCombine();
					if (bucket.Product.Details.Any(x => x.ExternID == key && x.EntityType == BCEntitiesAttribute.ProductOptionValue))
						continue;
					bucket.Product.AddDetail(BCEntitiesAttribute.ProductOptionValue, value.AttributeValueNoteId, key);
				}
			}
		}

		/// <summary>
		/// Add all options to the Option Mapping Table
		/// </summary>
		/// <param name="product"></param>
		/// <param name="syncId"></param>
		/// <param name="externId"></param>
		public virtual void AddOptionsConfigurationForImport(ProductData product, int? syncId, string externId)
		{
			var options = new List<ExternalTemplateOptions>();
			foreach (var option in product.Options)
			{
				foreach (var optionValue in option.Values)
				{
					var externalOption = new ExternalTemplateOptions()
					{
						SyncID = syncId.Value,
						ExternalID = externId,
						OptionID = option.Id?.ToString(),
						OptionDisplayName = option.Name,
						OptionName = option.Name,
						OptionValue = optionValue,
						OptionSortOrder = option.Position,
						OptionValueExternalId = optionValue

					};
					options.Add(externalOption);
				}
			}
			templateItemsMappingService.UpdateInsertOptions(options);
		}

		#endregion

		#region Export

		public override async Task<PullSimilarResult<MappedTemplateItem>> PullSimilar(ILocalEntity entity, CancellationToken cancellationToken = default)
		{
			TemplateItems localEnity = (TemplateItems)entity;
			string uniqueField = localEnity?.InventoryID?.Value;
			List<ProductData> datas = null;
			List<string> matrixIds = new List<string>();
			List<string> duplicateIds = new List<string>();
			if (localEnity?.Matrix?.Count > 0)
			{
				matrixIds = localEnity.Matrix.Select(x => x?.InventoryID?.Value).ToList();
				var filterQuery = string.Join(" OR ", matrixIds.Select(matrixId => $"sku:{matrixId}"));
				var existingItems = (await ProductGQLDataProvider.GetProductVariantsAsync(filterQuery, cancellationToken)).ToList();
				if (existingItems.Count > 0)
				{
					var matchedVariants = existingItems.Select(x => x.Product.IdNumber).Distinct().ToList();
					if (matchedVariants.Count > 0)
					{
						datas = new List<ProductData>();
						// to force the code to run asynchronously and keep UI responsive.
						//In some case it runs synchronously especially when using IAsyncEnumerable
						await Task.Yield();
						await foreach (var data in productDataProvider.GetAll(new FilterProducts() { IDs = string.Join(",", matchedVariants) }, cancellationToken))
							datas.Add(data);

						// collect duplicate SKUs when there's more than one product share same SKU(s)
						if (matchedVariants.Count > 1)
							duplicateIds = existingItems.GroupBy(x => x.Sku).Where(x => x.Count() > 1).Select(x => x.Key).ToList();

						// further filtering using product's name (Title in Shopify and Description in ERP)
						// Idea: if there's at least one product in SP whose name matches with template item's name in ERP
						// then we only need to care about those SP products and ignore other products
						// If there is ONLY one product in SP that has same name => map that product with the template item
						// Otherwise, if there is NONE or more than one product in SP having same name => throw an error
						if (matchedVariants.Count > 1 && datas.Count() > 1 && datas.Any(x => string.Equals(x.Title.Trim(), localEnity.Description?.Value?.Trim(), StringComparison.OrdinalIgnoreCase)))
						{
							datas = datas.Where(x => string.Equals(x.Title.Trim(), localEnity.Description?.Value?.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

							// if there's more than one product having same name => include product name in the uniqueField variable
							// so that it can be displayed in the error message later
							if (datas.Count() > 1 && localEnity.Description?.Value != null)
								duplicateIds.Add(localEnity.Description?.Value);
						}
					}
				}
			}

			// if there's anything in duplicateIds then it will be more useful then the original inventory Id
			if (duplicateIds.Count > 0 && datas.Count() > 1)
				uniqueField = string.Join(", ", duplicateIds);

			return new PullSimilarResult<MappedTemplateItem>() { UniqueField = uniqueField, Entities = datas == null ? null : datas.Select(data => new MappedTemplateItem(data, data.Id.ToString(), data.Title, data.DateModifiedAt.ToDate(false))) };
		}

		public override async Task<EntityStatus> GetBucketForExport(SPTemplateItemEntityBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			TemplateItems impl = GetTemplateItem(syncstatus.LocalID);
			if (impl == null || impl.Matrix?.Count == 0) return EntityStatus.None;

			impl.AttributesDef = new List<AttributeDefinition>();
			impl.AttributesValues = new List<AttributeValue>();
			int? inventoryID = null;
			foreach (PXResult<CSAttribute, CSAttributeGroup, INItemClass, InventoryItem> attributeDef in PXSelectJoin<CSAttribute,
			   InnerJoin<CSAttributeGroup, On<CSAttributeGroup.attributeID, Equal<CSAttribute.attributeID>>,
			   InnerJoin<INItemClass, On<INItemClass.itemClassID, Equal<CSAttributeGroup.entityClassID>>,
			   InnerJoin<InventoryItem, On<InventoryItem.itemClassID, Equal<INItemClass.itemClassID>>>>>,
			  Where<InventoryItem.isTemplate, Equal<True>,
			  And<InventoryItem.noteID, Equal<Required<InventoryItem.noteID>>,
			  And<CSAttribute.controlType, Equal<Required<CSAttribute.controlType>>,
			  And<CSAttributeGroup.isActive, Equal<True>,
			  And<CSAttributeGroup.attributeCategory, Equal<CSAttributeGroup.attributeCategory.variant>
			  >>>>>>.Select(this, impl.Id, 2))
			{
				AttributeDefinition def = new AttributeDefinition();
				var inventory = (InventoryItem)attributeDef;
				inventoryID = inventory.InventoryID;
				var attribute = (CSAttribute)attributeDef;
				var attributeGroup = (CSAttributeGroup)attributeDef;
				def.AttributeID = attribute.AttributeID.ValueField();
				def.Description = attribute.Description.ValueField();
				def.NoteID = attribute.NoteID.ValueField();
				def.Order = attributeGroup.SortOrder.ValueField();

				def.Values = new List<AttributeDefinitionValue>();
				var attributedetails = PXSelect<CSAttributeDetail, Where<CSAttributeDetail.attributeID, Equal<Required<CSAttributeDetail.attributeID>>>>.Select(this, def.AttributeID.Value);
				foreach (CSAttributeDetail value in attributedetails)
				{
					AttributeDefinitionValue defValue = new AttributeDefinitionValue();
					defValue.NoteID = value.NoteID.ValueField();
					defValue.ValueID = value.ValueID?.Trim()?.ValueField();
					defValue.Description = value.Description.ValueField();
					defValue.SortOrder = (value.SortOrder ?? 0).ToInt().ValueField();
					def.Values.Add(defValue);
				}

				if (def != null)
					impl.AttributesDef.Add(def);
			}

			foreach (PXResult<InventoryItem, CSAnswers> attributeDef in PXSelectJoin<InventoryItem,
			   InnerJoin<CSAnswers, On<InventoryItem.noteID, Equal<CSAnswers.refNoteID>>>,
			  Where<InventoryItem.templateItemID, Equal<Required<InventoryItem.templateItemID>>
			  >>.Select(this, inventoryID))
			{
				var inventory = (InventoryItem)attributeDef;
				var attribute = (CSAnswers)attributeDef;
				AttributeValue def = new AttributeValue
				{
					AttributeID = attribute.AttributeID.ValueField(),
					NoteID = inventory.NoteID.ValueField(),
					InventoryID = inventory.InventoryCD.ValueField(),
					Value = attribute.Value?.Trim()?.ValueField(),
					IsActive = attribute.IsActive.ValueField()
				};
				impl.AttributesValues.Add(def);
			}
			impl.InventoryItemID = inventoryID;

			MappedTemplateItem obj = bucket.Product = bucket.Product.Set(impl, impl.SyncID, impl.SyncTime);
			EntityStatus status = EnsureStatus(obj, SyncDirection.Export);

			//Only calculates the active values and belongs to the defaultVariant category definition
			var activeVariantAttrQty = impl.AttributesValues.Where(a => (a.IsActive?.Value ?? false) && impl.AttributesDef.Any(ad => ad.AttributeID?.Value == a.AttributeID?.Value)).Select(a => a.AttributeID?.Value).Distinct().Count();
			if (activeVariantAttrQty > ShopifyConstants.ProductOptionsLimit)
			{
				throw new PXException(ShopifyMessages.ProductOptionsOutOfScope, activeVariantAttrQty, impl.InventoryID.Value, ShopifyConstants.ProductOptionsLimit);
			}
			if (impl.Matrix?.Count > 0)
			{
				var activeMatrixItems = impl.Matrix.Where(x => x?.ItemStatus?.Value == PX.Objects.IN.Messages.Active);
				if (activeMatrixItems.Count() == 0)
				{
					throw new PXException(BCMessages.NoMatrixCreated);
				}
				if (activeMatrixItems.Count() > ShopifyConstants.ProductVarantsLimit)
				{
					throw new PXException(ShopifyMessages.ProductVariantsOutOfScope, activeMatrixItems.Count(), impl.InventoryID.Value, ShopifyConstants.ProductVarantsLimit);
				}
				foreach (var category in activeMatrixItems)
				{
					if (!bucket.VariantMappings.ContainsKey(category.InventoryID?.Value))
						bucket.VariantMappings.Add(category.InventoryID?.Value, null);
				}
			}
			if (obj.Local.Categories != null)
			{
				foreach (CategoryStockItem category in obj.Local.Categories)
				{
					if (!SalesCategories.ContainsKey(category.CategoryID.Value.Value))
					{
						BCItemSalesCategory implCat = cbapi.Get<BCItemSalesCategory>(new BCItemSalesCategory() { CategoryID = new IntSearch() { Value = category.CategoryID.Value } });
						if (implCat == null) continue;
						if (category.CategoryID.Value != null)
						{
							SalesCategories[category.CategoryID.Value.Value] = implCat.Description.Value;
						}
					}
				}
			}

			if (!string.IsNullOrWhiteSpace(syncstatus.ExternID))
			{
				ProductData product = await this.productDataProvider.GetByID(syncstatus.ExternID, includedMetafields: true, cancellationToken);
				if (product is not null && product.Variants is not null && product.Variants.Any())
				{
					foreach (var variant in product.Variants)
						bucket.ExternalVariants[variant.Id.Value] = variant;
				}
			}

			return status;
		}

		public override async Task MapBucketExport(SPTemplateItemEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			TemplateItems local = bucket.Product.Local;
			ProductData external = bucket.Product.Extern = new ProductData();

			MappedTemplateItem existingMapped = existing as MappedTemplateItem;
			ProductData existingData = existing?.Extern as ProductData;

			if (local.Matrix == null || local.Matrix?.Count == 0)
			{
				throw new PXException(BCMessages.NoMatrixCreated);
			}

			templateItemsMappingService.RemapVariantOptionsForExport(bucket.Product.SyncID, local, bucket?.Product?.ExternID);

			MapInventoryItem(local, external, existingData);

			MapMetadata(bucket, local, external, existingData);

			string visibility = local?.Visibility?.Value;
			if (visibility == null || visibility == BCCaptions.StoreDefault) visibility = BCItemVisibility.Convert(GetBindingExt<BCBindingExt>().Visibility);

			SetProductStatus(external, local.ItemStatus?.Value, local.Availability?.Value, visibility);

			MapProductOptions(local, external);

			MapProductVariants(bucket, existingMapped);
		}

		public virtual void MapInventoryItem(TemplateItems local, ProductData external, ProductData existingData)
		{
			external.Title = local.Description?.Value;
			external.BodyHTML = GetHelper<SPHelper>().ClearHTMLContent(local.Content?.Value);
			external.ProductType = SelectFrom<INItemClass>
				.Where<INItemClass.itemClassCD.IsEqual<@P.AsString>>
				.View.Select(this, local.ItemClass.Value).FirstOrDefault()?.GetItem<INItemClass>().Descr;
			external.Vendor = GetDefaultVendorName(local.VendorDetails);
			//Put all categories to the Tags later if CombineCategoriesToTags setting is true
			var categories = local.Categories?.Select(x => { if (SalesCategories.TryGetValue(x.CategoryID.Value.Value, out var desc)) return desc; else return string.Empty; }).Where(x => !string.IsNullOrEmpty(x)).ToList();
			if (categories != null && categories.Count > 0)
				external.Categories = categories;
			if (!string.IsNullOrEmpty(local.SearchKeywords?.Value))
				external.Tags = local.SearchKeywords?.Value;
			if (!string.IsNullOrEmpty(local.PageTitle?.Value))
				external.GlobalTitleTag = local.PageTitle?.Value;
			if (!string.IsNullOrEmpty(local.MetaDescription?.Value))
				external.GlobalDescriptionTag = local.MetaDescription?.Value;
		}

		public virtual void MapMetadata(SPTemplateItemEntityBucket bucket, TemplateItems local, ProductData external, ProductData existingData)
		{
			var categories = local.Categories?.Select(x => { if (SalesCategories.TryGetValue(x.CategoryID.Value.Value, out var desc)) return desc; else return string.Empty; }).Where(x => !string.IsNullOrEmpty(x)).ToList();
			if (categories != null && categories.Count > 0)
				external.Categories = categories;
			if (!string.IsNullOrEmpty(local.SearchKeywords?.Value))
				external.Tags = local.SearchKeywords?.Value;
			if (!string.IsNullOrEmpty(local.PageTitle?.Value))
				external.GlobalTitleTag = local.PageTitle?.Value;
			if (!string.IsNullOrEmpty(local.MetaDescription?.Value))
				external.GlobalDescriptionTag = local.MetaDescription?.Value;

			if (!string.IsNullOrEmpty(bucket.Product.ExternID))
			{
				external.Id = bucket.Product.ExternID.ToLong();
			}
			else
			{
				external.Metafields = new List<MetafieldData>() { new MetafieldData() { Key = ShopifyCaptions.Product, Value = local.Id.Value.ToString(), Type = ShopifyConstants.ValueType_SingleString, Namespace = BCObjectsConstants.Namespace_Global } };
				external.Metafields.Add(new MetafieldData() { Key = ShopifyCaptions.ProductId, Value = local.InventoryID.Value, Type = ShopifyConstants.ValueType_SingleString, Namespace = BCObjectsConstants.Namespace_Global });
				external.Metafields.Add(new MetafieldData() { Key = nameof(ProductTypes), Value = BCCaptions.TemplateItem, Type = ShopifyConstants.ValueType_SingleString, Namespace = BCObjectsConstants.Namespace_Global });
			}
		}

		public virtual void MapProductOptions(TemplateItems local, ProductData external)
		{
			if (local.AttributesDef?.Count > 0)
			{
				external.Options = new List<ProductOptionData>();
				int optionSortOrder = 1;
				//We only want attribute defs that have active matrix items.
				var activeAttributeValues = local.AttributesValues.Where(a => a.IsActive?.Value ?? false)
					.Select(a => a.AttributeID?.Value)
					.Distinct();
				var activeAttributeDefinitions = local.AttributesDef.Where(a => a.AttributeID?.Value.IsIn(activeAttributeValues) ?? false);
				//Shopify only allows maximum 3 options
				foreach (var attribute in activeAttributeDefinitions.OrderBy(x => x.Order?.Value ?? short.MaxValue).Take(ShopifyConstants.ProductOptionsLimit))
				{
					external.Options.Add(new ProductOptionData() { Name = attribute.Description?.Value, Position = optionSortOrder });
					optionSortOrder++;
				}
			}
		}

		public virtual void MapProductVariants(SPTemplateItemEntityBucket bucket, MappedTemplateItem existing)
		{
			TemplateItems local = bucket.Product.Local;
			ProductData external = bucket.Product.Extern;

			var existingData = existing?.Extern;

			var inventoryItems = SelectFrom<InventoryItem>
				.LeftJoin<InventoryItemCurySettings>
				.On<InventoryItemCurySettings.inventoryID.IsEqual<InventoryItem.inventoryID>>
				.LeftJoin<INItemXRef>
				.On<InventoryItem.inventoryID.IsEqual<INItemXRef.inventoryID>
					.And<INItemXRef.alternateType.IsEqual<INAlternateType.vPN>>
					.And<INItemXRef.bAccountID.IsEqual<InventoryItemCurySettings.preferredVendorID>
						.Or<INItemXRef.alternateType.IsEqual<INAlternateType.barcode>>>>
				.Where<InventoryItem.templateItemID.IsEqual<@P.AsInt>>
				.View.Select(this, local.InventoryItemID).AsEnumerable()
				.Cast<PXResult<InventoryItem, InventoryItemCurySettings, INItemXRef>>()?.ToList();

			external.Variants = new List<ProductVariantData>();
			foreach (var item in bucket.Product.Local.Matrix.Where(x => IsVariantActive(x)).Take(ShopifyConstants.ProductVarantsLimit))
			{
				var matchedInventoryItems = inventoryItems?
					.Where(x => x.GetItem<InventoryItem>().InventoryCD.Trim() == item.InventoryID?.Value?.Trim())
					.ToList();
				var matchedInventoryItem = matchedInventoryItems.FirstOrDefault()?.GetItem<InventoryItem>();

				ProductVariantData variant = new ProductVariantData();

				MapVariantMetadata(bucket, item, variant);

				var matchingVariant = GetMatchingVariant(variant, existingData);

				variant.Id = matchingVariant?.Id;

				MapVariantInventoryItem(bucket, item, matchedInventoryItem, variant);
				MapVariantTaxability(local, variant);

				variant.Barcode = GetBarcode(item, local.SalesUOM, local.BaseUOM);

				if (local.IsStockItem?.Value == true)
				{
					MapVariantAvailability(bucket, item, matchedInventoryItem, variant, existingData);
				}
				else
				{
					string externAvailabilityMode = existingData?.Variants?.FirstOrDefault(x => x.Id == variant.Id)?.InventoryManagement ?? ShopifyConstants.InventoryManagement_Shopify;
					string localAvailabilityMode = BCItemAvailabilities.Convert(matchedInventoryItem.Availability);
					variant.InventoryManagement = localAvailabilityMode == BCCaptions.DoNotUpdate ? externAvailabilityMode : null;
					variant.InventoryPolicy = InventoryPolicy.Deny;
					variant.RequiresShipping = local.RequireShipment.Value ?? false;
				}
				
				if (variant.Id == null || variant.Id == 0)
				{
					variant.VariantMetafields = new List<MetafieldData>() { new MetafieldData() { Key = ShopifyConstants.Variant, Value = local.Id.Value.ToString(), Type = ShopifyConstants.ValueType_SingleString, Namespace = BCObjectsConstants.Namespace_Global } };
				}

				external.Variants.Add(variant);
			}
			MapVariantPositions(local, external);
		}

		public virtual void MapVariantInventoryItem(SPTemplateItemEntityBucket bucket, ProductItem local, InventoryItem localInventoryItem, ProductVariantData external)
		{
			TemplateItems parent = bucket.Product.Local;
			decimal? price = local.DefaultPrice.Value;
			decimal? msrp = local.MSRP?.Value != null && local.MSRP?.Value > local.DefaultPrice.Value ? local.MSRP?.Value : 0;
			if (localInventoryItem.BaseUnit != localInventoryItem.SalesUnit)
			{
				CalculatePrices(localInventoryItem.InventoryCD.Trim(), ref msrp, ref price);
			}

			external.LocalID = local.Id.Value;
			external.Title = local.Description?.Value ?? parent.Description?.Value;
			external.Price = price;

			bool isVariantSkuEmpty = !external.Id.HasValue
				|| !bucket.ExternalVariants.ContainsKey(external.Id.Value)
				|| string.IsNullOrWhiteSpace(bucket.ExternalVariants[external.Id.Value].Sku);

			string variantSku = isVariantSkuEmpty ? local.InventoryID?.Value : bucket.ExternalVariants[external.Id.Value].Sku;

			external.Sku = variantSku;
			external.OriginalPrice = msrp;
			external.Weight = (localInventoryItem?.BaseItemWeight ?? 0) != 0 ? localInventoryItem?.BaseItemWeight : parent.DimensionWeight?.Value;
			external.WeightUnit = (localInventoryItem?.WeightUOM ?? parent.WeightUOM?.Value)?.ToLower();
		}

		public virtual void MapVariantTaxability(TemplateItems local, ProductVariantData external)
		{
			bool isTaxable;
			bool.TryParse(GetHelper<SPHelper>().GetSubstituteLocalByExtern(GetBindingExt<BCBindingExt>().TaxCategorySubstitutionListID, local.TaxCategory?.Value, String.Empty), out isTaxable);
			external.Taxable = isTaxable;
		}

		public virtual void MapVariantAvailability(SPTemplateItemEntityBucket bucket, ProductItem local, InventoryItem localInventoryItem, ProductVariantData external, ProductData existingProduct)
		{
			string existingVariantID = bucket.Primary.Details.FirstOrDefault(d => d.EntityType == BCEntitiesAttribute.Variant && d.LocalID == local.Id)?.ExternID;
			ProductVariantData existingVariant = existingProduct?.Variants.FirstOrDefault(v => v?.Id.ToString() == existingVariantID);

			string availability = BCItemAvailabilities.Convert(localInventoryItem.Availability);

			if (availability == null || availability == BCCaptions.StoreDefault)
			{
				availability = BCItemAvailabilities.Convert(GetBindingExt<BCBindingExt>().Availability);
			}
			string notAvailable = BCItemAvailabilities.Convert(localInventoryItem.NotAvailMode);

			if (notAvailable == null || notAvailable == BCCaptions.StoreDefault)
			{
				notAvailable = BCItemNotAvailModes.Convert(GetBindingExt<BCBindingExt>().NotAvailMode);
			}

			if (local.ItemStatus?.Value == PX.Objects.IN.Messages.Active || local.ItemStatus?.Value == PX.Objects.IN.Messages.NoPurchases || local.ItemStatus?.Value == PX.Objects.IN.Messages.NoRequest)
			{
				if (availability == BCCaptions.AvailableTrack)
				{
					external.InventoryManagement = ShopifyConstants.InventoryManagement_Shopify;

					if (notAvailable == BCCaptions.DoNothing || notAvailable == BCCaptions.DoNotUpdate)
					{
						external.InventoryPolicy = existingVariant?.InventoryPolicy ?? InventoryPolicy.Continue;
					}
					else if (notAvailable == BCCaptions.DisableItem)
					{
						external.InventoryPolicy = InventoryPolicy.Deny;
					}
					else if (notAvailable == BCCaptions.PreOrderItem || notAvailable == BCCaptions.ContinueSellingItem || notAvailable == BCCaptions.EnableSellingItem)
					{
						external.InventoryPolicy = InventoryPolicy.Continue;
					}
				}
				else if (availability == BCCaptions.DoNotUpdate)
				{
					// Value can be null or 'shopify' but if the externalTemplateItems item does not exist, we need to go along with the SP standard behaviour - set 'shopify'
					external.InventoryManagement = existingProduct is null ? ShopifyConstants.InventoryManagement_Shopify : existingProduct.Variants[0]?.InventoryManagement;
					external.InventoryPolicy = existingVariant?.InventoryPolicy ?? InventoryPolicy.Deny;
				}
			}
			else if (local.ItemStatus?.Value == PX.Objects.IN.Messages.Inactive || local.ItemStatus?.Value == PX.Objects.IN.Messages.NoSales || local.ItemStatus?.Value == PX.Objects.IN.Messages.ToDelete)
			{
				external.InventoryManagement = null;
				external.InventoryPolicy = InventoryPolicy.Deny;
			}
		}

		public virtual void MapVariantMetadata(SPTemplateItemEntityBucket bucket, ProductItem local, ProductVariantData external)
		{
			ProductData externalParent = bucket.Product.Extern;
			var def = bucket.Product.Local.AttributesValues.Where(x => x.NoteID.Value == local.Id).ToList();
			foreach (var attrItem in def)
			{
				var optionObj = bucket.Product.Local.AttributesDef.FirstOrDefault(x => x.AttributeID.Value == attrItem.AttributeID.Value);
				if (optionObj != null)
				{

					var option = externalParent.Options.FirstOrDefault(x => optionObj != null && x.Name == optionObj.Description?.Value);
					if (option == null) continue;
					var attrValue = optionObj.Values.FirstOrDefault(x => x.ValueID?.Value == attrItem?.Value.Value);
					if (attrValue != null)
					{
						switch (option.Position)
						{
							case 1:
								{
									external.Option1 = attrValue?.Description?.Value;
									external.OptionSortOrder1 = attrValue.SortOrder.Value.Value;
									break;
								}
							case 2:
								{
									external.Option2 = attrValue?.Description?.Value;
									external.OptionSortOrder2 = attrValue.SortOrder.Value.Value;
									break;
								}
							case 3:
								{
									external.Option3 = attrValue?.Description?.Value;
									external.OptionSortOrder3 = attrValue.SortOrder.Value.Value;
									break;
								}
							default:
								break;
						}
					}
				}
			}
		}

		public virtual void MapVariantPositions(TemplateItems local, ProductData external)
		{
			if (external.Variants?.Count > 0)
			{
				external.Variants = external.Variants.OrderBy(x => x.OptionSortOrder1).ThenBy(x => x.OptionSortOrder2).ThenBy(x => x.OptionSortOrder3).ToList();
			}
			else throw new PXException(ShopifyMessages.NoProductVariants, local.InventoryID.Value);
		}

		public override object GetAttribute(SPTemplateItemEntityBucket bucket, string attributeID)
		{
			return GetAttribute(bucket.Product?.Local, attributeID);
		}

		public IEnumerable<string> GetSyncedIds(SPTemplateItemEntityBucket bucket)
		{
			if (bucket.Product.Details == null)
				yield break;
			foreach (var detail in bucket.Product.Details.Where(x => x.EntityType == BCEntitiesAttribute.Variant))
			{
				yield return detail.ExternID;
			}
		}

		public ProductVariantData GetMatchingVariant(ProductVariantData variant, ProductData product)
		{
			if (product?.Variants == null || variant == null)
				return null;
			return product.Variants.FirstOrDefault(x => x.Option1 == variant.Option1 && x.Option2 == variant.Option2 && x.Option3 == variant.Option3);
		}
		/// <summary>
		/// When exporting: we need to know which variants must be deleted in the external system.
		/// NOTE: Before implementing the import, it was relying on the SKUs to find out which variants must be deleted in Shopify.
		/// However, it is not a good idea because if the product already exists in Shopify and does not have any SKUs in the variants, it won't work.
		/// Same if we relid on SyncDetails content; if the product exists in Shopify and is about to be exported for the first time from the ERP (merged),
		/// then the SyncDetails will be empty and the variants won't be deleted.
		/// This new method is looking for the variants that do have a combination of options in the External system but do not exist in the ERP.
		/// These varianst (combinations of options) must be deleted in the external system.
		/// </summary>
		/// <param name="bucket"></param>
		/// <param name="existing">Existing object in the external system.</param>
		/// <returns></returns>
		public async Task DeletedVariantsInExternalSystem(SPTemplateItemEntityBucket bucket, IMappedEntity existing)
		{
			if (bucket.Product.ExternID == null || bucket.Product.Extern?.Variants == null || existing?.Extern == null)
				return;

			//Get all variants in the external system and check whether they still exist in the ERP
			var existingExternalTemplateItems = existing.Extern as ProductData;
			foreach(var variant in existingExternalTemplateItems.Variants)
			{
				var option1 = variant.Option1;
				var option2 = variant.Option2;
				var option3 = variant.Option3;

				//Compare with the object that is going to by synced.
				var matchedVariant = bucket.Product.Extern.Variants.FirstOrDefault(x => x.Option1 == option1 && x.Option2 == option2 && x.Option3 == option3);
				if (matchedVariant == null)
				{
					//This variant is not going to be synced, so it must be deleted.
					await productVariantDataProvider.Delete(bucket.Product.ExternID, variant.Id?.ToString());					
				}
			}
		}

		public override async Task SaveBucketExport(SPTemplateItemEntityBucket bucket, IMappedEntity existing, String operation, CancellationToken cancellationToken = default)
		{
			MappedTemplateItem obj = bucket.Product;
			ProductData data = null;

			if (obj.Extern.Categories?.Count > 0 && GetBindingExt<BCBindingShopify>()?.CombineCategoriesToTags == BCSalesCategoriesExportAttribute.SyncToProductTags)
			{
				obj.Extern.Tags = string.Join(",", obj.Extern.Categories) + (string.IsNullOrEmpty(obj.Extern.Tags) ? "" : $",{obj.Extern.Tags}");
			}

			try
			{
				if (obj.ExternID == null)
				{
					data = await productDataProvider.Create(obj.Extern);
				}
				else
				{
					await DeletedVariantsInExternalSystem(bucket, existing);				
					data = await productDataProvider.Update(obj.Extern, obj.ExternID);
				}
			}
			catch (Exception ex)
			{
				throw new PXException(ex.Message);
			}

			templateItemsMappingService.UpdateMappingOptionsForExport(bucket.Product.Local, obj.SyncID, data.Id.ToString());

			obj.ClearDetails(BCEntitiesAttribute.Variant);
			obj.AddExtern(data, data.Id?.ToString(), data.Title, data.DateModifiedAt.ToDate(false));
			if (data.Variants?.Count > 0)
			{
				var localVariants = obj.Local.Matrix;
				foreach (var externVariant in data.Variants)
				{
					var matchItem = localVariants.FirstOrDefault(x => x.InventoryID?.Value == externVariant.Sku);
					if (matchItem != null)
					{
						obj.AddDetail(BCEntitiesAttribute.Variant, matchItem.Id.Value, externVariant.Id.ToString());
					}
				}
			}

			templateItemsMappingService.UpdateExternalIdForMappings(obj.SyncID, data.Id?.ToString());

			await SaveImages(obj, obj.Local?.FileURLs, cancellationToken);

			UpdateStatus(obj, operation);
		}
		#endregion

		public virtual bool IsVariantActive(ProductItem item)
		{
			return !(item.ItemStatus?.Value == PX.Objects.IN.Messages.Inactive || item.ItemStatus?.Value == PX.Objects.IN.Messages.ToDelete || item.ItemStatus?.Value == PX.Objects.IN.Messages.NoSales)
				&& item.ExportToExternal.Value == true;
		}

		/// <summary>
		/// Retrieves the Template item using the <see cref="Core.Model.ICBAPIService"/>.
		/// </summary>
		/// <param name="localID"></param>
		/// <returns>The fetched <see cref="TemplateItems"/>.</returns>
		public virtual TemplateItems GetTemplateItem(Guid? localID)
		{
			TemplateItems templateItem = cbapi.GetByID(localID, CreateEntityForGet(), GetCustomFieldsForExport());
			if (templateItem == null) return null;

			templateItem.Matrix = new List<ProductItem>();
			List<StockItem> stockMatrixItems = cbapi.GetAll(CreateProductItemFilter<StockItem>(templateItem.InventoryID.Value)).ToList();
			templateItem.Matrix.AddRange(stockMatrixItems);
			List<NonStockItem> nonStockMatrixItems = cbapi.GetAll(CreateProductItemFilter<NonStockItem>(templateItem.InventoryID.Value)).ToList();
			templateItem.Matrix.AddRange(nonStockMatrixItems);

			return templateItem;
		}

		/// <summary>
		/// Creates a <typeparamref name="ProductItemType"/> using <paramref name="inventoryID"/> as filter for <see cref="ProductItem.TemplateItemID"/>.
		/// </summary>
		/// <typeparam name="ProductItemType">Type of the filter to be created.</typeparam>
		/// <param name="inventoryID"></param>
		/// <returns>A new <typeparamref name="ProductItemType"/>.</returns>
		public virtual ProductItemType CreateProductItemFilter<ProductItemType>(string inventoryID) where ProductItemType : ProductItem, new()
		{
			return new ProductItemType()
			{
				TemplateItemID = new StringSearch() { Value = inventoryID },
				Attributes = new List<AttributeValue>(),
				Custom = GetCustomFieldsForExport(),
				CrossReferences = new List<InventoryItemCrossReference>(),
				ReturnBehavior = ReturnBehavior.All
			};
		}
		
		/// <summary>
		/// Iterates through <paramref name="listOfVendors"/> and returns the default vendor name or the first one. 
		/// </summary>
		/// <param name="listOfVendors"></param>
		/// <returns>Return the default vendor name or the first one, if exists.</returns>
		public virtual string GetDefaultVendorName(IEnumerable<ProductItemVendorDetail> listOfVendors)
		{
			if (listOfVendors == null) return null;
			ProductItemVendorDetail vendorDetail = listOfVendors.FirstOrDefault(vendor => vendor.Default?.Value == true) ?? listOfVendors.FirstOrDefault();
			return vendorDetail?.VendorName?.Value;
		}
	}
}
