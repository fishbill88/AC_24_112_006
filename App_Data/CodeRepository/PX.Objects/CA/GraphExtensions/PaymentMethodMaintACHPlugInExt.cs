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

using PX.ACHPlugInBase;
using PX.Common;
using PX.Data;
using PX.Objects.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.CA
{
	public class PaymentMethodMaintACHPlugInExt : PXGraphExtension<PaymentMethodMaint>
	{
		public static bool IsActive() => true;

		public delegate Dictionary<string, string> GetPlugInSettingsDelegate();

		[PXOverride]
		public virtual Dictionary<string, string> GetPlugInSettings(GetPlugInSettingsDelegate baseMethod)
		{
			var result = baseMethod();

			if (Base.IsACHPlugIn())
			{
				result.AddRange(ACHPlugInSettingsMapping.Settings);
			}

			return result;
		}

		public delegate Dictionary<SelectorType?, string> GetPlugInSelectorTypesDelegate();

		[PXOverride]
		public virtual Dictionary<SelectorType?, string> GetPlugInSelectorTypes(GetPlugInSelectorTypesDelegate baseMethod)
		{
			var result = baseMethod();

			if (Base.IsACHPlugIn())
			{
				result.AddRange(ACHPlugInSettingsMapping.SelectorTypes);
			}

			return result;
		}

		public delegate IEnumerable<ACHPlugInParameter> ApplyFiltersDelegate(IEnumerable<ACHPlugInParameter> aCHPlugInParameters);

		[PXOverride]
		public virtual IEnumerable<ACHPlugInParameter> ApplyFilters(IEnumerable<ACHPlugInParameter> aCHPlugInParameters, ApplyFiltersDelegate baseMethod)
		{
			aCHPlugInParameters = baseMethod(aCHPlugInParameters);

			if (!Base.IsACHPlugIn())
			{
				return aCHPlugInParameters;
			}

			bool includeOffsetRecord = false;
			bool includeAddendaRecords = false;

			foreach (ACHPlugInParameter param in aCHPlugInParameters)
			{
				if (param.ParameterID.ToUpper() == nameof(IACHExportParameters.IncludeOffsetRecord).ToUpper())
				{
					bool.TryParse(param.Value, out includeOffsetRecord);
				}
				if (param.ParameterID.ToUpper() == nameof(IACHExportParameters.IncludeAddendaRecords).ToUpper())
				{
					bool.TryParse(param.Value, out includeAddendaRecords);
				}
			}

			var result = new List<ACHPlugInParameter>();

			foreach (ACHPlugInParameter storedParam in aCHPlugInParameters)
			{
				if (Base.plugInFilter.Current?.ShowOffsetSettings != true && (storedParam.ParameterID.ToUpper() == nameof(IACHExportParameters.OffsetDFIAccountNbr).ToUpper()
										|| storedParam.ParameterID.ToUpper() == nameof(IACHExportParameters.OffsetReceivingDEFIID).ToUpper()
										|| storedParam.ParameterID.ToUpper() == nameof(IACHExportParameters.OffsetReceivingID).ToUpper()))
				{
					continue;
				}
				if (!includeAddendaRecords && storedParam.ParameterID.ToUpper() == nameof(IACHExportParameters.AddendaRecordTemplate).ToUpper())
				{
					continue;
				}

				result.Add(storedParam);
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
			if (isPluginSelected && !Base.aCHPlugInParameters.Any() && string.IsNullOrEmpty(pm.DirectDepositFileFormat))
			{
				if (string.IsNullOrEmpty(pm.APBatchExportPlugInTypeName))
				{
					var copy = (PaymentMethod)Base.PaymentMethod.Cache.CreateCopy(pm);
					copy.APBatchExportPlugInTypeName = ACHPlugInTypeAttribute.USACHPlugInType;
					Base.PaymentMethod.Update(copy);
				}

				if (!Base.IsACHPlugIn() || !string.IsNullOrEmpty(pm.DirectDepositFileFormat))
				{
					return;
				}

				if (!Base.IsCopyPasteContext)
				{
					bool settingsExists = CheckIfACHSettingsExists();
					if (!settingsExists)
					{
						AppendDefaultSettings();
						AppendDefaultPlugInParameters();
					}
					else
					{
						if (CheckAcumaticaExportScenariosMapping())
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

		private bool CheckIfACHSettingsExists() => Base.DetailsForCashAccount.Any() || Base.DetailsForVendor.Any();

		private bool CheckAcumaticaExportScenariosMapping()
		{
			var remSettings = new Dictionary<string, string>
			{
				{ "1","Beneficiary Account No:"},
				{ "2","Beneficiary Name:"},
				{ "3","Bank Routing Number (ABA):"},
				{ "4","Bank Name:"},
				{ "5","Company ID"},
				{ "6","Company ID Type"},
				{ "7","Offset ABA/Routing #"},
				{ "8","Offset Account #"},
				{ "9","Offset Description"},
			};

			var vendorSettings = new Dictionary<string, string>
			{
				{ "1","Beneficiary Account No:"},
				{ "2","Beneficiary Name:"},
				{ "3","Bank Routing Number (ABA):"},
				{ "4","Bank Name:"},
			};

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

		private void AppendDefaultSettings()
		{
			foreach (var settings in GetDefaultSettings(ACHPlugInBase.DefaultDetails.DetailsToAddByDefault))
			{
				if (settings.UseFor == PaymentMethodDetailUsage.UseForCashAccount)
				{
					Base.DetailsForCashAccount.Insert(settings.ToPaymentMethodDetail(Base.PaymentMethod.Current));
				}

				if (settings.UseFor == PaymentMethodDetailUsage.UseForVendor)
				{
					Base.DetailsForVendor.Insert(settings.ToPaymentMethodDetail(Base.PaymentMethod.Current));
				}
			}
		}

		private void AppendTransactionCodeSetting()
		{
			foreach (var settings in GetDefaultSettings(ACHPlugInBase.DefaultDetails.DetailsToAddTransactionCode))
			{
				if (settings.UseFor == PaymentMethodDetailUsage.UseForCashAccount)
				{
					Base.DetailsForCashAccount.Insert(settings.ToPaymentMethodDetail(Base.PaymentMethod.Current));
				}

				if (settings.UseFor == PaymentMethodDetailUsage.UseForVendor)
				{
					Base.DetailsForVendor.Insert(settings.ToPaymentMethodDetail(Base.PaymentMethod.Current));
				}
			}
		}

		private void UpdateDetailsAccordingToPlugIn()
		{
			foreach (PaymentMethodDetail detail in Base.DetailsForCashAccount.Select())
			{
				if (detail.DetailID == "5" && detail.Descr == "Company ID")
				{
					detail.ValidRegexp = "^([\\w]|\\s){0,10}$";
					Base.DetailsForCashAccount.Update(detail);
				}
				if (detail.DetailID == "6" && detail.Descr == "Company ID Type" && detail.IsRequired == true)
				{
					detail.IsRequired = false;
					Base.DetailsForCashAccount.Update(detail);
				}
			}

			AppendTransactionCodeSetting();
		}

		private void AppendOffsetSettings()
		{
			var newSettings = new Dictionary<DefaultPaymentMethodDetails, string>();

			foreach (var settings in GetDefaultSettings(ACHPlugInBase.DefaultDetails.DetailsToAddForOffset))
			{
				var ss = Base.DetailsForCashAccount.Insert(settings.ToPaymentMethodDetail(Base.PaymentMethod.Current));
				newSettings.Add((DefaultPaymentMethodDetails)settings.DetailIDInt, settings.DetailID);
			}

			foreach (ACHPlugInParameter item in Base.aCHPlugInParameters.View.QuickSelect())
			{
				if (item.ParameterID == nameof(IACHExportParameters.OffsetDFIAccountNbr).ToUpper())
				{
					item.Value = newSettings[DefaultPaymentMethodDetails.OffsetAccountNumber];
					Base.aCHPlugInParameters.Update(item);
				}

				if (item.ParameterID == nameof(IACHExportParameters.OffsetReceivingDEFIID).ToUpper())
				{
					item.Value = newSettings[DefaultPaymentMethodDetails.OffsetABARoutingNumber];
					Base.aCHPlugInParameters.Update(item);
				}

				if (item.ParameterID == nameof(IACHExportParameters.OffsetReceivingID).ToUpper())
				{
					item.Value = newSettings[DefaultPaymentMethodDetails.OffsetDescription];
					Base.aCHPlugInParameters.Update(item);
				}
			}
		}

		private IEnumerable<NewPaymentMethodDetail> GetDefaultSettings(DefaultPaymentMethodDetails[] details)
		{
			foreach (var id in details)
			{
				string mappingID;
				if (!DefaultPaymentMethodDetailsHelper.Dictionary.TryGetValue(id, out mappingID))
				{
					continue;
				}

				var descr = string.Empty;
				ACHPlugInBase.DefaultDetails.DefaultDetailDescription.TryGetValue(id, out descr);
				var useFor = string.Empty;
				ACHPlugInBase.DefaultDetails.DetailsUsedFor.TryGetValue(id, out useFor);
				var regExp = string.Empty;
				ACHPlugInBase.DefaultDetails.DefaultDetailValidationRegexp.TryGetValue(id, out regExp);
				var entryMask = string.Empty;
				ACHPlugInBase.DefaultDetails.DefaultDetailEntryMask.TryGetValue(id, out entryMask);
				var required = ACHPlugInBase.DefaultDetails.RequiredDetailsByDefault.Contains(id);
				var controlType = ACHPlugInBase.DefaultDetails.AccountTypeFields.Contains(id) ? PaymentMethodDetailType.AccountType : PaymentMethodDetailType.Text;
				var selected = ACHPlugInBase.DefaultDetails.NotSelectedFieldsByDefault.Contains(id);

				yield return new NewPaymentMethodDetail { DetailIDInt = (int)id, DetailID = mappingID, Description = descr, IsRequired = required, ControlType = (int)controlType, ValidRegexp = regExp, UseFor = useFor, EntryMask = entryMask };
			}
		}

		private bool CheckIfOffsetSettingsExists()
		{
			var offsetDescriptions = new string[] { ACHPlugInBase.Messages.OffsetAccountNumber,
													ACHPlugInBase.Messages.OffsetABARoutingNumber,
													ACHPlugInBase.Messages.OffsetDescription };

			// Acuminator disable once PX1015 IncorrectNumberOfSelectParameters [there is in(...) statement that cannot be recognized]
			return PXSelect<PaymentMethodDetail, Where<PaymentMethodDetail.paymentMethodID, Equal<Current<PaymentMethod.paymentMethodID>>,
				And<PaymentMethodDetail.descr, In<Required<PaymentMethodDetail.descr>>>>>.Select(Base, offsetDescriptions).Any();
		}

		#region ACHPlugInParameter
		protected virtual void _(Events.RowDeleting<ACHPlugInParameter> e)
		{
			e.Cancel = true;
		}

		protected virtual void _(Events.RowPersisting<ACHPlugInParameter> e)
		{
			if (Base.PaymentMethod.Current?.IsUsingPlugin != true)
			{
				return;
			}

			if (Base.PlugInParameters.Cache.Inserted.Any_())
			{
				Base.PlugInParameters.Cache.Clear();
			}

			bool offsetFieldsRequired = false;
			bool addendaTemplateRequired = false;

			if (Base.IsACHPlugIn())
			{
				GetAddendaAndOffsetRequirement(out offsetFieldsRequired, out addendaTemplateRequired);
			}

			var isValueEmpty = string.IsNullOrEmpty(e.Row?.Value);

			if (isValueEmpty)
			{
				if (e.Row.Required == true)
				{
					e.Cache.RaiseExceptionHandling<ACHPlugInParameter.value>(e.Row, e.Row?.Value, new PXSetPropertyException<ACHPlugInParameter.value>(CS.Messages.CannotBeEmpty));
				}

				if (Base.IsACHPlugIn())
				{
					ValidateACHSpecificFieldsOnRowPersisting(e.Cache, e.Row, offsetFieldsRequired, addendaTemplateRequired);
				}
			}
		}

		private void GetAddendaAndOffsetRequirement(out bool offsetFieldsRequired, out bool addendaTemplateRequired)
		{
			var includeOffsetRecordParam = Base.aCHPlugInParametersByParameter.SelectSingle(nameof(IACHExportParameters.IncludeOffsetRecord).ToUpper());
			bool.TryParse(includeOffsetRecordParam?.Value, out offsetFieldsRequired);

			var includeAddendaRecordsParam = Base.aCHPlugInParametersByParameter.SelectSingle(nameof(IACHExportParameters.IncludeAddendaRecords).ToUpper());
			bool.TryParse(includeAddendaRecordsParam?.Value, out addendaTemplateRequired);
		}

		private void ValidateACHSpecificFieldsOnRowPersisting(PXCache cache, ACHPlugInParameter row, bool offsetFieldsRequired, bool addendaTemplateRequired)
		{
			if (offsetFieldsRequired && (row.ParameterID == nameof(IACHExportParameters.OffsetDFIAccountNbr).ToUpper() ||
							row.ParameterID == nameof(IACHExportParameters.OffsetReceivingDEFIID).ToUpper() ||
							row.ParameterID == nameof(IACHExportParameters.OffsetReceivingID).ToUpper()))
			{
				cache.RaiseExceptionHandling<ACHPlugInParameter.value>(row, row?.Value, new PXSetPropertyException<ACHPlugInParameter.value>(CS.Messages.CannotBeEmpty));
			}

			if (addendaTemplateRequired && (row.ParameterID == nameof(IACHExportParameters.AddendaRecordTemplate).ToUpper()))
			{
				cache.RaiseExceptionHandling<ACHPlugInParameter.value>(row, row?.Value, new PXSetPropertyException<ACHPlugInParameter.value>(CS.Messages.CannotBeEmpty));
			}
		}
		protected virtual void _(Events.RowInserted<ACHPlugInParameter2> e)
		{
			var row = e.Row;
			var parameters = Base.GetParametersOfSelectedPlugIn().ToDictionary(m => m.ParameterID);

			if (parameters.ContainsKey(row?.ParameterID))
			{
				row.Description = parameters[row.ParameterID].Description;
				row.DetailMapping = parameters[row.ParameterID].DetailMapping;
				row.ExportScenarioMapping = parameters[row.ParameterID].ExportScenarioMapping;
				row.Type = parameters[row.ParameterID].Type;
				row.IsFormula = parameters[row.ParameterID].IsFormula;
				row.DataElementSize = parameters[row.ParameterID].DataElementSize;
			}

			var result = Base.aCHPlugInParameters.Insert((ACHPlugInParameter)row);
		}

		protected virtual void _(Events.FieldUpdated<ACHPlugInParameter, ACHPlugInParameter.value> e)
		{
			if (e.Row?.ParameterID.ToUpper() == nameof(IACHExportParameters.IncludeOffsetRecord).ToUpper())
			{
				var newValue = false;
				bool.TryParse(e.NewValue.ToString(), out newValue);

				if (newValue == true && !CheckIfOffsetSettingsExists())
				{
					var result = Base.PaymentMethod.Ask("Do you want to add remittance settings for the offset record?", MessageButtons.YesNo);

					if (result == WebDialogResult.Yes)
					{
						AppendOffsetSettings();
					}
				}

				Base.plugInFilter.Cache.SetValueExt<PlugInFilter.showOffsetSettings>(Base.plugInFilter.Current, newValue);
			}

			if (e.Row?.ParameterID.ToUpper() == nameof(IACHExportParameters.IncludeAddendaRecords).ToUpper())
			{
				Base.aCHPlugInParameters.View.RequestRefresh();
			}
		}

		protected virtual void _(Events.ExceptionHandling<ACHPlugInParameter.value> e)
		{
			if (e.Exception != null)
			{
				Base.plugInFilter.Cache.SetValueExt<PlugInFilter.showAllSettings>(Base.plugInFilter.Current, true);
			}
		}
		#endregion
	}
}
