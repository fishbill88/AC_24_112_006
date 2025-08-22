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
using PX.Objects.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon
{
    public class BCAmazonSOShipmentEntryExt : PXGraphExtension<SOShipmentEntry>
    {
        public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.amazonIntegration>(); }

		public delegate void CorrectShipmentDelegate(SOOrderEntry docGraph, SOShipment shipOrder);

		[PXOverride]
		public virtual void CorrectShipment(SOOrderEntry docGraph, SOShipment shipOrder, CorrectShipmentDelegate baseMethod)
		{
			if (shipOrder == null || shipOrder.NoteID == null) baseMethod(docGraph, shipOrder);

			var hasExtendID = PXSelect<BCSyncStatus, Where<BCSyncStatus.connectorType, Equal<Required<BCSyncStatus.connectorType>>,
				And<BCSyncStatus.entityType, Equal<Required<BCSyncStatus.entityType>>,
				And<BCSyncStatus.localID, Equal<Required<BCSyncStatus.localID>>>>>>
				.Select(Base, BCAmazonConnector.TYPE, BCEntitiesAttribute.Shipment, shipOrder.NoteID)
				.Any(s => s.GetItem<BCSyncStatus>().ExternID != null);

			if (hasExtendID)
			{
				throw new PXException(PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.ExportedShipmentCannotModify, shipOrder.ShipmentNbr));
			}

			baseMethod(docGraph, shipOrder);
		}
	}
}
