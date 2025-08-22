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
using System.Collections.Generic;

namespace PX.Objects.CA
{
	public static class ACHPlugInSettingsMapping
	{
		public static Dictionary<string, string> Settings = new Dictionary<string, string>
		{
			{ nameof(IACHExportParameters.CompanyDiscretionaryData).ToUpper(), nameof(ACHPlugInSettings.companyDiscretionaryData) },
			{ nameof(IACHExportParameters.CompanyEntryDescription).ToUpper(), nameof(ACHPlugInSettings.companyEntryDescription) },
			{ nameof(IACHExportParameters.CompanyIdentification).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IACHExportParameters.CompanyName).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IACHExportParameters.FileIDModifier).ToUpper(), nameof(ACHPlugInSettings.fileIDModifier) },
			{ nameof(IACHExportParameters.DFIAccountNbr).ToUpper(), nameof(ACHPlugInSettings.vendorSetting) },
			{ nameof(IACHExportParameters.ImmediateDestination).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IACHExportParameters.ImmediateDestinationName).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IACHExportParameters.ImmediateOrigin).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IACHExportParameters.ImmediateOriginName).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IACHExportParameters.IncludeAddendaRecords).ToUpper(), nameof(ACHPlugInSettings.checkBox) },
			{ nameof(IACHExportParameters.IncludeOffsetRecord).ToUpper(), nameof(ACHPlugInSettings.checkBox) },
			{ nameof(IACHExportParameters.OffsetDFIAccountNbr).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IACHExportParameters.OffsetReceivingDEFIID).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IACHExportParameters.OffsetReceivingID).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IACHExportParameters.OriginatingFDIID).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IACHExportParameters.OriginatorStatusCode).ToUpper(), nameof(ACHPlugInSettings.originatorStatusCode) },
			{ nameof(IACHExportParameters.PriorityCode).ToUpper(), nameof(ACHPlugInSettings.priorityCode) },
			{ nameof(IACHExportParameters.ReceivingDEFIID).ToUpper(), nameof(ACHPlugInSettings.vendorSetting) },
			{ nameof(IACHExportParameters.ReceivingID).ToUpper(), nameof(ACHPlugInSettings.vendorSetting) },
			{ nameof(IACHExportParameters.ServiceClassCode).ToUpper(), nameof(ACHPlugInSettings.serviceClassCode) },
			{ nameof(IACHExportParameters.StandardEntryClassCode).ToUpper(), nameof(ACHPlugInSettings.standardEntryClassCode) },
			{ nameof(IACHExportParameters.TransactionCode).ToUpper(), nameof(ACHPlugInSettings.vendorSetting) },
		};

		public static Dictionary<SelectorType?, string> SelectorTypes = new Dictionary<SelectorType?, string>
		{
			{ SelectorType.Checkbox, nameof(ACHPlugInSettings.checkBox) },
			{ SelectorType.FileIDModifier, nameof(ACHPlugInSettings.fileIDModifier) },
			{ SelectorType.RemittancePaymentMethodDetail, nameof(ACHPlugInSettings.remittanceSetting) },
			{ SelectorType.ServiceClassCode, nameof(ACHPlugInSettings.serviceClassCode) },
			{ SelectorType.StandardEntryCode, nameof(ACHPlugInSettings.standardEntryClassCode) },
			{ SelectorType.VendorPaymentMethodDetail, nameof(ACHPlugInSettings.vendorSetting) },
			{ SelectorType.TransactionCode, nameof(ACHPlugInSettings.vendorSetting) },
			{ SelectorType.Text, nameof(ACHPlugInSettings.companyDiscretionaryData) },
		};
	}
}
