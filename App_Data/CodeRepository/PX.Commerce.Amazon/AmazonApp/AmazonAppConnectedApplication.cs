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

using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace PX.Commerce.Amazon
{
    internal static class AmazonAppConnectedApplication
    {
        //Here until platform allows storing in DB
        internal const string AmazonAppClientId = "ED850204-2666-27D9-1380-30D24D81075E";
        private static readonly string AmazonAppClientSecret = "ELAn8kGGLDkXqAsQc8P6Dg";

        private static readonly List<string> AmazonAppRedirectUris = new List<string>
        {
            "https://amazonconnect.acumatica.com/auth/callback",
            "https://commerceconnect.acumatica.com/amz/auth/callback",
            "https://getpostman.com/oauth2/callback",
            "https://oauth.pstmn.io/v1/callback"
        };

        internal static Client AmazonAppClientForRegistration()
        {
            var amazonAppClient = new Client
            {
                ClientName = "Amazon Public App",
                Enabled = true,
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = AmazonAppRedirectUris,
                ClientSecrets = new List<Secret> { new Secret(AmazonAppClientSecret.Sha256()) },
                AccessTokenType = AccessTokenType.Reference,
                RequireConsent = false,
                AllowedScopes = new List<string>
                {
                    "api"
                }
            };
            return amazonAppClient;
        }
    }
}
