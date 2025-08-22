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
using PX.Commerce.BigCommerce.API.REST;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Data;
using PX.Api.ContractBased.Models;
using System.Threading.Tasks;
using System.Threading;

namespace PX.Commerce.BigCommerce
{
	public class BCCategoryEntityBucket : EntityBucketBase, IEntityBucket
	{
		public IMappedEntity Primary => Category;
		public IMappedEntity[] Entities => new IMappedEntity[] { Primary };
		public override IMappedEntity[] PreProcessors { get => new IMappedEntity[] { LocalParent, ExternParent }; }

		public MappedCategory Category;
		public MappedCategory LocalParent;
		public MappedCategory ExternParent;
	}

	[BCProcessor(typeof(BCConnector), BCEntitiesAttribute.SalesCategory, BCCaptions.SalesCategory, 40,
		IsInternal = false,
		Direction = SyncDirection.Bidirect,
		PrimaryDirection = SyncDirection.Export,
		PrimarySystem = PrimarySystem.Local,
		PrimaryGraph = typeof(PX.Objects.IN.INCategoryMaint),
		ExternTypes = new Type[] { typeof(ProductCategoryData) },
		LocalTypes = new Type[] { typeof(BCItemSalesCategory) },
		AcumaticaPrimaryType = typeof(PX.Objects.IN.INCategory),
		AcumaticaPrimarySelect = typeof(PX.Objects.IN.INCategory.categoryID),
		URL = "products/categories/{0}/edit",
		Requires = new string[] { }
	)]
	[BCProcessorRealtime(PushSupported = true, HookSupported = true, Hidden = true,
		PushSources = new String[] { "BC-PUSH-Category" }, PushDestination = BCConstants.PushNotificationDestination,
		WebHookType = typeof(WebHookProductCategory),
		WebHooks = new String[]
		{
			"store/category/created",
			"store/category/updated",
			"store/category/deleted"
		})]
	public class BCCategoryProcessor : BCProcessorSingleBase<BCCategoryProcessor, BCCategoryEntityBucket, MappedCategory>
	{
		[InjectDependency]
		protected IBCRestClientFactory bcRestClientFactory { get; set;}
		[InjectDependency]
		protected IBCRestDataProviderFactory<IParentRestDataProvider<ProductCategoryData>> categoryDataProviderFactory { get; set; }
		protected IParentRestDataProvider<ProductCategoryData> categoryDataProvider;

		#region Constructor
		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation, cancellationToken);

