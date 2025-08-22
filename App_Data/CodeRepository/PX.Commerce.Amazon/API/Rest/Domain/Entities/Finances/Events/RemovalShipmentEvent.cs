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
using PX.Commerce.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing a removal shipment event for a removal order.
	/// </summary>
	public class RemovalShipmentEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the merchant removal orderId.	
		/// </summary>
		/// <value>The merchant removal orderId.</value>
		[JsonProperty("MerchantOrderId")]
		public string MerchantOrderId { get; set; }

		/// <summary>
		/// Gets or sets the identifier for the removal shipment order.	
		/// </summary>
		/// <value>The identifier for the removal shipment order.</value>
		[JsonProperty("OrderId")]
		public string OrderId { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.	
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets a list of removal shipment items.	
		/// </summary>
		/// <value>A list of removal shipment items.</value>
		[JsonProperty("RemovalShipmentItemList")]
		public List<RemovalShipmentItem> RemovalShipmentItemList { get; set; }

		/// <summary>
		/// Gets or sets the type of removal order.	
		/// </summary>
		/// <value>The type of removal order.</value>
		[JsonProperty("TransactionType")]
		public string TransactionType { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.RemovalShipmentItemList != null
			&& this.RemovalShipmentItemList.All(shipmentItem => shipmentItem.IsValid)
			&& !string.IsNullOrWhiteSpace(this.TransactionType);
	}
}
