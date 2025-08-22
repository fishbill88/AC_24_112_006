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
using PX.EFTPlugIn;
using PX.Objects.CA;
using System.Collections.Generic;
using System.Linq;
using PX.Objects.Common;
using SelectorType = PX.ACHPlugInBase.SelectorType;
using PX.Common;

namespace PX.Objects.Localizations.CA
{
	public class PaymentMethodMaintEFTPlugInExt : PXGraphExtension<PaymentMethodMaint>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<Objects.CS.FeaturesSet.canadianLocalization>();

		private bool IsEFTPlugIn() => Base.PaymentMethod.Current.APBatchExportPlugInTypeName == typeof(EFTPlugIn.EFTPlugIn).FullName;

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(ACHExportMethod.ListAttribute))]
		[EFTExportMethod.List]
		public virtual void _(Events.CacheAttached<PaymentMethod.aPBatchExportMethod> e) { }

		public delegate Dictionary<string, string> GetPlugInSettingsDelegate();

		[PXOverride]
		public virtual Dictionary<string, string> GetPlugInSettings(GetPlugInSettingsDelegate baseMethod)
		{
			
			var result = baseMethod();

			if (IsEFTPlugIn())
			{
				result.AddRange(EFTPlugInSettingsMapping.Settings);
			}

			return result;
		}

		public delegate Dictionary<SelectorType?, string> GetPlugInSelectorTypesDelegate();

		[PXOverride]
		public virtual Dictionary<SelectorType?, string> GetPlugInSelectorTypes(GetPlugInSelectorTypesDelegate baseMethod)
		{
			var result = baseMethod();

			if (IsEFTPlugIn())
			{
				result.AddRange(EFTPlugInSettingsMapping.SelectorTypes);
			}

			return result;
		}

		/// <summary>
		/// Override of <see cref="PaymentMethodMaint.UpdatePlugInSettings(PaymentMethod)"/>
		/// </summary>
		[PXOverride]
		public virtual void UpdatePlugInSettings(PaymentMethod pm)
		{
			var isPluginSelected = pm.APBatchExportMethod == ACHExportMethod.PlugIn;
			if (isPluginSelected && !Base.aCHPlugInParameters.Any() && pm?.DirectDepositFileFormat == EFTDirectDepositType.Code)
			{
				if (string.IsNullOrEmpty(pm.APBatchExportPlugInTypeName))
				{
					var copy = (PaymentMethod)Base.PaymentMethod.Cache.CreateCopy(pm);
					copy.APBatchExportPlugInTypeName = typeof(EFTPlugIn.EFTPlugIn).FullName;
					Base.PaymentMethod.Update(copy);
				}

				if (!IsEFTPlugIn() || pm?.DirectDepositFileFormat != EFTDirectDepositType.Code)
				{
					return;
				}

				if (!Base.IsCopyPasteContext)
				{
					bool settingsExists = CheckIfEFTSettingsExists();
					if (!settingsExists)
					{
						AppendDefaultSettings(pm.DirectDepositFileFormat);
						AppendDefaultPlugInParameters();
					}
					else
					{
						if (CheckAcumaticaExportScenariosMapping(pm.DirectDepositFileFormat))
						{
							UpdateDetailsAccordingToPlugIn();
							AppendDefaultPlugInParameters(useExportScenarioMapping: true);
						}
						else
						{
							AppendDefaultPlugInParameters();
						}
					}
				}
			}
		}

		private void AppendDefaultPlugInParameters(bool useExportScenarioMapping = false) => Base.AppendDefaultPlugInParameters(DefaultPaymentMethodDetailsHelper.Dictionary.ToIntKey(), useExportScenarioMapping: useExportScenarioMapping);

		private bool CheckIfEFTSettingsExists() => Base.DetailsForCashAccount.Any() || Base.DetailsForVendor.Any();

		private bool CheckAcumaticaExportScenariosMapping(string directDepositFileFormat)
		{
			var defaultDetails = Base.DirectDepositService?.GetDefaults(directDepositFileFormat);
			var remSettings = defaultDetails.Where(m => m.UseFor == PaymentMethodDetailUsage.UseForCashAccount).ToDictionary(m => m.DetailID, m => m.Descr);
			var vendorSettings = defaultDetails.Where(m => m.UseFor == PaymentMethodDetailUsage.UseForVendor).ToDictionary(m => m.DetailID, m => m.Descr);

			try
			{
				foreach (PaymentMethodDetail detail in Base.DetailsForCashAccount.Select())
				{
					var id = detail.DetailID.Trim();
					if (remSettings[id] != detail.Descr.Trim())
					{
						return false;
					}
				}

				foreach (PaymentMethodDetail detail in Base.DetailsForVendor.Select())
				{
					var id = detail.DetailID.Trim();
					if (vendorSettings[id] != detail.Descr.Trim())
					{
						return false;
					}
				}
			}
			catch
			{
				return false;
			}

			return true;
		}

		private void AppendDefaultSettings(string directDepositFileFormat)
		{
			var details = Base.DirectDepositService?.GetDefaults(directDepositFileFormat);
			if (details != null)
			{
				foreach (var det in details)
				{
					Base.Details.Cache.Insert(det);
				}
			}
		}

		private void UpdateDetailsAccordingToPlugIn()
		{
			foreach (PaymentMethodDetail detail in Base.DetailsForCashAccount.Select())
			{
				if (detail.DetailID == "9" && detail.Descr == "File Name")
				{
					Base.DetailsForCashAccount.Delete(detail);
				}
			}

			foreach (PaymentMethodDetail detail in Base.DetailsForVendor.Select())
			{
				if (detail.DetailID == "2" && detail.Descr == "Branch" && detail.IsRequired != true)
				{
					detail.IsRequired = true;
					Base.DetailsForCashAccount.Update(detail);
				}
			}
		}
	}
}
