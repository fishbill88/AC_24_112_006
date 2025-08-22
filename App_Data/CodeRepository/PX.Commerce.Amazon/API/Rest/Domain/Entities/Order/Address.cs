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

/* 
 * Created class based on https://developer-docs.amazon.com/sp-api/docs/orders-api-v0-model
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using PX.Commerce.Core;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// The shipping address for the order.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public  class Address : BCAPIEntity
	{
       
        /// <summary>
        /// The address type of the shipping address.
        /// </summary>
        /// <value>The address type of the shipping address.</value>
        [JsonProperty("AddressType")]
        [CommerceDescription(AmazonCaptions.AddressType, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string AddressType { get; set; }
        
        /// <summary>
        /// The name.
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty("Name")]
        [CommerceDescription(AmazonCaptions.Name, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string Name { get; set; }

        /// <summary>
        /// The street address.
        /// </summary>
        /// <value>The street address.</value>
        [JsonProperty("AddressLine1")]
        [CommerceDescription(AmazonCaptions.AddressLine1, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Additional street address information, if required.
        /// </summary>
        /// <value>Additional street address information, if required.</value>
        [JsonProperty("AddressLine2")]
        [CommerceDescription(AmazonCaptions.AddressLine2, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Additional street address information, if required.
        /// </summary>
        /// <value>Additional street address information, if required.</value>
        [JsonProperty("AddressLine3")]
        [CommerceDescription(AmazonCaptions.AddressLine3, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string AddressLine3 { get; set; }

        /// <summary>
        /// The city 
        /// </summary>
        /// <value>The city </value>
        [JsonProperty("City")]
        [CommerceDescription(AmazonCaptions.City, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string City { get; set; }

        /// <summary>
        /// The county.
        /// </summary>
        /// <value>The county.</value>
        [JsonProperty("County")]
        [CommerceDescription(AmazonCaptions.County, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string County { get; set; }

        /// <summary>
        /// The district.
        /// </summary>
        /// <value>The district.</value>
        [JsonProperty("District")]
        [CommerceDescription(AmazonCaptions.District, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string District { get; set; }

        /// <summary>
        /// The state or region.
        /// </summary>
        /// <value>The state or region.</value>
        [JsonProperty("StateOrRegion")]
        [CommerceDescription(AmazonCaptions.StateOrRegion, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string StateOrRegion { get; set; }

        /// <summary>
        /// The municipality.
        /// </summary>
        /// <value>The municipality.</value>
        [JsonProperty("Municipality")]
        public string Municipality { get; set; }

        /// <summary>
        /// The postal code.
        /// </summary>
        /// <value>The postal code.</value>
        [JsonProperty("PostalCode")]
        [CommerceDescription(AmazonCaptions.PostalCode, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string PostalCode { get; set; }

        /// <summary>
        /// The country code. A two-character country code, in ISO 3166-1 alpha-2 format.
        /// </summary>
        /// <value>The country code. A two-character country code, in ISO 3166-1 alpha-2 format.</value>
        [JsonProperty("CountryCode")]
        public string CountryCode { get; set; }

        /// <summary>
        /// The phone number. Not returned for Fulfillment by Amazon (FBA) orders.
        /// </summary>
        /// <value>The phone number. Not returned for Fulfillment by Amazon (FBA) orders.</value>
        [JsonProperty("Phone")]
        [CommerceDescription(AmazonCaptions.Phone, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
        public string Phone { get; set; }

    }

}
