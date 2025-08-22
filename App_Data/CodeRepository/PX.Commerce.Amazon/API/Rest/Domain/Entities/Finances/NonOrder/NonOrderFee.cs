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

using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Commerce.Core;
using PX.Data;
using System;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents the DTO that stores non-order fee information.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[CommerceDescription(AmazonCaptions.NonOrderFee, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
	public class NonOrderFee : BCAPIEntity
	{
		[CommerceDescription("Amount", FieldFilterStatus.Filterable)]
		public decimal Amount { get; set; }

		[CommerceDescription("Description", FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
		public string FeeDescription { get; set; }

		[CommerceDescription("Posted-date", FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
		public DateTime? PostedDate { get; set; }

        public string EntryType { get; set; }
    }
}
