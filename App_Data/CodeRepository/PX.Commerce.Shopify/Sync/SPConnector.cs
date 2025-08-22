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

using CommonServiceLocator;
using Newtonsoft.Json;
using PX.Commerce.Shopify.API.REST;
using PX.Commerce.Core;
using PX.Commerce.Objects;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using PX.Concurrency;

namespace PX.Commerce.Shopify
{
	#region SPConnectorFactory

	public class SPConnectorFactory : BaseConnectorFactory<SPConnector>, IConnectorFactory
	{
		/// <inheritdoc cref="IConnectorFactory.Type"/>
		public override string Type => SPConnector.TYPE;
		/// <inheritdoc cref="IConnectorFactory.Description"/>
		public override string Description => SPConnector.NAME;
		/// <inheritdoc cref="IConnectorFactory.Enabled"/>
		public override bool Enabled => CommerceFeaturesHelper.ShopifyConnector;


		public SPConnectorFactory(ProcessorFactory factory)
			: base(factory)
		{
		}

		/// <inheritdoc cref="IConnectorFactory.GenerateExternID(BCExternNotification)"/>
		public virtual Guid? GenerateExternID(BCExternNotification message)
		{
			string scope = message.AdditionalInfo["X-Shopify-Topic"]?.ToString();
			string validation = message.AdditionalInfo["X-Shopify-Hmac-Sha256"]?.ToString();
			string apiVersion = message.AdditionalInfo["X-Shopify-API-Version"]?.ToString();
			string storehash = message.AdditionalInfo["X-Shopify-Shop-Domain"]?.ToString();
			string bindingId = message.AdditionalInfo["bindingId"]?.ToString();

			EntityInfo info = _processors.Values.FirstOrDefault(e => e.ExternRealtime.Supported && e.ExternRealtime.WebHookType != null && e.ExternRealtime.WebHooks.Contains(scope));
			WebHookMessage messageData = JsonConvert.DeserializeObject<WebHookMessage>(message.Json);
			String id = messageData.Id?.ToString();

			if (messageData == null || string.IsNullOrEmpty(id)) return null;

			Byte[] bytes = new Byte[16];
			BitConverter.GetBytes(SPConnector.TYPE.GetHashCode()).CopyTo(bytes, 0); //Connector
			BitConverter.GetBytes(info.EntityType.GetHashCode()).CopyTo(bytes, 4); //EntityType
			BitConverter.GetBytes(bindingId.GetHashCode()).CopyTo(bytes, 8); //Store
			BitConverter.GetBytes(id.GetHashCode()).CopyTo(bytes, 12); //ID

			return new Guid(bytes);
		}
	}
	#endregion

	/// <inheritdoc cref="IConnector"/>
	public class SPConnector : BCConnectorBase<SPConnector>, IConnector
	{
		#region IConnector
		public const string TYPE = ShopifyConstants.ShopifyConnector;
		public const string NAME = ShopifyConstants.ShopifyName;

		/// <inheritdoc cref="IConnector.ConnectorType"/>
		public override string ConnectorType { get => TYPE; }
		/// <inheritdoc cref="IConnector.ConnectorName"/>
		public override string ConnectorName { get => NAME; }
		ILongOperationManager IConnector.LongOperationManager => this.LongOperationManager;

		public class spConnectorType : PX.Data.BQL.BqlString.Constant<spConnectorType>
		{
			public spConnectorType() : base(TYPE) { }
		}

		[InjectDependency]
		public IShopifyRestClientFactory shopifyRestClientFactory { get; set; }
		public override async Task Initialise(List<EntityInfo> entities)
		{
			await base.Initialise(entities);
		}


		public override async Task<IEnumerable<TInfo>> GetExternalInfo<TInfo>(string infoType, int? bindingID, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(infoType) || bindingID == null) return null;
			BCBindingShopify shopifyBinding = BCBindingShopify.PK.Find(this, bindingID);
			BCBindingExt store = BCBindingExt.PK.Find(this, bindingID);
			BCBinding binding = BCBinding.PK.Find(this, ConnectorType, bindingID);
			if (binding == null || shopifyBinding == null || store == null) return null;

