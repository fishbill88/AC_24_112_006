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
using System.Collections.Generic;
using System.Linq;

namespace PX.Commerce.Amazon.API.Rest
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class OrderFinancialEvents : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets a list of shipment event information.
		/// </summary>
		/// <value>A list of shipment event information.</value>
		[JsonProperty("ShipmentEventList")]
		public List<ShipmentEvent> ShipmentEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of refund events.
		/// </summary>
		/// <value>A list of refund events.</value>
		[JsonProperty("RefundEventList")]
		public List<ShipmentEvent> RefundEventList { get; set; }

		/// <summary>
		/// Defines if events of the instance are not empty.
		/// </summary>
		/// <returns>True if events of the instance are not empty; otherwise - false.</returns>
		public bool IsValid
		{
			get => this.ShipmentEventList?.Any() == true
				&& this.RefundEventList?.Any() == true;
		}
	}
}
