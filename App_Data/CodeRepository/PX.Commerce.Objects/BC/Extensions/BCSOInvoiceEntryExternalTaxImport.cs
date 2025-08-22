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
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Objects.TX;
using PX.Common;
using PX.TaxProvider;
using PX.Objects.Common.Extensions;
using PX.Data.EP;
using PX.Data.BQL;
using PX.Objects.CM;
using PX.Commerce.Core;
using PX.Objects.CS;
using PX.Commerce.Objects;

namespace PX.Commerce.Objects
{
    public class BCSOInvoiceEntryExternalTaxImport : PXGraphExtension<ARInvoiceEntry>
	{
		public static bool IsActive() { return CommerceFeaturesHelper.CommerceEdition; }

        // a workaround for AC-278644
        protected void ARShippingAddress_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e, PXRowUpdated baseHandler)
        {
			bool isAPIScope = BCAPISyncScope.GetScoped() != null;
			if (!isAPIScope)
			{
                baseHandler?.Invoke(sender, e);
            }
        }

        protected void ARInvoice_TaxZoneID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e, PXFieldDefaulting baseHandler)
		{
			baseHandler?.Invoke(sender, e);

			if (e.NewValue == null)
			{
				BCAPISyncScope.BCSyncScopeContext context = BCAPISyncScope.GetScoped();
				if (context == null) return;

				BCBindingExt store = BCBindingExt.PK.Find(Base, context.Binding);
				if (store != null && store.TaxSynchronization == true)
					e.NewValue = store.DefaultTaxZoneID;
			}
		}

	}
}