			try
			{
				List<TInfo> result = new List<TInfo>();
				if (infoType == BCObjectsConstants.BCInventoryLocation)
				{
					InventoryLocationRestDataProvider provider = new InventoryLocationRestDataProvider(shopifyRestClientFactory.GetRestClient(shopifyBinding));
					// to force the code to run asynchronously and keep UI responsive.
					//In some case it runs synchronously especially when using IAsyncEnumerable
					await Task.Yield();
					await foreach (var location in provider.GetAll(null, cancellationToken))
						result.Add(location as TInfo);
				}
				return result;
			}
			catch (Exception ex)
			{
				LogError(new BCLogTypeScope(typeof(SPConnector)), ex);
			}

			return null;
		}
		public override async Task<List<Tuple<String, String, String>>> GetExternalFields(Int32? binding, String entity, CancellationToken cancellationToken = default)
		{
			List<Tuple<String, String, String>> fieldsList = new List<Tuple<string, string, string>>();
			if (entity != BCEntitiesAttribute.ProductAvailability || entity != BCEntitiesAttribute.Shipment) return fieldsList;

			BCBindingShopify bindingShopify = PXSelectReadonly<BCBindingShopify,
				Where<BCBinding.bindingID, Equal<Required<BCBindingShopify.bindingID>>>>.Select(this, binding);

			if (bindingShopify != null)
			{
				InventoryLocationRestDataProvider provider = new InventoryLocationRestDataProvider(shopifyRestClientFactory.GetRestClient(bindingShopify));
				List<InventoryLocationData> fields = new List<InventoryLocationData>();
				// to force the code to run asynchronously and keep UI responsive.
				//In some case it runs synchronously especially when using IAsyncEnumerable
				await Task.Yield();
				await foreach (var field in provider.GetAll(null, cancellationToken))
					fields.Add(field);


				if (fields == null || fields.Count() <= 0) return fieldsList;
				foreach (var item in fields.Where(x => x.Active == true))
				{
					fieldsList.Add(new Tuple<string, string, string>(entity, item.Id?.ToString(), item.Name));
				}
			}
			return fieldsList;
		}

