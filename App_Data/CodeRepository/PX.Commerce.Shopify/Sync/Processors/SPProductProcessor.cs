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
using PX.Commerce.Shopify.API.GraphQL;
using PX.Commerce.Shopify.API.REST;
using PX.Commerce.Shopify.Sync.Processors.Utility;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.Common.Extensions;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Shopify
{
	public class SPProductEntityBucket<TPrimaryMapped> : EntityBucketBase, IEntityBucket
		where TPrimaryMapped : IMappedEntity
	{
		public IMappedEntity Primary => Product;
		public IMappedEntity[] Entities => new IMappedEntity[] { Primary };
		public TPrimaryMapped Product;
		public Dictionary<string, long?> VariantMappings = new Dictionary<string, long?>();

		/// <summary>
		/// Gets or sets the external product data.
		/// </summary>
		public ProductData ExternalProduct { get; set; }
	}

	public abstract class SPProductProcessor<TGraph, TEntityBucket, TPrimaryMapped, TExternType, TLocalType> :
		ProductProcessorBase<TGraph, TEntityBucket, TPrimaryMapped, TExternType, TLocalType>
		where TGraph : PXGraph
		where TEntityBucket : SPProductEntityBucket<TPrimaryMapped>, new()
		where TPrimaryMapped : class, IMappedEntityLocal<TLocalType>, new()
		where TExternType : BCAPIEntity, IExternEntity, new()
		where TLocalType : ProductItem, ILocalEntity, new()
	{
		protected IProductRestDataProvider<ProductData> productDataProvider;
		protected IChildRestDataProvider<ProductVariantData> productVariantDataProvider;
		protected IChildRestDataProvider<ProductImageData> productImageDataProvider;
		protected IEnumerable<ProductVariantData> ExternProductVariantData = new List<ProductVariantData>();
		protected Dictionary<int, string> SalesCategories;
		protected IMetafieldsGQLDataProvider metafieldDataGQLProvider;
		protected IProductGQLDataProvider ProductGQLDataProvider { get; set; }

		public ProductImportService ProductImportService { get; set; }

		#region Factories

		[InjectDependency]
		protected ISPRestDataProviderFactory<IProductRestDataProvider<ProductData>> productDataProviderFactory { get; set; }

		[InjectDependency]
		protected ISPRestDataProviderFactory<IChildRestDataProvider<ProductVariantData>> productVariantDataProviderFactory { get; set; }

		[InjectDependency]
		protected ISPRestDataProviderFactory<IChildRestDataProvider<ProductImageData>> productImageDataProviderFactory { get; set; }
		[InjectDependency]
		public ISPGraphQLDataProviderFactory<MetaFielsGQLDataProvider> metafieldGrahQLDataProviderFactory { get; set; }

		[InjectDependency]
		public ISPGraphQLAPIClientFactory shopifyGraphQLClientFactory { get; set; }

		[InjectDependency]
		public ISPMetafieldsMappingServiceFactory spMetafieldsMappingServiceFactory { get; set; }

		public SPGraphQLAPIClient shopifyGraphQLClient { get; set; }

		private ISPMetafieldsMappingService metafieldsMappingService;

		[InjectDependency]
		protected ISPGraphQLDataProviderFactory<ProductGQLDataProvider> SPGraphQLDataProviderFactory { get; set; }
		/// <summary>
		/// Factory used to create the Restclient factory used by any DataProviders.
		/// </summary>
		[InjectDependency]
		internal IShopifyRestClientFactory shopifyRestClientFactory { get; set; }

		/// <summary>
		/// Gets ot sets the product item availability provider factory.
		/// </summary>
		[InjectDependency]
		protected IAvailabilityProviderFactory AvailabilityProviderFactory { get; set; }

		#endregion

		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation, cancellationToken);

			var client = shopifyRestClientFactory.GetRestClient(GetBindingExt<BCBindingShopify>());

			productDataProvider = productDataProviderFactory.CreateInstance(client);
			productVariantDataProvider = productVariantDataProviderFactory.CreateInstance(client);
			productImageDataProvider = productImageDataProviderFactory.CreateInstance(client);

			var graphQLClient = shopifyGraphQLClientFactory.GetClient(GetBindingExt<BCBindingShopify>());
			metafieldDataGQLProvider = metafieldGrahQLDataProviderFactory.GetProvider(graphQLClient);
			metafieldsMappingService = spMetafieldsMappingServiceFactory.GetInstance(metafieldDataGQLProvider);
			ProductGQLDataProvider = SPGraphQLDataProviderFactory.GetProvider(graphQLClient);
			SalesCategories = new Dictionary<int, string>();

			ProductImportService = GetHelper<CommerceHelper>().GetExtension<ProductImportService>();
		}

		#region Common



		/// <summary>
		/// Initialize a new object of the entity to be used to Get bucket
		/// </summary>
		/// <returns>The initialized entity</returns>
		protected abstract TLocalType CreateEntityForGet();

		/// <inheritdoc/>
		protected override async Task DeleteImageFromExternalSystem(string parentId, string imageId)
		{
			try
			{
				await productImageDataProvider.Delete(parentId, imageId);
			}
			catch (RestException ex)
			{
				throw new PXException(ex, ShopifyMessages.ErrorDuringImageDeletionExceptionMessage, ex.Message);
			}
		}

		/// <summary>
		/// Calulates Default and Msrp prices if baseuom and sales UOn are different
		/// </summary>
		/// <param name="inventoryCD"></param>
		/// <param name="msrp"></param>
		/// <param name="defaultPrice"></param>
		public virtual void CalculatePrices(string inventoryCD, ref decimal? msrp, ref decimal? defaultPrice)
		{
			INUnit unit = null;
			var price = GetDefaultPrice(inventoryCD);
			if (price == null)
			{
				//convert based on converion rate
				unit = GetUnit(inventoryCD);
				defaultPrice = INUnitAttribute.ConvertValue(defaultPrice.Value, unit, INPrecision.UNITCOST);
			}
			else
			{
				defaultPrice = price;
			}

			if (msrp > 0)
			{
				if (unit == null)
				{
					unit = GetUnit(inventoryCD);
				}
				msrp = INUnitAttribute.ConvertValue(msrp.Value, unit, INPrecision.UNITCOST);
			}
		}

		public virtual INUnit GetUnit(string inventoryCD)
		{
			return PXSelectJoin<INUnit, InnerJoin<InventoryItem, On<INUnit.inventoryID, Equal<InventoryItem.inventoryID>
							   , And<INUnit.fromUnit, Equal<InventoryItem.salesUnit>>>>,
								   Where<INUnit.unitType, Equal<INUnitType.inventoryItem>,
									   And<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>>.Select(this, inventoryCD);
		}



		/// Return sales price from Sales prices form where
		/// 1.Price base type is base price
		/// 2 price is effective and not expired
		/// 3.Whose warehouse is null
		/// 4. price uom is equal to inventory sales unit
		/// 5.In case of multicurrency  price whose Currency matches with store base currency
		/// </summary>
		/// <param name="inventoryId"></param>
		/// <returns></returns>
		public virtual decimal? GetDefaultPrice(string inventoryCd)
		{
			decimal? price = null;
			var baseCurrency = Branch.PK.Find(this, GetBinding().BranchID)?.BaseCuryID.ValueField();
			var storeDefaultCurrency = GetBindingExt<Objects.BCBindingExt>().DefaultStoreCurrency;
			foreach (PXResult<ARSalesPrice, InventoryItem, INSite> item in PXSelectJoin<PX.Objects.AR.ARSalesPrice,
				InnerJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<ARSalesPrice.inventoryID>, And<ARSalesPrice.uOM, Equal<InventoryItem.salesUnit>>>,
				LeftJoin<INSite, On<INSite.siteID, Equal<ARSalesPrice.siteID>>>>,
				Where<ARSalesPrice.priceType, Equal<PriceTypes.basePrice>,
				And<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>>.Select(this, inventoryCd))
			{
				ARSalesPrice salesPrice = (ARSalesPrice)item;
				InventoryItem inventoryItem = (InventoryItem)item;
				INSite warehouse = (INSite)item;
				if (salesPrice != null && (salesPrice.CuryID ?? baseCurrency?.Value) == storeDefaultCurrency && salesPrice.TaxCalcMode != PX.Objects.TX.TaxCalculationMode.Gross)
				{
					if ((salesPrice.BreakQty ?? 1) > 1 || warehouse?.SiteCD != null ||
					(salesPrice.ExpirationDate != null && ((DateTime)salesPrice.ExpirationDate.Value).Date < PX.Common.PXTimeZoneInfo.Now.Date) ||
					(salesPrice.EffectiveDate != null && ((DateTime)salesPrice.EffectiveDate.Value).Date > PX.Common.PXTimeZoneInfo.Now.Date))
					{
						continue;
					}

					if (salesPrice.BreakQty == null || salesPrice.BreakQty == 1)//if brk qty is 1 use it
					{
						return salesPrice.SalesPrice;
					}

					price = salesPrice.SalesPrice; //if brk qty is 0 then  check if brkqty 1 is present if not then use 0
				}
			}
			return price;
		}

		public virtual async Task SaveImages(IMappedEntity obj, List<InventoryFileUrls> urls, CancellationToken cancellationToken = default)
		{
			var fileURLs = urls?.Where(x => x.FileType?.Value == BCCaptions.Image && !string.IsNullOrEmpty(x.FileURL?.Value))?.ToList();

			await SyncDeletedMediaUrls(obj, fileURLs);

			if (fileURLs == null || fileURLs.Count() == 0) return;

			List<ProductImageData> imageList = null;
			obj.ClearDetails(BCEntitiesAttribute.ProductImage);

			foreach (var image in fileURLs)
			{
				ProductImageData productImageData = null;
				try
				{
					if (imageList == null)
					{
						imageList = new List<ProductImageData>();
						// to force the code to run asynchronously and keep UI responsive.
						//In some case it runs synchronously especially when using IAsyncEnumerable
						await Task.Yield();
						await foreach (var data in productImageDataProvider.GetAll(obj.ExternID, new FilterWithFields() { Fields = "id,product_id,src,variant_ids,position" }, cancellationToken: cancellationToken))
							imageList.Add(data);
					}
					if (imageList?.Count > 0)
					{
						productImageData = imageList.FirstOrDefault(x => (x.Metafields != null && x.Metafields.Any(m => string.Equals(m.Key, ShopifyConstants.ProductImage, StringComparison.OrdinalIgnoreCase)
							&& string.Equals(m.Value, image.FileURL.Value, StringComparison.OrdinalIgnoreCase))));
						if (productImageData != null)
						{
							if (obj.Details?.Any(x => x.EntityType == BCEntitiesAttribute.ProductImage && x.LocalID == image.NoteID?.Value) == false)
							{
								obj.AddDetail(BCEntitiesAttribute.ProductImage, image.NoteID.Value, productImageData.Id.ToString());
							}
							continue;
						}
					};
					productImageData = new ProductImageData()
					{
						Src = Uri.EscapeUriString(System.Web.HttpUtility.UrlDecode(image.FileURL.Value)),
						Metafields = new List<MetafieldData>() { new MetafieldData() { Key = ShopifyConstants.ProductImage, Value = image.FileURL.Value, Type = ShopifyConstants.ValueType_SingleString, Namespace = BCObjectsConstants.Namespace_Global } },
					};
					var metafields = productImageData.Metafields;
					productImageData = await productImageDataProvider.Create(productImageData, obj.ExternID);
					productImageData.Metafields = metafields;
					if (obj.Details?.Any(x => x.EntityType == BCEntitiesAttribute.ProductImage && x.LocalID == image.NoteID?.Value) == false)
					{
						obj.AddDetail(BCEntitiesAttribute.ProductImage, image.NoteID.Value, productImageData.Id.ToString());
					}
					imageList = imageList ?? new List<ProductImageData>();
					imageList.Add(productImageData);
				}
				catch (Exception ex)
				{
					throw new PXException(ex.Message);
				}
			}
		}

		public virtual void SetProductStatus(ProductData data, string status, string availability, string visibility)
		{
			if (availability != BCCaptions.DoNotUpdate)
			{
				if (status.Equals(PX.Objects.IN.Messages.Inactive) || status.Equals(PX.Objects.IN.Messages.NoSales) || status.Equals(PX.Objects.IN.Messages.ToDelete))
				{
					data.Status = ProductStatus.Draft;
					data.Published = false;
				}
				else
				{
					data.Status = ProductStatus.Active;
					if (visibility == BCCaptions.Invisible || availability == BCCaptions.Disabled)
					{
						data.PublishedScope = PublishedScope.Web;
						data.Published = false;
					}
					else
					{
						data.PublishedScope = PublishedScope.Global;
						data.Published = true;
					}
				}
			}
		}

		public override List<(string fieldName, string fieldValue)> GetExternCustomFieldList(BCEntity entity, ExternCustomFieldInfo customFieldInfo)
		{
			List<(string fieldName, string fieldValue)> fieldsList = new List<(string fieldName, string fieldValue)>() { (BCConstants.MetafieldFormat, BCConstants.MetafieldFormat) };

			return fieldsList;
		}
		public override void ValidateExternCustomField(BCEntity entity, ExternCustomFieldInfo customFieldInfo, string sourceObject, string sourceField, string targetObject, string targetField, EntityOperationType direction)
		{
			//Validate the field format
			if (customFieldInfo.Identifier == BCConstants.ShopifyMetaFields)
			{
				var fieldStrGroup = direction == EntityOperationType.ImportMapping ? sourceField.Split('.') : targetField.Split('.');
				if (fieldStrGroup.Length == 2)
				{
					var keyFieldName = fieldStrGroup[0].Replace("[", "").Replace("]", "").Replace(" ", "");
					if (!string.IsNullOrWhiteSpace(keyFieldName) && string.Equals(keyFieldName, BCConstants.MetafieldFormat, StringComparison.OrdinalIgnoreCase) == false)
						return;
				}
				throw new PXException(BCMessages.InvalidFilter, "Target", BCConstants.MetafieldFormat);
			}
		}

		public override object GetExternCustomFieldValue(TEntityBucket entity, ExternCustomFieldInfo customFieldInfo, object sourceData, string sourceObject, string sourceField, out string displayName)
		{
			displayName = null;
			if (customFieldInfo.Identifier == BCConstants.ShopifyMetaFields)
			{
				return new List<object>() { sourceData };
			}
			else if (customFieldInfo.Identifier == BCAPICaptions.Matrix)
			{
				if (string.IsNullOrWhiteSpace(sourceField))
				{
					return ((TemplateItems)sourceData)?.Matrix ?? new List<ProductItem>();
					// we need make sure order of matrix items to be returned will be as same as that of Extern which are to be exported to Shopify
					// so that values to be mapped as specified in the Entities screen (if there's any) for Matrix Items will be correctly mapped 
					var matrixItems = ((ProductData)entity.Primary.Extern)?.Variants?.Select(x => ((TemplateItems)sourceData)?.Matrix?.FirstOrDefault(y => y.InventoryID.Value.Trim() == x.Sku)).ToList();
					return matrixItems ?? new List<ProductItem>();
				}

				var result = GetPropertyValue(sourceData, sourceField, out displayName);
				displayName = sourceData != null && sourceData is ProductItem ? $"{BCAPICaptions.Matrix}{BCConstants.Arrow} {((ProductItem)sourceData)?.InventoryID?.Value}" : displayName;
				return result;
			}
			return null;
		}

		public override void SetExternCustomFieldValue(TEntityBucket entity, ExternCustomFieldInfo customFieldInfo, object targetData, string targetObject, string targetField, string sourceObject, object value, IMappedEntity existing)
		{
			if (value == null)
			{
				LogWarning(Operation.LogScope(entity.Product), BCMessages.CustomFieldsWithoutValue, ((TLocalType)entity.Product.Local).InventoryID.Value, sourceObject);
				return;
			}
			else if (value == PXCache.NotSetValue)
				return;

			if (customFieldInfo.Identifier == BCConstants.MatrixItems)
			{
				SetPropertyValue(targetData, targetField, value);
				return;
			}

			if (customFieldInfo.Identifier != BCConstants.ShopifyMetaFields)
				return;

			var targetinfo = targetField?.Split('.');
			if (targetinfo.Length != 2)
				return;

			var nameSpaceField = targetinfo[0].Replace("[", "").Replace("]", "")?.Trim();
			var keyField = targetinfo[1].Replace("[", "").Replace("]", "")?.Trim();

			ProductData data = (ProductData)entity.Primary.Extern;
			ProductData existingProduct = existing?.Extern as ProductData;

			var entityType = customFieldInfo.ExternEntityType == typeof(ProductVariantData) ?
				ShopifyGraphQLConstants.OWNERTYPE_PRODUCTVARIANT : ShopifyGraphQLConstants.OWNERTYPE_PRODUCT;

			//Correct the metafield name - metafield names are case sensitive. But if we submit the same metafield name with different case, Shopify will raise an error
			//thus, we must check whethe the metafield exists or not and correct the name if needed
			var correctedName = metafieldsMappingService.CorrectMetafieldName(entityType, nameSpaceField, keyField, existingProduct?.Metafields);
			nameSpaceField = correctedName.Item1;
			keyField = correctedName.Item2;

			var metafieldValue = metafieldsMappingService.GetFormattedMetafieldValue(entityType, nameSpaceField, keyField, Convert.ToString(value));

			var newMetaField = new MetafieldData()
			{
				Namespace = nameSpaceField,
				Key = keyField,
				Value = metafieldValue.Value,
				Type = metafieldValue.GetShopifyType()
			};

			if (customFieldInfo.ExternEntityType == typeof(ProductData))
			{
				var metaFieldList = data.Metafields = data.Metafields ?? new List<MetafieldData>();
				if (existingProduct != null && existingProduct.Metafields?.Count > 0)
				{
					var existedMetaField = existingProduct.Metafields.FirstOrDefault(x => string.Equals(x.Namespace, nameSpaceField, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Key, keyField, StringComparison.OrdinalIgnoreCase));
					newMetaField.Id = existedMetaField?.Id;
					if (existedMetaField?.Type != null && !String.IsNullOrEmpty(existedMetaField.Type)) //always keep the original type of the metafield
						newMetaField.Type = existedMetaField.Type;
				}
				var matchedData = metaFieldList.FirstOrDefault(x => string.Equals(x.Namespace, nameSpaceField, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Key, keyField, StringComparison.OrdinalIgnoreCase));
				if (matchedData != null)
				{
					matchedData = newMetaField;
				}
				else
					metaFieldList.Add(newMetaField);

				return;
			}

			if (customFieldInfo.ExternEntityType == typeof(ProductVariantData))
			{
				bool anyFound = false;
				string matrixItemFormat = $"{BCAPICaptions.Matrix}{BCConstants.Arrow}";
				foreach (var variantItem in data.Variants)
				{
					var metaFieldList = variantItem.VariantMetafields = variantItem.VariantMetafields ?? new List<MetafieldData>();
					bool targetIsVariantData = targetData is ProductVariantData;
					bool sourceObjectHasMatrixItemFormat = sourceObject.StartsWith(matrixItemFormat);
					if (sourceObjectHasMatrixItemFormat || targetIsVariantData)
					{
						if (sourceObjectHasMatrixItemFormat && string.Equals(variantItem.Sku, sourceObject.Substring(matrixItemFormat.Length)?.Trim(), StringComparison.OrdinalIgnoreCase) ||
							(targetIsVariantData && string.Equals(((ProductVariantData)targetData)?.Sku, variantItem.Sku, StringComparison.OrdinalIgnoreCase)))
							anyFound = true;
						else
							continue;
					}
					else
					{
						//If source object is not from Matrix item itself, all variants should have the same metafield data, but the ID should be different in each variant metafield record.
						//We should avoid to reference the same metafield object and cause both of them have the same ID value, so create the new object each time.
						newMetaField = new MetafieldData()
						{
							Namespace = nameSpaceField,
							Key = keyField,
							Value = metafieldValue.Value,
							Type = metafieldValue.GetShopifyType()
						};
					}
					if (existingProduct?.Variants?.Count > 0)
					{
						var existedVariant = existingProduct.Variants.FirstOrDefault(x => string.Equals(x.Sku, variantItem.Sku, StringComparison.OrdinalIgnoreCase));
						if (existedVariant != null && existedVariant.VariantMetafields?.Count > 0)
						{
							var existedMetaField = existedVariant.VariantMetafields.FirstOrDefault(x => string.Equals(x.Namespace, nameSpaceField, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Key, keyField, StringComparison.OrdinalIgnoreCase));
							newMetaField.Id = existedMetaField?.Id;
						}
					}
					var matchedData = metaFieldList.FirstOrDefault(x => string.Equals(x.Namespace, nameSpaceField, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Key, keyField, StringComparison.OrdinalIgnoreCase));
					if (matchedData != null)
					{
						matchedData = newMetaField;
					}
					else
						metaFieldList.Add(newMetaField);
					if (anyFound) break;
				}
			}
		}

		protected virtual string GetBarcode(ProductItem entity, StringValue salesUnit, StringValue baseUnit)
		{
			return GetCrossReferenceValue(entity, salesUnit, PX.Objects.IN.Messages.GIN) ??
					GetCrossReferenceValue(entity, baseUnit, PX.Objects.IN.Messages.GIN) ??
					GetCrossReferenceValue(entity, null, PX.Objects.IN.Messages.GIN) ??
					GetCrossReferenceValue(entity, salesUnit, PX.Objects.IN.Messages.Barcode) ??
					GetCrossReferenceValue(entity, baseUnit, PX.Objects.IN.Messages.Barcode) ??
					GetCrossReferenceValue(entity, null, PX.Objects.IN.Messages.Barcode) ?? string.Empty;
		}

		protected virtual string GetCrossReferenceValue(ProductItem entity, StringValue UOMUnit, string AlternateTypeValue)
		{
			string result = null;

			if (!string.IsNullOrWhiteSpace(UOMUnit?.Value))
			{
				result = entity.CrossReferences?.FirstOrDefault(x => x.AlternateType?.Value == AlternateTypeValue && x.UOM?.Value == UOMUnit?.Value)?.AlternateID?.Value;
			}
			else
			{
				result = entity.CrossReferences?.FirstOrDefault(x => x.AlternateType?.Value == AlternateTypeValue)?.AlternateID?.Value;
			}

			return result;
		}

		#endregion

		#region Export
		public override async Task<EntityStatus> GetBucketForExport(TEntityBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			TLocalType item = CreateEntityForGet();
			TLocalType impl = cbapi.GetByID<TLocalType>(syncstatus.LocalID, item, GetCustomFieldsForExport());
			if (impl == null) return EntityStatus.None;

			TPrimaryMapped obj = bucket.Product = bucket.Product.Set(impl, impl.SyncID, impl.SyncTime);
			EntityStatus status = EnsureStatus(obj, SyncDirection.Export);
			if (!bucket.VariantMappings.ContainsKey(impl.InventoryID.Value))
				bucket.VariantMappings.Add(impl.InventoryID.Value, null);
			var categories = impl.Categories;
			if (categories != null)
			{
				foreach (CategoryStockItem category in categories)
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
				bucket.ExternalProduct = await this.productDataProvider.GetByID(syncstatus.ExternID, cancellationToken: cancellationToken);

			return status;
		}

		/// <summary>
		/// Gets the product SKU basing on the specified <paramref name="local"/> locally stored data and the specified <paramref name="external"/> external data.
		/// </summary>
		/// <param name="local">The locally stored product data.</param>
		/// <param name="external">The external product data.</param>
		/// <returns>The SKU.</returns>
		protected virtual string GetProductSku(ProductItem local, ProductData external)
		{
			string externalSku = external?.Variants?.FirstOrDefault().Sku;

			return string.IsNullOrWhiteSpace(externalSku)
				? local?.InventoryID?.Value
				: externalSku;
		}
		#endregion

		#region "Import"
		/// <inheritdoc/>
		public override async Task<bool> CheckExistingForImport(TEntityBucket bucket, BCSyncStatus status, CancellationToken cancellationToken = default)
		{
			var result = await base.CheckExistingForImport(bucket, status, cancellationToken);

			var existing = bucket.Product.Local as ProductItem;
			var external = bucket.Product.Extern as ProductData;
			var firstVariant = external.Variants.First();

			if (existing is null && this.ProductImportService.IsDuplicateInventoryCode(firstVariant.Sku))
				throw new PXException(ShopifyMessages.PIDuplicateInventoryCode, external.Title, firstVariant.Sku, this.GetBindingExt<BCBinding>().BindingName);

			return result;
		}

		/// <summary>
		/// Maps all the fields common between StockItems and Non Stock Items
		/// </summary>		
		/// <param name="external">The product from Shopify</param>
		/// <param name="variant">The first variant of the product</param>
		/// <param name="existing">The existing ERP product if it has already been synced</param>
		/// <param name="defaultItemClassId">The default class id (from the settings)</param>
		public virtual T MapProductImport<T>(ProductData external,ProductVariantData variant,
											 ProductItem existing, int? defaultItemClassId) where T : ProductItem, new()
		{
			var local = new T();

			//Validate that we can merge with the existing product item.
			//If we can't merge and an existing product exists with the same SKU, the method throws an exception.
			if (existing != null)
				ProductImportService.ValidateMergingWithExistingItem(existing, external.Id.ToString(), Operation.EntityType);

			local.Description = external.Title.ValueField();
			local.Content = external.BodyHTML.ValueField();
			local.ItemStatus = GetItemStatus(external).ValueField();

			local.InventoryID = existing?.InventoryID?.Value !=null ?
								existing.InventoryID : ProductImportService.GetNewProductInventoryID(variant?.Sku, external.Title).ValueField();

			local.Visibility = (external.Status == ProductStatus.Active ? BCItemVisibility.Visible : BCItemVisibility.Invisible).ValueField();
			local.DefaultPrice = variant.Price?.ValueField();
			local.MSRP = variant.OriginalPrice?.ValueField();

			if (existing is null)
				this.MapItemClassAndTaxCategory(external, local);

			this.UpdateVendorDetails(local, existing, external.Vendor);

			return local;
		}

		/// <summary>
		/// Maps the item class and related tax category.
		/// </summary>
		/// <param name="external">The external product data.</param>
		/// <param name="local">The local product data.</param>
		public virtual void MapItemClassAndTaxCategory<T>(ProductData external, T local)
			where T : ProductItem, new()
		{
			string itemClassId = this.ProductImportService.GetItemClassFromID(external.ProductType);

			if (string.IsNullOrWhiteSpace(itemClassId))
				return;

			local.ItemClass = itemClassId.ValueField();
			INItemClass itemClass = this.ProductImportService.SearchForItemClassByID(itemClassId);
			this.SetTaxCaterogyForProductItem(local, external, itemClass);
			this.ProductImportService.ValidateItemClass(itemClass, itemClassId, external.Title);
		}

		/// <summary>
		/// Set the tax category from substitution list.
		/// If no tax category is found, the method tries to get the tax category from the item class if item class is not null.
		/// NB: If tax category has been mapped, it will not be updated.
		/// </summary>
		/// <param name="productItem">The local product item.</param>
		/// <param name="external">The external product item.</param>
		/// <param name="itemClass">The item class.</param>
		public virtual void SetTaxCaterogyForProductItem(ProductItem productItem, ProductData external, INItemClass itemClass)
		{
			var taxCategoryHasCustomMapping = this.ImportMappings.Select().Select(x => x.GetItem<BCEntityImportMapping>().TargetField).Contains(nameof(productItem.TaxCategory));

			//if the tax category has been mapped (custom mapping) no need to update it.
			if (taxCategoryHasCustomMapping || external.Variants.IsEmpty())
				return;

			string taxCode = external.Variants.First().Taxable?.ToString();
			string taxCategory = !string.IsNullOrWhiteSpace(taxCode)
				? this.ProductImportService.GetTaxCategoryFromSubstitutionList(taxCode)
				: string.Empty;

			if (string.IsNullOrWhiteSpace(taxCategory) && itemClass != null)
				taxCategory = ProductImportService.GetTaxCategoryFromItemClass(itemClass.ItemClassCD);

			productItem.TaxCategory = taxCategory?.ValueField();

			if (string.IsNullOrWhiteSpace(productItem.TaxCategory?.Value))
				throw new PXException(BCMessages.PIINoTaxCategoryFound, external.Title);
		}

		/// <summary>
		/// Updates the vendor details of the specified <paramref name="local"/> with the specified <paramref name="vendorName"/>.
		/// </summary>
		/// <param name="local">The created product item to be sent to the CBAPI.</param>
		/// <param name="existing">The existing product item.</param>
		/// <param name="vendorName">The vendor name from the store.</param>
		public virtual void UpdateVendorDetails<T>(T local, ProductItem existing, string vendorName)
			where T : ProductItem
		{
			if (string.IsNullOrWhiteSpace(vendorName))
				return;

			var vendors = SelectFrom<Vendor>.Where<BAccount.acctName.IsEqual<P.AsString>>.View.Select(this, vendorName).RowCast<Vendor>();

			if (vendors is null || vendors.IsEmpty() || vendors.Count() > 1)
				return;

			local.VendorDetails ??= new List<ProductItemVendorDetail>();

			bool isVendorLinkedToProduct = false;

			if (existing?.VendorDetails?.Any() == true)
			{
				foreach (ProductItemVendorDetail vendorDetail in existing.VendorDetails)
				{
					ProductItemVendorDetail vendorDetailToLink = new() { RecordID = vendorDetail.RecordID };

					if (string.Equals(vendorDetail.VendorName.Value, vendorName, StringComparison.OrdinalIgnoreCase))
					{
						vendorDetailToLink.Default = true.ValueField();
						isVendorLinkedToProduct = true;
					}
					else
					{
						vendorDetailToLink.Default = false.ValueField();
					}

					local.VendorDetails.Add(vendorDetailToLink);
				}
			}

			if (isVendorLinkedToProduct)
				return;

			local.VendorDetails.Add(new ProductItemVendorDetail
			{
				VendorID = vendors.Single().AcctCD.ValueField(),
				Default = true.ValueField()
			});
		}

		public virtual string GetLocalWeightUnit(string unit)
		{
			switch (unit)
			{
				case ShopifyConstants.ShopifyWeightUnitGr:
				case ShopifyConstants.ShopifyWeightUnitOz:
					return "Kg";
				case ShopifyConstants.ShopifyWeightUnitLb:
					return "pound";
				default:
					return unit;
			}
		}
		public virtual decimal? GetWeight(decimal? weight, string unit)
		{
			if (weight == null)
				return null;
			if (weight == 0)
				return 0;

			switch (unit)
			{
				case ShopifyConstants.ShopifyWeightUnitGr:
					return weight * ShopifyConstants.ShopifyWeightUnitGrToKg;
				case ShopifyConstants.ShopifyWeightUnitOz:
					return weight * ShopifyConstants.ShopifyWeightUnitOzToKg;
				default:
					return weight;
			}
		}

		public virtual bool IsTemplateItem(ProductData product)
		{
			if (product.Variants.Count == 1 && product.Options != null && product.Options.Count == 1 && product.Options[0].Name == ShopifyConstants.RegularProductDefaultOptionName)
				return false;
			return true;
		}

		/// <summary>
		/// Returns the Item Status based on the imported product status
		/// </summary>
		/// <param name="external"></param>
		/// <returns></returns>
		/// <exception cref="PXInvalidOperationException"></exception>
		public virtual string GetItemStatus(ProductData external)
		{
			return external.Status switch
			{
				ProductStatus.Active => PX.Objects.IN.Messages.Active,
				ProductStatus.Draft => PX.Objects.IN.Messages.Inactive,
				ProductStatus.Archived => PX.Objects.IN.Messages.ToDelete,
				_ => throw new PXInvalidOperationException(BCMessages.PIInvalidProductStatus, external.Status.ToString(), external.Id)
			};
		}

		/// <summary>
		/// Fills the variant mappings within the specified <paramref name="bucket"/> with the specified <paramref name="productData"/> data.
		/// </summary>
		/// <param name="bucket">The bucket contains the variant mappings.</param>
		/// <param name="productData">The external product data to fill the variant mappings.</param>
		protected void FillVariantMappings<TMappedEntity>(SPProductEntityBucket<TMappedEntity> bucket, ProductData productData)
			where TMappedEntity : IMappedEntity
		{
			if (bucket is null
				|| productData is null
				|| productData.Variants is null
				|| productData.Variants.IsEmpty())
				return;

			foreach (ProductVariantData variant in productData.Variants)
			{
				if (string.IsNullOrWhiteSpace(variant.Sku))
					continue;

				bucket.VariantMappings[variant.Sku] = variant.Id;
			}
		}
		#endregion
	}
}
