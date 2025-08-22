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
using PX.EFTPlugIn;
using PX.Objects.CA;
using System.Collections.Generic;

namespace PX.Objects.Localizations.CA
{
	public static class EFTPlugInSettingsMapping
	{
		public static Dictionary<string, string> Settings = new Dictionary<string, string>
		{
			{ nameof(IEFTExportParameters.CompressToZIPFormat).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IEFTExportParameters.OriginatorID).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IEFTExportParameters.DestinationDataCenter).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IEFTExportParameters.InstIDNumberBank).ToUpper(), nameof(ACHPlugInSettings.vendorSetting) },
			{ nameof(IEFTExportParameters.InstIDNumberBranch).ToUpper(), nameof(ACHPlugInSettings.vendorSetting) },
			{ nameof(IEFTExportParameters.InstIDNbrForReturnsBank).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IEFTExportParameters.InstIDNbrForReturnsBranch).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IEFTExportParameters.PayeeAccountNumber).ToUpper(), nameof(ACHPlugInSettings.vendorSetting) },
			{ nameof(IEFTExportParameters.OriginatorShortName).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IEFTExportParameters.PayeeName).ToUpper(), nameof(ACHPlugInSettings.vendorSetting) },
			{ nameof(IEFTExportParameters.OriginatorLongName).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IEFTExportParameters.OrigDirectClearerUserID).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IEFTExportParameters.AccountNumberForReturns).ToUpper(), nameof(ACHPlugInSettings.remittanceSetting) },
			{ nameof(IEFTExportParameters.CreationDate).ToUpper(), nameof(ACHPlugInSettings.creationDate) },
		};

		public static Dictionary<SelectorType?, string> SelectorTypes = new Dictionary<SelectorType?, string>
		{
			{ SelectorType.Checkbox, nameof(ACHPlugInSettings.checkBox) },
			{ SelectorType.RemittancePaymentMethodDetail, nameof(ACHPlugInSettings.remittanceSetting) },
			{ SelectorType.VendorPaymentMethodDetail, nameof(ACHPlugInSettings.vendorSetting) },
			{ SelectorType.Text, nameof(ACHPlugInSettings.companyDiscretionaryData) },
			{ SelectorType.CreationDate, nameof(ACHPlugInSettings.creationDate) },
		};
	}
}
