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
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Shopify
{
	public class SPStockItemEntityBucket : SPProductEntityBucket<MappedStockItem>
	{
	}

	public class SPStockItemRestrictor : BCBaseRestrictor, IRestrictor
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
			return base.Restrict<MappedStockItem>(mapped, delegate (MappedStockItem obj)
			{
				ProductData external = obj.Extern;
				if (external.Variants.Count > 1)
				{
					return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogProductItemImportSkippedTemplateItem, external.Title));
				}

				if (external.Variants.Count == 1 && external.Options != null && external.Options.Count == 1 && external.Options[0].Name != ShopifyConstants.RegularProductDefaultOptionName)
				{
					return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogProductItemImportSkippedTemplateItem, external.Title));
				}

				if (external.Variants.Any(v => v.RequiresShipping == false))
				{
					return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogStockImportSkippedNonStockItem, external.Title));
				}

				if (external.Status == ProductStatus.Archived)
				{
					return new FilterResult(FilterStatus.Filtered, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.PIInvalidProductStatus, external.Title));
				}

				return null;
			});
		}
	}

	[BCProcessor(typeof(SPConnector), BCEntitiesAttribute.StockItem, BCCaptions.StockItem, 30,
		IsInternal = false,
		Direction = SyncDirection.Bidirect,
		PrimaryDirection = SyncDirection.Export,
		PrimarySystem = PrimarySystem.Local,
		PrimaryGraph = typeof(PX.Objects.IN.InventoryItemMaint),
		ExternTypes = new Type[] { typeof(ProductData) },
		LocalTypes = new Type[] { typeof(StockItem) },
		AcumaticaPrimaryType = typeof(PX.Objects.IN.InventoryItem),
		AcumaticaPrimarySelect = typeof(Search<
			PX.Objects.IN.InventoryItem.inventoryCD,
			Where<PX.Objects.IN.InventoryItem.stkItem, Equal<True>>>),
		URL = "products/{0}",
		Requires = new string[] { }
	)]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.ProductVideo, EntityName = BCCaptions.ProductVideo, AcumaticaType = typeof(BCInventoryFileUrls))]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.RelatedItem, EntityName = BCCaptions.RelatedItem, AcumaticaType = typeof(PX.Objects.IN.InventoryItem))]
	[BCProcessorRealtime(PushSupported = true, HookSupported = false,
		PushSources = new String[] { "BC-PUSH-Stocks" },
		PushDestination = BCConstants.PushNotificationDestination,
		WebHookType = typeof(WebHookMessage),
		WebHooks = new String[]
		{
			"products/create",
			"products/delete",
			"products/update"
		})]
	[BCProcessorExternCustomField(BCConstants.ShopifyMetaFields, ShopifyCaptions.Metafields, nameof(ProductData.Metafields), typeof(ProductData), writeAsCollection: false)]
	[BCProcessorExternCustomField(BCConstants.ShopifyMetaFields, ShopifyCaptions.Metafields, nameof(ProductVariantData.VariantMetafields), typeof(ProductVariantData), writeAsCollection: false)]
	public class SPStockItemProcessor : SPProductProcessor<SPStockItemProcessor, SPStockItemEntityBucket, MappedStockItem, ProductData, StockItem>
	{
		#region Fields
		/// <summary>
		/// The service used to map local entities to external entities and vice versa.
		/// </summary>
		private IEntityMappingService<ProductData, StockItem> _stockItemMappingService;

		private IAvailabilityProvider _availabilityProvider;
		#endregion

		#region Constructor
		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation, cancellationToken);

			_availabilityProvider = this.AvailabilityProviderFactory.CreateInstance<StockItemAvailabilityProvider>();
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
			ProductData data = await productDataProvider.GetByID(externID);
			if (data == null) return null;

			MappedStockItem obj = new MappedStockItem(data, data.Id?.ToString(), data.Title, data.DateModifiedAt.ToDate(false));

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
			return new MappedStockItem(entity, entity.SyncID, entity.SyncTime);
		}

		#endregion

		#region Import

		/// <inheritdoc />
		public override async Task<PullSimilarResult<MappedStockItem>> PullSimilar(IExternEntity entity, CancellationToken cancellationToken = default)
		{
			string uniqueStr = ((ProductData)entity)?.Variants?.FirstOrDefault()?.Sku;

			if (string.IsNullOrEmpty(uniqueStr))
				return null;

			var products = ProductImportService.FindSimilarProductForSku<StockItem>(uniqueStr);

			var entities = cbapi.GetAll<StockItem>(new StockItem() { CrossReferences = new List<InventoryItemCrossReference>() }, null, null, null);

			if (products == null || !products.Any())
				return null;

			FilterWithFields filter = new FilterWithFields
			{
				Fields = "Id, TemplateItemID, LastModifiedDateTime, NoteID, InventoryID",
			};

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
				UpdatedAtMin = minDateTime == null ? (DateTime?)null : minDateTime.Value.ToLocalTime().AddSeconds(-GetBindingExt<BCBindingShopify>().ApiDelaySeconds ?? 0),
				UpdatedAtMax = maxDateTime == null ? (DateTime?)null : maxDateTime.Value.ToLocalTime()
			};

			// to force the code to run asynchronously and keep UI responsive.
			//In some case it runs synchronously especially when using IAsyncEnumerable
			await Task.Yield();
			await foreach (ProductData data in productDataProvider.GetAll(filter, cancellationToken))
			{
				if (base.IsTemplateItem(data) || data.Variants.Any(v => v.RequiresShipping == false))
					continue;

				SPStockItemEntityBucket bucket = CreateBucket();

				IMappedEntity obj = bucket.Product = new MappedStockItem(data, data.Id?.ToString(), data.Title, data.DateModifiedAt.ToDate());
				EntityStatus status = EnsureStatus(obj, SyncDirection.Import);

				this.FillVariantMappings(bucket, data);
			}
		}
		public override async Task<EntityStatus> GetBucketForImport(SPStockItemEntityBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			ProductData data = await productDataProvider.GetByID(syncstatus.ExternID, includedMetafields: true, cancellationToken);

			if (data == null) return EntityStatus.None;

			if (data == null && syncstatus.LocalID.HasValue)
				return EntityStatus.Pending;

			this.FillVariantMappings(bucket, data);

			MappedStockItem obj = bucket.Product = bucket.Product.Set(data, data.Id?.ToString(), data.Title, data.DateModifiedAt.ToDate(false));
			EntityStatus status = EnsureStatus(obj, SyncDirection.Import);

			return status;
		}

		public override async Task MapBucketImport(SPStockItemEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			ProductData external = bucket.Product.Extern;

			MappedStockItem existingMapped = existing as MappedStockItem;

			StockItem existingData = existingMapped?.Local;

			ProductVariantData variant = external.Variants.FirstOrDefault();

			if (variant is null)
				throw new PXInvalidOperationException(BCMessages.PIProductHasNoVariant, external.Id);

			Objects.BCBindingExt bindingExt = GetBindingExt<Objects.BCBindingExt>();

			StockItem local = MapProductImport<StockItem>(external, variant, existingData, bindingExt.StockItemClassID);

			if (existingData == null)
			{
				local.Availability = _availabilityProvider.GetAvailablity(variant.InventoryManagement);

				if (variant.InventoryManagement == null)
				{
					local.NotAvailable = BCItemNotAvailModes.StoreDefault.ValueField();
				}
				else if (variant.InventoryManagement == ShopifyConstants.InventoryManagement_Shopify)
				{
					string notAvailableBehavior = variant.InventoryPolicy == InventoryPolicy.Continue
												? BCItemNotAvailModes.DoNothing : BCItemNotAvailModes.DisableItem;

					local.NotAvailable = notAvailableBehavior.ValueField();
				}
			}

			GetHelper<SPHelper>().AddExternalSku(existingData, local, variant.Sku);
			GetHelper<SPHelper>().AddBarCode(existingData, local, variant.Barcode);

			local.WeightUOM = GetLocalWeightUnit(variant.WeightUnit)?.ValueField();
			var weight = GetWeight(variant.Weight, variant.WeightUnit);
			local.DimensionWeight = weight == null ? null : weight.ValueField();
			bucket.Product.Local = local;
		}

		public override async Task SaveBucketImport(SPStockItemEntityBucket bucket, IMappedEntity existing, String operation, CancellationToken cancellationToken = default)
		{
			StockItem local = bucket.Product.Local;

			local = cbapi.Put<StockItem>(local, bucket.Product.LocalID);
			bucket.Product.AddLocal(local, local.SyncID, local.SyncTime);
			bucket.Product.AddVariantToDetails();

			UpdateStatus(bucket.Product, operation);
		}

		#endregion

		#region Export
		public override async Task<PullSimilarResult<MappedStockItem>> PullSimilar(ILocalEntity entity, CancellationToken cancellationToken = default)
		{
			var uniqueStr = ((StockItem)entity)?.InventoryID?.Value;
			List<ProductData> datas = null;
			if (!string.IsNullOrEmpty(uniqueStr))
			{
				var existingItems = (await ProductGQLDataProvider.GetProductVariantsAsync($"sku:{uniqueStr}", cancellationToken)).ToList();
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
					}
				}
			}

			return new PullSimilarResult<MappedStockItem>() { UniqueField = uniqueStr, Entities = datas == null ? null : datas.Select(data => new MappedStockItem(data, data.Id.ToString(), data.Title, data.DateModifiedAt.ToDate(false))) };
		}

		public override async Task MapBucketExport(SPStockItemEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			StockItem local = bucket.Product.Local;
			ProductData external = bucket.Product.Extern = new ProductData();

			MappedStockItem existingMapped = existing as MappedStockItem;
			ProductData existingData = existingMapped?.Extern;

			MapInventoryItem(bucket, local, external, existingData);

			var productVariant = external.Variants[0];

			MapAvailability(bucket, local, productVariant, existingData);
			MapTaxability(local, productVariant);
			MapMetadata(bucket, local, external);

			var visibility = local?.Visibility?.Value;
			if (visibility == null || visibility == BCCaptions.StoreDefault) visibility = BCItemVisibility.Convert(GetBindingExt<Objects.BCBindingExt>().Visibility);

			SetProductStatus(external, local.ItemStatus?.Value, productVariant.InventoryManagement, visibility);
		}

		public virtual void MapInventoryItem(SPStockItemEntityBucket bucket, StockItem local, ProductData external, ProductData existingData)
		{
			external.Title = local.Description?.Value;
			external.BodyHTML = GetHelper<SPHelper>().ClearHTMLContent(local.Content?.Value);
			external.ProductType = SelectFrom<INItemClass>
				.Where<INItemClass.itemClassCD.IsEqual<@P.AsString>>
				.View.Select(this, local.ItemClass.Value).FirstOrDefault()?.GetItem<INItemClass>().Descr;
			external.Vendor = (local.VendorDetails?.FirstOrDefault(v => v.Default?.Value == true) ?? local.VendorDetails?.FirstOrDefault())?.VendorName?.Value;
			external.Variants = external.Variants ?? new List<ProductVariantData>();
			decimal? price = local.CurySpecificPrice.Value;
			decimal? msrp = local.CurySpecificMSRP?.Value != null && local.CurySpecificMSRP?.Value > local.CurySpecificPrice.Value ? local.CurySpecificMSRP?.Value : 0;
			if (local.BaseUOM.Value != local.SalesUOM.Value)
			{
				CalculatePrices(local.InventoryID.Value.Trim(), ref msrp, ref price);
			}
			ProductVariantData productVariant = new ProductVariantData()
			{
				Id = bucket.VariantMappings.ContainsKey(local.InventoryID?.Value) ? bucket.VariantMappings[local.InventoryID?.Value] : null,
				Title = local.Description?.Value,
				Price = price,
				Sku = this.GetProductSku(bucket.Product.Local, bucket.ExternalProduct),
				OriginalPrice = msrp,
				Weight = local.DimensionWeight?.Value,
				WeightUnit = local.WeightUOM?.Value?.ToLower(),
				VariantMetafields = bucket.VariantMappings.ContainsKey(local.InventoryID?.Value) ? null :
					new List<MetafieldData>() { new MetafieldData() { Key = ShopifyConstants.Variant, Value = local.NoteID.Value.ToString(), Type = ShopifyConstants.ValueType_SingleString, Namespace = BCObjectsConstants.Namespace_Global } }
			};

			productVariant.Barcode = GetBarcode(local, local.SalesUOM, local.BaseUOM);

			external.Variants.Add(productVariant);
		}

		public virtual void MapAvailability(SPStockItemEntityBucket bucket, StockItem local, ProductVariantData external, ProductData existingProduct)
		{
			string availability = local.Availability?.Value;

			if (availability == null || availability == BCCaptions.StoreDefault)
			{
				availability = BCItemAvailabilities.Convert(GetBindingExt<BCBindingExt>().Availability);
			}
			string notAvailable = local.NotAvailable?.Value;

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
						external.InventoryPolicy = existingProduct?.Variants[0]?.InventoryPolicy ?? InventoryPolicy.Continue;
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
				else if (availability == BCCaptions.AvailableSkip)
				{
					external.InventoryManagement = null;
					external.InventoryPolicy = InventoryPolicy.Deny;
				}
				else if (availability == BCCaptions.PreOrder)
				{
					external.InventoryManagement = null;
					external.InventoryPolicy = InventoryPolicy.Deny;
				}
				else if (availability == BCCaptions.DisableItem)
				{
					external.InventoryManagement = null;
					external.InventoryPolicy = InventoryPolicy.Deny;
				}
				else if (availability == BCCaptions.DoNotUpdate)
				{
					// Value can be null or 'shopify' but if the external item does not exist, we need to go along with the SP standard behaviour - set 'shopify'
					external.InventoryManagement = existingProduct is null ? ShopifyConstants.InventoryManagement_Shopify : existingProduct.Variants[0]?.InventoryManagement;
					external.InventoryPolicy = existingProduct?.Variants[0]?.InventoryPolicy ?? InventoryPolicy.Deny;
				}
			}
			else if (local.ItemStatus?.Value == PX.Objects.IN.Messages.Inactive || local.ItemStatus?.Value == PX.Objects.IN.Messages.NoSales || local.ItemStatus?.Value == PX.Objects.IN.Messages.ToDelete)
			{
				external.InventoryManagement = null;
				external.InventoryPolicy = InventoryPolicy.Deny;
			}
		}

		public virtual void MapTaxability(StockItem local, ProductVariantData external)
		{
			bool isTaxable;
			bool.TryParse(GetHelper<SPHelper>().GetSubstituteLocalByExtern(GetBindingExt<BCBindingExt>().TaxCategorySubstitutionListID, local.TaxCategory?.Value, String.Empty), out isTaxable);
			external.Taxable = isTaxable;
		}

		public virtual void MapMetadata(SPStockItemEntityBucket bucket, StockItem local, ProductData external)
		{
			var categories = local.Categories?.Select(x => { if (SalesCategories.TryGetValue(x.CategoryID.Value.Value, out var desc)) return desc; else return string.Empty; }).Where(x => !string.IsNullOrEmpty(x)).ToList();
			//Put all categories to the Tags
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
				external.Metafields = new List<MetafieldData>() { new MetafieldData() { Key = ShopifyCaptions.Product, Value = local.NoteID.Value.ToString(), Type = ShopifyConstants.ValueType_SingleString, Namespace = BCObjectsConstants.Namespace_Global } };
				external.Metafields.Add(new MetafieldData() { Key = nameof(ProductTypes), Value = BCCaptions.StockItem, Type = ShopifyConstants.ValueType_SingleString, Namespace = BCObjectsConstants.Namespace_Global });
			}

		}

		public override object GetAttribute(SPStockItemEntityBucket bucket, string attributeID)
		{
			return GetAttribute(bucket.Product?.Local, attributeID);
		}

		public override void AddAttributeValue(SPStockItemEntityBucket bucket, string attributeID, object attributeValue)
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

		public override async Task SaveBucketExport(SPStockItemEntityBucket bucket, IMappedEntity existing, String operation, CancellationToken cancellationToken = default)
		{
			MappedStockItem obj = bucket.Product;
			ProductData data = null;
			if (obj.Extern.Categories?.Count > 0 && GetBindingExt<BCBindingShopify>()?.CombineCategoriesToTags == BCSalesCategoriesExportAttribute.SyncToProductTags)
			{
				obj.Extern.Tags = string.Join(",", obj.Extern.Categories) + (string.IsNullOrEmpty(obj.Extern.Tags) ? "" : $",{obj.Extern.Tags}");
			}
			try
			{
				if (obj.ExternID == null)
					data = await productDataProvider.Create(obj.Extern);
				else
				{
					var skus = obj.Extern.Variants.Select(x => x.Sku).ToList();
					var notExistedVariantIds = bucket.VariantMappings.Where(x => !skus.Contains(x.Key)).Select(x => x.Value).ToList();
					if (notExistedVariantIds?.Count > 0)
					{
						notExistedVariantIds.ForEach(async x =>
						{
							if (x != null) await productVariantDataProvider.Delete(obj.ExternID, x.ToString());
						});
					}
					data = await productDataProvider.Update(obj.Extern, obj.ExternID);
				}
			}
			catch (Exception ex)
			{
				throw new PXException(ex.Message);
			}

			obj.AddExtern(data, data.Id?.ToString(), data.Title, data.DateModifiedAt.ToDate(false));
			obj.AddVariantToDetails();

			await SaveImages(obj, obj.Local?.FileURLs, cancellationToken);

			UpdateStatus(obj, operation);
		}

		#endregion
	}
}
