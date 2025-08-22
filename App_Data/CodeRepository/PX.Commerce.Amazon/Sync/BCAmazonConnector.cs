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
using PX.CloudServices;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.API.Rest.Client.Common;
using PX.Commerce.Core;
using PX.Commerce.Objects;
using PX.Concurrency;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon
{
	#region AmazonConnectorFactory
	public class AmazonConnectorFactory : BaseConnectorFactory<BCAmazonConnector>, IConnectorFactory
	{
		public override string Type => BCAmazonConnector.TYPE;
		public override string Description => BCAmazonConnector.NAME;
		public override bool Enabled => PXAccess.FeatureInstalled("PX.Objects.CS.FeaturesSet+AmazonIntegration");
		public AmazonConnectorFactory(ProcessorFactory processors)
			: base(processors)
		{
		}

		public Guid? GenerateExternID(BCExternNotification message)
		{
			throw new NotImplementedException();
		}
	}
	#endregion

	public class BCAmazonConnector : BCConnectorBase<BCAmazonConnector>, IConnector
	{
		public const string TYPE = "AZC";
		public const string NAME = "Amazon";

		[InjectDependency]
		private IAmazonCloudServiceClient CloudServiceClient { get; set; }

		[InjectDependency]
		private IHttpClientFactory HttpClientFactory { get; set; }

		public override string ConnectorType { get => TYPE; }
		public override string ConnectorName { get => NAME; }

		ILongOperationManager IConnector.LongOperationManager => this.LongOperationManager;

		public class azConnectorType : PX.Data.BQL.BqlString.Constant<azConnectorType>
		{
			public azConnectorType() : base(TYPE) { }
		}

		public override async Task Initialise(List<EntityInfo> entities)
		{
			await base.Initialise(entities);
		}

		public IAmazonRestClient GetRestClient(BCBindingAmazon bindingAmazon, BCBinding binding)
		{
			var serializer = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore,
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				DateTimeZoneHandling = DateTimeZoneHandling.Unspecified,
				Formatting = Formatting.Indented,
				ContractResolver = new Core.REST.GetOnlyContractResolver(),
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore
			};

			var options = new RestOptions(bindingAmazon, binding);
			var logger = ServiceLocator.Current.GetInstance<Serilog.ILogger>();

			return new AmazonRestClient(this.CloudServiceClient, this.HttpClientFactory, serializer, options, logger);
		}

		public IEnumerable<TInfo> GetExternalInfo<TInfo>(string infoType, int? bindingID) where TInfo : class
		{
			List<TInfo> result = new List<TInfo>();
			if (string.IsNullOrEmpty(infoType) || bindingID == null) return null;
			return result;
		}

		public async Task<DateTime> GetSyncTimeAsync(ConnectorOperation operation)
		{
			//used .result as core method is not async
			SellerDataProvider sellerDataProvider = new SellerDataProvider(GetRestClient(BCBindingAmazon.PK.Find(this, operation.Binding), BCBinding.PK.Find(this, ConnectorType, operation.Binding)));
			PXDatabase.SelectDate(out DateTime dtLocal, out DateTime dtUtc);
			if (!DateTime.TryParse(await sellerDataProvider.GetServerTime(), out DateTime syncTime))
				return PX.Common.PXTimeZoneInfo.ConvertTimeFromUtc(dtUtc, PX.Common.LocaleInfo.GetTimeZone());
			else
			{
				syncTime = syncTime.ToUniversalTime();
				if (syncTime > dtUtc)
					syncTime = PX.Common.PXTimeZoneInfo.ConvertTimeFromUtc(dtUtc, PX.Common.LocaleInfo.GetTimeZone());
				else
					syncTime = PX.Common.PXTimeZoneInfo.ConvertTimeFromUtc(syncTime, PX.Common.LocaleInfo.GetTimeZone());

				return syncTime;
			}
		}

		[Obsolete("The method will be removed in the future versions. Instead, use GetSyncTimeAsync.")]
		public DateTime GetSyncTime(ConnectorOperation operation)
		{
			return DateTime.MinValue;
		}

		async Task<DateTime> IConnector.GetSyncTime(ConnectorOperation operation)
		{
			SellerDataProvider sellerDataProvider = new SellerDataProvider(GetRestClient(BCBindingAmazon.PK.Find(this, operation.Binding), BCBinding.PK.Find(this, ConnectorType, operation.Binding)));
			PXDatabase.SelectDate(out DateTime dtLocal, out DateTime dtUtc);
			if (!DateTime.TryParse(await sellerDataProvider.GetServerTime(), out DateTime syncTime))
				return PX.Common.PXTimeZoneInfo.ConvertTimeFromUtc(dtUtc, PX.Common.LocaleInfo.GetTimeZone());
			else
			{
				syncTime = syncTime.ToUniversalTime();
				if (syncTime > dtUtc)
					syncTime = PX.Common.PXTimeZoneInfo.ConvertTimeFromUtc(dtUtc, PX.Common.LocaleInfo.GetTimeZone());
				else
					syncTime = PX.Common.PXTimeZoneInfo.ConvertTimeFromUtc(syncTime, PX.Common.LocaleInfo.GetTimeZone());

				return syncTime;
			}
		}

		public void NavigateExtern(ISyncStatus status, ISyncDetail detail = null)
		{
			if (status?.ExternID == null) return;

			EntityInfo info = GetEntities().FirstOrDefault(e => e.EntityType == status.EntityType);
			BCBindingAmazon binding = BCBindingAmazon.PK.Find(this, status.BindingID);

			if (binding == null || string.IsNullOrEmpty(binding.Marketplace) || string.IsNullOrEmpty(binding.Region) || string.IsNullOrEmpty(info.URL)) return;
			if (AmazonUrlProvider.TryGetRegion(binding.Region, out Region region))
			{
				var marketplaceURL = region.Marketplaces.Where(x => x.MarketplaceValue == binding.Marketplace).FirstOrDefault()?.MarketplaceUrl?.ToString();

				string[] parts = status.ExternID.Split(new char[] { ';' });
				string url = string.Format(info.URL, parts.Length > 2 ? parts.Take(2).ToArray() : parts);
				string redirectUrl = marketplaceURL.TrimEnd('/') + "/" + url;

				throw new PXRedirectToUrlException(redirectUrl, PXBaseRedirectException.WindowMode.New, string.Empty);
			}
		}

		public async Task<ConnectorOperationResult> Process(ConnectorOperation operation, int?[] syncIDs = null, CancellationToken cancellationToken = default)
		{
			// Acuminator disable once PX1051 NonLocalizableString [Used only for logs]
			LogInfo(operation.LogScope(), BCMessages.LogConnectorStarted, PXMessages.LocalizeNoPrefix(NAME));

			EntityInfo info = GetEntities().FirstOrDefault(e => e.EntityType == operation.EntityType);
			using (IProcessor graph = (IProcessor)PXGraph.CreateInstance(info.ProcessorType))
			{
				await graph.Initialise(this, operation);
				return await graph.Process(syncIDs);
			}
		}

		public override async Task StartWebHook(string baseUrl, BCWebHook hook, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public override async Task StopWebHook(string baseUrl, BCWebHook hook, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		Task IConnector.ProcessHook(IEnumerable<BCExternQueueMessage> messages, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public virtual async Task<IEnumerable<TInfo>> GetDefaultShippingMethods<TInfo>(int? bindingID)
			where TInfo : class
		{
			throw new NotImplementedException();
		}
	}
}
