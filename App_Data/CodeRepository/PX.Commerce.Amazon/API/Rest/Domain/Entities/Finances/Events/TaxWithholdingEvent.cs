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
	/// Represents a dto for storing a TaxWithholding event on seller's account.
	/// </summary>
	public class TaxWithholdingEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the amount which tax was withheld against.
		/// </summary>
		/// <value>The amount which tax was withheld against.</value>
		[JsonProperty("BaseAmount")]
		public Currency BaseAmount { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets the time period for which tax is withheld.
		/// </summary>
		/// <value>The time period for which tax is withheld.</value>
		[JsonProperty("TaxWithholdingPeriod")]
		public TaxWithholdingPeriod TaxWithholdingPeriod { get; set; }

		/// <summary>
		/// Gets or sets the amount of the tax withholding deducted from seller's account.
		/// </summary>
		/// <value>The amount of the tax withholding deducted from seller's account.</value>
		[JsonProperty("WithheldAmount")]
		public Currency WithheldAmount { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.WithheldAmount != null
			&& this.WithheldAmount.IsValid;
	}
}
