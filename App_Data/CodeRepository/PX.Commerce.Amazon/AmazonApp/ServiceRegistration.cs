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

using Autofac;
using Autofac.Builder;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.API.Rest.Client.Factory;
using PX.Commerce.Amazon.API.Rest.Client.Interface;
using PX.Commerce.Amazon.API.Rest.Converters;
using PX.Commerce.Amazon.API.Rest.Interfaces;
using PX.Commerce.Amazon.Sync.Factories;
using PX.Commerce.Amazon.Sync.Helpers;
using PX.Commerce.Amazon.Sync.Interfaces;

namespace PX.Commerce.Amazon.AmazonApp
{
	internal class AmazonAppRegistration : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder
				.RegisterType<ShipmentEventDataProviderFactory>()
				.As<IShipmentEventDataProviderFactory>()
				.PreserveExistingDefaults();

			builder
				.RegisterType<OrderFinanceEventsDataProviderFactory>()
				.As<IOrderFinanceEventsDataProviderFactory>()
				.PreserveExistingDefaults();

			builder
				.RegisterType<NonOrderFinanceEventsDataProviderFactory>()
				.As<INonOrderFinanceEventsDataProviderFactory>()
			.PreserveExistingDefaults();

			builder
				.RegisterType<FeeComponentToNonOrderFeeConverter>()
				.As<IConverter<FeeComponent, NonOrderFee>>()
			.PreserveExistingDefaults();

			builder
				.RegisterType<ChargeComponentToNonOrderFeeConverter>()
				.As<IConverter<ChargeComponent, NonOrderFee>>()
				.PreserveExistingDefaults();

			builder
				.RegisterType<NonOrderFinancialEventsToNonOrderFeeConverterFactory>()
				.As<INonOrderFinancialEventsToNonOrderFeeConverterFactory>()
				.PreserveExistingDefaults();

			builder
				.RegisterType<PaymentMethodFeeTypeHandlerFactory>()
				.As<IPaymentMethodFeeTypeHandlerFactory>()
				.PreserveExistingDefaults();

			builder
				.RegisterType<NonOrderFeeGroupHandlerFactory>()
				.As<INonOrderFeeGroupHandlerFactory>()
				.PreserveExistingDefaults();

			builder
				.RegisterType<StatementPeriodParser>()
				.As<IStatementPeriodParser>()
				.PreserveExistingDefaults();

			builder.RegisterType<JsonFeedDataProvider>()
				.As<IJsonFeedDataProvider>()
				.PreserveExistingDefaults();

			builder
				.RegisterType<XmlFeedDataProvider>()
				.As<IXmlFeedDataProvider>()
				.PreserveExistingDefaults();

			builder
				.RegisterType<ReportReader>()
				.As<IAmazonReportReader>()
				.PreserveExistingDefaults();
		}
	}
}
