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
using PX.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
	internal static class TokenManager
	{
		private static readonly ConcurrentDictionary<int, Token> _accessTokenCache = new ConcurrentDictionary<int, Token>();
		private static readonly ConcurrentDictionary<(int, string), Token> _restrictedDataTokenCache = new ConcurrentDictionary<(int, string), Token>();
		private static readonly object _accessTokenLock = new object();
		private static readonly object _restrictedDataTokenLock = new object();
		internal static readonly TimeSpan TokenSafetyMargin = TimeSpan.FromMinutes(5);
		internal delegate Task<CreateRestrictedDataTokenResponse> GetRDTTokenDelegate(CreateRestrictedDataTokenRequest dataTokenRequest, CancellationToken cancellationToken);
		internal delegate Task<Token> GetAccessTokenDelegate(BCBindingAmazon bindingAmazon, CancellationToken cancellationToken = default);

		private static SemaphoreSlim semaphoreSlimForAccessToken = new SemaphoreSlim(1);
		internal static async Task<string> GetAccessToken(BCBindingAmazon bindingAmazon, BCBinding binding, GetAccessTokenDelegate getAccessToken, Serilog.ILogger Logger, bool forceNewToken, CancellationToken cancellationToken = default)
		{
			//Check if token is present for given bindingID
			if (!forceNewToken && _accessTokenCache.TryGetValue(binding.BindingID.Value, out var entry) && entry != null)
			{
				// get the time when access token is supposed to expire based off LastModifiedDateTime and ExpiresIn
				// (access token cannot and should not be generated before LastModifiedDateTime
				// - or in other words, access token should only be generated after the store is last saved)
				var expectedExpirationTime = PX.Common.PXTimeZoneInfo.ConvertTimeToUtc(binding.LastModifiedDateTime.Value, PX.Common.LocaleInfo.GetTimeZone()).AddSeconds(entry.ExpiresIn);
				// If access token is present and valid then check if store details have been modified since access token in cache was obtained
				// If the store details have not been modified then just use the one stored in cache
				if (entry.AmazonToken != null && entry.ExpiresAt != null && entry.IsValid() && entry.ExpiresAt.UtcDateTime >= expectedExpirationTime)
					return entry.AmazonToken;
				else
					Logger.Information("{CommerceCaption}: Access token has expired.", BCCaptions.CommerceLogCaption);
			}

			if (string.IsNullOrWhiteSpace(bindingAmazon?.RefreshToken)) throw new PXException(AmazonMessages.AuthorizationRequired);

			await semaphoreSlimForAccessToken.WaitAsync(cancellationToken);
			try
			{
				var token = await getAccessToken(bindingAmazon, cancellationToken);
				_accessTokenCache[binding.BindingID.Value] = token;

				return token.AmazonToken;
			}
			finally
			{
				semaphoreSlimForAccessToken.Release();
			}

		}

		private static SemaphoreSlim semaphoreSlimForRestrictedDataToken = new SemaphoreSlim(1);
		internal static async Task<string> GetRestrictedDataToken(BCBinding binding,Serilog.ILogger Logger, bool forceNewToken, GetRDTTokenDelegate getRDTToken = null, CreateRestrictedDataTokenRequest dataTokenRequest = null, CancellationToken cancellationToken = default)
		{
			//Check if token is present for given bindingID and request
			if (!forceNewToken && _restrictedDataTokenCache.TryGetValue((binding.BindingID.Value, dataTokenRequest.RequestType), out var entry) && entry != null)
			{
				// get the time when access token is supposed to expire based off LastModifiedDateTime and ExpiresIn
				// (access token cannot and should not be generated before LastModifiedDateTime
				// - or in other words, access token should only be generated after the store is last saved)
				var expectedExpirationTime = PX.Common.PXTimeZoneInfo.ConvertTimeToUtc(binding.LastModifiedDateTime.Value, PX.Common.LocaleInfo.GetTimeZone()).AddSeconds(entry.ExpiresIn);
				// If rdt token is present and valid then check if store details have been modified since access token in cache was obtained
				// If the store details have not been modified then just use the one stored in cache
				if (entry.AmazonToken != null && entry.ExpiresAt != null && entry.IsValid() && entry.ExpiresAt.UtcDateTime >= expectedExpirationTime)
					return entry.AmazonToken;
				else
					Logger.Information("{CommerceCaption}: Restricted Data token has expired.", BCCaptions.CommerceLogCaption);
			}

			await semaphoreSlimForRestrictedDataToken.WaitAsync();
			try
			{
				var rdtToken = await getRDTToken(dataTokenRequest, cancellationToken);
				_restrictedDataTokenCache[(binding.BindingID.Value, dataTokenRequest.RequestType)] = new(rdtToken.RestrictedDataToken, DateTimeOffset.UtcNow.AddSeconds(rdtToken.ExpiresIn), rdtToken.ExpiresIn);
				return rdtToken.RestrictedDataToken;
			}
			catch (RestrictedDataTokenNotAuthorizedException ex)
			{
				return null;
			}
			finally
			{
				semaphoreSlimForRestrictedDataToken.Release();
			}
		}
	}

	public class Token
	{
		//accessToken or Restricted Data Token
		public readonly string AmazonToken;
		public readonly string RefreshToken;
		public readonly DateTimeOffset ExpiresAt;
		public readonly double ExpiresIn;

		public Token(string accessToken, DateTimeOffset expiresAt, double expiresIn, string refreshToken = null)
		{
			if(string.IsNullOrWhiteSpace(accessToken))
				throw new PXArgumentException(nameof(accessToken));
			AmazonToken = accessToken;
			RefreshToken = refreshToken;
			ExpiresAt = expiresAt;
			ExpiresIn = expiresIn;
		}

		internal bool IsValid() => ExpiresAt > DateTimeOffset.Now.Add(TokenManager.TokenSafetyMargin);
	}

}
