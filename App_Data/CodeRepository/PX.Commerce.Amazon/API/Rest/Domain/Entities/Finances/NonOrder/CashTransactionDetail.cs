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

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents the DTO that stores cash transaction details information.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[CommerceDescription(AmazonCaptions.NonOrderFee, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
	public class CashTransactionDetail : CBAPIEntity
	{
		[CommerceDescription("Amount", FieldFilterStatus.Filterable)]
		public DecimalValue Amount { get; set; }

		[CommerceDescription("Amount Description", FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
		public StringValue AmountDescription { get; set; }
	}
}
