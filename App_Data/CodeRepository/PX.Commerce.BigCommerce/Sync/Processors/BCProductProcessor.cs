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
using PX.Commerce.Objects.Substitutes;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;
using PX.Objects.IN.RelatedItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce
{

	public class BCProductEntityBucket<TPrimaryMapped> : EntityBucketBase, IEntityBucket
		where TPrimaryMapped : IMappedEntity
	{
		public IMappedEntity Primary => Product;
		public IMappedEntity[] Entities => new IMappedEntity[] { Primary };
		public TPrimaryMapped Product;
		public List<MappedCategory> Categories = new List<MappedCategory>();

		/// <summary>
		/// Gets or sets the external product data.
		/// </summary>
		public ProductData ExternalProduct { get; set; }
	}
	public abstract class BCProductProcessor<TGraph, TEntityBucket, TPrimaryMapped, TExternType, TLocalType> :
		ProductProcessorBase<TGraph, TEntityBucket, TPrimaryMapped, TExternType, TLocalType>

		where TGraph : PXGraph
		where TEntityBucket : BCProductEntityBucket<TPrimaryMapped>, new()
		where TPrimaryMapped : class, IMappedEntityLocal<TLocalType>, new()
		where TExternType : BCAPIEntity, IExternEntity, new()
		where TLocalType : ProductItem, ILocalEntity, new()

	{
		private IChildRestDataProvider<ProductsImageData> productImageDataProvider;
		private IChildRestDataProvider<ProductsVideo> productVideoDataProvider;
		protected IStockRestDataProvider<ProductData> productDataProvider;
		protected IParentReadOnlyRestDataProvider<ProductsTaxData> taxDataProvider;
		protected IChildRestDataProvider<ProductsCustomFieldData> productsCustomFieldDataProvider;

		protected IParentRestDataProvider<ProductCategoryData> categoryDataProvider;

		public ProductImportService ProductImportService { get; set; }
		protected List<ProductsTaxData> taxClasses;
		protected IBigCommerceRestClient client;

		#region Factories
		[InjectDependency]
		protected IBCRestClientFactory bcRestClientFactory { get; set; }
		[InjectDependency]
		protected IBCRestDataProviderFactory<IChildRestDataProvider<ProductsImageData>> productImageDataProviderFactory { get; set; }
		[InjectDependency]
		protected IBCRestDataProviderFactory<IChildRestDataProvider<ProductsVideo>> productVideoDataProviderFactory { get; set; }
		[InjectDependency]
		protected IBCRestDataProviderFactory<IChildRestDataProvider<ProductsCustomFieldData>> productsCustomFieldDataProviderFactory { get; set; }
		[InjectDependency]
		protected IBCRestDataProviderFactory<IStockRestDataProvider<ProductData>> productDataProviderFactory { get; set; }
		[InjectDependency]
		protected IBCRestDataProviderFactory<IParentReadOnlyRestDataProvider<ProductsTaxData>> taxDataProviderFactory { get; set; }
		[InjectDependency]
		protected IBCRestDataProviderFactory<IParentRestDataProvider<ProductCategoryData>> categoryDataProviderFactory { get; set; }

		#endregion

		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation, cancellationToken);
			client = bcRestClientFactory.GetRestClient(GetBindingExt<BCBindingBigCommerce>());
			productDataProvider = productDataProviderFactory.CreateInstance(client);
			productImageDataProvider = productImageDataProviderFactory.CreateInstance(client);
			productVideoDataProvider = productVideoDataProviderFactory.CreateInstance(client);
			productsCustomFieldDataProvider = productsCustomFieldDataProviderFactory.CreateInstance(client);
			taxDataProvider = taxDataProviderFactory.CreateInstance(client);
			taxClasses = new List<ProductsTaxData>();
			// to force the code to run asynchronously and keep UI responsive.
			//In some case it runs synchronously especially when using IAsyncEnumerable
			await Task.Yield();
			await foreach (var data in taxDataProvider.GetAll())
				taxClasses.Add(data);

			ProductImportService = GetHelper<CommerceHelper>().GetExtension<ProductImportService>();

			if (ShouldImportSalesCategories)
			{ 
				categoryDataProvider = categoryDataProviderFactory.CreateInstance(client);
			}
		}


		/// <summary>
		/// Returns true when sales category is enabled and direction is import or bidirectional
		/// Used to enable/disable the sales category import as part of products import.
		/// </summary>
		protected bool ShouldImportSalesCategories
		{
			get
			{
				var entity = GetEntity(BCEntitiesAttribute.SalesCategory);
				if (entity?.IsActive == true &&
					(entity?.Direction == BCSyncDirectionAttribute.Import || entity?.Direction == BCSyncDirectionAttribute.Bidirect))
				{
					return true;
				}

				return false;
			}
		}
		/// <summary>
		/// Returns true when sales category is enabled and direction is export or bidirectional
		/// Used to enable/disable the sales category export as part of products import.
		/// </summary>
		protected bool ShouldExportSalesCategories
		{
			get
			{
				var entity = GetEntity(BCEntitiesAttribute.SalesCategory);
				if (entity?.IsActive == true &&
					(entity?.Direction == BCSyncDirectionAttribute.Export || entity?.Direction == BCSyncDirectionAttribute.Bidirect))
				{
					return true;
				}

				return false;
			}
		}

		#region Common

		/// <summary>
		/// Initialize a new object of the entity to be used to Get bucket
		/// </summary>
		/// <returns>The initialized entity</returns>
		protected abstract TLocalType CreateEntityForGet();

		public async Task<string> PullSimilar(string description, string inventoryId, List<ProductData> datas, CancellationToken cancellationToken = default)
		{
			string uniqueField = inventoryId;

			if (!string.IsNullOrEmpty(inventoryId))
			{
				// to force the code to run asynchronously and keep UI responsive.
				//In some case it runs synchronously especially when using IAsyncEnumerable
				await Task.Yield();
				await foreach (var data in productDataProvider.GetAll(new FilterProducts() { SKU = inventoryId }, cancellationToken))
					datas.Add(data);
			}
			if ((datas == null || datas.Count == 0) && !string.IsNullOrEmpty(description))
			{
				uniqueField = description;
				// to force the code to run asynchronously and keep UI responsive.
				//In some case it runs synchronously especially when using IAsyncEnumerable
				await Task.Yield();
				await foreach (var data in productDataProvider.GetAll(new FilterProducts() { Name = description }, cancellationToken))
					datas.Add(data);
			}

			if (datas == null) return null;
			var id = datas.FirstOrDefault(x => x.Id != null)?.Id;
			if (id != null)
			{
				var statuses = PXSelect<BCSyncStatus,
					Where<BCSyncStatus.connectorType, Equal<Required<BCSyncStatus.connectorType>>,
						And<BCSyncStatus.bindingID, Equal<Required<BCSyncStatus.bindingID>>,
						And<BCSyncStatus.externID, Equal<Required<BCSyncStatus.externID>>>>>>.Select(this, Operation.ConnectorType, Operation.Binding, id);
				if (statuses != null)
				{
					if ((Operation.EntityType == BCEntitiesAttribute.ProductWithVariant && statuses.Any(x => x.GetItem<BCSyncStatus>().EntityType == BCEntitiesAttribute.StockItem || x.GetItem<BCSyncStatus>().EntityType == BCEntitiesAttribute.NonStockItem)) ||
						(Operation.EntityType == BCEntitiesAttribute.StockItem && statuses.Any(x => x.GetItem<BCSyncStatus>().EntityType == BCEntitiesAttribute.ProductWithVariant || x.GetItem<BCSyncStatus>().EntityType == BCEntitiesAttribute.NonStockItem)) ||
						(Operation.EntityType == BCEntitiesAttribute.NonStockItem && statuses.Any(x => x.GetItem<BCSyncStatus>().EntityType == BCEntitiesAttribute.StockItem || x.GetItem<BCSyncStatus>().EntityType == BCEntitiesAttribute.ProductWithVariant)))
					{
						throw new PXException(BigCommerceMessages.MappedToOtherEntity, uniqueField);
					}

				}
			}

			return uniqueField;
		}

		public virtual void MapCustomUrl(IMappedEntity existing, string url, ProductData data)
		{
			ProductData existingProduct = existing?.Extern as ProductData;
			if (existingProduct?.CustomUrl?.Url == null && url != null)// For new Product
				data.CustomUrl = new ProductCustomUrl() { Url = url, IsCustomized = true };
			//Bigcommerce do not allow to update product with same custom Url so skip if url is same.
			if (existingProduct?.CustomUrl?.Url != null && url != null)
				if (!existingProduct.CustomUrl.Url.TrimEnd(new char[] { '/' }).Equals(url.TrimEnd(new char[] { '/' })))
					data.CustomUrl = new ProductCustomUrl() { Url = url, IsCustomized = true };
		}


		/// <inheritdoc/>
		protected override async Task DeleteImageFromExternalSystem(string parentId, string imageId)
		{
			try
			{
				await productImageDataProvider.Delete(imageId, parentId);
			}
			catch (RestException ex)
			{
				throw new PXException(ex, BigCommerceMessages.ErrorDuringImageDeletionExceptionMessage, ex.Message);
			}
		}

		public virtual async Task SaveImages(IMappedEntity obj, List<InventoryFileUrls> urls, CancellationToken cancellationToken = default)
		{
			var fileURLs = urls?.Where(x => x.FileType?.Value == BCCaptions.Image);

			await SyncDeletedMediaUrls(obj, fileURLs?.ToList());

			if (fileURLs == null) return;

			List<DetailInfo> existingList = new List<DetailInfo>(obj.Details);

			List<ProductsImageData> existingImages = new List<ProductsImageData>();
			// to force the code to run asynchronously and keep UI responsive.
			//In some case it runs synchronously especially when using IAsyncEnumerable
			await Task.Yield();
			await foreach (var data in productImageDataProvider.GetAll(obj.ExternID, cancellationToken))
				existingImages.Add(data);
			// clear all Product Image records from Details 
			obj.ClearDetails(BCEntitiesAttribute.ProductImage);


			foreach (var image in fileURLs)
			{
				if (!string.IsNullOrEmpty(image.FileURL?.Value))
				{
					var productImage = new ProductsImageData();
					productImage.ImageUrl = Uri.EscapeUriString(System.Web.HttpUtility.UrlDecode(image.FileURL?.Value));
					try
					{
						ProductsImageData response;
						if (existingList.Any(x => x.LocalID == image.NoteID?.Value) == false)
						{
							response = await productImageDataProvider.Create(productImage, obj.ExternID);
							if (response.Id > 0)
								obj.AddDetail(BCEntitiesAttribute.ProductImage, image.NoteID.Value, response.Id.ToString());
						}
						else
						{
							var detail = existingList.FirstOrDefault(x => x.LocalID == image.NoteID?.Value);

							if (int.TryParse(detail.ExternID, out int id) && id > 0)
							{
								// check if the image still exists in the external store before updating
								if (existingImages.Any(x => x.Id == id))
								{
									// the image still exists => update it
									response = await productImageDataProvider.Update(productImage, detail.ExternID, obj.ExternID);
									obj.AddDetail(BCEntitiesAttribute.ProductImage, image.NoteID.Value, detail.ExternID);
								}
								else
								{
									// the image no longer exists => create a new one
									response = await productImageDataProvider.Create(productImage, obj.ExternID);
									if (response.Id > 0)
									{
										// add a new detail record with the new ExternId 
										obj.AddDetail(BCEntitiesAttribute.ProductImage, image.NoteID.Value, response.Id.ToString());
									}
								}
							}
						}
					}
					catch (RestException ex)
					{
						if (ex.ResponceStatusCode == HttpStatusCode.BadRequest.ToString())
							throw new PXException(BigCommerceMessages.InvalidImage);
						throw;
					}
				}
			}
		}
		public virtual async Task SaveVideos(IMappedEntity obj, List<InventoryFileUrls> urls)
		{
			var fileURLs = urls?.Where(x => x.FileType?.Value == BCCaptions.Video);
			if (fileURLs == null) return;

			//map Videos
			foreach (var video in fileURLs)
			{
				if (!string.IsNullOrEmpty(video.FileURL?.Value) && obj.Details?.Any(x => x.LocalID == video.NoteID?.Value) == false)
				{
					var productVideo = new ProductsVideo();
					try
					{
						productVideo.VideoId = Regex.Match(video.FileURL?.Value, @"^.*(?:(?:youtu\.be\/|v\/|vi\/|u\/\w\/|embed\/)|(?:(?:watch)?\?v(?:i)?=|\&v(?:i)?=))([^#\&\?]*).*", RegexOptions.IgnoreCase).Groups[1].Value;
						ProductsVideo response = await productVideoDataProvider.Create(productVideo, obj.ExternID);
						obj.AddDetail(BCEntitiesAttribute.ProductVideo, video.NoteID.Value, response.Id.ToString());
					}
					catch (RestException ex)
					{
						if (ex.ResponceStatusCode == HttpStatusCode.Conflict.ToString())
							throw new PXException(BigCommerceMessages.InvalidVideo);
						throw;
					}
				}
			}

		}

		public virtual async Task<List<ProductsCustomFieldData>> ExportCustomFields(IMappedEntity obj, IList<ProductsCustomField> customFields, ProductData data, CancellationToken cancellationToken = default)
		{
			if (customFields == null || customFields.Count <= 0) return null;

			var cFields = new List<ProductsCustomFieldData>(customFields?.Select(c => c.Data));
			if (obj.ExternID != null && cFields != null)
			{
				List<ProductsCustomFieldData> externalcustomFields = new List<ProductsCustomFieldData>();
				// to force the code to run asynchronously and keep UI responsive.
				//In some case it runs synchronously especially when using IAsyncEnumerable
				await Task.Yield();
				await foreach (var fieldData in productsCustomFieldDataProvider.GetAll(obj.ExternID, cancellationToken))
					externalcustomFields.Add(fieldData);
				foreach (var cdata in cFields)
				{
					var extID = externalcustomFields.Where(x => x.Name == cdata.Name).FirstOrDefault();
					//Update Custom field if value is specified in local system
					if (extID != null && !String.IsNullOrEmpty(cdata.Value))
					{
						cdata.Id = extID.Id;
						//productsCustomFieldDataProvider.Update(cdata.Data, extID.Id.ToString(), data.Id.ToString());
					}
					//Delete Custom field if value is not specified in local system but exists in external
					else if (extID != null && String.IsNullOrEmpty(cdata.Value))
					{
						await productsCustomFieldDataProvider.Delete(extID.Id.ToString(), obj.ExternID);
					}
				}
			}
			return cFields.Where(x => !String.IsNullOrEmpty(x.Value))?.ToList();
		}
		public override List<(string fieldName, string fieldValue)> GetExternCustomFieldList(BCEntity entity, ExternCustomFieldInfo customFieldInfo)
		{
			return new List<(string fieldName, string fieldValue)>() { (BCConstants.AutoMapping, BCConstants.AutoMapping) };
		}
		public override void ValidateExternCustomField(BCEntity entity, ExternCustomFieldInfo customFieldInfo, string sourceObject, string sourceField, string targetObject, string targetField, EntityOperationType direction)
		{
			if (!string.IsNullOrEmpty(targetField) && targetField != BCConstants.AutoMapping && direction == EntityOperationType.ExportMapping)
			{
				throw new PXException(BCMessages.InvalidSourceFieldAutoMapping);
			}
			if (!string.IsNullOrEmpty(sourceField) && sourceField.StartsWith("=") && direction == EntityOperationType.ExportMapping)
			{
				throw new PXException(BCMessages.InvalidSourceFieldCustomFields);
			}
		}

		public override void SetExternCustomFieldValue(TEntityBucket entity, ExternCustomFieldInfo customFieldInfo,
			object targetData, string targetObject, string targetField, string sourceObject, object value, IMappedEntity existing)
		{
			if (value == null)
			{
				LogWarning(Operation.LogScope(entity.Product), BCMessages.CustomFieldsWithoutValue, ((TLocalType) entity.Product.Local).InventoryID.Value, sourceObject);
				return;
			}

			if (customFieldInfo.Identifier == BCConstants.MatrixItems)
			{
				SetPropertyValue(targetData, targetField, value);
				return;
			}

			if (value != PXCache.NotSetValue)
			{
				var sourceinfo = sourceObject?.Split('.');
				string sFieldName = (sourceinfo?.Length == 2) ? sourceinfo?[1] : sourceinfo?[0];
				if (!String.IsNullOrEmpty(sFieldName))
				{
					ProductData data = (ProductData)entity.Primary.Extern;
					data.CustomFields.Add(new ProductsCustomField() { Data = new ProductsCustomFieldData() { Id = null, Name = sFieldName, Value = value.ToString() } });
				}
			}
		}

		/// <inheritdoc/>
		public override object GetExternCustomFieldValue(TEntityBucket entity, ExternCustomFieldInfo customFieldInfo, object sourceData, string sourceObject, string sourceField, out string displayName)
		{
			displayName = null;
			if (customFieldInfo.Identifier == BCAPICaptions.Matrix && string.IsNullOrWhiteSpace(sourceField))
				return ((TemplateItems)sourceData)?.Matrix ?? new List<ProductItem>();

			return this.GetPropertyValue(sourceData, sourceField, out displayName);
		}

		public virtual List<int> MapRelatedItems(IMappedEntity obj)
		{
			BCBinding binding = GetBinding();
			string[] categoriesAllowed = GetBindingExt<BCBindingExt>().RelatedItems?.Split(',');
			Boolean anyRelation = false;
			List<int> ids = new List<int>();
			if (categoriesAllowed != null && categoriesAllowed.Count() > 0 && !String.IsNullOrWhiteSpace(categoriesAllowed[0]))
			{
				PXResultset<PX.Objects.IN.InventoryItem, INRelatedInventory, BCChildrenInventoryItem, BCSyncStatus> relates = PXSelectJoin<PX.Objects.IN.InventoryItem,
					InnerJoin<INRelatedInventory, On<PX.Objects.IN.InventoryItem.inventoryID, Equal<INRelatedInventory.inventoryID>>,
					InnerJoin<BCChildrenInventoryItem, On<INRelatedInventory.relatedInventoryID, Equal<BCChildrenInventoryItem.inventoryID>>,
					LeftJoin<BCSyncStatus, On<BCSyncStatus.localID, Equal<BCChildrenInventoryItem.noteID>,
						And<BCSyncStatus.connectorType, Equal<Required<BCSyncStatus.connectorType>>,
						And<BCSyncStatus.bindingID, Equal<Required<BCSyncStatus.bindingID>>,
						And<Where<BCSyncStatus.entityType, Equal<BCEntitiesAttribute.stockItem>, Or<BCSyncStatus.entityType, Equal<BCEntitiesAttribute.nonStockItem>>>
						>>>>>>>,
					   Where<PX.Objects.IN.InventoryItem.noteID, Equal<Required<PX.Objects.IN.InventoryItem.noteID>>>>
						.Select<PXResultset<PX.Objects.IN.InventoryItem, INRelatedInventory, BCChildrenInventoryItem, BCSyncStatus>>(this,
						binding.ConnectorType,
						binding.BindingID,
						obj.LocalID);

				if (relates?.Count > 0)
				{
					var existing = new List<DetailInfo>(obj.Details);
					obj.ClearDetails();
					existing.RemoveAll(x => x.EntityType == BCEntitiesAttribute.RelatedItem);
					foreach (var detail in existing)
					{
						if (!obj.Details.Any(x => x.EntityType == detail.EntityType && x.LocalID == detail.LocalID))
							obj.AddDetail(detail.EntityType, detail.LocalID, detail.ExternID);
					}
				}

				foreach (var rel in relates)
				{
					anyRelation = true;
					BCChildrenInventoryItem inventoryItem = rel.GetItem<BCChildrenInventoryItem>();
					INRelatedInventory row = rel.GetItem<INRelatedInventory>();
					if (row.IsActive == true
						&& categoriesAllowed.Contains(row.Relation)
						&& (row.ExpirationDate == null || row.ExpirationDate > DateTime.Now))
					{
						string relatedItemExternID = rel.GetItem<BCSyncStatus>().ExternID;
						if (relatedItemExternID != null)
							ids.Add((int)relatedItemExternID.ToInt());
						if (!obj.Details.Any(x => x.EntityType == BCEntitiesAttribute.RelatedItem && x.LocalID == inventoryItem.NoteID))
							obj.AddDetail(BCEntitiesAttribute.RelatedItem, inventoryItem.NoteID, relatedItemExternID);
					}
				}
			}
			return anyRelation ? ids : null;
		}

		public virtual async Task UpdateRelatedItems(IMappedEntity obj)
		{
			string[] categoriesAllowed = GetBindingExt<BCBindingExt>()?.RelatedItems?.Split(',');
			BCBinding binding = GetBinding();
			if (categoriesAllowed != null && categoriesAllowed.Count() > 0 && !String.IsNullOrWhiteSpace(categoriesAllowed[0]))
			{
				List<IMappedEntity> relatedMappedProducts = new List<IMappedEntity>();
				List<RelatedProductsData> relatedProductsData = new List<RelatedProductsData>();
				foreach (PXResult<BCSyncDetail, BCSyncStatus> relatedItems in PXSelectJoin<BCSyncDetail,
							InnerJoin<BCSyncStatus, On<BCSyncStatus.syncID, Equal<BCSyncDetail.syncID>>>,
							Where<BCSyncStatus.connectorType, Equal<Current<BCEntity.connectorType>>,
								And<BCSyncStatus.bindingID, Equal<Current<BCEntity.bindingID>>,
								And<BCSyncDetail.entityType, Equal<Required<BCSyncDetail.entityType>>,
								And<BCSyncDetail.localID, Equal<Required<BCSyncDetail.localID>>,
								And<BCSyncDetail.externID, IsNull>>>>>>.Select(this, BCEntitiesAttribute.RelatedItem, obj.LocalID))
				{
					var pstatus = relatedItems.GetItem<BCSyncStatus>();
					var detail = relatedItems.GetItem<BCSyncDetail>();
					IMappedEntity item;
					if (pstatus.EntityType.Equals(BCEntitiesAttribute.NonStockItem))
					{
						item = new MappedNonStockItem() { SyncID = pstatus.SyncID }.Set(pstatus);

					}
					else
					{
						item = new MappedStockItem() { SyncID = pstatus.SyncID }.Set(pstatus);
					}
					item.LocalTimeStamp = DateTime.MaxValue;
					EnsureDetails(item);
					relatedMappedProducts.Add(item);
					var relatedIds = new List<int>() { (int)obj.ExternID.ToInt() };
					var existingIds = item.Details?.Where(x => x.ExternID != null)?.Select(x => (int)x.ExternID.ToInt());
					if (existingIds?.Count() > 0)
						relatedIds.AddRange(existingIds);
					relatedProductsData.Add(new RelatedProductsData()
					{
						Id = item.ExternID.ToInt(),
						RelatedProducts = relatedIds,
					});
				}

				bool retryAttempt = true;
				while (retryAttempt && relatedProductsData.Count() > 0)
				{
					bool attemptedToRemoveFailingEntry = false;
					retryAttempt = false;
					await productDataProvider.UpdateAllRelations(relatedProductsData, async (callback) =>
					{
						IMappedEntity item;
						if (callback.IsSuccess)
						{
							UpdateRelatedItemStatus(obj, relatedMappedProducts, callback);
						}
						else
						{
							if (!attemptedToRemoveFailingEntry && string.Equals(callback.Error.ResponceStatusCode, "422"))
							{
								attemptedToRemoveFailingEntry = true;
								string[] messages = callback.Error.Message.Split('\n');
								int failedID;
								string clean = Regex.Replace(messages.First(), "[^0-9]", "");
								if (messages.First().ToLower().Contains("not found") && int.TryParse(clean, out failedID))
								{
									RelatedProductsData failedItem = relatedProductsData.Find(i => i.Id == failedID);
									relatedProductsData.Remove(failedItem);
									Log(failedID, SyncDirection.Export, callback.Error);
									retryAttempt = true;
									return;
								}
							}
							if (attemptedToRemoveFailingEntry && !retryAttempt)
								await productDataProvider.UpdateAllRelations(new List<RelatedProductsData>() { relatedProductsData[callback.Index] }, (retrycallback) =>
								{
									item = relatedMappedProducts[callback.Index];
									if (retrycallback.IsSuccess)
									{
										UpdateRelatedItemStatus(obj, relatedMappedProducts, callback);
									}
									else
									{
										Log(item.SyncID, SyncDirection.Export, callback.Error);
									}
									return Task.CompletedTask;

								});
						}
						return;
					});
				}
			}
		}

		public virtual void UpdateRelatedItemStatus(IMappedEntity obj, List<IMappedEntity> relatedMappedProducts, ItemProcessCallback<RelatedProductsData> callback)
		{
			IMappedEntity item = relatedMappedProducts.FirstOrDefault(i => i.ExternID?.ToInt() == callback.Result.Id);
			item.ExternTimeStamp = callback.Result.DateModified;
			var existing = new List<DetailInfo>(item.Details);
			var detailToUpdate = existing?.FirstOrDefault(x => x.LocalID == obj.LocalID);
			detailToUpdate.ExternID = obj.ExternID;
			item.ClearDetails();
			foreach (var detail in existing)
			{
				if (!item.Details.Any(x => x.EntityType == detail.EntityType && x.LocalID == detail.LocalID))
					item.AddDetail(detail.EntityType, detail.LocalID, detail.ExternID);
			}
			UpdateStatus(item, BCSyncOperationAttribute.ExternUpdate);
		}

		protected virtual string GetCrossReferenceValueWithFallBack(ProductItem entity, StringValue salesUnit, StringValue baseUnit, string alternateType)
		{
			return GetCrossReferenceValue(entity, salesUnit, alternateType) ??
				   GetCrossReferenceValue(entity, baseUnit, alternateType) ??
				   GetCrossReferenceValue(entity, null, alternateType) ?? string.Empty;
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
			TLocalType impl = cbapi.GetByID(syncstatus.LocalID, item, GetCustomFieldsForExport());
			if (impl == null) return EntityStatus.None;

			TPrimaryMapped obj = bucket.Product = bucket.Product.Set(impl, impl.SyncID, impl.SyncTime);
			EntityStatus status = EnsureStatus(obj, SyncDirection.Export);

			if (ShouldExportSalesCategories)
			{
				var categories = impl.Categories;
				if (categories != null)
				{
					foreach (CategoryStockItem category in categories)
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
							throw new PXException(BCMessages.CategoryIsDeletedForItem, category.CategoryID.Value, impl.Description);
						if (mappedCategoryStatus == EntityStatus.Pending)
							bucket.Categories.Add(mappedCategory);
					}
				}
			}

			if (!string.IsNullOrWhiteSpace(syncstatus.ExternID))
				bucket.ExternalProduct = await this.productDataProvider.GetByID(syncstatus.ExternID);

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
			return string.IsNullOrWhiteSpace(external?.Sku)
				? local?.InventoryID?.Value
				: external.Sku;
		}
		#endregion

		#region Import
		/// <inheritdoc/>
		public override async Task<bool> CheckExistingForImport(TEntityBucket bucket, BCSyncStatus status, CancellationToken cancellationToken = default)
		{
			var result = await base.CheckExistingForImport(bucket, status, cancellationToken);

			var existing = bucket.Product.Local as ProductItem;
			var external = bucket.Product.Extern as ProductData;

			if (existing is null && this.ProductImportService.IsDuplicateInventoryCode(external.Sku))
				throw new PXException(BigCommerceMessages.PIDuplicateInventoryCode, external.Name, external.Sku, this.GetBindingExt<BCBinding>().BindingName);

			return result;
		}

		/// <summary>
		/// Creates buckets for Sales Categories import.
		/// </summary>
		/// <param name="bucket"></param>		
		public virtual async Task AddSalesCategoryBucketForImport(TEntityBucket bucket)
		{
			var external = bucket.Primary.Extern as ProductData;

			if (!ShouldImportSalesCategories || external?.Categories == null || external?.Categories.Count == 0)
				return;

			var categoriesToFecth = String.Join(",", external.Categories.Select(x => x.ToString()));
			var filter = new FilterProductCategories { IdIn = categoriesToFecth };

			var externalCategories = new List<ProductCategoryData>();

			externalCategories = categoryDataProvider.GetAll(filter).ToListAsync().Result;

			foreach (var category in external.Categories)
			{
				var externalCategory = externalCategories.FirstOrDefault(x => x.Id == category);
				if (externalCategory == null)
					continue;
				MappedCategory mappedCategory = new MappedCategory(externalCategory, externalCategory.Id?.ToString(), externalCategory.Name, externalCategory.CalculateHash());
				EnsureStatus(mappedCategory, SyncDirection.Import);
				bucket.Categories.Add(mappedCategory);
			}
		}

		/// <summary>
		/// Map the categories attached to the imported product with the ERP product
		/// </summary>
		/// <param name="bucket"></param>
		/// <param name="external"></param>
		/// <param name="local"></param>
		public virtual void MapSalesCategories(TEntityBucket bucket, ProductItem existing, ProductData external, ProductItem local)			
		{	
			local.Categories = new List<CategoryStockItem>();

			if (external.Categories == null || bucket.Categories == null)
				return;	

			if (external.Categories != null)
			{
				var syncedLocalIds = bucket.Categories.Select(x=>x.LocalID).ToList();
				var localCategories = SelectFrom<INCategory>.Where<INCategory.noteID.IsIn<P.AsGuid>>.View.Select(this, syncedLocalIds).ToList();

				foreach (var id in external.Categories)
				{
					var synced = bucket.Categories.Where(x => x.ExternID == id.ToString()).FirstOrDefault();
					if (synced != null && synced.LocalID.HasValue)
					{
						var cat = localCategories.Where(x => x.GetItem<INCategory>().NoteID == synced.LocalID).Select(x => x.GetItem<INCategory>()).FirstOrDefault();
						var alreadyExist = existing != null && existing.Categories != null && existing.Categories.Any(x => x.CategoryID.Value == cat.CategoryID.Value);
						if (cat != null)// && !alreadyExist)
							local.Categories.Add(new CategoryStockItem() { CategoryID = cat.CategoryID.ValueField() });
					}
				}
			}

			//Add those to delete
			if (existing != null && existing.Categories != null)
			{
				foreach (var localCategory in existing.Categories)
				{
					if (local.Categories.Any(x => x.CategoryID == localCategory.CategoryID))
						continue;
					local.Categories.Add(new CategoryStockItem() { CategoryID = localCategory.CategoryID, Id = localCategory.Id, Delete = true });
				}
			}
		}

		/// <summary>
		/// Maps all the fields common between StockItems and Non Stock Items
		/// </summary>		
		/// <param name="external">The product from Shopify</param>
		/// <param name="existing">The existing ERP product if it has already been synced</param>
		/// <param name="defaultItemClassId">The default class id (from the settings)</param>		
		public virtual T MapProductImport<T>(ProductData external, ProductItem existing, int? defaultItemClassId) where T : ProductItem, new()
		{
			var local = new T();

			//Validate that we can merge with the existing product item.
			//If we can't merge and an existing product exists with the same SKU, the method throws an exception.
			if (existing != null)
				ProductImportService.ValidateMergingWithExistingItem(existing,  external.Id.ToString(), Operation.EntityType);

			local.ExternalSku = external.Sku?.Trim().ValueField();
			local.Description = external.Name.Trim().ValueField();
			local.Content = external.Description.ValueField();

			local.InventoryID = existing?.InventoryID?.Value!=null ?
				existing.InventoryID : ProductImportService.GetNewProductInventoryID(external.Sku, external.Name).ValueField();

			local.DimensionWeight = external.Weight.ValueField();

			if (external.IsVisible == true)
			{
				local.Visibility = external.IsFeatured == true ? BCCaptions.Featured.ValueField() : BCCaptions.Visible.ValueField();
			}
			else
			{
				local.Visibility = BCCaptions.Invisible.ValueField();
			}

			if (existing == null)
			{
				MapItemClassAndTaxCategory(external, local);
				local.DefaultPrice = external.Price.ValueField();
				local.MSRP = external.RetailPrice.ValueField();
			}

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
			string itemClassId = ProductImportService.Settings.GetDefaultItemClassID();

			if (string.IsNullOrWhiteSpace(itemClassId))
				return;

			local.ItemClass = itemClassId.ValueField();
			INItemClass itemClass = this.ProductImportService.SearchForItemClassByID(itemClassId);
			this.SetTaxCaterogyForProductItem(local, external, itemClass);
			this.ProductImportService.ValidateItemClass(itemClass, itemClassId, external.Name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="external"></param>
		/// <param name="local"></param>
		[Obsolete(PX.Objects.Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		public virtual void MapItemClass<T>(ProductData external, T local) where T : ProductItem, new()
		{
			var id = ProductImportService.Settings.GetDefaultItemClassID();
			local.ItemClass = id?.ValueField();
			if (!string.IsNullOrEmpty(id))
			{
				var itemClass = ProductImportService.SearchForItemClassByID(id);
				ProductImportService.ValidateItemClass(itemClass, id, external.Name);
				SetTaxCaterogyForProductItem(local, external, itemClass);
			}
		}

		/// <summary>
		/// Set the properties that are specific to non stock items
		/// </summary>
		/// <param name="productItem"></param>
		/// <param name="itemClass"></param>
		public virtual void SetClassPropertiesForNonStockItems(ProductItem productItem, INItemClass itemClass)
		{
			if (!(productItem is NonStockItem) || itemClass == null)
				return;
			var nonStockItem = productItem as NonStockItem;
			nonStockItem.SalesUnit = itemClass?.SalesUnit?.ValueField();
			nonStockItem.PurchaseUnit = itemClass?.PurchaseUnit?.ValueField();
			nonStockItem.BaseUnit = itemClass?.BaseUnit?.ValueField();
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
			if (taxCategoryHasCustomMapping)
				return;

			var taxDescription = external.TaxClassId?.ToString();
			if (external.TaxClassId.HasValue)
				taxDescription = taxClasses.Where(x => x.Id == external.TaxClassId.Value).FirstOrDefault()?.Name;

			string taxCategory = ProductImportService.GetTaxCategoryFromSubstitutionList(taxDescription);

			if (string.IsNullOrEmpty(taxCategory) && itemClass!=null)
				taxCategory = ProductImportService.GetTaxCategoryFromItemClass(itemClass.ItemClassCD);

			if (string.IsNullOrWhiteSpace(taxCategory))
				throw new PXException(BCMessages.PIINoTaxCategoryFound, external.Description);

			productItem.TaxCategory = taxCategory.ValueField();
		}

		public virtual string GetProductItemAvailabilityForNonStockItems(ProductData external)
		{
			switch (external.Availability)
			{
				case BigCommerceConstants.AvailabilityAvailable:
					return BCItemAvailabilities.AvailableSkip;					
				case BigCommerceConstants.AvailabilityPreOrder:
					return BCItemAvailabilities.PreOrder;
				default:
					return BCItemAvailabilities.Disabled;
			}
		}

		public virtual string GetProductItemAvailabilityForStockItems(ProductData external)
		{
			switch (external.Availability)
			{
				case BigCommerceConstants.AvailabilityAvailable:
					if (external.InventoryTracking == BigCommerceConstants.InventoryTrackingProduct ||
						external.InventoryTracking == BigCommerceConstants.InventoryTrackingVariant)
						return BCItemAvailabilities.AvailableTrack;

					if (external.InventoryTracking == BigCommerceConstants.InventoryTrackingNone)
						return BCItemAvailabilities.AvailableSkip;

					break;
				case BigCommerceConstants.AvailabilityPreOrder:
					return BCItemAvailabilities.PreOrder;
				default:
					return BCItemAvailabilities.Disabled;
			}
			return BCItemAvailabilities.Disabled;
		}

		[Obsolete(PX.Objects.Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		public virtual string GetProductItemStatus(ProductData external)
		{
			var visible = external.IsVisible ?? false;
			if (visible &&				
				(external.Availability == BigCommerceConstants.AvailabilityAvailable ||
				 external.Availability == BigCommerceConstants.AvailabilityPreOrder))
			{
				return PX.Objects.IN.Messages.Active;
			}
			return PX.Objects.IN.Messages.Inactive;
		}
		#endregion
	}
}
