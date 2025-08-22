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
using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Commerce.Core;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PX.Commerce.Amazon.API
{
	public class InventoryMessageData : BCAPIEntity
	{
		public List<InventoryMessage> InventoryMessages = new List<InventoryMessage>();
	}

	public class InventoryMessage
	{
		private const string AttributePath = "/attributes/fulfillment_availability";

		private const string ProductTypeConstant = "PRODUCT";

		[JsonProperty("messageId")]
		public int MessageID { get; set; }

		[JsonProperty("sku")]
		public string SKU { get; set; }

		[JsonProperty("operationType")]
		public string OperationType { get; set; }

		[JsonProperty("productType")]
		public string ProductType { get; set; }

		[JsonProperty("patches")]
		public List<Patch> Patches { get; set; }

		[JsonIgnore]
		public Guid OriginalInventotyNoteId { get; set; }
		public InventoryMessage(string sku, int qty)
		{
			this.SKU = sku;
			OperationType = OperationTypes.Patch;
			ProductType = ProductTypeConstant;
			Patches = new() { new() { op = PatchOperations.Replace,
				Path = AttributePath,
				Values = new() { new() { FullfillmentChannelCode = ListingFulfillmentChannel.Default,
				Quantity = qty } } } };
		}

	}

	public class Patch
	{
		[JsonProperty("op")]
		public string op { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("value")]
		public List<Value> Values { get; set; }
	}

	public class Value
	{
		[JsonProperty("fulfillment_channel_code")]
		public string FullfillmentChannelCode { get; set; }

		[JsonProperty("quantity")]
		public int Quantity { get; set; }
	}
}
