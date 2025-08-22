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
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using System;

namespace PX.Commerce.BigCommerce.API.REST
{
	[CommerceDescription(BigCommerceCaptions.VariantOptions, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
	[JsonObject(Description = "Product -> Product Variant -> Product Variant Option Value")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ProductVariantOptionValueData
	{
		[JsonProperty("option_display_name")]

		public string OptionDisplayName { get; set; }

		[JsonProperty("label")]
		public string Label { get; set; }

		[JsonProperty("id")]
		public int? Id { get; set; }

		[JsonProperty("option_id")]
		public int? OptionId { get; set; }

		/// <summary>
		/// Defines if the instance is similar to the specified <paramref name="attribute"/>.
		/// </summary>
		/// <param name="attribute">The attribute value to compare.</param>
		/// <returns>True if current instance is similar to the specified attribute; otherwise false.</returns>
		public bool IsSimilarTo(AttributeValue attribute)
		{
			if (attribute is null)
				throw new ArgumentNullException(nameof(attribute));

			if (attribute.AttributeDescription is null)
				throw new ArgumentNullException(nameof(attribute.AttributeDescription));

			if (attribute.Value is null)
				throw new ArgumentNullException(nameof(attribute.Value));

			return attribute.AttributeDescription.Value.Equals(this.OptionDisplayName, StringComparison.OrdinalIgnoreCase)
				&& attribute.Value.Value.Equals(this.Label, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Defines if the instance is similar to the specified <paramref name="mappedOption"/>.
		/// </summary>
		/// <param name="mappedOption">The template mapped option to compare.</param>
		/// <returns>True if current instance is similar to the specified template mapped option; otherwise false.</returns>
		public bool IsSimilarTo(TemplateMappedOptions mappedOption)
		{
			if (mappedOption is null)
				throw new ArgumentNullException(nameof(mappedOption));

			return mappedOption.OptionName.Equals(this.OptionDisplayName, StringComparison.OrdinalIgnoreCase)
				&& mappedOption.OptionValue.Equals(this.Label, StringComparison.OrdinalIgnoreCase);
		}
	}
}
