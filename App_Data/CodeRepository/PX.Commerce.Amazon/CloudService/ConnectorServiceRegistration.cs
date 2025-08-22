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
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using PX.CloudServices.Diagnostic;
using PX.Commerce.Core;
using PX.Owin;
using System;
using System.Net.Http;

namespace PX.Commerce.Amazon
{
	internal class ConnectorServiceRegistration : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<AmazonAuthenticationHandler>()
				 .As<IOwinConfigurationPart>().SingleInstance();

			//cloud service
			builder.RegisterType<AmazonCloudServiceClient>()
				.As<IAmazonCloudServiceClient>()
				.As<ICloudServiceClientWithDiagnostics>()
				.SingleInstance();

			Client amazonAppClient = AmazonAppConnectedApplication.AmazonAppClientForRegistration();

			builder
				.RegisterInstance(amazonAppClient)
					.Keyed<Client>(Guid.Parse(AmazonAppConnectedApplication.AmazonAppClientId));

			builder.RegisterType<BCLoginScopeFactory>()
				.As<IBCLoginScopeFactory>();

			builder.Register<IHttpClientFactory>(_ =>
			{
				var services = new ServiceCollection();
				services.AddHttpClient();
				var provider = services.BuildServiceProvider();

				return provider.GetRequiredService<IHttpClientFactory>();
			})
				.PreserveExistingDefaults();
		}
	}
}