		public virtual async Task<IEnumerable<TInfo>> GetDefaultShippingMethods<TInfo>(int? bindingID)
			where TInfo : class
		{
			BCBindingShopify shopifyBinding = BCBindingShopify.PK.Find(this, bindingID);
			BCBindingExt store = BCBindingExt.PK.Find(this, bindingID);
			BCBinding binding = BCBinding.PK.Find(this, ConnectorType, bindingID);
			if (binding == null || shopifyBinding == null || store == null) return null;
			try
			{
				List<TInfo> result = new List<TInfo>();
				var shippingZones = new List<ShippingZoneData>();

				// retrieving default shipping methods from the database
				List<BCDefaultShippingMapping> defaultShippingMethods = PXSelect<BCDefaultShippingMapping,
					Where<BCDefaultShippingMapping.connectorType, Equal<Required<BCDefaultShippingMapping.connectorType>>>>
					.Select(this, ConnectorType).Select(x => x.GetItem<BCDefaultShippingMapping>()).ToList();

				foreach (BCDefaultShippingMapping method in defaultShippingMethods)
				{
					var currentShippingZone = shippingZones.FirstOrDefault(x => x.Name == method.ShippingZone);
					if (currentShippingZone == null)
					{
						currentShippingZone = new ShippingZoneData { Enabled = true, Name = method.ShippingZone, ShippingMethods = new List<IShippingMethod>() };
						shippingZones.Add(currentShippingZone);
					}

					var shippingMethod = new ShippingMethod() { Name = method.ShippingMethod, Type = ConnectorName, Enabled = true };
					currentShippingZone.ShippingMethods.Add(shippingMethod);
				}

				result = shippingZones.Cast<TInfo>().ToList();

				// to force the code to run asynchronously and keep UI responsive.
				//In some case it runs synchronously especially when using IAsyncEnumerable
				await Task.Yield();
				// retrieving existing shipping methods from the Shopify store
				var dataList = await (new StoreRestDataProvider(shopifyRestClientFactory.GetRestClient(shopifyBinding))).GetShippingZones();
				foreach (var shippingZone in dataList ?? new List<ShippingZoneData>())
				{
					if (shippingZone != null)
					{
						shippingZone.Enabled = true;
						var methods = new List<IShippingMethod>();

						if (shippingZone.PriceBasedShippingRates != null && shippingZone.PriceBasedShippingRates.Count > 0)
						{
							shippingZone.PriceBasedShippingRates.ForEach(p =>
							{
								if (!methods.Any(x => string.Equals(x.Name, p.Name, StringComparison.OrdinalIgnoreCase)))
								{
									methods.Add(new ShippingMethod()
									{
										Id = p.ShippingZoneId,
										Name = p.Name,
										Type = "Shopify",
										Enabled = true
									});
								}
							});
						}

						if (shippingZone.WeightBasedShippingRates != null && shippingZone.WeightBasedShippingRates.Count > 0)
						{
							shippingZone.WeightBasedShippingRates.ForEach(p =>
							{
								if (!methods.Any(x => string.Equals(x.Name, p.Name, StringComparison.OrdinalIgnoreCase)))
								{
									methods.Add(new ShippingMethod()
									{
										Id = p.ShippingZoneId,
										Name = p.Name,
										Type = "Shopify",
										Enabled = true
									});
								}
							});
						}

						if (shippingZone.CarrierShippingRates != null && shippingZone.CarrierShippingRates.Count > 0)
						{
							shippingZone.CarrierShippingRates.ForEach(p =>
							{
								var carrierService = p.GetService();
								if (carrierService != null && carrierService.Count > 0)
								{
									foreach (var service in carrierService)
									{
										if (!methods.Any(x => string.Equals(x.Name, service, StringComparison.OrdinalIgnoreCase)))
										{
											methods.Add(new ShippingMethod()
											{
												Id = p.ShippingZoneId,
												Name = service,
												Type = "Carrier",
												Enabled = true
											});
										}
									}
								}
							});
						}

						shippingZone.ShippingMethods = methods;
					}
					result.Add(shippingZone as TInfo);
				}

				return result;
			}
			catch (Exception ex)
			{
				LogError(new BCLogTypeScope(typeof(SPConnector)), ex);
			}

			return null;
		}
		public virtual IEnumerable<TInfo> GetDefaultPaymentMethods<TInfo>(int? bindingID)
			where TInfo : class
		{
			BCBindingExt store = BCBindingExt.PK.Find(this, bindingID);
			BCBinding binding = BCBinding.PK.Find(this, ConnectorType, bindingID);
			if (binding == null || store == null) return null;
			try
			{
				List<TInfo> result = new List<TInfo>();
				var defaultCurrency = store.DefaultStoreCurrency ?? (binding.BranchID != null ? PX.Objects.GL.Branch.PK.Find(this, binding.BranchID)?.BaseCuryID : null);
				if (defaultCurrency != null)
				{
					List<BCDefaultPaymentMapping> defaultPaymentMethods = PXSelect<BCDefaultPaymentMapping,
						Where<BCDefaultPaymentMapping.connectorType, Equal<Required<BCDefaultPaymentMapping.connectorType>>>>
						.Select(this, ConnectorType).Select(x => x.GetItem<BCDefaultPaymentMapping>()).ToList();

					foreach (BCDefaultPaymentMapping method in defaultPaymentMethods)
					{
						object paymentItem = new PaymentMethod(method.StorePaymentMethod, defaultCurrency, method.CreatePaymentfromOrder ?? false);
						result.Add((TInfo)paymentItem);
					}
				}
				return result;
			}
			catch (Exception ex)
			{
				LogError(new BCLogTypeScope(typeof(SPConnector)), ex);
			}

			return null;
		}
		#endregion