			cbapi.UseNoteID = true;
			categoryDataProvider = categoryDataProviderFactory.CreateInstance(bcRestClientFactory.GetRestClient(GetBindingExt<BCBindingBigCommerce>()));
		}
		#endregion

		#region Common
		public override async Task<MappedCategory> PullEntity(Guid? localID, Dictionary<string, object> fields, CancellationToken cancellationToken = default)
		{
			BCItemSalesCategory impl = cbapi.GetByID<BCItemSalesCategory>(localID);
			if (impl == null) return null;

			MappedCategory obj = new MappedCategory(impl, impl.SyncID, impl.SyncTime);

			return obj;
		}
		public override async Task<MappedCategory> PullEntity(String externID, String jsonObject, CancellationToken cancellationToken = default)
		{
			ProductCategoryData data = await categoryDataProvider.GetByID(externID);
			if (data == null) return null;

			MappedCategory obj = new MappedCategory(data, data.Id.ToString(), data.Name, data.CalculateHash());

			return obj;
		}

		public override async Task<PullSimilarResult<MappedCategory>> PullSimilar(IExternEntity entity, CancellationToken cancellationToken = default)
		{
			string uniqueFieldValue = ((ProductCategoryData)entity)?.Name;
			var parent = ((ProductCategoryData)entity)?.ParentId;
			if (uniqueFieldValue == null) return null;

			PX.Objects.IN.INCategory incategory = PXSelectJoin<PX.Objects.IN.INCategory,
					LeftJoin<BCSyncStatus, On<PX.Objects.IN.INCategory.noteID, Equal<BCSyncStatus.localID>>>,
					Where<BCSyncStatus.connectorType, Equal<Current<BCEntity.connectorType>>,
						And<BCSyncStatus.bindingID, Equal<Current<BCEntity.bindingID>>,
						And<BCSyncStatus.entityType, Equal<Current<BCEntity.entityType>>,
						And<BCSyncStatus.externID, Equal<Required<BCSyncStatus.externID>>>>>>>.Select(this, parent);
			int parentId = incategory?.CategoryID ?? 0;
			BCItemSalesCategory[] impls = cbapi.GetAll<BCItemSalesCategory>(new BCItemSalesCategory() { Description = uniqueFieldValue.SearchField(), ParentCategoryID = parentId.SearchField() },
				filters: GetFilter(Operation.EntityType).LocalFiltersRows.Cast<PXFilterRow>().ToArray(), supportPagination: false, cancellationToken: cancellationToken).ToArray();
			if (impls == null) return null;
			if (impls.Length == 1)
			{
				var impl = impls.First();
				var existedStatus = BCSyncStatus.LocalIDIndex.Find(this, Operation.ConnectorType, Operation.Binding, Operation.EntityType, impl.SyncID);
				if (existedStatus != null && existedStatus.ExternID != null && existedStatus.ExternID != ((ProductCategoryData)entity)?.Id?.ToString())
				{
					//Check the existed ExternID in BC whether has been deleted.
					ProductCategoryData externResult = await categoryDataProvider.GetByID(existedStatus.ExternID);
					if (externResult == null)
					{
						existedStatus.ExternID = null;
						Statuses.Update(existedStatus);
						Statuses.Cache.Persist(existedStatus, PXDBOperation.Update);
					}
				}
			}

			return new PullSimilarResult<MappedCategory>() { UniqueField = uniqueFieldValue, Entities = impls.Select(impl => new MappedCategory(impl, impl.SyncID, impl.SyncTime)) };
		}
		public override async Task<PullSimilarResult<MappedCategory>> PullSimilar(ILocalEntity entity, CancellationToken cancellationToken = default)
		{
			var uniqueFieldValue = ((BCItemSalesCategory)entity)?.Description?.Value;
			var parent = ((BCItemSalesCategory)entity)?.ParentCategoryID?.Value;
			if (uniqueFieldValue == null) return null;
			BCSyncStatus parentStatus = PXSelectJoin<BCSyncStatus,
					LeftJoin<PX.Objects.IN.INCategory, On<PX.Objects.IN.INCategory.noteID, Equal<BCSyncStatus.localID>>>,
					Where<BCSyncStatus.connectorType, Equal<Current<BCEntity.connectorType>>,
						And<BCSyncStatus.bindingID, Equal<Current<BCEntity.bindingID>>,
						And<BCSyncStatus.entityType, Equal<Required<BCEntity.entityType>>,
						And<PX.Objects.IN.INCategory.categoryID, Equal<Required<PX.Objects.IN.INCategory.categoryID>>>>>>>
						.Select(this, BCEntitiesAttribute.SalesCategory, parent);
			int parentId = parentStatus?.ExternID?.ToInt() ?? 0;
			List<ProductCategoryData> datas = new List<ProductCategoryData>();
			// to force the code to run asynchronously and keep UI responsive.
			//In some case it runs synchronously especially when using IAsyncEnumerable
			await Task.Yield();
			await foreach (var data in categoryDataProvider.GetAll(new FilterProductCategories() { Name = uniqueFieldValue, ParentId = parentId }, cancellationToken))
				datas.Add(data);
			if (datas == null) return null;
			if (datas.Count == 1)
			{
				var impl = datas.First();
				var existedStatus = BCSyncStatus.ExternIDIndex.Find(this, Operation.ConnectorType, Operation.Binding, Operation.EntityType, impl.Id.ToString());
				if (existedStatus != null && existedStatus.LocalID != null && existedStatus.LocalID != ((BCItemSalesCategory)entity)?.SyncID)
				{
					//Check the existed LocalID in ERP whether has been deleted.
					BCItemSalesCategory externResult = cbapi.GetByID<BCItemSalesCategory>(existedStatus.LocalID);
					if (externResult == null)
					{
						existedStatus.ExternID = null;
						Statuses.Update(existedStatus);
						Statuses.Cache.Persist(existedStatus, PXDBOperation.Update);
					}
				}
			}

			return new PullSimilarResult<MappedCategory>() { UniqueField = uniqueFieldValue, Entities = datas.Select(data => new MappedCategory(data, data.Id.ToString(), data.Name, data.CalculateHash())) };
		}

		public override void NavigateLocal(IConnector connector, ISyncStatus status, ISyncDetail detail = null)
		{
			PX.Objects.IN.INCategoryMaint extGraph = PXGraph.CreateInstance<PX.Objects.IN.INCategoryMaint>();
			PX.Commerce.Objects.BCINCategoryMaintExt extGraphExt = extGraph.GetExtension<PX.Commerce.Objects.BCINCategoryMaintExt>();
			extGraphExt.SelectedCategory.Current = PXSelect<PX.Commerce.Objects.BCINCategoryMaintExt.SelectedINCategory,
			  Where<PX.Commerce.Objects.BCINCategoryMaintExt.SelectedINCategory.noteID, Equal<Required<PX.Commerce.Objects.BCINCategoryMaintExt.SelectedINCategory.noteID>>>>.Select(extGraph, status.LocalID);

			throw new PXRedirectRequiredException(extGraph, true, "Navigation");
		}
		#endregion

		#region Import
		public override async Task FetchBucketsForImport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			FilterProductCategories filter = null; //No DateTime filtering for Category
			// to force the code to run asynchronously and keep UI responsive.
			//In some case it runs synchronously especially when using IAsyncEnumerable
			await Task.Yield();
			await foreach(ProductCategoryData data in categoryDataProvider.GetAll(filter, cancellationToken))
			{
				if (data == null) continue;

				BCCategoryEntityBucket bucket = CreateBucket();

				MappedCategory obj = bucket.Category = bucket.Category.Set(data, data.Id?.ToString(), data.Name, data.CalculateHash());
				EntityStatus status = EnsureStatus(obj, SyncDirection.Import);
			}
		}
		public override async Task<EntityStatus> GetBucketForImport(BCCategoryEntityBucket bucket, BCSyncStatus bcstatus, CancellationToken cancellationToken = default)
		{
			ProductCategoryData data = await categoryDataProvider.GetByID(bcstatus.ExternID);
			if (data == null) return EntityStatus.None;

			MappedCategory obj = bucket.Category = bucket.Category.Set(data, data.Id?.ToString(), data.Name, data.CalculateHash());
			EntityStatus status = EnsureStatus(obj, SyncDirection.Import);

			Int32? parent = obj.Extern.ParentId;
			if (parent != null && parent > 0)
			{
				ProductCategoryData parentData = await categoryDataProvider.GetByID(parent.ToString());
				if (parentData != null)
				{
					MappedCategory parentObj = bucket.ExternParent = bucket.ExternParent.Set(parentData, parentData.Id?.ToString(), data.Name, data.CalculateHash());

					EntityStatus parentStatus = EnsureStatus(parentObj);
				}
			}

			return status;
		}

		public override async Task MapBucketImport(BCCategoryEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			MappedCategory obj = bucket.Category;

			ProductCategoryData data = obj.Extern;
			BCItemSalesCategory impl = obj.Local = new BCItemSalesCategory();
			impl.Custom = GetCustomFieldsForImport();

			//Category
			impl.Description = data.Name.ValueField();
			impl.SortOrder = data.SortOrder.ValueField();

			if (data.ParentId != null && data.ParentId > 0 && impl.Description?.Value != null)
			{
				PX.Objects.IN.INCategory incategory = PXSelectJoin<PX.Objects.IN.INCategory,
					LeftJoin<BCSyncStatus, On<PX.Objects.IN.INCategory.noteID, Equal<BCSyncStatus.localID>>>,
					Where<BCSyncStatus.connectorType, Equal<Current<BCEntity.connectorType>>,
						And<BCSyncStatus.bindingID, Equal<Current<BCEntity.bindingID>>,
						And<BCSyncStatus.entityType, Equal<Current<BCEntity.entityType>>,
						And<BCSyncStatus.externID, Equal<Required<BCSyncStatus.externID>>>>>>>.Select(this, data.ParentId);
				if (incategory == null) throw new PXException(BCMessages.CategoryNotSyncronizedParent, impl.Description.Value);
				obj.Local.ParentCategoryID = incategory.CategoryID.ValueField();
			}
			else obj.Local.ParentCategoryID = 0.ValueField();
		}
		public override async Task SaveBucketImport(BCCategoryEntityBucket bucket, IMappedEntity existing, String operation, CancellationToken cancellationToken = default)
		{
			//Category
			MappedCategory obj = bucket.Category;

			if (existing?.Local != null) obj.Local.CategoryID = ((BCItemSalesCategory)existing.Local).CategoryID.Value.SearchField();

			// Prevent to save of category with no changes. Workaround for a bug in Acumatica, where it returns the wrong record in case not changes.
			if (existing != null && ((BCItemSalesCategory)existing.Local).Description?.Value == obj.Local.Description?.Value
				&& (((BCItemSalesCategory)existing.Local).ParentCategoryID?.Value ?? 0) == (obj.Local.ParentCategoryID?.Value ?? 0)
				&& (((BCItemSalesCategory)existing.Local).SortOrder?.Value ?? 0) == (obj.Local.SortOrder?.Value ?? 0))
			{
				UpdateStatus(obj, operation);
				return;
			}

			BCItemSalesCategory impl = cbapi.Put<BCItemSalesCategory>(obj.Local, obj.LocalID);

			bucket.Category.AddLocal(impl, impl.SyncID, impl.SyncTime);
			UpdateStatus(obj, operation);
		}
		#endregion

		#region Export
		public override async Task FetchBucketsForExport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			IEnumerable<BCItemSalesCategory> impls = cbapi.GetAll<BCItemSalesCategory>(new BCItemSalesCategory() { Path = new StringReturn() },
				minDateTime, maxDateTime, filters, supportPagination: false, cancellationToken: cancellationToken);
			var invIDs = new List<int>();

			foreach (BCItemSalesCategory impl in impls)
			{
				if (impl.SyncID == null) continue; //We need to skip the root node, which does not have a ID

				MappedCategory obj = new MappedCategory(impl, impl.SyncID, impl.SyncTime);
				EntityStatus status = EnsureStatus(obj, SyncDirection.Export);
				if (status == EntityStatus.Pending) invIDs.Add(impl?.CategoryID?.Value ?? 0);
			}
		}
		public override async Task<EntityStatus> GetBucketForExport(BCCategoryEntityBucket bucket, BCSyncStatus bcstatus, CancellationToken cancellationToken = default)
		{
			BCItemSalesCategory impl = cbapi.GetByID<BCItemSalesCategory>(bcstatus.LocalID, GetCustomFieldsForExport());
			if (impl == null) return EntityStatus.None;

			MappedCategory obj = bucket.Category = bucket.Category.Set(impl, impl.SyncID, impl.SyncTime);
			EntityStatus status = EnsureStatus(obj, SyncDirection.Export);

			Int32? parent = obj.Local.ParentCategoryID?.Value;
			if (parent != null && parent > 0)
			{
				BCItemSalesCategory parentImpl = cbapi.Get<BCItemSalesCategory>(new BCItemSalesCategory() { CategoryID = parent.ValueField() });
				if (parentImpl != null)
				{
					MappedCategory parentObj = bucket.LocalParent = bucket.LocalParent.Set(parentImpl, parentImpl.SyncID, parentImpl.SyncTime);
					EntityStatus parentStatus =	EnsureStatus(parentObj);
				}
			}
			return status;
		}

		public override async Task MapBucketExport(BCCategoryEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			MappedCategory obj = bucket.Category;

			BCItemSalesCategory impl = obj.Local;
			ProductCategoryData data = obj.Extern = new ProductCategoryData();

			//Contact
			data.Name = impl.Description?.Value;
			data.SortOrder = impl.SortOrder?.Value;

			if (impl.ParentCategoryID?.Value != null && impl.ParentCategoryID?.Value > 0 && data.Name != null)
			{
				BCSyncStatus parentStatus = PXSelectJoin<BCSyncStatus,
					LeftJoin<PX.Objects.IN.INCategory, On<PX.Objects.IN.INCategory.noteID, Equal<BCSyncStatus.localID>>>,
					Where<BCSyncStatus.connectorType, Equal<Current<BCEntity.connectorType>>,
						And<BCSyncStatus.bindingID, Equal<Current<BCEntity.bindingID>>,
						And<BCSyncStatus.entityType, Equal<Required<BCEntity.entityType>>,
						And<PX.Objects.IN.INCategory.categoryID, Equal<Required<PX.Objects.IN.INCategory.categoryID>>>>>>>
						.Select(this, BCEntitiesAttribute.SalesCategory, impl.ParentCategoryID?.Value);
				if (parentStatus == null) throw new PXException(BCMessages.CategoryNotSyncronizedParent, data.Name);

				data.ParentId = parentStatus.ExternID.ToInt();
			}
			else data.ParentId = 0;
		}


		public override async Task SaveBucketExport(BCCategoryEntityBucket bucket, IMappedEntity existing, String operation, CancellationToken cancellationToken = default)
		{
			//Category
			MappedCategory obj = bucket.Category;

			ProductCategoryData data = null;
			if (obj.ExternID == null)
				data = await categoryDataProvider.Create(obj.Extern);
			else
				data = await categoryDataProvider.Update(obj.Extern, obj.ExternID);
			obj.AddExtern(data, data.Id?.ToString(), data.Name, data.CalculateHash());
			UpdateStatus(obj, operation);
		}
		#endregion
	}
}
