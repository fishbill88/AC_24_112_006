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
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
using System;

namespace PX.Objects.PM
{
	public class SOShipmentEntryExt : PXGraphExtension<SOShipmentEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.projectAccounting>();
		}

		[PXOverride]
		public virtual void PostShipment(INRegisterEntryBase docgraph, PXResult<SOOrderShipment, SOOrder> sh, DocumentList<INRegister> list, ARInvoice invoice,
			Action<INRegisterEntryBase, PXResult<SOOrderShipment, SOOrder>, DocumentList<INRegister>, ARInvoice> baseMethod)
		{
			SOOrderShipment shiporder = (SOOrderShipment)sh;
			SOShipment shipment = Base.Document.Search<SOShipment.shipmentNbr>(shiporder.ShipmentNbr);
			if (shipment != null && shipment.ShipmentType == SOShipmentType.Transfer)
			{
				INTransferEntryExt transferExt = docgraph.GetExtension<INTransferEntryExt>();
				if (transferExt != null)
				{
					transferExt.IsShipmentPosting = true;

					try
					{
						baseMethod(docgraph, sh, list, invoice);
					}
					finally
					{
						transferExt.IsShipmentPosting = false;
					}
				}
			}
			else
			{
				INIssueEntryExt issueExt = docgraph.GetExtension<INIssueEntryExt>();
				if (issueExt != null)
				{
					issueExt.IsShipmentPosting = true;

					try
					{
						baseMethod(docgraph, sh, list, invoice);
					}
					finally
					{
						issueExt.IsShipmentPosting = false;
					}
				}
			}
		}
	}
}