		#region Navigation
		public virtual void NavigateExtern(ISyncStatus status, ISyncDetail detail = null)
		{
			if (status?.ExternID == null) return;

			EntityInfo info = GetEntities().FirstOrDefault(e => e.EntityType == status.EntityType);
			BCBindingShopify binding = BCBindingShopify.PK.Find(this, status.BindingID);
			if (binding == null || string.IsNullOrEmpty(binding.ShopifyApiBaseUrl) || string.IsNullOrEmpty(info.URL)) return;


			string url = null;
			if (status.ExternID.Contains("gid://"))
			{
				var idNumber = status.ExternID.ConvertGidToId();
				url = string.Format(info.URL, idNumber);
			}
			else
			{
				string[] parts = status.ExternID.Split(new char[] { ';' });
				url = string.Format(info.URL, parts.Length > 2 ? parts.Take(2).ToArray() : parts);
			}

			string redirectUrl = binding.ShopifyApiBaseUrl.TrimEnd('/') + "/" + url;

			throw new PXRedirectToUrlException(redirectUrl, PXBaseRedirectException.WindowMode.New, string.Empty);

		}
		#endregion

		#region Process
		public virtual async Task<ConnectorOperationResult> Process(ConnectorOperation operation, Int32?[] syncIDs = null, CancellationToken cancellationToken = default)
		{
			LogInfo(operation.LogScope(), BCMessages.LogConnectorStarted, PXMessages.LocalizeNoPrefix(NAME));

			EntityInfo info = GetEntities().FirstOrDefault(e => e.EntityType == operation.EntityType);
			using (IProcessor graph = (IProcessor)PXGraph.CreateInstance(info.ProcessorType))
			{
				await graph.Initialise(this, operation, cancellationToken);
				return await graph.Process(syncIDs, cancellationToken);
			}
		}

		public async Task<DateTime> GetSyncTime(ConnectorOperation operation)
		{
			BCBindingShopify binding = BCBindingShopify.PK.Find(this, operation.Binding);

			//Shopify Server Response Time
			StoreData store = await new StoreRestDataProvider(shopifyRestClientFactory.GetRestClient(binding)).Get();
			DateTime syncTime = store.ResponseTime;
			syncTime = syncTime.ToUniversalTime();

			//Acumatica Time
			PXDatabase.SelectDate(out DateTime dtLocal, out DateTime dtUtc);
			if (syncTime > dtUtc)
				syncTime = PX.Common.PXTimeZoneInfo.ConvertTimeFromUtc(dtUtc, PX.Common.LocaleInfo.GetTimeZone());
			else
				syncTime = PX.Common.PXTimeZoneInfo.ConvertTimeFromUtc(syncTime, PX.Common.LocaleInfo.GetTimeZone());

			return syncTime;
		}
		#endregion

