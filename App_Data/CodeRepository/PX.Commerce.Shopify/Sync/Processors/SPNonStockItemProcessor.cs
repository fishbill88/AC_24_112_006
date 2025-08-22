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
using PX.Objects.Common.Extensions;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Shopify
{
	public class SPNonStockItemEntityBucket : SPProductEntityBucket<MappedNonStockItem> { }

	public class SPNonStockItemRestrictor : BCBaseRestrictor, IRestrictor
	{
		public virtual FilterResult RestrictExport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			#region StockItems
			return base.Restrict<MappedNonStockItem>(mapped, delegate (MappedNonStockItem obj)
			{
				if (obj.Local != null && obj.Local.TemplateItemID?.Value != null)
				{
					return new FilterResult(FilterStatus.Invalid,
						PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogNonStockSkippedVariant, obj.Local.InventoryID?.Value ?? obj.Local.SyncID.ToString()));
				}

				if (obj.Local != null && obj.Local.ExportToExternal?.Value == false)
				{
					return new FilterResult(FilterStatus.Invalid,
						PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogItemNoExport, obj.Local.InventoryID?.Value ?? obj.Local.SyncID.ToString()));
				}

				BCBindingExt bindingExt = processor.GetBindingExt<BCBindingExt>();
				if (bindingExt.GiftCertificateItemID != null && obj.Local?.InventoryID?.Value != null)
				{
					PX.Objects.IN.InventoryItem giftCertificate = bindingExt.GiftCertificateItemID != null ? PX.Objects.IN.InventoryItem.PK.Find((PXGraph)processor, bindingExt.GiftCertificateItemID) : null;
					if (giftCertificate != null && obj.Local?.InventoryID?.Value.Trim() == giftCertificate?.InventoryCD?.Trim())
					{
						return new FilterResult(FilterStatus.Invalid,
							PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogNonStockSkippedGift, obj.Local.InventoryID?.Value ?? obj.Local.SyncID.ToString()));
					}
				}

				if (bindingExt.RefundAmountItemID != null && obj.Local?.InventoryID?.Value != null)
				{
					PX.Objects.IN.InventoryItem refundItem = bindingExt.RefundAmountItemID != null ? PX.Objects.IN.InventoryItem.PK.Find((PXGraph)processor, bindingExt.RefundAmountItemID) : null;
					if (refundItem != null && obj.Local?.InventoryID?.Value.Trim() == refundItem?.InventoryCD?.Trim())
					{
						return new FilterResult(FilterStatus.Invalid,
							PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogNonStockSkippedRefund, obj.Local.InventoryID?.Value ?? obj.Local.SyncID.ToString()));
					}
				}
				return null;
			});

			#endregion
		}

		public virtual FilterResult RestrictImport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return base.Restrict<MappedNonStockItem>(mapped, delegate (MappedNonStockItem obj)
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

				if (external.Variants.Any(v => v.RequiresShipping == true))
				{
					return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogNonStockImportSkippedStockItem, external.Title));
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

	[BCProcessor(typeof(SPConnector), BCEntitiesAttribute.NonStockItem, BCCaptions.NonStockItem, 40,
		IsInternal = false,
		Direction = SyncDirection.Bidirect,
		PrimaryDirection = SyncDirection.Export,
		PrimarySystem = PrimarySystem.Local,
		PrimaryGraph = typeof(PX.Objects.IN.NonStockItemMaint),
		ExternTypes = new Type[] { typeof(ProductData) },
		LocalTypes = new Type[] { typeof(NonStockItem) },
		AcumaticaPrimaryType = typeof(PX.Objects.IN.InventoryItem),
		AcumaticaPrimarySelect = typeof(Search<PX.Objects.IN.InventoryItem.inventoryCD, Where<PX.Objects.IN.InventoryItem.stkItem, Equal<False>>>),
		URL = "products/{0}",
		Requires = new string[] { }
	)]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.ProductVideo, EntityName = BCCaptions.ProductVideo, AcumaticaType = typeof(BCInventoryFileUrls))]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.RelatedItem, EntityName = BCCaptions.RelatedItem, AcumaticaType = typeof(PX.Objects.IN.InventoryItem))]
	[BCProcessorRealtime(PushSupported = true, HookSupported = false,
		PushSources = new String[] { "BC-PUSH-NonStocks" },
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
	public class SPNonStockItemProcessor : SPProductProcessor<SPNonStockItemProcessor, SPNonStockItemEntityBucket, MappedNonStockItem, ProductData, NonStockItem>
	{
		private IAvailabilityProvider _availabilityProvider;

		#region Constructor
		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation, cancellationToken);

			_availabilityProvider = this.AvailabilityProviderFactory.CreateInstance<NonStockItemAvailabilityProvider>();
		}
		#endregion

		#region Common
		public override async Task<MappedNonStockItem> PullEntity(Guid? localID, Dictionary<string, object> externalInfo, CancellationToken cancellationToken = default)
		{
			NonStockItem impl = cbapi.GetByID(localID,
				new NonStockItem()
				{
					ReturnBehavior = ReturnBehavior.OnlySpecified,
					Attributes = new List<AttributeValue>() { new AttributeValue() },
					Categories = new List<CategoryStockItem>() { new CategoryStockItem() },
					CrossReferences = new List<InventoryItemCrossReference>() { new InventoryItemCrossReference() },
					VendorDetails = new List<ProductItemVendorDetail>() { new ProductItemVendorDetail() },
					FileUrls = new List<InventoryFileUrls>() { new InventoryFileUrls() },
				});
			if (impl == null) return null;

			MappedNonStockItem obj = new MappedNonStockItem(impl, impl.SyncID, impl.SyncTime);

			return obj;
		}
		public override async Task<MappedNonStockItem> PullEntity(String externID, String externalInfo, CancellationToken cancellationToken = default)
		{
			ProductData data = await productDataProvider.GetByID(externID);
			if (data == null) return null;

			MappedNonStockItem obj = new MappedNonStockItem(data, data.Id?.ToString(), data.Title, data.DateModifiedAt.ToDate(false));

			return obj;
		}

		/// <summary>
		/// Initialize a new object of the entity to be used to Fetch bucket
		/// </summary>
		/// <returns>The initialized entity</returns>
		protected override NonStockItem CreateEntityForFetch()
		{
			NonStockItem entity = new NonStockItem
			{
				InventoryID = new StringReturn(),
				TemplateItemID = new StringReturn(),
				Categories = new List<CategoryStockItem>() { new CategoryStockItem() { CategoryID = new IntReturn() } },
				ExportToExternal = new BooleanReturn()
			};

			return entity;
		}

		/// <summary>
		/// Initialize a new object of the entity to be used to Get bucket
		/// </summary>
		/// <returns>The initialized entity</returns>
		protected override NonStockItem CreateEntityForGet()
		{
			NonStockItem entity = new NonStockItem();

			entity.ReturnBehavior = ReturnBehavior.OnlySpecified;
			entity.Attributes = new List<AttributeValue>() { new AttributeValue() };
			entity.Categories = new List<CategoryStockItem>() { new CategoryStockItem() };
			entity.CrossReferences = new List<InventoryItemCrossReference>() { new InventoryItemCrossReference() };
			entity.VendorDetails = new List<ProductItemVendorDetail>() { new ProductItemVendorDetail() };
			entity.FileUrls = new List<InventoryFileUrls>() { new InventoryFileUrls() };

			return entity;
		}

		/// <summary>
		/// Creates a mapped entity for the passed entity
		/// </summary>
		/// <param name="entity">The entity to create the mapped entity from</param>
		/// <param name="syncId">The sync id of the entity</param>
		/// <param name="syncTime">The timestamp of the last modification</param>
		/// <returns>The mapped entity</returns>
		protected override MappedNonStockItem CreateMappedEntity(NonStockItem entity, Guid? syncId, DateTime? syncTime)
		{
			return new MappedNonStockItem(entity, entity.SyncID, entity.SyncTime);
		}

		#endregion

		#region Import

		public override async Task<PullSimilarResult<MappedNonStockItem>> PullSimilar(IExternEntity entity, CancellationToken cancellationToken = default)
		{
			string uniqueStr = ((ProductData)entity)?.Variants?.FirstOrDefault()?.Sku;

			if (string.IsNullOrEmpty(uniqueStr))
				return null;

			var products = ProductImportService.FindSimilarProductForSku<NonStockItem>(uniqueStr);
			if (products == null || !products.Any())
				return null;

			return new PullSimilarResult<MappedNonStockItem>
			{
				UniqueField = uniqueStr,
				Entities = products.Select(item => new MappedNonStockItem((NonStockItem)item, item.Id, item.LastModifiedDateTime.Value)).ToList()
			};
		}
		public override async Task FetchBucketsForImport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			//No DateTime filtering for Category
			FilterProducts filter = new FilterProducts
			{
				UpdatedAtMin = minDateTime?.ToLocalTime().AddSeconds(-GetBindingExt<BCBindingShopify>().ApiDelaySeconds ?? 0),
				UpdatedAtMax = maxDateTime?.ToLocalTime()
			};

			BCEntity entity = GetEntity(Operation.EntityType);
			int countNum = 0;
			List<IMappedEntity> mappedList = new List<IMappedEntity>();

			// to force the code to run asynchronously and keep UI responsive.
			//In some case it runs synchronously especially when using IAsyncEnumerable
			await Task.Yield();
			await foreach (ProductData data in productDataProvider.GetAll(filter, cancellationToken))
			{
				if (base.IsTemplateItem(data) || data.Variants.Any(v => v.RequiresShipping == true))
					continue;

				SPNonStockItemEntityBucket bucket = CreateBucket();

				IMappedEntity obj = bucket.Product = new MappedNonStockItem(data, data.Id?.ToString(), data.Title, data.DateModifiedAt.ToDate());

				if (entity.EntityType != obj.EntityType)
					entity = GetEntity(obj.EntityType);

				EntityStatus status = EnsureStatus(obj, SyncDirection.Import);

				this.FillVariantMappings(bucket, data);
			}
		}

		/// <inheritdoc/>	
		public override async Task<EntityStatus> GetBucketForImport(SPNonStockItemEntityBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			ProductData data = await productDataProvider.GetByID(syncstatus.ExternID, includedMetafields: true, cancellationToken);
			if (data == null) return EntityStatus.None;

			if (data == null && syncstatus.LocalID.HasValue)
				return EntityStatus.Pending;

			this.FillVariantMappings(bucket, data);

			var externalId = data?.Id?.ToString() ?? syncstatus.ExternID;
			MappedNonStockItem obj = bucket.Product = bucket.Product.Set(data, externalId, data?.Title, data?.DateModifiedAt.ToDate(false));
			EntityStatus status = EnsureStatus(obj, SyncDirection.Import);

			return status;
		}

		/// <inheritdoc/>	
		public override async Task MapBucketImport(SPNonStockItemEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			ProductData external = bucket.Product.Extern;
			if (external == null && bucket.Product.Local != null)
				return;

			MappedNonStockItem existingMapped = existing as MappedNonStockItem;
			NonStockItem existingData = existingMapped?.Local;

			ProductVariantData variant = external.Variants.FirstOrDefault();

			if (variant is null)
			{
				throw new PXInvalidOperationException(BCMessages.PIProductHasNoVariant, external.Id);
			}

			Objects.BCBindingExt bindingExt = GetBindingExt<Objects.BCBindingExt>();
			NonStockItem local = MapProductImport<NonStockItem>(external, variant, existingData, bindingExt.NonStockItemClassID);

			if (existingData == null)
			{
				local.Availability = _availabilityProvider.GetAvailablity(variant.InventoryManagement);
			}

			local.RequireShipment = new BooleanValue() { Value = false };

			GetHelper<SPHelper>().AddExternalSku(existingData, local, variant.Sku);
			GetHelper<SPHelper>().AddBarCode(existingData, local, variant.Barcode);

			bucket.Product.Local = local;
		}

		/// <inheritdoc/>
		public override async Task SaveBucketImport(SPNonStockItemEntityBucket bucket, IMappedEntity existing, String operation, CancellationToken cancellationToken = default)
		{
			NonStockItem local = bucket.Product.Local;
			MappedNonStockItem obj = bucket.Product;
			var external = bucket.Product.Extern;

			//Save the item
			local = cbapi.Put(local, bucket.Product.LocalID);
			bucket.Product.AddLocal(local, local.SyncID, local.SyncTime);
			obj.AddVariantToDetails();

			UpdateStatus(bucket.Product, operation);
		}
		#endregion

		#region Export

		public override async Task<PullSimilarResult<MappedNonStockItem>> PullSimilar(ILocalEntity entity, CancellationToken cancellationToken = default)
		{
			var uniqueStr = ((NonStockItem)entity)?.InventoryID?.Value;
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

			return new PullSimilarResult<MappedNonStockItem>() { UniqueField = uniqueStr, Entities = datas == null ? null : datas.Select(data => new MappedNonStockItem(data, data.Id.ToString(), data.Title, data.DateModifiedAt.ToDate(false))) };
		}

		public override async Task MapBucketExport(SPNonStockItemEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			NonStockItem local = bucket.Product.Local;
			ProductData external = bucket.Product.Extern = new ProductData();

			MappedNonStockItem existingMapped = existing as MappedNonStockItem;
			ProductData existingData = existingMapped?.Extern;

			MapInventoryItem(bucket, local, external, existingData);

			var productVariant = external.Variants[0];

			MapAvailability(bucket, local, productVariant, existingData);
			MapTaxability(local, productVariant);
			MapMetadata(bucket, local, external);

			string visibility = local?.Visibility?.Value;
			if (visibility == null || visibility == BCCaptions.StoreDefault) visibility = BCItemVisibility.Convert(GetBindingExt<BCBindingExt>().Visibility);

			SetProductStatus(external, local.ItemStatus?.Value, local.Availability?.Value, visibility);
		}

		public virtual void MapInventoryItem(SPNonStockItemEntityBucket bucket, NonStockItem local, ProductData external, ProductData existingData)
		{
			external.Title = local.Description?.Value;
			external.BodyHTML = GetHelper<SPHelper>().ClearHTMLContent(local.Content?.Value);
			external.ProductType = SelectFrom<INItemClass>
				.Where<INItemClass.itemClassCD.IsEqual<@P.AsString>>
				.View.Select(this, local.ItemClass.Value).FirstOrDefault()?.GetItem<INItemClass>().Descr;
			external.Vendor = (local.VendorDetails?.FirstOrDefault(v => v.Default?.Value == true) ?? local.VendorDetails?.FirstOrDefault())?.VendorName?.Value;
			decimal? price = local.CurySpecificPrice.Value;
			decimal? msrp = local.CurySpecificMSRP?.Value != null && local.CurySpecificMSRP?.Value > local.CurySpecificPrice.Value ? local.CurySpecificMSRP?.Value : 0;
			if (local.BaseUnit.Value != local.SalesUnit.Value)
			{
				CalculatePrices(local.InventoryID.Value.Trim(), ref msrp, ref price);
			}

			var productVariant = new ProductVariantData()
			{
				Id = bucket.VariantMappings.ContainsKey(local.InventoryID?.Value) ? bucket.VariantMappings[local.InventoryID?.Value] : null,
				Title = local.Description?.Value,
				Price = price,
				Sku = this.GetProductSku(bucket.Product.Local, bucket.ExternalProduct),
				OriginalPrice = msrp,
				Weight = local.DimensionWeight?.Value,
				WeightUnit = local.WeightUOM?.Value?.ToLower(),
				VariantMetafields = bucket.VariantMappings.ContainsKey(local.InventoryID?.Value) ? null :
								new List<MetafieldData>() { new MetafieldData() { Key = ShopifyConstants.Variant, Value = local.Id.Value.ToString(), Type = ShopifyConstants.ValueType_SingleString, Namespace = BCObjectsConstants.Namespace_Global } }
			};

			productVariant.Barcode = GetBarcode(local, local.SalesUnit, local.BaseUnit);

			external.Variants.Add(productVariant);
		}

		public virtual void MapMetadata(SPNonStockItemEntityBucket bucket, NonStockItem local, ProductData external)
		{
			//Put all categories to the Tags
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
				external.Metafields.Add(new MetafieldData() { Key = nameof(ProductTypes), Value = BCCaptions.NonStockItem, Type = ShopifyConstants.ValueType_SingleString, Namespace = BCObjectsConstants.Namespace_Global });
			}
		}

		public virtual void MapAvailability(SPNonStockItemEntityBucket bucket, NonStockItem local, ProductVariantData productVariant, ProductData existing)
		{
			// Value can be null or 'shopify' but if the external item does not exist, we need to go along with the SP standard behaviour - set 'shopify'
			string currentAvailability = existing != null ? existing?.Variants[0]?.InventoryManagement : ShopifyConstants.InventoryManagement_Shopify;
			productVariant.InventoryManagement = currentAvailability != BCCaptions.DoNotUpdate ? null : currentAvailability;
			productVariant.InventoryPolicy = bucket.Product.IsNew ? InventoryPolicy.Continue : existing?.Variants.FirstOrDefault()?.InventoryPolicy;
			productVariant.RequiresShipping = local.RequireShipment.Value ?? false;
		}

		public virtual void MapTaxability(NonStockItem local, ProductVariantData external)
		{
			bool isTaxable;
			bool.TryParse(GetHelper<SPHelper>().GetSubstituteLocalByExtern(GetBindingExt<BCBindingExt>().TaxCategorySubstitutionListID, local.TaxCategory?.Value, String.Empty), out isTaxable);
			external.Taxable = isTaxable;
		}

		public override object GetAttribute(SPNonStockItemEntityBucket bucket, string attributeID)
		{
			return GetAttribute(bucket.Product?.Local, attributeID);
		}

		public override void AddAttributeValue(SPNonStockItemEntityBucket bucket, string attributeID, object attributeValue)
		{
			MappedNonStockItem obj = bucket.Product;
			NonStockItem impl = obj.Local;
			impl.Attributes = impl.Attributes ?? new List<AttributeValue>();
			AttributeValue attribute = new AttributeValue();
			attribute.AttributeID = new StringValue() { Value = attributeID };
			attribute.Value = new StringValue() { Value = attributeValue?.ToString() };
			attribute.ValueDescription = new StringValue() { Value = attributeValue?.ToString() };
			impl.Attributes.Add(attribute);
		}

		public override async Task SaveBucketExport(SPNonStockItemEntityBucket bucket, IMappedEntity existing, String operation, CancellationToken cancellationToken = default)
		{
			MappedNonStockItem obj = bucket.Product;
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
					data = await productDataProvider.Update(obj.Extern, obj.ExternID);
					if (notExistedVariantIds?.Count > 0)
					{
						notExistedVariantIds.ForEach(async x =>
						{
							if (x != null) await productVariantDataProvider.Delete(obj.ExternID, x.ToString());
						});
					}
				}
			}
			catch (Exception ex)
			{
				throw new PXException(ex.Message);
			}

			obj.AddExtern(data, data.Id?.ToString(), data.Title, data.DateModifiedAt.ToDate(false));
			obj.AddVariantToDetails();

			await SaveImages(obj, obj.Local?.FileUrls);

			UpdateStatus(obj, operation);
		}

		#endregion
	}
}
