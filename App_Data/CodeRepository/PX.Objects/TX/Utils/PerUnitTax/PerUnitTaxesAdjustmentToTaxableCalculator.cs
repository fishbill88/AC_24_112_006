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

using PX.Common;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.TX
{
	/// <summary>
	/// A per unit taxes adjustment to taxable calculator.
	/// </summary>
	public class PerUnitTaxesAdjustmentToTaxableCalculator
	{
		public static readonly PerUnitTaxesAdjustmentToTaxableCalculator Instance = new PerUnitTaxesAdjustmentToTaxableCalculator();

		protected PerUnitTaxesAdjustmentToTaxableCalculator() { }

		public virtual decimal GetPerUnitTaxAmountForTaxableAdjustmentCalculation(Tax taxForTaxableAdustment, PXCache taxDetailCache,
																				  object row, PXCache rowCache, string curyTaxAmtFieldName,
																				  Func<List<object>> perUnitTaxSelector)
		{
			taxForTaxableAdustment.ThrowOnNull(nameof(taxForTaxableAdustment));
			taxDetailCache.ThrowOnNull(nameof(taxDetailCache));
			row.ThrowOnNull(nameof(row));
			rowCache.ThrowOnNull(nameof(rowCache));
			curyTaxAmtFieldName.ThrowOnNullOrWhiteSpace(nameof(curyTaxAmtFieldName));
			perUnitTaxSelector.ThrowOnNull(nameof(perUnitTaxSelector));

			if (taxForTaxableAdustment.TaxType == CSTaxType.PerUnit)
				return 0m;

			Type taxAmountField = taxDetailCache.GetBqlField(curyTaxAmtFieldName);
			List<object> allPerUnitTaxes = perUnitTaxSelector?.Invoke();

			if (allPerUnitTaxes == null || allPerUnitTaxes.Count == 0)
				return 0m;

			var (perUnitInclusiveTaxes, perUnitLevel1Taxes) = GetNonExcludedPerUnitTaxesByCalculationLevel(allPerUnitTaxes);

			if (perUnitInclusiveTaxes.Count == 0 && perUnitLevel1Taxes.Count == 0)
				return 0m;

			switch (taxForTaxableAdustment.TaxCalcLevel)
			{
				case CSTaxCalcLevel.Inclusive when perUnitLevel1Taxes.Count > 0:
					PXTrace.WriteInformation(Messages.CombinationOfExclusivePerUnitTaxAndInclusveTaxIsForbiddenErrorMsg);
					throw new PXSetPropertyException(Messages.CombinationOfExclusivePerUnitTaxAndInclusveTaxIsForbiddenErrorMsg, PXErrorLevel.Error);

				case CSTaxCalcLevel.Inclusive:
					//The adjustment to taxable is amount of all per unit taxes. The level 1 per unit taxes are prohibited.
					return SumTaxAmountsWithReverseAdjustment(taxDetailCache.Graph, perUnitInclusiveTaxes, taxAmountField);

				case CSTaxCalcLevel.CalcOnItemAmt:
					var allNonExcludedPerUnitTaxes = perUnitInclusiveTaxes.Concat(perUnitLevel1Taxes);
					return SumTaxAmountsWithReverseAdjustment(taxDetailCache.Graph, allNonExcludedPerUnitTaxes, taxAmountField);

				case CSTaxCalcLevel.CalcOnItemAmtPlusTaxAmt:
					// For level 2 taxes:
					// Taxable = LineAmt - InclusiveTaxAmt (including per unit inclusive taxes) + Level 1 Taxes amount (including per unit level 1 taxes)
					// Therefore, we need to add only the previously subtracted amount of inclusive per unit taxes
					return SumTaxAmountsWithReverseAdjustment(taxDetailCache.Graph, perUnitInclusiveTaxes, taxAmountField);

				default:
					return 0m;
			}
		}

		private (List<object> PerUnitInclusiveTaxes, List<object> PerUnitLevel1Taxes) GetNonExcludedPerUnitTaxesByCalculationLevel(List<object> allPerUnitTaxes)
		{
			List<object> perUnitLevel1Taxes = new List<object>(capacity: allPerUnitTaxes.Count);
			List<object> perUnitInclusiveTaxes = new List<object>(capacity: allPerUnitTaxes.Count);

			foreach (object taxRow in allPerUnitTaxes)
			{
				Tax perUnitTax = PXResult.Unwrap<Tax>(taxRow);

				if (perUnitTax.TaxCalcLevel2Exclude != false)
					continue;

				switch (perUnitTax.TaxCalcLevel)
				{
					case CSTaxCalcLevel.CalcOnItemQtyInclusively:
						perUnitInclusiveTaxes.Add(taxRow);
						continue;
					case CSTaxCalcLevel.CalcOnItemQtyExclusively:
						perUnitLevel1Taxes.Add(taxRow);
						continue;
				}
			}

			return (perUnitInclusiveTaxes, perUnitLevel1Taxes);
		}

		private decimal SumTaxAmountsWithReverseAdjustment(PXGraph graph, IEnumerable<object> perUnitTaxes, Type taxAmountField)
		{
			decimal totalTaxAmount = 0.0m;
			Type taxType = BqlCommand.GetItemType(taxAmountField);
			PXCache taxDetailCache = graph.Caches[taxType];

			foreach (PXResult taxRow in perUnitTaxes)
			{
				decimal? taxDetailAmount = taxDetailCache.GetValue(taxRow[taxType], taxAmountField.Name) as decimal?;
				Tax tax = (Tax)taxRow[typeof(Tax)];
				decimal multiplier = tax.ReverseTax == true
					? Decimal.MinusOne
					: Decimal.One;

				totalTaxAmount += (taxDetailAmount ?? 0m) * multiplier;
			}

			return totalTaxAmount;
		}
	}
}
