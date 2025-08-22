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

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing a fee event related to Amazon Imaging services.
	/// </summary>
	public class ImagingServicesFeeEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the Amazon Standard Identification Number (ASIN) of the item for which the imaging service was requested.
		/// </summary>
		/// <value>The Amazon Standard Identification Number (ASIN) of the item for which the imaging service was requested.</value>
		[JsonProperty("ASIN")]
		public string Asin { get; set; }

		/// <summary>
		/// Gets or sets the Amazon Standard Identification Number (ASIN) of the item for which the imaging service was requested.
		/// </summary>
		/// <value>The Amazon Standard Identification Number (ASIN) of the item for which the imaging service was requested.</value>
		[JsonProperty("FeeList")]
		public List<FeeComponent> FeeList { get; set; }

		/// <summary>
		/// Gets or sets the identifier for the imaging services request.
		/// </summary>
		/// <value>The identifier for the imaging services request.</value>
		[JsonProperty("ImagingRequestBillingItemID")]
		public string ImagingRequestBillingItemID { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid => this.FeeList != null;
	}
}
