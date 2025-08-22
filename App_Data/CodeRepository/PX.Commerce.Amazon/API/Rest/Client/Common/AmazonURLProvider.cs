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

using Newtonsoft.Json;
using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Commerce.Amazon
{
	internal class AmazonUrlProvider
	{
		/*reference :https://developer-docs.amazon.com/sp-api/docs/sp-api-endpoints
		 * https://developer-docs.amazon.com/sp-api/docs/marketplace-ids
		 * https://developer-docs.amazon.com/sp-api/docs/seller-central-urls
		 */
		private static readonly Dictionary<string, Region> AmazonUrls = new Dictionary<string, Region>
		{
			{
				AWSRegions.NorthAmerica,
				new ActiveRegion
				{
					AWSRegion = AWSRegions.NorthAmerica,
					RegionLabel = AmazonCaptions.NorthAmerica,
					RegionURL = new Uri("https://sellingpartnerapi-na.amazon.com"),
					Marketplaces = new Marketplace[]
					{
						new Marketplace { IsActive = true, MarketplaceUrl = new Uri("https://sellercentral.amazon.com"), MarketplaceValue = "ATVPDKIKX0DER", MarketplaceLabel = AmazonCaptions.US, Currency = "USD" },
						new Marketplace { IsActive = true, MarketplaceUrl = new Uri("https://sellercentral.amazon.ca"), MarketplaceValue = "A2EUQ1WTGCTBG2", MarketplaceLabel = AmazonCaptions.Canada, Currency = "CAD" },
						new Marketplace { IsActive = true, MarketplaceUrl = new Uri ("https://sellercentral.amazon.com.mx"), MarketplaceValue = "A1AM78C64UM0Y8", MarketplaceLabel = AmazonCaptions.Mexico, Currency = "MXN" },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral.amazon.com.br"), MarketplaceValue = "A2Q3Y263D00KWC", MarketplaceLabel = AmazonCaptions.Brazil }
					}
				}
			},
			{
				AWSRegions.FarEast,
				new Region
				{
					AWSRegion = AWSRegions.FarEast,
					RegionLabel = AmazonCaptions.FarEast,
					RegionURL = new Uri("https://sellingpartnerapi-fe.amazon.com"),
					Marketplaces = new Marketplace[]
					{
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral.amazon.sg"), MarketplaceValue = "A19VAU5U5O7RUS", MarketplaceLabel = AmazonCaptions.Singapore },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral.amazon.com.au"), MarketplaceValue = "A39IBJ37TRP1C6", MarketplaceLabel = AmazonCaptions.Australia },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral.amazon.co.jp"), MarketplaceValue = "A1VC38T7YXB528", MarketplaceLabel = AmazonCaptions.Japan }
					}
				}
			},
			{
				AWSRegions.Europe,
				new ActiveRegion
				{
					AWSRegion = AWSRegions.Europe,
					RegionLabel = AmazonCaptions.Europe,
					RegionURL = new Uri("https://sellingpartnerapi-eu.amazon.com"),
					Marketplaces = new Marketplace[]
					{
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral-europe.amazon.com"), MarketplaceValue = "A1RKKUPIHCS9HS", MarketplaceLabel = AmazonCaptions.Spain },
						new Marketplace { IsActive = true, MarketplaceUrl = new Uri("https://sellercentral-europe.amazon.com"), MarketplaceValue = "A1F83G8C2ARO7P", MarketplaceLabel = AmazonCaptions.UnitedKingdom },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral-europe.amazon.com"), MarketplaceValue = "A13V1IB3VIYZZH", MarketplaceLabel = AmazonCaptions.France },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral.amazon.com.nl"), MarketplaceValue = "A1805IZSGTT6HS", MarketplaceLabel = AmazonCaptions.Netherlands },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral-europe.amazon.com"), MarketplaceValue = "A1PA6795UKMFR9", MarketplaceLabel = AmazonCaptions.Germany },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral-europe.amazon.com"), MarketplaceValue = "APJ6JRA9NG5V4", MarketplaceLabel = AmazonCaptions.Italy },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral.amazon.com.se"), MarketplaceValue = "A2NODRKZP88ZB9", MarketplaceLabel = AmazonCaptions.Sweden },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral.amazon.eg"), MarketplaceValue = "ARBP9OOSHTCHU", MarketplaceLabel = AmazonCaptions.Egypt },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral.amazon.com.pl"), MarketplaceValue = "A1C3SOZRARQ6R3", MarketplaceLabel = AmazonCaptions.Poland },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral.amazon.com.sa"), MarketplaceValue = "A17E79C6D8DWNP", MarketplaceLabel = AmazonCaptions.SaudiArabia },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral.amazon.com.tr"), MarketplaceValue = "A33AVAJ2PDY3EV", MarketplaceLabel = AmazonCaptions.Turkey },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral.amazon.ae"), MarketplaceValue = "A2VIGQ35RCS4UG", MarketplaceLabel = AmazonCaptions.UnitedArabEmirates },
						new Marketplace { MarketplaceUrl = new Uri("https://sellercentral.amazon.in"), MarketplaceValue = "A21TJRUUN4KGV", MarketplaceLabel = AmazonCaptions.India }
					}
				}
			},
		};

		private static IEnumerable<Region> ActiveRegions => AmazonUrls.Values.Where(region => region.IsActive);

		public static string[] RegionCaptions => ActiveRegions
			.Select(region => region.RegionLabel)
			.ToArray();

		public static string[] Regions => ActiveRegions
			.Select(region => region.AWSRegion)
			.ToArray();

		public static bool TryGetRegion(string regionKey, out Region region)
		{
			region = null;

			if (regionKey is null)
				throw new ArgumentNullException(nameof(regionKey));

			if (string.IsNullOrWhiteSpace(regionKey))
				throw new ArgumentException(nameof(regionKey));

			if (!AmazonUrls.TryGetValue(regionKey, out Region foundRegion)
				|| !foundRegion.IsActive)
			{
				return false;
			}

			region = foundRegion;

			return true;
		}
	}

	internal class Region
	{
		public virtual bool IsActive => false;

		public string AWSRegion { get; set; }

		public string RegionLabel { get; set; }

		public Uri RegionURL { get; set; }

		public virtual Marketplace[] Marketplaces { get; set; }
	}

	internal class ActiveRegion : Region
	{
		public override bool IsActive => true;

		public override Marketplace[] Marketplaces
		{
			get => base.Marketplaces;
			set => base.Marketplaces = value.Where(marketplace => marketplace.IsActive).ToArray();
		}
	}

	internal class Marketplace
	{
		public string Currency { get; set; }

		public bool IsActive { get; set; } = false;

		public string MarketplaceLabel { get; set; }

		public Uri MarketplaceUrl { get; set; }

		public string MarketplaceValue { get; set; }
	}
}
