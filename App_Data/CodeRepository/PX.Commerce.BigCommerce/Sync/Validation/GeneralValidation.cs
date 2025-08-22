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

using PX.Commerce.Core;
using PX.Data;
using PX.Objects.CS;

namespace PX.Commerce.BigCommerce
{
	public class GeneralValidator : BCBaseValidator, ISettingsValidator, IExternValidator
	{
		public int Priority { get { return int.MaxValue; } }

		/// <inheritdoc/>
		public virtual void Validate(IProcessor processor)
		{
			if (processor.Operation.ConnectorType != BigCommerceConstants.BigCommerceConnector) return;

			string connectorName = BigCommerceConstants.BigCommerceName;
			if (PXAccess.FeatureInstalled<FeaturesSet.subItem>() == true)
				throw new PXException(BCMessages.FeatureNotSupported, PXMessages.LocalizeNoPrefix(BCCaptions.InventorySubitems), connectorName);
			if (PXAccess.FeatureInstalled<FeaturesSet.financialStandard>() == false)
				throw new PXException(BCMessages.FeatureRequired, PXMessages.LocalizeNoPrefix(BCCaptions.StandardFinancials), connectorName);
			if (PXAccess.FeatureInstalled<FeaturesSet.accountLocations>() == false)
				throw new PXException(BCMessages.FeatureRequired, PXMessages.LocalizeNoPrefix(BCCaptions.BusinessAccountsLocation), connectorName);
			if (PXAccess.FeatureInstalled<FeaturesSet.distributionModule>() == false)
				throw new PXException(BCMessages.FeatureRequired, PXMessages.LocalizeNoPrefix(BCCaptions.Distribution), connectorName);
		}

		/// <inheritdoc/>
		public virtual void Validate(IProcessor processor, IExternEntity entity)
		{
			RunAttributesValidation(processor, entity);
		}
	}
}
