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

namespace PX.Objects.TX
{
	public interface ITaxableDetail
	{
		string TaxID { get; }
		decimal? GroupDiscountRate { get; }
		decimal? DocumentDiscountRate { get; }
		decimal? CuryTranAmt { get; }
		decimal? TranAmt { get; }
		decimal? CuryRetainageAmt { get; }
		decimal? RetainageAmt { get; }
	}

	public interface ITaxDetailWithAmounts
	{
		string TaxID { get; }
		decimal? CuryTaxableAmt { get; }
		decimal? TaxableAmt { get; }
		decimal? CuryTaxAmt { get; }
		decimal? TaxAmt { get; }
		decimal? CuryRetainedTaxableAmt { get; }
		decimal? RetainedTaxableAmt { get; }
	}
}
