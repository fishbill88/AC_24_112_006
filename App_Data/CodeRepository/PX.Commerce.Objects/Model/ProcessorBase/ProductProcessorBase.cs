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

using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Objects
{
	public abstract class ProductProcessorBase<TGraph, TEntityBucket, TPrimaryMapped, TExternType, TLocalType> : BCProcessorSingleBase<TGraph, TEntityBucket, TPrimaryMapped>
		where TGraph : PXGraph
		where TEntityBucket : class, IEntityBucket, new()
		where TPrimaryMapped : class, IMappedEntity, new()
		where TExternType : BCAPIEntity, IExternEntity, new()
		where TLocalType : ProductItem, ILocalEntity, new()
	{

		#region Common

		/// <summary>
		/// Initialize a new object of the entity to be used to Fetch bucket
		/// </summary>
		/// <returns>The initialized entity</returns>
		protected abstract TLocalType CreateEntityForFetch();

		/// <summary>
		/// Creates a mapped entity for the passed entity
		/// </summary>
		/// <param name="entity">The entity to create the mapped entity from</param>
		/// <param name="syncId">The sync id of the entity</param>
		/// <param name="syncTime">The timestamp of the last modification</param>
		/// <returns>The mapped entity</returns>
		protected abstract TPrimaryMapped CreateMappedEntity(TLocalType entity, Guid? syncId, DateTime? syncTime);

		/// <summary>
		/// Updates the local timestamp value for the specified <paramref name="mappedEntity"/> according to the latest stored data.
		/// </summary>
		/// <param name="mappedEntity">The mapped template item that represents the BCSyncStatus.</param>
		/// <remarks>The parent template item is updated when child records are added. The latest timestamp should be populated in BCSyncStatus.LocalTS field.</remarks>
		protected void UpdateMappedEntityTimestamp(IMappedEntity mappedEntity)
		{
			var storedTemplateItem = cbapi.GetByID<TemplateItems>(mappedEntity.LocalID);
			mappedEntity.LocalTimeStamp = storedTemplateItem.LastModifiedDateTime.Value;
		}

		/// <summary>
		/// Marks the product variants as removed.
		/// </summary>
		/// <typeparam name="T">Type of the product item.</typeparam>
		/// <param name="localVariants">The collection of variants already stored.</param>
		/// <param name="externalVariants">The collection of variants at a store.</param>
		protected void MarkRemovedProductVariants<T>(IEnumerable<ProductItem> localVariants, IEnumerable<ProductItem> externalVariants)
			where T : ProductItem, new()
		{
			if (localVariants is null)
				return;

			foreach (ProductItem localVariant in localVariants)
			{
				if (localVariant.ItemStatus.Value == PX.Objects.IN.Messages.ToDelete)
					continue;

				if (externalVariants.Any(productItemVariant => productItemVariant.InventoryID.Value.Equals(localVariant.InventoryID.Value, StringComparison.OrdinalIgnoreCase)))
					continue;

				T productItemVariant = new()
				{
					ItemStatus = PX.Objects.IN.Messages.ToDelete.ValueField(),
					Id = localVariant.Id
				};
				cbapi.Put<T>(productItemVariant, productItemVariant.Id.Value);
			}
		}
		public override object GetAttribute(ILocalEntity impl, string attributeID)
		{
			if (impl is ProductItem item)
				return item?.Attributes?.Where(x => string.Equals(x?.AttributeDescription?.Value, attributeID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

			return base.GetAttribute(impl, attributeID);
		}

		#endregion

		#region Export
		public override async Task FetchBucketsForExport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			TLocalType item = CreateEntityForFetch();
			IEnumerable<TLocalType> impls = cbapi.GetAll(item, minDateTime, maxDateTime, filters, cancellationToken: cancellationToken);

			if (impls != null)
			{
				int countNum = 0;
				List<IMappedEntity> mappedList = new List<IMappedEntity>();
				foreach (TLocalType impl in impls)
				{
					IMappedEntity obj = CreateMappedEntity(impl, impl.SyncID, impl.SyncTime);

					mappedList.Add(obj);
					countNum++;
					if (countNum % BatchFetchCount == 0)
					{
						ProcessMappedListForExport(mappedList);
					}
				}
				if (mappedList.Any())
				{
					ProcessMappedListForExport(mappedList);
				}
			}
		}


		/// <summary>
		/// This method checks for deleted mediaUrls in the ERP and deletes them on the external system.
		/// The method checks for product's details and the list of previous images attached to it (before this synchronization)
		/// Any image in the details is not part of the list of media Urls to sync, then it is considered as deleted from the ERP
		/// and therefor, should be deleted on the external system as well.
		/// </summary>
		/// <param name="obj">The mapped entity that is currently synchronized</param>
		/// <param name="imagesUrls">The list of media urls (fileType == BCCaptions.Image)</param>
		public virtual async Task SyncDeletedMediaUrls(IMappedEntity obj, List<InventoryFileUrls> imagesUrls)
		{
			var listOfPreviousImages = obj.Details?.Where(i => i.EntityType == BCEntitiesAttribute.ProductImage);
			if (listOfPreviousImages == null || !listOfPreviousImages.Any())
				return;

			foreach (var image in listOfPreviousImages)
			{
				var existingUrl = imagesUrls?.Where(i => i.Id == image.LocalID).FirstOrDefault();
				if (existingUrl != null)
					continue;

				await DeleteImageFromExternalSystem(obj.ExternID, image.ExternID);
			}
		}

		/// <summary>
		/// Calls the API of the external system to actually delete the image.
		/// </summary>
		/// <param name="parentId">The Id of the parent product on the external system.</param>
		/// <param name="imageId">The Id of the image to be deleted</param>
		/// <exception cref="PXException">In case of a REST error (other than 404), it raises an exception and stops the synchronization for the current object</exception>
		protected virtual async Task DeleteImageFromExternalSystem(string parentId, string imageId) { }

		#endregion
	}
}
