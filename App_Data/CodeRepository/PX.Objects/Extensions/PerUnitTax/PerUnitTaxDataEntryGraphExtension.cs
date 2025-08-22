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

using System.Linq;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.TX;

namespace PX.Objects.Extensions.PerUnitTax
{
	/// <summary>
	/// A per-unit tax graph extension for data entry graphs which will forbid edit of per-unit taxes in UI.
	/// </summary>
	public abstract class PerUnitTaxDataEntryGraphExtension<TGraph, TaxDetailDAC> : PXGraphExtension<TGraph>
	where TGraph : PXGraph
	where TaxDetailDAC : TaxDetail, IBqlTable, new()
	{
		protected static bool IsActiveBase() => PXAccess.FeatureInstalled<FeaturesSet.perUnitTaxSupport>();

		protected virtual void _(Events.RowSelected<TaxDetailDAC> e)
		{
			if (e.Row == null)
				return;

			const string fieldName = nameof(TaxDetail.TaxID);
			var uiFieldAttribute = e.Cache.GetAttributesOfType<PXUIFieldAttribute>(e.Row, fieldName)
										  .FirstOrDefault();

			if (uiFieldAttribute == null || uiFieldAttribute.Enabled == false)
				return;

			Tax tax = GetTax(e.Row);
			ConfigureUI(e.Cache, e.Row, tax);
		}

		protected virtual void TaxDetailDAC_RowPersisting(Events.RowPersisting<TaxDetailDAC> e)
		{
			if (!(e.Row is TaxTran))
				return;

			Tax tax = GetTax(e.Row);

			if (tax != null)
			{
				ConfigureChecks(e.Cache, e.Row, tax);
			}
		}

		/// <summary>
		/// Gets a tax from <see cref="TaxDetail"/>.
		/// </summary>
		/// <param name="taxDetail">The taxDetail to act on.</param>
		/// <returns/>
		protected Tax GetTax(TaxDetailDAC taxDetail)
		{
			if (taxDetail == null)
				return null;

			return PXSelect<Tax,
					  Where<Tax.taxID, Equal<Required<Tax.taxID>>>>
				  .SelectSingleBound(Base, currents: null, pars: taxDetail.TaxID);
		}

		protected virtual void ConfigureUI(PXCache cache, TaxDetailDAC taxDetail, Tax tax)
		{
			bool isPerUnitTax = tax?.TaxType == CSTaxType.PerUnit;
			PXUIFieldAttribute.SetEnabled(cache, taxDetail, !isPerUnitTax);
		}

		protected virtual void ConfigureChecks(PXCache cache, TaxDetailDAC taxDetail, Tax tax)
		{
			bool areAccountAndSubaccountRequired = tax.TaxType != CSTaxType.PerUnit || tax.PerUnitTaxPostMode == PerUnitTaxPostOptions.TaxAccount;
			PXPersistingCheck persistingCheck = areAccountAndSubaccountRequired
				? PXPersistingCheck.NullOrBlank
				: PXPersistingCheck.Nothing;

			cache.Adjust<PXDefaultAttribute>(taxDetail)
				 .For<TaxTran.accountID>(a => a.PersistingCheck = persistingCheck)
				 .SameFor<TaxTran.subID>();
		}

		protected void _(Events.RowDeleting<TaxDetailDAC> e)
		{
			if (e.ExternalCall && CheckIfTaxDetailHasPerUnitTaxType(e.Row)) //Forbid to delete per-unit tax from UI
			{
				e.Cancel = true;
				throw new PXException(TX.Messages.PerUnitTaxCannotBeDeletedManuallyErrorMsg);
			}
		}

		private bool CheckIfTaxDetailHasPerUnitTaxType(TaxDetailDAC taxDeatil)
		{
			return GetTax(taxDeatil)?.TaxType == CSTaxType.PerUnit;
		}
	}
}
