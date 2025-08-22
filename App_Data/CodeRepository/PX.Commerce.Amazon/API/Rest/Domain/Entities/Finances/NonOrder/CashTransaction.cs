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

using PX.Api.ContractBased.Models;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a DTO that stores cash transaction information.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[CommerceDescription(AmazonCaptions.NonOrderFee, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
	public class CashTransaction : CBAPIEntity
	{
		public StringValue Description { get; set; }

		public DateTimeValue PostedDate { get; set; }

		public StringValue ExternalReferenceNumber { get; set; }

		public StringValue CashAccountCD { get; set; }

		public List<CashTransactionDetail> Details { get; set; }

		public StringValue EntryTypeCD { get; set; }

        public BooleanValue Hold { get; set; }

		public BooleanValue Approved { get; set; }
	}
}
