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

using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;
using System;

namespace PX.Objects.CA
{
	public sealed class CABankTranFeedSource : PXCacheExtension<CABankTran>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.bankFeedIntegration>();
		}

		public static class FK
		{
			public class BankFeedAccountMappingRow : CA.CABankFeedAccountMapping.PK.ForeignKeyOf<CABankFeedAccountMapping>.By<bankFeedAccountMapID> { }
		}

		#region Source
		public abstract class source : BqlString.Field<source> { }
		[PXUIField(DisplayName = "Source")]
		[CABankFeedType.List]
		[PXDBString(1, IsFixed = true)]
		public string Source { get; set; }
		#endregion

		#region BankFeedAccountMapID
		public abstract class bankFeedAccountMapID : PX.Data.BQL.BqlGuid.Field<bankFeedAccountMapID> { }
		/// <summary>
		/// Indicates that the bank transaction was retrieved from the bank feed account that relates to the bank feed in the multiple mapping mode.
		/// </summary>
		[PXDBGuid]
		[PXUIField(DisplayName = "Bank Feed Account", Enabled = false)]
		[PXSelector(typeof(Search<CABankFeedAccountMapping.bankFeedAccountMapID>),
			typeof(CABankFeedAccountMapping.accountName),
			DescriptionField = typeof(CABankFeedAccountMapping.accountNameMask))]
		public Guid? BankFeedAccountMapID { get; set; }
		#endregion
	}
}
