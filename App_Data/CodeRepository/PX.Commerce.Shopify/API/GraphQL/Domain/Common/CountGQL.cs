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

namespace PX.Commerce.Shopify.API.GraphQL
{
	/// <summary>
	/// The precision of the value returned by a count field.
	/// </summary>
	public class CountGQL : BCAPIEntity
	{
		/// <summary>
		/// Count of elements.
		/// </summary>
		[JsonProperty("count")]
		[GraphQLField("count", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.Int)]
		public int? Count { get; set; }

		/// <summary>
		/// Precision of count, how exact is the value.<br/>
		/// Usually a <see cref="CountPrecision"/>.
		/// </summary>
		[JsonProperty("precision")]
		[GraphQLField("precision", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.String)]
		public string Precision { get; set; }
	}
}
