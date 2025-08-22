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
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.Localizations.CA.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.Localizations.CA
{
	/// <summary>
	/// Extention of <see cref="APSetup"/> with settings used in Canadian Localization
	/// </summary>
	[PXTable(IsOptional = true)]
	public sealed class CanadianAPSetup : PXCacheExtension<APSetup>
	{
		#region IsActive

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.canadianLocalization>();
		}

		#endregion

		public abstract class t5018ThresholdAmount: BqlDecimal.Field<t5018ThresholdAmount> { }
		/// <summary>
		/// Decimal value for the default threshold amount on T5018 forms.
		/// </summary>
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "500.00", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = T5018Messages.ThresholdAmount)]
		public decimal? T5018ThresholdAmount
		{
			get;
			set;
		}

	}
}
