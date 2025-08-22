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
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.Constants;
using PX.Commerce.Core;
using PX.Commerce.Objects;
using PX.Common;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Commerce.Amazon
{
	#region BCAmazonMarketPlaceAttribute
	public class BCAmazonMarketPlaceAttribute : PXStringListAttribute, IPXRowSelectedSubscriber
	{
		Type _region;

		public BCAmazonMarketPlaceAttribute(Type region)
		{
			_region = region;
		}

		public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			BCBindingAmazon row = e.Row as BCBindingAmazon;
			if (row == null) return;
			String endpoint = (String)sender.GetValue(row, _region.Name);
			if (endpoint != null)
			{
				if (AmazonUrlProvider.TryGetRegion(endpoint, out Region region))
					PXStringListAttribute.SetList<BCBindingAmazon.marketplace>(sender, row, region.Marketplaces.Select(x => x.MarketplaceValue).ToArray(), region.Marketplaces.Select(x => x.MarketplaceLabel).ToArray());
			}
		}
	}
	#endregion

	#region BCAmazonEndpointAttribute
	public class BCAmazonRegionAttribute : PXStringListAttribute
	{
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			_AllowedValues = AmazonUrlProvider.Regions;
			_AllowedLabels = AmazonUrlProvider.RegionCaptions;
		}
	}
	#endregion

	#region MyRegion
	public static class AuthType
	{
		public const string Cloud = "CLD";
		public const string Self = "SLF";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute() : base(new string[] { Cloud, Self },
										  new string[] { AmazonCaptions.CloudAuth, AmazonCaptions.SelfAuth })
			{ }
		}
	}
	#endregion

	#region FeeTypesList
	public class FeeTypesListAttribute : PXStringListAttribute
	{
		public FeeTypesListAttribute()
			: base(new string[] { FeeType.OrderRelated, FeeType.NonOrderRelated, },
				   new string[] { AmazonCaptions.OrderFeeType, AmazonCaptions.NonOrderFeeType })
		{ }
	}
	#endregion
}
