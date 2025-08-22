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
using IdentityModel.Client;
using Microsoft.Owin;
using Owin;
using PX.Data;
using PX.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.SessionState;
using PX.Commerce.Core;
using Serilog;

namespace PX.Commerce.Amazon
{
	public class AmazonAuthenticationHandler : IOwinEndpointByPrefix
	{
		IReadOnlyCollection<object> IOwinEndpointByPrefix.Metadata { get; } = new object[]
			{
				new SessionStateBehaviorSelector(_ => SessionStateBehavior.Required)
			};
		public const string Prefix = "OAuthAmazonAuthenticationHandler";
		private const string SPAPICode = "spapi_oauth_code";
		private const string SellerPartnerID = "selling_partner_id";
		private readonly IBCLoginScopeFactory _scope;
		private readonly ILogger _logger;
		string IOwinEndpointByPrefix.Prefix => "/" + Prefix;
		public static string ReturnUrl
		{
			get
			{
				var applicationPath = HttpContext.Current.Request.ApplicationPath?.Trim('/');
				applicationPath = string.IsNullOrEmpty(applicationPath)
					? string.Empty
					: applicationPath + "/";
				return HttpContext.Current.Request.GetWebsiteUrl() + applicationPath
						+ Prefix;
			}
		}

		public AmazonAuthenticationHandler(IBCLoginScopeFactory scope, ILogger logger)
		{
			_scope = scope;
			_logger = logger;
		}

		public void Configure(IAppBuilder app)
		{
			app
				.Use((ctx, next) => next())
				.Run(ProcessAuthorizationCode);
		}

		/// <summary>
		/// Method to exchange authorization code to get Refresh token
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		private async Task ProcessAuthorizationCode(IOwinContext ctx)
		{
			var authorizeResponse = new AuthorizeResponse(ctx.Request.QueryString.Value);
			var code = ctx.Request.Query[SPAPICode];
			var seller = ctx.Request.Query[SellerPartnerID];

			(int BindingID, string Company) state = AmazonStateParameters.GetSplittedState(authorizeResponse.State);
			_logger
				.ForContext("Scope", new BCLogTypeScope(GetType()))
				.Information("{CommerceLogCaption}: AmazonAuthenticationHandler.ProcessAuthorizationCode: {BindingID}, {Company}, {Seller}", BCCaptions.CommerceLogCaption, state.BindingID, state.Company, seller);

			using (_scope.CreateScope(state.Company, null))
			{
				var graph = PXGraph.CreateInstance<BCAmazonStoreMaint>();
				await graph.ProcessAuthorizationCode(code, seller, state.BindingID); ;
			}

			ctx.Response.Write(CloseWindow());
		}

		private static string CloseWindow()
		{
			return @"
					<html>
					<script type='text/javascript'>
					window.onunload = refreshParent;
					window.close();
				    function refreshParent() {
						window.opener.location.reload();
					}
					</script>
					</html>";
		}

	}

}