		#region Notifications
		public override async Task StartWebHook(String baseUrl, BCWebHook hook, CancellationToken cancellationToken = default)
		{
			BCBinding store = BCBinding.PK.Find(this, hook.ConnectorType, hook.BindingID);
			BCBindingShopify storeShopify = BCBindingShopify.PK.Find(this, hook.BindingID);

			WebHookRestDataProvider restClient = new WebHookRestDataProvider(shopifyRestClientFactory.GetRestClient(storeShopify));

			//URL and HASH
			string url = new Uri(baseUrl, UriKind.RelativeOrAbsolute).ToString();
			if (url.EndsWith("/")) url = url.TrimEnd('/');
			url += hook.Destination;
			if (url.EndsWith("/")) url = url.TrimEnd('/');
			url += $"?type={hook.ConnectorType}&company={PXAccess.GetCompanyName() ?? ""}&binding={store.BindingID}";
			url = Uri.EscapeUriString(System.Web.HttpUtility.UrlDecode(url));
			//Searching for the existing hook
			if (hook.HookRef != null)
			{
				WebHookData data = await restClient.GetByID(hook.HookRef.Value.ToString());
				if (data != null)
				{
					if (data.Address != url || data.Topic != hook.Scope)
						await restClient.Delete(hook.HookRef.Value.ToString());
					else if (hook.IsActive == false)
					{
						hook.IsActive = true;
						hook.ValidationHash = storeShopify.StoreSharedSecret;
						Hooks.Update(hook);
						Actions.PressSave();
						return;
					}
					else
						return;
				}
			}
			else
			{
				// to force the code to run asynchronously and keep UI responsive.
				//In some case it runs synchronously especially when using IAsyncEnumerable
				await Task.Yield();
				await foreach (WebHookData data in restClient.GetAll(cancellationToken: cancellationToken))
				{
					if (data.Topic == hook.Scope && data.Address == url)
					{
						hook.IsActive = true;
						hook.HookRef = data.Id;
						hook.ValidationHash = storeShopify.StoreSharedSecret;
						Hooks.Update(hook);
						Actions.PressSave();
						return;
					}
				}
			}

			//Create a new Hook
			WebHookData webHook = new WebHookData();
			webHook.Topic = hook.Scope;
			webHook.Address = url;
			webHook.Format = "json";
			webHook.Fields = new string[] { "id", "order_id", "created_at", "updated_at" };
			webHook = await restClient.Create(webHook);

			//Saving
			hook.IsActive = true;
			hook.HookRef = webHook.Id;
			hook.ValidationHash = storeShopify.StoreSharedSecret;

			Hooks.Update(hook);
			Actions.PressSave();
		}
		public override async Task StopWebHook(String baseUrl, BCWebHook hook, CancellationToken cancellationToken = default)
		{
			BCBindingShopify storeShopify = BCBindingShopify.PK.Find(this, hook.BindingID);

			WebHookRestDataProvider restClient = new WebHookRestDataProvider(shopifyRestClientFactory.GetRestClient(storeShopify));

			if (hook.HookRef != null)
			{
				WebHookData data = await restClient.GetByID(hook.HookRef.ToString());
				if (data != null)
				{
					await restClient.Delete(hook.HookRef.Value.ToString());
				}
			}
			else if (baseUrl != null)
			{
				string url = new Uri(baseUrl, UriKind.RelativeOrAbsolute).ToString();
				if (url.EndsWith("/")) url = url.TrimEnd('/');
				url += hook.Destination;
				if (url.EndsWith("/")) url = url.TrimEnd('/');
				url += $"?type={hook.ConnectorType}&company={PXAccess.GetCompanyName() ?? ""}&binding={storeShopify.BindingID}";
				url = Uri.EscapeUriString(System.Web.HttpUtility.UrlDecode(url));

				// to force the code to run asynchronously and keep UI responsive.
				//In some case it runs synchronously especially when using IAsyncEnumerable
				await Task.Yield();
				await foreach (WebHookData data in restClient.GetAll(cancellationToken: cancellationToken))
				{
					if (data.Topic == hook.Scope && data.Address == url)
					{
						await restClient.Delete(data.Id.Value.ToString());
					}
				}
			}

			//Saving
			hook.IsActive = false;
			hook.HookRef = null;
			hook.ValidationHash = null;

			Hooks.Update(hook);
			Actions.PressSave();
		}

