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
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing an information about the taxes withheld.
	/// </summary>
	public class TaxWithheldComponent
	{
		/// <summary>
		/// Gets os sets the tax collection model applied to the item.
		/// </summary>
		/// <value>The tax collection model applied to the item.</value>
		[JsonProperty("TaxCollectionModel")]
		public string TaxCollectionModel { get; set; }

		/// <summary>
		/// Gets os sets a list of charges that represent the types and amounts of taxes withheld.
		/// </summary>
		/// <value>A list of charges that represent the types and amounts of taxes withheld.</value>
		[JsonProperty("TaxesWithheld")]
		public List<ChargeComponent> TaxesWithheld { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid => this.TaxesWithheld != null;
	}
}
