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

using PX.Payroll.Data;
using System;

namespace PX.Objects.PR
{
	/// <summary>
	/// This utility class is needed because the PRTaxCode DAC needs to be an inheritor of the PXBqlTable class like all other DACs.
	/// Earlier the PRTaxCode class was an inheritor of the PRTaxTypeSettingMapper class.
	/// However, starting from the 2024r1 version this inheritance (PRTaxCode : PRTaxTypeSettingMapper) prevents
	/// adding user fields to the PRTaxCode DAC and table in customization projects.
	/// </summary>
	[Serializable]
	public class PRTaxCodeTaxSettingMapper : PRTaxTypeSettingMapper
	{
		public PRTaxCodeTaxSettingMapper(PRTaxCode taxCode)
		{
			TaxState = taxCode.TaxState;
			TypeName = taxCode.TypeName;
		}

		public override string TaxState { get; set; }
		public override string TypeName { get; set; }
	}
}