		public virtual async Task ProcessHook(IEnumerable<BCExternQueueMessage> messages, CancellationToken cancellationToken = default)
		{
			Dictionary<RecordKey, RecordValue<String>> toProcess = new Dictionary<RecordKey, RecordValue<String>>();
			foreach (BCExternQueueMessage message in messages)
			{
				WebHookMessage messageData = JsonConvert.DeserializeObject<WebHookMessage>(message.Json);
				string scope = message.AdditionalInfo["X-Shopify-Topic"]?.ToString();
				string validation = message.AdditionalInfo["X-Shopify-Hmac-Sha256"]?.ToString();
				string apiVersion = message.AdditionalInfo["X-Shopify-API-Version"]?.ToString();
				string storehash = message.AdditionalInfo["X-Shopify-Shop-Domain"]?.ToString();
				string bindingId = message.AdditionalInfo["bindingId"]?.ToString();

				DateTime? created = message.TimeStamp.ToDate();

				foreach (BCWebHook hook in PXSelect<BCWebHook,
					Where<BCWebHook.connectorType, Equal<SPConnector.spConnectorType>,
						And<BCWebHook.bindingID, Equal<Required<BCWebHook.bindingID>>,
						And<BCWebHook.scope, Equal<Required<BCWebHook.scope>>>>>>.Select(this, bindingId, scope))
				{
					HMACSHA256 code = new HMACSHA256(Encoding.UTF8.GetBytes(hook.ValidationHash));
					string hash = Convert.ToBase64String(code.ComputeHash(Encoding.UTF8.GetBytes(message.Json)));
					if (hash != validation)
					{
						LogError(new BCLogTypeScope(typeof(SPConnector)), new PXException(BCMessages.WrongValidationHash, storehash ?? "", scope));
						continue;
					}

					foreach (EntityInfo info in this.GetEntities().Where(e => e.ExternRealtime.Supported && e.ExternRealtime.WebHookType != null && e.ExternRealtime.WebHooks.Contains(scope)))
					{
						BCBinding binding = BCBinding.PK.Find(this, TYPE, hook.BindingID.Value);
						BCEntity entity = BCEntity.PK.Find(this, TYPE, hook.BindingID.Value, info.EntityType);

						if (binding == null || !(binding.IsActive ?? false) || entity == null || !(entity.IsActive ?? false)
							|| entity?.ImportRealTimeStatus != BCRealtimeStatusAttribute.Run || entity.Direction == BCSyncDirectionAttribute.Export)
							continue;

						if (messageData == null || messageData.Id == null) continue;

						toProcess[new RecordKey(entity.ConnectorType, entity.BindingID, entity.EntityType, messageData.Id.ToString())]
							= new RecordValue<String>((entity.RealTimeMode == BCSyncModeAttribute.PrepareAndProcess), (DateTime)created, message.Json);
					}
				}
			}

			Dictionary<Int32, ConnectorOperation> toSync = new Dictionary<int, ConnectorOperation>();
			foreach (KeyValuePair<RecordKey, RecordValue<String>> pair in toProcess)
			{
				//Trigger Provider
				ConnectorOperation operation = new ConnectorOperation();
				operation.ConnectorType = pair.Key.ConnectorType;
				operation.Binding = pair.Key.BindingID.Value;
				operation.EntityType = pair.Key.EntityType;
				operation.PrepareMode = PrepareMode.None;
				operation.SyncMethod = SyncMode.Changed;

				Int32? syncID = null;
				EntityInfo info = this.GetEntities().FirstOrDefault(e => e.EntityType == pair.Key.EntityType);

				//Performance optimization - skip push if no value for that
				BCSyncStatus status = null;
				if (pair.Value.Timestamp != null)
				{
					status = BCSyncStatus.ExternIDIndex.Find(this, operation.ConnectorType, operation.Binding, operation.EntityType, pair.Key.RecordID);
					//Let the processor decide if deleted entries should resync - do not filter out deleted statuses
					if (status != null && (status.LastOperation == BCSyncOperationAttribute.Skipped
						|| (status.ExternTS != null && pair.Value.Timestamp <= status.ExternTS)))
						continue;
				}

				if (status == null || status.PendingSync == null || status.PendingSync != true)
				{
					using (IProcessor graph = (IProcessor)PXGraph.CreateInstance(info.ProcessorType))
					{						
						syncID = await graph.ProcessHook(this, operation, pair.Key.RecordID, pair.Value.Timestamp, pair.Value.ExternalInfo, status, cancellationToken);
					}
				}
				else if (status.SyncInProcess == false) syncID = status.SyncID;

				if (syncID != null && pair.Value.AutoSync) toSync[syncID.Value] = operation;
			}
			if (toSync.Count > 0)
			{

				foreach (KeyValuePair<Int32, ConnectorOperation> pair in toSync)
				{
					IConnector connector = ConnectorHelper.GetConnector(pair.Value.ConnectorType);
					try
					{
						await connector.Process(pair.Value, new Int32?[] { pair.Key }, cancellationToken);
					}
					catch (Exception ex)
					{
						connector.LogError(pair.Value.LogScope(pair.Key), ex);
					}
				}
			}

		}

		#endregion
	}
}
