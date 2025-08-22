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
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.Common.Extensions;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce
{
	public class BCProductWithVariantEntityBucket : BCProductEntityBucket<MappedTemplateItem>
	{
		public override IMappedEntity[] PreProcessors { get => Categories.ToArray(); }
		public List<MappedStockItemVariant> StockItemVariants = new List<MappedStockItemVariant>();
		public List<MappedNonStockItemVariant> NonStockItemVariants = new List<MappedNonStockItemVariant>();

		/// <summary>
		/// Get the external variants by variant id.
		/// </summary>
		public Dictionary<long, ProductsVariantData> ExternalVariants { get; } = new Dictionary<long, ProductsVariantData>();
	}

	public class BCTemplateItemRestrictor : BCBaseRestrictor, IRestrictor
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

				if (external.BaseVariantId.HasValue || external.Variants == null || external.Variants.Count == 0 ||
					(external.Variants.Count == 1 && external.Variants.FirstOrDefault().Sku == external.Sku))
				{

					if (mode != FilterMode.Merge && external.Type == ProductTypes.digital.ToString())
					{
						return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogTemplateImportSkippedNonStockItem, external.Name));
					}

					if (mode != FilterMode.Merge && external.Type == ProductTypes.physical.ToString())
					{
						return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogTemplateImportSkippedStockItem, external.Name));
					}					
				}

				return null;
			});
		}
	}


	[BCProcessor(typeof(BCConnector), BCEntitiesAttribute.ProductWithVariant, BCCaptions.TemplateItem, 70,
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
		URL = "products/{0}/edit",
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
		WebHookType = typeof(WebHookProduct),
		WebHooks = new String[]
		{
			"store/product/created",
			"store/product/updated",
			"store/product/deleted"
		}
		)]
	[BCProcessorExternCustomField(BCConstants.CustomFields, BigCommerceCaptions.CustomFields, nameof(ProductData.CustomFields), typeof(ProductData))]
	[BCProcessorExternCustomField(BCAPICaptions.Matrix, BCAPICaptions.Matrix, nameof(TemplateItems.Matrix), typeof(TemplateItems), readAsCollection: true, writeAsCollection: true)]
	public class BCTemplateItemProcessor : BCProductProcessor<BCTemplateItemProcessor, BCProductWithVariantEntityBucket, MappedTemplateItem, ProductData, TemplateItems>
	{
		private IChildRestDataProvider<ProductsOptionData> productsOptionRestDataProvider;
		private ISubChildRestDataProvider<ProductOptionValueData> productsOptionValueRestDataProvider;
		private IChildRestDataProvider<ProductsVariantData> productVariantRestDataProvider;
		protected IChildUpdateAllRestDataProvider<ProductsVariantData> productvariantBatchProvider;

		#region Factories
		[InjectDependency]
		private IBCRestDataProviderFactory<IChildRestDataProvider<ProductsOptionData>> productsOptionRestDataProviderFactory { get; set; }
		[InjectDependency]
		private IBCRestDataProviderFactory<ISubChildRestDataProvider<ProductOptionValueData>> productsOptionValueRestDataProviderFactory { get; set; }
		[InjectDependency]
		protected IBCRestDataProviderFactory<IChildUpdateAllRestDataProvider<ProductsVariantData>> productvariantBatchProviderFactory { get; set; }
		[InjectDependency]
		protected IBCRestDataProviderFactory<IChildRestDataProvider<ProductsVariantData>> productVariantRestDataProviderFactory { get; set; }
		#endregion		
		protected TemplateItemsMappingService templateItemsMappingService { get; set; }

		#region Constructor
		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation, cancellationToken);
			productsOptionRestDataProvider = productsOptionRestDataProviderFactory.CreateInstance(client);
			productsOptionValueRestDataProvider = productsOptionValueRestDataProviderFactory.CreateInstance(client);
			productvariantBatchProvider = productvariantBatchProviderFactory.CreateInstance(client);
			productVariantRestDataProvider = productVariantRestDataProviderFactory.CreateInstance(client);
			templateItemsMappingService = GetHelper<CommerceHelper>().GetExtension<TemplateItemsMappingService>();
		}
		#endregion

		#region Common
		public override async Task<MappedTemplateItem> PullEntity(Guid? localID, Dictionary<string, object> fields, CancellationToken cancellationToken = default)
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

		public override async Task<MappedTemplateItem> PullEntity(String externID, String jsonObject, CancellationToken cancellationToken = default)
		{
			ProductData data = await productDataProvider.GetByID(externID, new FilterProducts { Include = "variants,options" });
			if (data == null) return null;

			MappedTemplateItem obj = new MappedTemplateItem(data, data.Id?.ToString(), data.Name, data.DateModifiedUT.ToDate());

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
		/// <returns>The initialized entity</returns>
		protected override TemplateItems CreateEntityForGet()
		{
			TemplateItems entity = new TemplateItems();

			entity.ReturnBehavior = ReturnBehavior.OnlySpecified;
			entity.Attributes = new List<AttributeValue>() { new AttributeValue() };
			entity.Categories = new List<CategoryStockItem>() { new CategoryStockItem() };
			entity.FileURLs = new List<InventoryFileUrls>() { new InventoryFileUrls() };
			entity.Matrix = new List<ProductItem>() { new ProductItem() };
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
			return new MappedTemplateItem(entity, syncId, syncTime);
		}

		/// <inheritdoc/>
		public override object GetTargetObjectExport(object currentSourceObject, IExternEntity data, BCEntityExportMapping mapping)
		{
			ProductData productData = data as ProductData;
			ProductItem productItem = currentSourceObject as ProductItem;
			//Flag that indicates if we are trying to find a matching matrix to a variant.
			bool isMatrixToVariants = mapping.SourceObject.Contains(BCConstants.Matrix) && mapping.TargetObject.Contains(BCConstants.Variants);

			return (isMatrixToVariants) ?
				productData?.Variants?.FirstOrDefault(variant => variant.Sku == productItem.InventoryID.Value) :
				base.GetTargetObjectExport(currentSourceObject, data, mapping);
		}
		#endregion

		#region Import		
		public override async Task<PullSimilarResult<MappedTemplateItem>> PullSimilar(IExternEntity entity, CancellationToken cancellationToken = default)
		{

			var externalTemplate = entity as ProductData;
			if (externalTemplate == null || externalTemplate.Variants?.Count < 1)
				return null;

			//retrieve the list of skus and names
			var uniqueSKUs = externalTemplate.Variants
				.Select(variant => variant.Sku)
				.Where(sku => !string.IsNullOrWhiteSpace(sku))
				.Distinct()
				.ToList();

			var templates = ProductImportService.FindSimilarTemplateItems(uniqueSKUs, null, externalTemplate.Name);

			if (templates == null || !templates.Any())
				return null;

			return new PullSimilarResult<MappedTemplateItem>
			{
				UniqueField = externalTemplate.Name,
				Entities = templates.Select(item => new MappedTemplateItem((TemplateItems)item, item.Id, item.LastModifiedDateTime.Value)).ToList()
			};
		}

		public override async Task FetchBucketsForImport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			FilterProducts filter = new FilterProducts
			{
				Include = "variants,options",
				MinDateModified = minDateTime == null ? null : minDateTime.Value.AddDays(-1),
				MaxDateModified = maxDateTime == null ? null : maxDateTime.Value.AddDays(1),
			};

			// to force the code to run asynchronously and keep UI responsive.
			//In some case it runs synchronously especially when using IAsyncEnumerable
			await Task.Yield();
			await foreach (ProductData data in productDataProvider.GetAll(filter, cancellationToken))
			{
				if (data.BaseVariantId.HasValue) continue;
				BCProductWithVariantEntityBucket bucket = CreateBucket();

				MappedTemplateItem obj = bucket.Product = bucket.Product.Set(data, data.Id?.ToString(), data.Name, data.DateModifiedUT.ToDate(), data.CalculateHashOptional(nameof(ProductData.ProductsOptionData)));
				EntityStatus status = EnsureStatus(obj, SyncDirection.Import);
				RemoveInvalidVariant(data);
				AddOptionsConfigurationForImport(data, obj.SyncID, obj.ExternID);
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
			foreach (var option in product.ProductsOptionData)
			{
				foreach (var optionValue in option.OptionValues)
				{
					var externalOption = new ExternalTemplateOptions()
					{
						SyncID = syncId.Value,
						ExternalID = externId,
						OptionID = option.Id?.ToString(),
						OptionDisplayName = option.DisplayName,
						OptionName = option.Name,
						OptionValue = optionValue.Label,
						OptionSortOrder = option.SortOrder,
						OptionValueExternalId = optionValue.Id?.ToString(),
					};
					options.Add(externalOption);
				}
			}
			templateItemsMappingService.UpdateInsertOptions(options);
		}

		public override async Task<EntityStatus> GetBucketForImport(BCProductWithVariantEntityBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			FilterProducts filter = new FilterProducts { Include = "variants,options,images,modifiers" };
			ProductData data = await productDataProvider.GetByID(syncstatus.ExternID, filter);
			if (data == null) return EntityStatus.None;
			
			RemoveInvalidVariant(data);
			MappedTemplateItem obj = bucket.Product = bucket.Product.Set(data, data.Id?.ToString(), data.Name, data.DateModifiedUT.ToDate(), data.CalculateHashOptional(nameof(ProductData.ProductsOptionData)));
			EntityStatus status = EnsureStatus(obj, SyncDirection.Import);
			await base.AddSalesCategoryBucketForImport(bucket);

			return status;
		}

		/// <summary>
		/// Big Commerce creates by default one variant with the same SKU of the product depending of the method that the product was created.
		/// RemoveInvalidVariant objective is to remove this invalid method from the fetched data, so it can be treated and synced properly.
		/// </summary>
		/// <param name="externData"></param>
		public virtual void RemoveInvalidVariant(ProductData externData)
			=> externData.Variants?.Remove(externData.Variants?.FirstOrDefault(variant => variant.Sku == externData.Sku));


		/// <inheritdoc/>
		public override async Task MapBucketImport(BCProductWithVariantEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			ProductData external = bucket.Product.Extern;
			if (external == null && bucket.Product.Local != null)
				return;

			var syncid = bucket.Primary.SyncID;

			//Options may have changed since the prepare.
			AddOptionsConfigurationForImport(bucket.Product.Extern, syncid, bucket.Product.ExternID);

			var existingLocalTemplateItem = existing?.Local as TemplateItems;

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

			string subEntityType = external.Type == ProductTypes.physical.ToString() ? BCEntitiesAttribute.StockItem : BCEntitiesAttribute.NonStockItem;
			base.ProductImportService.Settings.SubEntityTypeForTemplate = subEntityType;

			MappedTemplateItem existingMapped = existing as MappedTemplateItem;
			TemplateItems existingData = existingMapped?.Local;

			Objects.BCBindingExt bindingExt = GetBindingExt<Objects.BCBindingExt>();
			TemplateItems local = MapProductImport<TemplateItems>(external, existingData, bindingExt.StockItemClassID);

			var itemClass = templateItemsMappingService.GetTemplateMappedItemClass(syncid);

			local.ItemClass = itemClass.ItemClassCD.ValueField();
			local.Attributes = new List<AttributeValue>();
			local.Attributes = templateItemsMappingService.GetAttributesDefitionsForClassID(itemClass.ItemClassID.Value)?.ToList();

			local.IsStockItem = local.RequireShipment = (external.Type == ProductTypes.physical.ToString()).ValueField();

			SetTaxCaterogyForProductItem(local, external, itemClass);

			if (existingData == null)
			{
				ProductImportService.ValidateItemClass(itemClass, itemClass?.ItemClassCD, external.Name, local.Attributes.Count());

				if (local.IsStockItem.Value == true)
					local.Availability = GetProductItemAvailabilityForStockItems(external).ValueField();
				else
				{
					local.Availability = GetProductItemAvailabilityForNonStockItems(external).ValueField();
					local.NotAvailable = BCItemNotAvailModes.StoreDefault.ValueField();
				}
			}

			local.DimensionWeight = external.Weight?.ValueField();

			if (local.IsStockItem.Value == false)
			{
				SetClassPropertiesForNonStockItems(local, itemClass);
			}

			MapSalesCategories(bucket, existingLocalTemplateItem, external, local);

			if (local.Matrix == null)
				local.Matrix = new List<ProductItem>();

			IEnumerable<TemplateMappedOptions> mappedOptions = templateItemsMappingService.GetMappedOptionsForTemplate(syncid);

			foreach (var variant in external.Variants)
			{
				ProductItem existingLocalVariant = null;
				//Check for existing Matrix Item.
				if (existingLocalTemplateItem != null)
				{
					var syncedNoteId = templateItemsMappingService.GetSyncedMatrixItemNoteID(syncid, variant.Id.ToString());

					if (syncedNoteId != null)
					{
						existingLocalVariant = existingLocalTemplateItem.Matrix.FirstOrDefault(x => x.NoteID?.Value == syncedNoteId);
					}
					else
					{
						var similarItems = ProductImportService.FindSimilarProductForSku<ProductItem>(variant.Sku, existingLocalTemplateItem.InventoryID.Value).Where(x => x.TemplateItemID != null).ToList();
						var templateId = similarItems?.FirstOrDefault()?.TemplateItemID;

						if (similarItems != null && similarItems.Count() == 1 && templateId != null && templateId.Value == existingLocalTemplateItem.InventoryID.Value)
						{
							var inventoryId = similarItems.FirstOrDefault()?.InventoryID?.Value?.Trim();
							existingLocalVariant = existingLocalTemplateItem.Matrix.FirstOrDefault(x => x.InventoryID.Value == inventoryId);
						}
					}

					if (existingLocalVariant is null)
						existingLocalVariant = this.GetSimilarLocalProductItem(existingLocalTemplateItem.Matrix, variant, mappedOptions);
				}

				if (local.IsStockItem.Value == true)
				{
					StockItem item = GetMappedVariant<StockItem>(variant, local, existingLocalTemplateItem, external, syncid, existingLocalVariant == null);

					item.InventoryID = existingLocalVariant?.InventoryID?.Value != null ?
							existingLocalVariant.InventoryID : ProductImportService.GetNewProductInventoryID(variant.Sku, item.Description?.Value).ValueField();

					MapSalesCategories(bucket, existingLocalVariant, external, item);
					item.NoteID = existingLocalVariant?.NoteID;

					item.SalesUOM = local.SalesUOM;
					item.NotAvailable = local.NotAvailable;
					var mappedStockItem = new MappedStockItemVariant(item, existingLocalVariant?.NoteID.Value, external.DateModifiedUT.ToDate());
					mappedStockItem.ExternID = variant.Id.ToString();
					mappedStockItem.ExternDescription = variant.Sku;
					mappedStockItem.Local = item;
					mappedStockItem.Extern = variant;
					mappedStockItem.LocalID = existingLocalVariant?.NoteID?.Value;

					GetHelper<BCHelper>().AddExternalSku(existingLocalVariant, item, variant.Sku);
					GetHelper<BCHelper>().AddGITN(existingLocalVariant, item, external.GTIN);

					bucket.StockItemVariants.Add(mappedStockItem);
					local.Matrix.Add(item);
				}
				else
				{
					NonStockItem item = GetMappedVariant<NonStockItem>(variant, local, existingLocalTemplateItem, external, syncid, existingLocalVariant == null);

					item.InventoryID = existingLocalVariant?.InventoryID?.Value != null ?
								existingLocalVariant.InventoryID : ProductImportService.GetNewProductInventoryID(variant.Sku, item.Description?.Value).ValueField();

					item.RequireShipment = new BooleanValue() { Value = false };
					item.NoteID = existingLocalVariant?.NoteID;

					SetClassPropertiesForNonStockItems(item, itemClass);
					MapSalesCategories(bucket, existingLocalVariant, external, item);

					var mappedNonStockItem = new MappedNonStockItemVariant(item, existingLocalVariant?.NoteID.Value, external.DateModifiedUT.ToDate());
					mappedNonStockItem.ExternID = variant.Id.ToString();
					mappedNonStockItem.ExternDescription = variant.Sku;
					mappedNonStockItem.Local = item;
					mappedNonStockItem.Extern = variant;
					mappedNonStockItem.LocalID = existingLocalVariant?.NoteID?.Value;

					GetHelper<BCHelper>().AddExternalSku(existingLocalVariant, item, variant.Sku);
					GetHelper<BCHelper>().AddBarCode(existingLocalVariant, item, external.GTIN);

					bucket.NonStockItemVariants.Add(mappedNonStockItem);
					local.Matrix.Add(item);
				}
			}

			bucket.Product.Local = local;
		}

		public virtual T GetMappedVariant<T>(ProductsVariantData variant, ProductItem local, ProductItem existingTemplateItem, ProductData exernalTemplate,  int? syncid, bool isFirstImport) where T : ProductItem, new()
		{
			var item = new T();
			item.ExternalSku = variant.Sku.ValueField();
			item.DimensionWeight = variant.Weight != null ? variant.Weight.ValueField() : local.DimensionWeight;

			item.Description = $"{variant.Sku} ({local.Description?.Value})".ValueField();
			
			item.TemplateItemID = local.InventoryID.Value.SearchField();

			//Price and MSRP should be mapped only for the first import.
			//First time the template is imported (therefore, the first time the variant is imported):
			//		- If the variant has a price, then the variant price is mapped to the template price.
			//		- If the variant has no price, then the template price is mapped to the variant price.
			//Template already imported but first time we import the variant (it has been added after the first import for instance)
			//		- If the variant has a price, then the variant price is mapped to the variant price.
			//		- If the variant has no price, then the existing template in the ERP price is mapped to the variant price.
			if (isFirstImport)
			{
				var templateDefaultPrice = existingTemplateItem == null ? exernalTemplate.Price?.ValueField() : existingTemplateItem.DefaultPrice;
				var templateMSRP = existingTemplateItem == null ? exernalTemplate.RetailPrice?.ValueField() : existingTemplateItem.MSRP;

				item.DefaultPrice = variant.Price != null ? variant.Price.ValueField() : templateDefaultPrice;
				item.MSRP = variant.RetailPrice != null ? variant.RetailPrice.ValueField() : templateMSRP;
			}

			item.PostingClass = local.PostingClass;
			item.ItemClass = local.ItemClass;
			item.TaxCategory = local.TaxCategory;
			item.Availability = local.Availability;
			item.Visibility = local.Visibility;
			item.ItemStatus = variant.PurchasingDisabled.Value == true ? PX.Objects.IN.Messages.NoSales.ValueField() : local.ItemStatus;
			item.DimensionWeight = variant.Weight.HasValue ? variant.Weight.Value.ValueField() : exernalTemplate.Weight?.ValueField();

			if (item.Attributes == null) item.Attributes = new List<AttributeValue>();
			var mappedOptions = templateItemsMappingService.GetMappedOptionsForTemplate(syncid);

			var rowNumber = 1;
			foreach (var option in variant.OptionValues)
			{
				var mappedOption = mappedOptions.FirstOrDefault(o => o.OptionID == option.OptionId.ToString() && o.OptionValue == option.Label);
				if (mappedOption == null)
					continue; //throw an exception.

				var attributeValue = new AttributeValue()
				{
					AttributeID = mappedOption.AttributeID.ValueField(),
					AttributeDescription = mappedOption.AttributeDescription.ValueField(),
					Value = mappedOption.Value.ValueField(),
					ValueDescription = mappedOption.Value.ValueField(),
					InventoryID = local.InventoryID,
					RowNumber = rowNumber++,
					IsActive = true.ValueField(),
				};
				item.Attributes.Add(attributeValue);
			}

			this.templateItemsMappingService.UpdateAttributeActivity(local, item);

			return item;
		}

		/// <summary>
		/// Add sync details for options and options' values
		/// </summary>
		/// <param name="bucket"></param>
		public virtual void AddOptionValuesDetails(BCProductWithVariantEntityBucket bucket)
		{
			var details = templateItemsMappingService.GetAttributesDetails(bucket.Product.SyncID);
			if (details == null || details.Count == 0) return;

			var byOption = details.GroupBy(x => x.OptionExternId).ToDictionary(x => x.Key, x => x.ToList());

			foreach(var option in byOption.Keys)
			{			
				if (bucket.Product.Details.Any(x=>x.ExternID == option))
					continue;
				var attribute = byOption[option].FirstOrDefault();
				bucket.Product.AddDetail(BCEntitiesAttribute.ProductOption, attribute.AttributeNoteId, option);

				//add values
				foreach(var value in byOption[option])
				{
					var key = new object[] { option, value.OptionValueExternId }.KeyCombine();
					if (bucket.Product.Details.Any(x => x.ExternID == key && x.EntityType == BCEntitiesAttribute.ProductOptionValue))
						continue;
					bucket.Product.AddDetail(BCEntitiesAttribute.ProductOptionValue, value.AttributeValueNoteId, key);
				}				
			}
		}

		public override async Task SaveBucketImport(BCProductWithVariantEntityBucket bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
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

		private void SaveStockDetails(TemplateItems impl, BCProductWithVariantEntityBucket bucket)
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

		private void SaveNonStockDetails(TemplateItems impl, BCProductWithVariantEntityBucket bucket)
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
		#endregion

		#region Export
		public override async Task<PullSimilarResult<MappedTemplateItem>> PullSimilar(ILocalEntity entity, CancellationToken cancellationToken = default)
		{
			List<ProductData> datas = new List<ProductData>();
			var uniqueField = await PullSimilar(((TemplateItems)entity)?.Description?.Value, ((TemplateItems)entity)?.InventoryID?.Value, datas, cancellationToken);
			return new PullSimilarResult<MappedTemplateItem>() { UniqueField = uniqueField, Entities = datas == null ? null : datas.Select(data => new MappedTemplateItem(data, data.Id.ToString(), data.Name, data.DateModifiedUT.ToDate())) };
		}
		
		public override async Task<EntityStatus> GetBucketForExport(BCProductWithVariantEntityBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			TemplateItems impl = GetTemplateItem(syncstatus.LocalID);
			if (impl == null) return EntityStatus.None;

			int? inventoryID = null;
			
			impl.AttributesDef = new List<AttributeDefinition>();
			impl.AttributesValues = new List<AttributeValue>();

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
				def.AttributeID = attribute.AttributeID.ValueField();
				def.Description = attribute.Description.ValueField();
				def.NoteID = attribute.NoteID.ValueField();
				def.Values = new List<AttributeDefinitionValue>();
				var group = (CSAttributeGroup)attributeDef;
				def.Order = group.SortOrder.ValueField();
				var attributedetails = PXSelect<CSAttributeDetail, Where<CSAttributeDetail.attributeID, Equal<Required<CSAttributeDetail.attributeID>>>>.Select(this, def.AttributeID.Value);
				foreach (CSAttributeDetail value in attributedetails)
				{
					AttributeDefinitionValue defValue = new AttributeDefinitionValue();
					defValue.NoteID = value.NoteID.ValueField();
					defValue.ValueID = value.ValueID?.Trim()?.ValueField();
					defValue.Description = value.Description.ValueField();
					defValue.SortOrder = value.SortOrder.ToInt().ValueField();
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
				AttributeValue def = new AttributeValue();
				def.AttributeID = attribute.AttributeID.ValueField();
				def.NoteID = inventory.NoteID.ValueField();
				def.InventoryID = inventory.InventoryCD.ValueField();
				def.Value = attribute.Value?.Trim()?.ValueField();
				impl.AttributesValues.Add(def);
			}
			impl.InventoryItemID = inventoryID;

			MappedTemplateItem obj = bucket.Product = bucket.Product.Set(impl, impl.SyncID, impl.SyncTime);
			EntityStatus status = EnsureStatus(obj, SyncDirection.Export);

			if (GetEntity(BCEntitiesAttribute.SalesCategory)?.IsActive == true)
			{
				if (obj.Local.Categories != null)
				{
					foreach (CategoryStockItem category in obj.Local.Categories)
					{
						BCSyncStatus result = PXSelectJoin<BCSyncStatus,
							InnerJoin<PX.Objects.IN.INCategory, On<PX.Objects.IN.INCategory.noteID, Equal<BCSyncStatus.localID>>>,
							Where<BCSyncStatus.connectorType, Equal<Current<BCEntity.connectorType>>,
								And<BCSyncStatus.bindingID, Equal<Current<BCEntity.bindingID>>,
								And<BCSyncStatus.entityType, Equal<Required<BCEntity.entityType>>,
								And<PX.Objects.IN.INCategory.categoryID, Equal<Required<PX.Objects.IN.INCategory.categoryID>>>>>>>
							.Select(this, BCEntitiesAttribute.SalesCategory, category.CategoryID.Value);
						if (result != null && result.ExternID != null && result.LocalID != null) continue;

						BCItemSalesCategory implCat = cbapi.Get<BCItemSalesCategory>(new BCItemSalesCategory() { CategoryID = new IntSearch() { Value = category.CategoryID.Value } });
						if (implCat == null) continue;

						MappedCategory mappedCategory = new MappedCategory(implCat, implCat.SyncID, implCat.SyncTime);
						EntityStatus mappedCategoryStatus = EnsureStatus(mappedCategory, SyncDirection.Export);
						if (mappedCategoryStatus == EntityStatus.Deleted)
							throw new PXException(BCMessages.CategoryIsDeletedForItem, category.CategoryID.Value, impl.Description.Value);
						if (mappedCategoryStatus == EntityStatus.Pending)
							bucket.Categories.Add(mappedCategory);

					}
				}
			}

			if (!string.IsNullOrWhiteSpace(syncstatus.ExternID))
			{
				ProductData product = await this.productDataProvider.GetByID(syncstatus.ExternID, new FilterProducts { Include = "variants,options" });
				if (product is not null && product.Variants is not null && product.Variants.Any())
				{
					foreach (var variant in product.Variants)
						bucket.ExternalVariants[variant.Id.Value] = variant;
				}
			}

			return status;
		}

		public override async Task MapBucketExport(BCProductWithVariantEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			TemplateItems local = bucket.Product.Local;
			ProductData externDataToBeExported = bucket.Product.Extern = new ProductData();

			MappedTemplateItem existingMapped = existing as MappedTemplateItem;
			ProductData existingExternData = existing?.Extern as ProductData;

			if (local.Matrix == null || local.Matrix?.Count == 0)
			{
				throw new PXException(BCMessages.NoMatrixCreated);
			}

			//Always try to remap options for export. If there are no option mappings in the database (BCOptionsMapping table)
			//it will be skipped.
			templateItemsMappingService.RemapVariantOptionsForExport(bucket.Product.SyncID, local, bucket?.Product?.ExternID);

			await MapInventoryItem(bucket, local, externDataToBeExported, existingExternData);
			MapCustomFields(local, externDataToBeExported);
			if (local.CustomURL?.Value != null)
				MapCustomUrl(existing, local.CustomURL?.Value, externDataToBeExported);
			MapVisibility(local, externDataToBeExported);

			MapProductOptions(local, externDataToBeExported, existingExternData);
			
			MapMetadata(local, externDataToBeExported, existingExternData);

			await MapProductVariants(bucket, existingMapped);

			MapAvailability(bucket, local, externDataToBeExported, existingExternData);
		}

		private async Task MapInventoryItem(BCProductWithVariantEntityBucket bucket, TemplateItems local, ProductData external, ProductData existingData)
		{
			external.Name = local.Description?.Value;
			external.Description = GetHelper<BCHelper>().ClearHTMLContent(local.Content?.Value);
			if (local.IsStockItem?.Value == false)
				external.Type = local.RequireShipment?.Value == true ? ProductsType.Physical.ToEnumMemberAttrValue() : ProductsType.Digital.ToEnumMemberAttrValue();
			else
			{
				external.Type = ProductsType.Physical.ToEnumMemberAttrValue();
				external.BinPickingNumber = local.DefaultIssueLocationID?.Value;

			}
			external.Price = await GetHelper<BCHelper>().RoundToStoreSetting(local.CurySpecificPrice?.Value);
			external.Weight = local.DimensionWeight.Value;
			external.CostPrice = local.CurrentStdCost.Value;
			external.RetailPrice = await GetHelper<BCHelper>().RoundToStoreSetting(local.CurySpecificMSRP?.Value);
			if (existingData == null)
				external.Sku = local.InventoryID?.Value;
			external.TaxClassId = taxClasses?.Find(i => i.Name.Equals(GetHelper<BCHelper>().GetSubstituteLocalByExtern(GetBindingExt<BCBindingExt>().TaxCategorySubstitutionListID, local.TaxCategory?.Value, String.Empty)))?.Id;
		}

		public virtual void MapCustomFields(TemplateItems local, ProductData external)
		{
			external.PageTitle = local.PageTitle?.Value;
			external.MetaDescription = local.MetaDescription?.Value;
			external.MetaKeywords = local.MetaKeywords?.Value != null ? local.MetaKeywords?.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null;
			external.SearchKeywords = local.SearchKeywords?.Value;

		}

		public virtual void MapVisibility(TemplateItems local, ProductData external)
		{
			string visibility = local?.Visibility?.Value;
			if (visibility == null || visibility == BCCaptions.StoreDefault) visibility = BCItemVisibility.Convert(GetBindingExt<BCBindingExt>().Visibility);
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

		public virtual void MapAvailability(BCProductWithVariantEntityBucket bucket, TemplateItems local, ProductData external, ProductData existingProduct)
		{
			string availability = local.Availability?.Value;

			if (availability == null || availability == BCCaptions.StoreDefault)
			{
				availability = BCItemAvailabilities.Convert(GetBindingExt<BCBindingExt>().Availability);
			}

			if (local.ItemStatus?.Value == PX.Objects.IN.Messages.Active || local.ItemStatus?.Value == PX.Objects.IN.Messages.NoPurchases || local.ItemStatus?.Value == PX.Objects.IN.Messages.NoRequest)
			{
				if (availability == BCCaptions.AvailableTrack)
				{
					external.Availability = BigCommerceConstants.AvailabilityAvailable;
					external.InventoryTracking = BigCommerceConstants.InventoryTrackingVariant;

					bool? positiveInventoryLevel = existingProduct?.InventoryLevel > 0;
					bool? purchasableVariants = external?.Variants.Any(v => v.PurchasingDisabled == false);

					if (existingProduct == null)
					{
						external.Availability = BigCommerceConstants.AvailabilityAvailable;
						external.InventoryTracking = BigCommerceConstants.InventoryTrackingVariant;
					}
					else if (positiveInventoryLevel == true && purchasableVariants == true)
					{
						external.Availability = BigCommerceConstants.AvailabilityAvailable;
						external.InventoryTracking = BigCommerceConstants.InventoryTrackingVariant;
					}
					else if (positiveInventoryLevel == false && purchasableVariants == true)
					{
						external.Availability = BigCommerceConstants.AvailabilityPreOrder;
						external.InventoryTracking = BigCommerceConstants.InventoryTrackingNone;
					}
					else if (purchasableVariants == false)
					{
						external.Availability = BigCommerceConstants.AvailabilityDisabled;
						external.InventoryTracking = BigCommerceConstants.InventoryTrackingVariant;
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

		public virtual void MapProductOptions(TemplateItems local, ProductData externDataToBeExported, ProductData existingExternData)
		{
			if (local.AttributesDef?.Count > 0)
			{
				foreach (var def in local.AttributesDef)
				{
					if (def.Values.Count > 0)
					{
						var descriptionDuplicated = def.Values.GroupBy(x => x.Description?.Value?.Trim()).Any(x => !string.IsNullOrEmpty(x.Key) && x.Count() > 1);

						if (descriptionDuplicated) throw new PXException(BCMessages.AttributeDuplicateOptionDescription, def.AttributeID.Value);
					}
				}

				foreach (var item in local.Matrix.Where(productItem => this.IsVariantActive(productItem)))
				{
					var def = local.AttributesValues.Where(x => x.NoteID.Value == item.Id).ToList();

					foreach (var attrValue in def)
					{
						if (attrValue.AttributeID.Value == null || attrValue.Value.Value == null) continue;

						var attribute = local.AttributesDef.FirstOrDefault(x => string.Equals(x.AttributeID.Value?.Trim(), attrValue.AttributeID.Value.Trim(), StringComparison.InvariantCultureIgnoreCase));
						if (attribute == null) continue;
						var value = attribute.Values.FirstOrDefault(y => string.Equals(y.ValueID.Value?.Trim(), attrValue.Value.Value.Trim(), StringComparison.InvariantCultureIgnoreCase));
						if (value == null) continue;

						ProductsOptionData productsOptionData = externDataToBeExported.ProductsOptionData.FirstOrDefault(x => x.LocalID == attribute.NoteID.Value);
						ProductsOptionData existingProductsOptionData = existingExternData?.ProductsOptionData.FirstOrDefault(x => x.DisplayName == attribute.Description?.Value);
						if (productsOptionData == null)
						{
							productsOptionData = new ProductsOptionData();
							productsOptionData.Name = attribute.AttributeID?.Value;
							productsOptionData.DisplayName = attribute.Description?.Value;
							productsOptionData.LocalID = attribute.NoteID?.Value;
							productsOptionData.Type = string.IsNullOrEmpty(existingProductsOptionData?.Type) ? BigCommerceOptionTypes.Dropdown : existingProductsOptionData?.Type;

							productsOptionData.Id = existingProductsOptionData?.Id;
							productsOptionData.SortOrder = attribute.Order?.Value.ToInt() ?? 0;
							externDataToBeExported.ProductsOptionData.Add(productsOptionData);
						}
						if (!productsOptionData.OptionValues.Any(x => x.LocalID == value.NoteID.Value))
						{
							ProductOptionValueData productOptionValueData = new ProductOptionValueData();
							productOptionValueData.Label = value.Description?.Value ?? value.ValueID?.Value;
							productOptionValueData.LocalID = value.NoteID?.Value;
							productOptionValueData.SortOrder = value.SortOrder?.Value ?? 0;
							var existingOptionValue = existingProductsOptionData?.OptionValues.FirstOrDefault(x => x.Label == productOptionValueData.Label);
							productOptionValueData.Id = existingOptionValue?.Id;
							if (string.Equals(productsOptionData.Type, BigCommerceOptionTypes.Swatch, StringComparison.InvariantCultureIgnoreCase))
							{
								if (existingOptionValue != null)// if existing then copy the value data as is
								{
									productOptionValueData.ValueData = existingOptionValue.ValueData;
								}
								else
								{ //if new option value is added for example in case of color option where
								  //type swatch then we reset type to drop down as value data is manadatory
									productsOptionData.Type = BigCommerceOptionTypes.Dropdown;
								}
							}
							productsOptionData.OptionValues.Add(productOptionValueData);
						}

					}
				}
			}
		}

		public virtual void MapMetadata(TemplateItems local, ProductData external, ProductData existingData)
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

					if (status?.ExternID == null)
					{
						continue;
					}

					external.Categories.Add(status.ExternID.ToInt().Value);
				}
				if ((external.Categories ?? Enumerable.Empty<int>()).Empty_())
				{
					String categories = null;
					if (local.IsStockItem?.Value == false)
						categories = GetBindingExt<BCBindingExt>().NonStockSalesCategoriesIDs;
					else
						categories = GetBindingExt<BCBindingExt>().StockSalesCategoriesIDs;

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

		public virtual async Task MapProductVariants(BCProductWithVariantEntityBucket bucket, MappedTemplateItem existing)
		{
			var externDataToBeExported = bucket.Product.Extern;
			var local = bucket.Product.Local;
			var obj = bucket.Product;
			var existingExternData = existing?.Extern;
			var existingSyncDetails = bucket.Product.Details.ToList();

			var existingExternProductVariants = existing?.Extern?.Variants ?? new List<ProductsVariantData>();
			//delete inactive variants
			existingSyncDetails.RemoveAll(x => obj.Local.Matrix.All(y => x.EntityType == BCEntitiesAttribute.Variant && (x.LocalID != y.Id || !IsVariantActive(y))));

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
				.Cast<PXResult<InventoryItem>>()?.ToList();

			foreach (var localVariant in obj.Local.Matrix.Where(x => IsVariantActive(x)))
			{
				var existingId = existingSyncDetails?.FirstOrDefault(x => x.LocalID == localVariant.Id)?.ExternID?.ToInt();
				ProductsVariantData existingVariant_old = existingExternProductVariants?.FirstOrDefault(x => (existingId != null && string.Equals(existingId, x.Id?.ToString())) ||
					string.Equals(x.Sku?.Trim(), localVariant.InventoryID.Value?.Trim(), StringComparison.OrdinalIgnoreCase));

				ProductsVariantData existingVariant = existingExternProductVariants?.FirstOrDefault(x => (existingId.HasValue && x.Id.HasValue && existingId.Value == x.Id.Value) ||
									(!string.IsNullOrEmpty(localVariant.ExternalSku?.Value) && string.Equals(x.Sku?.Trim(), localVariant.ExternalSku.Value?.Trim(), StringComparison.OrdinalIgnoreCase)) ||
									string.Equals(x.Sku?.Trim(), localVariant.InventoryID.Value?.Trim(), StringComparison.OrdinalIgnoreCase));


				existingId = existingVariant?.Id;

				List<PXResult<InventoryItem>> matchedInventoryItems = inventoryItems?.Where(x => x.GetItem<InventoryItem>().InventoryCD.Trim() == localVariant.InventoryID?.Value?.Trim()).ToList();
				InventoryItem matchedItem = matchedInventoryItems.FirstOrDefault()?.GetItem<InventoryItem>();

				ProductsVariantData externalVariant = new ProductsVariantData();

				await MapVariantInventoryItem(bucket, localVariant, externalVariant, existingId, matchedItem, matchedInventoryItems);
				MapVariantAvailability(local, localVariant, externalVariant, existingVariant, matchedItem);
				externDataToBeExported.Variants.Add(externalVariant);
			}

			// for all other variants, we need to determine whether we need to delete them in BigCommerce
			foreach (ProductsVariantData variant in existingExternProductVariants.Where(existingExtern => !externDataToBeExported.Variants.Any(variantsToExport => variantsToExport.Id == existingExtern.Id)
																							&& existingExtern.Id != existingExternData?.BaseVariantId))
			{

				//Since we import now local items, they come with SKUs of more than 10 characters.
				//Therefore: also check for the ExternalSku field
				//var matchMatrixItem = obj.Local.Matrix.FirstOrDefault(x => string.Equals(variant.Sku?.Trim(), x.InventoryID.Value?.Trim(), StringComparison.OrdinalIgnoreCase) || string.Equals(variant.Sku?.Trim(), x.ExternalSku.Value?.Trim(), StringComparison.OrdinalIgnoreCase));
				var matchMatrixItem = obj.Local.Matrix.FirstOrDefault(x => string.Equals(variant.Sku?.Trim(), x.InventoryID.Value?.Trim(), StringComparison.OrdinalIgnoreCase));

				if (matchMatrixItem is null)
					matchMatrixItem = this.GetSimilarLocalProductItem(obj.Local.Matrix, variant);

				// if there's no matrix item with a matching sku/inventoryID present in ERP then it means either the item has been deleted OR linked to another local item
				// in such case we need to delete such variants in BigCommerce
				if (matchMatrixItem == null)
					externDataToBeExported.VariantsToBeDeleted.Add(variant);
				// if there's a match matrix item and it is not an active variant, we export them but mark as not purchasable
				else if (!IsVariantActive(matchMatrixItem))
				{
					variant.PurchasingDisabled = true;
					externDataToBeExported.Variants.Add(variant);
				}
			}
		}

		private ProductItem GetSimilarLocalProductItem(IEnumerable<ProductItem> localProductItems, ProductsVariantData externalVariantData, IEnumerable<TemplateMappedOptions> mappedOptions = null)
		{
			foreach (ProductItem productItem in localProductItems)
			{
				//
				// Define if the local product attributes are similar to the external product options.
				//
				if (productItem.Attributes.SequenceEqual(externalVariantData.OptionValues, (AttributeValue attribute, ProductVariantOptionValueData option) => option.IsSimilarTo(attribute)))
					return productItem;

				if (mappedOptions is null || mappedOptions.IsEmpty())
					continue;

				//
				// Get the list of the local mapped options which correspond to the local product attributes.
				//
				List<TemplateMappedOptions> productMappedOptions = mappedOptions
					.Intersect(productItem.Attributes, (TemplateMappedOptions option, AttributeValue attribute) => option.IsSimilarTo(attribute))
					.ToList();

				//
				// Define if the local mapped options are similar to the external product options.
				//
				if (productMappedOptions.SequenceEqual(externalVariantData.OptionValues, (TemplateMappedOptions mappedOption, ProductVariantOptionValueData variantOption) => variantOption.IsSimilarTo(mappedOption)))
					return productItem;
			}

			return null;
		}

		[Obsolete(PX.Objects.Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		public virtual async Task MapVariantInventoryItem(TemplateItems local, ProductData external, ProductItem localVariant, ProductsVariantData externalVariant, int? existingId, InventoryItem matchedItem, List<PXResult<InventoryItem>> matchedInventoryItems) { }

		public virtual async Task MapVariantInventoryItem(BCProductWithVariantEntityBucket bucket, ProductItem localVariant, ProductsVariantData externalVariant, int? existingId, InventoryItem matchedItem, List<PXResult<InventoryItem>> matchedInventoryItems)
		{
			var external = bucket.Product.Extern;
			var local = bucket.Product.Local;

			externalVariant.LocalID = localVariant.Id;
			externalVariant.ProductId = external.Id;
			if (existingId != null) externalVariant.Id = existingId;

			bool isVariantSkuEmpty = !externalVariant.Id.HasValue
				|| !bucket.ExternalVariants.ContainsKey(externalVariant.Id.Value)
				|| string.IsNullOrWhiteSpace(bucket.ExternalVariants[externalVariant.Id.Value].Sku);

			string variantSku = isVariantSkuEmpty ? localVariant.InventoryID?.Value : bucket.ExternalVariants[externalVariant.Id.Value].Sku;

			externalVariant.Sku = variantSku;
			externalVariant.Price = await GetHelper<BCHelper>().RoundToStoreSetting(localVariant.DefaultPrice?.Value);
			externalVariant.RetailPrice = await GetHelper<BCHelper>().RoundToStoreSetting(localVariant.MSRP?.Value);
			externalVariant.Mpn = matchedInventoryItems?.FirstOrDefault(x => x.GetItem<INItemXRef>().AlternateType == INAlternateType.VPN)?.GetItem<INItemXRef>()?.AlternateID;

			externalVariant.Upc = GetCrossReferenceValueWithFallBack(localVariant, local.SalesUOM, local.BaseUOM, PX.Objects.IN.Messages.Barcode);
			externalVariant.Gtin = GetCrossReferenceValueWithFallBack(localVariant, local.SalesUOM, local.BaseUOM, PX.Objects.IN.Messages.GIN);
			externalVariant.Weight = (matchedItem.BaseItemWeight ?? 0) != 0 ? matchedItem.BaseItemWeight : local.DimensionWeight?.Value;
		}

		public virtual void MapVariantAvailability(TemplateItems parent, ProductItem local, ProductsVariantData external, ProductsVariantData existing, InventoryItem matchedInventoryItem)
		{
			string variantAvailability = BCItemAvailabilities.Convert(matchedInventoryItem.Availability);
			if (variantAvailability == null || variantAvailability == BCCaptions.StoreDefault)
			{
				variantAvailability = BCItemAvailabilities.Convert(GetBindingExt<BCBindingExt>().Availability);
			}

			string variantNotAvailable = BCItemAvailabilities.Convert(matchedInventoryItem.NotAvailMode);
			if (variantNotAvailable == null || variantAvailability == BCCaptions.StoreDefault)
			{
				variantNotAvailable = BCItemAvailabilities.Convert(GetBindingExt<BCBindingExt>().NotAvailMode);
			}

			string parentAvailability = parent?.Availability?.Value;
			if (parentAvailability == null || parentAvailability == BCCaptions.StoreDefault)
			{
				parentAvailability = BCItemAvailabilities.Convert(GetBindingExt<BCBindingExt>().Availability);
			}

			string variantStatus = local.ItemStatus?.Value;
			string parentStatus = parent.ItemStatus?.Value;
			if (parentStatus == PX.Objects.IN.Messages.Active || parentStatus == PX.Objects.IN.Messages.NoPurchases || parentStatus == PX.Objects.IN.Messages.NoRequest)
			{
				if (variantStatus == PX.Objects.IN.Messages.Active || variantStatus == PX.Objects.IN.Messages.NoPurchases || variantStatus == PX.Objects.IN.Messages.NoRequest)
				{
					if (variantAvailability == BCCaptions.AvailableTrack && parentAvailability == BCCaptions.AvailableTrack)
					{
						if (existing?.InventoryLevel > 0)
						{
							external.PurchasingDisabled = false;
						}
						else
						{
							external.PurchasingDisabled = false;
							if (variantNotAvailable == BCCaptions.DisableItem)
							{
								external.PurchasingDisabled = true;
							}
							else if (variantNotAvailable == BCCaptions.PreOrderItem || variantNotAvailable == BCCaptions.ContinueSellingItem || variantNotAvailable == BCCaptions.EnableSellingItem)
							{
								external.PurchasingDisabled = false;
							}
							else if (variantNotAvailable == BCCaptions.DoNothing || variantNotAvailable == BCCaptions.DoNotUpdate)
							{
								//If there is no existing product default to available
								external.PurchasingDisabled = existing?.PurchasingDisabled ?? false;
							}
						}
					}
					else if (variantAvailability == BCCaptions.AvailableSkip)
					{
						external.PurchasingDisabled = false;
					}
					else if (variantAvailability == BCCaptions.PreOrder)
					{
						external.PurchasingDisabled = false;
					}
					else if (variantAvailability == BCCaptions.DoNotUpdate)
					{
						external.PurchasingDisabled = existing?.PurchasingDisabled ?? false;
					}
					else if (variantAvailability == BCCaptions.Disabled)
					{
						external.PurchasingDisabled = true;
					}
					else
					{
						external.PurchasingDisabled = false;
					}
				}
				else if (variantStatus == PX.Objects.IN.Messages.Inactive || variantStatus == PX.Objects.IN.Messages.NoSales || variantStatus == PX.Objects.IN.Messages.ToDelete)
				{
					external.PurchasingDisabled = true;
				}
			}
			else if (parentStatus == PX.Objects.IN.Messages.Inactive || parentStatus == PX.Objects.IN.Messages.NoSales || parentStatus == PX.Objects.IN.Messages.ToDelete)
			{
				external.PurchasingDisabled = true;
			}
		}

		public virtual void MapVariantOptions(MappedTemplateItem obj, ProductItem item, ProductsVariantData variant)
		{
			variant.OptionValues = new List<ProductVariantOptionValueData>();
			var def = obj.Local.AttributesValues.Where(x => x.NoteID.Value == item.Id).ToList();
			foreach (var value in def)
			{
				ProductVariantOptionValueData optionValueData = new ProductVariantOptionValueData();
				var optionObj = obj.Local.AttributesDef.FirstOrDefault(x => x.AttributeID.Value == value.AttributeID.Value);
				if (optionObj == null) continue;

				var optionValueObj = optionObj.Values.FirstOrDefault(y => y.ValueID.Value == value.Value.Value);
				var detailObj = obj.Details.FirstOrDefault(x => x.LocalID == optionValueObj?.NoteID?.Value);
				if (detailObj == null) continue;

				optionValueData.OptionId = detailObj.ExternID.KeySplit(0).ToInt();
				optionValueData.Id = detailObj.ExternID.KeySplit(1).ToInt();
				optionValueData.Label = optionValueObj.ValueID.Value;
				optionValueData.OptionDisplayName = optionValueObj.Description.Value;

				variant.OptionValues.Add(optionValueData);
			}
		}

		public override object GetAttribute(BCProductWithVariantEntityBucket bucket, string attributeID)
		{
			return GetAttribute(bucket.Product?.Local, attributeID);
		}
		
		public override async Task SaveBucketExport(BCProductWithVariantEntityBucket bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
		{
			MappedTemplateItem obj = bucket.Product;
			MappedTemplateItem existingMapped = existing as MappedTemplateItem;
			ProductData existingExternData = existing?.Extern as ProductData;
			ProductData externDataToBeExported = null;
			List<DetailInfo> existingSyncDetails = null;
			try
			{
				ValidateLinks(existing, obj);

				obj.Extern.CustomFieldsData = await ExportCustomFields(obj, obj.Extern.CustomFields, externDataToBeExported, cancellationToken);

				//Save the variants to be synced separately after the parent product has been synced.
				var externVariantsToBeExported = obj.Extern.Variants;
				obj.Extern.Variants = null;

				if (obj.ExternID == null)
				{
					externDataToBeExported = await productDataProvider.Create(obj.Extern);
				}
				else
					externDataToBeExported = await productDataProvider.Update(obj.Extern, obj.ExternID);

				templateItemsMappingService.UpdateMappingOptionsForExport(bucket.Product.Local, obj.SyncID, externDataToBeExported.Id.ToString());

				externDataToBeExported.Variants = externVariantsToBeExported;
				externDataToBeExported.VariantsToBeDeleted = obj.Extern.VariantsToBeDeleted;

				existingSyncDetails = new List<DetailInfo>(obj.Details);
				obj.ClearDetails();
				existingSyncDetails.Where(x => x.EntityType != BCEntitiesAttribute.ProductOptionValue && x.EntityType != BCEntitiesAttribute.ProductOption && x.EntityType != BCEntitiesAttribute.Variant)?
					.ToList()
					.ForEach(x => obj.AddDetail(x.EntityType, x.LocalID, x.ExternID));

				await UpdateProductVariantOptions(obj, externDataToBeExported, existingSyncDetails, existingExternData);
				await UpdateProductVariant(obj, externDataToBeExported, existingExternData);
			}
			catch
			{
				existingSyncDetails?.ForEach(x =>
				{
					if (!obj.Details.Any(y => y.LocalID == x.LocalID))
						obj.AddDetail(x.EntityType, x.LocalID, x.ExternID);
				});
				throw;
			}

			obj.AddExtern(externDataToBeExported, externDataToBeExported.Id?.ToString(), externDataToBeExported.Name, externDataToBeExported.DateModifiedUT.ToDate());
			obj.ExternHash = externDataToBeExported.CalculateHashOptional(nameof(ProductData.ProductsOptionData));

			templateItemsMappingService.UpdateExternalIdForMappings(obj.SyncID, externDataToBeExported.Id?.ToString());

			await SaveImages(obj, obj.Local.FileURLs, cancellationToken);
			await SaveVideos(obj, obj.Local.FileURLs);
			UpdateStatus(obj, operation);
		}

		public virtual void ValidateLinks(IMappedEntity existing, MappedTemplateItem obj)
		{
			if (existing != null && (obj.Details == null || obj.Details?.Count() == 0))//only while linking to existing 
			{
				var existingProduct = existing.Extern as ProductData;
				if (existingProduct.ProductsOptionData.Count() != obj.Extern.ProductsOptionData.Count() || existingProduct.ProductsOptionData.Any(x => obj.Extern.ProductsOptionData.All(y => !string.Equals(y.DisplayName.Trim(), x.DisplayName?.Trim(), StringComparison.InvariantCultureIgnoreCase))))
				{
					throw new PXException(BigCommerceMessages.OptionsNotMatched, obj.ExternID);
				}
			}
		}

		public virtual async Task UpdateProductVariantOptions(MappedTemplateItem obj, ProductData data, List<DetailInfo> existingList, ProductData existing)
		{
			var existedProductOptionData = existing?.ProductsOptionData;
			//remove deleted attributes and values from BC
			var deletedOption = existingList.Where(x => obj.Extern.ProductsOptionData.All(y => x.LocalID != y.LocalID && x.EntityType == BCEntitiesAttribute.ProductOption)).ToList();
			if (deletedOption?.Count > 0)
			{
				foreach (var option in deletedOption)
				{
					//Check external ProductOptionData whether has data first
					if (existedProductOptionData?.Any(x => string.Equals(x.Id?.ToString(), option?.ExternID)) ?? false)
					{
						await productsOptionRestDataProvider.Delete(option?.ExternID, data.Id.ToString());
					}
					existingList.RemoveAll(x => x.LocalID == option.LocalID);
				}
			}

			var allOptionValues = obj.Extern.ProductsOptionData.SelectMany(y => y.OptionValues);
			var deletedValues = existingList.Where(x => allOptionValues.All(y => x.LocalID != y.LocalID && x.EntityType == BCEntitiesAttribute.ProductOptionValue)).ToList();
			//Check external Option values, find all values are not in the push list
			var shouldDelExternalValues = existedProductOptionData?.Count > 0 ? existedProductOptionData.SelectMany(x => x.OptionValues).
				Where(o => allOptionValues.Any(v => (v.Id != null && v.Id == o.Id) || (v.Id == null && string.Equals(v.Label, o.Label, StringComparison.OrdinalIgnoreCase))) == false).ToList() : null;
			if (deletedValues?.Count > 0)
			{
				foreach (var value in deletedValues)
				{
					if (existedProductOptionData?.Any(x => string.Equals(x.Id?.ToString(), value?.ExternID?.KeySplit(0))
					&& x.OptionValues.Any(v => string.Equals(v.Id?.ToString(), value?.ExternID?.KeySplit(1)))) ?? false)
					{
						existingList.RemoveAll(x => x.LocalID == value.LocalID);
					}
				}
			}

			foreach (var option in obj.Extern.ProductsOptionData)
			{
				var localObj = obj.Local.AttributesDef.FirstOrDefault(x => x.NoteID?.Value == option.LocalID);
				var detailObj = existingList?.Where(x => x.LocalID == localObj?.NoteID?.Value)?.ToList();
				ProductsOptionData existingOption = null;
				var savedOptionID = detailObj?.FirstOrDefault()?.ExternID;
				if (existedProductOptionData != null)
				{
					existingOption = existedProductOptionData.FirstOrDefault(x => (savedOptionID != null && string.Equals(savedOptionID, x.Id?.ToString())) || string.Equals(x.DisplayName?.Trim(), option.DisplayName?.Trim(), StringComparison.OrdinalIgnoreCase));
				}

				var optionID = existingOption?.Id?.ToString();
				ProductsOptionData response = null;
				if (optionID != null)
				{
					response = await productsOptionRestDataProvider.Update(option, optionID, data.Id.ToString());
					obj.AddDetail(BCEntitiesAttribute.ProductOption, localObj?.NoteID?.Value, optionID);
					foreach (var value in localObj.Values)
					{
						option.Id = optionID.ToInt();
						var optionValue = option.OptionValues.FirstOrDefault(x => x.LocalID == value.NoteID?.Value);
						if (optionValue == null) continue;
						var existingDetail = existingList.FirstOrDefault(x => x.LocalID == value.NoteID.Value);
						string optionValueID = existingDetail?.ExternID?.KeySplit(1);
						if (optionValueID == null)//check if there is existing non synced optionvalue at BC
							optionValueID = response?.OptionValues?.FirstOrDefault(x => string.Equals(x.Label?.Trim(), optionValue.Label?.Trim(), StringComparison.InvariantCultureIgnoreCase))?.Id?.ToString();
						if (optionValueID != null)
						{
							optionValue.Id = optionValueID.ToInt();
							await productsOptionValueRestDataProvider.Update(optionValue, data.Id.ToString(), optionID, optionValueID);
						}
						else
						{
							// if option value not present try to create it one by one as update Option api does not add new option values
							var optionValueResponse = await productsOptionValueRestDataProvider.Create(optionValue, data.Id.ToString(), optionID);
							if (optionValueResponse != null)
								obj.AddDetail(BCEntitiesAttribute.ProductOptionValue, value.NoteID?.Value, new object[] { optionID, optionValueResponse.Id.ToString() }.KeyCombine());
						}
					}
				}
				else
				{
					response = await productsOptionRestDataProvider.Create(option, data.Id.ToString());
					if (response != null)
						obj.AddDetail(BCEntitiesAttribute.ProductOption, localObj?.NoteID?.Value, response.Id.ToString());

				}
				
				foreach (ProductOptionValueData productOptionValueData in response?.OptionValues ?? Enumerable.Empty<ProductOptionValueData>())
					{
					Guid? localId = GetMatchingOptionDetailIDByLabel(localObj, productOptionValueData.Label);
					obj.AddDetail(BCEntitiesAttribute.ProductOptionValue, localId, new object[] { response.Id.ToString(), productOptionValueData.Id.ToString() }.KeyCombine());
					}
				}
			}

		/// <summary>
		/// Retrieves the attribute definition local id correspondent to external option value label.
		/// </summary>
		/// <param name="attributeDefinition">Local attribute definition</param>
		/// <param name="productOptionValueLabelData">External option value label data.</param>
		/// <returns>The <see cref="Guid"/> of the matching value or null.</returns>
		public virtual Guid? GetMatchingOptionDetailIDByLabel(AttributeDefinition attributeDefinition, string productOptionValueLabelData)
		{
			//We still have to support ValueID matching since many customers use them.
			List<AttributeDefinitionValue> matchingDefinitions = attributeDefinition.Values.Where(attributeDefinition =>
				string.Equals(attributeDefinition.Description?.Value, productOptionValueLabelData, StringComparison.InvariantCultureIgnoreCase)
				|| string.Equals(attributeDefinition.ValueID?.Value, productOptionValueLabelData, StringComparison.InvariantCultureIgnoreCase))
				?.ToList();

			AttributeDefinitionValue matchingDefinitionValue = (matchingDefinitions?.Count == 1)
				? matchingDefinitions.First()
				/* We should use description over ValueID as field to resolve conflict because:
				 * 1. It is not allowed to export option where there are duplicated descriptions, so for the connector this field is unique.
				 * 2. Description constrains more characters and it more often used by customers.
				 * 3. External store doesn't know what is ValueID, this is for internal use. They use Description as value (DisplayName).*/
				: matchingDefinitions?.FirstOrDefault(attributeDefinition => string.Equals(attributeDefinition.Description?.Value, productOptionValueLabelData, StringComparison.InvariantCultureIgnoreCase));

			return matchingDefinitionValue?.NoteID?.Value;
		}

		public virtual async Task UpdateProductVariant(MappedTemplateItem obj, ProductData externDataToBeExported, ProductData existingExternData)
		{
			List<ProductsVariantData> variantsToBeExported = externDataToBeExported.Variants.ToList();
			List<ProductsVariantData> variantsToBeDeleted = externDataToBeExported.VariantsToBeDeleted.ToList();

			// delete variants that do not exist in ERP
			if (variantsToBeDeleted.Any())
			{
				foreach (var variant in variantsToBeDeleted)
				{
					if (variant.Id != null)
						await productVariantRestDataProvider.Delete(variant.Id.ToString(), externDataToBeExported.Id.ToString());
				}
			}

			foreach (var item in variantsToBeExported)
			{
				item.ProductId = externDataToBeExported.Id;
				var localVariant = obj.Local.Matrix.FirstOrDefault(m => m.Id == item.LocalID);
				if (localVariant != null) MapVariantOptions(obj, localVariant, item);
			}

			await productvariantBatchProvider.UpdateAll(variantsToBeExported, (callback) =>
			{
				ProductsVariantData request = variantsToBeExported[callback.Index];
				if (callback.IsSuccess)
				{
					ProductsVariantData productsVariantData = callback.Result;
					obj.AddDetail(BCEntitiesAttribute.Variant, request.LocalID, productsVariantData.Id.ToString());
				}
				else
				{
					throw callback.Error;
				}

				return Task.CompletedTask;
			});
		}

		public virtual bool IsVariantActive(ProductItem item)
		{
			return !(item.ItemStatus?.Value == PX.Objects.IN.Messages.Inactive || item.ItemStatus?.Value == PX.Objects.IN.Messages.ToDelete || item.ItemStatus?.Value == PX.Objects.IN.Messages.NoSales)
				&& item.ExportToExternal?.Value == true;
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

		public virtual bool IsVariantPurchasable(ProductItem item, InventoryItem matchedItem)
		{
			return BCItemAvailabilities.Resolve(BCItemAvailabilities.Convert(matchedItem.Availability), GetBindingExt<BCBindingExt>().Availability) != BCCaptions.Disabled;
		}

		#endregion
	}
}
