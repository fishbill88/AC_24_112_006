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

using System;
using System.Collections.Generic;
using PX.Data;

namespace PX.Objects.Common
{
	/// <summary>
	/// Represents a scope used to shut down <see cref="PXFormulaAttribute"/>
	/// calculation (e.g. for performance reasons). For consistency, the 
	/// field values should be assigned manually within the scope.
	/// </summary>
	public class DisableFormulaCalculationScope : OverrideAttributePropertyScope<PXFormulaAttribute, bool>
	{
		public DisableFormulaCalculationScope(PXCache cache, params Type[] fields)
			: base(
				cache, 
				fields,
				(formula, value) => formula.CancelCalculation = value,
				(formula) => formula.CancelCalculation,
				true)
		{ }

		protected override void AssertAttributesCount(IEnumerable<PXFormulaAttribute> attributesOfType, string fieldName)
		{

		}
	}
}
