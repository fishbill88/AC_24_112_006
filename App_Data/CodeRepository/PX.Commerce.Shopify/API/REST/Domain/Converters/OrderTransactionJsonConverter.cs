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
using Newtonsoft.Json.Linq;
using PX.Commerce.Core;
using System;
using System.Reflection;

namespace PX.Commerce.Shopify.API.REST
{
	/// <summary>
	/// Provides functionality to convert an <see cref="OrderTransaction"/> object to and from JSON.
	/// </summary>
	public sealed class OrderTransactionJsonConverter : JsonConverter<OrderTransaction>
	{
		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read. If there is no existing value then <c>null</c> will be used.</param>
		/// <param name="hasExistingValue">The existing value has a value.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>The object value.</returns>
		public override OrderTransaction ReadJson(JsonReader reader, Type objectType, OrderTransaction existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var jObject = JObject.Load(reader);
			var orderTransaction = new OrderTransaction();
			serializer.Populate(jObject.CreateReader(), orderTransaction);
			orderTransaction.Gateway = this.GetGateway(orderTransaction);

			return orderTransaction;
		}

		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
		/// <param name="orderTransaction">The order transaction.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, OrderTransaction orderTransaction, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			var properties = orderTransaction.GetType().GetProperties();

			foreach (PropertyInfo property in properties)
			{
				if (!property.CanWrite
					|| property.GetCustomAttribute<ShouldNotSerializeAttribute>() != null
					|| property.GetCustomAttribute<JsonIgnoreAttribute>() != null)
					continue;

				object propertyValue = property.GetValue(orderTransaction);
				var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();

				if (propertyValue is null &&
					(jsonPropertyAttribute != null && jsonPropertyAttribute.NullValueHandling == NullValueHandling.Ignore
					|| jsonPropertyAttribute is null && serializer.NullValueHandling == NullValueHandling.Ignore))
					continue;

				string propertyName = jsonPropertyAttribute is null
					? property.Name
					: jsonPropertyAttribute.PropertyName;

				writer.WritePropertyName(propertyName);

				if (propertyValue is null)
				{
					writer.WriteNull();
					continue;
				}

				JToken jToken = JToken.FromObject(propertyValue, serializer);
				jToken.WriteTo(writer);
			}
			writer.WriteEndObject();
		}

		private string GetGateway(OrderTransaction orderTransaction)
		{
			if (!string.IsNullOrWhiteSpace(orderTransaction.GatewayInternal)
				&& orderTransaction.GatewayInternal.Equals(ShopifyConstants.ShopifyPayments)
				&& !string.IsNullOrWhiteSpace(orderTransaction.PaymentDetail?.PaymentMethodName)
				&& orderTransaction.PaymentDetail.PaymentMethodName.Equals(ShopifyConstants.ShopifyInstallments))
				return orderTransaction.PaymentDetail.PaymentMethodName;

			return orderTransaction.GatewayInternal;
		}
	}
}
