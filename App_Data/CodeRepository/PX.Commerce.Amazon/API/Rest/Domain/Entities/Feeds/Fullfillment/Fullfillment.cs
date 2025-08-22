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
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PX.Commerce.Amazon.API
{
	public class AmazonShipmentLineDetails
	{
		public string AmazonOrderID;
		public string AmazonOrderItemCode;
		public int Quantity;
		public Guid? PackageId;
	}
	public class AmazonFulfillmentData : BCAPIEntity
	{
		public List<OrderFulfillment> Fulfillments;
	}
	public class OrderFulfillment
	{
		public string AmazonOrderID;
		public string MerchantFulfillmentID;
		public string FulfillmentDate;
		public FulfillmentData FulfillmentData;
		[XmlElement("Item")]
		public List<Item> Item;
		[XmlIgnore]
		public Guid OriginalShipmentNoteId;
	}
	public class FulfillmentData
	{
		public string CarrierName;
		public string ShippingMethod;
		public string ShipperTrackingNumber;
	}
	public class Item
	{
		public string AmazonOrderItemCode;
		public int Quantity;
	}
}