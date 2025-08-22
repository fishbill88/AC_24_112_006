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

using IdentityModel.Client;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PX.Commerce.Amazon.API.Rest;
using PX.Data;

namespace PX.Commerce.Amazon
{
	public class AmazonStateParameters
	{
		private const string Separator = ",";

		public string State { get; }
		public string MarketplaceURL { get; }
		public string RedirectURI { get; }

		public AmazonStateParameters(string bindingID, string marketplaceURL, string CompanyName)
		{
			State = string.Join(Separator, bindingID, CompanyName);
			MarketplaceURL = marketplaceURL;
			RedirectURI = AmazonAuthenticationHandler.ReturnUrl;
		}

		public static (int, string) GetSplittedState(string authorizeResponseState)
		{
			if (string.IsNullOrWhiteSpace(authorizeResponseState) || !authorizeResponseState.Contains(Separator))
			{
				throw new InvalidOperationException("Wrong state");
			}
			int separatorIndex = authorizeResponseState.IndexOf(Separator);

			string binding = authorizeResponseState.Substring(0, separatorIndex);
			string company = authorizeResponseState.Substring(separatorIndex + 1);

			int bindingID;
			if (!int.TryParse(binding, out bindingID))
			{
				throw new PXException(AmazonMessages.WrongBindingState, binding);
			}

			if (string.IsNullOrWhiteSpace(company))
			{
				throw new PXException(AmazonMessages.WrongBindingCompany, company);
			}

			return (bindingID, company);
		}
	}
}
