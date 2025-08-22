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
using System.Linq;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM.Extensions;
using PX.Objects.CS;
using PX.Objects.TX;
using PX.Objects.IN;

using Messages = PX.Objects.TX.Messages;
using PX.Objects.Extensions.MultiCurrency;

namespace PX.Objects.Extensions.SalesTax
{
	public abstract partial class TaxBaseGraph<TGraph, TPrimary> : PXGraphExtension<TGraph>
		 where TGraph : PXGraph
		 where TPrimary : class, IBqlTable, new()
	{
		protected virtual bool IsPerUnitTax(Tax tax) => tax?.TaxType == CSTaxType.PerUnit;

		#region Filling Taxes For Lines
		/// <summary>
		/// Fill tax details for line for per unit taxes.
		/// </summary>
		/// <exception cref="PXArgumentException">Thrown when a PX Argument error condition occurs.</exception>
		/// <param name="row">The row.</param>
		/// <param name="tax">The tax.</param>
		/// <param name="taxRevision">The tax revision.</param>
		/// <param name="taxDetail">The tax detail.</param>
		protected virtual void TaxSetLineDefaultForPerUnitTaxes(Detail row, Tax tax, TaxRev taxRevision, TaxDetail taxDetail)
		{
			decimal taxableQty;
			decimal curyTaxAmount;

			switch (tax.TaxCalcLevel)
			{
				case CSTaxCalcLevel.CalcOnItemQtyExclusively:
				case CSTaxCalcLevel.CalcOnItemQtyInclusively:
					taxableQty = GetTaxableQuantityForPerUnitTaxes(row, tax, taxRevision);
					curyTaxAmount = GetTaxAmountForPerUnitTaxWithCorrectSign(row, tax, taxRevision, taxDetail, taxableQty).CuryTaxAmount;
					break;
				default:
					PXTrace.WriteError(Messages.NotSupportedPerUnitTaxCalculationLevelErrorMsg);
					throw new PXArgumentException(Messages.NotSupportedPerUnitTaxCalculationLevelErrorMsg);
			}

			FillTaxDetailValuesForPerUnitTax(tax, taxRevision, taxDetail, row, taxableQty, curyTaxAmount);
		}

		private decimal GetTaxableQuantityForPerUnitTaxes(Detail row, Tax tax, TaxRev taxRevison)
		{
			if (row.Qty == null || row.Qty == 0m || tax.TaxUOM == null)
				return 0m;

			// Even in case the TaxUOM and Line UOM are equal we still need to execute conversion 
			// because line quantity is specified in the base UOM, which could differ from the Tax UOM  
			decimal? lineQuantity = ConvertLineQtyToTaxUOM(row, tax);

			if (lineQuantity == null)
				return 0m;

			return GetAdjustedTaxableQuantity(lineQuantity.Value, taxRevison);
		}

		private decimal GetAdjustedTaxableQuantity(decimal lineQty, TaxRev taxRevison)
		{
			if (taxRevison.TaxableMaxQty == null)
				return lineQty;

			return lineQty <= taxRevison.TaxableMaxQty.Value
				? lineQty
				: taxRevison.TaxableMaxQty.Value;
		}

		protected virtual decimal? ConvertLineQtyToTaxUOM(Detail row, Tax tax)
		{
			if (tax.TaxUOM == row.UOM)  //Optimization to avoid conversion at all if document line UOM is equal to per-unit tax UOM
				return row.Qty;

			if (row.InventoryID == null && string.IsNullOrEmpty(row.UOM))
			{
				string errorMessage = PXMessages.LocalizeFormatNoPrefixNLA(Messages.MissingInventoryAndLineUomForPerUnitTaxErrorMsgFormat,
																		   tax.TaxID, CurrentDocument.TaxZoneID, row.TaxCategoryID);
				SetPerUnitTaxUomConversionError(row, tax, errorMessage);
				return null;
			}

			decimal? lineQtyInTaxUom = null;

			if (row.InventoryID != null)
			{
				try
				{
					decimal lineQuantityInBaseUomNotRounded = GetNotRoundedLineQuantityInBaseUOM(row);
					lineQtyInTaxUom = INUnitAttribute.ConvertFromBase(Details.Cache, row.InventoryID, tax.TaxUOM,
																	  lineQuantityInBaseUomNotRounded, INPrecision.QUANTITY);
				}
				catch (PXUnitConversionException)
				{
					lineQtyInTaxUom = null;
				}
			}

			if (lineQtyInTaxUom == null)  //Try to use global conversions
			{
				lineQtyInTaxUom = ConvertLineQtyToTaxUOMWithGlobalConversions(row, tax);

				if (lineQtyInTaxUom == null)
				{
					string errorMessage = PXMessages.LocalizeFormatNoPrefixNLA(Messages.MissingUomConversionForPerUnitTaxErrorMsgFormat,
																			   tax.TaxID, CurrentDocument.TaxZoneID, row.TaxCategoryID, tax.TaxUOM);
					SetPerUnitTaxUomConversionError(row, tax, errorMessage);
				}
			}

			return lineQtyInTaxUom;
		}

		protected virtual decimal GetNotRoundedLineQuantityInBaseUOM(Detail row)
		{
			return INUnitAttribute.ConvertToBase(Details.Cache, row.InventoryID, row.UOM, row.Qty.Value, INPrecision.NOROUND);
		}

		protected decimal? ConvertLineQtyToTaxUOMWithGlobalConversions(Detail row, Tax tax)
		{
			if (INUnitAttribute.TryConvertGlobalUnits(Base, row.UOM, tax.TaxUOM, row.Qty.Value, INPrecision.QUANTITY, out decimal lineQtyInTaxUom))
				return lineQtyInTaxUom;
			else
				return null;
		}

		protected virtual void SetPerUnitTaxUomConversionError(Detail row, Tax tax, string errorMessage)
		{
			PXException exception = new PXSetPropertyException(errorMessage, PXErrorLevel.Error);
			Details.Cache.RaiseExceptionHandling(GetDetailMapping().UOM.Name, row, row.UOM, exception);
		}

		protected virtual (decimal TaxAmount, decimal CuryTaxAmount) GetTaxAmountForPerUnitTaxWithCorrectSign(
			Detail row, Tax tax, TaxRev taxRevison, TaxDetail taxDetail, decimal taxableQty)
		{
			var (taxAmount, curyTaxAmount) = GetTaxAmountForPerUnitTax(taxRevison, taxDetail, taxableQty);

			if (taxAmount == 0m && curyTaxAmount == 0m)
				return (taxAmount, curyTaxAmount);

			if (InvertPerUnitTaxAmountSign(row, tax, taxRevison, taxDetail))
				return (-taxAmount, -curyTaxAmount);
			else
				return (taxAmount, curyTaxAmount);
		}

		protected virtual (decimal TaxAmount, decimal CuryTaxAmount) GetTaxAmountForPerUnitTax(TaxRev taxRevison, TaxDetail taxDetail, decimal taxableQty)
		{
			decimal taxRateForPerUnitTaxes = GetTaxRateForPerUnitTaxes(taxRevison);
			decimal taxAmount = taxableQty * taxRateForPerUnitTaxes;
			CurrencyInfo rowCuryInfo = Base.FindImplementation<IPXCurrencyHelper>().GetDefaultCurrencyInfo();
			return (rowCuryInfo.RoundCury(taxAmount), rowCuryInfo.CuryConvCury(taxAmount));
		}

		protected virtual decimal GetTaxRateForPerUnitTaxes(TaxRev taxRevison) =>
			taxRevison.TaxRate > 0m ? taxRevison.TaxRate.Value : 0m;

		protected virtual bool InvertPerUnitTaxAmountSign(Detail row, Tax tax, TaxRev taxRevison, TaxDetail taxDetail)
		{
			return false;
		}

		protected virtual void FillTaxDetailValuesForPerUnitTax(
			Tax tax, TaxRev taxRevision, TaxDetail taxDetail, Detail row, decimal taxableQty, decimal curyTaxAmount)
		{
			taxDetail.TaxUOM = tax.TaxUOM;
			taxDetail.TaxableQty = taxableQty;
			taxDetail.TaxRate = taxRevision.TaxRate;
			taxDetail.NonDeductibleTaxRate = taxRevision.NonDeductibleTaxRate;

			switch (tax.TaxCalcLevel)
			{
				case CSTaxCalcLevel.CalcOnItemQtyInclusively:
					FillLineTaxableAndTaxAmountsForInclusivePerUnitTax(taxDetail, row, tax);
					break;

				case CSTaxCalcLevel.CalcOnItemQtyExclusively when tax.TaxCalcLevel2Exclude == false:
					CheckThatExclusivePerUnitTaxIsNotUsedWithInclusiveNonPerUnitTax(row);
					break;
			}

			bool isExemptTaxCategory = IsExemptTaxCategory(row);

			if (!isExemptTaxCategory)
			{
				SetTaxDetailTaxAmount(Taxes.Cache, taxDetail, curyTaxAmount);
			}

			///FillDiscountAmountsForPerUnitTax( taxDetail);

			if (taxRevision.TaxID != null && tax.DirectTax != true)
			{
				Taxes.Update(taxDetail);
			}
			else
			{
				Delete(Taxes.Cache, taxDetail);
			}
		}

		//private void FillDiscountAmountsForPerUnitTax(PXCache taxDetailCache, TaxDetail taxDetail)
		//{
		//	const decimal curyTaxableDiscountAmt = 0m;
		//	const decimal curyTaxDiscountAmt = 0m;

		//	SetValueOptional(taxDetailCache, taxDetail, curyTaxableDiscountAmt, _CuryTaxableDiscountAmt);
		//	SetValueOptional(taxDetailCache, taxDetail, curyTaxDiscountAmt, _CuryTaxDiscountAmt);
		//}

		private void CheckThatExclusivePerUnitTaxIsNotUsedWithInclusiveNonPerUnitTax(Detail row)
		{
			var hasInclusiveNonPerUnitTaxes = SelectInclusiveTaxes(row)
				.Select(taxRow => PXResult.Unwrap<Tax>(taxRow))
				.Any(inclusiveTax => inclusiveTax != null && !IsPerUnitTax(inclusiveTax));
			if (hasInclusiveNonPerUnitTaxes)
			{
				PXTrace.WriteInformation(Messages.CombinationOfExclusivePerUnitTaxAndInclusveTaxIsForbiddenErrorMsg);
				throw new PXSetPropertyException(Messages.CombinationOfExclusivePerUnitTaxAndInclusveTaxIsForbiddenErrorMsg, PXErrorLevel.Error);
			}
		}

		private void FillLineTaxableAndTaxAmountsForInclusivePerUnitTax(TaxDetail taxDetail, Detail row, Tax tax)
		{
			decimal curyLineAmount = GetCuryTranAmt(Details.Cache, row) ?? 0m;
			decimal curyLineInclusivePerUnitTaxAmount = GetInclusivePerUnitTaxesTotalAmount(taxDetail, row);
			decimal curyLineTaxableAmount = curyLineAmount - curyLineInclusivePerUnitTaxAmount;

			SetTaxableAmt(row, curyLineTaxableAmount);
			SetTaxAmt(row, curyLineInclusivePerUnitTaxAmount);
		}

		private decimal GetInclusivePerUnitTaxesTotalAmount(TaxDetail taxDetail, Detail row)
		{
			//Type taxDetailsCuryTaxAmountFieldType = taxDetailCache.GetBqlField(_CuryTaxAmt);

			//if (taxDetailsCuryTaxAmountFieldType == null)
			//	return 0m;

			List<object> inclusivePerUnitTaxRows = GetInclusivePerUnitTaxRows(row)?.ToList();

			if (inclusivePerUnitTaxRows == null || inclusivePerUnitTaxRows.Count == 0)
				return 0m;

			decimal curyTotalInclusivePerUnitTaxAmount = 0m;

			foreach (PXResult inclusiveNonPerUnitTaxRow in inclusivePerUnitTaxRows)
			{
				TaxRev inclusiveTaxRevision = inclusiveNonPerUnitTaxRow.GetItem<TaxRev>();
				Tax currentInclusiveTax = inclusiveNonPerUnitTaxRow.GetItem<Tax>();
				decimal taxableQty = GetTaxableQuantityForPerUnitTaxes(row, currentInclusiveTax, inclusiveTaxRevision);
				decimal? taxRate = inclusiveTaxRevision.TaxRate;
				var (_, curyTaxAmount) = GetTaxAmountForPerUnitTaxWithCorrectSign(row, currentInclusiveTax, inclusiveTaxRevision, taxDetail, taxableQty);
				if (currentInclusiveTax.ReverseTax == true)
				{
					curyTaxAmount = -curyTaxAmount;
				}

				curyTotalInclusivePerUnitTaxAmount += curyTaxAmount;
			}

			return curyTotalInclusivePerUnitTaxAmount;
		}

		private IEnumerable<object> GetInclusivePerUnitTaxRows(Detail row)
		{
			List<object> inclusiveTaxes = SelectInclusiveTaxes(row);

			if (inclusiveTaxes == null || inclusiveTaxes.Count == 0)
				yield break;

			foreach (PXResult inclusiveTaxRow in inclusiveTaxes)
			{
				Tax inclusiveTax = inclusiveTaxRow.GetItem<Tax>();

				if (inclusiveTax != null && IsPerUnitTax(inclusiveTax))
				{
					yield return inclusiveTaxRow;
				}
			}
		}

		#endregion

		#region Filling Aggregated Taxes
		/// <summary>
		/// Fill aggregated tax detail for per unit tax.
		/// </summary>
		/// <param name="rowCache">The row cache.</param>
		/// <param name="row">The row.</param>
		/// <param name="tax">The tax.</param>
		/// <param name="taxRevision">The tax revision.</param>
		/// <param name="aggrTaxDetail">The aggregated tax detail.</param>
		/// <param name="taxItems">The tax items.</param>
		/// <returns/>
		protected virtual TaxTotal FillAggregatedTaxDetailForPerUnitTax(object row, Tax tax, TaxRev taxRevision, TaxTotal aggrTaxDetail, List<object> taxItems)
		{
			aggrTaxDetail.TaxableQty = Sum(taxItems, typeof(TaxDetail.taxableQty));
			aggrTaxDetail.TaxUOM = tax.TaxUOM;
			return aggrTaxDetail;
		}
		#endregion

		#region Calculation of Per Unit Tax correction to taxable amount for other taxes
		protected virtual decimal GetPerUnitTaxAmountForTaxableAdjustmentCalculation(Tax taxForTaxableAdustment, TaxDetail taxDetail,																					 Detail row)
		{
			if (taxForTaxableAdustment.TaxType == CSTaxType.PerUnit)
				return 0m;

			PerUnitTaxesAdjustmentToTaxableCalculator taxAdjustmentCalculator = GetPerUnitTaxAdjustmentCalculator();

			if (taxAdjustmentCalculator == null)
				return 0m;

			decimal? taxAdjustment =
				taxAdjustmentCalculator?.GetPerUnitTaxAmountForTaxableAdjustmentCalculation(taxForTaxableAdustment, Taxes.Cache, row, Details.Cache,
															curyTaxAmtFieldName: nameof(TaxDetail.CuryTaxAmt),
															perUnitTaxSelector: () => SelectPerUnitTaxesForTaxableAdjustmentCalculation(Taxes.Cache.Graph, row));
			return taxAdjustment ?? 0m;
		}

		protected virtual PerUnitTaxesAdjustmentToTaxableCalculator GetPerUnitTaxAdjustmentCalculator() =>
			PerUnitTaxesAdjustmentToTaxableCalculator.Instance;

		protected virtual List<object> SelectPerUnitTaxesForTaxableAdjustmentCalculation(PXGraph graph, object row) =>
			IsExemptTaxCategory(row)
				? new List<object>()
				: SelectTaxes<Where<Tax.taxType, Equal<CSTaxType.perUnit>,
								And<Tax.taxCalcLevel2Exclude, Equal<False>>>>(graph, row, PXTaxCheck.Line);
		#endregion
	}
}
